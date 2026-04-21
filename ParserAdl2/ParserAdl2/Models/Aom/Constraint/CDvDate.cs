namespace Clarotech.openEHR.ADL2;

/// <summary>Constraint on a DV_DATE value.</summary>
public sealed class CDvDate : CObject
{
    /// <summary>ISO 8601 date constraint pattern (e.g. "yyyy-MM-dd" or "yyyy-??-XX").</summary>
    public string? Pattern      { get; init; }
    public string? AssumedValue { get; init; }
}
