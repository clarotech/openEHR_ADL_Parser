using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests.Models;

public class ArchetypeDescriptionTests
{
    private static readonly Archetype BloodPressure = TestFixtures.BloodPressure;
    private static ArchetypeDescription Desc => BloodPressure.Description;

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
        Assert.Equal("2006-03-22", Desc.AuthorDate);

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
    public void EnglishDetail_Copyright_IsPopulated() =>
        Assert.False(string.IsNullOrWhiteSpace(Desc.Details["en"].Copyright));

    [Fact]
    public void EnglishDetail_Keywords_ArePopulated() =>
        Assert.NotEmpty(Desc.Details["en"].Keywords);

    [Fact]
    public void EnglishDetail_Keywords_ContainsBp() =>
        Assert.Contains("bp", Desc.Details["en"].Keywords);

    [Fact]
    public void SwedishDetail_HasNoCopyright() =>
        // Swedish translation omits copyright in this archetype
        Assert.Null(Desc.Details["sv"].Copyright);

    // ── other_details helpers ─────────────────────────────────────────────────

    [Fact]
    public void Revision_IsPopulated() =>
        Assert.False(string.IsNullOrWhiteSpace(Desc.Revision));

    [Fact]
    public void CustodianOrganisation_IsOpenEHR() =>
        Assert.Equal("openEHR Foundation", Desc.CustodianOrganisation);

    [Fact]
    public void OriginalNamespace_IsPopulated() =>
        Assert.False(string.IsNullOrWhiteSpace(Desc.OriginalNamespace));

    [Fact]
    public void BuildUid_IsPopulated() =>
        Assert.False(string.IsNullOrWhiteSpace(Desc.BuildUid));

    [Fact]
    public void Licence_IsPopulated() =>
        Assert.False(string.IsNullOrWhiteSpace(Desc.Licence));
}
