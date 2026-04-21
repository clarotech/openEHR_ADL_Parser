using Antlr4.Runtime;

namespace Clarotech.openEHR.ADL2;

/// <summary>
/// Maps a Cadl14Parser parse tree to typed AOM model objects.
/// </summary>
internal static class CadlMapper
{
    public static CComplexObject MapComplexObject(Cadl14Parser.CComplexObjectContext ctx)
    {
        var attrs = ctx.cComplexObjectDef() is { } def
            ? def.cAttribute().Select(MapAttribute).ToList()
            : (List<CAttribute>)[];

        return new CComplexObject
        {
            RmTypeName   = ctx.rmTypeId().GetText(),
            NodeId       = ctx.nodeId()?.adl14_at_code().GetText() ?? "",
            Occurrences  = MapOccurrences(ctx.cOccurrences()),
            Attributes   = attrs,
        };
    }

    // ── Attribute ────────────────────────────────────────────────────────────

    private static CAttribute MapAttribute(Cadl14Parser.CAttributeContext ctx)
    {
        var name      = ctx.rmAttributeId().GetText();
        var existence = MapExistence(ctx.cExistence());

        List<CObject> children;
        if (ctx.cAttributeDef() is { } def)
            children = def.cRegularObject().Select(MapRegularObject).ToList();
        else if (ctx.cInlinePrimitiveObject() is { } inlinePrim)
            children = [MapDvFromInlinePrimitive("", "", null, inlinePrim)];
        else
            children = [];

        var cardinality = ctx.cCardinality() is { } cc ? MapCardinality(cc) : null;

        if (cardinality != null)
        {
            return new CMultipleAttribute
            {
                RmAttributeName = name,
                Existence       = existence,
                Cardinality     = cardinality,
                Children        = children,
            };
        }
        return new CSingleAttribute
        {
            RmAttributeName = name,
            Existence       = existence,
            Children        = children,
        };
    }

    // ── Regular object dispatch ───────────────────────────────────────────────

    private static CObject MapRegularObject(Cadl14Parser.CRegularObjectContext ctx)
    {
        if (ctx.cComplexObject()          is { } cc) return MapComplexObjectOrDv(cc);
        if (ctx.archetypeSlot()           is { } sl) return MapArchetypeSlot(sl);
        if (ctx.cComplexObjectProxy()     is { } pr) return MapInternalRef(pr);
        if (ctx.domainSpecificExtension() is { } ds) return MapDomainSpecificExtension(ds);
        if (ctx.cOrdinal()                is { } co) return MapDvOrdinal(co);
        if (ctx.cRegularPrimitiveObject() is { } rp)
        {
            var rmTypeName = rp.rmTypeId().GetText();
            var nodeId     = rp.nodeId()?.adl14_at_code().GetText() ?? "";
            var occ        = MapOccurrences(rp.cOccurrences());

            if (rp.cInlinePrimitiveObject() is { } inlinePrim)
                return MapDvFromInlinePrimitive(rmTypeName, nodeId, occ, inlinePrim);

            return DvStub(rmTypeName, nodeId, occ)
                ?? new CComplexObject { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ, Attributes = [] };
        }
        throw new InvalidDataException($"Unhandled cRegularObject: {ctx.GetText()}");
    }

