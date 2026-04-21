using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests.Models;

/// <summary>
/// Tests for CDvParagraph constraints.
/// No fixture currently contains a DV_PARAGRAPH element.
/// Add an archetype with: value matches { DV_PARAGRAPH matches { ... } }
/// </summary>
public class CDvParagraphTests
{
    // TODO: add a CKM archetype containing DV_PARAGRAPH, register it in TestFixtures, then implement these tests
    [Fact(Skip = "No fixture contains DV_PARAGRAPH — add an archetype with a paragraph element to enable these tests")]
    public void Paragraph_Value_IsCDvParagraph() { }

    [Fact(Skip = "No fixture contains DV_PARAGRAPH — add an archetype with a paragraph element to enable these tests")]
    public void Paragraph_RmTypeName_IsDvParagraph() { }
}
