namespace Clarotech.openEHR.ADL2;

/// <summary>A real-number interval, e.g. for magnitude constraints.</summary>
/// <param name="Lower">Inclusive lower bound (or exclusive when <see cref="LowerUnbounded"/> is true).</param>
/// <param name="Upper">Inclusive upper bound; null means unbounded.</param>
/// <param name="LowerUnbounded">True when lower bound is exclusive (uses &lt; syntax).</param>
/// <param name="UpperUnbounded">True when upper bound is exclusive (uses &lt; syntax).</param>
public sealed record IntervalOfReal(
    double? Lower,
    double? Upper,
    bool LowerUnbounded = false,
    bool UpperUnbounded = false)
{
    public override string ToString() =>
        $"|{(LowerUnbounded ? "<" : "")}{Lower}..{(UpperUnbounded ? "<" : "")}{Upper}|";
}
