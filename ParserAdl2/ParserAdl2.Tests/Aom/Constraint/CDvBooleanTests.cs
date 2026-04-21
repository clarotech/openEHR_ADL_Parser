using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests.Models;

/// <summary>
/// Tests for CDvBoolean constraints.
/// No fixture currently contains a DV_BOOLEAN element.
/// Add an archetype with: value matches { DV_BOOLEAN matches { true_valid matches {true} } }
/// </summary>
public class CDvBooleanTests
{
    // TODO: add a CKM archetype containing DV_BOOLEAN (e.g. a yes/no clinical flag), register it in TestFixtures, then implement these tests
    [Fact(Skip = "No fixture contains DV_BOOLEAN — add an archetype with a boolean element to enable these tests")]
    public void Boolean_Value_IsCDvBoolean() { }

    [Fact(Skip = "No fixture contains DV_BOOLEAN — add an archetype with a boolean element to enable these tests")]
    public void Boolean_RmTypeName_IsDvBoolean() { }

    [Fact(Skip = "No fixture contains DV_BOOLEAN — add an archetype with a boolean element to enable these tests")]
    public void Boolean_TrueValid_IsTrue() { }

    [Fact(Skip = "No fixture contains DV_BOOLEAN — add an archetype with a boolean element to enable these tests")]
    public void Boolean_FalseValid_IsFalse() { }
}
