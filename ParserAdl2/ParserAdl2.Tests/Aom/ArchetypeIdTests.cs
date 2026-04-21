using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests.Models;

public class ArchetypeIdTests
{
    [Fact]
    public void Parse_StandardId_ExtractsAllComponents()
    {
        var id = ArchetypeId.Parse("openEHR-EHR-OBSERVATION.blood_pressure.v2");

        Assert.Equal("openEHR-EHR-OBSERVATION.blood_pressure.v2", id.FullId);
        Assert.Equal("openEHR",        id.RmPublisher);
        Assert.Equal("EHR",            id.RmPackage);
        Assert.Equal("OBSERVATION",    id.RmClass);
        Assert.Equal("blood_pressure", id.ConceptName);
        Assert.Equal("v2",             id.VersionId);
    }

    [Fact]
    public void Parse_ClusterArchetype_ExtractsAllComponents()
    {
        var id = ArchetypeId.Parse("openEHR-EHR-CLUSTER.address.v0");

        Assert.Equal("CLUSTER",  id.RmClass);
        Assert.Equal("address",  id.ConceptName);
        Assert.Equal("v0",       id.VersionId);
    }

    [Fact]
    public void ToString_ReturnsFullId()
    {
        var id = ArchetypeId.Parse("openEHR-EHR-OBSERVATION.blood_pressure.v2");
        Assert.Equal("openEHR-EHR-OBSERVATION.blood_pressure.v2", id.ToString());
    }

    [Fact]
    public void Parse_MissingDot_Throws()
    {
        Assert.Throws<FormatException>(() => ArchetypeId.Parse("openEHR-EHR-OBSERVATION"));
    }

    [Fact]
    public void Parse_WrongRmPartCount_Throws()
    {
        Assert.Throws<FormatException>(() => ArchetypeId.Parse("openEHR-EHR.blood_pressure.v2"));
    }
}
