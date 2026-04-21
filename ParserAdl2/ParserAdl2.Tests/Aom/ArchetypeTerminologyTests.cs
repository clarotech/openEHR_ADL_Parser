using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests.Models;

public class ArchetypeTerminologyTests
{
    private static readonly Archetype BloodPressure = TestFixtures.BloodPressure;
    private static ArchetypeTerminology Term => BloodPressure.Terminology;

    // ── terminologies_available ───────────────────────────────────────────────

    [Fact]
    public void TerminologiesAvailable_ContainsSnomedCt() =>
        Assert.Contains("SNOMED-CT", Term.TerminologiesAvailable);

    // ── term_definitions ──────────────────────────────────────────────────────

    [Fact]
    public void TermDefinitions_ContainsEnglish() =>
        Assert.True(Term.TermDefinitions.ContainsKey("en"));

    [Fact]
    public void TermDefinitions_EnglishHasManyEntries() =>
        Assert.True(Term.TermDefinitions["en"].Count > 10);

    [Fact]
    public void GetTermDefinition_ReturnsEnglishByDefault()
    {
        var def = Term.GetTermDefinition("at0000");
        Assert.NotNull(def);
        Assert.Equal("at0000", def.Code);
    }

    [Fact]
    public void GetTermDefinition_At0000_TextIsBloodPressure() =>
        Assert.Equal("Blood pressure", Term.GetTermDefinition("at0000")?.Text);

    [Fact]
    public void GetTermDefinition_At0000_DescriptionIsPopulated() =>
        Assert.False(string.IsNullOrWhiteSpace(Term.GetTermDefinition("at0000")?.Description));

    [Fact]
    public void GetTermDefinition_At0000_HasComment() =>
        Assert.False(string.IsNullOrWhiteSpace(Term.GetTermDefinition("at0000")?.Comment));

    [Fact]
    public void GetTermDefinition_At0001_CommentIsNull() =>
        // at0001 (History) has no comment in this archetype
        Assert.Null(Term.GetTermDefinition("at0001")?.Comment);

    [Fact]
    public void GetTermDefinition_At0004_TextIsSystolic() =>
        Assert.Equal("Systolic", Term.GetTermDefinition("at0004")?.Text);

    [Fact]
    public void GetTermDefinition_At0005_TextIsDiastolic() =>
        Assert.Equal("Diastolic", Term.GetTermDefinition("at0005")?.Text);

    [Fact]
    public void GetTermDefinition_UnknownCode_ReturnsNull() =>
        Assert.Null(Term.GetTermDefinition("at9999"));

    [Fact]
    public void GetTermDefinition_UnknownLanguage_ReturnsNull() =>
        Assert.Null(Term.GetTermDefinition("at0000", "xx"));

    [Fact]
    public void GetTermDefinition_JapaneseAt0000_IsPopulated()
    {
        var def = Term.GetTermDefinition("at0000", "ja");
        Assert.NotNull(def);
        Assert.False(string.IsNullOrWhiteSpace(def.Text));
    }

    // ── term_bindings ─────────────────────────────────────────────────────────

    [Fact]
    public void TermBindings_ContainsSnomedCt() =>
        Assert.True(Term.TermBindings.ContainsKey("SNOMED-CT"));

    [Fact]
    public void SnomedBinding_At0000_IsPopulated()
    {
        var binding = Term.TermBindings["SNOMED-CT"]["at0000"];
        Assert.Equal("at0000", binding.Code);
        Assert.NotNull(binding.Target);
    }

    [Fact]
    public void SnomedBinding_At0000_TargetCodeIsCorrect() =>
        Assert.Equal("364090009", Term.TermBindings["SNOMED-CT"]["at0000"].Target.CodeString);

    [Fact]
    public void SnomedBinding_At0004_TargetCodeIsCorrect() =>
        Assert.Equal("271649006", Term.TermBindings["SNOMED-CT"]["at0004"].Target.CodeString);
}
