namespace Clarotech.openEHR.ADL2;

/// <summary>Container attribute constraint with cardinality (AOM C_MULTIPLE_ATTRIBUTE).</summary>
public sealed class CMultipleAttribute : CAttribute
{
    public Cardinality? Cardinality { get; init; }
}
