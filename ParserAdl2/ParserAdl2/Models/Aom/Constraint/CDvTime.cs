namespace Clarotech.openEHR.ADL2;

/// <summary>Constraint on a DV_TIME value.</summary>
public sealed class CDvTime : CObject
{
    /// <summary>ISO 8601 time constraint pattern.</summary>
    public string? Pattern      { get; init; }
    public string? AssumedValue { get; init; }
}
