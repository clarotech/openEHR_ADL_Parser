using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests.Models;

/// <summary>
/// Tests for CDvProportion constraints.
/// No fixture currently contains a DV_PROPORTION element.
/// Add an archetype with: value matches { DV_PROPORTION matches { ... } }
/// </summary>
public class CDvProportionTests
{
    // TODO: add a CKM archetype containing DV_PROPORTION (e.g. a percentage or ratio element), register it in TestFixtures, then implement these tests
    [Fact(Skip = "No fixture contains DV_PROPORTION — add an archetype with a proportion element to enable these tests")]
    public void Proportion_Value_IsCDvProportion() { }

    [Fact(Skip = "No fixture contains DV_PROPORTION — add an archetype with a proportion element to enable these tests")]
    public void Proportion_RmTypeName_IsDvProportion() { }
}