    /// <summary>
    /// Routes a cComplexObject to a typed CDv* class when the rmTypeId is a known DV data type,
    /// otherwise falls through to the generic <see cref="MapComplexObject"/>.
    /// </summary>
    private static CObject MapComplexObjectOrDv(Cadl14Parser.CComplexObjectContext ctx)
    {
        var rmTypeName    = ctx.rmTypeId().GetText();
        var nodeId        = ctx.nodeId()?.adl14_at_code().GetText() ?? "";
        var occ           = MapOccurrences(ctx.cOccurrences());
        var unconstrained = ctx.cComplexObjectDef()?.SYM_ASTERISK() != null;

        switch (rmTypeName)
        {
            case "DV_TEXT":
                return new CDvText      { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ };
            case "DV_CODED_TEXT":
                return unconstrained
                    ? new CDvCodedText  { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ }
                    : MapDvCodedText(rmTypeName, nodeId, occ, ctx);
            case "DV_BOOLEAN":
                return new CDvBoolean  { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ };
            case "DV_DATE":
                return new CDvDate     { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ };
            case "DV_TIME":
                return new CDvTime     { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ };
            case "DV_DATE_TIME":
                return new CDvDateTime { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ };
            case "DV_DURATION":
                return unconstrained
                    ? new CDvDuration  { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ }
                    : MapDvDuration(rmTypeName, nodeId, occ, ctx);
            case "DV_COUNT":
                return unconstrained
                    ? new CDvCount     { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ }
                    : MapDvCount(rmTypeName, nodeId, occ, ctx);
            case "DV_ORDINAL":
                return new CDvOrdinal  { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ };
            case "DV_IDENTIFIER":
                return new CDvIdentifier  { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ };
            case "DV_PARAGRAPH":
                return new CDvParagraph   { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ };
            case "DV_STATE":
                return new CDvState       { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ };
            case "DV_SCALE":
                return new CDvScale       { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ };
            case "DV_PROPORTION":
                return new CDvProportion  { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ };
            case "DV_MULTIMEDIA":
                return new CDvMultimedia  { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ };
            case "DV_PARSABLE":
                return new CDvParsable    { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ };
            case "DV_URI":
                return new CDvUri         { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ };
            case "DV_EHR_URI":
                return new CDvEhrUri      { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ };
            default:
                // Structural type (OBSERVATION, HISTORY, ELEMENT, etc.)
                if (unconstrained)
                    return new CComplexObject { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ, Attributes = [] };
                return MapComplexObject(ctx);
        }
    }

    // ── Typed DV mappers ─────────────────────────────────────────────────────

    private static CDvCodedText MapDvCodedText(
        string rmTypeName, string nodeId, IntervalOfInt? occ,
        Cadl14Parser.CComplexObjectContext ctx)
    {
        CCodePhrase? definingCode = null;

        if (ctx.cComplexObjectDef() is { } def)
        {
            var dcAttr = def.cAttribute()
                .FirstOrDefault(a => a.rmAttributeId().GetText() == "defining_code");

            if (dcAttr != null)
            {
                // Pattern: defining_code matches { [local:: ...] }  — cInlinePrimitiveObject
                if (dcAttr.cInlinePrimitiveObject()?.cTerminologyCode() is { } ctc)
                    definingCode = MapCCodePhrase(ctc);

                // Pattern: defining_code matches { <regular_object> } — cAttributeDef
                else if (dcAttr.cAttributeDef() is { } ad)
                {
                    var ro = ad.cRegularObject().FirstOrDefault();
                    if (ro?.cRegularPrimitiveObject()?.cInlinePrimitiveObject()
                            ?.cTerminologyCode() is { } ctc2)
                        definingCode = MapCCodePhrase(ctc2);
                }
            }
        }

        return new CDvCodedText
        {
            RmTypeName   = rmTypeName,
            NodeId       = nodeId,
            Occurrences  = occ,
            DefiningCode = definingCode,
        };
    }

    private static CDvDuration MapDvDuration(
        string rmTypeName, string nodeId, IntervalOfInt? occ,
        Cadl14Parser.CComplexObjectContext ctx)
    {
        string? pattern = null;
        string? range   = null;

        if (ctx.cComplexObjectDef() is { } def)
        {
            var valAttr = def.cAttribute()
                .FirstOrDefault(a => a.rmAttributeId().GetText() == "value");

            if (valAttr != null)
            {
                Cadl14Parser.CDurationContext? durCtx = null;

                // Path A: value matches { <inline_primitive> }
                durCtx ??= valAttr.cInlinePrimitiveObject()
                    ?.cInlineOrderedObject()?.cInlineDTemporalObject()?.cDuration();

                // Path B: value matches { cRegularPrimitiveObject }
                if (durCtx == null)
                {
                    var rp = valAttr.cAttributeDef()?.cRegularObject().FirstOrDefault()
                                    ?.cRegularPrimitiveObject();
                    durCtx = rp?.cInlinePrimitiveObject()
                        ?.cInlineOrderedObject()?.cInlineDTemporalObject()?.cDuration();
                }

                if (durCtx != null)
                {
                    pattern = durCtx.DURATION_CONSTRAINT_PATTERN()?.GetText();
                    // Capture the full text of the value constraint as the raw range
                    // (covers both specific values like PT24H and intervals like |>=PT0S|)
                    if (pattern == null)
                        range = durCtx.GetText();
                }
            }
        }

        return new CDvDuration
        {
            RmTypeName  = rmTypeName,
            NodeId      = nodeId,
            Occurrences = occ,
            Pattern     = pattern,
            Range       = string.IsNullOrEmpty(range) ? null : range,
        };
    }

