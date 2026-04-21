namespace Clarotech.openEHR.ADL2;

/// <summary>
/// The parsed terminology (ontology) section of an ADL 1.4 archetype.
/// Corresponds to the AOM ARCHETYPE_TERMINOLOGY class.
/// </summary>
public sealed class ArchetypeTerminology
{
    /// <summary>External terminologies referenced in term_bindings.</summary>
    public IReadOnlyList<string> TerminologiesAvailable { get; init; } = [];

    /// <summary>
    /// Term definitions keyed first by ISO 639-1 language code, then by at-code.
    /// e.g. TermDefinitions["en"]["at0000"].Text
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, TermDefinition>> TermDefinitions
        { get; init; } = new Dictionary<string, IReadOnlyDictionary<string, TermDefinition>>();

    /// <summary>
    /// Term bindings keyed first by terminology name, then by at-code.
    /// e.g. TermBindings["SNOMED-CT"]["at0000"].Target
    /// </summary>
    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, TermBinding>> TermBindings
        { get; init; } = new Dictionary<string, IReadOnlyDictionary<string, TermBinding>>();

    /// <summary>
    /// Returns the term definition for the given code and language (defaults to English).
    /// Returns null when either the language or the code is absent.
    /// </summary>
    public TermDefinition? GetTermDefinition(string code, string language = "en") =>
        TermDefinitions.TryGetValue(language, out var lang) &&
        lang.TryGetValue(code, out var def) ? def : null;
}
