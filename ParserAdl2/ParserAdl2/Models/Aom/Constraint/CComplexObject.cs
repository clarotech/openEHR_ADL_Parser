namespace Clarotech.openEHR.ADL2;

/// <summary>Complex object constraint — the main structural node in cADL (AOM C_COMPLEX_OBJECT).</summary>
public sealed class CComplexObject : CObject
{
    public IReadOnlyList<CAttribute> Attributes { get; init; } = [];

    /// <summary>Find a direct child attribute by name.</summary>
    public CAttribute? GetAttribute(string name) =>
        Attributes.FirstOrDefault(a => a.RmAttributeName == name);
}