    private static CDvCount MapDvCount(
        string rmTypeName, string nodeId, IntervalOfInt? occ,
        Cadl14Parser.CComplexObjectContext ctx)
    {
        IntervalOfInt? magnitude = null;

        if (ctx.cComplexObjectDef() is { } def)
        {
            var magAttr = def.cAttribute()
                .FirstOrDefault(a => a.rmAttributeId().GetText() == "magnitude");

            if (magAttr != null)
            {
                var ci = magAttr.cInlinePrimitiveObject()
                    ?.cInlineOrderedObject()?.cInteger();
                if (ci?.integerInterval() is { } ii)
                    magnitude = MapIntegerIntervalRange(ii.integerIntervalRange());
            }
        }

        return new CDvCount
        {
            RmTypeName  = rmTypeName,
            NodeId      = nodeId,
            Occurrences = occ,
            Magnitude   = magnitude,
        };
    }

    /// <summary>
    /// Maps a cRegularPrimitiveObject's inline primitive to the appropriate CDv* class.
    /// <paramref name="rmTypeName"/> may be empty when called from an attribute-level inline primitive.
    /// </summary>
    private static CObject MapDvFromInlinePrimitive(
        string rmTypeName, string nodeId, IntervalOfInt? occ,
        Cadl14Parser.CInlinePrimitiveObjectContext ctx)
    {
        if (ctx.cBoolean() is { } cb)
        {
            bool trueValid = false, falseValid = false;
            bool? assumed = null;
            if (cb.booleanValue() is { } bv)
            {
                trueValid  = bv.SYM_TRUE()  != null;
                falseValid = bv.SYM_FALSE() != null;
            }
            else if (cb.booleanValues() is { } bvs)
            {
                foreach (var v in bvs.booleanValue())
                {
                    if (v.SYM_TRUE()  != null) trueValid  = true;
                    if (v.SYM_FALSE() != null) falseValid = true;
                }
            }
            if (cb.assumedBooleanValue() is { } abv)
                assumed = abv.booleanValue().SYM_TRUE() != null;

            return new CDvBoolean
            {
                RmTypeName   = rmTypeName,
                NodeId       = nodeId,
                Occurrences  = occ,
                TrueValid    = trueValid,
                FalseValid   = falseValid,
                AssumedValue = assumed,
            };
        }

        if (ctx.cString() is { } cs)
        {
            string? pattern = null;
            List<string> list = [];
            string? assumedStr = null;
            if (cs.DELIMITED_REGEX() is { } rx)
                pattern = StripDelimitedRegex(rx.GetText());
            else if (cs.stringValues() is { } svs)
                list = svs.stringValue().Select(sv => StripStringQuotes(sv.GetText())).ToList();
            else if (cs.stringValue() is { } sv)
                list = [StripStringQuotes(sv.GetText())];
            if (cs.assumedStringValue() is { } asv)
                assumedStr = StripStringQuotes(asv.stringValue().GetText());

            return new CDvText
            {
                RmTypeName   = rmTypeName,
                NodeId       = nodeId,
                Occurrences  = occ,
                Pattern      = pattern,
                List         = list,
                AssumedValue = assumedStr,
            };
        }

        if (ctx.cTerminologyCode() is { } ctc)
        {
            return new CDvCodedText
            {
                RmTypeName   = rmTypeName,
                NodeId       = nodeId,
                Occurrences  = occ,
                DefiningCode = MapCCodePhrase(ctc),
            };
        }

        if (ctx.cInlineOrderedObject() is { } ordered)
        {
            if (ordered.cInteger() is { } ci)
            {
                IntervalOfInt? mag = null;
                int? assumedInt = null;
                if (ci.integerInterval() is { } ii)
                    mag = MapIntegerIntervalRange(ii.integerIntervalRange());
                else if (ci.integerValues() is { } ivs)
                    mag = new IntervalOfInt(ParseInt(ivs.integerValue()[0].GetText()), null);
                else if (ci.integerValue() is { } iv)
                    mag = new IntervalOfInt(ParseInt(iv.GetText()), ParseInt(iv.GetText()));
                if (ci.assumedIntegerValue() is { } aiv)
                    assumedInt = ParseInt(aiv.integerValue().GetText());

                return new CDvCount
                {
                    RmTypeName   = rmTypeName,
                    NodeId       = nodeId,
                    Occurrences  = occ,
                    Magnitude    = mag,
                    AssumedValue = assumedInt,
                };
            }

            if (ordered.cInlineDTemporalObject() is { } dt)
            {
                if (dt.cDate() is { } cd)
                    return new CDvDate
                    {
                        RmTypeName   = rmTypeName,
                        NodeId       = nodeId,
                        Occurrences  = occ,
                        Pattern      = cd.DATE_CONSTRAINT_PATTERN()?.GetText(),
                        AssumedValue = cd.assumedDateValue()?.dateValue().GetText(),
                    };

                if (dt.cTime() is { } ct)
                    return new CDvTime
                    {
                        RmTypeName   = rmTypeName,
                        NodeId       = nodeId,
                        Occurrences  = occ,
                        Pattern      = ct.TIME_CONSTRAINT_PATTERN()?.GetText(),
                        AssumedValue = ct.assumedTimeValue()?.timeValue().GetText(),
                    };

                if (dt.cDateTime() is { } cdt)
                    return new CDvDateTime
                    {
                        RmTypeName   = rmTypeName,
                        NodeId       = nodeId,
                        Occurrences  = occ,
                        Pattern      = cdt.DATE_TIME_CONSTRAINT_PATTERN()?.GetText(),
                        AssumedValue = cdt.assumedDateTimeValue()?.dateTimeValue().GetText(),
                    };

                if (dt.cDuration() is { } cdu)
                    return new CDvDuration
                    {
                        RmTypeName   = rmTypeName,
                        NodeId       = nodeId,
                        Occurrences  = occ,
                        Pattern      = cdu.DURATION_CONSTRAINT_PATTERN()?.GetText(),
                        AssumedValue = cdu.assumedDurationValue()?.durationValue().GetText(),
                    };
            }
        }

        // Fallback: unknown inline primitive
        return DvStub(rmTypeName, nodeId, occ)
            ?? new CComplexObject { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ, Attributes = [] };
    }

