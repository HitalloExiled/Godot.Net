#define MODULE_SVG_ENABLED
namespace Godot.Net.Editor;

using Godot.Net.Core.Enums;
using Godot.Net.Core.Error;
using Godot.Net.Core.IO;
using Godot.Net.Core.Math;
using Godot.Net.Modules.SVG;
using Godot.Net.Scene.Resources;

public class EditorThemes
{
    #if MODULE_SVG_ENABLED
    private static ImageTexture? EditorGenerateIcon(string source, float scale, float saturation, Dictionary<Color, Color>? convertColors = null)
    {
        var img = new Image();

        // Upsample icon generation only if the editor scale isn't an integer multiplier.
        // Generating upsampled icons is slower, and the benefit is hardly visible
        // with integer editor scales.
        var upsample = !MathX.IsEqualApprox(Math.Round(scale), scale);

        var err = ImageLoaderSVG.CreateImageFromString(img, source, scale, upsample, convertColors ?? new());
        if (ERR_FAIL_COND_V_MSG(err != Error.OK, "Failed generating icon, unsupported or invalid SVG data in editor theme."))
        {
            return null;
        }

        if (saturation != 1)
        {
            img.AdjustBcs(1, 1, saturation);
        }

        return ImageTexture.CreateFromImage(img);
    }
    #endif

    private static void EditorRegisterAndGenerateIcons(Theme theme, bool darkTheme, float iconSaturation, int thumbSize, bool onlyThumbs = false)
    {
        #if MODULE_SVG_ENABLED
        // Before we register the icons, we adjust their colors and saturation.
        // Most icons follow the standard rules for color conversion to follow the editor
        // theme's polarity (dark/light). We also adjust the saturation for most icons,
        // following the editor setting.
        // Some icons are excluded from this conversion, and instead use the configured
        // accent color to replace their innate accent color to match the editor theme.
        // And then some icons are completely excluded from the conversion.

        // Standard color conversion map.
        var colorConversionMap = new Dictionary<Color, Color>();
        // Icons by default are set up for the dark theme, so if the theme is light,
        // we apply the dark-to-light color conversion map.
        if (!darkTheme)
        {
            foreach (var entry in EditorColorMap.ColorConversionMap)
            {
                colorConversionMap[entry.Key] = entry.Value;
            }
        }
        // These colors should be converted even if we are using a dark theme.
        var errorColor   = theme.GetColor("error_color", "Editor");
        var successColor = theme.GetColor("success_color", "Editor");
        var warningColor = theme.GetColor("warning_color", "Editor");

        colorConversionMap[Color.Html("#ff5f5f")] = errorColor;
        colorConversionMap[Color.Html("#5fff97")] = successColor;
        colorConversionMap[Color.Html("#ffdd65")] = warningColor;

        // The names of the icons to exclude from the standard color conversion.
        var conversionExceptions = EditorColorMap.ColorConversionExceptions;

        // The names of the icons to exclude when adjusting for saturation.
        var saturationExceptions = new HashSet<string>
        {
            "DefaultProjectIcon",
            "Godot",
            "Logo"
        };

        // Accent color conversion map.
        // It is used on some icons (checkbox, radio, toggle, etc.), regardless of the dark
        // or light mode.
        var accentColorMap   = new Dictionary<Color, Color>();
        var accentColorIcons = new HashSet<string>();

        var accentColor = theme.GetColor("accent_color", "Editor");
        accentColorMap[Color.Html("699ce8")] = accentColor;
        if (accentColor.Luminance > 0.75)
        {
            accentColorMap[Color.Html("ffffff")] = new Color(0.2f, 0.2f, 0.2f);
        }

        accentColorIcons.Add("GuiChecked");
        accentColorIcons.Add("GuiRadioChecked");
        accentColorIcons.Add("GuiIndeterminate");
        accentColorIcons.Add("GuiToggleOn");
        accentColorIcons.Add("GuiToggleOnMirrored");
        accentColorIcons.Add("PlayOverlay");

        // Generate icons.
        if (!onlyThumbs)
        {
            foreach (var entry in EditorIcons.Sources)
            {
                ImageTexture? icon;

                if (accentColorIcons.Contains(entry.Key))
                {
                    icon = EditorGenerateIcon(entry.Value, EditorScale.Value, 1, accentColorMap);
                }
                else
                {
                    var saturation = iconSaturation;
                    if (saturationExceptions.Contains(entry.Key))
                    {
                        saturation = 1;
                    }

                    icon = conversionExceptions.Contains(entry.Key)
                        ? EditorGenerateIcon(entry.Value, EditorScale.Value, saturation)
                        : EditorGenerateIcon(entry.Value, EditorScale.Value, saturation, colorConversionMap);
                }

                theme.SetIcon(entry.Key, "EditorIcons", icon);
            }
        }

        // Generate thumbnail icons with the given thumbnail size.
        // See editor\icons\editor_icons_builders.py for the code that determines which icons are thumbnails.
        if (thumbSize >= 64)
        {
            var scale = thumbSize / 64f * EditorScale.Value;
            foreach (var entry in EditorIcons.BgThumbs)
            {
                ImageTexture? icon;

                if (accentColorIcons.Contains(entry))
                {
                    icon = EditorGenerateIcon(EditorIcons.Sources[entry], scale, 1, accentColorMap);
                }
                else
                {
                    var saturation = iconSaturation;
                    if (saturationExceptions.Contains(entry))
                    {
                        saturation = 1;
                    }

                    icon = conversionExceptions.Contains(entry)
                        ? EditorGenerateIcon(EditorIcons.Sources[entry], scale, saturation)
                        : EditorGenerateIcon(EditorIcons.Sources[entry], scale, saturation, colorConversionMap);
                }

                theme.SetIcon(entry, "EditorIcons", icon);
            }
        }
        else
        {
            var scale = thumbSize / 32f * EditorScale.Value;
            foreach (var entry in EditorIcons.MdThumbs)
            {
                ImageTexture? icon;

                if (accentColorIcons.Contains(entry))
                {
                    icon = EditorGenerateIcon(EditorIcons.Sources[entry], scale, 1, accentColorMap);
                }
                else
                {
                    var saturation = iconSaturation;
                    if (saturationExceptions.Contains(entry))
                    {
                        saturation = 1;
                    }

                    icon = conversionExceptions.Contains(entry)
                        ? EditorGenerateIcon(EditorIcons.Sources[entry], scale, saturation)
                        : EditorGenerateIcon(EditorIcons.Sources[entry], scale, saturation, colorConversionMap);
                }

                theme.SetIcon(entry, "EditorIcons", icon);
            }
        }
        #else
        WARN_PRINT("SVG support disabled, editor icons won't be rendered.");
        #endif
    }

    private static StyleBoxEmpty MakeEmptyStylebox(
        float marginLeft   = -1,
        float marginTop    = -1,
        float marginRight  = -1,
        float marginBottom = -1)
    {
        var style = new StyleBoxEmpty();

        style.SetContentMarginIndividual(
            marginLeft   * EditorScale.Value,
            marginTop    * EditorScale.Value,
            marginRight  * EditorScale.Value,
            marginBottom * EditorScale.Value
        );

        return style;
    }

    private static StyleBoxFlat MakeFlatStylebox(
        in Color color,
        float    marginLeft   = -1,
        float    marginTop    = -1,
        float    marginRight  = -1,
        float    marginBottom = -1,
        int      cornerWidth  = 0
    )
    {
        var style = new StyleBoxFlat
        {
            BgColor = color,
            // Adjust level of detail based on the corners' effective sizes.
            CornerDetail = (int)MathF.Ceiling(0.8f * cornerWidth * EditorScale.Value)
        };

        style.SetCornerRadiusAll(cornerWidth * (int)EditorScale.Value);
        style.SetContentMarginIndividual(marginLeft * EditorScale.Value, marginTop * EditorScale.Value, marginRight * EditorScale.Value, marginBottom * EditorScale.Value);
        // Work around issue about antialiased edges being blurrier (GH-35279).
        style.AntiAliased = false;

        return style;
    }

    private static StyleBoxLine MakeLineStylebox(
        in Color color,
        int      thickness = 1,
        float    growBegin = 1,
        float    growEnd   = 1,
        bool     vertical  = false
    )
    {
        var style = new StyleBoxLine
        {
            Color     = color,
            GrowBegin = growBegin,
            GrowEnd   = growEnd,
            Thickness = thickness,
            Vertical  = vertical
        };

        return style;
    }

    private static StyleBoxTexture MakeStylebox(
        Texture2D texture,
        float     left,
        float     top,
        float     right,
        float     bottom,
        float     marginLeft   = -1,
        float     marginTop    = -1,
        float     marginRight  = -1,
        float     marginBottom = -1,
        bool      drawCenter   = true
    )
    {
        var style = new StyleBoxTexture
        {
            Texture = texture
        };

        style.SetTextureMarginIndividual(
            left   * EditorScale.Value,
            top    * EditorScale.Value,
            right  * EditorScale.Value,
            bottom * EditorScale.Value
        );

        style.SetContentMarginIndividual(
            (left   + marginLeft)   * EditorScale.Value,
            (top    + marginTop)    * EditorScale.Value,
            (right  + marginRight)  * EditorScale.Value,
            (bottom + marginBottom) * EditorScale.Value
        );

        style.DrawCenter = drawCenter;

        return style;
    }

    public static Theme CreateCustomTheme(Theme? theme = null)
    {
        var editorTheme = CreateEditorTheme(theme);

        var customThemePath = EDITOR_GET<string>("interface/theme/custom_theme");
        if (!string.IsNullOrEmpty(customThemePath))
        {
            if (ResourceLoader.Load(customThemePath) is Theme customTheme)
            {
                editorTheme.MergeWith(customTheme);
            }
        }

        return editorTheme;
    }

