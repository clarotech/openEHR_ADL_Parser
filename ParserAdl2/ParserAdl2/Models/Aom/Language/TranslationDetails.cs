namespace Clarotech.openEHR.ADL2;

/// <summary>
/// Details for a single translated language entry in the language section,
/// corresponding to one item in the translations keyed map.
/// </summary>
public sealed class TranslationDetails
{
    public required TerminologyCode Language { get; init; }

    /// <summary>Free-text key/value pairs describing the translator.</summary>
    public IReadOnlyDictionary<string, string> Author { get; init; }
        = new Dictionary<string, string>();

    public string? AuthorName         => Author.GetValueOrDefault("name");
    public string? AuthorOrganisation => Author.GetValueOrDefault("organisation");
    public string? AuthorEmail        => Author.GetValueOrDefault("email");

    public string? Accreditation { get; init; }

    public IReadOnlyDictionary<string, string> OtherDetails { get; init; }
        = new Dictionary<string, string>();
}
