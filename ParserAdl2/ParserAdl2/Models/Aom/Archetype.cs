namespace Clarotech.openEHR.ADL2;

/// <summary>
/// Top-level AOM representation of an authored archetype.
/// Terminology and definition sections will be added as modelling progresses.
/// </summary>
public sealed class Archetype
{
    public required ArchetypeId       Id       { get; init; }
    public required ArchetypeMetaData MetaData { get; init; }

    /// <summary>The ADL 1.4 concept code, e.g. "at0000".</summary>
    public required string ConceptCode { get; init; }

    public required ArchetypeLanguage    Language     { get; init; }
    public required ArchetypeDescription Description  { get; init; }
    public required ArchetypeTerminology Terminology  { get; init; }
    public required CComplexObject       Definition   { get; init; }
}
