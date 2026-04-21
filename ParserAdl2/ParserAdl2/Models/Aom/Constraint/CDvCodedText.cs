namespace Clarotech.openEHR.ADL2;

/// <summary>
/// Constraint on a DV_CODED_TEXT value.
/// <see cref="DefiningCode"/> is null when the coded text is unconstrained (matches {*}).
/// </summary>
public sealed class CDvCodedText : CObject
{
    public CCodePhrase? DefiningCode { get; init; }
}
