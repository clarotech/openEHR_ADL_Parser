using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests.Models;

/// <summary>
/// Tests for CDvScale constraints.
/// No fixture currently contains a DV_SCALE element.
/// Add an archetype with: value matches { DV_SCALE matches { ... } }
/// </summary>
public class CDvScaleTests
{
    // TODO: add a CKM archetype containing DV_SCALE (real-valued ordinal scale), register it in TestFixtures, then implement these tests
    [Fact(Skip = "No fixture contains DV_SCALE — add an archetype with a scale element to enable these tests")]
    public void Scale_Value_IsCDvScale() { }

    [Fact(Skip = "No fixture contains DV_SCALE — add an archetype with a scale element to enable these tests")]
    public void Scale_RmTypeName_IsDvScale() { }
}
