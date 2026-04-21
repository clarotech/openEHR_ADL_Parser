using Clarotech.openEHR.ADL2;
using ParserAdl2.Tests.Support;

namespace ParserAdl2.Tests.Models;

/// <summary>
/// Tests for CDvDateTime constraints.
/// Source: ELEMENT[at0077] Last updated in problem_diagnosis.v1
///   value matches { DV_DATE_TIME matches {*} }
/// </summary>
public class CDvDateTimeTests
{
    private static readonly CComplexObject Def = TestFixtures.ProblemDiagnosis.Definition;

    [Fact]
    public void LastUpdated_Value_IsCDvDateTime()
    {
        var el = AomHelpers.FindElement(Def, "at0077")!;
        Assert.Contains(el.GetAttribute("value")!.Children, c => c is CDvDateTime);
    }

    [Fact]
    public void LastUpdated_RmTypeName_IsDvDateTime()
    {
        Assert.Equal("DV_DATE_TIME", GetDvDateTime()!.RmTypeName);
    }

    [Fact]
    public void LastUpdated_Unconstrained_PatternIsNull()
    {
        Assert.Null(GetDvDateTime()!.Pattern);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static CDvDateTime? GetDvDateTime()
    {
        var el = AomHelpers.FindElement(Def, "at0077");
        return el?.GetAttribute("value")?.Children.OfType<CDvDateTime>().FirstOrDefault();
    }
}
