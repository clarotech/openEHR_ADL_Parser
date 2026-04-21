using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests.Models;

/// <summary>
/// Tests for CDvUri constraints.
/// No fixture currently contains a DV_URI element.
/// Add an archetype with: value matches { DV_URI matches { ... } }
/// </summary>
public class CDvUriTests
{
    // TODO: add a CKM archetype containing DV_URI, register it in TestFixtures, then implement these tests
    [Fact(Skip = "No fixture contains DV_URI — add an archetype with a URI element to enable these tests")]
    public void Uri_Value_IsCDvUri() { }

    [Fact(Skip = "No fixture contains DV_URI — add an archetype with a URI element to enable these tests")]
    public void Uri_RmTypeName_IsDvUri() { }
}
