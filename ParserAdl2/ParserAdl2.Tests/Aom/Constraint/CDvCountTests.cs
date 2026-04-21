using Clarotech.openEHR.ADL2;
using ParserAdl2.Tests.Support;

namespace ParserAdl2.Tests.Models;

/// <summary>
/// Tests for CDvCount constraints.
/// Source: ELEMENT[at0022] Total score in braden_scale_q.v0
///   value matches { DV_COUNT matches { magnitude matches {|7..28|} } }
/// </summary>
public class CDvCountTests
{
    private static readonly CComplexObject Def = TestFixtures.BradenScale.Definition;

    [Fact]
    public void TotalScore_Value_IsCDvCount()
    {
        var el = AomHelpers.FindElement(Def, "at0022")!;
        Assert.Contains(el.GetAttribute("value")!.Children, c => c is CDvCount);
    }

    [Fact]
    public void TotalScore_RmTypeName_IsDvCount()
    {
        Assert.Equal("DV_COUNT", GetDvCount()!.RmTypeName);
    }

    [Fact]
    public void TotalScore_Magnitude_IsNotNull()
    {
        Assert.NotNull(GetDvCount()!.Magnitude);
    }

    [Fact]
    public void TotalScore_Magnitude_LowerIsSeven()
    {
        Assert.Equal(7, GetDvCount()!.Magnitude!.Lower);
    }

    [Fact]
    public void TotalScore_Magnitude_UpperIsTwentyEight()
    {
        Assert.Equal(28, GetDvCount()!.Magnitude!.Upper);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static CDvCount? GetDvCount()
    {
        var el = AomHelpers.FindElement(Def, "at0022");
        return el?.GetAttribute("value")?.Children.OfType<CDvCount>().FirstOrDefault();
    }
}