    // ── Archetype slot ───────────────────────────────────────────────────────

    private static ArchetypeSlot MapArchetypeSlot(Cadl14Parser.ArchetypeSlotContext ctx)
    {
        var includes = ctx.cIncludes()?.archetypeIdConstraint()
            .Select(c => StripDelimitedRegex(c.DELIMITED_REGEX().GetText()))
            .ToList() ?? [];
        var excludes = ctx.cExcludes()?.archetypeIdConstraint()
            .Select(c => StripDelimitedRegex(c.DELIMITED_REGEX().GetText()))
            .ToList() ?? [];

        return new ArchetypeSlot
        {
            RmTypeName  = ctx.rmTypeId().GetText(),
            NodeId      = ctx.nodeId().adl14_at_code().GetText(),
            Occurrences = MapOccurrences(ctx.cOccurrences()),
            Includes    = includes,
            Excludes    = excludes,
        };
    }

    private static string StripDelimitedRegex(string raw)
    {
        if (raw.StartsWith('/') && raw.EndsWith('/') && raw.Length >= 2)
            return raw[1..^1];
        return raw;
    }

    // ── Internal ref (use_node) ──────────────────────────────────────────────

    private static ArchetypeInternalRef MapInternalRef(Cadl14Parser.CComplexObjectProxyContext ctx)
    {
        return new ArchetypeInternalRef
        {
            RmTypeName  = ctx.rmTypeId().GetText(),
            NodeId      = "",
            Occurrences = MapOccurrences(ctx.cOccurrences()),
            TargetPath  = ctx.adlPath().GetText(),
        };
    }

    // ── Domain-specific extension (C_DV_QUANTITY etc.) ───────────────────────