    public static Theme CreateEditorTheme(Theme? baseTheme = null)
    {
        var theme = new Theme
        {
            // Controls may rely on the scale for their internal drawing logic.
            DefaultBaseScale = EditorScale.Value
        };

        // Theme settings
        var accentColor                = EDITOR_GET<Color>("interface/theme/accent_color");
        var baseColor                  = EDITOR_GET<Color>("interface/theme/base_color");
        var contrast                   = EDITOR_GET<float>("interface/theme/contrast");
        var increaseScrollbarTouchArea = EDITOR_GET<bool>("interface/touchscreen/increase_scrollbar_touch_area");
        var drawExtraBorders           = EDITOR_GET<bool>("interface/theme/draw_extra_borders");
        var iconSaturation             = EDITOR_GET<float>("interface/theme/icon_saturation");
        var relationshipLineOpacity    = EDITOR_GET<float>("interface/theme/relationship_line_opacity");
        var preset                     = EDITOR_GET<string>("interface/theme/preset");
        var borderSize                 = EDITOR_GET<int>("interface/theme/border_size");
        var cornerRadius               = EDITOR_GET<int>("interface/theme/corner_radius");

        var presetAccentColor      = new Color();
        var presetBaseColor        = new Color();
        var presetContrast         = 0f;
        var presetDrawExtraBorders = false;
        var defaultContrast        = 0.3f;

        // Please use alphabetical order if you're adding a new theme here
        // (after "Custom")

        if (preset == "Custom")
        {
            accentColor = EDITOR_GET<Color>("interface/theme/accent_color");
            baseColor   = EDITOR_GET<Color>("interface/theme/base_color");
            contrast    = EDITOR_GET<float>("interface/theme/contrast");
        }
        else if (preset == "Breeze Dark")
        {
            presetAccentColor = new Color(0.26f, 0.76f, 1);
            presetBaseColor   = new Color(0.24f, 0.26f, 0.28f);
            presetContrast    = defaultContrast;
        }
        else if (preset == "Godot 2")
        {
            presetAccentColor = new Color(0.53f, 0.67f, 0.89f);
            presetBaseColor   = new Color(0.24f, 0.23f, 0.27f);
            presetContrast    = defaultContrast;
        }
        else if (preset == "Gray")
        {
            presetAccentColor = new Color(0.44f, 0.73f, 0.98f);
            presetBaseColor   = new Color(0.24f, 0.24f, 0.24f);
            presetContrast    = defaultContrast;
        }
        else if (preset == "Light")
        {
            presetAccentColor = new Color(0.18f, 0.50f, 1);
            presetBaseColor   = new Color(0.9f, 0.9f, 0.9f);
            // A negative contrast rate looks better for light themes, since it better follows the natural order of UI "elevation".
            presetContrast    = -0.08f;
        }
        else if (preset == "Solarized (Dark)")
        {
            presetAccentColor = new Color(0.15f, 0.55f, 0.82f);
            presetBaseColor   = new Color(0.04f, 0.23f, 0.27f);
            presetContrast    = defaultContrast;
        }
        else if (preset == "Solarized (Light)")
        {
            presetAccentColor = new Color(0.15f, 0.55f, 0.82f);
            presetBaseColor   = new Color(0.89f, 0.86f, 0.79f);
            // A negative contrast rate looks better for light themes, since it better follows the natural order of UI "elevation".
            presetContrast    = -0.08f;
        }
        else if (preset == "Black (OLED)")
        {
            presetAccentColor      = new Color(0.45f, 0.75f, 1);
            presetBaseColor        = new Color(0, 0, 0);
            // The contrast rate value is irrelevant on a fully black theme.
            presetContrast         = 0;
            presetDrawExtraBorders = true;
        }
        else
        {
            // Default
            presetAccentColor = new Color(0.44f, 0.73f, 0.98f);
            presetBaseColor   = new Color(0.21f, 0.24f, 0.29f);
            presetContrast    = defaultContrast;
        }

        if (preset != "Custom")
        {
            accentColor      = presetAccentColor;
            baseColor        = presetBaseColor;
            contrast         = presetContrast;
            drawExtraBorders = presetDrawExtraBorders;

            EditorSettings.Singleton.SetInitialValue("interface/theme/accent_color",       accentColor);
            EditorSettings.Singleton.SetInitialValue("interface/theme/base_color",         baseColor);
            EditorSettings.Singleton.SetInitialValue("interface/theme/contrast",           contrast);
            EditorSettings.Singleton.SetInitialValue("interface/theme/draw_extra_borders", drawExtraBorders);
        }

        EditorSettings.Singleton.SetManually("interface/theme/preset",             preset);
        EditorSettings.Singleton.SetManually("interface/theme/accent_color",       accentColor);
        EditorSettings.Singleton.SetManually("interface/theme/base_color",         baseColor);
        EditorSettings.Singleton.SetManually("interface/theme/contrast",           contrast);
        EditorSettings.Singleton.SetManually("interface/theme/draw_extra_borders", drawExtraBorders);

        // Colors
        var darkTheme = EditorSettings.Singleton.IsDarkTheme;

        #if MODULE_SVG_ENABLED
        if (darkTheme)
        {
            ImageLoaderSVG.SetForcedColorMap(new());
        }
        else
        {
            ImageLoaderSVG.SetForcedColorMap(EditorColorMap.ColorConversionMap);
        }
        #endif

        // Ensure base colors are in the 0..1 luminance range to avoid 8-bit integer overflow or text rendering issues.
        // Some places in the editor use 8-bit integer colors.
        var darkColor1 = baseColor.Lerp(new Color(0, 0, 0, 1), contrast).Clamp();
        var darkColor2 = baseColor.Lerp(new Color(0, 0, 0, 1), contrast * 1.5f).Clamp();
        var darkColor3 = baseColor.Lerp(new Color(0, 0, 0, 1), contrast * 2).Clamp();

        // Only used when the Draw Extra Borders editor setting is enabled.
        var extraBorderColor1 = new Color(0.5f, 0.5f, 0.5f);
        var extraBorderColor2 = darkTheme ? new Color(0.3f, 0.3f, 0.3f) : new Color(0.7f, 0.7f, 0.7f);

        var backgroundColor = darkColor2;

        // White (dark theme) or black (light theme), will be used to generate the rest of the colors
        var monoColor = darkTheme ? new Color(1, 1, 1) : new Color(0, 0, 0);

        var contrastColor1 = baseColor.Lerp(monoColor, Math.Max(contrast, defaultContrast));
        var contrastColor2 = baseColor.Lerp(monoColor, Math.Max(contrast * 1.5f, defaultContrast * 1.5f));

        var fontColor             = monoColor.Lerp(baseColor, 0.25f);
        var fontHoverColor        = monoColor.Lerp(baseColor, 0.125f);
        var fontFocusColor        = monoColor.Lerp(baseColor, 0.125f);
        var fontHoverPressedColor = fontHoverColor.Lerp(accentColor, 0.74f);
        var fontDisabledColor     = new Color(monoColor.R, monoColor.G, monoColor.B, 0.35f);
        var fontReadonlyColor     = new Color(monoColor.R, monoColor.G, monoColor.B, 0.65f);
        var fontPlaceholderColor  = new Color(monoColor.R, monoColor.G, monoColor.B, 0.6f);
        var fontOutlineColor      = new Color(0, 0, 0, 0);
        var selectionColor        = accentColor * new Color(1, 1, 1, 0.4f);
        var disabledColor         = monoColor.Inverted().Lerp(baseColor, 0.7f);
        var disabledBgColor       = monoColor.Inverted().Lerp(baseColor, 0.9f);

        var iconNormalColor = new Color(1, 1, 1);
        var iconHoverColor  = iconNormalColor * (darkTheme ? 1.15f : 1.45f);
        iconHoverColor.A = 1;

        var iconFocusColor    = iconHoverColor;
        var iconDisabledColor = new Color(iconNormalColor, 0.4f);
        // Make the pressed icon color overbright because icons are not completely white on a dark theme.
        // On a light theme, icons are dark, so we need to modulate them with an even brighter color.
        var iconPressedColor = accentColor * (darkTheme ? 1.15f : 3.5f);
        iconPressedColor.A = 1;

        var separatorColor         = new Color(monoColor.R, monoColor.G, monoColor.B, 0.1f);
        var highlightColor         = new Color(accentColor.R, accentColor.G, accentColor.B, 0.275f);
        var disabledHighlightColor = highlightColor.Lerp(darkTheme ? new Color(0, 0, 0) : new Color(1, 1, 1), 0.5f);

        // Can't save single float in theme, so using Color.
        theme.SetColor("icon_saturation",            "Editor", new Color(iconSaturation, iconSaturation, iconSaturation));
        theme.SetColor("accent_color",               "Editor", accentColor);
        theme.SetColor("highlight_color",            "Editor", highlightColor);
        theme.SetColor("disabled_highlight_color",   "Editor", disabledHighlightColor);
        theme.SetColor("base_color",                 "Editor", baseColor);
        theme.SetColor("dark_color_1",               "Editor", darkColor1);
        theme.SetColor("dark_color_2",               "Editor", darkColor2);
        theme.SetColor("dark_color_3",               "Editor", darkColor3);
        theme.SetColor("contrast_color_1",           "Editor", contrastColor1);
        theme.SetColor("contrast_color_2",           "Editor", contrastColor2);
        theme.SetColor("box_selection_fill_color",   "Editor", accentColor * new Color(1, 1, 1, 0.3f));
        theme.SetColor("box_selection_stroke_color", "Editor", accentColor * new Color(1, 1, 1, 0.8f));

        theme.SetColor("axis_x_color", "Editor", new Color(0.96f, 0.20f, 0.32f));
        theme.SetColor("axis_y_color", "Editor", new Color(0.53f, 0.84f, 0.01f));
        theme.SetColor("axis_z_color", "Editor", new Color(0.16f, 0.55f, 0.96f));
        theme.SetColor("axis_w_color", "Editor", new Color(0.55f, 0.55f, 0.55f));

        var propColorSaturation = accentColor.S * 0.75f;
        var propColorValue      = accentColor.V;

        theme.SetColor("property_color_x", "Editor", Color.FromHsv(0 / 3 + 0.05f, propColorSaturation, propColorValue));
        theme.SetColor("property_color_y", "Editor", Color.FromHsv(1 / 3 + 0.05f, propColorSaturation, propColorValue));
        theme.SetColor("property_color_z", "Editor", Color.FromHsv(2 / 3 + 0.05f, propColorSaturation, propColorValue));
        theme.SetColor("property_color_w", "Editor", Color.FromHsv(1.5f / 3 + 0.05f, propColorSaturation, propColorValue));

        theme.SetColor("font_color",             "Editor", fontColor);
        theme.SetColor("highlighted_font_color", "Editor", fontHoverColor);
        theme.SetColor("disabled_font_color",    "Editor", fontDisabledColor);
        theme.SetColor("readonly_font_color",    "Editor", fontReadonlyColor);

        theme.SetColor("mono_color", "Editor", monoColor);

        var successColor         = new Color(0.45f, 0.95f, 0.5f);
        var warningColor         = new Color(1, 0.87f, 0.4f);
        var errorColor           = new Color(1, 0.47f, 0.42f);
        var propertyColor        = fontColor.Lerp(new Color(0.5f, 0.5f, 0.5f), 0.5f);
        var readonlyColor        = propertyColor.Lerp(darkTheme ? new Color(0, 0, 0) : new Color(1, 1, 1), 0.25f);
        var readonlyWarningColor = errorColor.Lerp(darkTheme ? new Color(0, 0, 0) : new Color(1, 1, 1), 0.25f);

        if (!darkTheme)
        {
            // Darken some colors to be readable on a light background.
            successColor = successColor.Lerp(monoColor, 0.35f);
            warningColor = warningColor.Lerp(monoColor, 0.35f);
            errorColor   = errorColor.Lerp(monoColor, 0.25f);
        }

        theme.SetColor("success_color",  "Editor", successColor);
        theme.SetColor("warning_color",  "Editor", warningColor);
        theme.SetColor("error_color",    "Editor", errorColor);
        theme.SetColor("property_color", "Editor", propertyColor);
        theme.SetColor("readonly_color", "Editor", readonlyColor);

        if (!darkTheme)
        {
            theme.SetColor("highend_color", "Editor", Color.Hex(0xad1128ff));
        }
        else
        {
            theme.SetColor("highend_color", "Editor", new Color(1, 0, 0));
        }
        var thumbSize = EDITOR_GET<int>("filesystem/file_dialog/thumbnail_size");
        theme.SetConstant("scale", "Editor", (int)EditorScale.Value);
        theme.SetConstant("thumb_size", "Editor", thumbSize);
        theme.SetConstant("dark_theme", "Editor", Convert.ToInt32(darkTheme));
        theme.SetConstant("color_picker_button_height", "Editor", (int)(28 * EditorScale.Value));

        // Register editor icons.
        // If the settings are comparable to the old theme, then just copy them over.
        // Otherwise, regenerate them. Also check if we need to regenerate "thumb" icons.
        var keepOldIcons         = false;
        var regenerateThumbIcons = true;
        if (baseTheme != null)
        {
            // We check editor scale, theme dark/light mode, icon saturation, and accent color.

            // That doesn't really work as expected, since theme constants are integers, and scales are floats.
            // So this check will never work when changing between 100-199% values.
            var prevScale          = (float)baseTheme.GetConstant("scale", "Editor");
            var prevDarkTheme      = Convert.ToBoolean(baseTheme.GetConstant("dark_theme", "Editor"));
            var prevAccentColor    = baseTheme.GetColor("accent_color", "Editor");
            var prevIconSaturation = baseTheme.GetColor("icon_saturation", "Editor").R;

            keepOldIcons = MathX.IsEqualApprox(prevScale, EditorScale.Value) &&
                prevDarkTheme      == darkTheme &&
                prevAccentColor    == accentColor &&
                prevIconSaturation == iconSaturation;

            var prevThumbSize = (double)baseTheme.GetConstant("thumb_size", "Editor");

            regenerateThumbIcons = !MathX.IsEqualApprox(prevThumbSize, thumbSize);
        }

        if (keepOldIcons)
        {
            foreach (var entry in EditorIcons.Sources)
            {
                theme.SetIcon(entry.Key, "EditorIcons", baseTheme!.GetIcon(entry.Key, "EditorIcons"));
            }
        }
        else
        {
            EditorRegisterAndGenerateIcons(theme, darkTheme, iconSaturation, thumbSize, false);
        }
        if (regenerateThumbIcons)
        {
            EditorRegisterAndGenerateIcons(theme, darkTheme, iconSaturation, thumbSize, true);
        }

        // Register editor fonts.
        EditorFonts.EditorRegisterFonts(theme);

        // Ensure borders are visible when using an editor scale below 100%.
        var borderWidth       = Math.Clamp(borderSize, 0, 2) * Math.Max(1, EditorScale.Value);
        var cornerWidth       = Math.Clamp(cornerRadius, 0, 6);
        var defaultMarginSize = 4;
        var marginSizeExtra   = defaultMarginSize + Math.Clamp(borderSize, 0, 2);

        // Styleboxes
        // This is the most commonly used stylebox, variations should be made as duplicate of this
        var styleDefault = MakeFlatStylebox(baseColor, defaultMarginSize, defaultMarginSize, defaultMarginSize, defaultMarginSize, cornerWidth);
        styleDefault.SetBorderWidthAll((int)borderWidth);
        styleDefault.BorderColor = baseColor;

        // Button and widgets
        var extraSpacing = EDITOR_GET<float>("interface/theme/additional_spacing");

        var widgetDefaultMargin = new Vector2<RealT>(extraSpacing + 6, extraSpacing + defaultMarginSize + 1) * EditorScale.Value;

        var styleWidget = (StyleBoxFlat)styleDefault.Duplicate();
        styleWidget.SetContentMarginIndividual(widgetDefaultMargin.X, widgetDefaultMargin.Y, widgetDefaultMargin.X, widgetDefaultMargin.Y);
        styleWidget.BgColor = darkColor1;
        if (drawExtraBorders)
        {
            styleWidget.SetBorderWidthAll((int)Math.Round(EditorScale.Value));
            styleWidget.BorderColor = extraBorderColor1;
        }
        else
        {
            styleWidget.BorderColor = darkColor2;
        }

        var styleWidgetDisabled = (StyleBoxFlat)styleWidget.Duplicate();
        styleWidgetDisabled.BorderColor = drawExtraBorders ? extraBorderColor2 : disabledColor;
        styleWidgetDisabled.BgColor     = disabledBgColor;

        var styleWidgetFocus = (StyleBoxFlat)styleWidget.Duplicate();
        styleWidgetFocus.DrawCenter  = false;
        styleWidgetFocus.SetBorderWidthAll((int)Math.Round(2 * Math.Max(1, EditorScale.Value)));
        styleWidgetFocus.BorderColor = accentColor;

        var styleWidgetPressed = (StyleBoxFlat)styleWidget.Duplicate();
        styleWidgetPressed.BgColor = darkColor1.Darkened(0.125f);

        var styleWidgetHover = (StyleBoxFlat)styleWidget.Duplicate();
        styleWidgetHover.BgColor     = monoColor * new Color(1, 1, 1, 0.11f);
        styleWidgetHover.BorderColor = drawExtraBorders ? extraBorderColor1 : monoColor * new Color(1, 1, 1, 0.05f);

        // Style for windows, popups, etc..
        var stylePopup      = (StyleBoxFlat)styleDefault.Duplicate();
        var popupMarginSize = defaultMarginSize * EditorScale.Value * 3;

        stylePopup.SetContentMarginAll(popupMarginSize);
        stylePopup.BorderColor = contrastColor1;

        var shadowColor = new Color(0, 0, 0, darkTheme ? 0.3f : 0.1f);

        stylePopup.ShadowColor = shadowColor;
        stylePopup.ShadowSize  = (int)(4 * EditorScale.Value);
        // Popups are separate windows by default in the editor. Windows currently don't support per-pixel transparency
        // in 4.0, and even if it was, it may not always work in practice (e.g. running with compositing disabled).
        stylePopup.SetCornerRadiusAll(0);

        var stylePopupSeparator = new StyleBoxLine
        {
            Color     = separatorColor,
            GrowBegin = popupMarginSize - Math.Max((float)Math.Round(EditorScale.Value), borderWidth),
            GrowEnd   = popupMarginSize - Math.Max((float)Math.Round(EditorScale.Value), borderWidth),
            Thickness = (int)Math.Max((float)Math.Round(EditorScale.Value), borderWidth)
        };

        var stylePopupLabeledSeparatorLeft = new StyleBoxLine
        {
            GrowBegin = popupMarginSize - Math.Max((float)Math.Round(EditorScale.Value), borderWidth),
            Color     = separatorColor,
            Thickness = (int)Math.Max((float)Math.Round(EditorScale.Value), borderWidth)
        };

        var stylePopupLabeledSeparatorRight = new StyleBoxLine
        {
            GrowEnd   = popupMarginSize - Math.Max((float)Math.Round(EditorScale.Value), borderWidth),
            Color     = separatorColor,
            Thickness = (int)Math.Max(Math.Round(EditorScale.Value), borderWidth)
        };

        var styleEmpty = MakeEmptyStylebox(defaultMarginSize, defaultMarginSize, defaultMarginSize, defaultMarginSize);

        // TabBar

        var styleTabBase = (StyleBoxFlat)styleWidget.Duplicate();

        styleTabBase.SetBorderWidthAll(0);
        // Don't round the top corners to avoid creating a small blank space between the tabs and the main panel.
        // This also makes the top highlight look better.
        styleTabBase.SetCornerRadius(Corner.CORNER_BOTTOM_LEFT,  0);
        styleTabBase.SetCornerRadius(Corner.CORNER_BOTTOM_RIGHT, 0);

        // When using a border width greater than 0, visually line up the left of the selected tab with the underlying panel.
        styleTabBase.SetExpandMargin(Side.SIDE_LEFT, -borderWidth);

        styleTabBase.SetContentMargin(Side.SIDE_LEFT,   widgetDefaultMargin.X + 5 * EditorScale.Value);
        styleTabBase.SetContentMargin(Side.SIDE_RIGHT,  widgetDefaultMargin.X + 5 * EditorScale.Value);
        styleTabBase.SetContentMargin(Side.SIDE_BOTTOM, widgetDefaultMargin.Y);
        styleTabBase.SetContentMargin(Side.SIDE_TOP,    widgetDefaultMargin.Y);

        var styleTabSelected = (StyleBoxFlat)styleTabBase.Duplicate();

        styleTabSelected.BgColor = baseColor;
        // Add a highlight line at the top of the selected tab.
        styleTabSelected.SetBorderWidth(Side.SIDE_TOP, (int)Math.Round(2 * EditorScale.Value));
        // Make the highlight line prominent, but not too prominent as to not be distracting.
        var tabHighlight = darkColor2.Lerp(accentColor, 0.75f);
        styleTabSelected.BorderColor = tabHighlight;
        styleTabSelected.SetCornerRadiusAll(0);

        var styleTabUnselected = (StyleBoxFlat)styleTabBase.Duplicate();
        styleTabUnselected.SetExpandMargin(Side.SIDE_BOTTOM, 0);
        styleTabUnselected.BgColor = darkColor1;
        // Add some spacing between unselected tabs to make them easier to distinguish from each other
        styleTabUnselected.BorderColor = new Color(0, 0, 0, 0);

        var styleTabDisabled = (StyleBoxFlat)styleTabBase.Duplicate();
        styleTabDisabled.SetExpandMargin(Side.SIDE_BOTTOM, 0);
        styleTabDisabled.BgColor = disabledBgColor;
        styleTabDisabled.BorderColor = disabledBgColor;

        // Editor background
        var backgroundColorOpaque = backgroundColor;
        backgroundColorOpaque.A = 1;

        theme.SetColor("background", "Editor", backgroundColorOpaque);
        theme.SetStylebox("Background", "EditorStyles", MakeFlatStylebox(backgroundColorOpaque, defaultMarginSize, defaultMarginSize, defaultMarginSize, defaultMarginSize));

        // Focus
        theme.SetStylebox("Focus", "EditorStyles", styleWidgetFocus);
        // Use a less opaque color to be less distracting for the 2D and 3D editor viewports.
        var styleWidgetFocusViewport = (StyleBoxFlat)styleWidgetFocus.Duplicate();
        styleWidgetFocusViewport.BorderColor = accentColor * new Color(1, 1, 1, 0.5f);

        theme.SetStylebox("FocusViewport", "EditorStyles", styleWidgetFocusViewport);

        // Menu
        var styleMenu = (StyleBoxFlat)styleWidget.Duplicate();
        styleMenu.DrawCenter = false;
        styleMenu.SetBorderWidthAll(0);

        theme.SetStylebox("panel",     "PanelContainer", styleMenu);
        theme.SetStylebox("MenuPanel", "EditorStyles",   styleMenu);

        // CanvasItem Editor
        var styleCanvasEditorInfo = MakeFlatStylebox(new Color(0, 0, 0, 0.2f));
        styleCanvasEditorInfo.SetExpandMarginAll(4 * EditorScale.Value);

        theme.SetStylebox("CanvasItemInfoOverlay", "EditorStyles", styleCanvasEditorInfo);

        // 2D and 3D contextual toolbar.
        // Use a custom stylebox to make contextual menu items stand out from the rest.
        // This helps with editor usability as contextual menu items change when selecting nodes,
        // even though it may not be immediately obvious at first.
        var toolbarStylebox = new StyleBoxFlat
        {
            BgColor = accentColor * new Color(1, 1, 1, 0.1f)
        };

        toolbarStylebox.SetCornerRadius(Corner.CORNER_TOP_LEFT, (int)(cornerRadius * EditorScale.Value));
        toolbarStylebox.SetCornerRadius(Corner.CORNER_TOP_RIGHT, (int)(cornerRadius * EditorScale.Value));
        toolbarStylebox.AntiAliased = false;
        // Add an underline to the StyleBox, but prevent its minimum vertical size from changing.
        toolbarStylebox.BorderColor = accentColor;
        toolbarStylebox.SetBorderWidth(Side.SIDE_BOTTOM, (int)Math.Round(2 * EditorScale.Value));
        toolbarStylebox.SetContentMargin(Side.SIDE_BOTTOM, 0);

        theme.SetStylebox("ContextualToolbar", "EditorStyles", toolbarStylebox);

        // Script Editor
        theme.SetStylebox("ScriptEditorPanel", "EditorStyles", MakeEmptyStylebox(defaultMarginSize, 0, defaultMarginSize, defaultMarginSize));
        theme.SetStylebox("ScriptEditor",      "EditorStyles", MakeEmptyStylebox(0, 0, 0, 0));

        // Launch Pad and Play buttons
        var styleLaunchPad = MakeFlatStylebox(darkColor1, 2 * EditorScale.Value, 0, 2 * EditorScale.Value, 0, cornerWidth);
        styleLaunchPad.SetCornerRadiusAll((int)(cornerRadius * EditorScale.Value));

        theme.SetStylebox("LaunchPadNormal", "EditorStyles", styleLaunchPad);

        var styleLaunchPadMovie = (StyleBoxFlat)styleLaunchPad.Duplicate();

        styleLaunchPadMovie.BgColor     = accentColor * new Color(1, 1, 1, 0.1f);
        styleLaunchPadMovie.BorderColor = accentColor;
        styleLaunchPadMovie.SetBorderWidthAll((int)Math.Round(2 * EditorScale.Value));
        theme.SetStylebox("LaunchPadMovieMode", "EditorStyles", styleLaunchPadMovie);

        theme.SetStylebox("MovieWriterButtonNormal", "EditorStyles", MakeEmptyStylebox(0, 0, 0, 0));

        var styleWriteMovieButton = (StyleBoxFlat)styleWidgetPressed.Duplicate();
        styleWriteMovieButton.BgColor = accentColor;
        styleWriteMovieButton.SetCornerRadiusAll((int)(cornerRadius * EditorScale.Value));
        styleWriteMovieButton.SetContentMargin(Side.SIDE_TOP, 0);
        styleWriteMovieButton.SetContentMargin(Side.SIDE_BOTTOM, 0);
        styleWriteMovieButton.SetContentMargin(Side.SIDE_LEFT, 0);
        styleWriteMovieButton.SetContentMargin(Side.SIDE_RIGHT, 0);
        styleWriteMovieButton.SetExpandMargin(Side.SIDE_RIGHT, 2 * EditorScale.Value);

        theme.SetStylebox("MovieWriterButtonPressed", "EditorStyles", styleWriteMovieButton);

        // MenuButton
        theme.SetStylebox("normal",   "MenuButton", styleMenu);
        theme.SetStylebox("hover",    "MenuButton", styleWidgetHover);
        theme.SetStylebox("pressed",  "MenuButton", styleMenu);
        theme.SetStylebox("focus",    "MenuButton", styleMenu);
        theme.SetStylebox("disabled", "MenuButton", styleMenu);

        theme.SetColor("font_color",               "MenuButton", fontColor);
        theme.SetColor("font_hover_color",         "MenuButton", fontHoverColor);
        theme.SetColor("font_hover_pressed_color", "MenuButton", fontHoverPressedColor);
        theme.SetColor("font_focus_color",         "MenuButton", fontFocusColor);
        theme.SetColor("font_outline_color",       "MenuButton", fontOutlineColor);

        theme.SetConstant("outline_size", "MenuButton", 0);

        theme.SetStylebox("MenuHover", "EditorStyles", styleWidgetHover);

        // Buttons
        theme.SetStylebox("normal",   "Button", styleWidget);
        theme.SetStylebox("hover",    "Button", styleWidgetHover);
        theme.SetStylebox("pressed",  "Button", styleWidgetPressed);
        theme.SetStylebox("focus",    "Button", styleWidgetFocus);
        theme.SetStylebox("disabled", "Button", styleWidgetDisabled);

        theme.SetColor("font_color",               "Button", fontColor);
        theme.SetColor("font_hover_color",         "Button", fontHoverColor);
        theme.SetColor("font_hover_pressed_color", "Button", fontHoverPressedColor);
        theme.SetColor("font_focus_color",         "Button", fontFocusColor);
        theme.SetColor("font_pressed_color",       "Button", accentColor);
        theme.SetColor("font_disabled_color",      "Button", fontDisabledColor);
        theme.SetColor("font_outline_color",       "Button", fontOutlineColor);

        theme.SetColor("icon_normal_color",   "Button", iconNormalColor);
        theme.SetColor("icon_hover_color",    "Button", iconHoverColor);
        theme.SetColor("icon_focus_color",    "Button", iconFocusColor);
        theme.SetColor("icon_pressed_color",  "Button", iconPressedColor);
        theme.SetColor("icon_disabled_color", "Button", iconDisabledColor);

        theme.SetConstant("outline_size", "Button", 0);

        var actionButtonExtraMargin = (int)(32 * EditorScale.Value);

        theme.SetTypeVariation("InspectorActionButton", "Button");

        var colorInspectorAction = darkColor1.Lerp(monoColor, 0.12f);
        colorInspectorAction.A = 0.5f;

        var styleInspectorAction = (StyleBoxFlat)styleWidget.Duplicate();

        styleInspectorAction.BgColor = colorInspectorAction;
        styleInspectorAction.SetContentMargin(Side.SIDE_RIGHT, actionButtonExtraMargin);

        theme.SetStylebox("normal", "InspectorActionButton", styleInspectorAction);

        styleInspectorAction = (StyleBoxFlat)styleWidgetHover.Duplicate();
        styleInspectorAction.SetContentMargin(Side.SIDE_RIGHT, actionButtonExtraMargin);

        theme.SetStylebox("hover", "InspectorActionButton", styleInspectorAction);

        styleInspectorAction = (StyleBoxFlat)styleWidgetPressed.Duplicate();
        styleInspectorAction.SetContentMargin(Side.SIDE_RIGHT, actionButtonExtraMargin);

        theme.SetStylebox("pressed", "InspectorActionButton", styleInspectorAction);

        styleInspectorAction = (StyleBoxFlat)styleWidgetDisabled.Duplicate();
        styleInspectorAction.SetContentMargin(Side.SIDE_RIGHT, actionButtonExtraMargin);

        theme.SetStylebox("disabled", "InspectorActionButton", styleInspectorAction);
        theme.SetConstant("h_separation", "InspectorActionButton", actionButtonExtraMargin);

        // Variation for Editor Log filter buttons
        theme.SetTypeVariation("EditorLogFilterButton", "Button");
        // When pressed, don't tint the icons with the accent color, just leave them normal.
        theme.SetColor("icon_pressed_color", "EditorLogFilterButton", iconNormalColor);
        // When unpressed, dim the icons.
        theme.SetColor("icon_normal_color", "EditorLogFilterButton", iconDisabledColor);
        // When pressed, add a small bottom border to the buttons to better show their active state,
        // similar to active tabs.
        var editorLogButtonPressed = (StyleBoxFlat)styleWidgetPressed.Duplicate();
        editorLogButtonPressed.SetBorderWidth(Side.SIDE_BOTTOM, 2 * (int)EditorScale.Value);
        editorLogButtonPressed.BorderColor = accentColor;
        theme.SetStylebox("pressed", "EditorLogFilterButton", editorLogButtonPressed);

        // MenuBar
        theme.SetStylebox("normal",   "MenuBar", styleWidget);
        theme.SetStylebox("hover",    "MenuBar", styleWidgetHover);
        theme.SetStylebox("pressed",  "MenuBar", styleWidgetPressed);
        theme.SetStylebox("focus",    "MenuBar", styleWidgetFocus);
        theme.SetStylebox("disabled", "MenuBar", styleWidgetDisabled);

        theme.SetColor("font_color",               "MenuBar", fontColor);
        theme.SetColor("font_hover_color",         "MenuBar", fontHoverColor);
        theme.SetColor("font_hover_pressed_color", "MenuBar", fontHoverPressedColor);
        theme.SetColor("font_focus_color",         "MenuBar", fontFocusColor);
        theme.SetColor("font_pressed_color",       "MenuBar", accentColor);
        theme.SetColor("font_disabled_color",      "MenuBar", fontDisabledColor);
        theme.SetColor("font_outline_color",       "MenuBar", fontOutlineColor);

        theme.SetColor("icon_normal_color",   "MenuBar", iconNormalColor);
        theme.SetColor("icon_hover_color",    "MenuBar", iconHoverColor);
        theme.SetColor("icon_focus_color",    "MenuBar", iconFocusColor);
        theme.SetColor("icon_pressed_color",  "MenuBar", iconPressedColor);
        theme.SetColor("icon_disabled_color", "MenuBar", iconDisabledColor);

        theme.SetConstant("outline_size", "MenuBar", 0);

        // OptionButton
        var styleOptionButtonFocus    = (StyleBoxFlat)styleWidgetFocus.Duplicate();
        var styleOptionButtonNormal   = (StyleBoxFlat)styleWidget.Duplicate();
        var styleOptionButtonHover    = (StyleBoxFlat)styleWidgetHover.Duplicate();
        var styleOptionButtonPressed  = (StyleBoxFlat)styleWidgetPressed.Duplicate();
        var styleOptionButtonDisabled = (StyleBoxFlat)styleWidgetDisabled.Duplicate();

        styleOptionButtonFocus.SetContentMargin(Side.SIDE_RIGHT, 4 * EditorScale.Value);
        styleOptionButtonNormal.SetContentMargin(Side.SIDE_RIGHT, 4 * EditorScale.Value);
        styleOptionButtonHover.SetContentMargin(Side.SIDE_RIGHT, 4 * EditorScale.Value);
        styleOptionButtonPressed.SetContentMargin(Side.SIDE_RIGHT, 4 * EditorScale.Value);
        styleOptionButtonDisabled.SetContentMargin(Side.SIDE_RIGHT, 4 * EditorScale.Value);

        theme.SetStylebox("focus",    "OptionButton", styleOptionButtonFocus);
        theme.SetStylebox("normal",   "OptionButton", styleWidget);
        theme.SetStylebox("hover",    "OptionButton", styleWidgetHover);
        theme.SetStylebox("pressed",  "OptionButton", styleWidgetPressed);
        theme.SetStylebox("disabled", "OptionButton", styleWidgetDisabled);

        theme.SetStylebox("normal_mirrored",   "OptionButton", styleOptionButtonNormal);
        theme.SetStylebox("hover_mirrored",    "OptionButton", styleOptionButtonHover);
        theme.SetStylebox("pressed_mirrored",  "OptionButton", styleOptionButtonPressed);
        theme.SetStylebox("disabled_mirrored", "OptionButton", styleOptionButtonDisabled);

        theme.SetColor("font_color",               "OptionButton", fontColor);
        theme.SetColor("font_hover_color",         "OptionButton", fontHoverColor);
        theme.SetColor("font_hover_pressed_color", "OptionButton", fontHoverPressedColor);
        theme.SetColor("font_focus_color",         "OptionButton", fontFocusColor);
        theme.SetColor("font_pressed_color",       "OptionButton", accentColor);
        theme.SetColor("font_disabled_color",      "OptionButton", fontDisabledColor);
        theme.SetColor("font_outline_color",       "OptionButton", fontOutlineColor);

        theme.SetColor("icon_normal_color",   "OptionButton", iconNormalColor);
        theme.SetColor("icon_hover_color",    "OptionButton", iconHoverColor);
        theme.SetColor("icon_focus_color",    "OptionButton", iconFocusColor);
        theme.SetColor("icon_pressed_color",  "OptionButton", iconPressedColor);
        theme.SetColor("icon_disabled_color", "OptionButton", iconDisabledColor);

        theme.SetIcon("arrow", "OptionButton", theme.GetIcon("GuiOptionArrow", "EditorIcons"));
        theme.SetConstant("arrow_margin",   "OptionButton", (int)(widgetDefaultMargin.X - 2 * EditorScale.Value));
        theme.SetConstant("modulate_arrow", "OptionButton", Convert.ToInt32(true));
        theme.SetConstant("h_separation",   "OptionButton", (int)(4 * EditorScale.Value));
        theme.SetConstant("outline_size",   "OptionButton", 0);

        // CheckButton
        theme.SetStylebox("normal",        "CheckButton", styleMenu);
        theme.SetStylebox("pressed",       "CheckButton", styleMenu);
        theme.SetStylebox("disabled",      "CheckButton", styleMenu);
        theme.SetStylebox("hover",         "CheckButton", styleMenu);
        theme.SetStylebox("hover_pressed", "CheckButton", styleMenu);

        theme.SetIcon("checked",            "CheckButton", theme.GetIcon("GuiToggleOn",          "EditorIcons"));
        theme.SetIcon("checked_disabled",   "CheckButton", theme.GetIcon("GuiToggleOnDisabled",  "EditorIcons"));
        theme.SetIcon("unchecked",          "CheckButton", theme.GetIcon("GuiToggleOff",         "EditorIcons"));
        theme.SetIcon("unchecked_disabled", "CheckButton", theme.GetIcon("GuiToggleOffDisabled", "EditorIcons"));

        theme.SetIcon("checked_mirrored",            "CheckButton", theme.GetIcon("GuiToggleOnMirrored",          "EditorIcons"));
        theme.SetIcon("checked_disabled_mirrored",   "CheckButton", theme.GetIcon("GuiToggleOnDisabledMirrored",  "EditorIcons"));
        theme.SetIcon("unchecked_mirrored",          "CheckButton", theme.GetIcon("GuiToggleOffMirrored",         "EditorIcons"));
        theme.SetIcon("unchecked_disabled_mirrored", "CheckButton", theme.GetIcon("GuiToggleOffDisabledMirrored", "EditorIcons"));

        theme.SetColor("font_color",               "CheckButton", fontColor);
        theme.SetColor("font_hover_color",         "CheckButton", fontHoverColor);
        theme.SetColor("font_hover_pressed_color", "CheckButton", fontHoverPressedColor);
        theme.SetColor("font_focus_color",         "CheckButton", fontFocusColor);
        theme.SetColor("font_pressed_color",       "CheckButton", accentColor);
        theme.SetColor("font_disabled_color",      "CheckButton", fontDisabledColor);
        theme.SetColor("font_outline_color",       "CheckButton", fontOutlineColor);

        theme.SetColor("icon_normal_color",   "CheckButton", iconNormalColor);
        theme.SetColor("icon_hover_color",    "CheckButton", iconHoverColor);
        theme.SetColor("icon_focus_color",    "CheckButton", iconFocusColor);
        theme.SetColor("icon_pressed_color",  "CheckButton", iconPressedColor);
        theme.SetColor("icon_disabled_color", "CheckButton", iconDisabledColor);

        theme.SetConstant("h_separation",   "CheckButton", 8 * (int)EditorScale.Value);
        theme.SetConstant("check_v_offset", "CheckButton", 0);
        theme.SetConstant("outline_size",   "CheckButton", 0);

        // Checkbox
        var sbCheckbox = (StyleBoxFlat)styleMenu.Duplicate();
        sbCheckbox.SetContentMarginAll(defaultMarginSize * EditorScale.Value);

        theme.SetStylebox("normal",        "CheckBox", sbCheckbox);
        theme.SetStylebox("pressed",       "CheckBox", sbCheckbox);
        theme.SetStylebox("disabled",      "CheckBox", sbCheckbox);
        theme.SetStylebox("hover",         "CheckBox", sbCheckbox);
        theme.SetStylebox("hover_pressed", "CheckBox", sbCheckbox);

        theme.SetIcon("checked",                  "CheckBox", theme.GetIcon("GuiChecked",                "EditorIcons"));
        theme.SetIcon("unchecked",                "CheckBox", theme.GetIcon("GuiUnchecked",              "EditorIcons"));
        theme.SetIcon("radio_checked",            "CheckBox", theme.GetIcon("GuiRadioChecked",           "EditorIcons"));
        theme.SetIcon("radio_unchecked",          "CheckBox", theme.GetIcon("GuiRadioUnchecked",         "EditorIcons"));
        theme.SetIcon("checked_disabled",         "CheckBox", theme.GetIcon("GuiCheckedDisabled",        "EditorIcons"));
        theme.SetIcon("unchecked_disabled",       "CheckBox", theme.GetIcon("GuiUncheckedDisabled",      "EditorIcons"));
        theme.SetIcon("radio_checked_disabled",   "CheckBox", theme.GetIcon("GuiRadioCheckedDisabled",   "EditorIcons"));
        theme.SetIcon("radio_unchecked_disabled", "CheckBox", theme.GetIcon("GuiRadioUncheckedDisabled", "EditorIcons"));

        theme.SetColor("font_color",               "CheckBox", fontColor);
        theme.SetColor("font_hover_color",         "CheckBox", fontHoverColor);
        theme.SetColor("font_hover_pressed_color", "CheckBox", fontHoverPressedColor);
        theme.SetColor("font_focus_color",         "CheckBox", fontFocusColor);
        theme.SetColor("font_pressed_color",       "CheckBox", accentColor);
        theme.SetColor("font_disabled_color",      "CheckBox", fontDisabledColor);
        theme.SetColor("font_outline_color",       "CheckBox", fontOutlineColor);

        theme.SetColor("icon_normal_color",   "CheckBox", iconNormalColor);
        theme.SetColor("icon_hover_color",    "CheckBox", iconHoverColor);
        theme.SetColor("icon_focus_color",    "CheckBox", iconFocusColor);
        theme.SetColor("icon_pressed_color",  "CheckBox", iconPressedColor);
        theme.SetColor("icon_disabled_color", "CheckBox", iconDisabledColor);

        theme.SetConstant("h_separation",   "CheckBox", 8 * (int)EditorScale.Value);
        theme.SetConstant("check_v_offset", "CheckBox", 0);
        theme.SetConstant("outline_size",   "CheckBox", 0);

        // PopupDialog
        theme.SetStylebox("panel", "PopupDialog", stylePopup);

        // PopupMenu
        var stylePopupMenu = (StyleBoxFlat)stylePopup.Duplicate();
        // Use 1 pixel for the sides, since if 0 is used, the highlight of hovered items is drawn
        // on top of the popup border. This causes a 'gap' in the panel border when an item is highlighted,
        // and it looks weird. 1px solves this.
        stylePopupMenu.SetContentMarginIndividual(EditorScale.Value, 2 * EditorScale.Value, EditorScale.Value, 2 * EditorScale.Value);
        // Always display a border for PopupMenus so they can be distinguished from their background.
        stylePopupMenu.SetBorderWidthAll((int)EditorScale.Value);
        stylePopupMenu.BorderColor = drawExtraBorders ? extraBorderColor2 : darkColor2;
        theme.SetStylebox("panel", "PopupMenu", stylePopupMenu);

        var styleMenuHover = (StyleBoxFlat)styleWidgetHover.Duplicate();
        // Don't use rounded corners for hover highlights since the StyleBox touches the PopupMenu's edges.
        styleMenuHover.SetCornerRadiusAll(0);
        theme.SetStylebox("hover", "PopupMenu", styleMenuHover);

        theme.SetStylebox("separator",               "PopupMenu", stylePopupSeparator);
        theme.SetStylebox("labeled_separator_left",  "PopupMenu", stylePopupLabeledSeparatorLeft);
        theme.SetStylebox("labeled_separator_right", "PopupMenu", stylePopupLabeledSeparatorRight);

        theme.SetColor("font_color",             "PopupMenu", fontColor);
        theme.SetColor("font_hover_color",       "PopupMenu", fontHoverColor);
        theme.SetColor("font_accelerator_color", "PopupMenu", fontDisabledColor);
        theme.SetColor("font_disabled_color",    "PopupMenu", fontDisabledColor);
        theme.SetColor("font_separator_color",   "PopupMenu", fontDisabledColor);
        theme.SetColor("font_outline_color",     "PopupMenu", fontOutlineColor);

        theme.SetIcon("checked",                  "PopupMenu", theme.GetIcon("GuiChecked",                "EditorIcons"));
        theme.SetIcon("unchecked",                "PopupMenu", theme.GetIcon("GuiUnchecked",              "EditorIcons"));
        theme.SetIcon("radio_checked",            "PopupMenu", theme.GetIcon("GuiRadioChecked",           "EditorIcons"));
        theme.SetIcon("radio_unchecked",          "PopupMenu", theme.GetIcon("GuiRadioUnchecked",         "EditorIcons"));
        theme.SetIcon("checked_disabled",         "PopupMenu", theme.GetIcon("GuiCheckedDisabled",        "EditorIcons"));
        theme.SetIcon("unchecked_disabled",       "PopupMenu", theme.GetIcon("GuiUncheckedDisabled",      "EditorIcons"));
        theme.SetIcon("radio_checked_disabled",   "PopupMenu", theme.GetIcon("GuiRadioCheckedDisabled",   "EditorIcons"));
        theme.SetIcon("radio_unchecked_disabled", "PopupMenu", theme.GetIcon("GuiRadioUncheckedDisabled", "EditorIcons"));
        theme.SetIcon("submenu",                  "PopupMenu", theme.GetIcon("ArrowRight",                "EditorIcons"));
        theme.SetIcon("submenu_mirrored",         "PopupMenu", theme.GetIcon("ArrowLeft",                 "EditorIcons"));
        theme.SetIcon("visibility_hidden",        "PopupMenu", theme.GetIcon("GuiVisibilityHidden",       "EditorIcons"));
        theme.SetIcon("visibility_visible",       "PopupMenu", theme.GetIcon("GuiVisibilityVisible",      "EditorIcons"));
        theme.SetIcon("visibility_xray",          "PopupMenu", theme.GetIcon("GuiVisibilityXray",         "EditorIcons"));

        // Force the v_separation to be even so that the spacing on top and bottom is even.
        // If the vsep is odd and cannot be split into 2 even groups (of pixels), then it will be lopsided.
        // We add 2 to the vsep to give it some extra spacing which looks a bit more modern (see Windows, for example).
        var vsepBase      = extraSpacing + defaultMarginSize + 6;
        var forceEvenVsep = vsepBase + vsepBase % 2;

        theme.SetConstant("v_separation",       "PopupMenu", (int)(forceEvenVsep * EditorScale.Value));
        theme.SetConstant("outline_size",       "PopupMenu", 0);
        theme.SetConstant("item_start_padding", "PopupMenu", (int)(defaultMarginSize * 1.5f * EditorScale.Value));
        theme.SetConstant("item_end_padding",   "PopupMenu", (int)(defaultMarginSize * 1.5f * EditorScale.Value));

        // Sub-inspectors
        for (var i = 0; i < 16; i++)
        {
            var siBaseColor = accentColor;
            var hueRotate   = i * 2 % 16 / 16f;

            siBaseColor.SetHsv(siBaseColor.H + hueRotate % 1f, siBaseColor.S, siBaseColor.V);
            siBaseColor = accentColor.Lerp(siBaseColor, EDITOR_GET<float>("docks/property_editor/subresource_hue_tint"));

            // Sub-inspector background.
            var subInspectorBg = (StyleBoxFlat)styleDefault.Duplicate();
            subInspectorBg.BgColor = darkColor1.Lerp(siBaseColor, 0.08f);
            subInspectorBg.SetBorderWidthAll(2 * (int)EditorScale.Value);
            subInspectorBg.BorderColor = siBaseColor * new Color(0.7f, 0.7f, 0.7f, 0.8f);
            subInspectorBg.SetContentMarginAll(4 * EditorScale.Value);
            subInspectorBg.SetCornerRadius(Corner.CORNER_TOP_LEFT, 0);
            subInspectorBg.SetCornerRadius(Corner.CORNER_TOP_RIGHT, 0);

            theme.SetStylebox($"sub_inspector_bg{i}", "Editor", subInspectorBg);

            // EditorProperty background while it has a sub-inspector open.
            var bgColor = MakeFlatStylebox(siBaseColor * new Color(0.7f, 0.7f, 0.7f, 0.8f), 0, 0, 0, 0, cornerRadius);
            bgColor.AntiAliased = false;
            bgColor.SetCornerRadius(Corner.CORNER_BOTTOM_LEFT, 0);
            bgColor.SetCornerRadius(Corner.CORNER_BOTTOM_RIGHT, 0);

            theme.SetStylebox($"sub_inspector_property_bg{i}", "Editor", bgColor);
        }

        theme.SetColor("sub_inspector_property_color", "Editor", darkTheme ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 1));

