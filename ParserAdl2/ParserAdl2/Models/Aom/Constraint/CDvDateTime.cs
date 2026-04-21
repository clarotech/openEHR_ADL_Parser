namespace Clarotech.openEHR.ADL2;

/// <summary>Constraint on a DV_DATE_TIME value.</summary>
public sealed class CDvDateTime : CObject
{
    /// <summary>ISO 8601 date-time constraint pattern.</summary>
    public string? Pattern      { get; init; }
    public string? AssumedValue { get; init; }
}
