using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests;

/// <summary>
/// Integration tests that parse the embedded blood pressure ADL fixture
/// and assert the full AOM model is populated correctly.
/// </summary>
public class ArchetypeParserTests
{
    private static readonly Archetype BloodPressure = TestFixtures.BloodPressure;

    // ── identity ──────────────────────────────────────────────────────────────

    [Fact]
    public void ArchetypeId_FullId_IsCorrect() =>
        Assert.Equal("openEHR-EHR-OBSERVATION.blood_pressure.v2", BloodPressure.Id.FullId);

    [Fact]
    public void ArchetypeId_Components_AreCorrect()
    {
        Assert.Equal("openEHR",        BloodPressure.Id.RmPublisher);
        Assert.Equal("EHR",            BloodPressure.Id.RmPackage);
        Assert.Equal("OBSERVATION",    BloodPressure.Id.RmClass);
        Assert.Equal("blood_pressure", BloodPressure.Id.ConceptName);
        Assert.Equal("v2",             BloodPressure.Id.VersionId);
    }

    // ── metadata ──────────────────────────────────────────────────────────────

    [Fact]
    public void MetaData_AdlVersion_Is1_4() =>
        Assert.Equal("1.4", BloodPressure.MetaData.AdlVersion);

    [Fact]
    public void MetaData_Uid_IsPresent() =>
        Assert.False(string.IsNullOrEmpty(BloodPressure.MetaData.Uid));

    // ── concept ───────────────────────────────────────────────────────────────

    [Fact]
    public void ConceptCode_IsAt0000() =>
        Assert.Equal("at0000", BloodPressure.ConceptCode);

    // ── language section ──────────────────────────────────────────────────────

    [Fact]
    public void Language_OriginalLanguage_IsEnglish() =>
        Assert.Equal("[ISO_639-1::en]", BloodPressure.Language.OriginalLanguage.ToString());

    [Fact]
    public void Language_Translations_ContainGerman()
    {
        Assert.True(BloodPressure.Language.Translations.ContainsKey("de"));
        var de = BloodPressure.Language.Translations["de"];
        Assert.Equal("[ISO_639-1::de]", de.Language.ToString());
    }

    [Fact]
    public void Language_GermanTranslation_AuthorName_IsPopulated()
    {
        var de = BloodPressure.Language.Translations["de"];
        Assert.False(string.IsNullOrWhiteSpace(de.AuthorName));
    }

    [Fact]
    public void Language_GermanTranslation_AuthorOrganisation_IsPopulated()
    {
        var de = BloodPressure.Language.Translations["de"];
        Assert.False(string.IsNullOrWhiteSpace(de.AuthorOrganisation));
    }

    [Fact]
    public void Language_RussianTranslation_AuthorEmail_IsPopulated()
    {
        // Russian translation includes an email address
        var ru = BloodPressure.Language.Translations["ru"];
        Assert.False(string.IsNullOrWhiteSpace(ru.AuthorEmail));
    }

    [Fact]
    public void Language_AuthorHelper_ReturnsNull_WhenKeyAbsent()
    {
        // Arabic translation has only name, no organisation or email
        var arSy = BloodPressure.Language.Translations["ar-sy"];
        Assert.Null(arSy.AuthorOrganisation);
        Assert.Null(arSy.AuthorEmail);
    }

    // ── description (smoke test; detail covered in ArchetypeDescriptionTests) ──

    [Fact]
    public void Description_IsPopulated() =>
        Assert.NotNull(BloodPressure.Description);

    [Fact]
    public void Description_HasEnglishDetail() =>
        Assert.True(BloodPressure.Description.Details.ContainsKey("en"));

    [Fact]
    public void Terminology_IsPopulated() =>
        Assert.NotNull(BloodPressure.Terminology);

    [Fact]
    public void Terminology_HasTermDefinitions() =>
        Assert.NotEmpty(BloodPressure.Terminology.TermDefinitions);
}