        // EditorSpinSlider.
        theme.SetColor("label_color", "EditorSpinSlider", fontColor);
        theme.SetColor("read_only_label_color", "EditorSpinSlider", fontReadonlyColor);

        var editorSpinLabelBg = (StyleBoxFlat)styleDefault.Duplicate();
        editorSpinLabelBg.BgColor = darkColor3;
        editorSpinLabelBg.SetBorderWidthAll(0);
        theme.SetStylebox("label_bg", "EditorSpinSlider", editorSpinLabelBg);

        // EditorProperty
        var stylePropertyBg = (StyleBoxFlat)styleDefault.Duplicate();
        stylePropertyBg.BgColor = highlightColor;
        stylePropertyBg.SetBorderWidthAll(0);

        var stylePropertyChildBg = (StyleBoxFlat)styleDefault.Duplicate();
        stylePropertyChildBg.BgColor = darkColor2;
        stylePropertyChildBg.SetBorderWidthAll(0);

        theme.SetConstant("font_offset", "EditorProperty", 8 * (int)EditorScale.Value);

        theme.SetStylebox("bg_selected", "EditorProperty", stylePropertyBg);
        theme.SetStylebox("bg",          "EditorProperty", new StyleBoxEmpty());
        theme.SetStylebox("child_bg",    "EditorProperty", stylePropertyChildBg);

