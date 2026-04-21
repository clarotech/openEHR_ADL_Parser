using Antlr4.Runtime;

namespace Clarotech.openEHR.ADL2;

public static class ArchetypeParser
{
    /// <summary>
    /// Two-pass parse of an ADL 1.4 archetype text.
    /// Pass 1: Adl14Lexer/Parser splits the file into section token blobs.
    /// Pass 2: OdinLexer/Parser re-parses each ODIN section into a typed tree.
    /// </summary>
    public static Archetype Parse(string adlText)
    {
        // Strip UTF-8 BOM if present — ANTLR lexer does not recognise it.
        adlText = adlText.TrimStart('\uFEFF');

        // ── Pass 1: ADL structural parse ────────────────────────────────────
        var adlStream = new AntlrInputStream(adlText);
        var adlLexer  = new Adl14Lexer(adlStream);
        var adlTokens = new CommonTokenStream(adlLexer);
        var adlParser = new Adl14Parser(adlTokens);

        var arch = adlParser.adlObject().authoredArchetype();

        // ── Pass 2: re-parse ODIN section blobs ─────────────────────────────
        var langOdin = ParseOdin(arch.languageSection().odinText());
        var descOdin = ParseOdin(arch.descriptionSection().odinText());
        var termOdin = ParseOdin(arch.terminologySection().odinText());

        return new Archetype
        {
            Id            = ArchetypeId.Parse(arch.header().ARCHETYPE_REF().GetText()),
            MetaData      = MapMetaData(arch.header()),
            ConceptCode   = arch.conceptSection().ADL14_AT_CODE().GetText(),
            Language      = MapLanguage(langOdin),
            Description  = MapDescription(descOdin),
            Terminology  = MapTerminology(termOdin),
            Definition    = CadlMapper.MapComplexObject(
                                ParseCadl(arch.definitionSection().cadlText())),
        };
    }

    // ── section mappers ──────────────────────────────────────────────────────

    private static ArchetypeMetaData MapMetaData(Adl14Parser.HeaderContext header)
    {
        var md = header.metaData();
        if (md == null)
            return new ArchetypeMetaData();

        var others = new Dictionary<string, string>();
        var flags  = new HashSet<string>();
        string? adlVersion = null, uid = null;

        foreach (var item in md.metaDataItem())
        {
            var valItem = item.metaDataValueItem();
            if (valItem != null)
            {
                var key = valItem.ALPHANUM_ID().GetText();
                var val = valItem.metaDataItemValue().GetText();
                switch (key)
                {
                    case "adl_version": adlVersion = val; break;
                    case "uid":         uid        = val; break;
                    default:            others[key] = val; break;
                }
            }
            else
            {
                flags.Add(item.metaDataFlag().ALPHANUM_ID().GetText());
            }
        }

        return new ArchetypeMetaData
        {
            AdlVersion = adlVersion,
            Uid        = uid,
            OtherItems = others,
            Flags      = flags,
        };
    }

    private static ArchetypeLanguage MapLanguage(OdinParser.OdinObjectContext ctx)
    {
        var originalLanguage = OdinReader.TermCode(OdinReader.Attr(ctx, "original_language"))
            ?? throw new InvalidDataException("language section missing 'original_language'");

        var translations = new Dictionary<string, TranslationDetails>();

        var transBlock = OdinReader.Attr(ctx, "translations");
        foreach (var (langCode, langBlock) in OdinReader.KeyedBlocks(transBlock))
        {
            translations[langCode] = new TranslationDetails
            {
                Language      = OdinReader.TermCode(OdinReader.Attr(langBlock, "language"))
                                ?? throw new InvalidDataException(
                                       $"Translation '{langCode}' missing 'language'"),
                Author        = OdinReader.StringMap(OdinReader.Attr(langBlock, "author")),
                Accreditation = OdinReader.String(OdinReader.Attr(langBlock, "accreditation")),
            };
        }

        return new ArchetypeLanguage
        {
            OriginalLanguage = originalLanguage,
            Translations     = translations,
        };
    }

