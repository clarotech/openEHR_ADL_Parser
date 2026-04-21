namespace Clarotech.openEHR.ADL2;

/// <summary>
/// One allowed unit option within a C_DV_QUANTITY constraint (AOM C_QUANTITY_ITEM).
/// </summary>
public sealed class CQuantityItem
{
    /// <summary>Units string, e.g. "mm[Hg]", "deg", "kg".</summary>
    public required string Units { get; init; }
    /// <summary>Allowed magnitude range, e.g. |0.0..<1000.0|. Null means unconstrained.</summary>
    public IntervalOfReal? Magnitude { get; init; }
    /// <summary>Allowed precision range (decimal places), e.g. |0|. Null means unconstrained.</summary>
    public IntervalOfInt? Precision { get; init; }
}
