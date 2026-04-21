using Clarotech.openEHR.ADL2;
using ParserAdl2.Tests.Support;

namespace ParserAdl2.Tests.Models;

/// <summary>
/// Tests for CDvDate constraints.
/// Source: ELEMENT[at0004] Date of injury in glasgow_outcome_scale_extended.v1
///   value matches { DV_DATE matches {*} }
/// </summary>
public class CDvDateTests
{
    private static readonly CComplexObject Def = TestFixtures.GlasgowOutcomeScale.Definition;

    [Fact]
    public void DateOfInjury_Value_IsCDvDate()
    {
        var el = AomHelpers.FindElement(Def, "at0004")!;
        Assert.Contains(el.GetAttribute("value")!.Children, c => c is CDvDate);
    }

    [Fact]
    public void DateOfInjury_RmTypeName_IsDvDate()
    {
        Assert.Equal("DV_DATE", GetDvDate()!.RmTypeName);
    }

    [Fact]
    public void DateOfInjury_Unconstrained_PatternIsNull()
    {
        Assert.Null(GetDvDate()!.Pattern);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static CDvDate? GetDvDate()
    {
        var el = AomHelpers.FindElement(Def, "at0004");
        return el?.GetAttribute("value")?.Children.OfType<CDvDate>().FirstOrDefault();
    }
}
