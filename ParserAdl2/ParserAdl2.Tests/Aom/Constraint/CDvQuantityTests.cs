using Clarotech.openEHR.ADL2;
using ParserAdl2.Tests.Support;

namespace ParserAdl2.Tests.Models;

public class CDvQuantityTests
{
    private static readonly Archetype BP = TestFixtures.BloodPressure;

    // ── Systolic (at0004) ─────────────────────────────────────────────────────

    [Fact]
    public void Systolic_Value_IsCDvQuantity()
    {
        var domainType = GetDomainType("at0004");
        Assert.IsType<CDvQuantity>(domainType);
    }

    [Fact]
    public void Systolic_Property_CodeIs125()
    {
        var dq = (CDvQuantity)GetDomainType("at0004")!;
        Assert.Equal("125", dq.Property?.CodeString);
    }

    [Fact]
    public void Systolic_Property_TerminologyIsOpenehr()
    {
        var dq = (CDvQuantity)GetDomainType("at0004")!;
        Assert.Equal("openehr", dq.Property?.TerminologyId);
    }

    [Fact]
    public void Systolic_List_HasOneItem()
    {
        var dq = (CDvQuantity)GetDomainType("at0004")!;
        Assert.Single(dq.List);
    }

    [Fact]
    public void Systolic_Units_IsMmHg()
    {
        var dq = (CDvQuantity)GetDomainType("at0004")!;
        Assert.Equal("mm[Hg]", dq.List[0].Units);
    }

    [Fact]
    public void Systolic_Magnitude_LowerIsZero()
    {
        var dq = (CDvQuantity)GetDomainType("at0004")!;
        Assert.Equal(0.0, dq.List[0].Magnitude?.Lower);
    }

    [Fact]
    public void Systolic_Magnitude_UpperIs1000()
    {
        var dq = (CDvQuantity)GetDomainType("at0004")!;
        Assert.Equal(1000.0, dq.List[0].Magnitude?.Upper);
    }

    [Fact]
    public void Systolic_Magnitude_UpperIsExclusive()
    {
        // |0.0..<1000.0| — upper bound is exclusive (<)
        var dq = (CDvQuantity)GetDomainType("at0004")!;
        Assert.True(dq.List[0].Magnitude?.UpperUnbounded);
    }

    [Fact]
    public void Systolic_Precision_IsZero()
    {
        // precision = <|0|>
        var dq = (CDvQuantity)GetDomainType("at0004")!;
        Assert.Equal(0, dq.List[0].Precision?.Lower);
        Assert.Equal(0, dq.List[0].Precision?.Upper);
    }

    // ── Tilt (at1005) uses degrees — different property and units ─────────────

    [Fact]
    public void Tilt_Property_CodeIs497()
    {
        var dq = (CDvQuantity)GetDomainType("at1005")!;
        Assert.Equal("497", dq.Property?.CodeString);
    }

    [Fact]
    public void Tilt_Units_IsDeg()
    {
        var dq = (CDvQuantity)GetDomainType("at1005")!;
        Assert.Equal("deg", dq.List[0].Units);
    }

    [Fact]
    public void Tilt_Magnitude_LowerIsMinus90()
    {
        // magnitude = <|-90.0..90.0|>
        var dq = (CDvQuantity)GetDomainType("at1005")!;
        Assert.Equal(-90.0, dq.List[0].Magnitude?.Lower);
    }

    [Fact]
    public void Tilt_Magnitude_UpperIs90()
    {
        var dq = (CDvQuantity)GetDomainType("at1005")!;
        Assert.Equal(90.0, dq.List[0].Magnitude?.Upper);
    }

    // ── RawOdin still populated ───────────────────────────────────────────────

    [Fact]
    public void Systolic_RawOdin_IsPopulated()
    {
        var dq = (CDvQuantity)GetDomainType("at0004")!;
        Assert.False(string.IsNullOrWhiteSpace(dq.RawOdin));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static CDvQuantity? GetDomainType(string elementNodeId)
    {
        var el = FindElement(BP.Definition, elementNodeId);
        var valueAttr = el?.GetAttribute("value");
        return valueAttr?.Children.OfType<CDvQuantity>().FirstOrDefault();
    }

    private static CComplexObject? FindElement(CComplexObject root, string nodeId) =>
        AomHelpers.FindElement(root, nodeId);
}
