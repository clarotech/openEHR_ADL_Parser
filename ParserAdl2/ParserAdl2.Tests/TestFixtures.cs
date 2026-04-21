using Clarotech.openEHR.ADL2;

namespace ParserAdl2.Tests;

internal static class TestFixtures
{
    public static readonly Archetype BloodPressure    = Load("blood_pressure.v2.adl");
    public static readonly Archetype ProblemDiagnosis = Load("problem_diagnosis.v1.adl");
    public static readonly Archetype AgeAssertion          = Load("age_assertion.v1.adl");
    public static readonly Archetype BradenScale           = Load("braden_scale_q.v0.adl");
    public static readonly Archetype GlasgowOutcomeScale   = Load("glasgow_outcome_scale_extended.v1.adl");

    private static Archetype Load(string suffix)
    {
        var asm  = typeof(ArchetypeParser).Assembly;
        var name = asm.GetManifestResourceNames().Single(n => n.EndsWith(suffix));
        using var stream = asm.GetManifestResourceStream(name)!;
        using var reader = new StreamReader(stream);
        return ArchetypeParser.Parse(reader.ReadToEnd());
    }
}
