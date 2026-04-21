using System.Reflection;
using Antlr4.Runtime.Tree;
using Clarotech.openEHR.ADL2;

// Load the trimmed blood pressure archetype embedded in the assembly.
var assembly     = Assembly.GetExecutingAssembly();
var resourceName = assembly.GetManifestResourceNames()
    .Single(n => n.EndsWith("openEHR-EHR-OBSERVATION.age_assertion.v1.adl"));

string adlText;
using (var stream = assembly.GetManifestResourceStream(resourceName)!)
using (var reader = new StreamReader(stream))
    adlText = reader.ReadToEnd();

// Two-pass parse → typed AOM model.
var archetype = ArchetypeParser.Parse(adlText);

// ── Identity ─────────────────────────────────────────────────────────────────
Console.WriteLine($"Archetype    : {archetype.Id.FullId}");
Console.WriteLine($"  publisher  : {archetype.Id.RmPublisher}");
Console.WriteLine($"  package    : {archetype.Id.RmPackage}");
Console.WriteLine($"  class      : {archetype.Id.RmClass}");
Console.WriteLine($"  concept    : {archetype.Id.ConceptName}");
Console.WriteLine($"  version    : {archetype.Id.VersionId}");
Console.WriteLine();

// ── Metadata ──────────────────────────────────────────────────────────────────
Console.WriteLine($"ADL version  : {archetype.MetaData.AdlVersion}");
Console.WriteLine($"UID          : {archetype.MetaData.Uid}");
Console.WriteLine();

// ── Concept ───────────────────────────────────────────────────────────────────
Console.WriteLine($"Concept code : {archetype.ConceptCode}");
Console.WriteLine();

// ── Language ──────────────────────────────────────────────────────────────────
Console.WriteLine($"Original language : {archetype.Language.OriginalLanguage}");
if (archetype.Language.Translations.Any())
{
    Console.WriteLine($"Translations ({archetype.Language.Translations.Count}):");
    foreach (var (code, td) in archetype.Language.Translations)
    {
        var authorName = td.Author.GetValueOrDefault("name", "");
        Console.WriteLine($"  [{code}]  {td.Language}  — {authorName}");
    }
}
Console.WriteLine();

// ── Description ───────────────────────────────────────────────────────────────
var desc = archetype.Description;
Console.WriteLine($"Author       : {desc.AuthorName} ({desc.AuthorOrganisation})");
Console.WriteLine($"Author email : {desc.AuthorEmail}");
Console.WriteLine($"Author date  : {desc.AuthorDate}");
Console.WriteLine($"Lifecycle    : {desc.LifecycleState}");
Console.WriteLine($"Revision     : {desc.Revision}");
Console.WriteLine($"Contributors : {desc.OtherContributors.Count}");
Console.WriteLine($"Details ({desc.Details.Count} languages):");
if (desc.Details.TryGetValue("en", out var en))
{
    Console.WriteLine($"  [en] purpose  : {en.Purpose[..Math.Min(80, en.Purpose.Length)]}...");
    Console.WriteLine($"  [en] keywords : {string.Join(", ", en.Keywords)}");
}
Console.WriteLine();

// ── Terminology ───────────────────────────────────────────────────────────────
var term = archetype.Terminology;
Console.WriteLine($"Terminologies : {string.Join(", ", term.TerminologiesAvailable)}");
Console.WriteLine($"Term def languages : {string.Join(", ", term.TermDefinitions.Keys)}");
Console.WriteLine($"Term binding sets  : {string.Join(", ", term.TermBindings.Keys)}");
Console.WriteLine();

var at0000 = term.GetTermDefinition("at0000");
Console.WriteLine($"at0000 text : {at0000?.Text}");
Console.WriteLine($"at0000 desc : {at0000?.Description}");
Console.WriteLine($"at0000 comment : {at0000?.Comment}");
Console.WriteLine();

if (term.TermBindings.TryGetValue("SNOMED-CT", out var snomedBindings))
{
    Console.WriteLine($"SNOMED-CT bindings ({snomedBindings.Count}):");
    foreach (var b in snomedBindings.Values)
        Console.WriteLine($"  {b.Code} → {b.Target}");
}

// ── Definition ────────────────────────────────────────────────────────────────
var def = archetype.Definition;
Console.WriteLine($"Definition   : {def.RmTypeName}[{def.NodeId}]");
Console.WriteLine($"  Attributes : {string.Join(", ", def.Attributes.Select(a => a.RmAttributeName))}");
