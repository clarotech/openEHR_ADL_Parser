using Clarotech.openEHR.ADL2;
using ParserAdl2.Tests.Support;

namespace ParserAdl2.Tests.Models;

/// <summary>
/// Tests for CDvOrdinal constraints.
///
/// Source: openEHR-EHR-OBSERVATION.braden_scale_q.v0
///
/// ELEMENT[at0004] Sensory perception:
///   value matches { 1|[local::at0005], 2|[local::at0006], 3|[local::at0007], 4|[local::at0008] }
///
/// ELEMENT[at0021] Friction and shear:
///   value matches { 1|[local::at0023], 2|[local::at0024], 3|[local::at0025] }
/// </summary>
public class CDvOrdinalTests
{
    private static readonly CComplexObject Def = TestFixtures.BradenScale.Definition;

    // ── Sensory perception (at0004) — 4-item ordinal ──────────────────────────

    [Fact]
    public void SensoryPerception_Value_IsCDvOrdinal()
    {
        var el = AomHelpers.FindElement(Def, "at0004")!;
        Assert.Contains(el.GetAttribute("value")!.Children, c => c is CDvOrdinal);
    }

    [Fact]
    public void SensoryPerception_Ordinal_HasFourItems()
    {
        var ord = GetOrdinal("at0004")!;
        Assert.Equal(4, ord.Items.Count);
    }

    [Fact]
    public void SensoryPerception_Ordinal_FirstItem_ValueIsOne()
    {
        var ord = GetOrdinal("at0004")!;
        Assert.Equal(1, ord.Items[0].Value);
    }

    [Fact]
    public void SensoryPerception_Ordinal_FirstItem_CodeIsAt0005()
    {
        var ord = GetOrdinal("at0004")!;
        Assert.Equal("local::at0005", ord.Items[0].Code);
    }

    [Fact]
    public void SensoryPerception_Ordinal_LastItem_ValueIsFour()
    {
        var ord = GetOrdinal("at0004")!;
        Assert.Equal(4, ord.Items[3].Value);
    }

    [Fact]
    public void SensoryPerception_Ordinal_LastItem_CodeIsAt0008()
    {
        var ord = GetOrdinal("at0004")!;
        Assert.Equal("local::at0008", ord.Items[3].Code);
    }

    [Fact]
    public void SensoryPerception_Ordinal_ValuesAreSequential()
    {
        var ord = GetOrdinal("at0004")!;
        var values = ord.Items.Select(i => i.Value).ToList();
        Assert.Equal([1, 2, 3, 4], values);
    }

    // ── Friction and shear (at0021) — 3-item ordinal ──────────────────────────

    [Fact]
    public void FrictionAndShear_Ordinal_HasThreeItems()
    {
        var ord = GetOrdinal("at0021")!;
        Assert.Equal(3, ord.Items.Count);
    }

    [Fact]
    public void FrictionAndShear_Ordinal_CodesAreAt0023_To_At0025()
    {
        var ord   = GetOrdinal("at0021")!;
        var codes = ord.Items.Select(i => i.Code).ToList();
        Assert.Equal(["local::at0023", "local::at0024", "local::at0025"], codes);
    }

    // ── No assumed value ───────────────────────────────────────────────────────

    [Fact]
    public void SensoryPerception_Ordinal_AssumedValueIsNull()
    {
        var ord = GetOrdinal("at0004")!;
        Assert.Null(ord.AssumedValue);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static CDvOrdinal? GetOrdinal(string elementNodeId)
    {
        var el = AomHelpers.FindElement(Def, elementNodeId);
        return el?.GetAttribute("value")?.Children.OfType<CDvOrdinal>().FirstOrDefault();
    }
}
