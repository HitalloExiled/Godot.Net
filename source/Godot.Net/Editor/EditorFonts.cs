namespace Godot.Net.Editor;

using Godot.Net.Core.OS;
using Godot.Net.Scene.Resources;
using Godot.Net.Servers;

public class EditorFonts
{
    private static FontFile LoadExternalFont(
        string                         p_path,
        TextServer.Hinting             p_hinting,
        TextServer.FontAntialiasing    p_aa,
        bool                           p_autohint,
        TextServer.SubpixelPositioning p_font_subpixel_positioning,
        bool                           p_msdf = false,
        List<Font>?                    r_fallbacks = null
    ) => throw new NotImplementedException();

    private static FontFile LoadInternalFont(
        byte[]                         data,
        TextServer.Hinting             hinting,
        TextServer.FontAntialiasing    aa,
        bool                           autohint,
        TextServer.SubpixelPositioning fontSubpixelPositioning,
        bool                           msdf      = false,
        List<Font>?                    fallbacks = null
    )
    {
        var font = new FontFile
        {
            Data                            = data,
            MultichannelSignedDistanceField = msdf,
            Antialiasing                    = aa,
            Hinting                         = hinting,
            ForceAutohinter                 = autohint,
            SubpixelPositioning             = fontSubpixelPositioning
        };

        fallbacks?.Add(font);

        return font;
    }

    private static SystemFont LoadSystemFont(
        IList<string>                  p_names,
        TextServer.Hinting             p_hinting,
        TextServer.FontAntialiasing    p_aa,
        bool                           p_autohint,
        TextServer.SubpixelPositioning p_font_subpixel_positioning,
        bool                           p_msdf      = false,
        List<Font>?                    r_fallbacks = null
    ) => throw new NotImplementedException();

    private static FontVariation MakeBoldFont(Font font, float embolden, List<Font>? fallbacks = null)
    {
        var fontVar = new FontVariation
        {
            BaseFont          = font,
            VariationEmbolden = embolden
        };

        fallbacks?.Add(fontVar);

        return fontVar;
    }

