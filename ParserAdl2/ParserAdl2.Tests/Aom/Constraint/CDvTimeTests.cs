using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests.Models;

/// <summary>
/// Tests for CDvTime constraints.
/// No fixture currently contains a DV_TIME element.
/// Add an archetype with: value matches { DV_TIME matches {*} }
/// </summary>
public class CDvTimeTests
{
    // TODO: add a CKM archetype containing DV_TIME, register it in TestFixtures, then implement these tests
    [Fact(Skip = "No fixture contains DV_TIME — add an archetype with a time element to enable these tests")]
    public void Time_Value_IsCDvTime() { }

    [Fact(Skip = "No fixture contains DV_TIME — add an archetype with a time element to enable these tests")]
    public void Time_RmTypeName_IsDvTime() { }

    [Fact(Skip = "No fixture contains DV_TIME — add an archetype with a time element to enable these tests")]
    public void Time_Unconstrained_PatternIsNull() { }
}
