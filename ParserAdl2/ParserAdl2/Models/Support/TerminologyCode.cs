namespace Clarotech.openEHR.ADL2;

/// <summary>
/// A code from a named terminology, e.g. [ISO_639-1::en].
/// </summary>
public sealed record TerminologyCode(string TerminologyId, string CodeString)
{
    /// <summary>Parses "[ISO_639-1::en]" or "ISO_639-1::en".</summary>
    public static TerminologyCode Parse(string raw)
    {
        var s = raw.Trim('[', ']');
        var sep = s.IndexOf("::", StringComparison.Ordinal);
        if (sep < 0)
            throw new FormatException($"Not a valid terminology code: '{raw}'");
        return new TerminologyCode(s[..sep], s[(sep + 2)..]);
    }

    public override string ToString() => $"[{TerminologyId}::{CodeString}]";
}
