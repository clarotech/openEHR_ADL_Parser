namespace Clarotech.openEHR.ADL2;

/// <summary>
/// The parsed description section of an archetype, corresponding to the
/// 'description' block in ADL 1.4.
/// </summary>
public sealed class ArchetypeDescription
{
    // ── original_author ───────────────────────────────────────────────────────

    public IReadOnlyDictionary<string, string> OriginalAuthor { get; init; }
        = new Dictionary<string, string>();

    public string? AuthorName         => OriginalAuthor.GetValueOrDefault("name");
    public string? AuthorOrganisation => OriginalAuthor.GetValueOrDefault("organisation");
    public string? AuthorEmail        => OriginalAuthor.GetValueOrDefault("email");
    public string? AuthorDate         => OriginalAuthor.GetValueOrDefault("date");

    // ── details ───────────────────────────────────────────────────────────────

    /// <summary>Per-language description items, keyed by ISO 639-1 language code.</summary>
    public IReadOnlyDictionary<string, ArchetypeDescriptionItem> Details { get; init; }
        = new Dictionary<string, ArchetypeDescriptionItem>();

    // ── top-level scalars / lists ─────────────────────────────────────────────

    public required string LifecycleState { get; init; }

    public IReadOnlyList<string> OtherContributors { get; init; } = [];

    // ── other_details ─────────────────────────────────────────────────────────

    public IReadOnlyDictionary<string, string> OtherDetails { get; init; }
        = new Dictionary<string, string>();

    /// <summary>Convenience helpers for the well-known other_details keys.</summary>
    public string? Licence               => OtherDetails.GetValueOrDefault("licence");
    public string? CustodianOrganisation => OtherDetails.GetValueOrDefault("custodian_organisation");
    public string? OriginalNamespace     => OtherDetails.GetValueOrDefault("original_namespace");
    public string? OriginalPublisher     => OtherDetails.GetValueOrDefault("original_publisher");
    public string? CustodianNamespace    => OtherDetails.GetValueOrDefault("custodian_namespace");
    public string? Revision              => OtherDetails.GetValueOrDefault("revision");
    public string? BuildUid              => OtherDetails.GetValueOrDefault("build_uid");
}
