namespace Clarotech.openEHR.ADL2;

/// <summary>Archetype slot constraint — allow_archetype (AOM ARCHETYPE_SLOT).</summary>
public sealed class ArchetypeSlot : CObject
{
    /// <summary>Include regex patterns from archetypeIdConstraint rules.</summary>
    public IReadOnlyList<string> Includes { get; init; } = [];
    /// <summary>Exclude regex patterns.</summary>
    public IReadOnlyList<string> Excludes { get; init; } = [];
}
