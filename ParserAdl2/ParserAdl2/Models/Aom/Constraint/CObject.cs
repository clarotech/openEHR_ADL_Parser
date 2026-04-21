namespace Clarotech.openEHR.ADL2;

/// <summary>Abstract base for all cADL constraint objects (AOM C_OBJECT).</summary>
public abstract class CObject
{
    public required string RmTypeName  { get; init; }
    /// <summary>Node id, e.g. "at0000", or empty string if absent.</summary>
    public required string NodeId      { get; init; }
    public IntervalOfInt?  Occurrences { get; init; }
}
