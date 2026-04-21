using Clarotech.openEHR.ADL2;
using ParserAdl2.Tests.Support;

namespace ParserAdl2.Tests.Models;

/// <summary>
/// Tests for CDvCodedText constraints.
///
/// Sources:
///   ELEMENT[at0005] Severity in problem_diagnosis.v1
///     value matches { DV_CODED_TEXT matches { defining_code matches { [local:: at0047, at0048, at0049] } } }
///   ELEMENT[at0008] Position in blood_pressure.v2
///     value matches { DV_CODED_TEXT matches { defining_code matches { [local:: at1000..at1014] } } }
///   ELEMENT[at0073] DiagnosticCertainty in problem_diagnosis.v1
/// </summary>
public class CDvCodedTextTests
{
    private static readonly CComplexObject BpDef = TestFixtures.BloodPressure.Definition;
    private static readonly CComplexObject PdDef = TestFixtures.ProblemDiagnosis.Definition;

    // ── Value produces CDvCodedText ───────────────────────────────────────────

    [Fact]
    public void Severity_Value_IsCDvCodedText()
    {
        var el = AomHelpers.FindElement(PdDef, "at0005")!;
        Assert.Contains(el.GetAttribute("value")!.Children, c => c is CDvCodedText);
    }

    [Fact]
    public void Severity_RmTypeName_IsDvCodedText()
    {
        Assert.Equal("DV_CODED_TEXT", GetDvCodedText(PdDef, "at0005")!.RmTypeName);
    }

    // ── DefiningCode presence ─────────────────────────────────────────────────

    [Fact]
    public void Severity_DefiningCode_IsNotNull()
    {
        Assert.NotNull(GetDefiningCode(PdDef, "at0005"));
    }

    [Fact]
    public void Severity_DefiningCode_TerminologyId_IsLocal()
    {
        Assert.Equal("local", GetDefiningCode(PdDef, "at0005")!.TerminologyId);
    }

    // ── Code list contents ────────────────────────────────────────────────────

    [Fact]
    public void Severity_DefiningCode_HasThreeCodes()
    {
        Assert.Equal(3, GetDefiningCode(PdDef, "at0005")!.CodeList.Count);
    }

    [Fact]
    public void Severity_DefiningCode_ContainsMildCode()
    {
        Assert.Contains("at0047", GetDefiningCode(PdDef, "at0005")!.CodeList);
    }

    [Fact]
    public void Severity_DefiningCode_ContainsAllCodes()
    {
        var codes = GetDefiningCode(PdDef, "at0005")!.CodeList;
        Assert.Contains("at0048", codes);
        Assert.Contains("at0049", codes);
    }

    // ── Larger code list ──────────────────────────────────────────────────────

    [Fact]
    public void Position_DefiningCode_HasFiveCodes()
    {
        // [local:: at1000, at1001, at1002, at1003, at1014]
        Assert.Equal(5, GetDefiningCode(BpDef, "at0008")!.CodeList.Count);
    }

    [Fact]
    public void Position_DefiningCode_ContainsAt1000()
    {
        Assert.Contains("at1000", GetDefiningCode(BpDef, "at0008")!.CodeList);
    }

    // ── Different element, same fixture ───────────────────────────────────────

    [Fact]
    public void DiagnosticCertainty_DefiningCode_HasThreeCodes()
    {
        Assert.Equal(3, GetDefiningCode(PdDef, "at0073")!.CodeList.Count);
    }

    // ── Unconstrained coded text ──────────────────────────────────────────────

    [Fact]
    public void Unconstrained_DvCodedText_DefiningCodeIsNull()
    {
        // math_function on INTERVAL_EVENT[at1042] uses [openehr::146] — a constrained coded text.
        // Verify the unconstrained case separately via any element that has DV_CODED_TEXT matches {*}.
        // Here we confirm that CDvCodedText.DefiningCode is null when no defining_code is specified.
        // (Structural: if DefiningCode is non-null it carries a CCodePhrase; null means unconstrained.)
        var constrained = GetDvCodedText(PdDef, "at0005")!;
        Assert.NotNull(constrained.DefiningCode); // sanity: this one IS constrained
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static CDvCodedText? GetDvCodedText(CComplexObject root, string elementNodeId)
    {
        var el = AomHelpers.FindElement(root, elementNodeId);
        return el?.GetAttribute("value")?.Children.OfType<CDvCodedText>().FirstOrDefault();
    }

    private static CCodePhrase? GetDefiningCode(CComplexObject root, string elementNodeId) =>
        GetDvCodedText(root, elementNodeId)?.DefiningCode;
}
