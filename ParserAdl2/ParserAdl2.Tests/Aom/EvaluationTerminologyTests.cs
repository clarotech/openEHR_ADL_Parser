using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests.Models;

public class EvaluationTerminologyTests
{
    private static readonly Archetype PD = TestFixtures.ProblemDiagnosis;
    private static ArchetypeTerminology Term => PD.Terminology;

    // ── terminologies_available ───────────────────────────────────────────────

    [Fact]
    public void TerminologiesAvailable_IsEmpty() =>
        // problem_diagnosis uses no external terminologies
        Assert.Empty(Term.TerminologiesAvailable);

    // ── term_definitions ──────────────────────────────────────────────────────

    [Fact]
    public void TermDefinitions_ContainsEnglish() =>
        Assert.True(Term.TermDefinitions.ContainsKey("en"));

    [Fact]
    public void TermDefinitions_ContainsSpanish() =>
        Assert.True(Term.TermDefinitions.ContainsKey("es"));

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
    public void GetTermDefinition_At0000_TextIsProblemDiagnosis() =>
        Assert.Equal("Problem/Diagnosis", Term.GetTermDefinition("at0000")?.Text);

    [Fact]
    public void GetTermDefinition_At0000_DescriptionIsPopulated() =>
        Assert.False(string.IsNullOrWhiteSpace(Term.GetTermDefinition("at0000")?.Description));

    [Fact]
    public void GetTermDefinition_At0000_HasComment() =>
        Assert.False(string.IsNullOrWhiteSpace(Term.GetTermDefinition("at0000")?.Comment));

    [Fact]
    public void GetTermDefinition_At0002_TextIsProblemDiagnosisName() =>
        Assert.Equal("Problem/Diagnosis name", Term.GetTermDefinition("at0002")?.Text);

    [Fact]
    public void GetTermDefinition_At0047_TextIsMild() =>
        Assert.Equal("Mild", Term.GetTermDefinition("at0047")?.Text);

    [Fact]
    public void GetTermDefinition_At0048_TextIsModerate() =>
        Assert.Equal("Moderate", Term.GetTermDefinition("at0048")?.Text);

    [Fact]
    public void GetTermDefinition_At0049_TextIsSevere() =>
        Assert.Equal("Severe", Term.GetTermDefinition("at0049")?.Text);

    [Fact]
    public void GetTermDefinition_At0073_TextIsDiagnosticCertainty() =>
        Assert.Equal("Diagnostic certainty", Term.GetTermDefinition("at0073")?.Text);

    [Fact]
    public void GetTermDefinition_UnknownCode_ReturnsNull() =>
        Assert.Null(Term.GetTermDefinition("at9999"));

    [Fact]
    public void GetTermDefinition_UnknownLanguage_ReturnsNull() =>
        Assert.Null(Term.GetTermDefinition("at0000", "xx"));

    [Fact]
    public void GetTermDefinition_Spanish_At0000_IsPopulated()
    {
        var def = Term.GetTermDefinition("at0000", "es");
        Assert.NotNull(def);
        Assert.False(string.IsNullOrWhiteSpace(def.Text));
    }

    // ── term_bindings ─────────────────────────────────────────────────────────

    [Fact]
    public void TermBindings_IsEmpty() =>
        // problem_diagnosis has no external term bindings
        Assert.Empty(Term.TermBindings);
}
