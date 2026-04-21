namespace Clarotech.openEHR.ADL2;

/// <summary>Constraint on a DV_TEXT value.</summary>
public sealed class CDvText : CObject
{
    /// <summary>Regex pattern the value must match, or null if unconstrained.</summary>
    public string? Pattern { get; init; }
    /// <summary>Explicit allowed string values; empty means unconstrained.</summary>
    public IReadOnlyList<string> List { get; init; } = [];
    public string? AssumedValue { get; init; }
}