    private static CObject MapDomainSpecificExtension(Cadl14Parser.DomainSpecificExtensionContext ctx)
    {
        var startToken = ctx.ODIN14_BLOCK_START().GetText();
        var rmType     = startToken.TrimStart('(').Split(')')[0];
        var rawOdin    = string.Concat(ctx.ODIN14_BLOCK_LINE().Select(t => t.GetText()));

        if (rmType == "C_DV_QUANTITY")
            return ParseDvQuantity(rmType, rawOdin);

        // Unknown domain extension — preserve raw ODIN inside a CDvQuantity-like stub
        // is not possible without a type; fall back to a bare CComplexObject.
        return new CComplexObject { RmTypeName = rmType, NodeId = "", Occurrences = null, Attributes = [] };
    }

    private static CDvQuantity ParseDvQuantity(string rmType, string rawOdin)
    {
        var stream = new AntlrInputStream(rawOdin);
        var lexer  = new OdinLexer(stream);
        var tokens = new CommonTokenStream(lexer);
        var parser = new OdinParser(tokens);
        var ctx    = parser.odinObject();

        var property    = OdinReader.TermCode(OdinReader.Attr(ctx, "property"));
        var listAttr    = OdinReader.Attr(ctx, "list");
        var assumedAttr = OdinReader.Attr(ctx, "assumed_value");

        var items = new List<CQuantityItem>();
        if (listAttr != null)
            foreach (var (_, itemBlock) in OdinReader.KeyedBlocks(listAttr))
                items.Add(MapQuantityItem(itemBlock));

        CQuantityItem? assumed = assumedAttr != null ? MapQuantityItem(assumedAttr) : null;

        return new CDvQuantity
        {
            RmTypeName   = rmType,
            NodeId       = "",
            RawOdin      = rawOdin,
            Property     = property,
            List         = items,
            AssumedValue = assumed,
        };
    }

    private static CQuantityItem MapQuantityItem(OdinParser.OdinObjectValueBlockContext ctx)
    {
        var units     = OdinReader.String(OdinReader.Attr(ctx, "units")) ?? "";
        var magnitude = ParseRealInterval(OdinReader.Attr(ctx, "magnitude")?.GetText());
        var precision = ParseIntegerInterval(OdinReader.Attr(ctx, "precision")?.GetText());
        return new CQuantityItem { Units = units, Magnitude = magnitude, Precision = precision };
    }

    // ── Ordinal (C_ORDINAL / cOrdinal grammar rule) ──────────────────────────

    private static CDvOrdinal MapDvOrdinal(Cadl14Parser.COrdinalContext ctx)
    {
        var items = ctx.ordinalTerm().Select(ot =>
        {
            var value = int.Parse(ot.ordinalValue().GetText());
            var code  = ot.cTerminologyCode().GetText().Trim('[', ']');
            return new OrdinalTerm(value, code);
        }).ToList();

        int? assumed = ctx.ordinalValue() is { } av ? int.Parse(av.GetText()) : null;

        return new CDvOrdinal
        {
            RmTypeName   = "",
            NodeId       = "",
            Items        = items,
            AssumedValue = assumed,
        };
    }

    // ── Code phrase ───────────────────────────────────────────────────────────

    private static CCodePhrase MapCCodePhrase(Cadl14Parser.CTerminologyCodeContext ctx)
    {
        if (ctx.cLocalTermCode() is { } local)
        {
            List<string> codes = [];
            if (local.localCodesList() is { } lcl)
            {
                codes.Add(lcl.adl14_at_code().GetText());
                codes.AddRange(lcl.termCodeItem().Select(ti => ti.adl14_at_code().GetText()));
            }
            string? assumed = local.termCodeDefault()?.adl14_at_code().GetText();
            return new CCodePhrase { TerminologyId = "local", CodeList = codes, AssumedValue = assumed };
        }

        if (ctx.terminologyLocalCode() is { } tlc)
        {
            string code = tlc.adl14_at_code().GetText();
            return new CCodePhrase { TerminologyId = "local", CodeList = [code] };
        }

        if (ctx.valueSetCode() is { } vsc)
        {
            string acCode = vsc.adl14_ac_code().GetText();
            string? assumed = vsc.termCodeDefault()?.adl14_at_code().GetText();
            return new CCodePhrase { ValueSetCode = acCode, AssumedValue = assumed };
        }

        if (ctx.cExternalTermCode() is { } ext)
        {
            var startText    = ext.C_EXTERNAL_TERM_CODE_START().GetText();
            var terminologyId = startText.TrimStart('[').TrimEnd(':');
            List<string> extCodes = [];
            if (ext.externalCodesList() is { } ecl)
            {
                extCodes.Add(ecl.C_EXTERNAL_TERM_CODE_STRING().GetText());
                extCodes.AddRange(ecl.externalTermCodeItem()
                    .Select(ti => ti.C_EXTERNAL_TERM_CODE_STRING().GetText()));
            }
            string? extAssumed = ext.externalTermCodeDefault()
                ?.C_EXTERNAL_TERM_CODE_STRING().GetText();
            return new CCodePhrase
            {
                TerminologyId = terminologyId,
                CodeList      = extCodes,
                AssumedValue  = extAssumed,
            };
        }

        return new CCodePhrase { TerminologyId = null, CodeList = [] };
    }

