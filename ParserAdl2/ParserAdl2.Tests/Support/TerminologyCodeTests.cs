using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests.Models;

public class TerminologyCodeTests
{
    [Theory]
    [InlineData("[ISO_639-1::en]",    "ISO_639-1", "en")]
    [InlineData("[ISO_639-1::pt-br]", "ISO_639-1", "pt-br")]
    [InlineData("[openehr::125]",     "openehr",   "125")]
    [InlineData("[SNOMED-CT::22298006]", "SNOMED-CT", "22298006")]
    public void Parse_BracketedCode_ExtractsComponents(
        string raw, string expectedTermId, string expectedCode)
    {
        var tc = TerminologyCode.Parse(raw);

        Assert.Equal(expectedTermId, tc.TerminologyId);
        Assert.Equal(expectedCode,   tc.CodeString);
    }

    [Theory]
    [InlineData("ISO_639-1::en",  "ISO_639-1", "en")]
    [InlineData("openehr::125",   "openehr",   "125")]
    public void Parse_UnbracketedCode_ExtractsComponents(
        string raw, string expectedTermId, string expectedCode)
    {
        var tc = TerminologyCode.Parse(raw);

        Assert.Equal(expectedTermId, tc.TerminologyId);
        Assert.Equal(expectedCode,   tc.CodeString);
    }

    [Fact]
    public void ToString_ReturnsCanonicalBracketedForm()
    {
        var tc = new TerminologyCode("ISO_639-1", "en");
        Assert.Equal("[ISO_639-1::en]", tc.ToString());
    }

    [Fact]
    public void Parse_MissingSeparator_Throws()
    {
        Assert.Throws<FormatException>(() => TerminologyCode.Parse("[ISO_639-1-en]"));
    }
}
