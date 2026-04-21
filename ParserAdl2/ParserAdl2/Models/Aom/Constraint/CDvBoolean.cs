namespace Clarotech.openEHR.ADL2;

/// <summary>Constraint on a DV_BOOLEAN value.</summary>
public sealed class CDvBoolean : CObject
{
    public bool  TrueValid    { get; init; }
    public bool  FalseValid   { get; init; }
    public bool? AssumedValue { get; init; }
}