    // ── Stub factory ─────────────────────────────────────────────────────────

    /// <summary>Returns a typed stub for known DV types, or null for structural types.</summary>
    private static CObject? DvStub(string rmTypeName, string nodeId, IntervalOfInt? occ) =>
        rmTypeName switch
        {
            "DV_TEXT"       => new CDvText       { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ },
            "DV_CODED_TEXT" => new CDvCodedText  { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ },
            "DV_BOOLEAN"    => new CDvBoolean    { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ },
            "DV_DATE"       => new CDvDate       { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ },
            "DV_TIME"       => new CDvTime       { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ },
            "DV_DATE_TIME"  => new CDvDateTime   { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ },
            "DV_DURATION"   => new CDvDuration   { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ },
            "DV_COUNT"      => new CDvCount      { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ },
            "DV_ORDINAL"    => new CDvOrdinal    { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ },
            "DV_SCALE"      => new CDvScale      { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ },
            "DV_PROPORTION" => new CDvProportion { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ },
            "DV_IDENTIFIER" => new CDvIdentifier { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ },
            "DV_PARAGRAPH"  => new CDvParagraph  { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ },
            "DV_STATE"      => new CDvState      { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ },
            "DV_MULTIMEDIA" => new CDvMultimedia { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ },
            "DV_PARSABLE"   => new CDvParsable   { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ },
            "DV_URI"        => new CDvUri        { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ },
            "DV_EHR_URI"    => new CDvEhrUri     { RmTypeName = rmTypeName, NodeId = nodeId, Occurrences = occ },
            _               => null,
        };

    // ── String/value parse helpers ────────────────────────────────────────────

    private static string StripStringQuotes(string raw)
    {
        if (raw.Length >= 2 && raw[0] == '"' && raw[^1] == '"')   return raw[1..^1];
        if (raw.Length >= 2 && raw[0] == '\'' && raw[^1] == '\'') return raw[1..^1];
        return raw;
    }

    private static int ParseInt(string text)
    {
        text = text.TrimStart('+');
        return int.Parse(text);
    }

    // ── Interval helpers ──────────────────────────────────────────────────────

    private static IntervalOfInt? MapOccurrences(Cadl14Parser.COccurrencesContext? ctx)
    {
        if (ctx == null) return null;
        return MapMultiplicity(ctx.multiplicity());
    }

    private static IntervalOfInt? MapExistence(Cadl14Parser.CExistenceContext? ctx)
    {
        if (ctx == null) return null;
        var text = ctx.existence().GetText();
        if (!text.Contains(".."))
        {
            var n = int.Parse(text);
            return new IntervalOfInt(n, n);
        }
        var parts = text.Split("..");
        return new IntervalOfInt(int.Parse(parts[0]), int.Parse(parts[1]));
    }

    private static Cardinality MapCardinality(Cadl14Parser.CCardinalityContext ctx)
    {
        var c = ctx.cardinality();
        var interval  = MapMultiplicity(c.multiplicity());
        bool isOrdered = true;
        bool isUnique  = false;
        foreach (var mod in c.multiplicityMod())
        {
            if (mod.orderingMod() is { } om)
                isOrdered = om.SYM_ORDERED() != null;
            else if (mod.uniqueMod() != null)
                isUnique = true;
        }
        return new Cardinality(isOrdered, isUnique, interval);
    }