        theme.SetConstant("v_separation", "EditorProperty", (int)((extraSpacing + defaultMarginSize) * EditorScale.Value));

        theme.SetColor("warning_color",          "EditorProperty", warningColor);
        theme.SetColor("property_color",         "EditorProperty", propertyColor);
        theme.SetColor("readonly_color",         "EditorProperty", readonlyColor);
        theme.SetColor("readonly_warning_color", "EditorProperty", readonlyWarningColor);

        var stylePropertyGroupNote = (StyleBoxFlat)styleDefault.Duplicate();
        var propertyGroupNoteColor = accentColor;
        propertyGroupNoteColor.A = 0.1f;
        stylePropertyGroupNote.BgColor = propertyGroupNoteColor;

        theme.SetStylebox("bg_group_note", "EditorProperty", stylePropertyGroupNote);

        // EditorInspectorSection
        var inspectorSectionColor = fontColor.Lerp(new Color(0.5f, 0.5f, 0.5f), 0.35f);
        theme.SetColor("font_color", "EditorInspectorSection", inspectorSectionColor);

        var inspectorIndentColor = accentColor;
        inspectorIndentColor.A = 0.2f;
        var inspectorIndentStyle = MakeFlatStylebox(inspectorIndentColor, 2, 0, 2, 0);

        theme.SetStylebox("indent_box", "EditorInspectorSection", inspectorIndentStyle);

        theme.SetConstant("indent_size",      "EditorInspectorSection", 6);
        theme.SetConstant("inspector_margin", "Editor",                 12 * (int)EditorScale.Value);

        // Tree & ItemList background
        var styleTreeBg = (StyleBoxFlat)styleDefault.Duplicate();
        // Make Trees easier to distinguish from other controls by using a darker background color.
        styleTreeBg.BgColor = darkColor1.Lerp(darkColor2, 0.5f);
        if (drawExtraBorders)
        {
            styleTreeBg.SetBorderWidthAll((int)Math.Round(EditorScale.Value));
            styleTreeBg.BorderColor = extraBorderColor2;
        }
        else
        {
            styleTreeBg.BorderColor = darkColor3;
        }

        theme.SetStylebox("panel", "Tree", styleTreeBg);

        // Tree
        theme.SetIcon("checked",                  "Tree", theme.GetIcon("GuiChecked", "EditorIcons"));
        theme.SetIcon("indeterminate",            "Tree", theme.GetIcon("GuiIndeterminate", "EditorIcons"));
        theme.SetIcon("unchecked",                "Tree", theme.GetIcon("GuiUnchecked", "EditorIcons"));
        theme.SetIcon("arrow",                    "Tree", theme.GetIcon("GuiTreeArrowDown", "EditorIcons"));
        theme.SetIcon("arrow_collapsed",          "Tree", theme.GetIcon("GuiTreeArrowRight", "EditorIcons"));
        theme.SetIcon("arrow_collapsed_mirrored", "Tree", theme.GetIcon("GuiTreeArrowLeft", "EditorIcons"));
        theme.SetIcon("updown",                   "Tree", theme.GetIcon("GuiTreeUpdown", "EditorIcons"));
        theme.SetIcon("select_arrow",             "Tree", theme.GetIcon("GuiDropdown", "EditorIcons"));

        theme.SetStylebox("focus",                 "Tree", styleWidgetFocus);
        theme.SetStylebox("custom_button",         "Tree", MakeEmptyStylebox());
        theme.SetStylebox("custom_button_pressed", "Tree", MakeEmptyStylebox());
        theme.SetStylebox("custom_button_hover",   "Tree", styleWidget);

