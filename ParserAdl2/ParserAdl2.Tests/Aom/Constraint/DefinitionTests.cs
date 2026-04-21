using Clarotech.openEHR.ADL2;
using ParserAdl2.Tests.Support;

namespace ParserAdl2.Tests.Models;

public class DefinitionTests
{
    private static readonly Archetype BloodPressure = TestFixtures.BloodPressure;
    private static CComplexObject Def => BloodPressure.Definition;

    // ── Root object ───────────────────────────────────────────────────────────

    [Fact]
    public void Definition_RmTypeName_IsObservation() =>
        Assert.Equal("OBSERVATION", Def.RmTypeName);

    [Fact]
    public void Definition_NodeId_IsAt0000() =>
        Assert.Equal("at0000", Def.NodeId);

    [Fact]
    public void Definition_HasDataAttribute() =>
        Assert.NotNull(Def.GetAttribute("data"));

    // ── data/events/items ─────────────────────────────────────────────────────

    [Fact]
    public void Data_Child_IsHistory()
    {
        var data = Def.GetAttribute("data");
        Assert.NotNull(data);
        var history = data.Children.OfType<CComplexObject>().FirstOrDefault();
        Assert.NotNull(history);
        Assert.Equal("HISTORY", history.RmTypeName);
        Assert.Equal("at0001", history.NodeId);
    }

    [Fact]
    public void Events_HasCardinality()
    {
        var history = Def.GetAttribute("data")!.Children
            .OfType<CComplexObject>().First();
        var events = history.GetAttribute("events") as CMultipleAttribute;
        Assert.NotNull(events);
        Assert.NotNull(events.Cardinality);
    }

    [Fact]
    public void Events_Cardinality_IsOneOrMore()
    {
        var history = Def.GetAttribute("data")!.Children
            .OfType<CComplexObject>().First();
        var events = (CMultipleAttribute)history.GetAttribute("events")!;
        Assert.Equal(1, events.Cardinality!.Interval.Lower);
        Assert.Null(events.Cardinality.Interval.Upper);
    }

    // ── Systolic element ──────────────────────────────────────────────────────

    [Fact]
    public void Systolic_Element_Exists()
    {
        var systolic = FindElement("at0004");
        Assert.NotNull(systolic);
    }

    [Fact]
    public void Systolic_Element_RmType_IsElement() =>
        Assert.Equal("ELEMENT", FindElement("at0004")!.RmTypeName);

    [Fact]
    public void Systolic_Occurrences_AreZeroToOne()
    {
        var systolic = FindElement("at0004")!;
        Assert.Equal(new IntervalOfInt(0, 1), systolic.Occurrences);
    }

    [Fact]
    public void Systolic_Value_IsDomainType()
    {
        var systolic = FindElement("at0004")!;
        var valueAttr = systolic.GetAttribute("value");
        Assert.NotNull(valueAttr);
        var domainType = valueAttr.Children.OfType<CDvQuantity>().FirstOrDefault();
        Assert.NotNull(domainType);
        Assert.Equal("C_DV_QUANTITY", domainType.RmTypeName);
    }

    // ── Archetype slot ────────────────────────────────────────────────────────

    [Fact]
    public void ArchetypeSlot_Exertion_Exists()
    {
        var slot = FindSlot("at1030");
        Assert.NotNull(slot);
    }

    [Fact]
    public void ArchetypeSlot_Exertion_HasInclude()
    {
        var slot = FindSlot("at1030")!;
        Assert.NotEmpty(slot.Includes);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static CComplexObject? FindElement(string nodeId) =>
        AomHelpers.FindElement(Def, nodeId);

    private static ArchetypeSlot? FindSlot(string nodeId) =>
        AomHelpers.FindSlot(Def, nodeId);
}
