using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests.Models;

/// <summary>
/// Tests for CDvIdentifier constraints.
/// No fixture currently contains a DV_IDENTIFIER element.
/// Add an archetype with: value matches { DV_IDENTIFIER matches { ... } }
/// </summary>
public class CDvIdentifierTests
{
    // TODO: add a CKM archetype containing DV_IDENTIFIER (issuer/assigner/id/type), register it in TestFixtures, then implement these tests
    [Fact(Skip = "No fixture contains DV_IDENTIFIER — add an archetype with an identifier element to enable these tests")]
    public void Identifier_Value_IsCDvIdentifier() { }

    [Fact(Skip = "No fixture contains DV_IDENTIFIER — add an archetype with an identifier element to enable these tests")]
    public void Identifier_RmTypeName_IsDvIdentifier() { }
}