        theme.SetColor("custom_button_font_highlight", "Tree", fontHoverColor);
        theme.SetColor("font_color",                   "Tree", fontColor);
        theme.SetColor("font_selected_color",          "Tree", monoColor);
        theme.SetColor("font_outline_color",           "Tree", fontOutlineColor);
        theme.SetColor("title_button_color",           "Tree", fontColor);
        theme.SetColor("drop_position_color",          "Tree", accentColor);

        theme.SetConstant("v_separation",  "Tree", (int)(widgetDefaultMargin.Y - EditorScale.Value));
        theme.SetConstant("h_separation",  "Tree", (int)(6 * EditorScale.Value));
        theme.SetConstant("guide_width",   "Tree", (int)borderWidth);
        theme.SetConstant("item_margin",   "Tree", (int)(3 * defaultMarginSize * EditorScale.Value));
        theme.SetConstant("button_margin", "Tree", (int)(defaultMarginSize * EditorScale.Value));
        theme.SetConstant("scroll_border", "Tree", 40);
        theme.SetConstant("scroll_speed",  "Tree", 12);
        theme.SetConstant("outline_size",  "Tree", 0);

        var guideColor            = monoColor * new Color(1, 1, 1, 0.05f);
        var relationshipLineColor = monoColor * new Color(1, 1, 1, relationshipLineOpacity);

        theme.SetConstant("draw_guides", "Tree", Convert.ToInt32(relationshipLineOpacity < 0.01f));
        theme.SetColor("guide_color", "Tree", guideColor);

        var relationshipLineWidth = 1;
        var parentLineColor       = monoColor * new Color(1, 1, 1, Math.Clamp(relationshipLineOpacity + 0.45f, 0, 1));
        var childrenLineColor     = monoColor * new Color(1, 1, 1, Math.Clamp(relationshipLineOpacity + 0.25f, 0, 1));

        theme.SetConstant("draw_relationship_lines", "Tree", Convert.ToInt32(relationshipLineOpacity >= 0.01));
        theme.SetConstant("relationship_line_width", "Tree", relationshipLineWidth);
        theme.SetConstant("parent_hl_line_width",    "Tree", relationshipLineWidth * 2);
        theme.SetConstant("children_hl_line_width",  "Tree", relationshipLineWidth);
        theme.SetConstant("parent_hl_line_margin",   "Tree", relationshipLineWidth * 3);

        theme.SetColor("relationship_line_color", "Tree", relationshipLineColor);
        theme.SetColor("parent_hl_line_color",    "Tree", parentLineColor);
        theme.SetColor("children_hl_line_color",  "Tree", childrenLineColor);

        var styleTreeBtn = (StyleBoxFlat)styleDefault.Duplicate();
        styleTreeBtn.BgColor = highlightColor;
        styleTreeBtn.SetBorderWidthAll(0);
        theme.SetStylebox("button_pressed", "Tree", styleTreeBtn);

        var styleTreeHover = (StyleBoxFlat)styleDefault.Duplicate();
        styleTreeHover.BgColor = highlightColor * new Color(1, 1, 1, 0.4f);
        styleTreeHover.SetBorderWidthAll(0);
        theme.SetStylebox("hover", "Tree", styleTreeHover);

        var styleTreeFocus = (StyleBoxFlat)styleDefault.Duplicate();
        styleTreeFocus.BgColor = highlightColor;
        styleTreeFocus.SetBorderWidthAll(0);
        theme.SetStylebox("selected_focus", "Tree", styleTreeFocus);

        var styleTreeSelected = (StyleBoxFlat)styleTreeFocus.Duplicate();
        theme.SetStylebox("selected", "Tree", styleTreeSelected);

        var styleTreeCursor = (StyleBoxFlat)styleDefault.Duplicate();
        styleTreeCursor.DrawCenter = false;
        styleTreeCursor.SetBorderWidthAll((int)Math.Max(1, borderWidth));
        styleTreeCursor.BorderColor = contrastColor1;

        var styleTreeTitle = (StyleBoxFlat)styleDefault.Duplicate();
        styleTreeTitle.BgColor = darkColor3;
        styleTreeTitle.SetBorderWidthAll(0);
        theme.SetStylebox("cursor", "Tree", styleTreeCursor);
        theme.SetStylebox("cursor_unfocused", "Tree", styleTreeCursor);
        theme.SetStylebox("title_button_normal", "Tree", styleTreeTitle);
        theme.SetStylebox("title_button_hover", "Tree", styleTreeTitle);
        theme.SetStylebox("title_button_pressed", "Tree", styleTreeTitle);

        var propCategoryColor   = darkColor1.Lerp(monoColor, 0.12f);
        var propSectionColor    = darkColor1.Lerp(monoColor, 0.09f);
        var propSubsectionColor = darkColor1.Lerp(monoColor, 0.06f);

        theme.SetColor("prop_category",       "Editor", propCategoryColor);
        theme.SetColor("prop_section",        "Editor", propSectionColor);
        theme.SetColor("prop_subsection",     "Editor", propSubsectionColor);
        theme.SetColor("drop_position_color", "Tree", accentColor);

        // EditorInspectorCategory
        var categoryBg = (StyleBoxFlat)styleDefault.Duplicate();
        categoryBg.BgColor = propCategoryColor;
        categoryBg.BorderColor = propCategoryColor;
        theme.SetStylebox("bg", "EditorInspectorCategory", categoryBg);

        // ItemList
        var styleItemlistBg = (StyleBoxFlat)styleDefault.Duplicate();
        styleItemlistBg.BgColor = darkColor1;

        if (drawExtraBorders)
        {
            styleItemlistBg.SetBorderWidthAll((int)Math.Round(EditorScale.Value));
            styleItemlistBg.BorderColor = extraBorderColor2;
        }
        else
        {
            styleItemlistBg.SetBorderWidthAll((int)borderWidth);
            styleItemlistBg.BorderColor = darkColor3;
        }

        var styleItemlistCursor = (StyleBoxFlat)styleDefault.Duplicate();
        styleItemlistCursor.DrawCenter = false;
        styleItemlistCursor.SetBorderWidthAll((int)borderWidth);
        styleItemlistCursor.BorderColor = highlightColor;

        theme.SetStylebox("panel",            "ItemList", styleItemlistBg);
        theme.SetStylebox("focus",            "ItemList", styleWidgetFocus);
        theme.SetStylebox("cursor",           "ItemList", styleItemlistCursor);
        theme.SetStylebox("cursor_unfocused", "ItemList", styleItemlistCursor);
        theme.SetStylebox("selected_focus",   "ItemList", styleTreeFocus);
        theme.SetStylebox("selected",         "ItemList", styleTreeSelected);

        theme.SetColor("font_color",          "ItemList", fontColor);
        theme.SetColor("font_selected_color", "ItemList", monoColor);
        theme.SetColor("font_outline_color",  "ItemList", fontOutlineColor);
        theme.SetColor("guide_color",         "ItemList", guideColor);

        theme.SetConstant("v_separation",    "ItemList", (int)(forceEvenVsep * 0.5f * EditorScale.Value));
        theme.SetConstant("h_separation",    "ItemList", 6 * (int)EditorScale.Value);
        theme.SetConstant("icon_margin",     "ItemList", 6 * (int)EditorScale.Value);
        theme.SetConstant("line_separation", "ItemList", 3 * (int)EditorScale.Value);
        theme.SetConstant("outline_size",    "ItemList", 0);

        // TabBar & TabContainer
        var styleTabbarBackground = MakeFlatStylebox(darkColor1, 0, 0, 0, 0, (int)(cornerRadius * EditorScale.Value));
        styleTabbarBackground.SetCornerRadius(Corner.CORNER_BOTTOM_LEFT, 0);
        styleTabbarBackground.SetCornerRadius(Corner.CORNER_BOTTOM_RIGHT, 0);
        theme.SetStylebox("tabbar_background", "TabContainer", styleTabbarBackground);

        theme.SetStylebox("tab_selected",     "TabContainer", styleTabSelected);
        theme.SetStylebox("tab_unselected",   "TabContainer", styleTabUnselected);
        theme.SetStylebox("tab_disabled",     "TabContainer", styleTabDisabled);
        theme.SetStylebox("tab_selected",     "TabBar",       styleTabSelected);
        theme.SetStylebox("tab_unselected",   "TabBar",       styleTabUnselected);
        theme.SetStylebox("tab_disabled",     "TabBar",       styleTabDisabled);
        theme.SetStylebox("button_pressed",   "TabBar",       styleMenu);
        theme.SetStylebox("button_highlight", "TabBar",       styleMenu);

        theme.SetColor("font_selected_color",   "TabContainer", fontColor);
        theme.SetColor("font_unselected_color", "TabContainer", fontDisabledColor);
        theme.SetColor("font_outline_color",    "TabContainer", fontOutlineColor);
        theme.SetColor("font_selected_color",   "TabBar",       fontColor);
        theme.SetColor("font_unselected_color", "TabBar",       fontDisabledColor);
        theme.SetColor("font_outline_color",    "TabBar",       fontOutlineColor);
        theme.SetColor("drop_mark_color",       "TabContainer", tabHighlight);
        theme.SetColor("drop_mark_color",       "TabBar",       tabHighlight);

        theme.SetIcon("menu",                "TabContainer", theme.GetIcon("GuiTabMenu", "EditorIcons"));
        theme.SetIcon("menu_highlight",      "TabContainer", theme.GetIcon("GuiTabMenuHl", "EditorIcons"));
        theme.SetIcon("close",               "TabBar",       theme.GetIcon("GuiClose", "EditorIcons"));
        theme.SetIcon("increment",           "TabContainer", theme.GetIcon("GuiScrollArrowRight", "EditorIcons"));
        theme.SetIcon("decrement",           "TabContainer", theme.GetIcon("GuiScrollArrowLeft", "EditorIcons"));
        theme.SetIcon("increment",           "TabBar",       theme.GetIcon("GuiScrollArrowRight", "EditorIcons"));
        theme.SetIcon("decrement",           "TabBar",       theme.GetIcon("GuiScrollArrowLeft", "EditorIcons"));
        theme.SetIcon("increment_highlight", "TabBar",       theme.GetIcon("GuiScrollArrowRightHl", "EditorIcons"));
        theme.SetIcon("decrement_highlight", "TabBar",       theme.GetIcon("GuiScrollArrowLeftHl", "EditorIcons"));
        theme.SetIcon("increment_highlight", "TabContainer", theme.GetIcon("GuiScrollArrowRightHl", "EditorIcons"));
        theme.SetIcon("decrement_highlight", "TabContainer", theme.GetIcon("GuiScrollArrowLeftHl", "EditorIcons"));
        theme.SetIcon("drop_mark",           "TabContainer", theme.GetIcon("GuiTabDropMark", "EditorIcons"));
        theme.SetIcon("drop_mark",           "TabBar",       theme.GetIcon("GuiTabDropMark", "EditorIcons"));

        theme.SetConstant("side_margin",  "TabContainer", 0);
        theme.SetConstant("outline_size", "TabContainer", 0);
        theme.SetConstant("h_separation", "TabBar",       4 * (int)EditorScale.Value);
        theme.SetConstant("outline_size", "TabBar",       0);

        // Content of each tab.
        var styleContentPanel = (StyleBoxFlat)styleDefault.Duplicate();
        styleContentPanel.BorderColor = darkColor3;
        styleContentPanel.SetBorderWidthAll((int)borderWidth);
        styleContentPanel.SetBorderWidth(Side.SIDE_TOP, 0);
        styleContentPanel.SetCornerRadius(Corner.CORNER_TOP_LEFT, 0);
        styleContentPanel.SetCornerRadius(Corner.CORNER_TOP_RIGHT, 0);
        // Compensate for the border.
        styleContentPanel.SetContentMarginIndividual(marginSizeExtra * EditorScale.Value, (2 + marginSizeExtra) * EditorScale.Value, marginSizeExtra * EditorScale.Value, marginSizeExtra * EditorScale.Value);
        theme.SetStylebox("panel", "TabContainer", styleContentPanel);

        // Bottom panel.
        var styleBottomPanel = (StyleBoxFlat)styleContentPanel.Duplicate();
        styleBottomPanel.SetCornerRadiusAll((int)(cornerRadius * EditorScale.Value));
        theme.SetStylebox("BottomPanel", "EditorStyles", styleBottomPanel);

        // TabContainerOdd can be used on tabs against the base color background (e.g. nested tabs).
        theme.SetTypeVariation("TabContainerOdd", "TabContainer");

        var styleTabSelectedOdd = (StyleBoxFlat)styleTabSelected.Duplicate();
        styleTabSelectedOdd.BgColor = disabledBgColor;
        theme.SetStylebox("tab_selected", "TabContainerOdd", styleTabSelectedOdd);

        var styleContentPanelOdd = (StyleBoxFlat)styleContentPanel.Duplicate();
        styleContentPanelOdd.BgColor = disabledBgColor;
        theme.SetStylebox("panel", "TabContainerOdd", styleContentPanelOdd);

        // This stylebox is used in 3d and 2d viewports (no borders).
        var styleContentPanelVp = (StyleBoxFlat)styleContentPanel.Duplicate();
        styleContentPanelVp.SetContentMarginIndividual(borderWidth * 2, defaultMarginSize * EditorScale.Value, borderWidth * 2, borderWidth * 2);
        theme.SetStylebox("Content", "EditorStyles", styleContentPanelVp);

        // This stylebox is used by preview tabs in the Theme Editor.
        var styleThemePreviewTab = (StyleBoxFlat)styleTabSelectedOdd.Duplicate();
        styleThemePreviewTab.SetExpandMargin(Side.SIDE_BOTTOM, 5 * EditorScale.Value);
        theme.SetStylebox("ThemeEditorPreviewFG", "EditorStyles", styleThemePreviewTab);
        var styleThemePreviewBgTab = (StyleBoxFlat)styleTabUnselected.Duplicate();
        styleThemePreviewBgTab.SetExpandMargin(Side.SIDE_BOTTOM, 2 * EditorScale.Value);
        theme.SetStylebox("ThemeEditorPreviewBG", "EditorStyles", styleThemePreviewBgTab);

        // Separators
        theme.SetStylebox("separator", "HSeparator", MakeLineStylebox(separatorColor, (int)Math.Max(Math.Round(EditorScale.Value), borderWidth)));
        theme.SetStylebox("separator", "VSeparator", MakeLineStylebox(separatorColor, (int)Math.Max(Math.Round(EditorScale.Value), borderWidth), 0, 0, true));

        // Debugger

        var stylePanelDebugger = (StyleBoxFlat)styleContentPanel.Duplicate();
        stylePanelDebugger.SetBorderWidth(Side.SIDE_BOTTOM, 0);
        theme.SetStylebox("DebuggerPanel", "EditorStyles", stylePanelDebugger);

        var stylePanelInvisibleTop = (StyleBoxFlat)styleContentPanel.Duplicate();
        var styleboxOffset = (int)(theme.GetFont("tab_selected", "TabContainer")!.GetHeight(theme.GetFontSize("tab_selected", "TabContainer")) + theme.GetStyleBox("tab_selected", "TabContainer")!.MinimumSize.Y + theme.GetStyleBox("panel", "TabContainer")!.GetContentMargin(Side.SIDE_TOP));
        stylePanelInvisibleTop.SetExpandMargin(Side.SIDE_TOP, -styleboxOffset);
        stylePanelInvisibleTop.SetContentMargin(Side.SIDE_TOP, 0);
        theme.SetStylebox("BottomPanelDebuggerOverride", "EditorStyles", stylePanelInvisibleTop);

        // LineEdit

