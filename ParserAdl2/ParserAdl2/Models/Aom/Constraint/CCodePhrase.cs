namespace Clarotech.openEHR.ADL2;

/// <summary>
/// Constraint on a CODE_PHRASE / DV_CODED_TEXT.defining_code (AOM C_CODE_PHRASE / openEHR C_CODE_PHRASE).
/// Covers: local inline code lists [local:: at0047, at0048], value-set ac-codes [ac0001],
/// and single at-codes [at0047].
/// </summary>
public sealed class CCodePhrase
{
    /// <summary>
    /// Terminology identifier (e.g. "local", "openehr", "SNOMED-CT"), or null for unconstrained.
    /// </summary>
    public string?              TerminologyId { get; init; }
    /// <summary>Explicit allowed codes (local at-codes or external codes).</summary>
    public IReadOnlyList<string> CodeList     { get; init; } = [];
    /// <summary>Value-set ac-code when this is an indirect constraint.</summary>
    public string?              ValueSetCode  { get; init; }
    /// <summary>Default/assumed code (after ';' in the constraint).</summary>
    public string?              AssumedValue  { get; init; }
}
