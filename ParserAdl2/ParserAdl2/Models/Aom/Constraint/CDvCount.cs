namespace Clarotech.openEHR.ADL2;

/// <summary>Constraint on a DV_COUNT value.</summary>
public sealed class CDvCount : CObject
{
    public IntervalOfInt? Magnitude    { get; init; }
    public int?           AssumedValue { get; init; }
}
