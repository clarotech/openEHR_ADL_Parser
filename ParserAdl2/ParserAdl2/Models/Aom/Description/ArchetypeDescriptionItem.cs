namespace Clarotech.openEHR.ADL2;

/// <summary>
/// Per-language description block inside the 'details' map of an archetype
/// description section, keyed by ISO 639-1 language code.
/// </summary>
public sealed class ArchetypeDescriptionItem
{
    public required TerminologyCode Language { get; init; }
    public required string Purpose  { get; init; }
    public string? Use      { get; init; }
    public string? Misuse   { get; init; }
    public string? Copyright { get; init; }

    public IReadOnlyList<string> Keywords { get; init; } = [];

    /// <summary>Any detail keys not given their own property above.</summary>
    public IReadOnlyDictionary<string, string> OtherDetails { get; init; }
        = new Dictionary<string, string>();
}
