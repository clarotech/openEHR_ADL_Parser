using Clarotech.openEHR.ADL2;
using ParserAdl2.Tests.Support;

namespace ParserAdl2.Tests.Models;

/// <summary>
/// Tests for CDvDuration constraints.
///
/// Two cases from different fixtures:
///
/// A) Specific value — INTERVAL_EVENT[at1042] width in blood_pressure.v2
///      width matches { DV_DURATION matches { value matches {PT24H} } }
///      → CDvDuration { Range = "PT24H", Pattern = null }
///
/// B) Constraint pattern — ELEMENT[at0005] Age at injury in glasgow_outcome_scale_extended.v1
///      value matches { DV_DURATION matches { value matches {PY} } }
///      → CDvDuration { Pattern = "PY", Range = null }
/// </summary>
public class CDvDurationTests
{
    private static readonly CComplexObject BpDef     = TestFixtures.BloodPressure.Definition;
    private static readonly CComplexObject GoscDef   = TestFixtures.GlasgowOutcomeScale.Definition;

    // ── A: Specific value (PT24H) ─────────────────────────────────────────────

    [Fact]
    public void Width_IsCDvDuration()
    {
        var ev    = AomHelpers.FindElement(BpDef, "at1042")!;
        var width = ev.GetAttribute("width")!;
        Assert.Contains(width.Children, c => c is CDvDuration);
    }

    [Fact]
    public void Width_RmTypeName_IsDvDuration()
    {
        Assert.Equal("DV_DURATION", GetWidthDuration()!.RmTypeName);
    }

    [Fact]
    public void Width_SpecificValue_RangeIsPopulated()
    {
        Assert.False(string.IsNullOrWhiteSpace(GetWidthDuration()!.Range));
    }

    [Fact]
    public void Width_SpecificValue_PatternIsNull()
    {
        Assert.Null(GetWidthDuration()!.Pattern);
    }

    // ── B: Constraint pattern (PY) ────────────────────────────────────────────

    [Fact]
    public void AgeAtInjury_IsCDvDuration()
    {
        var el = AomHelpers.FindElement(GoscDef, "at0005")!;
        Assert.Contains(el.GetAttribute("value")!.Children, c => c is CDvDuration);
    }

    [Fact]
    public void AgeAtInjury_PatternIsPopulated()
    {
        Assert.False(string.IsNullOrWhiteSpace(GetAgeAtInjuryDuration()!.Pattern));
    }

    [Fact]
    public void AgeAtInjury_RangeIsNull()
    {
        Assert.Null(GetAgeAtInjuryDuration()!.Range);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static CDvDuration? GetWidthDuration()
    {
        var ev = AomHelpers.FindElement(BpDef, "at1042");
        return ev?.GetAttribute("width")?.Children.OfType<CDvDuration>().FirstOrDefault();
    }

    private static CDvDuration? GetAgeAtInjuryDuration()
    {
        var el = AomHelpers.FindElement(GoscDef, "at0005");
        return el?.GetAttribute("value")?.Children.OfType<CDvDuration>().FirstOrDefault();
    }
}
