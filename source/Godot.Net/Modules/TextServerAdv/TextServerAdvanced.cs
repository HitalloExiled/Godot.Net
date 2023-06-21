#define MODULE_FREETYPE_ENABLED
#define HB_VERSION_ATLEAST_5_1_0

namespace Godot.Net.Modules.TextServerAdv;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot.Net.Core;
using Godot.Net.Core.Config;
using Godot.Net.Core.Enums;
using Godot.Net.Core.Math;
using Godot.Net.Core.OS;
using Godot.Net.Core.String;
using Godot.Net.Extensions;
using Godot.Net.Servers.Text;
using Godot.Net.ThirdParty.Icu;
using HbBuffer        = HarfBuzzSharp.Buffer;
using HbBufferFlags   = HarfBuzzSharp.BufferFlags;
using HbDirection     = HarfBuzzSharp.Direction;
using HbFeature       = HarfBuzzSharp.Feature;
using HbFont          = HarfBuzzSharp.Font;
using HbFontExtents   = HarfBuzzSharp.FontExtents;
using HbFontFunctions = HarfBuzzSharp.FontFunctions;
using HbGlyphExtents  = HarfBuzzSharp.GlyphExtents;
using HbGlyphFlags    = HarfBuzzSharp.GlyphFlags;
using HbLanguage      = HarfBuzzSharp.Language;
using HbScript        = HarfBuzzSharp.Script;
using HbTag           = HarfBuzzSharp.Tag;

public partial class TextServerAdvanced : TextServerExtension, IDisposable
{
    #region private static fields
    private static readonly object padlock = new();
    #endregion private static fields

    #region private static fields
    private static HbTag italTag;
    private static HbTag wdthTag;
    private static HbTag wgthTag;
    #endregion private static fields

    #region private readonly properties
    private readonly Dictionary<string, uint>                   featureSets    = new();
    private readonly Dictionary<uint, FeatureInfo>              featureSetsInv = new();
    private readonly GuidOwner<FontAdvanced>                    fontOwner      = new();
    private readonly List<NumSystemData>                        numSystems     = new();
    private readonly GuidOwner<ShapedTextDataAdvanced>          shapedOwner    = new();
    private readonly Dictionary<string, byte[]>                 systemFontData = new();
    private readonly Dictionary<SystemFontKey, SystemFontCache> systemFonts    = new();
    #endregion private readonly properties

    #region private fields
    private bool             disposed;
    private HbFontFunctions? funcs;
    #endregion private fields

    #region public overrided properties
    public override Feature Features
    {
        get
        {
            var interfaceFeatures = Feature.FEATURE_SIMPLE_LAYOUT
                | Feature.FEATURE_BIDI_LAYOUT
                | Feature.FEATURE_VERTICAL_LAYOUT
                | Feature.FEATURE_SHAPING
                | Feature.FEATURE_KASHIDA_JUSTIFICATION
                | Feature.FEATURE_BREAK_ITERATORS
                | Feature.FEATURE_FONT_BITMAP
                | Feature.FEATURE_FONT_VARIABLE
                | Feature.FEATURE_CONTEXT_SENSITIVE_CASE_CONVERSION
                | Feature.FEATURE_USE_SUPPORT_DATA;
            #if MODULE_FREETYPE_ENABLED
            interfaceFeatures |= Feature.FEATURE_FONT_DYNAMIC;
            #endif
            #if MODULE_MSDFGEN_ENABLED
                interfaceFeatures |= Feature.FEATURE_FONT_MSDF;
            #endif

            return interfaceFeatures;
        }
    }

    #if GDEXTENSION
    public override string Name => "ICU / HarfBuzz / Graphite (GDExtension)";
    #else
    public override string Name => "ICU / HarfBuzz / Graphite (Built-in)";
    #endif
    #endregion public overrided properties

    public TextServerAdvanced()
    {
        this.InsertNumSystemsLang();
        this.InsertFeatureSets();
        this.BmpCreateFontFuncs();
    }

    ~TextServerAdvanced() =>
        this.Dispose(false);

    #region private static methods
    private static int ConvertPos(ShapedTextDataAdvanced sd, int pos)
    {
        var limit = pos;

        if (sd.Text.Length != sd.Utf16.Length)
        {
            var data = sd.Utf16;

            for (var i = 0; i < pos; i++)
            {
                if (char.IsHighSurrogate(data[i]))
                {
                    limit--;
                }
            }
        }

        return limit;
    }

    private static int ConvertPosInv(ShapedTextDataAdvanced sd, int pos)
    {
        var limit = pos;

        if (sd.Text.Length != sd.Utf16.Length)
        {
            for (var i = 0; i < pos; i++)
            {
                if (sd.Text[i] > 0xffff)
                {
                    limit++;
                }
            }
        }
        return limit;
    }

    private static void Realign(ShapedTextDataAdvanced sd)
    {
        // Align embedded objects to baseline.
        var fullAscent  = sd.Ascent;
        var fullDescent = sd.Descent;

        foreach (var item in sd.Objects)
        {
            if (item.Value.Pos >= sd.Start && item.Value.Pos < sd.End)
            {
                if (sd.Orientation == Orientation.ORIENTATION_HORIZONTAL)
                {
                    switch (item.Value.InlineAlign & (InlineAlignment.INLINE_ALIGNMENT_TEXT_MASK))
                    {
                        case InlineAlignment.INLINE_ALIGNMENT_TO_TOP:
                            item.Value.Rect = item.Value.Rect with
                            {
                                Position = item.Value.Rect.Position with
                                {
                                    Y = -(RealT)sd.Ascent
                                }
                            };

                            break;
                        case InlineAlignment.INLINE_ALIGNMENT_TO_CENTER:
                            item.Value.Rect = item.Value.Rect with
                            {
                                Position = item.Value.Rect.Position with
                                {
                                    Y = (RealT)(-sd.Ascent + sd.Descent) / 2
                                }
                            };

                            break;
                        case InlineAlignment.INLINE_ALIGNMENT_TO_BASELINE:
                            item.Value.Rect = item.Value.Rect with
                            {
                                Position = item.Value.Rect.Position with
                                {
                                    Y = 0
                                }
                            };

                            break;
                        case InlineAlignment.INLINE_ALIGNMENT_TO_BOTTOM:
                            item.Value.Rect = item.Value.Rect with
                            {
                                Position = item.Value.Rect.Position with
                                {
                                    Y = (RealT)sd.Descent
                                }
                            };

                            break;
                    }
                    switch (item.Value.InlineAlign & InlineAlignment.INLINE_ALIGNMENT_IMAGE_MASK)
                    {
                        case InlineAlignment.INLINE_ALIGNMENT_BOTTOM_TO:
                            item.Value.Rect = item.Value.Rect with
                            {
                                Position = item.Value.Rect.Position with
                                {
                                    Y = item.Value.Rect.Position.Y - item.Value.Rect.Size.Y
                                }
                            };

                            break;
                        case InlineAlignment.INLINE_ALIGNMENT_CENTER_TO:
                            item.Value.Rect = item.Value.Rect with
                            {
                                Position = item.Value.Rect.Position with
                                {
                                    Y = item.Value.Rect.Position.Y - item.Value.Rect.Size.Y / 2
                                }
                            };

                            break;
                        case InlineAlignment.INLINE_ALIGNMENT_BASELINE_TO:
                            item.Value.Rect = item.Value.Rect with
                            {
                                Position = item.Value.Rect.Position with
                                {
                                    Y = (RealT)(item.Value.Rect.Position.Y - item.Value.Baseline)
                                }
                            };

                            break;
                        case InlineAlignment.INLINE_ALIGNMENT_TOP_TO:
                            // NOP
                            break;
                    }

                    fullAscent = Math.Max(fullAscent, -item.Value.Rect.Position.Y);
                    fullDescent = Math.Max(fullDescent, item.Value.Rect.Position.Y + item.Value.Rect.Size.Y);
                }
                else
                {
                    switch (item.Value.InlineAlign & InlineAlignment.INLINE_ALIGNMENT_TEXT_MASK)
                    {
                        case InlineAlignment.INLINE_ALIGNMENT_TO_TOP:
                            item.Value.Rect = item.Value.Rect with
                            {
                                Position = item.Value.Rect.Position with
                                {
                                    X = -(RealT)sd.Ascent
                                }
                            };

                            break;
                        case InlineAlignment.INLINE_ALIGNMENT_TO_CENTER:
                            item.Value.Rect = item.Value.Rect with
                            {
                                Position = item.Value.Rect.Position with
                                {
                                    X = (RealT)(-sd.Ascent + sd.Descent) / 2
                                }
                            };

                            break;
                        case InlineAlignment.INLINE_ALIGNMENT_TO_BASELINE:
                            item.Value.Rect = item.Value.Rect with
                            {
                                Position = item.Value.Rect.Position with
                                {
                                    X = 0
                                }
                            };

                            break;
                        case InlineAlignment.INLINE_ALIGNMENT_TO_BOTTOM:
                            item.Value.Rect = item.Value.Rect with
                            {
                                Position = item.Value.Rect.Position with
                                {
                                    X = (RealT)sd.Descent
                                }
                            };

                            break;
                    }
                    switch (item.Value.InlineAlign & InlineAlignment.INLINE_ALIGNMENT_IMAGE_MASK)
                    {
                        case InlineAlignment.INLINE_ALIGNMENT_BOTTOM_TO:
                            item.Value.Rect = item.Value.Rect with
                            {
                                Position = item.Value.Rect.Position with
                                {
                                    X = item.Value.Rect.Position.X - item.Value.Rect.Size.X
                                }
                            };

                            break;
                        case InlineAlignment.INLINE_ALIGNMENT_CENTER_TO:
                            item.Value.Rect = item.Value.Rect with
                            {
                                Position = item.Value.Rect.Position with
                                {
                                    X = item.Value.Rect.Position.X - item.Value.Rect.Size.X / 2
                                }
                            };

                            break;
                        case InlineAlignment.INLINE_ALIGNMENT_BASELINE_TO:
                            item.Value.Rect = item.Value.Rect with
                            {
                                Position = item.Value.Rect.Position with
                                {
                                    X = (RealT)(item.Value.Rect.Position.X - item.Value.Baseline)
                                }
                            };

                            break;
                        case InlineAlignment.INLINE_ALIGNMENT_TOP_TO:
                            // NOP
                            break;
                    }
                    fullAscent  = Math.Max(fullAscent, -item.Value.Rect.Position.X);
                    fullDescent = Math.Max(fullDescent, item.Value.Rect.Position.X + item.Value.Rect.Size.X);
                }
            }
        }

        sd.Ascent  = fullAscent;
        sd.Descent = fullDescent;
    }
    #endregion private static methods

    #region protected static methods
    protected static void Invalidate(ShapedTextDataAdvanced shaped, bool text = false)
    {
        shaped.Valid                 = default;
        shaped.SortValid             = default;
        shaped.LineBreaksValid       = default;
        shaped.JustificationOpsValid = default;
        shaped.TextTrimmed           = default;
        shaped.Ascent                = default;
        shaped.Descent               = default;
        shaped.Width                 = default;
        shaped.Upos                  = default;
        shaped.Uthk                  = default;
        shaped.OverrunTrimData       = new();
        shaped.Utf16                 = "";
        shaped.Glyphs.Clear();
        shaped.GlyphsLogical.Clear();

        foreach (var bidi in shaped.BidiIter)
        {
            bidi.Dispose();
        }

        shaped.BidiIter.Clear();

        if (text)
        {
            shaped.ScriptIter    = default;
            shaped.BreakOpsValid = default;
            shaped.JsOpsValid    = default;
        }
    }
    #endregion protected static methods

    #region private methods
    private void AddFeatures(Dictionary<uint, Feature> source, out HbFeature[] ftrs) => throw new NotImplementedException();

    private void BmpCreateFontFuncs()
    {
        if (this.funcs == null)
        {
            this.funcs = new HbFontFunctions();

            this.funcs.SetHorizontalFontExtentsDelegate(this.BmpGetFontHExtents);
            this.funcs.SetNominalGlyphDelegate(this.BmpGetNominalGlyph);
            this.funcs.SetHorizontalGlyphAdvanceDelegate(this.BmpGetGlyphHAdvance);
            this.funcs.SetVerticalGlyphAdvanceDelegate(this.BmpGetGlyphVAdvance);
            this.funcs.SetVerticalGlyphOriginDelegate(this.BmpGetGlyphVOrigin);
            this.funcs.SetHorizontalGlyphAdvanceDelegate(this.BmpGetGlyphHKerning);
            this.funcs.SetGlyphExtentsDelegate(this.BmpGetGlyphExtents);

            this.funcs.MakeImmutable();
        }
    }

