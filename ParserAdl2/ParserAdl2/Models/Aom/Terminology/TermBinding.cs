namespace Clarotech.openEHR.ADL2;

/// <summary>
/// Maps a local at-code to an external terminology code,
/// e.g. at0000 → [SNOMED-CT(2003)::364090009].
/// </summary>
public sealed class TermBinding
{
    public required string          Code   { get; init; }
    public required TerminologyCode Target { get; init; }
}
