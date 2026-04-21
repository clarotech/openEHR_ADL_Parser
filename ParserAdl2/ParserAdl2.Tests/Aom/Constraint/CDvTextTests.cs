using Clarotech.openEHR.ADL2;
using ParserAdl2.Tests.Support;

namespace ParserAdl2.Tests.Models;

/// <summary>
/// Tests for CDvText constraints.
/// Source: ELEMENT[at1059] Clinical interpretation in blood_pressure.v2
///   value matches { DV_TEXT matches {*} }
/// </summary>
public class CDvTextTests
{
    private static readonly CComplexObject Def = TestFixtures.BloodPressure.Definition;

    [Fact]
    public void DvText_Unconstrained_YieldsCDvText()
    {
        var el = AomHelpers.FindElement(Def, "at1059")!;
        Assert.Contains(el.GetAttribute("value")!.Children, c => c is CDvText);
    }

    [Fact]
    public void DvText_RmTypeName_IsDvText()
    {
        var dvt = GetDvText("at1059")!;
        Assert.Equal("DV_TEXT", dvt.RmTypeName);
    }

    [Fact]
    public void DvText_Unconstrained_PatternIsNull()
    {
        Assert.Null(GetDvText("at1059")!.Pattern);
    }

    [Fact]
    public void DvText_Unconstrained_ListIsEmpty()
    {
        Assert.Empty(GetDvText("at1059")!.List);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static CDvText? GetDvText(string elementNodeId)
    {
        var el = AomHelpers.FindElement(Def, elementNodeId);
        return el?.GetAttribute("value")?.Children.OfType<CDvText>().FirstOrDefault();
    }
}
