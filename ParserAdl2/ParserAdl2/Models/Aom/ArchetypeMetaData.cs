namespace Clarotech.openEHR.ADL2;

/// <summary>
/// Key-value pairs from the archetype header parenthesis block,
/// e.g. (adl_version=1.4; uid=1811b084-...).
/// </summary>
public sealed class ArchetypeMetaData
{
    public string? AdlVersion { get; init; }
    public string? Uid        { get; init; }

    /// <summary>Any header items not given their own property above.</summary>
    public IReadOnlyDictionary<string, string> OtherItems { get; init; }
        = new Dictionary<string, string>();

    /// <summary>Flag items (no value) present in the header.</summary>
    public IReadOnlySet<string> Flags { get; init; }
        = new HashSet<string>();
}