        var styleLineEdit = (StyleBoxFlat)styleWidget.Duplicate();
        // The original style_widget style has an extra 1 pixel offset that makes LineEdits not align with Buttons,
        // so this compensates for that.
        styleLineEdit.SetContentMargin(Side.SIDE_TOP, styleLineEdit.GetContentMargin(Side.SIDE_TOP) - 1 * EditorScale.Value);

        // Don't round the bottom corner to make the line look sharper.
        styleTabSelected.SetCornerRadius(Corner.CORNER_BOTTOM_LEFT, 0);
        styleTabSelected.SetCornerRadius(Corner.CORNER_BOTTOM_RIGHT, 0);

        if (drawExtraBorders)
        {
            styleLineEdit.SetBorderWidthAll((int)Math.Round(EditorScale.Value));
            styleLineEdit.BorderColor = extraBorderColor1;
        }
        else
        {
            // Add a bottom line to make LineEdits more visible, especially in sectioned inspectors
            // such as the Project Settings.
            styleLineEdit.SetBorderWidth(Side.SIDE_BOTTOM, (int)Math.Round(2 * EditorScale.Value));
            styleLineEdit.BorderColor = darkColor2;
        }

        var styleLineEditDisabled = (StyleBoxFlat)styleLineEdit.Duplicate();
        styleLineEditDisabled.BorderColor = disabledColor;
        styleLineEditDisabled.BgColor = disabledBgColor;

        theme.SetStylebox("normal",    "LineEdit", styleLineEdit);
        theme.SetStylebox("focus",     "LineEdit", styleWidgetFocus);
        theme.SetStylebox("read_only", "LineEdit", styleLineEditDisabled);

        theme.SetIcon("clear", "LineEdit", theme.GetIcon("GuiClose", "EditorIcons"));

        theme.SetColor("font_color",                 "LineEdit", fontColor);
        theme.SetColor("font_selected_color",        "LineEdit", monoColor);
        theme.SetColor("font_uneditable_color",      "LineEdit", fontReadonlyColor);
        theme.SetColor("font_placeholder_color",     "LineEdit", fontPlaceholderColor);
        theme.SetColor("font_outline_color",         "LineEdit", fontOutlineColor);
        theme.SetColor("caret_color",                "LineEdit", fontColor);
        theme.SetColor("selection_color",            "LineEdit", selectionColor);
        theme.SetColor("clear_button_color",         "LineEdit", fontColor);
        theme.SetColor("clear_button_color_pressed", "LineEdit", accentColor);

        theme.SetConstant("outline_size", "LineEdit", 0);

        // TextEdit
        theme.SetStylebox("normal",    "TextEdit", styleLineEdit);
        theme.SetStylebox("focus",     "TextEdit", styleWidgetFocus);
        theme.SetStylebox("read_only", "TextEdit", styleLineEditDisabled);

        theme.SetIcon("tab",   "TextEdit", theme.GetIcon("GuiTab", "EditorIcons"));
        theme.SetIcon("space", "TextEdit", theme.GetIcon("GuiSpace", "EditorIcons"));

        theme.SetColor("font_color",             "TextEdit", fontColor);
        theme.SetColor("font_readonly_color",    "TextEdit", fontReadonlyColor);
        theme.SetColor("font_placeholder_color", "TextEdit", fontPlaceholderColor);
        theme.SetColor("font_outline_color",     "TextEdit", fontOutlineColor);
        theme.SetColor("caret_color",            "TextEdit", fontColor);
        theme.SetColor("selection_color",        "TextEdit", selectionColor);

        theme.SetConstant("line_spacing", "TextEdit", 4 * (int)EditorScale.Value);
        theme.SetConstant("outline_size", "TextEdit", 0);

        theme.SetIcon("h_grabber", "SplitContainer", theme.GetIcon("GuiHsplitter", "EditorIcons"));
        theme.SetIcon("v_grabber", "SplitContainer", theme.GetIcon("GuiVsplitter", "EditorIcons"));
        theme.SetIcon("grabber",   "VSplitContainer", theme.GetIcon("GuiVsplitter", "EditorIcons"));
        theme.SetIcon("grabber",   "HSplitContainer", theme.GetIcon("GuiHsplitter", "EditorIcons"));

        theme.SetConstant("separation", "SplitContainer",  (int)(defaultMarginSize * 2 * EditorScale.Value));
        theme.SetConstant("separation", "HSplitContainer", (int)(defaultMarginSize * 2 * EditorScale.Value));
        theme.SetConstant("separation", "VSplitContainer", (int)(defaultMarginSize * 2 * EditorScale.Value));

        theme.SetConstant("minimum_grab_thickness", "SplitContainer",  (int)(6 * EditorScale.Value));
        theme.SetConstant("minimum_grab_thickness", "HSplitContainer", (int)(6 * EditorScale.Value));
        theme.SetConstant("minimum_grab_thickness", "VSplitContainer", (int)(6 * EditorScale.Value));

        // Containers
        theme.SetConstant("separation",    "BoxContainer",    (int)(defaultMarginSize * EditorScale.Value));
        theme.SetConstant("separation",    "HBoxContainer",   (int)(defaultMarginSize * EditorScale.Value));
        theme.SetConstant("separation",    "VBoxContainer",   (int)(defaultMarginSize * EditorScale.Value));
        theme.SetConstant("margin_left",   "MarginContainer", 0);
        theme.SetConstant("margin_top",    "MarginContainer", 0);
        theme.SetConstant("margin_right",  "MarginContainer", 0);
        theme.SetConstant("margin_bottom", "MarginContainer", 0);
        theme.SetConstant("h_separation",  "GridContainer",   (int)(defaultMarginSize * EditorScale.Value));
        theme.SetConstant("v_separation",  "GridContainer",   (int)(defaultMarginSize * EditorScale.Value));
        theme.SetConstant("h_separation",  "FlowContainer",   (int)(defaultMarginSize * EditorScale.Value));
        theme.SetConstant("v_separation",  "FlowContainer",   (int)(defaultMarginSize * EditorScale.Value));
        theme.SetConstant("h_separation",  "HFlowContainer",  (int)(defaultMarginSize * EditorScale.Value));
        theme.SetConstant("v_separation",  "HFlowContainer",  (int)(defaultMarginSize * EditorScale.Value));
        theme.SetConstant("h_separation",  "VFlowContainer",  (int)(defaultMarginSize * EditorScale.Value));
        theme.SetConstant("v_separation",  "VFlowContainer",  (int)(defaultMarginSize * EditorScale.Value));

        // Custom theme type for MarginContainer with 4px margins.
        theme.SetTypeVariation("MarginContainer4px", "MarginContainer");

        theme.SetConstant("margin_left",   "MarginContainer4px", 4 * (int)EditorScale.Value);
        theme.SetConstant("margin_top",    "MarginContainer4px", 4 * (int)EditorScale.Value);
        theme.SetConstant("margin_right",  "MarginContainer4px", 4 * (int)EditorScale.Value);
        theme.SetConstant("margin_bottom", "MarginContainer4px", 4 * (int)EditorScale.Value);

        // Window

        // Prevent corner artifacts between window title and body.
        var styleWindowTitle = (StyleBoxFlat)styleDefault.Duplicate();
        styleWindowTitle.SetCornerRadius(Corner.CORNER_TOP_LEFT, 0);
        styleWindowTitle.SetCornerRadius(Corner.CORNER_TOP_RIGHT, 0);
        // Prevent visible line between window title and body.
        styleWindowTitle.SetExpandMargin(Side.SIDE_BOTTOM, 2 * EditorScale.Value);

        var styleWindow = (StyleBoxFlat)stylePopup.Duplicate();
        styleWindow.BorderColor = baseColor;
        styleWindow.SetBorderWidth(Side.SIDE_TOP, 24 * (int)EditorScale.Value);
        styleWindow.SetExpandMargin(Side.SIDE_TOP, 24 * (int)EditorScale.Value);
        theme.SetStylebox("embedded_border", "Window", styleWindow);

        theme.SetColor("title_color", "Window", fontColor);
        theme.SetIcon("close", "Window", theme.GetIcon("GuiClose", "EditorIcons"));
        theme.SetIcon("close_pressed", "Window", theme.GetIcon("GuiClose", "EditorIcons"));
        theme.SetConstant("close_h_offset", "Window", 22 * (int)EditorScale.Value);
        theme.SetConstant("close_v_offset", "Window", 20);
        theme.SetConstant("title_height", "Window", 24 * (int)EditorScale.Value);
        theme.SetConstant("resize_margin", "Window", 4 * (int)EditorScale.Value);
        theme.SetFont("title_font", "Window", theme.GetFont("title", "EditorFonts"));
        theme.SetFontSize("title_font_size", "Window", theme.GetFontSize("title_size", "EditorFonts"));

        // Complex window (currently only Editor Settings and Project Settings)
        var styleComplexWindow = (StyleBoxFlat)styleWindow.Duplicate();
        styleComplexWindow.BgColor = darkColor2;
        styleComplexWindow.BorderColor = darkColor2;
        theme.SetStylebox("panel", "EditorSettingsDialog", styleComplexWindow);
        theme.SetStylebox("panel", "ProjectSettingsEditor", styleComplexWindow);
        theme.SetStylebox("panel", "EditorAbout", styleComplexWindow);

        // AcceptDialog
        theme.SetStylebox("panel", "AcceptDialog", styleWindowTitle);
        theme.SetConstant("buttons_separation", "AcceptDialog", 8 * (int)EditorScale.Value);

        // HScrollBar
        var emptyIcon = new ImageTexture();

        if (increaseScrollbarTouchArea)
        {
            theme.SetStylebox("scroll", "HScrollBar", MakeLineStylebox(separatorColor, 50));
        }
        else
        {
            theme.SetStylebox("scroll", "HScrollBar", MakeStylebox(theme.GetIcon("GuiScrollBg", "EditorIcons")!, 5, 5, 5, 5, 1, 1, 1, 1));
        }
        theme.SetStylebox("scroll_focus",      "HScrollBar", MakeStylebox(theme.GetIcon("GuiScrollBg", "EditorIcons")!, 5, 5, 5, 5, 1, 1, 1, 1));
        theme.SetStylebox("grabber",           "HScrollBar", MakeStylebox(theme.GetIcon("GuiScrollGrabber", "EditorIcons")!, 6, 6, 6, 6, 1, 1, 1, 1));
        theme.SetStylebox("grabber_highlight", "HScrollBar", MakeStylebox(theme.GetIcon("GuiScrollGrabberHl", "EditorIcons")!, 5, 5, 5, 5, 1, 1, 1, 1));
        theme.SetStylebox("grabber_pressed",   "HScrollBar", MakeStylebox(theme.GetIcon("GuiScrollGrabberPressed", "EditorIcons")!, 6, 6, 6, 6, 1, 1, 1, 1));

        theme.SetIcon("increment",           "HScrollBar", emptyIcon);
        theme.SetIcon("increment_highlight", "HScrollBar", emptyIcon);
        theme.SetIcon("increment_pressed",   "HScrollBar", emptyIcon);
        theme.SetIcon("decrement",           "HScrollBar", emptyIcon);
        theme.SetIcon("decrement_highlight", "HScrollBar", emptyIcon);
        theme.SetIcon("decrement_pressed",   "HScrollBar", emptyIcon);

        // VScrollBar
        if (increaseScrollbarTouchArea)
        {
            theme.SetStylebox("scroll", "VScrollBar", MakeLineStylebox(separatorColor, 50, 1, 1, true));
        }
        else
        {
            theme.SetStylebox("scroll", "VScrollBar", MakeStylebox(theme.GetIcon("GuiScrollBg", "EditorIcons")!, 5, 5, 5, 5, 1, 1, 1, 1));
        }
        theme.SetStylebox("scroll_focus",      "VScrollBar", MakeStylebox(theme.GetIcon("GuiScrollBg", "EditorIcons")!, 5, 5, 5, 5, 1, 1, 1, 1));
        theme.SetStylebox("grabber",           "VScrollBar", MakeStylebox(theme.GetIcon("GuiScrollGrabber", "EditorIcons")!, 6, 6, 6, 6, 1, 1, 1, 1));
        theme.SetStylebox("grabber_highlight", "VScrollBar", MakeStylebox(theme.GetIcon("GuiScrollGrabberHl", "EditorIcons")!, 5, 5, 5, 5, 1, 1, 1, 1));
        theme.SetStylebox("grabber_pressed",   "VScrollBar", MakeStylebox(theme.GetIcon("GuiScrollGrabberPressed", "EditorIcons")!, 6, 6, 6, 6, 1, 1, 1, 1));

        theme.SetIcon("increment", "VScrollBar", emptyIcon);
        theme.SetIcon("increment_highlight", "VScrollBar", emptyIcon);
        theme.SetIcon("increment_pressed", "VScrollBar", emptyIcon);
        theme.SetIcon("decrement", "VScrollBar", emptyIcon);
        theme.SetIcon("decrement_highlight", "VScrollBar", emptyIcon);
        theme.SetIcon("decrement_pressed", "VScrollBar", emptyIcon);

        // HSlider
        theme.SetIcon("grabber_highlight", "HSlider", theme.GetIcon("GuiSliderGrabberHl", "EditorIcons")!);
        theme.SetIcon("grabber",           "HSlider", theme.GetIcon("GuiSliderGrabber", "EditorIcons")!);

        theme.SetStylebox("slider",                 "HSlider", MakeFlatStylebox(darkColor3, 0, defaultMarginSize / 2, 0, defaultMarginSize / 2, cornerWidth));
        theme.SetStylebox("grabber_area",           "HSlider", MakeFlatStylebox(contrastColor1, 0, defaultMarginSize / 2, 0, defaultMarginSize / 2, cornerWidth));
        theme.SetStylebox("grabber_area_highlight", "HSlider", MakeFlatStylebox(contrastColor1, 0, defaultMarginSize / 2, 0, defaultMarginSize / 2));
        theme.SetConstant("grabber_offset",         "HSlider", 0);

        // VSlider
        theme.SetIcon("grabber",           "VSlider", theme.GetIcon("GuiSliderGrabber", "EditorIcons")!);
        theme.SetIcon("grabber_highlight", "VSlider", theme.GetIcon("GuiSliderGrabberHl", "EditorIcons")!);

        theme.SetStylebox("slider",                 "VSlider", MakeFlatStylebox(darkColor3, defaultMarginSize / 2, 0, defaultMarginSize / 2, 0, cornerWidth));
        theme.SetStylebox("grabber_area",           "VSlider", MakeFlatStylebox(contrastColor1, defaultMarginSize / 2, 0, defaultMarginSize / 2, 0, cornerWidth));
        theme.SetStylebox("grabber_area_highlight", "VSlider", MakeFlatStylebox(contrastColor1, defaultMarginSize / 2, 0, defaultMarginSize / 2, 0));
        theme.SetConstant("grabber_offset",         "VSlider", 0);

        // RichTextLabel
        theme.SetColor("default_color",      "RichTextLabel", fontColor);
        theme.SetColor("font_shadow_color",  "RichTextLabel", new Color(0, 0, 0, 0));
        theme.SetColor("font_outline_color", "RichTextLabel", fontOutlineColor);

        theme.SetConstant("shadow_offset_x",     "RichTextLabel", 1 * (int)EditorScale.Value);
        theme.SetConstant("shadow_offset_y",     "RichTextLabel", 1 * (int)EditorScale.Value);
        theme.SetConstant("shadow_outline_size", "RichTextLabel", 1 * (int)EditorScale.Value);
        theme.SetConstant("outline_size",        "RichTextLabel", 0);

        theme.SetStylebox("focus", "RichTextLabel",  MakeEmptyStylebox());
        theme.SetStylebox("normal", "RichTextLabel", styleTreeBg);

        // Editor help.
        var styleEditorHelp = (StyleBoxFlat)styleDefault.Duplicate();
        styleEditorHelp.BgColor = darkColor2;
        styleEditorHelp.BorderColor = darkColor3;
        theme.SetStylebox("background", "EditorHelp", styleEditorHelp);

