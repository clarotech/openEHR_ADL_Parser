# openEHR ADL 1.4 Parser

A C# library that parses **ADL 1.4** (Archetype Definition Language) files into a fully-typed **Archetype Object Model (AOM)**. It targets openEHR archetypes and produces strongly-typed .NET objects for every section of an ADL file — identity, language, description, terminology, and the cADL constraint definition tree.

---

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Usage](#usage)
  - [Parsing an Archetype](#parsing-an-archetype)
  - [Archetype Identity](#archetype-identity)
  - [Metadata](#metadata)
  - [Language & Translations](#language--translations)
  - [Description](#description)
  - [Terminology](#terminology)
  - [Definition (cADL Constraint Tree)](#definition-cadl-constraint-tree)
- [AOM Model Reference](#aom-model-reference)
  - [Top-level Model](#top-level-model)
  - [Constraint Types](#constraint-types)
- [ADL Sections Parsed](#adl-sections-parsed)
- [Test Fixtures](#test-fixtures)
- [Dependencies](#dependencies)
- [License](#license)

---

## Overview

openEHR archetypes are defined in ADL (Archetype Definition Language), a domain-specific language for constraining clinical data models. ADL files contain several sections:

- A **header** with the archetype identifier and version metadata
- A **language** section listing available translations
- A **description** section with human-readable documentation
- A **terminology** section mapping internal at-codes to text definitions and external terminology bindings
- A **definition** section containing the cADL constraint tree — the structural model of the archetype

This library parses all of those sections and returns a single `Archetype` object that gives typed access to every element.

---

## Architecture

Parsing is a two-pass process:

1. **First pass — ADL structure** — ANTLR4 (using the official [openEHR-antlr4](https://github.com/openEHR/openEHR-antlr4) grammar) tokenises and splits the ADL file into its named sections, returning each section's raw text.

2. **Second pass — section parsing** — Each section is re-parsed:
   - Language, description and terminology sections use the ODIN grammar (a subset of ADL for structured metadata).
   - The definition section uses the cADL grammar for constraint expressions.
   - Hand-written mappers (`OdinReader`, `CadlMapper`) convert parse trees into the typed AOM model.

```
ADL file
   │
   ▼
Adl14Lexer / Adl14Parser   (ANTLR4 — splits into sections)
   │
   ├─ language / description / terminology ──► OdinParser ──► OdinReader ──► typed models
   │
   └─ definition ──────────────────────────► Cadl14Parser ──► CadlMapper ──► CComplexObject tree
```

The ANTLR4 grammar files are sourced from the `openEHR-antlr4` git submodule and compiled to C# at build time by `Antlr4BuildTasks`.

---

## Project Structure

```
AdlParser/
├── openEHR-antlr4/                  # git submodule — openEHR ANTLR4 grammars
└── ParserAdl2/
    ├── ParserAdl2.slnx              # Solution file
    ├── ParserAdl2/                  # Main library
    │   ├── ArchetypeParser.cs       # Entry point — Parse(string adlText)
    │   ├── Program.cs               # Example CLI usage
    │   ├── Mapping/
    │   │   ├── OdinReader.cs        # Extracts typed values from ODIN parse trees
    │   │   └── CadlMapper.cs        # Maps cADL parse tree to AOM constraint objects
    │   ├── Models/
    │   │   ├── Aom/
    │   │   │   ├── Archetype.cs
    │   │   │   ├── ArchetypeId.cs
    │   │   │   ├── ArchetypeMetaData.cs
    │   │   │   ├── Language/
    │   │   │   ├── Description/
    │   │   │   ├── Terminology/
    │   │   │   └── Constraint/      # CObject hierarchy (~27 CDv* types)
    │   │   └── Support/
    │   │       └── TerminologyCode.cs
    │   └── TestAdl/                 # Embedded ADL fixture files
    └── ParserAdl2.Tests/            # xUnit test suite
        ├── Aom/                     # Section-level tests
        └── Aom/Constraint/          # Per data-type constraint tests
```

---

## Prerequisites

- **.NET 10.0 SDK**
- The `openEHR-antlr4` submodule must be initialised:

```bash
git submodule update --init --recursive
```

---

## Getting Started

Clone the repository and restore dependencies:

```bash
git clone https://github.com/clarotech/openEHR_ADL_Parser.git
cd openEHR_ADL_Parser
git submodule update --init --recursive
cd ParserAdl2
dotnet build
dotnet test
```

---

## Usage

### Parsing an Archetype

```csharp
using Clarotech.openEHR.ADL2;

string adlText = File.ReadAllText("openEHR-EHR-OBSERVATION.blood_pressure.v2.adl");
Archetype archetype = ArchetypeParser.Parse(adlText);
```

`ArchetypeParser.Parse` accepts any ADL 1.4 string and returns a fully populated `Archetype`.

---

### Archetype Identity

```csharp
ArchetypeId id = archetype.Id;

Console.WriteLine(id.FullId);        // openEHR-EHR-OBSERVATION.blood_pressure.v2
Console.WriteLine(id.RmPublisher);   // openEHR
Console.WriteLine(id.RmPackage);     // EHR
Console.WriteLine(id.RmClass);       // OBSERVATION
Console.WriteLine(id.ConceptName);   // blood_pressure
Console.WriteLine(id.VersionId);     // v2
```

---

### Metadata

```csharp
ArchetypeMetaData meta = archetype.MetaData;

Console.WriteLine(meta.AdlVersion);  // 1.4
Console.WriteLine(meta.Uid);         // 36d47367-9cf6-4571-8998-b2a4e159a5db

// Any non-standard key/value pairs from the header parenthesis block
foreach (var (key, value) in meta.OtherItems)
    Console.WriteLine($"{key} = {value}");
```

---

### Language & Translations

```csharp
ArchetypeLanguage lang = archetype.Language;

Console.WriteLine(lang.OriginalLanguage);  // [ISO_639-1::en]

foreach (var (code, translation) in lang.Translations)
{
    Console.WriteLine($"[{code}] {translation.AuthorName}");
    Console.WriteLine($"       {translation.AuthorOrganisation}");
    Console.WriteLine($"       accreditation: {translation.Accreditation}");
}
```

---

### Description

```csharp
ArchetypeDescription desc = archetype.Description;

Console.WriteLine(desc.AuthorName);           // Suzanna Thunder
Console.WriteLine(desc.AuthorOrganisation);   // NEHTA
Console.WriteLine(desc.AuthorEmail);          // clinicalinfo@nehta.gov.au
Console.WriteLine(desc.LifecycleState);       // published

// Well-known other_details fields
Console.WriteLine(desc.Licence);
Console.WriteLine(desc.CustodianOrganisation);
Console.WriteLine(desc.Revision);

// Per-language description blocks
if (desc.Details.TryGetValue("en", out ArchetypeDescriptionItem? en))
{
    Console.WriteLine(en.Purpose);
    Console.WriteLine(en.Use);
    Console.WriteLine(en.Misuse);
    Console.WriteLine(en.Copyright);
    Console.WriteLine(string.Join(", ", en.Keywords));
}
```

---

### Terminology

```csharp
ArchetypeTerminology term = archetype.Terminology;

// External terminology names referenced in term_bindings
Console.WriteLine(string.Join(", ", term.TerminologiesAvailable));  // SNOMED-CT, LOINC, ...

// Look up a term definition (defaults to "en")
TermDefinition? at0000 = term.GetTermDefinition("at0000");
Console.WriteLine(at0000?.Text);         // Blood pressure
Console.WriteLine(at0000?.Description);  // The local measurement of arterial blood pressure...

// Look up in a specific language
TermDefinition? de = term.GetTermDefinition("at0000", "de");

// Iterate all term definitions
foreach (var (lang, terms) in term.TermDefinitions)
    foreach (var (code, def) in terms)
        Console.WriteLine($"[{lang}] {code}: {def.Text}");

// Term bindings to external terminologies
if (term.TermBindings.TryGetValue("SNOMED-CT", out var bindings))
    foreach (var (code, binding) in bindings)
        Console.WriteLine($"{binding.Code} → {binding.Target}");
```

---

### Definition (cADL Constraint Tree)

The `Definition` property contains the root of the constraint tree as a `CComplexObject`. Every node in the tree is a `CObject` subtype; structural nodes are `CComplexObject` instances containing `CAttribute` children, which in turn contain typed constraint leaf nodes.

```csharp
CComplexObject root = archetype.Definition;

Console.WriteLine(root.RmTypeName);  // OBSERVATION
Console.WriteLine(root.NodeId);      // at0000

// Iterate top-level attributes
foreach (CAttribute attr in root.Attributes)
{
    Console.WriteLine($"  {attr.RmAttributeName}");

    foreach (CObject child in attr.Children)
    {
        Console.WriteLine($"    [{child.RmTypeName}] {child.NodeId}");

        if (child is CComplexObject complexChild)
        {
            // Recurse into nested structure
        }
    }
}

// Find a node by at-code using AomHelpers
CComplexObject? element = AomHelpers.FindElement(root, "at0004");

// Inspect a leaf constraint
CAttribute? valueAttr = element?.GetAttribute("value");
CDvQuantity? qty = valueAttr?.Children.OfType<CDvQuantity>().FirstOrDefault();

// Navigate a slot
CAttribute? dataAttr = root.GetAttribute("data");
ArchetypeSlot? slot = dataAttr?.Children.OfType<ArchetypeSlot>().FirstOrDefault();

// Follow an internal reference
ArchetypeInternalRef? internalRef = valueAttr?.Children
    .OfType<ArchetypeInternalRef>()
    .FirstOrDefault();
Console.WriteLine(internalRef?.TargetPath);  // e.g., /data[at0001]/events[at0006]/data[at0003]/items[at0004]/value
```

---

## AOM Model Reference

### Top-level Model

| Class | Description |
|---|---|
| `Archetype` | Root model — holds all sections |
| `ArchetypeId` | Structured archetype identifier |
| `ArchetypeMetaData` | Header metadata (adl_version, uid, flags) |
| `ArchetypeLanguage` | Original language and translations |
| `TranslationDetails` | Per-language author and accreditation info |
| `ArchetypeDescription` | Human-readable documentation |
| `ArchetypeDescriptionItem` | Per-language purpose, use, misuse, keywords |
| `ArchetypeTerminology` | Term definitions and terminology bindings |
| `TermDefinition` | at-code → text, description, comment |
| `TermBinding` | at-code → external terminology target |
| `TerminologyCode` | A `[terminologyId::codeString]` pair |

### Constraint Types

| Class | RM Type | Description |
|---|---|---|
| `CComplexObject` | Any complex RM type | Structural node with attributes |
| `CSingleAttribute` | — | Single-valued attribute |
| `CMultipleAttribute` | — | Container attribute with cardinality |
| `ArchetypeSlot` | ARCHETYPED | Slot allowing child archetypes |
| `ArchetypeInternalRef` | — | Reference to another node in the same archetype |
| `CDvText` | DV_TEXT | Text constraint (pattern or value list) |
| `CDvCodedText` | DV_CODED_TEXT | Coded text with terminology binding |
| `CDvBoolean` | DV_BOOLEAN | Boolean constraint |
| `CDvCount` | DV_COUNT | Integer count constraint |
| `CDvQuantity` | DV_QUANTITY | Physical quantity with units |
| `CDvOrdinal` | DV_ORDINAL | Ordered set of coded values |
| `CDvScale` | DV_SCALE | Numeric scale constraint |
| `CDvProportion` | DV_PROPORTION | Ratio constraint |
| `CDvDate` | DV_DATE | Date constraint |
| `CDvTime` | DV_TIME | Time constraint |
| `CDvDateTime` | DV_DATE_TIME | Date-time constraint |
| `CDvDuration` | DV_DURATION | Duration constraint |
| `CDvUri` | DV_URI | URI constraint |
| `CDvEhrUri` | DV_EHR_URI | EHR-specific URI constraint |
| `CDvIdentifier` | DV_IDENTIFIER | Identifier constraint |
| `CDvMultimedia` | DV_MULTIMEDIA | Multimedia constraint |
| `CDvParsable` | DV_PARSABLE | Parsable content constraint |
| `CDvParagraph` | DV_PARAGRAPH | Paragraph constraint |
| `CDvState` | DV_STATE | State machine constraint |
| `IntervalOfInt` | — | Integer interval `lower..upper` |
| `IntervalOfReal` | — | Real interval `lower..upper` |
| `Cardinality` | — | Container cardinality (ordered, unique, interval) |

---

## ADL Sections Parsed

| ADL Section | Format | Model |
|---|---|---|
| Archetype header & identifier | ADL | `ArchetypeId`, `ArchetypeMetaData` |
| `concept` | ADL | `Archetype.ConceptCode` |
| `language` | ODIN | `ArchetypeLanguage`, `TranslationDetails` |
| `description` | ODIN | `ArchetypeDescription`, `ArchetypeDescriptionItem` |
| `terminology` | ODIN | `ArchetypeTerminology`, `TermDefinition`, `TermBinding` |
| `definition` | cADL | `CComplexObject` tree of `CObject` subtypes |

---

## Test Fixtures

The test project includes the following embedded ADL 1.4 archetypes sourced from the openEHR Clinical Knowledge Manager:

| Archetype | Description |
|---|---|
| `openEHR-EHR-OBSERVATION.blood_pressure.v2.adl` | Blood pressure measurement — complex archetype with 14 translations, DV_QUANTITY and DV_CODED_TEXT constraints, SNOMED-CT bindings |
| `openEHR-EHR-EVALUATION.problem_diagnosis.v1.adl` | Problem/diagnosis record with 14 translations |
| `openEHR-EHR-OBSERVATION.age_assertion.v1.adl` | Age assertion |
| `openEHR-EHR-OBSERVATION.braden_scale_q.v0.adl` | Braden pressure ulcer risk scale (draft) |
| `openEHR-EHR-OBSERVATION.glasgow_outcome_scale_extended.v1.adl` | Glasgow outcome scale with DV_SCALE constraints |

---

## Dependencies

| Package | Version | Purpose |
|---|---|---|
| `Antlr4.Runtime.Standard` | 4.13.1 | ANTLR4 runtime for generated parsers |
| `Antlr4BuildTasks` | 12.14.0 | Compiles `.g4` grammar files to C# at build time |
| xUnit | — | Test framework (test project only) |

Grammar files are provided by the [openEHR/openEHR-antlr4](https://github.com/openEHR/openEHR-antlr4) repository, included as a git submodule.

---

## License

Licensed under the [Apache License, Version 2.0](LICENSE).
