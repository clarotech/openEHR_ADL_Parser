using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests;

/// <summary>
/// Integration tests that parse the embedded problem/diagnosis EVALUATION archetype
/// and assert the full AOM model is populated correctly.
/// </summary>
public class EvaluationArchetypeParserTests
{
    private static readonly Archetype PD = TestFixtures.ProblemDiagnosis;

    // ── identity ──────────────────────────────────────────────────────────────

    [Fact]
    public void ArchetypeId_FullId_IsCorrect() =>
        Assert.Equal("openEHR-EHR-EVALUATION.problem_diagnosis.v1", PD.Id.FullId);

    [Fact]
    public void ArchetypeId_Components_AreCorrect()
    {
        Assert.Equal("openEHR",           PD.Id.RmPublisher);
        Assert.Equal("EHR",               PD.Id.RmPackage);
        Assert.Equal("EVALUATION",        PD.Id.RmClass);
        Assert.Equal("problem_diagnosis", PD.Id.ConceptName);
        Assert.Equal("v1",                PD.Id.VersionId);
    }

    // ── metadata ──────────────────────────────────────────────────────────────

    [Fact]
    public void MetaData_AdlVersion_Is1_4() =>
        Assert.Equal("1.4", PD.MetaData.AdlVersion);

    [Fact]
    public void MetaData_Uid_IsPresent() =>
        Assert.False(string.IsNullOrEmpty(PD.MetaData.Uid));

    // ── concept ───────────────────────────────────────────────────────────────

    [Fact]
    public void ConceptCode_IsAt0000() =>
        Assert.Equal("at0000", PD.ConceptCode);

    // ── language ──────────────────────────────────────────────────────────────

    [Fact]
    public void Language_OriginalLanguage_IsEnglish() =>
        Assert.Equal("[ISO_639-1::en]", PD.Language.OriginalLanguage.ToString());

    [Fact]
    public void Language_Translations_HasFourteenEntries() =>
        Assert.Equal(14, PD.Language.Translations.Count);

    [Fact]
    public void Language_Translations_ContainGerman() =>
        Assert.True(PD.Language.Translations.ContainsKey("de"));

    [Fact]
    public void Language_Translations_ContainFinnish() =>
        Assert.True(PD.Language.Translations.ContainsKey("fi"));

    [Fact]
    public void Language_Translations_ContainKorean() =>
        Assert.True(PD.Language.Translations.ContainsKey("ko"));

    [Fact]
    public void Language_GermanTranslation_AuthorName_IsPopulated()
    {
        var de = PD.Language.Translations["de"];
        Assert.False(string.IsNullOrWhiteSpace(de.AuthorName));
    }

    [Fact]
    public void Language_KoreanTranslation_HasAccreditation()
    {
        var ko = PD.Language.Translations["ko"];
        Assert.Equal("M.D.", ko.Accreditation);
    }

    [Fact]
    public void Language_FinnishTranslation_AuthorName_IsNull()
    {
        // Finnish translation has an empty author block
        var fi = PD.Language.Translations["fi"];
        Assert.Null(fi.AuthorName);
    }

    // ── description (smoke test) ───────────────────────────────────────────────

    [Fact]
    public void Description_IsPopulated() =>
        Assert.NotNull(PD.Description);

    [Fact]
    public void Description_HasEnglishDetail() =>
        Assert.True(PD.Description.Details.ContainsKey("en"));

    // ── terminology (smoke test) ──────────────────────────────────────────────

    [Fact]
    public void Terminology_IsPopulated() =>
        Assert.NotNull(PD.Terminology);

    [Fact]
    public void Terminology_HasTermDefinitions() =>
        Assert.NotEmpty(PD.Terminology.TermDefinitions);

    // ── definition (smoke test) ───────────────────────────────────────────────

    [Fact]
    public void Definition_IsPopulated() =>
        Assert.NotNull(PD.Definition);

    [Fact]
    public void Definition_RmTypeName_IsEvaluation() =>
        Assert.Equal("EVALUATION", PD.Definition.RmTypeName);
}