        theme.SetColor("title_color",     "EditorHelp", accentColor);
        theme.SetColor("headline_color",  "EditorHelp", monoColor);
        theme.SetColor("text_color",      "EditorHelp", fontColor);
        theme.SetColor("comment_color",   "EditorHelp", fontColor * new Color(1, 1, 1, 0.6f));
        theme.SetColor("symbol_color",    "EditorHelp", fontColor * new Color(1, 1, 1, 0.6f));
        theme.SetColor("value_color",     "EditorHelp", fontColor * new Color(1, 1, 1, 0.6f));
        theme.SetColor("qualifier_color", "EditorHelp", fontColor * new Color(1, 1, 1, 0.8f));
        theme.SetColor("type_color",      "EditorHelp", accentColor.Lerp(fontColor, 0.5f));
        theme.SetColor("selection_color", "EditorHelp", accentColor * new Color(1, 1, 1, 0.4f));
        theme.SetColor("link_color",      "EditorHelp", accentColor.Lerp(monoColor, 0.8f));
        theme.SetColor("code_color",      "EditorHelp", accentColor.Lerp(monoColor, 0.6f));
        theme.SetColor("kbd_color",       "EditorHelp", accentColor.Lerp(propertyColor, 0.6f));
        theme.SetColor("code_bg_color",   "EditorHelp", darkColor3);
        theme.SetColor("kbd_bg_color",    "EditorHelp", darkColor1);
        theme.SetColor("param_bg_color",  "EditorHelp", darkColor1);

        theme.SetConstant("line_separation",          "EditorHelp", (int)Math.Round(6 * EditorScale.Value));
        theme.SetConstant("table_h_separation",       "EditorHelp", 16 * (int)EditorScale.Value);
        theme.SetConstant("table_v_separation",       "EditorHelp", 6 * (int)EditorScale.Value);
        theme.SetConstant("text_highlight_h_padding", "EditorHelp", 1 * (int)EditorScale.Value);
        theme.SetConstant("text_highlight_v_padding", "EditorHelp", 2 * (int)EditorScale.Value);

        // Panel
        theme.SetStylebox("panel", "Panel", MakeFlatStylebox(darkColor1, 6, 4, 6, 4, cornerWidth));
        theme.SetStylebox("PanelForeground", "EditorStyles", styleDefault);

        // Label
        theme.SetStylebox("normal", "Label", styleEmpty);

        theme.SetColor("font_color",         "Label", fontColor);
        theme.SetColor("font_shadow_color",  "Label", new Color(0, 0, 0, 0));
        theme.SetColor("font_outline_color", "Label", fontOutlineColor);

        theme.SetConstant("shadow_offset_x",     "Label", 1 * (int)EditorScale.Value);
        theme.SetConstant("shadow_offset_y",     "Label", 1 * (int)EditorScale.Value);
        theme.SetConstant("shadow_outline_size", "Label", 1 * (int)EditorScale.Value);
        theme.SetConstant("line_spacing",        "Label", 3 * (int)EditorScale.Value);
        theme.SetConstant("outline_size",        "Label", 0);

        // LinkButton
        theme.SetStylebox("focus", "LinkButton", styleEmpty);

        theme.SetColor("font_color",               "LinkButton", fontColor);
        theme.SetColor("font_hover_color",         "LinkButton", fontHoverColor);
        theme.SetColor("font_hover_pressed_color", "LinkButton", fontHoverPressedColor);
        theme.SetColor("font_focus_color",         "LinkButton", fontFocusColor);
        theme.SetColor("font_pressed_color",       "LinkButton", accentColor);
        theme.SetColor("font_disabled_color",      "LinkButton", fontDisabledColor);
        theme.SetColor("font_outline_color",       "LinkButton", fontOutlineColor);

        theme.SetConstant("outline_size", "LinkButton", 0);

        // TooltipPanel + TooltipLabel
        // TooltipPanel is also used for custom tooltips, while TooltipLabel
        // is only relevant for default tooltips.
        var styleTooltip = (StyleBoxFlat)stylePopup.Duplicate();
        styleTooltip.ShadowSize = 0;
        styleTooltip.SetContentMarginAll(defaultMarginSize * EditorScale.Value * 0.5f);
        styleTooltip.BgColor = darkColor3 * new Color(0.8f, 0.8f, 0.8f, 0.9f);
        styleTooltip.SetBorderWidthAll(0);

        theme.SetColor("font_color", "TooltipLabel", fontHoverColor);
        theme.SetColor("font_shadow_color", "TooltipLabel", new Color(0, 0, 0, 0));
        theme.SetStylebox("panel", "TooltipPanel", styleTooltip);

        // PopupPanel
        theme.SetStylebox("panel", "PopupPanel", stylePopup);

        var controlEditorPopupStyle = (StyleBoxFlat)stylePopup.Duplicate();
        controlEditorPopupStyle.ShadowSize = 0;
        controlEditorPopupStyle.SetContentMargin(Side.SIDE_LEFT,   defaultMarginSize * EditorScale.Value);
        controlEditorPopupStyle.SetContentMargin(Side.SIDE_TOP,    defaultMarginSize * EditorScale.Value);
        controlEditorPopupStyle.SetContentMargin(Side.SIDE_RIGHT,  defaultMarginSize * EditorScale.Value);
        controlEditorPopupStyle.SetContentMargin(Side.SIDE_BOTTOM, defaultMarginSize * EditorScale.Value);
        controlEditorPopupStyle.SetBorderWidthAll(0);

        theme.SetStylebox("panel", "ControlEditorPopupPanel", controlEditorPopupStyle);
        theme.SetTypeVariation("ControlEditorPopupPanel", "PopupPanel");

        // SpinBox
        theme.SetIcon("updown",          "SpinBox", theme.GetIcon("GuiSpinboxUpdown", "EditorIcons")!);
        theme.SetIcon("updown_disabled", "SpinBox", theme.GetIcon("GuiSpinboxUpdownDisabled", "EditorIcons")!);

        // ProgressBar
        theme.SetStylebox("background", "ProgressBar", MakeStylebox(theme.GetIcon("GuiProgressBar", "EditorIcons")!, 4, 4, 4, 4, 0, 0, 0, 0));
        theme.SetStylebox("fill",       "ProgressBar", MakeStylebox(theme.GetIcon("GuiProgressFill", "EditorIcons")!, 6, 6, 6, 6, 2, 1, 2, 1));

        theme.SetColor("font_color",         "ProgressBar", fontColor);
        theme.SetColor("font_outline_color", "ProgressBar", fontOutlineColor);

        theme.SetConstant("outline_size", "ProgressBar", 0);

        // GraphEdit
        theme.SetStylebox("bg", "GraphEdit", styleTreeBg);
        if (darkTheme)
        {
            theme.SetColor("grid_major", "GraphEdit", new Color(1, 1, 1, 0.15f));
            theme.SetColor("grid_minor", "GraphEdit", new Color(1, 1, 1, 0.07f));
        }
        else
        {
            theme.SetColor("grid_major", "GraphEdit", new Color(0.0f, 0.0f, 0.0f, 0.15f));
            theme.SetColor("grid_minor", "GraphEdit", new Color(0.0f, 0.0f, 0.0f, 0.07f));
        }
        theme.SetColor("selection_fill",   "GraphEdit", theme.GetColor("box_selection_fill_color", "Editor")!);
        theme.SetColor("selection_stroke", "GraphEdit", theme.GetColor("box_selection_stroke_color", "Editor")!);
        theme.SetColor("activity",         "GraphEdit", accentColor);

        theme.SetIcon("minus",   "GraphEdit", theme.GetIcon("ZoomLess", "EditorIcons")!);
        theme.SetIcon("more",    "GraphEdit", theme.GetIcon("ZoomMore", "EditorIcons")!);
        theme.SetIcon("reset",   "GraphEdit", theme.GetIcon("ZoomReset", "EditorIcons")!);
        theme.SetIcon("snap",    "GraphEdit", theme.GetIcon("SnapGrid", "EditorIcons")!);
        theme.SetIcon("minimap", "GraphEdit", theme.GetIcon("GridMinimap", "EditorIcons")!);
        theme.SetIcon("layout",  "GraphEdit", theme.GetIcon("GridLayout", "EditorIcons")!);

        // GraphEditMinimap
        var styleMinimapBg = MakeFlatStylebox(darkColor1, 0, 0, 0, 0);
        styleMinimapBg.BorderColor = darkColor3;
        styleMinimapBg.SetBorderWidthAll(1);
        theme.SetStylebox("bg", "GraphEditMinimap", styleMinimapBg);

        StyleBoxFlat style_minimap_camera;
        StyleBoxFlat style_minimap_node;

        if (darkTheme)
        {
            style_minimap_camera = MakeFlatStylebox(new Color(0.65f, 0.65f, 0.65f, 0.2f), 0, 0, 0, 0);
            style_minimap_camera.BorderColor = new Color(0.65f, 0.65f, 0.65f, 0.45f);
            style_minimap_node = MakeFlatStylebox(new Color(1, 1, 1), 0, 0, 0, 0);
        }
        else
        {
            style_minimap_camera = MakeFlatStylebox(new Color(0.38f, 0.38f, 0.38f, 0.2f), 0, 0, 0, 0);
            style_minimap_camera.BorderColor = new Color(0.38f, 0.38f, 0.38f, 0.45f);
            style_minimap_node = MakeFlatStylebox(new Color(0, 0, 0), 0, 0, 0, 0);
        }
        style_minimap_camera.SetBorderWidthAll(1);
        theme.SetStylebox("camera", "GraphEditMinimap", style_minimap_camera);
        theme.SetStylebox("node",   "GraphEditMinimap", style_minimap_node);

        var minimapResizerColor = darkTheme ? new Color(1, 1, 1, 0.65f) : new Color(0, 0, 0, 0.65f);
        theme.SetIcon("resizer",        "GraphEditMinimap", theme.GetIcon("GuiResizerTopLeft", "EditorIcons"));
        theme.SetColor("resizer_color", "GraphEditMinimap", minimapResizerColor);

        // GraphNode
        var gnMarginSide   = 2;
        var gnMarginBottom = 2;

        // StateMachine
        var smMarginSide = 10;

        var graphnodeBg = darkColor3;
        if (!darkTheme)
        {
            graphnodeBg = propSectionColor;
        }

        var graphsb = MakeFlatStylebox(graphnodeBg.Lerp(styleTreeBg.BgColor, 0.3f), gnMarginSide, 24, gnMarginSide, gnMarginBottom, cornerWidth);
        graphsb.SetBorderWidthAll((int)borderWidth);
        graphsb.BorderColor = graphnodeBg;

        var graphsbselected = MakeFlatStylebox(graphnodeBg * new Color(1, 1, 1, 1), gnMarginSide, 24, gnMarginSide, gnMarginBottom, cornerWidth);
        graphsbselected.SetBorderWidthAll((int)(2 * EditorScale.Value + borderWidth));
        graphsbselected.BorderColor = new Color(accentColor.R, accentColor.G, accentColor.B, 0.6f);

        var graphsbcomment = MakeFlatStylebox(graphnodeBg * new Color(1, 1, 1, 0.3f), gnMarginSide, 24, gnMarginSide, gnMarginBottom, cornerWidth);
        graphsbcomment.SetBorderWidthAll((int)borderWidth);
        graphsbcomment.BorderColor = graphnodeBg;

        var graphsbcommentselected = MakeFlatStylebox(graphnodeBg * new Color(1, 1, 1, 0.4f), gnMarginSide, 24, gnMarginSide, gnMarginBottom, cornerWidth);
        graphsbcommentselected.SetBorderWidthAll((int)borderWidth);
        graphsbcommentselected.BorderColor = graphnodeBg;

        var graphsbbreakpoint = (StyleBoxFlat)graphsbselected.Duplicate();
        graphsbbreakpoint.DrawCenter = false;
        graphsbbreakpoint.BorderColor = warningColor;
        graphsbbreakpoint.ShadowColor = warningColor * new Color(1, 1, 1, 0.1f);

        var graphsbposition = (StyleBoxFlat)graphsbselected.Duplicate();
        graphsbposition.DrawCenter = false;
        graphsbposition.BorderColor = errorColor;
        graphsbposition.ShadowColor = errorColor * new Color(1, 1, 1, 0.2f);

        var graphsbslot = MakeEmptyStylebox(12, 0, 12, 0);

        var smgraphsb = MakeFlatStylebox(darkColor3 * new Color(1, 1, 1, 0.7f), smMarginSide, 24, smMarginSide, gnMarginBottom, cornerWidth);
        smgraphsb.SetBorderWidthAll((int)borderWidth);
        smgraphsb.BorderColor = graphnodeBg;

        var smgraphsbselected = MakeFlatStylebox(graphnodeBg * new Color(1, 1, 1, 0.9f), smMarginSide, 24, smMarginSide, gnMarginBottom, cornerWidth);
        smgraphsbselected.SetBorderWidthAll((int)(2 * EditorScale.Value + borderWidth));
        smgraphsbselected.BorderColor = new Color(accentColor.R, accentColor.G, accentColor.B, 0.9f);
        smgraphsbselected.ShadowSize = 8 * (int)EditorScale.Value;
        smgraphsbselected.ShadowColor = shadowColor;

        graphsb.SetBorderWidth(Side.SIDE_TOP, (int)(24 * EditorScale.Value));
        graphsbselected.SetBorderWidth(Side.SIDE_TOP, (int)(24 * EditorScale.Value));
        graphsbcomment.SetBorderWidth(Side.SIDE_TOP, (int)(24 * EditorScale.Value));
        graphsbcommentselected.SetBorderWidth(Side.SIDE_TOP, (int)(24 * EditorScale.Value));

        graphsb.CornerDetail = (int)(cornerRadius * EditorScale.Value);
        graphsbselected.CornerDetail = (int)(cornerRadius * EditorScale.Value);
        graphsbcomment.CornerDetail         = (int)(cornerRadius * EditorScale.Value);
        graphsbcommentselected.CornerDetail = (int)(cornerRadius * EditorScale.Value);

        theme.SetStylebox("frame",                        "GraphNode", graphsb);
        theme.SetStylebox("selected_frame",               "GraphNode", graphsbselected);
        theme.SetStylebox("comment",                      "GraphNode", graphsbcomment);
        theme.SetStylebox("comment_focus",                "GraphNode", graphsbcommentselected);
        theme.SetStylebox("breakpoint",                   "GraphNode", graphsbbreakpoint);
        theme.SetStylebox("position",                     "GraphNode", graphsbposition);
        theme.SetStylebox("slot",                         "GraphNode", graphsbslot);
        theme.SetStylebox("state_machine_frame",          "GraphNode", smgraphsb);
        theme.SetStylebox("state_machine_selected_frame", "GraphNode", smgraphsbselected);

        var nodeDecorationColor = darkColor1.Inverted();
        theme.SetColor("title_color", "GraphNode", nodeDecorationColor);
        nodeDecorationColor.A = 0.7f;
        theme.SetColor("close_color",   "GraphNode", nodeDecorationColor);
        theme.SetColor("resizer_color", "GraphNode", nodeDecorationColor);

        theme.SetConstant("port_offset",    "GraphNode", 0);
        theme.SetConstant("title_h_offset", "GraphNode", 12 * (int)EditorScale.Value);
        theme.SetConstant("title_offset",   "GraphNode", 21 * (int)EditorScale.Value);
        theme.SetConstant("close_h_offset", "GraphNode", -2 * (int)EditorScale.Value);
        theme.SetConstant("close_offset",   "GraphNode", 20);
        theme.SetConstant("separation",     "GraphNode", 1 * (int)EditorScale.Value);