    private static ArchetypeDescription MapDescription(OdinParser.OdinObjectContext ctx)
    {
        var details = new Dictionary<string, ArchetypeDescriptionItem>();

        foreach (var (langCode, langBlock) in OdinReader.KeyedBlocks(OdinReader.Attr(ctx, "details")))
        {
            details[langCode] = new ArchetypeDescriptionItem
            {
                Language  = OdinReader.TermCode(OdinReader.Attr(langBlock, "language"))
                            ?? throw new InvalidDataException(
                                   $"Description detail '{langCode}' missing 'language'"),
                Purpose   = OdinReader.String(OdinReader.Attr(langBlock, "purpose")) ?? "",
                Use       = OdinReader.String(OdinReader.Attr(langBlock, "use")),
                Keywords  = OdinReader.StringList(OdinReader.Attr(langBlock, "keywords")),
                Misuse    = OdinReader.String(OdinReader.Attr(langBlock, "misuse")),
                Copyright = OdinReader.String(OdinReader.Attr(langBlock, "copyright")),
            };
        }

        return new ArchetypeDescription
        {
            OriginalAuthor    = OdinReader.StringMap(OdinReader.Attr(ctx, "original_author")),
            OtherContributors = OdinReader.StringList(OdinReader.Attr(ctx, "other_contributors")),
            Details           = details,
            LifecycleState    = OdinReader.String(OdinReader.Attr(ctx, "lifecycle_state")) ?? "",
            OtherDetails      = OdinReader.StringMap(OdinReader.Attr(ctx, "other_details")),
        };
    }

    private static ArchetypeTerminology MapTerminology(OdinParser.OdinObjectContext ctx)
    {
        // term_definitions: [language] → items → [at-code] → { text, description, comment }
        var termDefs = new Dictionary<string, IReadOnlyDictionary<string, TermDefinition>>();
        foreach (var (lang, langBlock) in OdinReader.KeyedBlocks(OdinReader.Attr(ctx, "term_definitions")))
        {
            var items = new Dictionary<string, TermDefinition>();
            foreach (var (code, itemBlock) in OdinReader.KeyedBlocks(OdinReader.Attr(langBlock, "items")))
            {
                items[code] = new TermDefinition
                {
                    Code        = code,
                    Text        = OdinReader.String(OdinReader.Attr(itemBlock, "text"))        ?? "",
                    Description = OdinReader.String(OdinReader.Attr(itemBlock, "description")) ?? "",
                    Comment     = OdinReader.String(OdinReader.Attr(itemBlock, "comment")),
                };
            }
            termDefs[lang] = items;
        }

        // term_bindings: [terminology] → items → [at-code] → <TerminologyCode>
        var termBindings = new Dictionary<string, IReadOnlyDictionary<string, TermBinding>>();
        foreach (var (terminology, termBlock) in OdinReader.KeyedBlocks(OdinReader.Attr(ctx, "term_bindings")))
        {
            var items = new Dictionary<string, TermBinding>();
            foreach (var (code, valueBlock) in OdinReader.KeyedBlocks(OdinReader.Attr(termBlock, "items")))
            {
                var target = OdinReader.TermCode(valueBlock);
                if (target != null)
                    items[code] = new TermBinding { Code = code, Target = target };
            }
            termBindings[terminology] = items;
        }

        return new ArchetypeTerminology
        {
            TerminologiesAvailable = OdinReader.StringList(OdinReader.Attr(ctx, "terminologies_available")),
            TermDefinitions        = termDefs,
            TermBindings           = termBindings,
        };
    }

    // ── cADL second-pass helper ──────────────────────────────────────────────

    private static Cadl14Parser.CComplexObjectContext ParseCadl(Adl14Parser.CadlTextContext cadlText)
    {
        var raw    = string.Concat(cadlText.CADL_LINE().Select(t => t.GetText()));
        var stream = new AntlrInputStream(raw);
        var lexer  = new Cadl14Lexer(stream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new Cadl14Parser(tokens);
        return parser.cComplexObject();
    }

    // ── ODIN second-pass helper ──────────────────────────────────────────────

    // Each ODIN_LINE token already includes its trailing newline, so
    // concatenating them reconstructs the original section text faithfully.
    private static OdinParser.OdinObjectContext ParseOdin(Adl14Parser.OdinTextContext odinText)
    {
        var raw    = string.Concat(odinText.ODIN_LINE().Select(t => t.GetText()));
        var stream = new AntlrInputStream(raw);
        var lexer  = new OdinLexer(stream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new OdinParser(tokens);
        return parser.odinObject();
    }
}