    private bool BmpGetGlyphExtents(HbFont font, object fontData, uint glyph, out HbGlyphExtents extents) => throw new NotImplementedException();
    private int BmpGetGlyphHKerning(HbFont font, object fontData, uint glyph) => throw new NotImplementedException();
    private bool BmpGetGlyphVOrigin(HbFont font, object fontData, uint glyph, out int x, out int y) => throw new NotImplementedException();
    private int BmpGetGlyphVAdvance(HbFont font, object fontData, uint glyph) => throw new NotImplementedException();
    private int BmpGetGlyphHAdvance(HbFont font, object fontData, uint glyph) => throw new NotImplementedException();
    private bool BmpGetNominalGlyph(HbFont font, object fontData, uint unicode, out uint glyph) => throw new NotImplementedException();
    private bool BmpGetFontHExtents(HbFont font, object fontData, out HbFontExtents extents) => throw new NotImplementedException();
    private Guid CreateFont() => throw new NotImplementedException();
    private bool EnsureGlyph(FontAdvanced fontData, in Vector2<int> size, int glyph) => throw new NotImplementedException();
    private FontAntialiasing FontGetAntialiasing(Guid fontId) => throw new NotImplementedException();
    private double FontGetEmbolden(Guid fontId) => throw new NotImplementedException();
    private int FontGetFaceCount(Guid fontId) => throw new NotImplementedException();
    private int FontGetFixedSize(Guid fontId) => throw new NotImplementedException();
    private bool FontGetGenerateMipmaps(Guid fontId) => throw new NotImplementedException();
    private Vector2<RealT> FontGetGlyphAdvance(Guid fontId, int size, int glyph) => throw new NotImplementedException();
    private long FontGetGlyphIndex(Guid fontId, int size, long @char, long variationSelector) => throw new NotImplementedException();
    private HbFont FontGetHbHandle(Guid fontId, int size) => throw new NotImplementedException();
    private Hinting FontGetHinting(Guid fontId) => throw new NotImplementedException();
    private int FontGetMsdfPixelRange(Guid fontId) => throw new NotImplementedException();
    private int FontGetMsdfSize(Guid fontId) => throw new NotImplementedException();
    private string FontGetName(Guid fontId) => throw new NotImplementedException();
    private Dictionary<uint, Feature> FontGetOpentypeFeatureOverrides(Guid fontId) => throw new NotImplementedException();
    private double FontGetOversampling(Guid fontId) => throw new NotImplementedException();
    private double FontGetScale(Guid fontId, int size) => throw new NotImplementedException();
    private double FontGetUnderlinePosition(Guid fontId, int size) => throw new NotImplementedException();
    private double FontGetUnderlineThickness(Guid fontId, int size) => throw new NotImplementedException();
    private double GetExtraAdvance(Guid fontId, int fontSize) => throw new NotImplementedException();
    private Vector2<int> GetSize(FontAdvanced fontData, int size) => throw new NotImplementedException();
    private int FontGetStretch(Guid fontId) => throw new NotImplementedException();
    private FontStyle FontGetStyle(Guid fontId) => throw new NotImplementedException();
    private SubpixelPositioning FontGetSubpixelPositioning(Guid fontId) => throw new NotImplementedException();
    private Transform2D<RealT> FontGetTransform(Guid fontId) => throw new NotImplementedException();
    private Dictionary<uint, int> FontGetVariationCoordinates(Guid fontId) => throw new NotImplementedException();
    private int FontGetWeight(Guid fontId) => throw new NotImplementedException();
    private bool FontHasChar(Guid fontId, long @char) => throw new NotImplementedException();
    private bool FontIsAllowSystemFallback(Guid fontId) => throw new NotImplementedException();
    private bool FontIsForceAutohinter(Guid fontId) => throw new NotImplementedException();
    private bool FontIsLanguageSupported(Guid fontId, string language) => throw new NotImplementedException();
    private bool FontIsMultichannelSignedDistanceField(Guid fontId) => throw new NotImplementedException();
    private bool FontIsScriptSupported(Guid fontId, string script) => throw new NotImplementedException();
    private void FontSetAntialiasing(Guid fontId, FontAntialiasing antialiasing) => throw new NotImplementedException();
    private void FontSetData(Guid fontId, byte[] data) => throw new NotImplementedException();
    private void FontSetEmbolden(Guid fontId, double strength) => throw new NotImplementedException();
    private void FontSetFaceIndex(Guid fontId, int faceIndex) => throw new NotImplementedException();
    private void FontSetFixedSize(Guid fontId, int fixedSize) => throw new NotImplementedException();
    private void FontSetForceAutohinter(Guid fontId, bool forceAutohinter) => throw new NotImplementedException();
    private void FontSetGenerateMipmaps(Guid fontId, bool generateMipmaps) => throw new NotImplementedException();
    private void FontSetHinting(Guid fontId, Hinting hinting) => throw new NotImplementedException();
    private void FontSetMsdfPixelRange(Guid fontId, int msdfPixelRange) => throw new NotImplementedException();
    private void FontSetMsdfSize(Guid fontId, int msdfSize) => throw new NotImplementedException();
    private void FontSetMultichannelSignedDistanceField(Guid fontId, bool msdf) => throw new NotImplementedException();
    private void FontSetOversampling(Guid fontId, double oversampling) => throw new NotImplementedException();
    private void FontSetStretch(Guid fontId, int stretch) => throw new NotImplementedException();
    private void FontSetStyle(Guid fontId, FontStyle style) => throw new NotImplementedException();
    private void FontSetSubpixelPositioning(Guid fontId, SubpixelPositioning subpixel) => throw new NotImplementedException();
    private void FontSetTransform(Guid fontId, in Transform2D<RealT> transform) => throw new NotImplementedException();
    private void FontSetVariationCoordinates(Guid fontId, Dictionary<uint, int> variationCoordinates) => throw new NotImplementedException();
    private void FontSetWeight(Guid fontId, int weight) => throw new NotImplementedException();
    private Dictionary<uint, int> FontSupportedVariationList(Guid fontId) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void InsertFeature(string name, HbTag tag, VariantType type, bool hidden)
    {
        var fi = new FeatureInfo(name, type, hidden);

        this.featureSets[name]   = tag;
        this.featureSetsInv[tag] = fi;
    }

    private void InsertNumSystemsLang()
    {
        // Eastern Arabic numerals.
        var ar = new NumSystemData
        {
            Digits      = "٠١٢٣٤٥٦٧٨٩٫",
            Exp         = "اس",
            PercentSign = "٪",
            Lang        = new()
            {
                "ar",  // Arabic
                "ar_AE",
                "ar_BH",
                "ar_DJ",
                "ar_EG",
                "ar_ER",
                "ar_IL",
                "ar_IQ",
                "ar_JO",
                "ar_KM",
                "ar_KW",
                "ar_LB",
                "ar_MR",
                "ar_OM",
                "ar_PS",
                "ar_QA",
                "ar_SA",
                "ar_SD",
                "ar_SO",
                "ar_SS",
                "ar_SY",
                "ar_TD",
                "ar_YE",
                "ckb", // Central Kurdish
                "ckb_IQ",
                "ckb_IR",
                "sd",  // Sindhi
                "sd_PK",
                "sd_Arab",
                "sd_Arab_PK",
            },
        };

        this.numSystems.Add(ar);

        // Persian and Urdu numerals.
        var pr = new NumSystemData
        {
            Digits      = "۰۱۲۳۴۵۶۷۸۹٫",
            Exp         = "اس",
            PercentSign = "٪",
            Lang = new()
            {
                "fa",    // Persian
                "fa_AF",
                "fa_IR",
                "ks",    // Kashmiri
                "ks_IN",
                "ks_Arab",
                "ks_Arab_IN",
                "lrc",   // Northern Luri
                "lrc_IQ",
                "lrc_IR",
                "mzn",   // Mazanderani
                "mzn_IR",
                "pa_PK", // Panjabi
                "pa_Arab",
                "pa_Arab_PK",
                "ps",    // Pushto
                "ps_AF",
                "ps_PK",
                "ur_IN", // Urdu
                "uz_AF", // Uzbek
                "uz_Arab",
                "uz_Arab_AF",
            }
        };

        this.numSystems.Add(pr);

        // Bengali numerals.
        var bn = new NumSystemData
        {
            Digits      = "০১২৩৪৫৬৭৮৯.",
            Exp         = "e",
            PercentSign = "%",
            Lang = new()
            {
                "as",  // Assamese
                "as_IN",
                "bn",  // Bengali
                "bn_BD",
                "bn_IN",
                "mni", // Manipuri
                "mni_IN",
                "mni_Beng",
                "mni_Beng_IN",
            }
        };

        this.numSystems.Add(bn);

        // Devanagari numerals.
        var mr = new NumSystemData
        {
            Digits      = "०१२३४५६७८९.",
            Exp         = "e",
            PercentSign = "%",
            Lang        = new()
            {
                "sa_IN",
                "mr", // Marathi
                "mr_IN",
                "ne", // Nepali
                "ne_IN",
                "ne_NP",
                "sa", // Sanskrit
            }
        };

        this.numSystems.Add(mr);

        // Dzongkha numerals.
        var dz = new NumSystemData
        {
            Digits      = "༠༡༢༣༤༥༦༧༨༩.",
            Exp         = "e",
            PercentSign = "%",
            Lang        = new()
            {
                "dz", // Dzongkha
                "dz_BT",
            }
        };

        this.numSystems.Add(dz);

        // Santali numerals.
        var sat = new NumSystemData
        {
            Digits      = "᱐᱑᱒᱓᱔᱕᱖᱗᱘᱙.",
            Exp         = "e",
            PercentSign = "%",
            Lang        = new()
            {
                "sat", // Santali
                "sat_IN",
                "sat_Olck",
                "sat_Olck_IN",
            }
        };

        this.numSystems.Add(sat);

        // Burmese numerals.
        var my = new NumSystemData
        {
            Digits      = "၀၁၂၃၄၅၆၇၈၉.",
            Exp         = "e",
            PercentSign = "%",
            Lang        = new()
            {
                "my", // Burmese
                "my_MM",
            }
        };

        this.numSystems.Add(my);

        // Chakma numerals.
        var ccp = new NumSystemData
        {
            Digits      = "𑄶𑄷𑄸𑄹𑄺𑄻𑄼𑄽𑄾𑄿.",
            Exp         = "e",
            PercentSign = "%",
            Lang        = new()
            {
                "ccp", // Chakma
                "ccp_BD",
                "ccp_IN",
            }
        };

        this.numSystems.Add(ccp);

        // Adlam numerals.
        var ff = new NumSystemData
        {
            Digits      = "𞥐𞥑𞥒𞥓𞥔𞥕𞥖𞥗𞥘𞥙.",
            Exp         = "e",
            PercentSign = "%",
            Lang        = new()
            {
                "ff", // Fulah
                "ff_Adlm_BF",
                "ff_Adlm_CM",
                "ff_Adlm_GH",
                "ff_Adlm_GM",
                "ff_Adlm_GN",
                "ff_Adlm_GW",
                "ff_Adlm_LR",
                "ff_Adlm_MR",
                "ff_Adlm_NE",
                "ff_Adlm_NG",
                "ff_Adlm_SL",
                "ff_Adlm_SN",
            }
        };

        this.numSystems.Add(ff);
    }

