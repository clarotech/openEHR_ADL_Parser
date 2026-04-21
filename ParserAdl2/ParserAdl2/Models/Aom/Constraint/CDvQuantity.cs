namespace Clarotech.openEHR.ADL2;

/// <summary>Constraint on a DV_QUANTITY value (openEHR profile C_DV_QUANTITY).</summary>
public sealed class CDvQuantity : CObject
{
    /// <summary>
    /// Raw ODIN text inside the C_DV_QUANTITY angle-bracket block, preserved for diagnostics.
    /// </summary>
    public required string RawOdin { get; init; }

    /// <summary>
    /// The openEHR property code, e.g. <c>[openehr::125]</c> for pressure.
    /// Null when not specified in the archetype.
    /// </summary>
    public TerminologyCode? Property { get; init; }

    /// <summary>Ordered list of allowed unit options.</summary>
    public IReadOnlyList<CQuantityItem> List { get; init; } = [];

    /// <summary>Assumed/default value units when the attribute is optional.</summary>
    public CQuantityItem? AssumedValue { get; init; }
}