    private static IntervalOfInt MapMultiplicity(Cadl14Parser.MultiplicityContext ctx)
    {
        var text = ctx.GetText();
        if (text == "*") return new IntervalOfInt(0, null);
        if (!text.Contains(".."))
        {
            var n = int.Parse(text);
            return new IntervalOfInt(n, n);
        }
        var parts = text.Split("..");
        var lower = int.Parse(parts[0]);
        int? upper = parts[1] == "*" ? null : int.Parse(parts[1]);
        return new IntervalOfInt(lower, upper);
    }

    private static IntervalOfInt MapIntegerIntervalRange(Cadl14Parser.IntegerIntervalRangeContext ctx)
    {
        var vals = ctx.integerValue();
        if (vals.Length == 2)
        {
            bool lowerExcl = ctx.SYM_GT() != null;
            bool upperExcl = ctx.SYM_LT() != null;
            int lower = ParseInt(vals[0].GetText());
            int upper = ParseInt(vals[1].GetText());
            return new IntervalOfInt(lowerExcl ? lower + 1 : lower, upperExcl ? upper - 1 : upper);
        }
        if (vals.Length == 1)
        {
            int v = ParseInt(vals[0].GetText());
            if (ctx.relop() != null)
            {
                return ctx.relop().GetText() switch
                {
                    ">=" => new IntervalOfInt(v, null),
                    ">"  => new IntervalOfInt(v + 1, null),
                    "<=" => new IntervalOfInt(0, v),
                    "<"  => new IntervalOfInt(0, v - 1),
                    _    => new IntervalOfInt(v, v),
                };
            }
            return new IntervalOfInt(v, v);
        }
        return new IntervalOfInt(0, null);
    }

    private static IntervalOfReal? ParseRealInterval(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;

        raw = raw.Trim();
        if (raw.StartsWith('<')) raw = raw[1..];
        if (raw.EndsWith('>'))   raw = raw[..^1];
        raw = raw.Trim().Trim('|');

        if (!raw.Contains(".."))
        {
            return double.TryParse(raw, System.Globalization.NumberStyles.Any,
                                   System.Globalization.CultureInfo.InvariantCulture, out var v)
                ? new IntervalOfReal(v, v)
                : null;
        }

        var sepIdx = FindIntervalSep(raw);
        if (sepIdx < 0) return null;

        var lowerStr = raw[..sepIdx].Trim();
        var upperStr = raw[(sepIdx + 2)..].Trim();

        bool lowerExclusive = lowerStr.StartsWith('>');
        bool upperExclusive = upperStr.StartsWith('<');
        if (lowerExclusive) lowerStr = lowerStr[1..].Trim();
        if (upperExclusive) upperStr = upperStr[1..].Trim();

        double? lower = lowerStr == "*" ? null
            : double.TryParse(lowerStr, System.Globalization.NumberStyles.Any,
                              System.Globalization.CultureInfo.InvariantCulture, out var lo) ? lo : null;
        double? upper = upperStr == "*" ? null
            : double.TryParse(upperStr, System.Globalization.NumberStyles.Any,
                              System.Globalization.CultureInfo.InvariantCulture, out var hi) ? hi : null;

        return new IntervalOfReal(lower, upper, lowerExclusive, upperExclusive);
    }

    private static int FindIntervalSep(string s)
    {
        for (int i = 0; i < s.Length - 1; i++)
            if (s[i] == '.' && s[i + 1] == '.' && i > 0 && char.IsDigit(s[i - 1]))
                return i;
        return -1;
    }

    private static IntervalOfInt? ParseIntegerInterval(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;

        raw = raw.Trim();
        if (raw.StartsWith('<')) raw = raw[1..];
        if (raw.EndsWith('>'))   raw = raw[..^1];
        raw = raw.Trim().Trim('|');

        if (!raw.Contains(".."))
            return int.TryParse(raw.Trim(), out var v) ? new IntervalOfInt(v, v) : null;

        var sepIdx   = raw.IndexOf("..", StringComparison.Ordinal);
        var lowerStr = raw[..sepIdx].Trim();
        var upperStr = raw[(sepIdx + 2)..].Trim();

        if (!int.TryParse(lowerStr, out var lower)) return null;
        int? upper = upperStr == "*" ? null : int.TryParse(upperStr, out var u) ? u : null;
        return new IntervalOfInt(lower, upper);
    }
}