    public static void EditorRegisterFonts(Theme p_theme)
    {
        var fontAntialiasing        = EDITOR_GET<TextServer.FontAntialiasing>("interface/editor/font_antialiasing");
        var fontHintingSetting      = EDITOR_GET<int>("interface/editor/font_hinting");
        var fontSubpixelPositioning = EDITOR_GET<TextServer.SubpixelPositioning>("interface/editor/font_subpixel_positioning");

        TextServer.Hinting fontHinting;
        TextServer.Hinting fontMonoHinting;
        switch (fontHintingSetting)
        {
            case 0:
                // The "Auto" setting uses the setting that best matches the OS' font rendering:
                // - macOS doesn't use font hinting.
                // - Windows uses ClearType, which is in between "Light" and "Normal" hinting.
                // - Linux has configurable font hinting, but most distributions including Ubuntu default to "Light".
                #if MACOS
                fontHinting     = TextServer.Hinting.HINTING_NONE;
                fontMonoHinting = TextServer.Hinting.HINTING_NONE;
                #else
                fontHinting     = TextServer.Hinting.HINTING_LIGHT;
                fontMonoHinting = TextServer.Hinting.HINTING_LIGHT;
                #endif
                break;
            case 1:
                fontHinting     = TextServer.Hinting.HINTING_NONE;
                fontMonoHinting = TextServer.Hinting.HINTING_NONE;
                break;
            case 2:
                fontHinting     = TextServer.Hinting.HINTING_LIGHT;
                fontMonoHinting = TextServer.Hinting.HINTING_LIGHT;
                break;
            default:
                fontHinting     = TextServer.Hinting.HINTING_NORMAL;
                fontMonoHinting = TextServer.Hinting.HINTING_LIGHT;
                break;
        }

        // Load built-in fonts.
        var defaultFontSize = EDITOR_GET<int>("interface/editor/main_font_size") * (int)EditorScale.Value;
        var defaultFont     = LoadInternalFont(BuiltinFonts.NotoSansRegular, fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false);
        var defaultFontMsdf = LoadInternalFont(BuiltinFonts.NotoSansRegular, fontHinting, fontAntialiasing, true, fontSubpixelPositioning, true);

        var fallbacks      = new List<Font>();
        var arabicFont     = LoadInternalFont(BuiltinFonts.NotoNaskhArabicUiRegular,    fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacks);
        var bengaliFont    = LoadInternalFont(BuiltinFonts.NotoSansBengaliUiRegular,    fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacks);
        var devanagariFont = LoadInternalFont(BuiltinFonts.NotoSansDevanagariUiRegular, fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacks);
        var georgianFont   = LoadInternalFont(BuiltinFonts.NotoSansGeorgianRegular,     fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacks);
        var hebrewFont     = LoadInternalFont(BuiltinFonts.NotoSansHebrewRegular,       fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacks);
        var malayalamFont  = LoadInternalFont(BuiltinFonts.NotoSansMalayalamUiRegular,  fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacks);
        var oriyaFont      = LoadInternalFont(BuiltinFonts.NotoSansOriyaUiRegular,      fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacks);
        var sinhalaFont    = LoadInternalFont(BuiltinFonts.NotoSansSinhalaUiRegular,    fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacks);
        var tamilFont      = LoadInternalFont(BuiltinFonts.NotoSansTamilUiRegular,      fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacks);
        var teluguFont     = LoadInternalFont(BuiltinFonts.NotoSansTeluguUiRegular,     fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacks);
        var thaiFont       = LoadInternalFont(BuiltinFonts.NotoSansThaiUiRegular,       fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacks);
        var fallbackFont   = LoadInternalFont(BuiltinFonts.DroidSansFallback,           fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacks);
        var japaneseFont   = LoadInternalFont(BuiltinFonts.DroidSansJapanese,           fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacks);

        defaultFont.Fallbacks = fallbacks;
        defaultFontMsdf.Fallbacks = fallbacks;

        var defaultFontBold     = LoadInternalFont(BuiltinFonts.NotoSansBold, fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false);
        var defaultFontBoldMsdf = LoadInternalFont(BuiltinFonts.NotoSansBold, fontHinting, fontAntialiasing, true, fontSubpixelPositioning, true);

        var fallbacksBold = new List<Font>();

        var arabicFontBold     = LoadInternalFont(BuiltinFonts.NotoNaskhArabicUiBold,    fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacksBold);
        var bengaliFontBold    = LoadInternalFont(BuiltinFonts.NotoSansBengaliUiBold,    fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacksBold);
        var devanagariFontBold = LoadInternalFont(BuiltinFonts.NotoSansDevanagariUiBold, fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacksBold);
        var georgianFontBold   = LoadInternalFont(BuiltinFonts.NotoSansGeorgianBold,     fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacksBold);
        var hebrewFontBold     = LoadInternalFont(BuiltinFonts.NotoSansHebrewBold,       fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacksBold);
        var malayalamFontBold  = LoadInternalFont(BuiltinFonts.NotoSansMalayalamUiBold,  fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacksBold);
        var oriyaFontBold      = LoadInternalFont(BuiltinFonts.NotoSansOriyaUiBold,      fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacksBold);
        var sinhalaFontBold    = LoadInternalFont(BuiltinFonts.NotoSansSinhalaUiBold,    fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacksBold);
        var tamilFontBold      = LoadInternalFont(BuiltinFonts.NotoSansTamilUiBold,      fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacksBold);
        var teluguFontBold     = LoadInternalFont(BuiltinFonts.NotoSansTeluguUiBold,     fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacksBold);
        var thaiFontBold       = LoadInternalFont(BuiltinFonts.NotoSansThaiUiBold,       fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false, fallbacksBold);

        const float EMBOLDEN_STRENGTH = 0.6f;

        var fallbackFontBold = MakeBoldFont(fallbackFont, EMBOLDEN_STRENGTH, fallbacksBold);
        var japaneseFontBold = MakeBoldFont(japaneseFont, EMBOLDEN_STRENGTH, fallbacksBold);

        if (OS.Singleton.HasFeature("system_fonts"))
        {
            var emojiFontNames = new List<string>
            {
                "Apple Color Emoji",
                "Segoe UI Emoji",
                "Noto Color Emoji",
                "Twitter Color Emoji",
                "OpenMoji",
                "EmojiOne Color"
            };

            var emojiFont = LoadSystemFont(emojiFontNames, fontHinting, fontAntialiasing, true, fontSubpixelPositioning, false);
            fallbacks.Add(emojiFont);
            fallbacksBold.Add(emojiFont);
        }

        defaultFontBold.Fallbacks = fallbacksBold;
        defaultFontBoldMsdf.Fallbacks = fallbacksBold;

        var defaultFontMono = LoadInternalFont(BuiltinFonts.JetBrainsMonoRegular, fontMonoHinting, fontAntialiasing, true, fontSubpixelPositioning);
        defaultFontMono.Fallbacks = fallbacks;

        // Init base font configs and load custom fonts.
        var customFontPath       = EDITOR_GET<string>("interface/editor/main_font");
        var customFontPathBold   = EDITOR_GET<string>("interface/editor/main_font_bold");
        var customFontPathSource = EDITOR_GET<string>("interface/editor/code_font");

        var defaultFc = new FontVariation();
        if (customFontPath?.Length > 0 && Directory.Exists(customFontPath))
        {
            var customFont = LoadExternalFont(customFontPath, fontHinting, fontAntialiasing, true, fontSubpixelPositioning);
            {
                var fallbackCustom = new List<Font>
                {
                    defaultFont
                };
                customFont.Fallbacks = fallbackCustom;
            }
            defaultFc.BaseFont = customFont;
        }
        else
        {
            EditorSettings.Singleton.SetManually("interface/editor/main_font", "");
            defaultFc.BaseFont = defaultFont;
        }
        defaultFc.SetSpacing(TextServer.SpacingType.SPACING_TOP, -(int)EditorScale.Value);
        defaultFc.SetSpacing(TextServer.SpacingType.SPACING_BOTTOM, -(int)EditorScale.Value);

        var defaultFcMsdf = new FontVariation();
        if (customFontPath?.Length > 0 && Directory.Exists(customFontPath))
        {
            var customFont = LoadExternalFont(customFontPath, fontHinting, fontAntialiasing, true, fontSubpixelPositioning);
            {
                var fallbackCustom = new List<Font>
                {
                    defaultFontMsdf
                };
                customFont.Fallbacks = fallbackCustom;
            }

            defaultFcMsdf.BaseFont = customFont;
        }
        else
        {
            EditorSettings.Singleton.SetManually("interface/editor/main_font", "");
            defaultFcMsdf.BaseFont = defaultFontMsdf;
        }
        defaultFcMsdf.SetSpacing(TextServer.SpacingType.SPACING_TOP,    -(int)EditorScale.Value);
        defaultFcMsdf.SetSpacing(TextServer.SpacingType.SPACING_BOTTOM, -(int)EditorScale.Value);

        var boldFc = new FontVariation();
        if (customFontPathBold?.Length > 0 && Directory.Exists(customFontPathBold))
        {
            var customFont = LoadExternalFont(customFontPathBold, fontHinting, fontAntialiasing, true, fontSubpixelPositioning);
            {
                var fallbackCustom = new List<Font>
                {
                    defaultFontBold
                };
                customFont.Fallbacks = fallbackCustom;
            }

            boldFc.BaseFont = customFont;
        }
        else if (customFontPath?.Length > 0 && Directory.Exists(customFontPath))
        {
            var customFont = LoadExternalFont(customFontPath, fontHinting, fontAntialiasing, true, fontSubpixelPositioning);
            {
                var fallbackCustom = new List<Font>
                {
                    defaultFontBold
                };
                customFont.Fallbacks = fallbackCustom;
            }

            boldFc.BaseFont = customFont;
            boldFc.VariationEmbolden = EMBOLDEN_STRENGTH;
        }
        else
        {
            EditorSettings.Singleton.SetManually("interface/editor/main_font_bold", "");
            boldFc.BaseFont = defaultFontBold;
        }
        boldFc.SetSpacing(TextServer.SpacingType.SPACING_TOP, -(int)EditorScale.Value);
        boldFc.SetSpacing(TextServer.SpacingType.SPACING_BOTTOM, -(int)EditorScale.Value);

        var boldFcMsdf = new FontVariation();
        if (customFontPathBold?.Length > 0 && Directory.Exists(customFontPathBold))
        {
            var customFont = LoadExternalFont(customFontPathBold, fontHinting, fontAntialiasing, true, fontSubpixelPositioning);
            {
                var fallbackCustom = new List<Font>
                {
                    defaultFontBoldMsdf
                };
                customFont.Fallbacks = fallbackCustom;
            }
            boldFcMsdf.BaseFont = customFont;
        }
        else if (customFontPath?.Length > 0 && Directory.Exists(customFontPath))
        {
            var customFont = LoadExternalFont(customFontPath, fontHinting, fontAntialiasing, true, fontSubpixelPositioning);
            {
                var fallbackCustom = new List<Font>
                {
                    defaultFontBoldMsdf
                };
                customFont.Fallbacks = fallbackCustom;
            }
            boldFcMsdf.BaseFont = customFont;
            boldFcMsdf.VariationEmbolden = EMBOLDEN_STRENGTH;
        }
        else
        {
            EditorSettings.Singleton.SetManually("interface/editor/main_font_bold", "");
            boldFcMsdf.BaseFont = defaultFontBoldMsdf;
        }
        boldFcMsdf.SetSpacing(TextServer.SpacingType.SPACING_TOP, -(int)EditorScale.Value);
        boldFcMsdf.SetSpacing(TextServer.SpacingType.SPACING_BOTTOM, -(int)EditorScale.Value);

        var monoFc = new FontVariation();
        if (customFontPathSource?.Length > 0 && Directory.Exists(customFontPathSource))
        {
            var customFont = LoadExternalFont(customFontPathSource, fontMonoHinting, fontAntialiasing, true, fontSubpixelPositioning);
            {
                var fallbackCustom = new List<Font>
                {
                    defaultFontMono
                };
                customFont.Fallbacks = fallbackCustom;
            }
            monoFc.BaseFont = customFont;
        }
        else
        {
            EditorSettings.Singleton.SetManually("interface/editor/code_font", "");
            monoFc.BaseFont = defaultFontMono;
        }
        monoFc.SetSpacing(TextServer.SpacingType.SPACING_TOP, -(int)EditorScale.Value);
        monoFc.SetSpacing(TextServer.SpacingType.SPACING_BOTTOM, -(int)EditorScale.Value);

        var monoOtherFc = (FontVariation)monoFc.Duplicate();

        // Enable contextual alternates (coding ligatures) and custom features for the source editor font.
        var otMode = EDITOR_GET<int>("interface/editor/code_font_contextual_ligatures");
        switch (otMode)
        {
            case 1:
                {
                    // Disable ligatures.
                    var ftrs = new Dictionary<uint, TextServer.Feature>
                    {
                        [(uint)TextServerManager.Singleton.PrimaryInterface.NameToTag("calt")] = 0
                    };

                    monoFc.OpentypeFeatures = ftrs;
                }
                break;
            case 2:
                {
                    // Custom.
                    var subtag = (EDITOR_GET<string>("interface/editor/code_font_custom_opentype_features") ?? "").Split(",");
                    var ftrs   = new Dictionary<uint, TextServer.Feature>();

                    for (var i = 0; i < subtag.Length; i++)
                    {
                        var subtagA = subtag[i].Split("=");
                        if (subtagA.Length == 2)
                        {
                            ftrs[(uint)TextServerManager.Singleton.PrimaryInterface.NameToTag(subtagA[0])] = (TextServer.Feature)int.Parse(subtagA[1]);
                        }
                        else if (subtagA.Length == 1)
                        {
                            ftrs[(uint)TextServerManager.Singleton.PrimaryInterface.NameToTag(subtagA[0])] = (TextServer.Feature)1;
                        }
                    }

                    monoFc.OpentypeFeatures = ftrs;
                }
                break;
            default:
                {
                    // Enabled.
                    var ftrs = new Dictionary<uint, TextServer.Feature>
                    {
                        [(uint)TextServerManager.Singleton.PrimaryInterface.NameToTag("calt")] = (TextServer.Feature)1
                    };

                    monoFc.OpentypeFeatures = ftrs;
                }
                break;
        }

        {
            // Disable contextual alternates (coding ligatures).
            var ftrs = new Dictionary<uint, TextServer.Feature>
            {
                [(uint)TextServerManager.Singleton.PrimaryInterface.NameToTag("calt")] = 0
            };

            monoOtherFc.OpentypeFeatures = ftrs;
        }

        // Use fake bold/italics to style the editor log's `print_rich()` output.
        // Use stronger embolden strength to make bold easier to distinguish from regular text.
        var monoOtherFcBold = (FontVariation)monoOtherFc.Duplicate();
        monoOtherFcBold.VariationEmbolden = 0.8f;

        var monoOtherFcItalic = (FontVariation)monoOtherFc.Duplicate();
        monoOtherFcItalic.VariationTransform = new (1, (RealT)0.2, 0, 1, 0, 0);

        var monoOtherFcBoldItalic = (FontVariation)monoOtherFc.Duplicate();
        monoOtherFcBoldItalic.VariationEmbolden  = 0.8f;
        monoOtherFcBoldItalic.VariationTransform = new(1, (RealT)0.2, 0, 1, 0, 0);

        var monoOtherFcMono = (FontVariation)monoOtherFc.Duplicate();
        // Use a different font style to distinguish `[code]` in rich prints.
        // This emulates the "faint" styling used in ANSI escape codes by using a slightly thinner font.
        monoOtherFcMono.VariationEmbolden  = -0.25f;
        monoOtherFcMono.VariationTransform = new(1, (RealT)0.1, 0, 1, 0, 0);

        var italicFc = (FontVariation)defaultFc.Duplicate();
        italicFc.VariationTransform = new(1, (RealT)0.2, 0, 1, 0, 0);

        // Setup theme.

        p_theme.DefaultFont     = defaultFc; // Default theme font config.
        p_theme.DefaultFontSize = defaultFontSize;

        // Main font.

        p_theme.SetFont("main",      "EditorFonts", defaultFc);
        p_theme.SetFont("main_msdf", "EditorFonts", defaultFcMsdf);
        p_theme.SetFontSize("main_size", "EditorFonts", defaultFontSize);

        p_theme.SetFont("bold",           "EditorFonts", boldFc);
        p_theme.SetFont("main_bold_msdf", "EditorFonts", boldFcMsdf);
        p_theme.SetFontSize("bold_size", "EditorFonts", defaultFontSize);

        // Title font.

        p_theme.SetFont("title", "EditorFonts", boldFc);
        p_theme.SetFontSize("title_size", "EditorFonts", (int)(defaultFontSize + 1 * EditorScale.Value));

        p_theme.SetFont("main_button_font", "EditorFonts", boldFc);
        p_theme.SetFontSize("main_button_font_size", "EditorFonts", (int)(defaultFontSize + 1 * EditorScale.Value));

        p_theme.SetFont("font", "Label", defaultFc);

        p_theme.SetTypeVariation("HeaderSmall", "Label");
        p_theme.SetFont("font", "HeaderSmall", boldFc);
        p_theme.SetFontSize("font_size", "HeaderSmall", defaultFontSize);

        p_theme.SetTypeVariation("HeaderMedium", "Label");
        p_theme.SetFont("font", "HeaderMedium", boldFc);
        p_theme.SetFontSize("font_size", "HeaderMedium", (int)(defaultFontSize + 1 * EditorScale.Value));

        p_theme.SetTypeVariation("HeaderLarge", "Label");
        p_theme.SetFont("font", "HeaderLarge", boldFc);
        p_theme.SetFontSize("font_size", "HeaderLarge", (int)(defaultFontSize + 3 * EditorScale.Value));

        // Documentation fonts
        p_theme.SetFontSize("doc_size", "EditorFonts", EDITOR_GET<int>("text_editor/help/help_font_size") * (int)EditorScale.Value);

        p_theme.SetFont("doc",        "EditorFonts", defaultFc);
        p_theme.SetFont("doc_bold",   "EditorFonts", boldFc);
        p_theme.SetFont("doc_italic", "EditorFonts", italicFc);

        p_theme.SetFontSize("doc_title_size", "EditorFonts", EDITOR_GET<int>("text_editor/help/help_title_font_size") * (int)EditorScale.Value);

        p_theme.SetFont("doc_title", "EditorFonts", boldFc);

        p_theme.SetFontSize("doc_source_size", "EditorFonts", EDITOR_GET<int>("text_editor/help/help_source_font_size") * (int)EditorScale.Value);

        p_theme.SetFont("doc_source", "EditorFonts", monoFc);

        p_theme.SetFontSize("doc_keyboard_size", "EditorFonts", (EDITOR_GET<int>("text_editor/help/help_source_font_size") - 1) * (int)EditorScale.Value);

        p_theme.SetFont("doc_keyboard", "EditorFonts", monoFc);

        // Ruler font
        p_theme.SetFontSize("rulers_size", "EditorFonts", 8 * (int)EditorScale.Value);
        p_theme.SetFont("rulers", "EditorFonts", defaultFc);

        // Rotation widget font
        p_theme.SetFontSize("rotation_control_size", "EditorFonts", 14 * (int)EditorScale.Value);
        p_theme.SetFont("rotation_control", "EditorFonts", defaultFc);

        // Code font
        p_theme.SetFontSize("source_size", "EditorFonts", EDITOR_GET<int>("interface/editor/code_font_size") * (int)EditorScale.Value);
        p_theme.SetFont("source", "EditorFonts", monoFc);

        p_theme.SetFontSize("expression_size", "EditorFonts", (EDITOR_GET<int>("interface/editor/code_font_size") - 1) * (int)EditorScale.Value);
        p_theme.SetFont("expression", "EditorFonts", monoOtherFc);

        p_theme.SetFontSize("output_source_size", "EditorFonts", EDITOR_GET<int>("run/output/font_size") * (int)EditorScale.Value);
        p_theme.SetFont("output_source",             "EditorFonts", monoOtherFc);
        p_theme.SetFont("output_source_bold",        "EditorFonts", monoOtherFcBold);
        p_theme.SetFont("output_source_italic",      "EditorFonts", monoOtherFcItalic);
        p_theme.SetFont("output_source_bold_italic", "EditorFonts", monoOtherFcBoldItalic);
        p_theme.SetFont("output_source_mono",        "EditorFonts", monoOtherFcMono);

        p_theme.SetFontSize("status_source_size", "EditorFonts", defaultFontSize);
        p_theme.SetFont("status_source", "EditorFonts", monoOtherFc);
    }
}
