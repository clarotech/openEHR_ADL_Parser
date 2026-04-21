namespace Clarotech.openEHR.ADL2;

/// <summary>Cardinality constraint on a container attribute (AOM CARDINALITY).</summary>
public sealed record Cardinality(bool IsOrdered, bool IsUnique, IntervalOfInt Interval)
{
    public override string ToString() =>
        $"{Interval}; {(IsOrdered ? "ordered" : "unordered")}{(IsUnique ? "; unique" : "")}";
}
