using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests.Models;

/// <summary>
/// Tests for CDvState constraints.
/// No fixture currently contains a DV_STATE element.
/// Add an archetype with a state machine constraint to enable these tests.
/// </summary>
public class CDvStateTests
{
    // TODO: add a CKM archetype containing DV_STATE (finite state machine constraint), register it in TestFixtures, then implement these tests
    [Fact(Skip = "No fixture contains DV_STATE — add an archetype with a state element to enable these tests")]
    public void State_Value_IsCDvState() { }

    [Fact(Skip = "No fixture contains DV_STATE — add an archetype with a state element to enable these tests")]
    public void State_RmTypeName_IsDvState() { }
}
