namespace Clarotech.openEHR.ADL2;

/// <summary>
/// A single term definition entry within the terminology section,
/// keyed by archetype local at-code (e.g. "at0000").
/// </summary>
public sealed class TermDefinition
{
    public required string  Code        { get; init; }
    public required string  Text        { get; init; }
    public required string  Description { get; init; }
    public          string? Comment     { get; init; }
}