        theme.SetIcon("close",   "GraphNode", theme.GetIcon("GuiCloseCustomizable", "EditorIcons"));
        theme.SetIcon("resizer", "GraphNode", theme.GetIcon("GuiResizer",           "EditorIcons"));
        theme.SetIcon("port",    "GraphNode", theme.GetIcon("GuiGraphNodePort",     "EditorIcons"));

        theme.SetFont("title_font", "GraphNode", theme.GetFont("main_bold_msdf", "EditorFonts"));

        // GridContainer
        theme.SetConstant("v_separation", "GridContainer", (int)Math.Round(widgetDefaultMargin.Y - 2 * EditorScale.Value));

        // FileDialog
        theme.SetIcon("folder",         "FileDialog", theme.GetIcon("Folder", "EditorIcons"));
        theme.SetIcon("parent_folder",  "FileDialog", theme.GetIcon("ArrowUp", "EditorIcons"));
        theme.SetIcon("back_folder",    "FileDialog", theme.GetIcon("Back", "EditorIcons"));
        theme.SetIcon("forward_folder", "FileDialog", theme.GetIcon("Forward", "EditorIcons"));
        theme.SetIcon("reload",         "FileDialog", theme.GetIcon("Reload", "EditorIcons"));
        theme.SetIcon("toggle_hidden",  "FileDialog", theme.GetIcon("GuiVisibilityVisible", "EditorIcons"));
        // Use a different color for folder icons to make them easier to distinguish from files.
        // On a light theme, the icon will be dark, so we need to lighten it before blending it with the accent color.
        theme.SetColor("folder_icon_color", "FileDialog", (darkTheme ? new Color(1, 1, 1) : new Color(4.25f, 4.25f, 4.25f)).Lerp(accentColor, 0.7f));
        theme.SetColor("files_disabled",    "FileDialog", fontDisabledColor);

        // ColorPicker
        theme.SetConstant("margin",      "ColorPicker", (int)popupMarginSize);
        theme.SetConstant("sv_width",    "ColorPicker", 256 * (int)EditorScale.Value);
        theme.SetConstant("sv_height",   "ColorPicker", 256 * (int)EditorScale.Value);
        theme.SetConstant("h_width",     "ColorPicker", 30);
        theme.SetConstant("label_width", "ColorPicker", 10);

        theme.SetIcon("screen_picker",        "ColorPicker", theme.GetIcon("ColorPick", "EditorIcons"));
        theme.SetIcon("shape_circle",         "ColorPicker", theme.GetIcon("PickerShapeCircle", "EditorIcons"));
        theme.SetIcon("shape_rect",           "ColorPicker", theme.GetIcon("PickerShapeRectangle", "EditorIcons"));
        theme.SetIcon("shape_rect_wheel",     "ColorPicker", theme.GetIcon("PickerShapeRectangleWheel", "EditorIcons"));
        theme.SetIcon("add_preset",           "ColorPicker", theme.GetIcon("Add", "EditorIcons"));
        theme.SetIcon("sample_bg",            "ColorPicker", theme.GetIcon("GuiMiniCheckerboard", "EditorIcons"));
        theme.SetIcon("overbright_indicator", "ColorPicker", theme.GetIcon("OverbrightIndicator", "EditorIcons"));
        theme.SetIcon("bar_arrow",            "ColorPicker", theme.GetIcon("ColorPickerBarArrow", "EditorIcons"));
        theme.SetIcon("picker_cursor",        "ColorPicker", theme.GetIcon("PickerCursor", "EditorIcons"));

        // ColorPickerButton
        theme.SetIcon("bg", "ColorPickerButton", theme.GetIcon("GuiMiniCheckerboard", "EditorIcons"));

        // ColorPresetButton
        var presetSb = MakeFlatStylebox(new Color(1, 1, 1), 2, 2, 2, 2, 2);
        theme.SetStylebox("preset_fg", "ColorPresetButton", presetSb);

        theme.SetIcon("preset_bg",            "ColorPresetButton", theme.GetIcon("GuiMiniCheckerboard", "EditorIcons"));
        theme.SetIcon("overbright_indicator", "ColorPresetButton", theme.GetIcon("OverbrightIndicator", "EditorIcons"));

        // Information on 3D viewport
        var styleInfo3DViewport = (StyleBoxFlat)styleDefault.Duplicate();
        styleInfo3DViewport.BgColor *= new Color(1, 1, 1, 0.5f);
        styleInfo3DViewport.SetBorderWidthAll(0);
        theme.SetStylebox("Information3dViewport", "EditorStyles", styleInfo3DViewport);

        // Asset Library.
        theme.SetStylebox("bg",    "AssetLib", styleEmpty);
        theme.SetStylebox("panel", "AssetLib", styleContentPanel);

        theme.SetColor("status_color", "AssetLib", new Color(0.5f, 0.5f, 0.5f));
        theme.SetIcon("dismiss",       "AssetLib", theme.GetIcon("Close", "EditorIcons"));

        // Theme editor.
        theme.SetColor("preview_picker_overlay_color", "ThemeEditor", new Color(0.1f, 0.1f, 0.1f, 0.25f));

        var themePreviewPickerBgColor = accentColor;
        themePreviewPickerBgColor.A = 0.2f;

        var themePreviewPickerSb = MakeFlatStylebox(themePreviewPickerBgColor, 0, 0, 0, 0);
        themePreviewPickerSb.BorderColor = accentColor;
        themePreviewPickerSb.SetBorderWidthAll((int)1.0);

        theme.SetStylebox("preview_picker_overlay", "ThemeEditor", themePreviewPickerSb);

        var themePreviewPickerLabelBgColor = accentColor;
        themePreviewPickerLabelBgColor.V = 0.5f;

        var themePreviewPickerLabelSb = MakeFlatStylebox(themePreviewPickerLabelBgColor, 4, 1, 4, 3);
        theme.SetStylebox("preview_picker_label", "ThemeEditor", themePreviewPickerLabelSb);

        // Dictionary editor add item.
        // Expand to the left and right by 4px to compensate for the dictionary editor margins.
        var styleDictionaryAddItem = MakeFlatStylebox(propSubsectionColor, 0, 4, 0, 4, cornerRadius);
        styleDictionaryAddItem.SetExpandMargin(Side.SIDE_LEFT, 4 * EditorScale.Value);
        styleDictionaryAddItem.SetExpandMargin(Side.SIDE_RIGHT, 4 * EditorScale.Value);
        theme.SetStylebox("DictionaryAddItem", "EditorStyles", styleDictionaryAddItem);

        var vshaderLabelStyle = MakeEmptyStylebox(2, 1, 2, 1);
        theme.SetStylebox("label_style", "VShaderEditor", vshaderLabelStyle);

        // adaptive script theme constants
        // for comments and elements with lower relevance
        var dimColor = new Color(fontColor.R, fontColor.G, fontColor.B, 0.5f);

        var monoValue = monoColor.R;
        var alpha1    = new Color(monoValue, monoValue, monoValue, 0.07f);
        var alpha2    = new Color(monoValue, monoValue, monoValue, 0.14f);
        var alpha3    = new Color(monoValue, monoValue, monoValue, 0.27f);

        var symbolColor             = darkTheme ? new Color(0.67f, 0.79f, 1) : new Color(0, 0, 0.61f);
        var keywordColor            = darkTheme ? new Color(1, 0.44f, 0.52f) : new Color(0.9f, 0.135f, 0.51f);
        var controlFlowKeywordColor = darkTheme ? new Color(1, 0.55f, 0.8f) : new Color(0.743f, 0.12f, 0.8f);
        var baseTypeColor           = darkTheme ? new Color(0.26f, 1, 0.76f) : new Color(0, 0.6f, 0.2f);
        var engineTypeColor         = darkTheme ? new Color(0.56f, 1, 0.86f) : new Color(0.11f, 0.55f, 0.4f);
        var userTypeColor           = darkTheme ? new Color(0.78f, 1, 0.93f) : new Color(0.18f, 0.45f, 0.4f);
        var commentColor            = darkTheme ? dimColor : new Color(0.08f, 0.08f, 0.08f, 0.5f);
        var stringColor             = darkTheme ? new Color(1, 0.93f, 0.63f) : new Color(0.6f, 0.42f, 0);

        // Use the brightest background color on a light theme (which generally uses a negative contrast rate).
        var teBackgroundColor         = darkTheme ? backgroundColor : darkColor3;
        var completionBackgroundColor = darkTheme ? baseColor : backgroundColor;
        var completionSelectedColor   = alpha1;
        var completionExistingColor   = alpha2;
        // Same opacity as the scroll grabber editor icon.
        var completionScrollColor        = new Color(monoValue, monoValue, monoValue, 0.29f);
        var completionScrollHoveredColor = new Color(monoValue, monoValue, monoValue, 0.4f);
        var completionFontColor          = fontColor;
        var textColor                    = fontColor;
        var lineNumberColor              = dimColor;
        var safeLineNumberColor          = darkTheme ? (dimColor * new Color(1, 1.2f, 1, 1.5f)) : new Color(0, 0.4f, 0, 0.75f);
        var caretColor                   = monoColor;
        var caretBackgroundColor         = monoColor.Inverted();
        var textSelectedColor            = new Color(0, 0, 0, 0);
        var braceMismatchColor           = darkTheme ? errorColor : new Color(1, 0.08f, 0, 1);
        var currentLineColor             = alpha1;
        var lineLengthGuidelineColor     = darkTheme ? baseColor : backgroundColor;
        var wordHighlightedColor         = alpha1;
        var numberColor                  = darkTheme ? new Color(0.63f, 1, 0.88f) : new Color(0, 0.55f, 0.28f, 1);
        var functionColor                = darkTheme ? new Color(0.34f, 0.7f, 1) : new Color(0, 0.225f, 0.9f, 1);
        var memberVariableColor          = darkTheme ? new Color(0.34f, 0.7f, 1).Lerp(monoColor, 0.6f) : new Color(0, 0.4f, 0.68f, 1);
        var markColor                    = new Color(errorColor.R, errorColor.G, errorColor.B, 0.3f);
        var bookmarkColor                = new Color(0.08f, 0.49f, 0.98f);
        var breakpointColor              = darkTheme ? errorColor : new Color(1, 0.27f, 0.2f, 1);
        var executingLineColor           = new Color(0.98f, 0.89f, 0.27f);
        var codeFoldingColor             = alpha3;
        var searchResultColor            = alpha1;
        var searchResultBorderColor      = darkTheme ? new Color(0.41f, 0.61f, 0.91f, 0.38f) : new Color(0, 0.4f, 1, 0.38f);

        var setting = EditorSettings.Singleton;
        var textEditorColorTheme = setting.GetSetting<string>("text_editor/theme/color_theme")!;
        if (textEditorColorTheme == "Default")
        {
            setting.SetInitialValue("text_editor/theme/highlighting/symbol_color", symbolColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/keyword_color", keywordColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/control_flow_keyword_color", controlFlowKeywordColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/base_type_color", baseTypeColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/engine_type_color", engineTypeColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/user_type_color", userTypeColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/comment_color", commentColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/string_color", stringColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/background_color", teBackgroundColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/completion_background_color", completionBackgroundColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/completion_selected_color", completionSelectedColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/completion_existing_color", completionExistingColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/completion_scroll_color", completionScrollColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/completion_scroll_hovered_color", completionScrollHoveredColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/completion_font_color", completionFontColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/text_color", textColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/line_number_color", lineNumberColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/safe_line_number_color", safeLineNumberColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/caret_color", caretColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/caret_background_color", caretBackgroundColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/text_selected_color", textSelectedColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/selection_color", selectionColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/brace_mismatch_color", braceMismatchColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/current_line_color", currentLineColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/line_length_guideline_color", lineLengthGuidelineColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/word_highlighted_color", wordHighlightedColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/number_color", numberColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/function_color", functionColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/member_variable_color", memberVariableColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/mark_color", markColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/bookmark_color", bookmarkColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/breakpoint_color", breakpointColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/executing_line_color", executingLineColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/code_folding_color", codeFoldingColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/search_result_color", searchResultColor, true);
            setting.SetInitialValue("text_editor/theme/highlighting/search_result_border_color", searchResultBorderColor, true);
        }
        else if (textEditorColorTheme == "Godot 2")
        {
            setting.LoadTextEditorTheme();
        }

        // Now theme is loaded, apply it to CodeEdit.
        theme.SetFont("font", "CodeEdit", theme.GetFont("source", "EditorFonts"));
        theme.SetFontSize("font_size", "CodeEdit", theme.GetFontSize("source_size", "EditorFonts"));

        var codeEditStylebox = MakeFlatStylebox(EDITOR_GET<Color>("text_editor/theme/highlighting/background_color"), widgetDefaultMargin.X, widgetDefaultMargin.Y, widgetDefaultMargin.X, widgetDefaultMargin.Y, cornerRadius);
        theme.SetStylebox("normal",    "CodeEdit", codeEditStylebox);
        theme.SetStylebox("read_only", "CodeEdit", codeEditStylebox);
        theme.SetStylebox("focus",     "CodeEdit", new StyleBoxEmpty());

        theme.SetIcon("tab",            "CodeEdit", theme.GetIcon("GuiTab",               "EditorIcons"));
        theme.SetIcon("space",          "CodeEdit", theme.GetIcon("GuiSpace",             "EditorIcons"));
        theme.SetIcon("folded",         "CodeEdit", theme.GetIcon("CodeFoldedRightArrow", "EditorIcons"));
        theme.SetIcon("can_fold",       "CodeEdit", theme.GetIcon("CodeFoldDownArrow",    "EditorIcons"));
        theme.SetIcon("executing_line", "CodeEdit", theme.GetIcon("TextEditorPlay",       "EditorIcons"));
        theme.SetIcon("breakpoint",     "CodeEdit", theme.GetIcon("Breakpoint",           "EditorIcons"));

        theme.SetConstant("line_spacing", "CodeEdit", EDITOR_GET<int>("text_editor/appearance/whitespace/line_spacing"));

        theme.SetColor("background_color",                "CodeEdit", new Color(0, 0, 0, 0));
        theme.SetColor("completion_background_color",     "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/completion_background_color"));
        theme.SetColor("completion_selected_color",       "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/completion_selected_color"));
        theme.SetColor("completion_existing_color",       "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/completion_existing_color"));
        theme.SetColor("completion_scroll_color",         "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/completion_scroll_color"));
        theme.SetColor("completion_scroll_hovered_color", "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/completion_scroll_hovered_color"));
        theme.SetColor("completion_font_color",           "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/completion_font_color"));
        theme.SetColor("font_color",                      "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/text_color"));
        theme.SetColor("line_number_color",               "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/line_number_color"));
        theme.SetColor("caret_color",                     "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/caret_color"));
        theme.SetColor("font_selected_color",             "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/text_selected_color"));
        theme.SetColor("selection_color",                 "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/selection_color"));
        theme.SetColor("brace_mismatch_color",            "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/brace_mismatch_color"));
        theme.SetColor("current_line_color",              "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/current_line_color"));
        theme.SetColor("line_length_guideline_color",     "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/line_length_guideline_color"));
        theme.SetColor("word_highlighted_color",          "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/word_highlighted_color"));
        theme.SetColor("bookmark_color",                  "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/bookmark_color"));
        theme.SetColor("breakpoint_color",                "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/breakpoint_color"));
        theme.SetColor("executing_line_color",            "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/executing_line_color"));
        theme.SetColor("code_folding_color",              "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/code_folding_color"));
        theme.SetColor("search_result_color",             "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/search_result_color"));
        theme.SetColor("search_result_border_color",      "CodeEdit", EDITOR_GET<Color>("text_editor/theme/highlighting/search_result_border_color"));

        return theme;
    }
}
