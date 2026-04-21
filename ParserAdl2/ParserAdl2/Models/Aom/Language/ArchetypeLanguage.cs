namespace Clarotech.openEHR.ADL2;

/// <summary>
/// The parsed language section of an archetype, corresponding to the
/// 'language' block in ADL 1.4.
/// </summary>
public sealed class ArchetypeLanguage
{
    public required TerminologyCode OriginalLanguage { get; init; }

    /// <summary>Keyed by the ISO 639-1 language code, e.g. "de", "fr".</summary>
    public IReadOnlyDictionary<string, TranslationDetails> Translations { get; init; }
        = new Dictionary<string, TranslationDetails>();
}
