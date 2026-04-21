namespace Clarotech.openEHR.ADL2;

/// <summary>Constraint on a DV_ORDINAL value (ADL 1.4 C_ORDINAL).</summary>
public sealed class CDvOrdinal : CObject
{
    public IReadOnlyList<OrdinalTerm> Items        { get; init; } = [];
    public int?                       AssumedValue { get; init; }
}
