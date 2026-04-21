namespace Clarotech.openEHR.ADL2;

/// <summary>
/// Structured representation of an ADL 1.4 archetype identifier of the form
/// {rm_publisher}-{rm_package}-{rm_class}.{concept_name}.{version_id}
/// e.g. openEHR-EHR-OBSERVATION.blood_pressure.v2
/// </summary>
public sealed record ArchetypeId
{
    public string FullId      { get; init; } = "";
    public string RmPublisher { get; init; } = "";  // openEHR
    public string RmPackage   { get; init; } = "";  // EHR
    public string RmClass     { get; init; } = "";  // OBSERVATION
    public string ConceptName { get; init; } = "";  // blood_pressure
    public string VersionId   { get; init; } = "";  // v2

    public static ArchetypeId Parse(string raw)
    {
        // Split on first '.' to separate the RM type prefix from the rest
        var dotPos = raw.IndexOf('.');
        if (dotPos < 0)
            throw new FormatException($"Invalid archetype id: '{raw}'");

        var rmPart      = raw[..dotPos];          // openEHR-EHR-OBSERVATION
        var remainder   = raw[(dotPos + 1)..];    // blood_pressure.v2

        var rmParts = rmPart.Split('-');
        if (rmParts.Length != 3)
            throw new FormatException($"Invalid RM type prefix in archetype id: '{rmPart}'");

        var lastDot = remainder.LastIndexOf('.');
        if (lastDot < 0)
            throw new FormatException($"Missing version in archetype id: '{raw}'");

        return new ArchetypeId
        {
            FullId      = raw,
            RmPublisher = rmParts[0],
            RmPackage   = rmParts[1],
            RmClass     = rmParts[2],
            ConceptName = remainder[..lastDot],
            VersionId   = remainder[(lastDot + 1)..],
        };
    }

    public override string ToString() => FullId;
}
