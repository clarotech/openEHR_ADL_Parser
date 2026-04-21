namespace Clarotech.openEHR.ADL2;

/// <summary>Constraint on a DV_DURATION value.</summary>
public sealed class CDvDuration : CObject
{
    /// <summary>
    /// ISO 8601 duration constraint pattern (e.g. "PYMD"), or null when absent.
    /// </summary>
    public string? Pattern      { get; init; }
    /// <summary>
    /// Raw interval or value expression from the <c>value matches {...}</c> clause
    /// (e.g. "PT24H" or "|>=PT0S|"), or null when absent.
    /// </summary>
    public string? Range        { get; init; }
    public string? AssumedValue { get; init; }
}