    private void InsertFeatureSets()
    {
        // Registered OpenType feature tags.
        // Name, Tag, Data Type, Hidden
        this.InsertFeature("access_all_alternates",                   new HbTag('a', 'a', 'l', 't'), VariantType.INT, false);
        this.InsertFeature("above_base_forms",                        new HbTag('a', 'b', 'v', 'f'), VariantType.INT, true);
        this.InsertFeature("above_base_mark_positioning",             new HbTag('a', 'b', 'v', 'm'), VariantType.INT, true);
        this.InsertFeature("above_base_substitutions",                new HbTag('a', 'b', 'v', 's'), VariantType.INT, true);
        this.InsertFeature("alternative_fractions",                   new HbTag('a', 'f', 'r', 'c'), VariantType.INT, false);
        this.InsertFeature("akhands",                                 new HbTag('a', 'k', 'h', 'n'), VariantType.INT, true);
        this.InsertFeature("below_base_forms",                        new HbTag('b', 'l', 'w', 'f'), VariantType.INT, true);
        this.InsertFeature("below_base_mark_positioning",             new HbTag('b', 'l', 'w', 'm'), VariantType.INT, true);
        this.InsertFeature("below_base_substitutions",                new HbTag('b', 'l', 'w', 's'), VariantType.INT, true);
        this.InsertFeature("contextual_alternates",                   new HbTag('c', 'a', 'l', 't'), VariantType.BOOL, false);
        this.InsertFeature("case_sensitive_forms",                    new HbTag('c', 'a', 's', 'e'), VariantType.BOOL, false);
        this.InsertFeature("glyph_composition",                       new HbTag('c', 'c', 'm', 'p'), VariantType.INT, true);
        this.InsertFeature("conjunct_form_after_ro",                  new HbTag('c', 'f', 'a', 'r'), VariantType.INT, true);
        this.InsertFeature("contextual_half_width_spacing",           new HbTag('c', 'h', 'w', 's'), VariantType.INT, true);
        this.InsertFeature("conjunct_forms",                          new HbTag('c', 'j', 'c', 't'), VariantType.INT, true);
        this.InsertFeature("contextual_ligatures",                    new HbTag('c', 'l', 'i', 'g'), VariantType.BOOL, false);
        this.InsertFeature("centered_cjk_punctuation",                new HbTag('c', 'p', 'c', 't'), VariantType.BOOL, false);
        this.InsertFeature("capital_spacing",                         new HbTag('c', 'p', 's', 'p'), VariantType.BOOL, false);
        this.InsertFeature("contextual_swash",                        new HbTag('c', 's', 'w', 'h'), VariantType.INT, false);
        this.InsertFeature("cursive_positioning",                     new HbTag('c', 'u', 'r', 's'), VariantType.INT, true);
        this.InsertFeature("character_variant_01",                    new HbTag('c', 'v', '0', '1'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_02",                    new HbTag('c', 'v', '0', '2'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_03",                    new HbTag('c', 'v', '0', '3'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_04",                    new HbTag('c', 'v', '0', '4'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_05",                    new HbTag('c', 'v', '0', '5'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_06",                    new HbTag('c', 'v', '0', '6'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_07",                    new HbTag('c', 'v', '0', '7'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_08",                    new HbTag('c', 'v', '0', '8'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_09",                    new HbTag('c', 'v', '0', '9'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_10",                    new HbTag('c', 'v', '1', '0'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_11",                    new HbTag('c', 'v', '1', '1'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_12",                    new HbTag('c', 'v', '1', '2'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_13",                    new HbTag('c', 'v', '1', '3'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_14",                    new HbTag('c', 'v', '1', '4'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_15",                    new HbTag('c', 'v', '1', '5'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_16",                    new HbTag('c', 'v', '1', '6'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_17",                    new HbTag('c', 'v', '1', '7'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_18",                    new HbTag('c', 'v', '1', '8'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_19",                    new HbTag('c', 'v', '1', '9'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_20",                    new HbTag('c', 'v', '2', '0'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_21",                    new HbTag('c', 'v', '2', '1'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_22",                    new HbTag('c', 'v', '2', '2'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_23",                    new HbTag('c', 'v', '2', '3'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_24",                    new HbTag('c', 'v', '2', '4'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_25",                    new HbTag('c', 'v', '2', '5'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_26",                    new HbTag('c', 'v', '2', '6'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_27",                    new HbTag('c', 'v', '2', '7'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_28",                    new HbTag('c', 'v', '2', '8'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_29",                    new HbTag('c', 'v', '2', '9'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_30",                    new HbTag('c', 'v', '3', '0'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_31",                    new HbTag('c', 'v', '3', '1'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_32",                    new HbTag('c', 'v', '3', '2'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_33",                    new HbTag('c', 'v', '3', '3'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_34",                    new HbTag('c', 'v', '3', '4'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_35",                    new HbTag('c', 'v', '3', '5'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_36",                    new HbTag('c', 'v', '3', '6'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_37",                    new HbTag('c', 'v', '3', '7'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_38",                    new HbTag('c', 'v', '3', '8'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_39",                    new HbTag('c', 'v', '3', '9'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_40",                    new HbTag('c', 'v', '4', '0'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_41",                    new HbTag('c', 'v', '4', '1'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_42",                    new HbTag('c', 'v', '4', '2'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_43",                    new HbTag('c', 'v', '4', '3'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_44",                    new HbTag('c', 'v', '4', '4'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_45",                    new HbTag('c', 'v', '4', '5'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_46",                    new HbTag('c', 'v', '4', '6'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_47",                    new HbTag('c', 'v', '4', '7'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_48",                    new HbTag('c', 'v', '4', '8'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_49",                    new HbTag('c', 'v', '4', '9'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_50",                    new HbTag('c', 'v', '5', '0'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_51",                    new HbTag('c', 'v', '5', '1'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_52",                    new HbTag('c', 'v', '5', '2'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_53",                    new HbTag('c', 'v', '5', '3'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_54",                    new HbTag('c', 'v', '5', '4'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_55",                    new HbTag('c', 'v', '5', '5'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_56",                    new HbTag('c', 'v', '5', '6'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_57",                    new HbTag('c', 'v', '5', '7'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_58",                    new HbTag('c', 'v', '5', '8'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_59",                    new HbTag('c', 'v', '5', '9'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_60",                    new HbTag('c', 'v', '6', '0'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_61",                    new HbTag('c', 'v', '6', '1'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_62",                    new HbTag('c', 'v', '6', '2'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_63",                    new HbTag('c', 'v', '6', '3'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_64",                    new HbTag('c', 'v', '6', '4'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_65",                    new HbTag('c', 'v', '6', '5'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_66",                    new HbTag('c', 'v', '6', '6'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_67",                    new HbTag('c', 'v', '6', '7'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_68",                    new HbTag('c', 'v', '6', '8'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_69",                    new HbTag('c', 'v', '6', '9'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_70",                    new HbTag('c', 'v', '7', '0'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_71",                    new HbTag('c', 'v', '7', '1'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_72",                    new HbTag('c', 'v', '7', '2'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_73",                    new HbTag('c', 'v', '7', '3'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_74",                    new HbTag('c', 'v', '7', '4'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_75",                    new HbTag('c', 'v', '7', '5'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_76",                    new HbTag('c', 'v', '7', '6'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_77",                    new HbTag('c', 'v', '7', '7'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_78",                    new HbTag('c', 'v', '7', '8'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_79",                    new HbTag('c', 'v', '7', '9'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_80",                    new HbTag('c', 'v', '8', '0'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_81",                    new HbTag('c', 'v', '8', '1'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_82",                    new HbTag('c', 'v', '8', '2'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_83",                    new HbTag('c', 'v', '8', '3'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_84",                    new HbTag('c', 'v', '8', '4'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_85",                    new HbTag('c', 'v', '8', '5'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_86",                    new HbTag('c', 'v', '8', '6'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_87",                    new HbTag('c', 'v', '8', '7'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_88",                    new HbTag('c', 'v', '8', '8'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_89",                    new HbTag('c', 'v', '8', '9'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_90",                    new HbTag('c', 'v', '9', '0'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_91",                    new HbTag('c', 'v', '9', '1'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_92",                    new HbTag('c', 'v', '9', '2'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_93",                    new HbTag('c', 'v', '9', '3'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_94",                    new HbTag('c', 'v', '9', '4'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_95",                    new HbTag('c', 'v', '9', '5'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_96",                    new HbTag('c', 'v', '9', '6'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_97",                    new HbTag('c', 'v', '9', '7'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_98",                    new HbTag('c', 'v', '9', '8'), VariantType.BOOL, false);
        this.InsertFeature("character_variant_99",                    new HbTag('c', 'v', '9', '9'), VariantType.BOOL, false);
        this.InsertFeature("petite_capitals_from_capitals",           new HbTag('c', '2', 'p', 'c'), VariantType.BOOL, false);
        this.InsertFeature("small_capitals_from_capitals",            new HbTag('c', '2', 's', 'c'), VariantType.BOOL, false);
        this.InsertFeature("distances",                               new HbTag('d', 'i', 's', 't'), VariantType.INT, true);
        this.InsertFeature("discretionary_ligatures",                 new HbTag('d', 'l', 'i', 'g'), VariantType.BOOL, false);
        this.InsertFeature("denominators",                            new HbTag('d', 'n', 'o', 'm'), VariantType.BOOL, false);
        this.InsertFeature("dotless_forms",                           new HbTag('d', 't', 'l', 's'), VariantType.INT, true);
        this.InsertFeature("expert_forms",                            new HbTag('e', 'x', 'p', 't'), VariantType.BOOL, true);
        this.InsertFeature("final_glyph_on_line_alternates",          new HbTag('f', 'a', 'l', 't'), VariantType.INT, false);
        this.InsertFeature("terminal_forms_2",                        new HbTag('f', 'i', 'n', '2'), VariantType.INT, true);
        this.InsertFeature("terminal_forms_3",                        new HbTag('f', 'i', 'n', '3'), VariantType.INT, true);
        this.InsertFeature("terminal_forms",                          new HbTag('f', 'i', 'n', 'a'), VariantType.INT, true);
        this.InsertFeature("flattened_accent_forms",                  new HbTag('f', 'l', 'a', 'c'), VariantType.INT, true);
        this.InsertFeature("fractions",                               new HbTag('f', 'r', 'a', 'c'), VariantType.BOOL, false);
        this.InsertFeature("full_widths",                             new HbTag('f', 'w', 'i', 'd'), VariantType.BOOL, false);
        this.InsertFeature("half_forms",                              new HbTag('h', 'a', 'l', 'f'), VariantType.INT, true);
        this.InsertFeature("halant_forms",                            new HbTag('h', 'a', 'l', 'n'), VariantType.INT, true);
        this.InsertFeature("alternate_half_widths",                   new HbTag('h', 'a', 'l', 't'), VariantType.BOOL, false);
        this.InsertFeature("historical_forms",                        new HbTag('h', 'i', 's', 't'), VariantType.INT, false);
        this.InsertFeature("horizontal_kana_alternates",              new HbTag('h', 'k', 'n', 'a'), VariantType.BOOL, false);
        this.InsertFeature("historical_ligatures",                    new HbTag('h', 'l', 'i', 'g'), VariantType.BOOL, false);
        this.InsertFeature("hangul",                                  new HbTag('h', 'n', 'g', 'l'), VariantType.INT, false);
        this.InsertFeature("hojo_kanji_forms",                        new HbTag('h', 'o', 'j', 'o'), VariantType.INT, false);
        this.InsertFeature("half_widths",                             new HbTag('h', 'w', 'i', 'd'), VariantType.BOOL, false);
        this.InsertFeature("initial_forms",                           new HbTag('i', 'n', 'i', 't'), VariantType.INT, true);
        this.InsertFeature("isolated_forms",                          new HbTag('i', 's', 'o', 'l'), VariantType.INT, true);
        this.InsertFeature("italics",                                 new HbTag('i', 't', 'a', 'l'), VariantType.INT, false);
        this.InsertFeature("justification_alternates",                new HbTag('j', 'a', 'l', 't'), VariantType.INT, false);
        this.InsertFeature("jis78_forms",                             new HbTag('j', 'p', '7', '8'), VariantType.INT, false);
        this.InsertFeature("jis83_forms",                             new HbTag('j', 'p', '8', '3'), VariantType.INT, false);
        this.InsertFeature("jis90_forms",                             new HbTag('j', 'p', '9', '0'), VariantType.INT, false);
        this.InsertFeature("jis2004_forms",                           new HbTag('j', 'p', '0', '4'), VariantType.INT, false);
        this.InsertFeature("kerning",                                 new HbTag('k', 'e', 'r', 'n'), VariantType.BOOL, false);
        this.InsertFeature("left_bounds",                             new HbTag('l', 'f', 'b', 'd'), VariantType.INT, false);
        this.InsertFeature("standard_ligatures",                      new HbTag('l', 'i', 'g', 'a'), VariantType.BOOL, false);
        this.InsertFeature("leading_jamo_forms",                      new HbTag('l', 'j', 'm', 'o'), VariantType.INT, true);
        this.InsertFeature("lining_figures",                          new HbTag('l', 'n', 'u', 'm'), VariantType.INT, false);
        this.InsertFeature("localized_forms",                         new HbTag('l', 'o', 'c', 'l'), VariantType.INT, true);
        this.InsertFeature("left_to_right_alternates",                new HbTag('l', 't', 'r', 'a'), VariantType.INT, true);
        this.InsertFeature("left_to_right_mirrored_forms",            new HbTag('l', 't', 'r', 'm'), VariantType.INT, true);
        this.InsertFeature("mark_positioning",                        new HbTag('m', 'a', 'r', 'k'), VariantType.INT, true);
        this.InsertFeature("medial_forms_2",                          new HbTag('m', 'e', 'd', '2'), VariantType.INT, true);
        this.InsertFeature("medial_forms",                            new HbTag('m', 'e', 'd', 'i'), VariantType.INT, true);
        this.InsertFeature("mathematical_greek",                      new HbTag('m', 'g', 'r', 'k'), VariantType.BOOL, false);
        this.InsertFeature("mark_to_mark_positioning",                new HbTag('m', 'k', 'm', 'k'), VariantType.INT, true);
        this.InsertFeature("mark_positioning_via_substitution",       new HbTag('m', 's', 'e', 't'), VariantType.INT, true);
        this.InsertFeature("alternate_annotation_forms",              new HbTag('n', 'a', 'l', 't'), VariantType.INT, false);
        this.InsertFeature("nlc_kanji_forms",                         new HbTag('n', 'l', 'c', 'k'), VariantType.INT, false);
        this.InsertFeature("nukta_forms",                             new HbTag('n', 'u', 'k', 't'), VariantType.INT, true);
        this.InsertFeature("numerators",                              new HbTag('n', 'u', 'm', 'r'), VariantType.BOOL, false);
        this.InsertFeature("oldstyle_figures",                        new HbTag('o', 'n', 'u', 'm'), VariantType.INT, false);
        this.InsertFeature("optical_bounds",                          new HbTag('o', 'p', 'b', 'd'), VariantType.INT, true);
        this.InsertFeature("ordinals",                                new HbTag('o', 'r', 'd', 'n'), VariantType.BOOL, false);
        this.InsertFeature("ornaments",                               new HbTag('o', 'r', 'n', 'm'), VariantType.INT, false);
        this.InsertFeature("proportional_alternate_widths",           new HbTag('p', 'a', 'l', 't'), VariantType.BOOL, false);
        this.InsertFeature("petite_capitals",                         new HbTag('p', 'c', 'a', 'p'), VariantType.BOOL, false);
        this.InsertFeature("proportional_kana",                       new HbTag('p', 'k', 'n', 'a'), VariantType.BOOL, false);
        this.InsertFeature("proportional_figures",                    new HbTag('p', 'n', 'u', 'm'), VariantType.BOOL, false);
        this.InsertFeature("pre_base_forms",                          new HbTag('p', 'r', 'e', 'f'), VariantType.INT, true);
        this.InsertFeature("pre_base_substitutions",                  new HbTag('p', 'r', 'e', 's'), VariantType.INT, true);
        this.InsertFeature("post_base_forms",                         new HbTag('p', 's', 't', 'f'), VariantType.INT, true);
        this.InsertFeature("post_base_substitutions",                 new HbTag('p', 's', 't', 's'), VariantType.INT, true);
        this.InsertFeature("proportional_widths",                     new HbTag('p', 'w', 'i', 'd'), VariantType.BOOL, false);
        this.InsertFeature("quarter_widths",                          new HbTag('q', 'w', 'i', 'd'), VariantType.BOOL, false);
        this.InsertFeature("randomize",                               new HbTag('r', 'a', 'n', 'd'), VariantType.INT, false);
        this.InsertFeature("required_contextual_alternates",          new HbTag('r', 'c', 'l', 't'), VariantType.BOOL, true);
        this.InsertFeature("rakar_forms",                             new HbTag('r', 'k', 'r', 'f'), VariantType.INT, true);
        this.InsertFeature("required_ligatures",                      new HbTag('r', 'l', 'i', 'g'), VariantType.BOOL, true);
        this.InsertFeature("reph_forms",                              new HbTag('r', 'p', 'h', 'f'), VariantType.INT, true);
        this.InsertFeature("right_bounds",                            new HbTag('r', 't', 'b', 'd'), VariantType.INT, false);
        this.InsertFeature("right_to_left_alternates",                new HbTag('r', 't', 'l', 'a'), VariantType.INT, true);
        this.InsertFeature("right_to_left_mirrored_forms",            new HbTag('r', 't', 'l', 'm'), VariantType.INT, true);
        this.InsertFeature("ruby_notation_forms",                     new HbTag('r', 'u', 'b', 'y'), VariantType.INT, false);
        this.InsertFeature("required_variation_alternates",           new HbTag('r', 'v', 'r', 'n'), VariantType.INT, true);
        this.InsertFeature("stylistic_alternates",                    new HbTag('s', 'a', 'l', 't'), VariantType.INT, false);
        this.InsertFeature("scientific_inferiors",                    new HbTag('s', 'i', 'n', 'f'), VariantType.BOOL, false);
        this.InsertFeature("optical_size",                            new HbTag('s', 'i', 'z', 'e'), VariantType.INT, false);
        this.InsertFeature("small_capitals",                          new HbTag('s', 'm', 'c', 'p'), VariantType.BOOL, false);
        this.InsertFeature("simplified_forms",                        new HbTag('s', 'm', 'p', 'l'), VariantType.INT, false);
        this.InsertFeature("stylistic_set_01",                        new HbTag('s', 's', '0', '1'), VariantType.BOOL, false);
        this.InsertFeature("stylistic_set_02",                        new HbTag('s', 's', '0', '2'), VariantType.BOOL, false);
        this.InsertFeature("stylistic_set_03",                        new HbTag('s', 's', '0', '3'), VariantType.BOOL, false);
        this.InsertFeature("stylistic_set_04",                        new HbTag('s', 's', '0', '4'), VariantType.BOOL, false);
        this.InsertFeature("stylistic_set_05",                        new HbTag('s', 's', '0', '5'), VariantType.BOOL, false);
        this.InsertFeature("stylistic_set_06",                        new HbTag('s', 's', '0', '6'), VariantType.BOOL, false);
        this.InsertFeature("stylistic_set_07",                        new HbTag('s', 's', '0', '7'), VariantType.BOOL, false);
        this.InsertFeature("stylistic_set_08",                        new HbTag('s', 's', '0', '8'), VariantType.BOOL, false);
        this.InsertFeature("stylistic_set_09",                        new HbTag('s', 's', '0', '9'), VariantType.BOOL, false);
        this.InsertFeature("stylistic_set_10",                        new HbTag('s', 's', '1', '0'), VariantType.BOOL, false);
        this.InsertFeature("stylistic_set_11",                        new HbTag('s', 's', '1', '1'), VariantType.BOOL, false);
        this.InsertFeature("stylistic_set_12",                        new HbTag('s', 's', '1', '2'), VariantType.BOOL, false);
        this.InsertFeature("stylistic_set_13",                        new HbTag('s', 's', '1', '3'), VariantType.BOOL, false);
        this.InsertFeature("stylistic_set_14",                        new HbTag('s', 's', '1', '4'), VariantType.BOOL, false);
        this.InsertFeature("stylistic_set_15",                        new HbTag('s', 's', '1', '5'), VariantType.BOOL, false);
        this.InsertFeature("stylistic_set_16",                        new HbTag('s', 's', '1', '6'), VariantType.BOOL, false);
        this.InsertFeature("stylistic_set_17",                        new HbTag('s', 's', '1', '7'), VariantType.BOOL, false);
        this.InsertFeature("stylistic_set_18",                        new HbTag('s', 's', '1', '8'), VariantType.BOOL, false);
        this.InsertFeature("stylistic_set_19",                        new HbTag('s', 's', '1', '9'), VariantType.BOOL, false);
        this.InsertFeature("stylistic_set_20",                        new HbTag('s', 's', '2', '0'), VariantType.BOOL, false);
        this.InsertFeature("math_script_style_alternates",            new HbTag('s', 's', 't', 'y'), VariantType.INT, true);
        this.InsertFeature("stretching_glyph_decomposition",          new HbTag('s', 't', 'c', 'h'), VariantType.INT, true);
        this.InsertFeature("subscript",                               new HbTag('s', 'u', 'b', 's'), VariantType.BOOL, false);
        this.InsertFeature("superscript",                             new HbTag('s', 'u', 'p', 's'), VariantType.BOOL, false);
        this.InsertFeature("swash",                                   new HbTag('s', 'w', 's', 'h'), VariantType.INT, false);
        this.InsertFeature("titling",                                 new HbTag('t', 'i', 't', 'l'), VariantType.BOOL, false);
        this.InsertFeature("trailing_jamo_forms",                     new HbTag('t', 'j', 'm', 'o'), VariantType.INT, true);
        this.InsertFeature("traditional_name_forms",                  new HbTag('t', 'n', 'a', 'm'), VariantType.INT, false);
        this.InsertFeature("tabular_figures",                         new HbTag('t', 'n', 'u', 'm'), VariantType.BOOL, false);
        this.InsertFeature("traditional_forms",                       new HbTag('t', 'r', 'a', 'd'), VariantType.INT, false);
        this.InsertFeature("third_widths",                            new HbTag('t', 'w', 'i', 'd'), VariantType.BOOL, false);
        this.InsertFeature("unicase",                                 new HbTag('u', 'n', 'i', 'c'), VariantType.BOOL, false);
        this.InsertFeature("alternate_vertical_metrics",              new HbTag('v', 'a', 'l', 't'), VariantType.INT, false);
        this.InsertFeature("vattu_variants",                          new HbTag('v', 'a', 't', 'u'), VariantType.INT, true);
        this.InsertFeature("vertical_contextual_half_width_spacing",  new HbTag('v', 'c', 'h', 'w'), VariantType.BOOL, false);
        this.InsertFeature("vertical_alternates",                     new HbTag('v', 'e', 'r', 't'), VariantType.INT, false);
        this.InsertFeature("alternate_vertical_half_metrics",         new HbTag('v', 'h', 'a', 'l'), VariantType.BOOL, false);
        this.InsertFeature("vowel_jamo_forms",                        new HbTag('v', 'j', 'm', 'o'), VariantType.INT, true);
        this.InsertFeature("vertical_kana_alternates",                new HbTag('v', 'k', 'n', 'a'), VariantType.INT, false);
        this.InsertFeature("vertical_kerning",                        new HbTag('v', 'k', 'r', 'n'), VariantType.BOOL, false);
        this.InsertFeature("proportional_alternate_vertical_metrics", new HbTag('v', 'p', 'a', 'l'), VariantType.BOOL, false);
        this.InsertFeature("vertical_alternates_and_rotation",        new HbTag('v', 'r', 't', '2'), VariantType.INT, false);
        this.InsertFeature("vertical_alternates_for_rotation",        new HbTag('v', 'r', 't', 'r'), VariantType.INT, false);
        this.InsertFeature("slashed_zero",                            new HbTag('z', 'e', 'r', 'o'), VariantType.BOOL, false);

        // Registered OpenType variation tag.
        this.InsertFeature("italic",                                  new HbTag('i', 't', 'a', 'l'), VariantType.INT, false);
        this.InsertFeature("optical_size",                            new HbTag('o', 'p', 's', 'z'), VariantType.INT, false);
        this.InsertFeature("slant",                                   new HbTag('s', 'l', 'n', 't'), VariantType.INT, false);
        this.InsertFeature("width",                                   new HbTag('w', 'd', 't', 'h'), VariantType.INT, false);
        this.InsertFeature("weight",                                  new HbTag('w', 'g', 'h', 't'), VariantType.INT, false);
    }

    private void ShapeRun(
        ShapedTextDataAdvanced sd,
        int                    start,
        int                    end,
        HbScript               script,
        HbDirection            direction,
        IList<Guid>            fonts,
        int                    span,
        int                    fbIndex,
        int                    prevStart,
        int                    prevEnd
    )
    {
        Guid f = default;
        var fs = sd.Spans[span].FontSize;

        if (fbIndex >= 0 && fbIndex < fonts.Count)
        {
            // Try font from list.
            f = fonts[fbIndex];
        }
        else if (OS.Singleton.HasFeature("system_fonts") && fonts.Count > 0 && (fbIndex == fonts.Count || fbIndex > fonts.Count && start != prevStart))
        {
            // Try system fallback.
            var fdef = fonts[0];
            if (this.FontIsAllowSystemFallback(fdef))
            {
                var text        = sd.Text.Substring(start, 1);
                var fontName    = this.FontGetName(fdef);
                var fontStyle   = this.FontGetStyle(fdef);
                var fontWeight  = this.FontGetWeight(fdef);
                var fontStretch = this.FontGetStretch(fdef);
                var dvar        = this.FontGetVariationCoordinates(fdef);

                wgthTag = this.NameToTag("weight");
			    wdthTag = this.NameToTag("width");
			    italTag = this.NameToTag("italic");

                if (dvar.ContainsKey((uint)wgthTag))
                {
                    fontWeight = dvar[(uint)wgthTag];
                }

                if (dvar.ContainsKey((uint)wdthTag))
                {
                    fontStretch = dvar[(uint)wdthTag];
                }

                if (dvar.ContainsKey((uint)italTag) && dvar[(uint)italTag] == 1)
                {
                    fontStyle |= FontStyle.FONT_ITALIC;
                }

                var scriptCode = ((HbTag)(uint)script).ToString();
                var locale     = string.IsNullOrEmpty(sd.Spans[span].Language) ? TranslationServer.Singleton.GetToolLocale() : sd.Spans[span].Language;

                var fallbackFontName = OS.Singleton.GetSystemFontPathForText(
                    fontName,
                    text,
                    locale,
                    scriptCode,
                    fontWeight,
                    fontStretch,
                    fontStyle.HasFlag(FontStyle.FONT_ITALIC)
                );

                #if GDEXTENSION
                for (var fb = 0; fb < fallbackFontName.Count; fb++)
                {
                    var fallback = fallbackFontName[fb];
                #else
                foreach (var fallback in fallbackFontName)
                {
                #endif
                    var key = new SystemFontKey(fallback, fontStyle.HasFlag(FontStyle.FONT_ITALIC), fontWeight, fontStretch, fdef, this);
                    if (this.systemFonts.TryGetValue(key, out var systemFont))
                    {
                        var sysfCache = systemFont;
                        var bestScore = 0;
                        var bestMatch = -1;
                        for (var faceIdx = 0; faceIdx < sysfCache.Var.Count; faceIdx++)
                        {
                            var fontCacheRect = sysfCache.Var[faceIdx];

                            if (!this.FontHasChar(fontCacheRect.Id, text[0]))
                            {
                                continue;
                            }

                            var style   = this.FontGetStyle(fontCacheRect.Id);
                            var weight  = this.FontGetWeight(fontCacheRect.Id);
                            var stretch = this.FontGetStretch(fontCacheRect.Id);
                            var score   = 20 - Math.Abs(weight - fontWeight) / 50;

                            score += 20 - Math.Abs(stretch - fontStretch) / 10;

                            if (style.HasFlag(FontStyle.FONT_ITALIC) == fontStyle.HasFlag(FontStyle.FONT_ITALIC))
                            {
                                score += 30;
                            }

                            if (score >= bestScore)
                            {
                                bestScore = score;
                                bestMatch = faceIdx;
                            }

                            if (bestScore == 70)
                            {
                                break;
                            }
                        }
                        if (bestMatch != -1)
                        {
                            f = sysfCache.Var[bestMatch].Id;
                        }
                    }

                    if (f != default)
                    {
                        if (this.systemFonts.TryGetValue(key, out var systemFonts))
                        {
                            var sysfCache = systemFonts;

                            if (sysfCache.MaxVar == sysfCache.Var.Count)
                            {
                                // All subfonts already tested, skip.
                                continue;
                            }
                        }

                        if (!this.systemFontData.TryGetValue(fallback, out var systemFontData))
                        {
                            systemFontData = File.ReadAllBytes(fallback);
                            this.systemFontData[fallback] = systemFontData;
                        }

                        var fontData = systemFontData;

                        var sysf = new SystemFontCacheRec
                        {
                            Id = this.CreateFont()
                        };

                        this.FontSetData(sysf.Id, fontData);

                        var @var       = dvar;
                        // Select matching style from collection.
                        var bestScore = 0;
                        var bestMatch = -1;

                        for (var faceIdx = 0; faceIdx < this.FontGetFaceCount(sysf.Id); faceIdx++)
                        {
                            this.FontSetFaceIndex(sysf.Id, faceIdx);
                            if (!this.FontHasChar(sysf.Id, text[0]))
                            {
                                continue;
                            }
                            var style   = this.FontGetStyle(sysf.Id);
                            var weight  = this.FontGetWeight(sysf.Id);
                            var stretch = this.FontGetStretch(sysf.Id);
                            var score   = 20 - Math.Abs(weight - fontWeight) / 50;
                            score += 20 - Math.Abs(stretch - fontStretch) / 10;

                            if (style.HasFlag(FontStyle.FONT_ITALIC) == fontStyle.HasFlag(FontStyle.FONT_ITALIC))
                            {
                                score += 30;
                            }

                            if (score >= bestScore)
                            {
                                bestScore = score;
                                bestMatch = faceIdx;
                            }

                            if (bestScore == 70)
                            {
                                break;
                            }
                        }

                        if (bestMatch == -1)
                        {
                            this.FreeId(sysf.Id);
                            continue;
                        }
                        else
                        {
                            this.FontSetFaceIndex(sysf.Id, bestMatch);
                        }

                        sysf.Index = bestMatch;

                        // If it's a variable font, apply weight, stretch and italic coordinates to match requested style.
                        if (bestScore != 70)
                        {
                            var ftr = this.FontSupportedVariationList(sysf.Id);
                            if (ftr.ContainsKey((uint)wdthTag))
                            {
                                @var[(uint)wdthTag] = fontStretch;
                                this.FontSetStretch(sysf.Id, fontStretch);
                            }
                            if (ftr.ContainsKey((uint)wgthTag))
                            {
                                @var[(uint)wgthTag] = fontWeight;
                                this.FontSetWeight(sysf.Id, fontWeight);
                            }
                            if (fontStyle.HasFlag(FontStyle.FONT_ITALIC) && ftr.ContainsKey((uint)italTag))
                            {
                                @var[(uint)italTag] = 1;
                                this.FontSetStyle(sysf.Id, this.FontGetStyle(sysf.Id) | FontStyle.FONT_ITALIC);
                            }
                        }

                        this.FontSetAntialiasing(sysf.Id, key.Antialiasing); // void TextServerAdvanced::_font_set_antialiasing(Guid fontId, TextServer::FontAntialiasing p_antialiasing)
                        this.FontSetGenerateMipmaps(sysf.Id, key.Mipmaps); // void TextServerAdvanced::_font_set_generate_mipmaps(Guid fontId, bool p_generate_mipmaps)
                        this.FontSetMultichannelSignedDistanceField(sysf.Id, key.Msdf); // void TextServerAdvanced::_font_set_multichannel_signed_distance_field(Guid fontId, bool p_msdf)
                        this.FontSetMsdfPixelRange(sysf.Id, key.MsdfRange); // void TextServerAdvanced::_font_set_msdf_pixel_range(Guid fontId, int64_t p_msdf_pixel_range)
                        this.FontSetMsdfSize(sysf.Id, key.MsdfSourceSize); // void TextServerAdvanced::_font_set_msdf_size(Guid fontId, int64_t p_msdf_size)
                        this.FontSetFixedSize(sysf.Id, key.FixedSize); // void TextServerAdvanced::_font_set_fixed_size(Guid fontId, int64_t p_fixed_size)
                        this.FontSetForceAutohinter(sysf.Id, key.ForceAutohinter); // void TextServerAdvanced::_font_set_force_autohinter(Guid fontId, bool p_force_autohinter)
                        this.FontSetHinting(sysf.Id, key.Hinting); // void TextServerAdvanced::_font_set_hinting(Guid fontId, TextServer::Hinting p_hinting)
                        this.FontSetSubpixelPositioning(sysf.Id, key.SubpixelPositioning); // void TextServerAdvanced::_font_set_subpixel_positioning(Guid fontId, TextServer::SubpixelPositioning p_subpixel)
                        this.FontSetVariationCoordinates(sysf.Id, @var); // void TextServerAdvanced::_font_set_variation_coordinates(Guid fontId, const Dictionary &p_variation_coordinates)
                        this.FontSetOversampling(sysf.Id, key.Oversampling); // void TextServerAdvanced::_font_set_oversampling(Guid fontId, double p_oversampling)
                        this.FontSetEmbolden(sysf.Id, key.Embolden); // void TextServerAdvanced::_font_set_embolden(Guid fontId, double p_strength)
                        this.FontSetTransform(sysf.Id, key.Transform); // void TextServerAdvanced::_font_set_transform(Guid fontId, const Transform2D &p_transform)

                        if (this.systemFonts.TryGetValue(key, out systemFont))
                        {
                            systemFont.Var.Add(sysf);
                        }
                        else
                        {
                            var sysfCache = this.systemFonts[key];
                            sysfCache.MaxVar = this.FontGetFaceCount(sysf.Id);
                            sysfCache.Var.Add(sysf);
                        }
                        f = sysf.Id;
                    }
                    break;
                }
            }
        }

        if (f == default)
        {
            // No valid font, use fallback hex code boxes.
            for (var i = start; i < end; i++)
            {
                if (sd.PreserveInvalid || sd.PreserveControl && char.IsControl(sd.Text[i]))
                {
                    var gl = new Glyph
                    {
                        Start    = i + sd.Start,
                        End      = i + 1 + sd.Start,
                        Count    = 1,
                        Index    = sd.Text[i],
                        FontSize = fs,
                        FontId   = default
                    };

                    if (direction is HbDirection.RightToLeft or HbDirection.BottomToTop)
                    {
                        gl.Flags |= GraphemeFlag.GRAPHEME_IS_RTL;
                    }
                    if (sd.Orientation == Orientation.ORIENTATION_HORIZONTAL)
                    {
                        gl.Advance = this.GetHexCodeBoxSize(fs, gl.Index).X;
                        sd.Ascent  = Math.Max(sd.Ascent, this.GetHexCodeBoxSize(fs, gl.Index).Y);
                    }
                    else
                    {
                        gl.Advance   = this.GetHexCodeBoxSize(fs, gl.Index).Y;
                        gl.YOff      = this.GetHexCodeBoxSize(fs, gl.Index).Y;
                        gl.XOff      = -(float)Math.Round(this.GetHexCodeBoxSize(fs, gl.Index).X * 0.5);
                        sd.Ascent  = Math.Max(sd.Ascent, Math.Round(this.GetHexCodeBoxSize(fs, gl.Index).X * 0.5));
                        sd.Descent = Math.Max(sd.Descent, Math.Round(this.GetHexCodeBoxSize(fs, gl.Index).X * 0.5));
                    }
                    sd.Width += gl.Advance;

                    sd.Glyphs.Add(gl);
                }
            }
            return;
        }

        var fd = this.fontOwner.GetOrNull(f);
        if (ERR_FAIL_NULL(fd))
        {
            return;
        }

        using var _ = new MutexLock(fd.Mutex);

        var fss                     = this.GetSize(fd, fs);
        var hbFont                  = this.FontGetHbHandle(f, fs);
        var scale                   = this.FontGetScale(f, fs);
        var spSp                    = (double)sd.ExtraSpacing[(int)SpacingType.SPACING_SPACE];
        var spGl                    = (double)sd.ExtraSpacing[(int)SpacingType.SPACING_GLYPH];
        var lastRun                 = sd.End == end;
        var ea                      = this.GetExtraAdvance(f, fs);
        var fontSubpixelPositioning = this.FontGetSubpixelPositioning(f);
        var subpos                  = scale != 1.0 || fontSubpixelPositioning is SubpixelPositioning.SUBPIXEL_POSITIONING_ONE_HALF or SubpixelPositioning.SUBPIXEL_POSITIONING_ONE_QUARTER || fontSubpixelPositioning == SubpixelPositioning.SUBPIXEL_POSITIONING_AUTO && fs <= (int)SubpixelPositioning.SUBPIXEL_POSITIONING_ONE_HALF_MAX_SIZE;

        if (ERR_FAIL_NULL(hbFont))
        {
            return;
        }

        ASSERT_NOT_NULL(sd.HbBuffer);

        sd.HbBuffer.ClearContents();
        sd.HbBuffer.Direction = direction;

        var flags = (start == 0 ? HbBufferFlags.BeginningOfText : 0) | (end == sd.Text.Length ? HbBufferFlags.EndOfText : 0);

        if (sd.PreserveControl)
        {
            flags |= HbBufferFlags.PreserveDefaultIgnorables;
        }
        else
        {
            flags |= HbBufferFlags.Default;
        }

        #if HB_VERSION_ATLEAST_5_1_0
        flags |= (HbBufferFlags)128;
        #endif

        sd.HbBuffer.Flags  = flags;
        sd.HbBuffer.Script = script;

        if (string.IsNullOrEmpty(sd.Spans[span].Language))
        {
            var lang = new HbLanguage(TranslationServer.Singleton.GetToolLocale().ToASCII());

            sd.HbBuffer.Language = lang;
        }
        else
        {
            ASSERT_NOT_NULL(sd.Spans[span].Language);

            var lang = new HbLanguage(sd.Spans[span].Language!.ToASCII());
            sd.HbBuffer.Language = lang;
        }

        sd.HbBuffer.AddUtf32(sd.Text[start..end]);

        this.AddFeatures(this.FontGetOpentypeFeatureOverrides(f), out var ftrs);
        this.AddFeatures(sd.Spans[span].Features, out ftrs);

        hbFont.Shape(sd.HbBuffer, ftrs);

        var glyphInfo = sd.HbBuffer.GlyphInfos;
        var glyphPos  = sd.HbBuffer.GlyphPositions;

        var glyphCount = glyphInfo.Length;

        var mod = 0;
        if (fd.Antialiasing == FontAntialiasing.FONT_ANTIALIASING_LCD)
        {
            var layout = GLOBAL_GET<FontLCDSubpixelLayout>("gui/theme/lcd_subpixel_layout");
            if (layout != FontLCDSubpixelLayout.FONT_LCD_SUBPIXEL_LAYOUT_NONE)
            {
                mod = (int)layout << 24;
            }
        }

        // Process glyphs.
        if (glyphCount > 0)
        {
            var w = new Glyph[glyphCount];

            var currentEnd       = (direction is HbDirection.RightToLeft or HbDirection.BottomToTop) ? end : 0;
            var lastClusterId    = uint.MaxValue;
            var lastClusterIndex = 0;
            var lastClusterValid = true;

            for (var i = 0; i < glyphCount; i++)
            {
                if (i > 0 && lastClusterId != glyphInfo[i].Cluster)
                {
                    if (direction is HbDirection.RightToLeft or HbDirection.BottomToTop)
                    {
                        currentEnd = w[lastClusterIndex].Start;
                    }
                    else
                    {
                        for (var j = lastClusterIndex; j < i; j++)
                        {
                            w[j].End = (int)glyphInfo[i].Cluster;
                        }
                    }

                    if (direction is HbDirection.RightToLeft or HbDirection.BottomToTop)
                    {
                        w[lastClusterIndex].Flags |= GraphemeFlag.GRAPHEME_IS_RTL;
                    }

                    if (lastClusterValid)
                    {
                        w[lastClusterIndex].Flags |= GraphemeFlag.GRAPHEME_IS_VALID;
                    }

                    w[lastClusterIndex].Count = (byte)(i - lastClusterIndex);
                    lastClusterIndex = i;
                    lastClusterValid = true;
                }

                lastClusterId = glyphInfo[i].Cluster;

                var gl = w[i];

                gl = new Glyph
                {
                    Start    = (int)glyphInfo[i].Cluster,
                    End      = currentEnd,
                    Count    = 0,
                    FontId   = f,
                    FontSize = fs
                };

                if ((glyphInfo[i].Mask & (uint)HbGlyphFlags.UnsafeToBreak) != 0)
                {
                    gl.Flags |= GraphemeFlag.GRAPHEME_IS_CONNECTED;
                }

                #if HB_VERSION_ATLEAST_5_1_0
                if ((glyphInfo[i].Mask & 4) != 0)
                {
                    gl.Flags |= GraphemeFlag.GRAPHEME_IS_SAFE_TO_INSERT_TATWEEL;
                }
                #endif

                gl.Index = (int)glyphInfo[i].Codepoint;

                if (gl.Index != 0)
                {
                    this.EnsureGlyph(fd, fss, gl.Index | mod);
                    gl.Advance = sd.Orientation == Orientation.ORIENTATION_HORIZONTAL
                        ? subpos ? (float)(glyphPos[i].XAdvance / (64.0 / scale) + ea) : (float)Math.Round(glyphPos[i].XAdvance / (64.0 / scale) + ea)
                        : -(float)Math.Round(glyphPos[i].YAdvance / (64.0 / scale));

                    gl.XOff = subpos ? (float)(glyphPos[i].XOffset / (64.0 / scale)) : (float)Math.Round(glyphPos[i].XOffset / (64.0 / scale));
                    gl.YOff = -(float)Math.Round(glyphPos[i].YOffset / (64.0 / scale));
                }

                if (!lastRun || i < glyphCount - 1)
                {
                    // Do not add extra spacing to the last glyph of the string.
                    if (spSp > 0 && char.IsWhiteSpace(sd.Text[(int)glyphInfo[i].Cluster]))
                    {
                        gl.Advance += (float)spSp;
                    }
                    else
                    {
                        gl.Advance += (float)spGl;
                    }
                }

                lastClusterValid = sd.PreserveControl
                    ? lastClusterValid && (glyphInfo[i].Codepoint != 0 || sd.Text[(int)glyphInfo[i].Cluster] == 0x0009 || char.IsWhiteSpace(sd.Text[(int)glyphInfo[i].Cluster]) && gl.Advance != 0 || !char.IsWhiteSpace(sd.Text[(int)glyphInfo[i].Cluster]) && sd.Text[(int)glyphInfo[i].Cluster].IsLinebreak())
                    : lastClusterValid && (glyphInfo[i].Codepoint != 0 || sd.Text[(int)glyphInfo[i].Cluster] == 0x0009 || char.IsWhiteSpace(sd.Text[(int)glyphInfo[i].Cluster]) && gl.Advance != 0 || !char.IsWhiteSpace(sd.Text[(int)glyphInfo[i].Cluster]) && !sd.Text[(int)glyphInfo[i].Cluster].IsLinebreak());
            }
            if (direction is HbDirection.LeftToRight or HbDirection.TopToBottom)
            {
                for (var j = lastClusterIndex; j < glyphCount; j++)
                {
                    w[j].End = end;
                }
            }

            w[lastClusterIndex].Count = (byte)(glyphCount - lastClusterIndex);

            if (direction is HbDirection.RightToLeft or HbDirection.BottomToTop)
            {
                w[lastClusterIndex].Flags |= GraphemeFlag.GRAPHEME_IS_RTL;
            }

            if (lastClusterValid)
            {
                w[lastClusterIndex].Flags |= GraphemeFlag.GRAPHEME_IS_VALID;
            }

            // Fallback.
            var failedSubrunStart = end + 1;
            var failedSubrunEnd   = start;

            for (var i = 0; i < glyphCount; i++)
            {
                if (w[i].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_VALID))
                {
                    if (failedSubrunStart != end + 1)
                    {
                        this.ShapeRun(
                            sd,
                            failedSubrunStart,
                            failedSubrunEnd,
                            script,
                            direction,
                            fonts,
                            span,
                            fbIndex + 1,
                            start,
                            end
                        );

                        failedSubrunStart = end + 1;
                        failedSubrunEnd = start;
                    }

                    for (var j = 0; j < w[i].Count; j++)
                    {
                        if (sd.Orientation == Orientation.ORIENTATION_HORIZONTAL)
                        {
                            sd.Ascent  = Math.Max(sd.Ascent, -w[i + j].YOff);
                            sd.Descent = Math.Max(sd.Descent, w[i + j].YOff);
                        }
                        else
                        {
                            var gla = Math.Round(this.FontGetGlyphAdvance(f, fs, w[i + j].Index).X * 0.5);

                            sd.Ascent = Math.Max(sd.Ascent, gla);
                            sd.Descent = Math.Max(sd.Descent, gla);
                        }

                        sd.Width += w[i + j].Advance;
                        w[i + j].Start += sd.Start;
                        w[i + j].End += sd.Start;
                        sd.Glyphs.Add(w[i + j]);
                    }
                }
                else
                {
                    if (failedSubrunStart >= w[i].Start)
                    {
                        failedSubrunStart = w[i].Start;
                    }

                    if (failedSubrunEnd <= w[i].End)
                    {
                        failedSubrunEnd = w[i].End;
                    }
                }

                i += w[i].Count - 1;
            }

            if (failedSubrunStart != end + 1)
            {
                this.ShapeRun(
                    sd,
                    failedSubrunStart,
                    failedSubrunEnd,
                    script,
                    direction,
                    fonts,
                    span,
                    fbIndex + 1,
                    start,
                    end
                );
            }
            sd.Ascent  = Math.Max(sd.Ascent, this.FontGetAscent(f, fs));
            sd.Descent = Math.Max(sd.Descent, this.FontGetDescent(f, fs));
            sd.Upos    = Math.Max(sd.Upos, this.FontGetUnderlinePosition(f, fs));
            sd.Uthk    = Math.Max(sd.Uthk, this.FontGetUnderlineThickness(f, fs));
        }
    }

    private bool ShapeSubstr(ShapedTextDataAdvanced newSd, ShapedTextDataAdvanced sd, int start, int length)
    {
        if (newSd.Valid)
        {
            return true;
        }

        newSd.HbBuffer = new HbBuffer();

        newSd.LineBreaksValid       = sd.LineBreaksValid;
        newSd.JustificationOpsValid = sd.JustificationOpsValid;
        newSd.SortValid             = false;
        newSd.Upos                  = sd.Upos;
        newSd.Uthk                  = sd.Uthk;

        if (length > 0)
        {
            newSd.Text       = sd.Text.Substring(start - sd.Start, length);
            newSd.Utf16      = newSd.Text.ToUTF16();
            newSd.ScriptIter = new ScriptIterator(newSd.Text, 0, newSd.Text.Length);

            var sdSize   = sd.Glyphs.Count;
            var sdGlyphs = sd.Glyphs;

            for (var ov = 0; ov < sd.BidiOverride.Count; ov++)
            {
                if (sd.BidiOverride[ov].X >= start + length || sd.BidiOverride[ov].Y <= start)
                {
                    continue;
                }

                var ovStart      = ConvertPosInv(sd, sd.BidiOverride[ov].X);
                var currentStart = Math.Max(0, ConvertPosInv(sd, start) - ovStart);
                var currentEnd   = Math.Min(ConvertPosInv(sd, currentStart + length), ConvertPosInv(sd, sd.BidiOverride[ov].Y)) - ovStart;

                if (ERR_FAIL_COND_V_MSG(currentStart < 0 || currentEnd - currentStart > newSd.Utf16.Length, "Invalid BiDi override range."))
                {
                    return false;
                }

                try
                {
                    // Create temporary line bidi & shape.
                    var bidi = sd.BidiIter[ov].SetLine(currentStart, currentEnd);

                    newSd.BidiIter.Add(bidi);

                    var bidiRunCount = bidi.CountRuns();

                    for (var i = 0; i < bidiRunCount; i++)
                    {
                        bidi.GetVisualRun(i, out var outBidiRunStart, out var outBidiRunLength);

                        var bidiRunStart = ConvertPos(sd, ovStart + currentStart + outBidiRunStart);
                        var bidiRunEnd   = ConvertPos(sd, ovStart + currentStart + outBidiRunStart + outBidiRunLength);

                        for (var j = 0; j < sdSize; j++)
                        {
                            if (sdGlyphs[j].Start >= bidiRunStart && sdGlyphs[j].End <= bidiRunEnd)
                            {
                                // Copy glyphs.
                                var gl           = sdGlyphs[j];
                                var key          = new object();
                                var findEmbedded = false;

                                if (gl.Count == 1)
                                {
                                    foreach (var item in sd.Objects)
                                    {
                                        if (item.Value.Pos == gl.Start)
                                        {
                                            findEmbedded = true;
                                            key          = item.Key;

                                            newSd.Objects[key] = item.Value;

                                            break;
                                        }
                                    }
                                }

                                if (findEmbedded)
                                {
                                    if (newSd.Orientation == Orientation.ORIENTATION_HORIZONTAL)
                                    {
                                        newSd.Objects[key].Rect = newSd.Objects[key].Rect with
                                        {
                                            Position = newSd.Objects[key].Rect.Position with
                                            {
                                                X = (RealT)newSd.Width
                                            }
                                        };

                                        newSd.Width += newSd.Objects[key].Rect.Size.X;
                                    }
                                    else
                                    {
                                        newSd.Objects[key].Rect = newSd.Objects[key].Rect with
                                        {
                                            Position = newSd.Objects[key].Rect.Position with
                                            {
                                                Y = (RealT)newSd.Width
                                            }
                                        };

                                        newSd.Width += newSd.Objects[key].Rect.Size.Y;
                                    }
                                }
                                else
                                {
                                    if (gl.FontId != default)
                                    {
                                        if (newSd.Orientation == Orientation.ORIENTATION_HORIZONTAL)
                                        {
                                            newSd.Ascent  = Math.Max(newSd.Ascent, Math.Max(this.FontGetAscent(gl.FontId, gl.FontSize), -gl.YOff));
                                            newSd.Descent = Math.Max(newSd.Descent, Math.Max(this.FontGetDescent(gl.FontId, gl.FontSize), gl.YOff));
                                        }
                                        else
                                        {
                                            newSd.Ascent  = Math.Max(newSd.Ascent, Math.Round(this.FontGetGlyphAdvance(gl.FontId, gl.FontSize, gl.Index).X * 0.5));
                                            newSd.Descent = Math.Max(newSd.Descent, Math.Round(this.FontGetGlyphAdvance(gl.FontId, gl.FontSize, gl.Index).X * 0.5));
                                        }
                                    }
                                    else if (newSd.PreserveInvalid || newSd.PreserveControl && char.IsControl((char)gl.Index))
                                    {
                                        // Glyph not found, replace with hex code box.
                                        if (newSd.Orientation == Orientation.ORIENTATION_HORIZONTAL)
                                        {
                                            newSd.Ascent = Math.Max(newSd.Ascent, this.GetHexCodeBoxSize(gl.FontSize, gl.Index).Y);
                                        }
                                        else
                                        {
                                            newSd.Ascent  = Math.Max(newSd.Ascent, Math.Round(this.GetHexCodeBoxSize(gl.FontSize, gl.Index).X * 0.5));
                                            newSd.Descent = Math.Max(newSd.Descent, Math.Round(this.GetHexCodeBoxSize(gl.FontSize, gl.Index).X * 0.5));
                                        }
                                    }
                                    newSd.Width += gl.Advance * gl.Repeat;
                                }
                                newSd.Glyphs.Add(gl);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    if (ERR_FAIL_COND_V_MSG(true, exception.Message))
                    {
                        return false;
                    }
                }
            }
        }

        Realign(newSd);

        newSd.Valid = true;

        return true;
    }

    private bool ShapedTextShape(Guid shaped)
    {
        lock (padlock)
        {
            var sd = this.shapedOwner.GetOrNull(shaped);

            if (ERR_FAIL_NULL(sd))
            {
                return false;
            }

            using var _ = new MutexLock(sd.Mutex);

            if (sd.Valid)
            {
                return true;
            }

            Invalidate(sd, false);

            if (sd.Parent != default)
            {
                this.ShapedTextShape(sd.Parent);

                var parentSd = this.shapedOwner.GetOrNull(sd.Parent)!;

                return !ERR_FAIL_COND_V(!parentSd.Valid) && !ERR_FAIL_COND_V(!this.ShapeSubstr(sd, parentSd, sd.Start, sd.End - sd.Start));
            }

            if (string.IsNullOrEmpty(sd.Text))
            {
                sd.Valid = true;
                return true;
            }

            sd.Utf16 = sd.Text.ToUTF16();

            var data = sd.Utf16;

            // Create script iterator.
            sd.ScriptIter ??= new ScriptIterator(sd.Text!, 0, sd.Text!.Length);

            var baseParaDirection = UBiDiDirection.UBIDI_LTR;

            switch (sd.Direction)
            {
                case Direction.DIRECTION_LTR:
                    sd.ParaDirection = Direction.DIRECTION_LTR;
                    baseParaDirection = UBiDiDirection.UBIDI_LTR;

                    break;

                case Direction.DIRECTION_RTL:
                    sd.ParaDirection = Direction.DIRECTION_RTL;
                    baseParaDirection = UBiDiDirection.UBIDI_RTL;

                    break;

                case Direction.DIRECTION_INHERITED:
                case Direction.DIRECTION_AUTO:
                    {
                        var direction = BiDi.GetBaseDirection(data);

                        if (direction != UBiDiDirection.UBIDI_NEUTRAL)
                        {
                            sd.ParaDirection = (direction == UBiDiDirection.UBIDI_RTL) ? Direction.DIRECTION_RTL : Direction.DIRECTION_LTR;
                            baseParaDirection = direction;
                        }
                        else
                        {
                            sd.ParaDirection = Direction.DIRECTION_LTR;
                            baseParaDirection = UBiDiDirection.UBIDI_LTR;
                        }
                    }

                    break;
            }

            if (sd.BidiOverride.Count == 0)
            {
                sd.BidiOverride.Add(new(sd.Start, sd.End, (int)Direction.DIRECTION_INHERITED));
            }

            for (var ov = 0; ov < sd.BidiOverride.Count; ov++)
            {
                // Create BiDi iterator.
                var start = ConvertPosInv(sd, sd.BidiOverride[ov].X - sd.Start);
                var end   = ConvertPosInv(sd, sd.BidiOverride[ov].Y - sd.Start);

                if (start < 0 || end - start > sd.Utf16.Length)
                {
                    continue;
                }

                try
                {
                    var biDi = new BiDi(end - start, 0);

                    switch ((Direction)sd.BidiOverride[ov].Z)
                    {
                        case Direction.DIRECTION_LTR:
                            biDi.SetPara(data[start..], (byte)UBiDiDirection.UBIDI_LTR, null);
                            break;
                        case Direction.DIRECTION_RTL:
                            biDi.SetPara(data[start..], (byte)UBiDiDirection.UBIDI_RTL, null);
                            break;
                        case Direction.DIRECTION_INHERITED:
                            biDi.SetPara(data[start..], (byte)baseParaDirection, null);
                            break;
                        case Direction.DIRECTION_AUTO:
                            {
                                var direction = BiDi.GetBaseDirection(data[start..]);
                                if (direction != UBiDiDirection.UBIDI_NEUTRAL)
                                {
                                    biDi.SetPara(data[start..], (byte)direction, null);
                                }
                                else
                                {
                                    biDi.SetPara(data[start..], (byte)baseParaDirection, null);
                                }
                            }
                            break;
                    }

                    sd.BidiIter.Add(biDi);

                    var bidiRunCount = biDi.CountRuns();

                    for (var i = 0; i < bidiRunCount; i++)
                    {
                        var bidiRunDirection = HbDirection.Invalid;
                        var isRtl = biDi.GetVisualRun(i, out var visualBidiRunStart, out var visualBidiRunLength) == UBiDiDirection.UBIDI_LTR;
                        switch (sd.Orientation)
                        {
                            case Orientation.ORIENTATION_HORIZONTAL:
                                bidiRunDirection = isRtl ? HbDirection.LeftToRight : HbDirection.RightToLeft;

                                break;
                            case Orientation.ORIENTATION_VERTICAL:
                                bidiRunDirection = isRtl ? HbDirection.TopToBottom : HbDirection.BottomToTop;

                                break;
                        }

                        var bidiRunStart = ConvertPos(sd, start + visualBidiRunStart);
                        var bidiRunEnd   = ConvertPos(sd, start + visualBidiRunStart + visualBidiRunLength);

                        // Shape runs.

                        var scrFrom  = isRtl ? 0 : sd.ScriptIter.ScriptRanges.Count - 1;
                        var scrTo    = isRtl ? sd.ScriptIter.ScriptRanges.Count : -1;
                        var scrDelta = isRtl ? +1 : -1;

                        for (var j = scrFrom; j != scrTo; j += scrDelta)
                        {
                            if (sd.ScriptIter.ScriptRanges[j].Start < bidiRunEnd && sd.ScriptIter.ScriptRanges[j].End > bidiRunStart)
                            {
                                var scriptRunStart = Math.Max(sd.ScriptIter.ScriptRanges[j].Start, bidiRunStart);
                                var scriptRunEnd   = Math.Min(sd.ScriptIter.ScriptRanges[j].End, bidiRunEnd);
                                var scriptCode     = ((HbTag)(uint)sd.ScriptIter.ScriptRanges[j].Script).ToString();

                                var spnFrom  = isRtl ? 0 : sd.Spans.Count - 1;
                                var spnTo    = isRtl ? sd.Spans.Count : -1;
                                var spnDelta = isRtl ? +1 : -1;

                                for (var k = spnFrom; k != spnTo; k += spnDelta)
                                {
                                    var span = sd.Spans[k];

                                    if (span.Start - sd.Start >= scriptRunEnd || span.End - sd.Start <= scriptRunStart)
                                    {
                                        continue;
                                    }
                                    if (span.EmbeddedKey != null)
                                    {
                                        // Embedded object.
                                        if (sd.Orientation == Orientation.ORIENTATION_HORIZONTAL)
                                        {
                                            sd.Objects[span.EmbeddedKey].Rect = sd.Objects[span.EmbeddedKey].Rect with
                                            {
                                                Position = sd.Objects[span.EmbeddedKey].Rect.Position with { X = (RealT)sd.Width }
                                            };

                                            sd.Width += sd.Objects[span.EmbeddedKey].Rect.Size.X;
                                        }
                                        else
                                        {
                                            sd.Objects[span.EmbeddedKey].Rect = sd.Objects[span.EmbeddedKey].Rect with
                                            {
                                                Position = sd.Objects[span.EmbeddedKey].Rect.Position with { Y = (RealT)sd.Width }
                                            };

                                            sd.Width += sd.Objects[span.EmbeddedKey].Rect.Size.Y;
                                        }
                                        var gl = new Glyph
                                        {
                                            Start   = span.Start,
                                            End     = span.End,
                                            Count   = 1,
                                            Flags   = GraphemeFlag.GRAPHEME_IS_VALID | GraphemeFlag.GRAPHEME_IS_VIRTUAL,
                                            Advance = sd.Orientation == Orientation.ORIENTATION_HORIZONTAL
                                                ? sd.Objects[span.EmbeddedKey].Rect.Size.X
                                                : sd.Objects[span.EmbeddedKey].Rect.Size.Y
                                        };

                                        sd.Glyphs.Add(gl);
                                    }
                                    else
                                    {
                                        var fonts        = new List<Guid>();
                                        var fontsScrOnly = new List<Guid>();
                                        var fontsNoMatch = new List<Guid>();

                                        var fontCount = span.Fonts.Count;
                                        if (fontCount > 0)
                                        {
                                            fonts.Add(sd.Spans[k].Fonts[0]);
                                        }
                                        for (var l = 1; l < fontCount; l++)
                                        {
                                            if (this.FontIsScriptSupported(span.Fonts[l], scriptCode))
                                            {
                                                if (this.FontIsLanguageSupported(span.Fonts[l], span.Language!))
                                                {
                                                    fonts.Add(sd.Spans[k].Fonts[l]);
                                                }
                                                else
                                                {
                                                    fontsScrOnly.Add(sd.Spans[k].Fonts[l]);
                                                }
                                            }
                                            else
                                            {
                                                fontsNoMatch.Add(sd.Spans[k].Fonts[l]);
                                            }
                                        }

                                        fonts.AddRange(fontsScrOnly);
                                        fonts.AddRange(fontsNoMatch);

                                        this.ShapeRun(
                                            sd,
                                            Math.Max(sd.Spans[k].Start - sd.Start, scriptRunStart),
                                            Math.Min(sd.Spans[k].End - sd.Start, scriptRunEnd),
                                            sd.ScriptIter.ScriptRanges[j].Script,
                                            bidiRunDirection,
                                            fonts,
                                            k,
                                            0,
                                            0,
                                            0
                                        );
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ERR_FAIL_COND_V_MSG(true, ex.Message);

                    return false;
                }
            }

            Realign(sd);

            sd.Valid = true;
            return sd.Valid;
        }
    }
    #endregion private methods

    #region protected virtual methods
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.funcs?.Dispose();
            }

            this.disposed = true;
        }
    }
    #endregion protected virtual methods

    #region protected methods
    protected void FullCopy(ShapedTextDataAdvanced shaped)
    {
        var parent = this.shapedOwner.GetOrNull(shaped.Parent)!;

        foreach (var item in parent.Objects)
        {
            if (item.Value.Pos >= shaped.Start && item.Value.Pos < shaped.End)
            {
                shaped.Objects[item.Key] = item.Value;
            }
        }

        for (var i = 0; i < parent.Spans.Count; i++)
        {
            var span = parent.Spans[i];
            if (span.Start >= shaped.End || span.End <= shaped.Start)
            {
                continue;
            }

            span.Start = Math.Max(shaped.Start, span.Start);
            span.End = Math.Min(shaped.End, span.End);

            shaped.Spans.Add(span);
        }

        shaped.Parent = default;
    }
    #endregion protected methods

    #region public methods
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion public methods

    #region public overrided methods
    public override Guid CreateShapedText(Direction direction = Direction.DIRECTION_AUTO, Orientation orientation = Orientation.ORIENTATION_HORIZONTAL)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND_V_MSG(direction == Direction.DIRECTION_INHERITED, "Invalid text direction."))
            {
                return default;
            }

            var sd = new ShapedTextDataAdvanced
            {
                HbBuffer    = new HbBuffer(),
                Direction   = direction,
                Orientation = orientation
            };

            return this.shapedOwner.Initialize(sd);
        }
    }
    public override double FontGetAscent(Guid fontId, int size) => throw new NotImplementedException();
    public override double FontGetDescent(Guid fontId, int size) => throw new NotImplementedException();
    public override bool HasFeature(Feature feature)
    {
        switch (feature)
        {
            case Feature.FEATURE_SIMPLE_LAYOUT:
            case Feature.FEATURE_BIDI_LAYOUT:
            case Feature.FEATURE_VERTICAL_LAYOUT:
            case Feature.FEATURE_SHAPING:
            case Feature.FEATURE_KASHIDA_JUSTIFICATION:
            case Feature.FEATURE_BREAK_ITERATORS:
            case Feature.FEATURE_FONT_BITMAP:
            #if MODULE_FREETYPE_ENABLED
            case Feature.FEATURE_FONT_DYNAMIC:
            #endif
            #if MODULE_MSDFGEN_ENABLED
            case Feature.FEATURE_FONT_MSDF:
            #endif
            case Feature.FEATURE_FONT_VARIABLE:
            case Feature.FEATURE_CONTEXT_SENSITIVE_CASE_CONVERSION:
            case Feature.FEATURE_USE_SUPPORT_DATA:
            case Feature.FEATURE_UNICODE_IDENTIFIERS:
            case Feature.FEATURE_UNICODE_SECURITY:
                return true;
            default:
                break;
        }

        return false;
    }

    public override bool LoadSupportData(string filename) => throw new NotImplementedException();

    public override HbTag NameToTag(string name)
    {
        if (this.featureSets.TryGetValue(name, out var value))
        {
            return value;
	    }

        // No readable name, use tag string.
        return HbTag.Parse(name.Replace("custom_", ""));
    }

    public override bool ShapedTextAddString(
        Guid                       shaped,
        string                     text,
        IList<Guid>                fonts,
        long                       size,
        Dictionary<uint, Feature>? opentypeFeatures = null,
        string                     language         = "",
        object?                    meta             = null
    )
    {
        var sd = this.shapedOwner.GetOrNull(shaped);

        if (ERR_FAIL_NULL(sd))
        {
            return false;
        }

        if (ERR_FAIL_COND_V(size <= 0))
        {
            return false;
        }

        using var _ = new MutexLock(sd.Mutex);

        for (var i = 0; i < fonts.Count; i++)
        {
            if (ERR_FAIL_COND_V(this.fontOwner.GetOrNull(fonts[i]) == null))
            {
                return false;
            }
        }

        if (string.IsNullOrEmpty(text))
        {
            return true;
        }

        if (sd.Parent != default)
        {
            this.FullCopy(sd);
        }

        var start = sd.Text?.Length ?? 0;

        var span = new ShapedTextDataAdvanced.Span
        {
            Start    = start,
            End      = start + text.Length,
            Fonts    = fonts, // Do not pre-sort, spans will be divided to subruns later.
            FontSize = (int)size,
            Language = language,
            Features = opentypeFeatures ?? new(),
            Meta     = meta
        };

        sd.Spans.Add(span);

        sd.Text += text;
        sd.End  += text.Length;

        Invalidate(sd, true);

        return true;

    }

    public override void ShapedTextClear(Guid shaped)
    {
        var sd = this.shapedOwner.GetOrNull(shaped);

        if (ERR_FAIL_NULL(sd))
        {
            return;
        }

        using var _ = new MutexLock(sd.Mutex);

        sd.Parent = default;
        sd.Start  = default;
        sd.End    = default;
        sd.Text   = "";
        sd.Spans.Clear();
        sd.Objects.Clear();
        sd.BidiOverride.Clear();

        Invalidate(sd, true);
    }

    public override Vector2<float> ShapedTextGetSize(Guid shaped)
    {
        var sd = this.shapedOwner.GetOrNull(shaped);

        if (ERR_FAIL_NULL(sd))
        {
            return default;
        }

        using var _ = new MutexLock(sd.Mutex);

        if (!sd.Valid)
        {
            this.ShapedTextShape(shaped);
        }

        return sd.Orientation == Orientation.ORIENTATION_HORIZONTAL
            ? new Vector2<RealT>(
                (RealT)(sd.TextTrimmed ? sd.WidthTrimmed : sd.Width),
                (RealT)(sd.Ascent + sd.Descent + sd.ExtraSpacing[(int)SpacingType.SPACING_TOP] + sd.ExtraSpacing[(int)SpacingType.SPACING_BOTTOM])
            ).Ceil()
            : new Vector2<RealT>(
                (RealT)(sd.Ascent + sd.Descent + sd.ExtraSpacing[(int)SpacingType.SPACING_TOP] + sd.ExtraSpacing[(int)SpacingType.SPACING_BOTTOM]),
                (RealT)(sd.TextTrimmed ? sd.WidthTrimmed : sd.Width)
            ).Ceil();
    }

    public override bool ShapedTextIsReady(Guid shaped)
    {
        var sd = this.shapedOwner.GetOrNull(shaped);

        return !ERR_FAIL_COND_V(sd == null) && sd!.Valid;
    }

    public override void ShapedTextSetBidiOverride(Guid shaped, IList<object> @override)
    {
        var sd = this.shapedOwner.GetOrNull(shaped);

        if (ERR_FAIL_NULL(sd))
        {
            return;
        }

        using var _ = new MutexLock(sd.Mutex);

        if (sd.Parent != default)
        {
            this.FullCopy(sd);
        }

        sd.BidiOverride.Clear();

        foreach (var value in @override)
        {
            if (value is Vector3<int> vector3)
            {
                sd.BidiOverride.Add(vector3);
            }
            else if (value is Vector2<int> vector2)
            {
                sd.BidiOverride.Add(new(vector2.X, vector2.Y, (int)Direction.DIRECTION_INHERITED));
            }
        }

        Invalidate(sd, false);
    }

    public override void ShapedTextSetDirection(Guid shaped, Direction direction = Direction.DIRECTION_AUTO)
    {
        var sd = this.shapedOwner.GetOrNull(shaped);

        if (ERR_FAIL_COND_MSG(direction == Direction.DIRECTION_INHERITED, "Invalid text direction."))
        {
            return;
        }

        if (ERR_FAIL_NULL(sd))
        {
            return;
        }

        using var _ = new MutexLock(sd.Mutex);

        if (sd.Direction != direction)
        {
            if (sd.Parent != default)
            {
                this.FullCopy(sd);
            }
            sd.Direction = direction;
            Invalidate(sd, false);
        }
    }

    public override void ShapedTextSetPreserveControl(Guid shaped, bool enabled)
    {
        var sd = this.shapedOwner.GetOrNull(shaped);

        if (ERR_FAIL_NULL(sd))
        {
            return;
        }

        using var _ = new MutexLock(sd.Mutex);

        if (sd.PreserveControl != enabled)
        {
            if (sd.Parent != default)
            {
                this.FullCopy(sd);
            }

            sd.PreserveControl = enabled;

            Invalidate(sd, false);
        }
    }

    public override void ShapedTextSetSpacing(Guid shaped, SpacingType spacing, long value)
    {
        if (ERR_FAIL_INDEX((int)spacing, 4))
        {
            return;
        }

        var sd = this.shapedOwner.GetOrNull(shaped);

        if (ERR_FAIL_NULL(sd))
        {
            return;
        }

        using var _ = new MutexLock(sd.Mutex);

        if (sd.ExtraSpacing[(int)spacing] != value)
        {
            if (sd.Parent != default)
            {
                this.FullCopy(sd);
            }

            sd.ExtraSpacing[(int)spacing] = (int)value;

            Invalidate(sd, false);
        }
    }

    public override double ShapedTextGetAscent(Guid shaped)
    {
        var sd = this.shapedOwner.GetOrNull(shaped);

        if (ERR_FAIL_NULL(sd))
        {
            return default;
        }

        using var _ = new MutexLock(sd.Mutex);

        if (!sd.Valid)
        {
            this.ShapedTextShape(shaped);
        }

        return sd.Ascent + sd.ExtraSpacing[(int)SpacingType.SPACING_TOP];
    }

    public override double ShapedTextGetDescent(Guid shaped)
    {
        var sd = this.shapedOwner.GetOrNull(shaped);

        if (ERR_FAIL_NULL(sd))
        {
            return default;
        }

        using var _ = new MutexLock(sd.Mutex);

        if (!sd.Valid)
        {
            this.ShapedTextShape(shaped);
        }

        return sd.Descent + sd.ExtraSpacing[(int)SpacingType.SPACING_BOTTOM];
    }

    public override long ShapedTextGetEllipsisGlyphCount(Guid shaped)
    {
        var sd = this.shapedOwner.GetOrNull(shaped);

        if (ERR_FAIL_NULL(sd))
        {
            return default;
        }

        using var _ = new MutexLock(sd.Mutex);

        return sd.OverrunTrimData.EllipsisGlyphBuf.Count;
    }

    public override IList<Glyph> ShapedTextGetEllipsisGlyphs(Guid shaped)
    {
        var sd = this.shapedOwner.GetOrNull(shaped);

        if (ERR_FAIL_NULL(sd))
        {
            return Array.Empty<Glyph>();
        }

        using var _ = new MutexLock(sd.Mutex);

        return sd.OverrunTrimData.EllipsisGlyphBuf;
    }

    public override long ShapedTextGetEllipsisPos(Guid shaped)
    {
        var sd = this.shapedOwner.GetOrNull(shaped);

        if (ERR_FAIL_NULL(sd))
        {
            return -1;
        }

        using var _ = new MutexLock(sd.Mutex);

        return sd.OverrunTrimData.EllipsisPos;
    }

    public override long ShapedTextGetGlyphCount(Guid shaped)
    {
        var sd = this.shapedOwner.GetOrNull(shaped);

        if (ERR_FAIL_NULL(sd))
        {
            return default;
        }

        using var _ = new MutexLock(sd.Mutex);

        if (!sd.Valid)
        {
            this.ShapedTextShape(shaped);
        }

        var size = sd.Glyphs.Count;

        return size;
    }

    public override IList<Glyph> ShapedTextGetGlyphs(Guid shaped)
    {
        var sd = this.shapedOwner.GetOrNull(shaped);

        if (ERR_FAIL_NULL(sd))
        {
            return Array.Empty<Glyph>();
        }

        using var _ = new MutexLock(sd.Mutex);

        if (!sd.Valid)
        {
            this.ShapedTextShape(shaped);
        }

        return sd.Glyphs;
    }

    public override Direction ShapedTextGetInferredDirection(Guid shaped)
    {
        var sd = this.shapedOwner.GetOrNull(shaped);

        if (ERR_FAIL_NULL(sd))
        {
            return Direction.DIRECTION_LTR;
        }

        using var _ = new MutexLock(sd.Mutex);

        return sd.ParaDirection;
    }

    public override Vector2<int> ShapedTextGetRange(Guid shaped)
    {
        var sd = this.shapedOwner.GetOrNull(shaped);

        if (ERR_FAIL_NULL(sd))
        {
            return default;
        }

        using var _ = new MutexLock(sd.Mutex);

        var range = new Vector2<int>(sd.Start, sd.End);

        return range;
    }

    public override void ShapedTextOverrunTrimToWidth(Guid shaped, double width, TextOverrunFlag trimFlags)
    {
        var sd = this.shapedOwner.GetOrNull(shaped);

        if (ERR_FAIL_NULL(sd))
        {
            return;
        }

        using var _ = new MutexLock(sd.Mutex);

        if (!sd.Valid)
        {
            this.ShapedTextShape(shaped);
        }

        sd.TextTrimmed = false;
        sd.OverrunTrimData.EllipsisGlyphBuf.Clear();

        var addEllipsis        = trimFlags.HasFlag(TextOverrunFlag.OVERRUN_ADD_ELLIPSIS);
        var cutPerWord         = trimFlags.HasFlag(TextOverrunFlag.OVERRUN_TRIM_WORD_ONLY);
        var enforceEllipsis    = trimFlags.HasFlag(TextOverrunFlag.OVERRUN_ENFORCE_ELLIPSIS);
        var justificationAware = trimFlags.HasFlag(TextOverrunFlag.OVERRUN_JUSTIFICATION_AWARE);

        var sdGlyphs = sd.Glyphs;

        if ((trimFlags & TextOverrunFlag.OVERRUN_TRIM) == TextOverrunFlag.OVERRUN_NO_TRIM || sdGlyphs == null || width <= 0 || !(sd.Width > width || enforceEllipsis))
        {
            sd.OverrunTrimData.TrimPos     = -1;
            sd.OverrunTrimData.EllipsisPos = -1;

            return;
        }

        if (justificationAware && !sd.FitWidthMinimumReached)
        {
            return;
        }

        var spans = sd.Spans;

        if (sd.Parent != default)
        {
            var parentSd = this.shapedOwner.GetOrNull(sd.Parent);
            if (ERR_FAIL_NULL(parentSd) || ERR_FAIL_COND(!parentSd.Valid))
            {
                return;
            }

            spans = parentSd.Spans;
        }

        if (spans.Count == 0)
        {
            return;
        }

        var sdSize = sd.Glyphs.Count;
        var lastGlFontSize = sdGlyphs[sdSize - 1].FontSize;

        // Find usable fonts, if fonts from the last glyph do not have required chars.
        var dotGlFontRid = sdGlyphs[sdSize - 1].FontId;
        if (!this.FontHasChar(dotGlFontRid, '.'))
        {
            var fonts = spans[^1].Fonts;
            for (var i = 0; i < fonts.Count; i++)
            {
                if (this.FontHasChar(fonts[i], '.'))
                {
                    dotGlFontRid = fonts[i];
                    break;
                }
            }
        }

        var whitespaceGlFontRid = sdGlyphs[sdSize - 1].FontId;
        if (!this.FontHasChar(whitespaceGlFontRid, '.'))
        {
            var fonts = spans[^1].Fonts;
            for (var i = 0; i < fonts.Count; i++)
            {
                if (this.FontHasChar(fonts[i], ' '))
                {
                    whitespaceGlFontRid = fonts[i];
                    break;
                }
            }
        }

        var dotGlIdx = dotGlFontRid != default ? this.FontGetGlyphIndex(dotGlFontRid, lastGlFontSize, '.', 0) : -10;
        var dotAdv = dotGlFontRid != default ? this.FontGetGlyphAdvance(dotGlFontRid, lastGlFontSize, (int)dotGlIdx) : new Vector2<RealT>();
        var whitespaceGlIdx = whitespaceGlFontRid != default ? this.FontGetGlyphIndex(whitespaceGlFontRid, lastGlFontSize, ' ', 0) : -10;
        var whitespaceAdv = whitespaceGlFontRid != default ? this.FontGetGlyphAdvance(whitespaceGlFontRid, lastGlFontSize, (int)whitespaceGlIdx) : new Vector2<RealT>();

        var ellipsisWidth = 0;
        if (addEllipsis && whitespaceGlFontRid != default)
        {
            ellipsisWidth = (int)(3 * dotAdv.X + sd.ExtraSpacing[(int)SpacingType.SPACING_GLYPH] + (cutPerWord ? whitespaceAdv.X : 0));
        }

        var ellMinCharacters = 6;
        var sdWidth          = sd.Width;
        var isRtl            = sd.ParaDirection == Direction.DIRECTION_RTL;
        var trimPos          = isRtl ? sdSize : 0;
        var ellipsisPos      = enforceEllipsis ? 0 : -1;
        var lastValidCut     = 0;
        var found            = false;
        var glyphsFrom       = isRtl ? 0 : sdSize - 1;
        var glyphsTo         = isRtl ? sdSize - 1 : -1;
        var glyphsDelta      = isRtl ? +1 : -1;

        if (enforceEllipsis && sdWidth + ellipsisWidth <= width)
        {
            trimPos     = -1;
            ellipsisPos = isRtl ? 0 : sdSize;
        }
        else
        {
            for (var i = glyphsFrom; i != glyphsTo; i += glyphsDelta)
            {
                if (!isRtl)
                {
                    sdWidth -= sdGlyphs[i].Advance * sdGlyphs[i].Repeat;
                }
                if (sdGlyphs[i].Count > 0)
                {
                    var aboveMinCharThreshold = (isRtl ? sdSize - 1 - i : i) >= ellMinCharacters;

                    if (sdWidth + ((aboveMinCharThreshold && addEllipsis || enforceEllipsis) ? ellipsisWidth : 0) <= width)
                    {
                        if (cutPerWord && aboveMinCharThreshold)
                        {
                            if (sdGlyphs[i].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_BREAK_SOFT))
                            {
                                lastValidCut = i;
                                found        = true;
                            }
                        }
                        else
                        {
                            lastValidCut = i;
                            found        = true;
                        }
                        if (found)
                        {
                            trimPos = lastValidCut;

                            if (addEllipsis && (aboveMinCharThreshold || enforceEllipsis) && sdWidth - ellipsisWidth <= width)
                            {
                                ellipsisPos = trimPos;
                            }
                            break;
                        }
                    }
                }
                if (isRtl)
                {
                    sdWidth -= sdGlyphs[i].Advance * sdGlyphs[i].Repeat;
                }
            }
        }

        sd.OverrunTrimData.TrimPos     = trimPos;
        sd.OverrunTrimData.EllipsisPos = ellipsisPos;
        if (trimPos == 0 && enforceEllipsis && addEllipsis)
        {
            sd.OverrunTrimData.EllipsisPos = 0;
        }

        if (trimPos >= 0 && sd.Width > width || enforceEllipsis)
        {
            if (addEllipsis && (ellipsisPos > 0 || enforceEllipsis))
            {
                // Insert an additional space when cutting word bound for aesthetics.
                if (cutPerWord && ellipsisPos > 0)
                {
                    var gl = new Glyph
                    {
                        Count    = 1,
                        Advance  = whitespaceAdv.X,
                        Index    = (int)whitespaceGlIdx,
                        FontId   = whitespaceGlFontRid,
                        FontSize = lastGlFontSize,
                        Flags    = GraphemeFlag.GRAPHEME_IS_SPACE | GraphemeFlag.GRAPHEME_IS_BREAK_SOFT | GraphemeFlag.GRAPHEME_IS_VIRTUAL | (isRtl ? GraphemeFlag.GRAPHEME_IS_RTL : 0)
                    };

                    sd.OverrunTrimData.EllipsisGlyphBuf.Add(gl);
                }
                // Add ellipsis dots.
                if (dotGlIdx != 0)
                {
                    var gl = new Glyph
                    {
                        Count    = 1,
                        Repeat   = 3,
                        Advance  = dotAdv.X,
                        Index    = (int)dotGlIdx,
                        FontId   = dotGlFontRid,
                        FontSize = lastGlFontSize,
                        Flags    = GraphemeFlag.GRAPHEME_IS_PUNCTUATION | GraphemeFlag.GRAPHEME_IS_VIRTUAL | (isRtl ? GraphemeFlag.GRAPHEME_IS_RTL : 0)
                    };

                    sd.OverrunTrimData.EllipsisGlyphBuf.Add(gl);
                }
            }

            sd.TextTrimmed = true;
            sd.WidthTrimmed = sdWidth + ((ellipsisPos != -1) ? ellipsisWidth : 0);
        }
    }

    public override IList<Glyph> ShapedTextSortLogical(Guid shaped)
    {
        var sd = this.shapedOwner.GetOrNull(shaped);

        if (ERR_FAIL_NULL(sd))
        {
            return Array.Empty<Glyph>();
        }

        using var _ = new MutexLock(sd.Mutex);

        if (!sd.Valid)
        {
            this.ShapedTextShape(shaped);
        }

        if (!sd.SortValid)
        {
            sd.GlyphsLogical = sd.Glyphs;
            sd.GlyphsLogical.Sort();
            sd.SortValid = true;
        }

        var result = sd.GlyphsLogical;

        return result;
    }

    public override Guid ShapedTextSubstr(Guid shaped, int start, int length)
    {
        lock (padlock)
        {
            var sd = this.shapedOwner.GetOrNull(shaped);

            if (ERR_FAIL_NULL(sd))
            {
                return default;
            }

            using var _ = new MutexLock(sd.Mutex);

            if (sd.Parent != default)
            {
                return this.ShapedTextSubstr(sd.Parent, start, length);
            }
            if (!sd.Valid)
            {
                this.ShapedTextShape(shaped);
            }

            if (ERR_FAIL_COND_V(start < 0 || length < 0))
            {
                return default;
            }

            if (ERR_FAIL_COND_V(sd.Start > start || sd.End < start))
            {
                return default;
            }

            if (ERR_FAIL_COND_V(sd.End < start + length))
            {
                return default;
            }

            var newSd = new ShapedTextDataAdvanced
            {
                Parent        = shaped,
                Start         = start,
                End           = start + length,
                Orientation   = sd.Orientation,
                Direction     = sd.Direction,
                CustomPunct   = sd.CustomPunct,
                ParaDirection = sd.ParaDirection
            };
            for (var i = 0; i < (int)SpacingType.SPACING_MAX; i++)
            {
                newSd.ExtraSpacing[i] = sd.ExtraSpacing[i];
            }

            return !this.ShapeSubstr(newSd, sd, start, length) ? default : this.shapedOwner.Initialize(newSd);
        }
    }

    public override bool ShapedTextUpdateBreaks(Guid shaped)
    {
        var sd = this.shapedOwner.GetOrNull(shaped);

        if (ERR_FAIL_NULL(sd))
        {
            return false;
        }

        using var _ = new MutexLock(sd.Mutex);

        if (!sd.Valid)
        {
            this.ShapedTextShape(shaped);
        }

        if (sd.LineBreaksValid)
        {
            return true; // Nothing to do.
        }

        var data = sd.Utf16;

        if (!sd.BreakOpsValid)
        {
            sd.Breaks.Clear();
            sd.BreakInserts = 0;

            var i = 0;

            while (i < sd.Spans.Count)
            {
                var language = sd.Spans[i].Language;
                var rStart   = sd.Spans[i].Start;

                while (i + 1 < sd.Spans.Count && language == sd.Spans[i + 1].Language)
                {
                    i++;
                }

                var rEnd = sd.Spans[i].End;

                try
                {
                    using var bi = new BreakIterator(UBreakIteratorType.UBRK_LINE, language ?? TranslationServer.Singleton.GetToolLocale(), data[ConvertPosInv(sd, rStart)..]);

                    while (bi.Next() != (int)UBreakIteratorType.UBRK_DONE)
                    {
                        var pos        = ConvertPos(sd, bi.Current) + rStart;
                        var ruleStatus = bi.GetRuleStatus();

                        if (ruleStatus >= (int)ULineBreakTag.UBRK_LINE_HARD && ruleStatus < (int)ULineBreakTag.UBRK_LINE_HARD_LIMIT)
                        {
                            sd.Breaks[pos] = true;
                        }
                        else if (ruleStatus >= (int)ULineBreakTag.UBRK_LINE_SOFT && ruleStatus < (int)ULineBreakTag.UBRK_LINE_SOFT_LIMIT)
                        {
                            sd.Breaks[pos] = false;
                        }

                        var posP = pos - 1 - sd.Start;
                        var c    = sd.Text[posP];

                        if (pos - sd.Start != sd.End && !c.IsWhiteSpace() && c != 0xfffc)
                        {
                            sd.BreakInserts++;
                        }
                    }
                }
                catch (Exception)
                {
                    // No data loaded - use fallback.
                    for (var j = rStart; j < rEnd; j++)
                    {
                        var c = sd.Text[j - sd.Start];
                        if (c.IsWhiteSpace())
                        {
                            sd.Breaks[j + 1] = false;
                        }
                        if (c.IsLinebreak())
                        {
                            sd.Breaks[j + 1] = true;
                        }
                    }
                }

                i++;
            }

            sd.BreakOpsValid = true;
        }

        var glyphsNew = new List<Glyph>();
        var rewrite   = false;
        var sdShift   = 0;
        var sdSize    = sd.Glyphs.Count;

        List<Glyph> sdGlyphsNew;
        if (sd.BreakInserts > 0)
        {
            glyphsNew.Capacity = sd.Glyphs.Count + sd.BreakInserts;

            sdGlyphsNew = glyphsNew;
            rewrite = true;
        }
        else
        {
            sdGlyphsNew = sd.Glyphs;
        }

        sd.SortValid = false;
        sd.GlyphsLogical.Clear();
        var ch = sd.Text.ToCharArray();

        var cPunctSize = sd.CustomPunct.Length;
        var cPunct     = sd.CustomPunct.ToCharArray();

        for (var i = 0; i < sdSize; i++)
        {
            if (rewrite)
            {
                for (var j = 0; j < sd.Glyphs[i].Count; j++)
                {
                    sdGlyphsNew[sdShift + i + j] = sd.Glyphs[i + j];
                }
            }

            if (sd.Glyphs[i].Count > 0)
            {
                var c = ch[sd.Glyphs[i].Start - sd.Start];

                if (c == 0xfffc)
                {
                    i += sd.Glyphs[i].Count - 1;
                    continue;
                }

                if (c == 0x0009 || c == 0x000b)
                {
                    sdGlyphsNew[sdShift + i].Flags |= GraphemeFlag.GRAPHEME_IS_TAB;
                }

                if (c.IsWhiteSpace())
                {
                    sdGlyphsNew[sdShift + i].Flags |= GraphemeFlag.GRAPHEME_IS_SPACE;
                }

                if (cPunctSize == 0)
                {
                    if (Character.IsPunct(c) && c != 0x005f)
                    {
                        sdGlyphsNew[sdShift + i].Flags |= GraphemeFlag.GRAPHEME_IS_PUNCTUATION;
                    }
                }
                else
                {
                    for (var j = 0; j < cPunctSize; j++)
                    {
                        if (cPunct[j] == c)
                        {
                            sdGlyphsNew[sdShift + i].Flags |= GraphemeFlag.GRAPHEME_IS_PUNCTUATION;
                            break;
                        }
                    }
                }
                if (c.IsUnderscore())
                {
                    sdGlyphsNew[sdShift + i].Flags |= GraphemeFlag.GRAPHEME_IS_UNDERSCORE;
                }
                if (sd.Breaks.TryGetValue(sd.Glyphs[i].End, out var value))
                {
                    if (value && c.IsLinebreak())
                    {
                        sdGlyphsNew[sdShift + i].Flags |= GraphemeFlag.GRAPHEME_IS_BREAK_HARD;
                    }
                    else if (c.IsWhiteSpace())
                    {
                        sdGlyphsNew[sdShift + i].Flags |= GraphemeFlag.GRAPHEME_IS_BREAK_SOFT;
                    }
                    else
                    {
                        int count = sd.Glyphs[i].Count;

                        // Do not add extra space at the end of the line.
                        if (sd.Glyphs[i].End == sd.End)
                        {
                            i += sd.Glyphs[i].Count - 1;
                            continue;
                        }

                        // Do not add extra space after existing space.
                        if (sd.Glyphs[i].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_RTL))
                        {
                            if (i + count < sdSize - 1 && sd.Glyphs[i + count].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_SPACE | GraphemeFlag.GRAPHEME_IS_BREAK_SOFT))
                            {
                                i += sd.Glyphs[i].Count - 1;
                                continue;
                            }
                        }
                        else
                        {
                            if (sd.Glyphs[i].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_SPACE | GraphemeFlag.GRAPHEME_IS_BREAK_SOFT))
                            {
                                i += sd.Glyphs[i].Count - 1;

                                continue;
                            }
                        }

                        var gl = new Glyph
                        {
                            Start    = sd.Glyphs[i].Start,
                            End      = sd.Glyphs[i].End,
                            Count    = 1,
                            FontId   = sd.Glyphs[i].FontId,
                            FontSize = sd.Glyphs[i].FontSize,
                            Flags    = GraphemeFlag.GRAPHEME_IS_BREAK_SOFT | GraphemeFlag.GRAPHEME_IS_VIRTUAL | GraphemeFlag.GRAPHEME_IS_SPACE
                        };

                        if (sd.Glyphs[i].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_RTL))
                        {
                            gl.Flags |= GraphemeFlag.GRAPHEME_IS_RTL;

                            for (var j = sd.Glyphs[i].Count - 1; j >= 0; j--)
                            {
                                sdGlyphsNew[sdShift + i + j + 1] = sdGlyphsNew[sdShift + i + j];
                            }

                            sdGlyphsNew[sdShift + i] = gl;
                        }
                        else
                        {
                            sdGlyphsNew[sdShift + i + count] = gl;
                        }

                        sdShift++;

                        if (ERR_FAIL_COND_V_MSG(sdShift > sd.BreakInserts, "Invalid break insert count!"))
                        {
                            return false;
                        }
                    }
                }
                i += sd.Glyphs[i].Count - 1;
            }
        }

        if (sdShift < sd.BreakInserts)
        {
            // Note: should not happen with a normal text, but might be a case with special fonts that substitute a long string (with breaks opportunities in it) with a single glyph (like Font Awesome).
            glyphsNew.Capacity = sd.Glyphs.Count + sdShift;
        }

        if (sd.BreakInserts > 0)
        {
            sd.Glyphs = glyphsNew;
        }

        sd.LineBreaksValid = true;

        return sd.LineBreaksValid;
    }
    #endregion public overrided methods
}
