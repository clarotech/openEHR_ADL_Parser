namespace Clarotech.openEHR.ADL2;

/// <summary>Internal reference (use_node) to another node path in the same archetype.</summary>
public sealed class ArchetypeInternalRef : CObject
{
    public required string TargetPath { get; init; }
}
