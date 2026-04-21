using Clarotech.openEHR.ADL2;
using ParserAdl2.Tests.Support;

namespace ParserAdl2.Tests.Models;

public class EvaluationDefinitionTests
{
    private static readonly Archetype PD = TestFixtures.ProblemDiagnosis;
    private static CComplexObject Def => PD.Definition;

    // ── Root object ───────────────────────────────────────────────────────────

    [Fact]
    public void Definition_RmTypeName_IsEvaluation() =>
        Assert.Equal("EVALUATION", Def.RmTypeName);

    [Fact]
    public void Definition_NodeId_IsAt0000() =>
        Assert.Equal("at0000", Def.NodeId);

    [Fact]
    public void Definition_HasDataAndProtocolAttributes()
    {
        Assert.NotNull(Def.GetAttribute("data"));
        Assert.NotNull(Def.GetAttribute("protocol"));
    }

    // ── data / ITEM_TREE / items ──────────────────────────────────────────────

    [Fact]
    public void Data_Child_IsItemTree()
    {
        var data   = Def.GetAttribute("data")!;
        var tree   = data.Children.OfType<CComplexObject>().FirstOrDefault();
        Assert.NotNull(tree);
        Assert.Equal("ITEM_TREE", tree.RmTypeName);
        Assert.Equal("at0001",    tree.NodeId);
    }

    [Fact]
    public void Items_HasCardinality()
    {
        var items = GetItemsAttribute();
        Assert.IsType<CMultipleAttribute>(items);
        Assert.NotNull(((CMultipleAttribute)items).Cardinality);
    }

    [Fact]
    public void Items_Cardinality_IsOneOrMore()
    {
        var cardinality = ((CMultipleAttribute)GetItemsAttribute()).Cardinality!;
        Assert.Equal(1, cardinality.Interval.Lower);
        Assert.Null(cardinality.Interval.Upper);
    }

    [Fact]
    public void Items_Cardinality_IsUnordered()
    {
        var cardinality = ((CMultipleAttribute)GetItemsAttribute()).Cardinality!;
        Assert.False(cardinality.IsOrdered);
    }

    // ── ELEMENT occurrences ───────────────────────────────────────────────────

    [Fact]
    public void ProblemDiagnosisName_Element_Exists() =>
        Assert.NotNull(FindElement("at0002"));

    [Fact]
    public void ProblemDiagnosisName_Occurrences_AreNull()
    {
        // ELEMENT[at0002] has no explicit occurrences in the ADL
        var el = FindElement("at0002")!;
        Assert.Null(el.Occurrences);
    }

    [Fact]
    public void Variant_Element_Occurrences_AreZeroToMany()
    {
        // ELEMENT[at0079] occurrences matches {0..*}
        var el = FindElement("at0079")!;
        Assert.Equal(new IntervalOfInt(0, null), el.Occurrences);
    }

    [Fact]
    public void Severity_Element_Occurrences_AreZeroToOne()
    {
        // ELEMENT[at0005] occurrences matches {0..1}
        var el = FindElement("at0005")!;
        Assert.Equal(new IntervalOfInt(0, 1), el.Occurrences);
    }

    // ── ArchetypeSlots in data ────────────────────────────────────────────────

    [Fact]
    public void StructuredBodySite_Slot_Exists() =>
        Assert.NotNull(FindSlot("at0039"));

    [Fact]
    public void StructuredBodySite_Slot_HasInclude()
    {
        var slot = FindSlot("at0039")!;
        Assert.NotEmpty(slot.Includes);
    }

    [Fact]
    public void SpecificDetails_Slot_HasWildcardInclude()
    {
        // allow_archetype CLUSTER[at0043] include archetype_id/value matches {/.*/}
        var slot = FindSlot("at0043")!;
        Assert.NotEmpty(slot.Includes);
        Assert.Contains(".*", slot.Includes);
    }

    [Fact]
    public void Status_Slot_HasSpecificInclude()
    {
        var slot = FindSlot("at0046")!;
        Assert.NotEmpty(slot.Includes);
        // include contains openEHR-EHR-CLUSTER.problem_qualifier pattern
        Assert.Contains(slot.Includes, r => r.Contains("problem_qualifier"));
    }

    // ── protocol section ──────────────────────────────────────────────────────

    [Fact]
    public void Protocol_Child_IsItemTree()
    {
        var protocol = Def.GetAttribute("protocol")!;
        var tree     = protocol.Children.OfType<CComplexObject>().FirstOrDefault();
        Assert.NotNull(tree);
        Assert.Equal("ITEM_TREE", tree.RmTypeName);
        Assert.Equal("at0032",    tree.NodeId);
    }

    [Fact]
    public void Protocol_LastUpdated_Element_Exists() =>
        Assert.NotNull(FindElement("at0070"));

    [Fact]
    public void Protocol_Extension_Slot_HasWildcardInclude()
    {
        var slot = FindSlot("at0071")!;
        Assert.NotEmpty(slot.Includes);
        Assert.Contains(".*", slot.Includes);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static CAttribute GetItemsAttribute()
    {
        var dataTree = Def.GetAttribute("data")!.Children.OfType<CComplexObject>().First();
        return dataTree.GetAttribute("items")!;
    }

    private static CComplexObject? FindElement(string nodeId) =>
        AomHelpers.FindElement(Def, nodeId);

    private static ArchetypeSlot? FindSlot(string nodeId) =>
        AomHelpers.FindSlot(Def, nodeId);
}
