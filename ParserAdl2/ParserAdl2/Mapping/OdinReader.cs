using System.Runtime.CompilerServices;

namespace Clarotech.openEHR.ADL2;

/// <summary>
/// Static helpers for extracting typed values from an ODIN parse tree.
/// All methods are null-safe: a null input returns an appropriate empty result.
/// </summary>
internal static class OdinReader
{
    // Caches attribute-name → value-block per parse-tree node to avoid O(n)
    // linear scans on every Attr() call. ConditionalWeakTable uses weak keys
    // so cached entries are collected with their parse tree.
    private static readonly ConditionalWeakTable<OdinParser.OdinObjectContext,
        Dictionary<string, OdinParser.OdinObjectValueBlockContext>> _objectCache = new();

    private static readonly ConditionalWeakTable<OdinParser.OdinObjectValueBlockContext,
        Dictionary<string, OdinParser.OdinObjectValueBlockContext>> _blockCache = new();

    private static Dictionary<string, OdinParser.OdinObjectValueBlockContext> BuildAttrDict(
        IEnumerable<OdinParser.OdinAttrValContext> attrs) =>
        attrs.Where(a => a.odinObjectBlock()?.odinObjectValueBlock() != null)
             .ToDictionary(
                 a => a.odinAttrName().GetText(),
                 a => a.odinObjectBlock().odinObjectValueBlock());

    // ── attribute lookup ────────────────────────────────────────────────────

    /// <summary>Returns the value block for a named attribute on a top-level odinObject.</summary>
    public static OdinParser.OdinObjectValueBlockContext? Attr(
        OdinParser.OdinObjectContext? ctx, string name)
    {
        if (ctx == null) return null;
        var dict = _objectCache.GetValue(ctx, static c => BuildAttrDict(c.odinAttrVal()));
        return dict.TryGetValue(name, out var val) ? val : null;
    }

    /// <summary>Returns the value block for a named attribute nested inside another value block.</summary>
    public static OdinParser.OdinObjectValueBlockContext? Attr(
        OdinParser.OdinObjectValueBlockContext? ctx, string name)
    {
        if (ctx == null) return null;
        var dict = _blockCache.GetValue(ctx, static c => BuildAttrDict(c.odinAttrVal()));
        return dict.TryGetValue(name, out var val) ? val : null;
    }

    // ── keyed objects (dictionary-style blocks) ─────────────────────────────

    /// <summary>
    /// Returns the string-keyed sub-blocks from a value block containing
    /// odinKeyedObject entries, e.g. ["en"] = &lt;...&gt;.
    /// </summary>
    public static IReadOnlyDictionary<string, OdinParser.OdinObjectValueBlockContext> KeyedBlocks(
        OdinParser.OdinObjectValueBlockContext? ctx)
    {
        if (ctx == null) return new Dictionary<string, OdinParser.OdinObjectValueBlockContext>();

        return ctx.odinKeyedObject()
                  .ToDictionary(
                      ko => KeyString(ko.odinKeySpec()),
                      ko => ko.odinObjectBlock().odinObjectValueBlock());
    }

    // ── scalar extractors ───────────────────────────────────────────────────

    /// <summary>Extracts a single string value, stripping surrounding quotes.</summary>
    public static string? String(OdinParser.OdinObjectValueBlockContext? ctx)
    {
        var sv = ctx?.primitiveObject()?.primitiveValue()?.stringValue();
        return sv != null ? StripQuotes(sv.STRING().GetText()) : null;
    }

    /// <summary>Extracts a string list; also handles the single-item case.</summary>
    public static IReadOnlyList<string> StringList(OdinParser.OdinObjectValueBlockContext? ctx)
    {
        if (ctx == null) return [];

        var pobj = ctx.primitiveObject();
        if (pobj == null) return [];

        // multi-value list: "a", "b", "c"
        var slv = pobj.primitiveListValue()?.stringListValue();
        if (slv != null)
            return slv.stringValue()
                      .Select(sv => StripQuotes(sv.STRING().GetText()))
                      .ToArray();

        // single value falling back to a one-element list
        var sv2 = pobj.primitiveValue()?.stringValue();
        if (sv2 != null)
            return [StripQuotes(sv2.STRING().GetText())];

        return [];
    }

    /// <summary>Extracts a terminology code from a value block, e.g. [ISO_639-1::en].</summary>
    public static TerminologyCode? TermCode(OdinParser.OdinObjectValueBlockContext? ctx)
    {
        var tv = ctx?.primitiveObject()?.primitiveValue()?.termCodeValue();
        return tv != null ? TerminologyCode.Parse(tv.GetText()) : null;
    }

    /// <summary>
    /// Extracts a keyed string map where values are scalar strings,
    /// e.g. ["name"] = &lt;"Sam Heard"&gt;, ["organisation"] = &lt;"Ocean Informatics"&gt;.
    /// </summary>
    public static IReadOnlyDictionary<string, string> StringMap(
        OdinParser.OdinObjectValueBlockContext? ctx)
    {
        if (ctx == null) return new Dictionary<string, string>();

        return ctx.odinKeyedObject()
                  .Select(ko => (
                      key:   KeyString(ko.odinKeySpec()),
                      value: String(ko.odinObjectBlock().odinObjectValueBlock())))
                  .Where(p => p.value != null)
                  .ToDictionary(p => p.key, p => p.value!);
    }

    // ── private helpers ─────────────────────────────────────────────────────

    private static string KeyString(OdinParser.OdinKeySpecContext keySpec)
    {
        var pv = keySpec.primitiveValue();
        var sv = pv.stringValue();
        return sv != null ? StripQuotes(sv.STRING().GetText()) : pv.GetText();
    }

    private static string StripQuotes(string s) => s.Trim('"');
}
