using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests.Models;

/// <summary>
/// Tests for CDvEhrUri constraints.
/// No fixture currently contains a DV_EHR_URI element.
/// Add an archetype with: value matches { DV_EHR_URI matches { ... } }
/// </summary>
public class CDvEhrUriTests
{
    // TODO: add a CKM archetype containing DV_EHR_URI (EHR-internal reference URI), register it in TestFixtures, then implement these tests
    [Fact(Skip = "No fixture contains DV_EHR_URI — add an archetype with an EHR URI element to enable these tests")]
    public void EhrUri_Value_IsCDvEhrUri() { }

    [Fact(Skip = "No fixture contains DV_EHR_URI — add an archetype with an EHR URI element to enable these tests")]
    public void EhrUri_RmTypeName_IsDvEhrUri() { }
}
