using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests.Models;

/// <summary>
/// Tests for CDvParsable constraints.
/// No fixture currently contains a DV_PARSABLE element.
/// Add an archetype with: value matches { DV_PARSABLE matches { ... } }
/// </summary>
public class CDvParsableTests
{
    // TODO: add a CKM archetype containing DV_PARSABLE (value + formalism), register it in TestFixtures, then implement these tests
    [Fact(Skip = "No fixture contains DV_PARSABLE — add an archetype with a parsable element to enable these tests")]
    public void Parsable_Value_IsCDvParsable() { }

    [Fact(Skip = "No fixture contains DV_PARSABLE — add an archetype with a parsable element to enable these tests")]
    public void Parsable_RmTypeName_IsDvParsable() { }
}
