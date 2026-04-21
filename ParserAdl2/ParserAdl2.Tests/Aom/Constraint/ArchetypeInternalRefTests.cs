using Clarotech.openEHR.ADL2;
using ParserAdl2.Tests.Support;

namespace ParserAdl2.Tests.Models;

/// <summary>
/// Tests for ArchetypeInternalRef (use_node).
/// The blood pressure archetype's INTERVAL_EVENT[at1042] (24-hour average) re-uses
/// the data and state trees from the any-event node via use_node references.
/// </summary>
public class ArchetypeInternalRefTests
{
    private static readonly CComplexObject Def = TestFixtures.BloodPressure.Definition;

    // ── data attribute ref ────────────────────────────────────────────────────

    [Fact]
    public void IntervalEvent_DataAttribute_ContainsInternalRef()
    {
        var ev = AomHelpers.FindElement(Def, "at1042")!;
        var data = ev.GetAttribute("data");
        Assert.NotNull(data);
        Assert.Contains(data.Children, c => c is ArchetypeInternalRef);
    }

    [Fact]
    public void IntervalEvent_DataRef_RmTypeIsItemTree()
    {
        var ev  = AomHelpers.FindElement(Def, "at1042")!;
        var ref_ = ev.GetAttribute("data")!.Children.OfType<ArchetypeInternalRef>().First();
        Assert.Equal("ITEM_TREE", ref_.RmTypeName);
    }

    [Fact]
    public void IntervalEvent_DataRef_TargetPath_IsCorrect()
    {
        var ev  = AomHelpers.FindElement(Def, "at1042")!;
        var ref_ = ev.GetAttribute("data")!.Children.OfType<ArchetypeInternalRef>().First();
        Assert.Equal("/data[at0001]/events[at0006]/data[at0003]", ref_.TargetPath);
    }

    // ── state attribute ref ───────────────────────────────────────────────────

    [Fact]
    public void IntervalEvent_StateAttribute_ContainsInternalRef()
    {
        var ev    = AomHelpers.FindElement(Def, "at1042")!;
        var state = ev.GetAttribute("state");
        Assert.NotNull(state);
        Assert.Contains(state.Children, c => c is ArchetypeInternalRef);
    }

    [Fact]
    public void IntervalEvent_StateRef_RmTypeIsItemTree()
    {
        var ev  = AomHelpers.FindElement(Def, "at1042")!;
        var ref_ = ev.GetAttribute("state")!.Children.OfType<ArchetypeInternalRef>().First();
        Assert.Equal("ITEM_TREE", ref_.RmTypeName);
    }

    [Fact]
    public void IntervalEvent_StateRef_TargetPath_IsCorrect()
    {
        var ev  = AomHelpers.FindElement(Def, "at1042")!;
        var ref_ = ev.GetAttribute("state")!.Children.OfType<ArchetypeInternalRef>().First();
        Assert.Equal("/data[at0001]/events[at0006]/state[at0007]", ref_.TargetPath);
    }
}
