namespace Clarotech.openEHR.ADL2;

/// <summary>An integer interval, e.g. for occurrences, existence or cardinality.</summary>
/// <param name="Lower">Inclusive lower bound.</param>
/// <param name="Upper">Inclusive upper bound; null means unbounded (*).</param>
public sealed record IntervalOfInt(int Lower, int? Upper)
{
    public override string ToString() =>
        Upper.HasValue ? $"{Lower}..{Upper}" : $"{Lower}..*";
}
