using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests.Models;

/// <summary>
/// Tests for CDvMultimedia constraints.
/// No fixture currently contains a DV_MULTIMEDIA element.
/// Add an archetype with: value matches { DV_MULTIMEDIA matches { ... } }
/// </summary>
public class CDvMultimediaTests
{
    // TODO: add a CKM archetype containing DV_MULTIMEDIA (media type / URI / data), register it in TestFixtures, then implement these tests
    [Fact(Skip = "No fixture contains DV_MULTIMEDIA — add an archetype with a multimedia element to enable these tests")]
    public void Multimedia_Value_IsCDvMultimedia() { }

    [Fact(Skip = "No fixture contains DV_MULTIMEDIA — add an archetype with a multimedia element to enable these tests")]
    public void Multimedia_RmTypeName_IsDvMultimedia() { }
}
