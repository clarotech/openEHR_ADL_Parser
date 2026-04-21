using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests.Models;

public class EvaluationDescriptionTests
{
    private static readonly Archetype PD = TestFixtures.ProblemDiagnosis;
    private static ArchetypeDescription Desc => PD.Description;

    // ── original_author ───────────────────────────────────────────────────────

    [Fact]
    public void AuthorName_IsPopulated() =>
        Assert.Equal("Sam Heard", Desc.AuthorName);

    [Fact]
    public void AuthorOrganisation_IsPopulated() =>
        Assert.Equal("Ocean Informatics", Desc.AuthorOrganisation);

    [Fact]
    public void AuthorEmail_IsPopulated() =>
        Assert.Equal("sam.heard@oceaninformatics.com", Desc.AuthorEmail);

    [Fact]
    public void AuthorDate_IsPopulated() =>
        Assert.Equal("2006-04-23", Desc.AuthorDate);

    // ── lifecycle / contributors ──────────────────────────────────────────────

    [Fact]
    public void LifecycleState_IsPublished() =>
        Assert.Equal("published", Desc.LifecycleState);

    [Fact]
    public void OtherContributors_IsNonEmpty() =>
        Assert.NotEmpty(Desc.OtherContributors);

    // ── details ───────────────────────────────────────────────────────────────

    [Fact]
    public void Details_ContainsEnglish() =>
        Assert.True(Desc.Details.ContainsKey("en"));

    [Fact]
    public void Details_ContainsSpanish() =>
        Assert.True(Desc.Details.ContainsKey("es"));

    [Fact]
    public void Details_ContainsGerman() =>
        Assert.True(Desc.Details.ContainsKey("de"));

    [Fact]
    public void EnglishDetail_Language_IsEnglish() =>
        Assert.Equal("[ISO_639-1::en]", Desc.Details["en"].Language.ToString());

    [Fact]
    public void EnglishDetail_Purpose_IsPopulated() =>
        Assert.False(string.IsNullOrWhiteSpace(Desc.Details["en"].Purpose));

    [Fact]
    public void EnglishDetail_Use_IsPopulated() =>
        Assert.False(string.IsNullOrWhiteSpace(Desc.Details["en"].Use));

    [Fact]
    public void EnglishDetail_Misuse_IsPopulated() =>
        Assert.False(string.IsNullOrWhiteSpace(Desc.Details["en"].Misuse));

    [Fact]
    public void EnglishDetail_Keywords_ArePopulated() =>
        Assert.NotEmpty(Desc.Details["en"].Keywords);

    [Fact]
    public void EnglishDetail_Keywords_ContainsDiagnosis() =>
        Assert.Contains("diagnosis", Desc.Details["en"].Keywords);

    // ── other_details helpers ─────────────────────────────────────────────────

    [Fact]
    public void Revision_IsCorrect() =>
        Assert.Equal("1.6.0", Desc.Revision);

    [Fact]
    public void CustodianOrganisation_IsOpenEHR() =>
        Assert.Equal("openEHR Foundation", Desc.CustodianOrganisation);

    [Fact]
    public void OriginalNamespace_IsOpenEHR() =>
        Assert.Equal("org.openehr", Desc.OriginalNamespace);

    [Fact]
    public void OriginalPublisher_IsOpenEHR() =>
        Assert.Equal("openEHR Foundation", Desc.OriginalPublisher);

    [Fact]
    public void BuildUid_IsPopulated() =>
        Assert.False(string.IsNullOrWhiteSpace(Desc.BuildUid));

    [Fact]
    public void Licence_IsPopulated() =>
        Assert.False(string.IsNullOrWhiteSpace(Desc.Licence));
}
