namespace Clarotech.openEHR.ADL2;

/// <summary>Abstract base for cADL attribute constraints (AOM C_ATTRIBUTE).</summary>
public abstract class CAttribute
{
    public required string              RmAttributeName { get; init; }
    public IntervalOfInt?               Existence       { get; init; }
    public IReadOnlyList<CObject>       Children        { get; init; } = [];

    public override string ToString()
    {
        return RmAttributeName.ToString();
    }
}
