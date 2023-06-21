#define MODULE_SVG_ENABLED

namespace Godot.Net.Scene.Resources.DefaultTheme;

using Godot.Net.Core.Enums;
using Godot.Net.Core.Error;
using Godot.Net.Core.IO;
using Godot.Net.Core.Math;
using Godot.Net.Modules.SVG;
using Godot.Net.Scene.Theme;
using Godot.Net.Servers;

public static class DefaultTheme
{
    private const int DEFAULT_CORNER_RADIUS = 3;
    private const int DEFAULT_FONT_SIZE     = 16;
    private const int DEFAULT_MARGIN        = 4;

    private static float scale;

    public static void FillDefaultTheme(
        Theme          theme,
        Font?          defaultFont,
        Font?          boldFont,
        Font?          boldItalicsFont,
        Font?          italicsFont,
        out Texture2D  defaultIcon,
        out StyleBox   defaultStyle,
        float          scale
    )
    {
        DefaultTheme.scale = scale;

        // Default theme properties.
        theme.DefaultFont      = defaultFont;
        theme.DefaultFontSize  = (int)(DEFAULT_FONT_SIZE * scale);
        theme.DefaultBaseScale = scale;

        // Font colors
        var controlFontColor            = new Color(0.875f, 0.875f, 0.875f);
        var controlFontLowColor         = new Color(0.7f, 0.7f, 0.7f);
        var controlFontLowerColor       = new Color(0.65f, 0.65f, 0.65f);
        var controlFontHoverColor       = new Color(0.95f, 0.95f, 0.95f);
        var controlFontFocusColor       = new Color(0.95f, 0.95f, 0.95f);
        var controlFontDisabledColor    = controlFontColor * new Color(1, 1, 1, 0.5f);
        var controlFontPlaceholderColor = new Color(controlFontColor.R, controlFontColor.G, controlFontColor.B, 0.6f);
        var controlFontPressedColor     = new Color(1, 1, 1);
        var controlSelectionColor       = new Color(0.5f, 0.5f, 0.5f);

        // StyleBox colors
        var styleNormalColor      = new Color(0.1f, 0.1f, 0.1f, 0.6f);
        var styleHoverColor       = new Color(0.225f, 0.225f, 0.225f, 0.6f);
        var stylePressedColor     = new Color(0f, 0f, 0f, 0.6f);
        var styleDisabledColor    = new Color(0.1f, 0.1f, 0.1f, 0.3f);
        var styleFocusColor       = new Color(1, 1, 1, 0.75f);
        var stylePopupColor       = new Color(0.25f, 0.25f, 0.25f, 1);
        var stylePopupBorderColor = new Color(0.175f, 0.175f, 0.175f, 1);
        var stylePopupHoverColor  = new Color(0.4f, 0.4f, 0.4f, 1);
        var styleSelectedColor    = new Color(1, 1, 1, 0.3f);
        // Don't use a color too bright to keep the percentage readable.
        var styleProgressColor  = new Color(1, 1, 1, 0.4f);
        var styleSeparatorColor = new Color(0.5f, 0.5f, 0.5f);

        // Convert the generated icon sources to a dictionary for easier access.
        // Unlike the editor icons, there is no central repository of icons in the Theme resource itself to keep it tidy.
        var icons = new Dictionary<string, ImageTexture>();

        foreach (var entry in DefaultThemeIcons.Sources)
        {
            icons[entry.Key] = GenerateIcon(entry.Value);
        }

        // Panel
        theme.SetStylebox("panel", "Panel", MakeFlatStylebox(styleNormalColor, 0, 0, 0, 0));

        // Button

        var buttonNormal   = MakeFlatStylebox(styleNormalColor);
        var buttonHover    = MakeFlatStylebox(styleHoverColor);
        var buttonPressed  = MakeFlatStylebox(stylePressedColor);
        var buttonDisabled = MakeFlatStylebox(styleDisabledColor);
        var focus          = MakeFlatStylebox(styleFocusColor, DEFAULT_MARGIN, DEFAULT_MARGIN, DEFAULT_MARGIN, DEFAULT_MARGIN, DEFAULT_CORNER_RADIUS, false, 2);
        // Make the focus outline appear to be flush with the buttons it's focusing.
        focus.SetExpandMarginAll(2 * scale);

        theme.SetStylebox("normal",   "Button", buttonNormal);
        theme.SetStylebox("hover",    "Button", buttonHover);
        theme.SetStylebox("pressed",  "Button", buttonPressed);
        theme.SetStylebox("disabled", "Button", buttonDisabled);
        theme.SetStylebox("focus",    "Button", focus);

        theme.SetFont("font", "Button", null);
        theme.SetFontSize("font_size", "Button", -1);
        theme.SetConstant("outline_size", "Button", 0);

        theme.SetColor("font_color",               "Button", controlFontColor);
        theme.SetColor("font_pressed_color",       "Button", controlFontPressedColor);
        theme.SetColor("font_hover_color",         "Button", controlFontHoverColor);
        theme.SetColor("font_focus_color",         "Button", controlFontFocusColor);
        theme.SetColor("font_hover_pressed_color", "Button", controlFontPressedColor);
        theme.SetColor("font_disabled_color",      "Button", controlFontDisabledColor);
        theme.SetColor("font_outline_color",       "Button", new Color(1, 1, 1));

        theme.SetColor("icon_normal_color",        "Button", new Color(1, 1, 1, 1));
        theme.SetColor("icon_pressed_color",       "Button", new Color(1, 1, 1, 1));
        theme.SetColor("icon_hover_color",         "Button", new Color(1, 1, 1, 1));
        theme.SetColor("icon_hover_pressed_color", "Button", new Color(1, 1, 1, 1));
        theme.SetColor("icon_focus_color",         "Button", new Color(1, 1, 1, 1));
        theme.SetColor("icon_disabled_color",      "Button", new Color(1, 1, 1, 0.4f));

        theme.SetConstant("h_separation", "Button", (int)(2 * scale));

        // MenuBar
        theme.SetStylebox("normal",   "MenuBar", buttonNormal);
        theme.SetStylebox("hover",    "MenuBar", buttonHover);
        theme.SetStylebox("pressed",  "MenuBar", buttonPressed);
        theme.SetStylebox("disabled", "MenuBar", buttonDisabled);
        theme.SetStylebox("focus",    "MenuBar", focus);

        theme.SetFont("font", "MenuBar", null);
        theme.SetFontSize("font_size", "MenuBar", -1);
        theme.SetConstant("outline_size", "MenuBar", 0);

        theme.SetColor("font_color",               "MenuBar", controlFontColor);
        theme.SetColor("font_pressed_color",       "MenuBar", controlFontPressedColor);
        theme.SetColor("font_hover_color",         "MenuBar", controlFontHoverColor);
        theme.SetColor("font_focus_color",         "MenuBar", controlFontFocusColor);
        theme.SetColor("font_hover_pressed_color", "MenuBar", controlFontPressedColor);
        theme.SetColor("font_disabled_color",      "MenuBar", controlFontDisabledColor);
        theme.SetColor("font_outline_color",       "MenuBar", new Color(1, 1, 1));

        theme.SetConstant("h_separation", "MenuBar", (int)(4 * scale));

        // LinkButton

        theme.SetStylebox("focus", "LinkButton", focus);

        theme.SetFont("font", "LinkButton", null);
        theme.SetFontSize("font_size", "LinkButton", -1);

        theme.SetColor("font_color",         "LinkButton", controlFontColor);
        theme.SetColor("font_pressed_color", "LinkButton", controlFontPressedColor);
        theme.SetColor("font_hover_color",   "LinkButton", controlFontHoverColor);
        theme.SetColor("font_focus_color",   "LinkButton", controlFontFocusColor);
        theme.SetColor("font_outline_color", "LinkButton", new Color(1, 1, 1));

        theme.SetConstant("outline_size", "LinkButton", 0);
        theme.SetConstant("underline_spacing", "LinkButton", (int)(2 * scale));

        // OptionButton
        theme.SetStylebox("focus", "OptionButton", focus);

        var sbOptbuttonNormal   = MakeFlatStylebox(styleNormalColor, 2 * DEFAULT_MARGIN, DEFAULT_MARGIN, 21, DEFAULT_MARGIN);
        var sbOptbuttonHover    = MakeFlatStylebox(styleHoverColor, 2 * DEFAULT_MARGIN, DEFAULT_MARGIN, 21, DEFAULT_MARGIN);
        var sbOptbuttonPressed  = MakeFlatStylebox(stylePressedColor, 2 * DEFAULT_MARGIN, DEFAULT_MARGIN, 21, DEFAULT_MARGIN);
        var sbOptbuttonDisabled = MakeFlatStylebox(styleDisabledColor, 2 * DEFAULT_MARGIN, DEFAULT_MARGIN, 21, DEFAULT_MARGIN);

        theme.SetStylebox("normal",   "OptionButton", sbOptbuttonNormal);
        theme.SetStylebox("hover",    "OptionButton", sbOptbuttonHover);
        theme.SetStylebox("pressed",  "OptionButton", sbOptbuttonPressed);
        theme.SetStylebox("disabled", "OptionButton", sbOptbuttonDisabled);

        var sbOptbuttonNormalMirrored   = MakeFlatStylebox(styleNormalColor, 21, DEFAULT_MARGIN, 2 * DEFAULT_MARGIN, DEFAULT_MARGIN);
        var sbOptbuttonHoverMirrored    = MakeFlatStylebox(styleHoverColor, 21, DEFAULT_MARGIN, 2 * DEFAULT_MARGIN, DEFAULT_MARGIN);
        var sbOptbuttonPressedMirrored  = MakeFlatStylebox(stylePressedColor, 21, DEFAULT_MARGIN, 2 * DEFAULT_MARGIN, DEFAULT_MARGIN);
        var sbOptbuttonDisabledMirrored = MakeFlatStylebox(styleDisabledColor, 21, DEFAULT_MARGIN, 2 * DEFAULT_MARGIN, DEFAULT_MARGIN);

        theme.SetStylebox("normal_mirrored",   "OptionButton", sbOptbuttonNormalMirrored);
        theme.SetStylebox("hover_mirrored",    "OptionButton", sbOptbuttonHoverMirrored);
        theme.SetStylebox("pressed_mirrored",  "OptionButton", sbOptbuttonPressedMirrored);
        theme.SetStylebox("disabled_mirrored", "OptionButton", sbOptbuttonDisabledMirrored);

        theme.SetIcon("arrow", "OptionButton", icons["option_button_arrow"]);

        theme.SetFont("font", "OptionButton", null);
        theme.SetFontSize("font_size", "OptionButton", -1);

        theme.SetColor("font_color",               "OptionButton", controlFontColor);
        theme.SetColor("font_pressed_color",       "OptionButton", controlFontPressedColor);
        theme.SetColor("font_hover_color",         "OptionButton", controlFontHoverColor);
        theme.SetColor("font_hover_pressed_color", "OptionButton", controlFontPressedColor);
        theme.SetColor("font_focus_color",         "OptionButton", controlFontFocusColor);
        theme.SetColor("font_disabled_color",      "OptionButton", controlFontDisabledColor);
        theme.SetColor("font_outline_color",       "OptionButton", new Color(1, 1, 1));

        theme.SetConstant("h_separation",   "OptionButton", (int)(2 * scale));
        theme.SetConstant("arrow_margin",   "OptionButton", (int)(4 * scale));
        theme.SetConstant("outline_size",   "OptionButton", 0);
        theme.SetConstant("modulate_arrow", "OptionButton", 0);

        // MenuButton

        theme.SetStylebox("normal",   "MenuButton", buttonNormal);
        theme.SetStylebox("pressed",  "MenuButton", buttonPressed);
        theme.SetStylebox("hover",    "MenuButton", buttonHover);
        theme.SetStylebox("disabled", "MenuButton", buttonDisabled);
        theme.SetStylebox("focus",    "MenuButton", focus);

        theme.SetFont("font", "MenuButton", null);
        theme.SetFontSize("font_size", "MenuButton", -1);

        theme.SetColor("font_color",          "MenuButton", controlFontColor);
        theme.SetColor("font_pressed_color",  "MenuButton", controlFontPressedColor);
        theme.SetColor("font_hover_color",    "MenuButton", controlFontHoverColor);
        theme.SetColor("font_focus_color",    "MenuButton", controlFontFocusColor);
        theme.SetColor("font_disabled_color", "MenuButton", new Color(1, 1, 1, 0.3f));
        theme.SetColor("font_outline_color",  "MenuButton", new Color(1, 1, 1));

        theme.SetConstant("h_separation", "MenuButton", (int)(3 * scale));
        theme.SetConstant("outline_size", "MenuButton", 0);

        // CheckBox

        var cbxEmpty = new StyleBoxEmpty();
        cbxEmpty.SetContentMarginAll(4 * scale);
        var cbxFocus = (StyleBox)focus;
        cbxFocus.SetContentMarginAll(4 * scale);

        theme.SetStylebox("normal",        "CheckBox", cbxEmpty);
        theme.SetStylebox("pressed",       "CheckBox", cbxEmpty);
        theme.SetStylebox("disabled",      "CheckBox", cbxEmpty);
        theme.SetStylebox("hover",         "CheckBox", cbxEmpty);
        theme.SetStylebox("hover_pressed", "CheckBox", cbxEmpty);
        theme.SetStylebox("focus",         "CheckBox", cbxFocus);

        theme.SetIcon("checked",                  "CheckBox", icons["checked"]);
        theme.SetIcon("checked_disabled",         "CheckBox", icons["checked"]);
        theme.SetIcon("unchecked",                "CheckBox", icons["unchecked"]);
        theme.SetIcon("unchecked_disabled",       "CheckBox", icons["unchecked"]);
        theme.SetIcon("radio_checked",            "CheckBox", icons["radio_checked"]);
        theme.SetIcon("radio_checked_disabled",   "CheckBox", icons["radio_checked"]);
        theme.SetIcon("radio_unchecked",          "CheckBox", icons["radio_unchecked"]);
        theme.SetIcon("radio_unchecked_disabled", "CheckBox", icons["radio_unchecked"]);

        theme.SetFont("font", "CheckBox", null);
        theme.SetFontSize("font_size", "CheckBox", -1);

        theme.SetColor("font_color",               "CheckBox", controlFontColor);
        theme.SetColor("font_pressed_color",       "CheckBox", controlFontPressedColor);
        theme.SetColor("font_hover_color",         "CheckBox", controlFontHoverColor);
        theme.SetColor("font_hover_pressed_color", "CheckBox", controlFontPressedColor);
        theme.SetColor("font_focus_color",         "CheckBox", controlFontFocusColor);
        theme.SetColor("font_disabled_color",      "CheckBox", controlFontDisabledColor);
        theme.SetColor("font_outline_color",       "CheckBox", new Color(1, 1, 1));

        theme.SetConstant("h_separation",   "CheckBox", (int)(4 * scale));
        theme.SetConstant("check_v_offset", "CheckBox", 0);
        theme.SetConstant("outline_size",   "CheckBox", 0);

        // CheckButton

        var cbEmpty = new StyleBoxEmpty();
        cbEmpty.SetContentMarginIndividual(6 * scale, 4 * scale, 6 * scale, (int)(4 * scale));

        theme.SetStylebox("normal",        "CheckButton", cbEmpty);
        theme.SetStylebox("pressed",       "CheckButton", cbEmpty);
        theme.SetStylebox("disabled",      "CheckButton", cbEmpty);
        theme.SetStylebox("hover",         "CheckButton", cbEmpty);
        theme.SetStylebox("hover_pressed", "CheckButton", cbEmpty);
        theme.SetStylebox("focus",         "CheckButton", focus);

        theme.SetIcon("checked",            "CheckButton", icons["toggle_on"]);
        theme.SetIcon("checked_disabled",   "CheckButton", icons["toggle_on_disabled"]);
        theme.SetIcon("unchecked",          "CheckButton", icons["toggle_off"]);
        theme.SetIcon("unchecked_disabled", "CheckButton", icons["toggle_off_disabled"]);

        theme.SetIcon("checked_mirrored",            "CheckButton", icons["toggle_on_mirrored"]);
        theme.SetIcon("checked_disabled_mirrored",   "CheckButton", icons["toggle_on_disabled_mirrored"]);
        theme.SetIcon("unchecked_mirrored",          "CheckButton", icons["toggle_off_mirrored"]);
        theme.SetIcon("unchecked_disabled_mirrored", "CheckButton", icons["toggle_off_disabled_mirrored"]);

        theme.SetFont("font", "CheckButton", null);
        theme.SetFontSize("font_size", "CheckButton", -1);

        theme.SetColor("font_color",               "CheckButton", controlFontColor);
        theme.SetColor("font_pressed_color",       "CheckButton", controlFontPressedColor);
        theme.SetColor("font_hover_color",         "CheckButton", controlFontHoverColor);
        theme.SetColor("font_hover_pressed_color", "CheckButton", controlFontPressedColor);
        theme.SetColor("font_focus_color",         "CheckButton", controlFontFocusColor);
        theme.SetColor("font_disabled_color",      "CheckButton", controlFontDisabledColor);
        theme.SetColor("font_outline_color",       "CheckButton", new Color(1, 1, 1));

        theme.SetConstant("h_separation",   "CheckButton", (int)(4 * scale));
        theme.SetConstant("check_v_offset", "CheckButton", 0);
        theme.SetConstant("outline_size",   "CheckButton", 0);

        // Label

        theme.SetStylebox("normal", "Label", new StyleBoxEmpty());
        theme.SetFont("font", "Label", null);
        theme.SetFontSize("font_size", "Label", -1);

        theme.SetColor("font_color",         "Label", new Color(1, 1, 1));
        theme.SetColor("font_shadow_color",  "Label", new Color(0, 0, 0, 0));
        theme.SetColor("font_outline_color", "Label", new Color(1, 1, 1));

        theme.SetConstant("shadow_offset_x",     "Label", (int)scale);
        theme.SetConstant("shadow_offset_y",     "Label", (int)scale);
        theme.SetConstant("outline_size",        "Label", 0);
        theme.SetConstant("shadow_outline_size", "Label", (int)scale);
        theme.SetConstant("line_spacing",        "Label", (int)(3 * scale));

        theme.SetTypeVariation("HeaderSmall", "Label");
        theme.SetFontSize("font_size", "HeaderSmall", DEFAULT_FONT_SIZE + 4);

        theme.SetTypeVariation("HeaderMedium", "Label");
        theme.SetFontSize("font_size", "HeaderMedium", DEFAULT_FONT_SIZE + 8);

        theme.SetTypeVariation("HeaderLarge", "Label");
        theme.SetFontSize("font_size", "HeaderLarge", DEFAULT_FONT_SIZE + 12);

        // LineEdit

        var styleLineEdit = MakeFlatStylebox(styleNormalColor);
        // Add a line at the bottom to make LineEdits distinguishable from Buttons.
        styleLineEdit.SetBorderWidth(Side.SIDE_BOTTOM, 2);
        styleLineEdit.BorderColor = stylePressedColor;
        theme.SetStylebox("normal", "LineEdit", styleLineEdit);

        theme.SetStylebox("focus", "LineEdit", focus);

        var styleLineEditReadOnly = MakeFlatStylebox(styleDisabledColor);
        // Add a line at the bottom to make LineEdits distinguishable from Buttons.
        styleLineEditReadOnly.SetBorderWidth(Side.SIDE_BOTTOM, 2);
        styleLineEditReadOnly.BorderColor = stylePressedColor * new Color(1, 1, 1, 0.5f);
        theme.SetStylebox("read_only", "LineEdit", styleLineEditReadOnly);

        theme.SetFont("font", "LineEdit", null);
        theme.SetFontSize("font_size", "LineEdit", -1);

        theme.SetColor("font_color",                 "LineEdit", controlFontColor);
        theme.SetColor("font_selected_color",        "LineEdit", controlFontPressedColor);
        theme.SetColor("font_uneditable_color",      "LineEdit", controlFontDisabledColor);
        theme.SetColor("font_placeholder_color",     "LineEdit", controlFontPlaceholderColor);
        theme.SetColor("font_outline_color",         "LineEdit", new Color(1, 1, 1));
        theme.SetColor("caret_color",                "LineEdit", controlFontHoverColor);
        theme.SetColor("selection_color",            "LineEdit", controlSelectionColor);
        theme.SetColor("clear_button_color",         "LineEdit", controlFontColor);
        theme.SetColor("clear_button_color_pressed", "LineEdit", controlFontPressedColor);

        theme.SetConstant("minimum_character_width", "LineEdit", 4);
        theme.SetConstant("outline_size",            "LineEdit", 0);
        theme.SetConstant("caret_width",             "LineEdit", 1);

        theme.SetIcon("clear", "LineEdit", icons["line_edit_clear"]);

        // ProgressBar

        theme.SetStylebox("background", "ProgressBar", MakeFlatStylebox(styleDisabledColor, 2, 2, 2, 2, 6));
        theme.SetStylebox("fill",       "ProgressBar", MakeFlatStylebox(styleProgressColor, 2, 2, 2, 2, 6));

        theme.SetFont("font", "ProgressBar", null);
        theme.SetFontSize("font_size", "ProgressBar", -1);

        theme.SetColor("font_color",         "ProgressBar", controlFontHoverColor);
        theme.SetColor("font_shadow_color",  "ProgressBar", new Color(0, 0, 0));
        theme.SetColor("font_outline_color", "ProgressBar", new Color(1, 1, 1));

        theme.SetConstant("outline_size", "ProgressBar", 0);

        // TextEdit

        theme.SetStylebox("normal",    "TextEdit", styleLineEdit);
        theme.SetStylebox("focus",     "TextEdit", focus);
        theme.SetStylebox("read_only", "TextEdit", styleLineEditReadOnly);

        theme.SetIcon("tab",   "TextEdit", icons["text_edit_tab"]);
        theme.SetIcon("space", "TextEdit", icons["text_edit_space"]);

        theme.SetFont("font", "TextEdit", null);
        theme.SetFontSize("font_size", "TextEdit", -1);

        theme.SetColor("background_color",           "TextEdit", new Color(0, 0, 0, 0));
        theme.SetColor("font_color",                 "TextEdit", controlFontColor);
        theme.SetColor("font_selected_color",        "TextEdit", new Color(0, 0, 0, 0));
        theme.SetColor("font_readonly_color",        "TextEdit", controlFontDisabledColor);
        theme.SetColor("font_placeholder_color",     "TextEdit", controlFontPlaceholderColor);
        theme.SetColor("font_outline_color",         "TextEdit", new Color(1, 1, 1));
        theme.SetColor("selection_color",            "TextEdit", controlSelectionColor);
        theme.SetColor("current_line_color",         "TextEdit", new Color(0.25f, 0.25f, 0.26f, 0.8f));
        theme.SetColor("caret_color",                "TextEdit", controlFontColor);
        theme.SetColor("caret_background_color",     "TextEdit", new Color(0, 0, 0));
        theme.SetColor("word_highlighted_color",     "TextEdit", new Color(0.5f, 0.5f, 0.5f, 0.25f));
        theme.SetColor("search_result_color",        "TextEdit", new Color(0.3f, 0.3f, 0.3f));
        theme.SetColor("search_result_border_color", "TextEdit", new Color(0.3f, 0.3f, 0.3f, 0.4f));

        theme.SetConstant("line_spacing", "TextEdit", (int)(4 * scale));
        theme.SetConstant("outline_size", "TextEdit", 0);
        theme.SetConstant("caret_width",  "TextEdit", 1);

        // CodeEdit

        theme.SetStylebox("normal",     "CodeEdit", styleLineEdit);
        theme.SetStylebox("focus",      "CodeEdit", focus);
        theme.SetStylebox("read_only",  "CodeEdit", styleLineEditReadOnly);
        theme.SetStylebox("completion", "CodeEdit", MakeFlatStylebox(styleNormalColor, 0, 0, 0, 0));

        theme.SetIcon("tab",             "CodeEdit", icons["text_edit_tab"]);
        theme.SetIcon("space",           "CodeEdit", icons["text_edit_space"]);
        theme.SetIcon("breakpoint",      "CodeEdit", icons["breakpoint"]);
        theme.SetIcon("bookmark",        "CodeEdit", icons["bookmark"]);
        theme.SetIcon("executing_line",  "CodeEdit", icons["arrow_right"]);
        theme.SetIcon("can_fold",        "CodeEdit", icons["arrow_down"]);
        theme.SetIcon("folded",          "CodeEdit", icons["arrow_right"]);
        theme.SetIcon("folded_eol_icon", "CodeEdit", icons["text_edit_ellipsis"]);

        theme.SetFont("font", "CodeEdit", null);
        theme.SetFontSize("font_size", "CodeEdit", -1);

        theme.SetColor("background_color",                "CodeEdit", new Color(0, 0, 0, 0));
        theme.SetColor("completion_background_color",     "CodeEdit", new Color(0.17f, 0.16f, 0.2f));
        theme.SetColor("completion_selected_color",       "CodeEdit", new Color(0.26f, 0.26f, 0.27f));
        theme.SetColor("completion_existing_color",       "CodeEdit", new Color(0.87f, 0.87f, 0.87f, 0.13f));
        theme.SetColor("completion_scroll_color",         "CodeEdit", controlFontPressedColor * new Color(1, 1, 1, 0.29f));
        theme.SetColor("completion_scroll_hovered_color", "CodeEdit", controlFontPressedColor * new Color(1, 1, 1, 0.4f));
        theme.SetColor("completion_font_color",           "CodeEdit", new Color(0.67f, 0.67f, 0.67f));
        theme.SetColor("font_color",                      "CodeEdit", controlFontColor);
        theme.SetColor("font_selected_color",             "CodeEdit", new Color(0, 0, 0, 0));
        theme.SetColor("font_readonly_color",             "CodeEdit", new Color(controlFontColor.R, controlFontColor.G, controlFontColor.B, 0.5f));
        theme.SetColor("font_placeholder_color",          "CodeEdit", controlFontPlaceholderColor);
        theme.SetColor("font_outline_color",              "CodeEdit", new Color(1, 1, 1));
        theme.SetColor("selection_color",                 "CodeEdit", controlSelectionColor);
        theme.SetColor("bookmark_color",                  "CodeEdit", new Color(0.5f, 0.64f, 1, 0.8f));
        theme.SetColor("breakpoint_color",                "CodeEdit", new Color(0.9f, 0.29f, 0.3f));
        theme.SetColor("executing_line_color",            "CodeEdit", new Color(0.98f, 0.89f, 0.27f));
        theme.SetColor("current_line_color",              "CodeEdit", new Color(0.25f, 0.25f, 0.26f, 0.8f));
        theme.SetColor("code_folding_color",              "CodeEdit", new Color(0.8f, 0.8f, 0.8f, 0.8f));
        theme.SetColor("caret_color",                     "CodeEdit", controlFontColor);
        theme.SetColor("caret_background_color",          "CodeEdit", new Color(0, 0, 0));
        theme.SetColor("brace_mismatch_color",            "CodeEdit", new Color(1, 0.2f, 0.2f));
        theme.SetColor("line_number_color",               "CodeEdit", new Color(0.67f, 0.67f, 0.67f, 0.4f));
        theme.SetColor("word_highlighted_color",          "CodeEdit", new Color(0.8f, 0.9f, 0.9f, 0.15f));
        theme.SetColor("line_length_guideline_color",     "CodeEdit", new Color(0.3f, 0.5f, 0.8f, 0.1f));
        theme.SetColor("search_result_color",             "CodeEdit", new Color(0.3f, 0.3f, 0.3f));
        theme.SetColor("search_result_border_color",      "CodeEdit", new Color(0.3f, 0.3f, 0.3f, 0.4f));

        theme.SetConstant("completion_lines",        "CodeEdit", 7);
        theme.SetConstant("completion_max_width",    "CodeEdit", 50);
        theme.SetConstant("completion_scroll_width", "CodeEdit", 6);
        theme.SetConstant("line_spacing",            "CodeEdit", (int)(4 * scale));
        theme.SetConstant("outline_size",            "CodeEdit", 0);

        var emptyIcon = new ImageTexture();

        var styleHScrollbar                = MakeFlatStylebox(styleNormalColor, 0, 4, 0, 4, 10);
        var styleVScrollbar                = MakeFlatStylebox(styleNormalColor, 4, 0, 4, 0, 10);
        var styleScrollbarGrabber          = MakeFlatStylebox(styleProgressColor, 4, 4, 4, 4, 10);
        var styleScrollbarGrabberHighlight = MakeFlatStylebox(styleFocusColor, 4, 4, 4, 4, 10);
        var styleScrollbarGrabberPressed   = MakeFlatStylebox(styleFocusColor * new Color(0.75f, 0.75f, 0.75f), 4, 4, 4, 4, 10);

        // HScrollBar

        theme.SetStylebox("scroll",            "HScrollBar", styleHScrollbar);
        theme.SetStylebox("scroll_focus",      "HScrollBar", focus);
        theme.SetStylebox("grabber",           "HScrollBar", styleScrollbarGrabber);
        theme.SetStylebox("grabber_highlight", "HScrollBar", styleScrollbarGrabberHighlight);
        theme.SetStylebox("grabber_pressed",   "HScrollBar", styleScrollbarGrabberPressed);

        theme.SetIcon("increment",           "HScrollBar", emptyIcon);
        theme.SetIcon("increment_highlight", "HScrollBar", emptyIcon);
        theme.SetIcon("increment_pressed",   "HScrollBar", emptyIcon);
        theme.SetIcon("decrement",           "HScrollBar", emptyIcon);
        theme.SetIcon("decrement_highlight", "HScrollBar", emptyIcon);
        theme.SetIcon("decrement_pressed",   "HScrollBar", emptyIcon);

        // VScrollBar

        theme.SetStylebox("scroll",            "VScrollBar", styleVScrollbar);
        theme.SetStylebox("scroll_focus",      "VScrollBar", focus);
        theme.SetStylebox("grabber",           "VScrollBar", styleScrollbarGrabber);
        theme.SetStylebox("grabber_highlight", "VScrollBar", styleScrollbarGrabberHighlight);
        theme.SetStylebox("grabber_pressed",   "VScrollBar", styleScrollbarGrabberPressed);

        theme.SetIcon("increment",           "VScrollBar", emptyIcon);
        theme.SetIcon("increment_highlight", "VScrollBar", emptyIcon);
        theme.SetIcon("increment_pressed",   "VScrollBar", emptyIcon);
        theme.SetIcon("decrement",           "VScrollBar", emptyIcon);
        theme.SetIcon("decrement_highlight", "VScrollBar", emptyIcon);
        theme.SetIcon("decrement_pressed",   "VScrollBar", emptyIcon);

        var styleSlider                 = MakeFlatStylebox(styleNormalColor, 4, 4, 4, 4, 4);
        var styleSliderGrabber          = MakeFlatStylebox(styleProgressColor, 4, 4, 4, 4, 4);
        var styleSliderGrabberHighlight = MakeFlatStylebox(styleFocusColor, 4, 4, 4, 4, 4);

        // HSlider

        theme.SetStylebox("slider",                 "HSlider", styleSlider);
        theme.SetStylebox("grabber_area",           "HSlider", styleSliderGrabber);
        theme.SetStylebox("grabber_area_highlight", "HSlider", styleSliderGrabberHighlight);

        theme.SetIcon("grabber",           "HSlider", icons["slider_grabber"]);
        theme.SetIcon("grabber_highlight", "HSlider", icons["slider_grabber_hl"]);
        theme.SetIcon("grabber_disabled",  "HSlider", icons["slider_grabber_disabled"]);
        theme.SetIcon("tick",              "HSlider", icons["hslider_tick"]);

        theme.SetConstant("grabber_offset", "HSlider", 0);

        // VSlider

        theme.SetStylebox("slider",                 "VSlider", styleSlider);
        theme.SetStylebox("grabber_area",           "VSlider", styleSliderGrabber);
        theme.SetStylebox("grabber_area_highlight", "VSlider", styleSliderGrabberHighlight);

        theme.SetIcon("grabber",           "VSlider", icons["slider_grabber"]);
        theme.SetIcon("grabber_highlight", "VSlider", icons["slider_grabber_hl"]);
        theme.SetIcon("grabber_disabled",  "VSlider", icons["slider_grabber_disabled"]);
        theme.SetIcon("tick",              "VSlider", icons["vslider_tick"]);

        theme.SetConstant("grabber_offset", "VSlider", 0);

        // SpinBox

        theme.SetIcon("updown", "SpinBox", icons["updown"]);

        // ScrollContainer

        var empty = new StyleBoxEmpty();
        theme.SetStylebox("panel", "ScrollContainer", empty);

        // Window

        theme.SetStylebox("embedded_border", "Window", SbExpand(MakeFlatStylebox(stylePopupColor, 10, 28, 10, 8), 8, 32, 8, 6));

        theme.SetFont("title_font", "Window", null);
        theme.SetFontSize("title_font_size", "Window", -1);
        theme.SetColor("title_color",            "Window", controlFontColor);
        theme.SetColor("title_outline_modulate", "Window", new Color(1, 1, 1));
        theme.SetConstant("title_outline_size", "Window", 0);
        theme.SetConstant("title_height",       "Window", (int)(36 * scale));
        theme.SetConstant("resize_margin",      "Window", (int)(4 * scale));

        theme.SetIcon("close",         "Window", icons["close"]);
        theme.SetIcon("close_pressed", "Window", icons["close_hl"]);
        theme.SetConstant("close_h_offset", "Window", (int)(18 * scale));
        theme.SetConstant("close_v_offset", "Window", (int)(24 * scale));

        // Dialogs

        // AcceptDialog is currently the base dialog, so this defines styles for all extending nodes.
        theme.SetStylebox("panel",              "AcceptDialog", MakeFlatStylebox(stylePopupColor, 8 * scale, 8 * scale, 8 * scale, 8 * scale));
        theme.SetConstant("buttons_separation", "AcceptDialog", 10);

        // File Dialog

        theme.SetIcon("parent_folder",  "FileDialog", icons["folder_up"]);
        theme.SetIcon("back_folder",    "FileDialog", icons["arrow_left"]);
        theme.SetIcon("forward_folder", "FileDialog", icons["arrow_right"]);
        theme.SetIcon("reload",         "FileDialog", icons["reload"]);
        theme.SetIcon("toggle_hidden",  "FileDialog", icons["visibility_visible"]);
        theme.SetIcon("folder",         "FileDialog", icons["folder"]);
        theme.SetIcon("file",           "FileDialog", icons["file"]);
        theme.SetColor("folder_icon_color",   "FileDialog", new Color(1, 1, 1));
        theme.SetColor("file_icon_color",     "FileDialog", new Color(1, 1, 1));
        theme.SetColor("file_disabled_color", "FileDialog", new Color(1, 1, 1, 0.25f));

        // Popup

        theme.SetStylebox("panel", "PopupPanel", MakeFlatStylebox(styleNormalColor));

        // PopupDialog

        theme.SetStylebox("panel", "PopupDialog", MakeFlatStylebox(styleNormalColor));

        // PopupMenu

        var separatorHorizontal = new StyleBoxLine
        {
            Thickness = (int)Math.Round(scale),
            Color = styleSeparatorColor
        };

        separatorHorizontal.SetContentMarginIndividual(DEFAULT_MARGIN, 0, DEFAULT_MARGIN, 0);

        var separatorVertical = (StyleBoxLine)separatorHorizontal.Duplicate();
        separatorVertical.Vertical = true;
        separatorVertical.SetContentMarginIndividual(0, DEFAULT_MARGIN, 0, DEFAULT_MARGIN);

        // Always display a border for PopupMenus so they can be distinguished from their background.
        var stylePopupPanel = MakeFlatStylebox(stylePopupColor);
        stylePopupPanel.SetBorderWidthAll(2);
        stylePopupPanel.BorderColor = stylePopupBorderColor;

        var stylePopupPanelDisabled = (StyleBoxFlat)stylePopupPanel.Duplicate();
        stylePopupPanelDisabled.BgColor = styleDisabledColor;

        theme.SetStylebox("panel",                   "PopupMenu", stylePopupPanel);
        theme.SetStylebox("panel_disabled",          "PopupMenu", stylePopupPanelDisabled);
        theme.SetStylebox("hover",                   "PopupMenu", MakeFlatStylebox(stylePopupHoverColor));
        theme.SetStylebox("separator",               "PopupMenu", separatorHorizontal);
        theme.SetStylebox("labeled_separator_left",  "PopupMenu", separatorHorizontal);
        theme.SetStylebox("labeled_separator_right", "PopupMenu", separatorHorizontal);

        theme.SetIcon("checked",                  "PopupMenu", icons["checked"]);
        theme.SetIcon("checked_disabled",         "PopupMenu", icons["checked"]);
        theme.SetIcon("unchecked",                "PopupMenu", icons["unchecked"]);
        theme.SetIcon("unchecked_disabled",       "PopupMenu", icons["unchecked"]);
        theme.SetIcon("radio_checked",            "PopupMenu", icons["radio_checked"]);
        theme.SetIcon("radio_checked_disabled",   "PopupMenu", icons["radio_checked"]);
        theme.SetIcon("radio_unchecked",          "PopupMenu", icons["radio_unchecked"]);
        theme.SetIcon("radio_unchecked_disabled", "PopupMenu", icons["radio_unchecked"]);
        theme.SetIcon("submenu",                  "PopupMenu", icons["popup_menu_arrow_right"]);
        theme.SetIcon("submenu_mirrored",         "PopupMenu", icons["popup_menu_arrow_left"]);

        theme.SetFont("font",           "PopupMenu", null);
        theme.SetFont("font_separator", "PopupMenu", null);
        theme.SetFontSize("font_size",           "PopupMenu", -1);
        theme.SetFontSize("font_separator_size", "PopupMenu", -1);

        theme.SetColor("font_color",                   "PopupMenu", controlFontColor);
        theme.SetColor("font_accelerator_color",       "PopupMenu", new Color(0.7f, 0.7f, 0.7f, 0.8f));
        theme.SetColor("font_disabled_color",          "PopupMenu", new Color(0.4f, 0.4f, 0.4f, 0.8f));
        theme.SetColor("font_hover_color",             "PopupMenu", controlFontColor);
        theme.SetColor("font_separator_color",         "PopupMenu", controlFontColor);
        theme.SetColor("font_outline_color",           "PopupMenu", new Color(1, 1, 1));
        theme.SetColor("font_separator_outline_color", "PopupMenu", new Color(1, 1, 1));

        theme.SetConstant("indent",                 "PopupMenu", 10);
        theme.SetConstant("h_separation",           "PopupMenu", (int)(4 * scale));
        theme.SetConstant("v_separation",           "PopupMenu", (int)(4 * scale));
        theme.SetConstant("outline_size",           "PopupMenu", 0);
        theme.SetConstant("separator_outline_size", "PopupMenu", 0);
        theme.SetConstant("item_start_padding",     "PopupMenu", (int)(2 * scale));
        theme.SetConstant("item_end_padding",       "PopupMenu", (int)(2 * scale));

        // GraphNode
        var graphnodeNormal = MakeFlatStylebox(styleNormalColor, 18, 42, 18, 12);
        graphnodeNormal.SetBorderWidth(Side.SIDE_TOP, 30);
        graphnodeNormal.BorderColor = new Color(0.325f, 0.325f, 0.325f, 0.6f);

        var graphnodeSelected = (StyleBoxFlat)graphnodeNormal.Duplicate();
        graphnodeSelected.BorderColor = new Color(0.625f, 0.625f, 0.625f, 0.6f);

        var graphnodeCommentNormal = MakeFlatStylebox(stylePressedColor, 18, 42, 18, 12, 3, true, 2);
        graphnodeCommentNormal.BorderColor = stylePressedColor;

        var graphnodeCommentSelected = (StyleBoxFlat)graphnodeCommentNormal.Duplicate();
        graphnodeCommentSelected.BorderColor = styleHoverColor;

        var graphnodeBreakpoint = MakeFlatStylebox(stylePressedColor, 18, 42, 18, 12, 6, true, 4);
        graphnodeBreakpoint.BorderColor = new Color(0.9f, 0.29f, 0.3f);

        var graphnodePosition = MakeFlatStylebox(stylePressedColor, 18, 42, 18, 12, 6, true, 4);
        graphnodePosition.BorderColor = new Color(0.98f, 0.89f, 0.27f);

        var graphnodeSlot = MakeEmptyStylebox(0, 0, 0, 0);

        theme.SetStylebox("frame",          "GraphNode", graphnodeNormal);
        theme.SetStylebox("selected_frame", "GraphNode", graphnodeSelected);
        theme.SetStylebox("comment",        "GraphNode", graphnodeCommentNormal);
        theme.SetStylebox("comment_focus",  "GraphNode", graphnodeCommentSelected);
        theme.SetStylebox("breakpoint",     "GraphNode", graphnodeBreakpoint);
        theme.SetStylebox("position",       "GraphNode", graphnodePosition);
        theme.SetStylebox("slot",           "GraphNode", graphnodeSlot);

        theme.SetIcon("port",           "GraphNode", icons["graph_port"]);
        theme.SetIcon("close",          "GraphNode", icons["close"]);
        theme.SetIcon("resizer",        "GraphNode", icons["resizer_se"]);
        theme.SetFont("title_font",     "GraphNode", null);
        theme.SetColor("title_color",   "GraphNode", controlFontColor);
        theme.SetColor("close_color",   "GraphNode", controlFontColor);
        theme.SetColor("resizer_color", "GraphNode", controlFontColor);
        theme.SetConstant("separation",     "GraphNode", (int)(2 * scale));
        theme.SetConstant("title_offset",   "GraphNode", (int)(26 * scale));
        theme.SetConstant("title_h_offset", "GraphNode", 0);
        theme.SetConstant("close_offset",   "GraphNode", (int)(22 * scale));
        theme.SetConstant("close_h_offset", "GraphNode", (int)(22 * scale));
        theme.SetConstant("port_offset",    "GraphNode", 0);

        // Tree

        theme.SetStylebox("panel",                 "Tree", MakeFlatStylebox(styleNormalColor, 4, 4, 4, 5));
        theme.SetStylebox("focus",                 "Tree", focus);
        theme.SetStylebox("selected",              "Tree", MakeFlatStylebox(styleSelectedColor));
        theme.SetStylebox("selected_focus",        "Tree", MakeFlatStylebox(styleSelectedColor));
        theme.SetStylebox("cursor",                "Tree", focus);
        theme.SetStylebox("cursor_unfocused",      "Tree", focus);
        theme.SetStylebox("button_pressed",        "Tree", buttonPressed);
        theme.SetStylebox("title_button_normal",   "Tree", MakeFlatStylebox(stylePressedColor, 4, 4, 4, 4));
        theme.SetStylebox("title_button_pressed",  "Tree", MakeFlatStylebox(styleHoverColor, 4, 4, 4, 4));
        theme.SetStylebox("title_button_hover",    "Tree", MakeFlatStylebox(styleNormalColor, 4, 4, 4, 4));
        theme.SetStylebox("custom_button",         "Tree", buttonNormal);
        theme.SetStylebox("custom_button_pressed", "Tree", buttonPressed);
        theme.SetStylebox("custom_button_hover",   "Tree", buttonHover);

        theme.SetIcon("checked",                  "Tree", icons["checked"]);
        theme.SetIcon("unchecked",                "Tree", icons["unchecked"]);
        theme.SetIcon("indeterminate",            "Tree", icons["indeterminate"]);
        theme.SetIcon("updown",                   "Tree", icons["updown"]);
        theme.SetIcon("select_arrow",             "Tree", icons["option_button_arrow"]);
        theme.SetIcon("arrow",                    "Tree", icons["arrow_down"]);
        theme.SetIcon("arrow_collapsed",          "Tree", icons["arrow_right"]);
        theme.SetIcon("arrow_collapsed_mirrored", "Tree", icons["arrow_left"]);

        theme.SetFont("title_button_font", "Tree", null);
        theme.SetFont("font",              "Tree", null);
        theme.SetFontSize("font_size", "Tree", -1);

        theme.SetColor("title_button_color",           "Tree", controlFontColor);
        theme.SetColor("font_color",                   "Tree", controlFontLowColor);
        theme.SetColor("font_selected_color",          "Tree", controlFontPressedColor);
        theme.SetColor("font_outline_color",           "Tree", new Color(1, 1, 1));
        theme.SetColor("guide_color",                  "Tree", new Color(0.7f, 0.7f, 0.7f, 0.25f));
        theme.SetColor("drop_position_color",          "Tree", new Color(1, 1, 1));
        theme.SetColor("relationship_line_color",      "Tree", new Color(0.27f, 0.27f, 0.27f));
        theme.SetColor("parent_hl_line_color",         "Tree", new Color(0.27f, 0.27f, 0.27f));
        theme.SetColor("children_hl_line_color",       "Tree", new Color(0.27f, 0.27f, 0.27f));
        theme.SetColor("custom_button_font_highlight", "Tree", controlFontHoverColor);

        theme.SetConstant("h_separation",            "Tree", (int)(4 * scale));
        theme.SetConstant("v_separation",            "Tree", (int)(4 * scale));
        theme.SetConstant("item_margin",             "Tree", (int)(16 * scale));
        theme.SetConstant("button_margin",           "Tree", (int)(4 * scale));
        theme.SetConstant("draw_relationship_lines", "Tree", 0);
        theme.SetConstant("relationship_line_width", "Tree", 1);
        theme.SetConstant("parent_hl_line_width",    "Tree", 1);
        theme.SetConstant("children_hl_line_width",  "Tree", 1);
        theme.SetConstant("parent_hl_line_margin",   "Tree", 0);
        theme.SetConstant("draw_guides",             "Tree", 1);
        theme.SetConstant("scroll_border",           "Tree", 4);
        theme.SetConstant("scroll_speed",            "Tree", 12);
        theme.SetConstant("outline_size",            "Tree", 0);

        // ItemList

        theme.SetStylebox("panel",           "ItemList", MakeFlatStylebox(styleNormalColor));
        theme.SetStylebox("focus",           "ItemList", focus);
        theme.SetConstant("h_separation",    "ItemList", 4);
        theme.SetConstant("v_separation",    "ItemList", 2);
        theme.SetConstant("icon_margin",     "ItemList", 4);
        theme.SetConstant("line_separation", "ItemList", (int)(2 * scale));

        theme.SetFont("font", "ItemList", null);
        theme.SetFontSize("font_size", "ItemList", -1);

        theme.SetColor("font_color",          "ItemList", controlFontLowerColor);
        theme.SetColor("font_selected_color", "ItemList", controlFontPressedColor);
        theme.SetColor("font_outline_color",  "ItemList", new Color(1, 1, 1));
        theme.SetColor("guide_color",         "ItemList", new Color(0.7f, 0.7f, 0.7f, 0.25f));
        theme.SetStylebox("selected",         "ItemList", MakeFlatStylebox(styleSelectedColor));
        theme.SetStylebox("selected_focus",   "ItemList", MakeFlatStylebox(styleSelectedColor));
        theme.SetStylebox("cursor",           "ItemList", focus);
        theme.SetStylebox("cursor_unfocused", "ItemList", focus);

        theme.SetConstant("outline_size", "ItemList", 0);

        // TabContainer

        var styleTabSelected = MakeFlatStylebox(styleNormalColor, 10, 4, 10, 4, 0);
        styleTabSelected.SetBorderWidth(Side.SIDE_TOP, (int)Math.Round(2 * scale));
        styleTabSelected.BorderColor = styleFocusColor;
        var styleTabUnselected = MakeFlatStylebox(stylePressedColor, 10, 4, 10, 4, 0);
        // Add some spacing between unselected tabs to make them easier to distinguish from each other.
        styleTabUnselected.SetBorderWidth(Side.SIDE_LEFT, (int)Math.Round(scale));
        styleTabUnselected.SetBorderWidth(Side.SIDE_RIGHT, (int)Math.Round(scale));
        styleTabUnselected.BorderColor = stylePopupBorderColor;
        var styleTabDisabled = (StyleBoxFlat)styleTabUnselected.Duplicate();
        styleTabDisabled.BgColor = styleDisabledColor;

        theme.SetStylebox("tab_selected",      "TabContainer", styleTabSelected);
        theme.SetStylebox("tab_unselected",    "TabContainer", styleTabUnselected);
        theme.SetStylebox("tab_disabled",      "TabContainer", styleTabDisabled);
        theme.SetStylebox("panel",             "TabContainer", MakeFlatStylebox(styleNormalColor, 0, 0, 0, 0));
        theme.SetStylebox("tabbar_background", "TabContainer", MakeEmptyStylebox(0, 0, 0, 0));

        theme.SetIcon("increment",           "TabContainer", icons["scroll_button_right"]);
        theme.SetIcon("increment_highlight", "TabContainer", icons["scroll_button_right_hl"]);
        theme.SetIcon("decrement",           "TabContainer", icons["scroll_button_left"]);
        theme.SetIcon("decrement_highlight", "TabContainer", icons["scroll_button_left_hl"]);
        theme.SetIcon("drop_mark",           "TabContainer", icons["tabs_drop_mark"]);
        theme.SetIcon("menu",                "TabContainer", icons["tabs_menu"]);
        theme.SetIcon("menu_highlight",      "TabContainer", icons["tabs_menu_hl"]);

        theme.SetFont("font", "TabContainer", null);
        theme.SetFontSize("font_size", "TabContainer", -1);

        theme.SetColor("font_selected_color",   "TabContainer", controlFontHoverColor);
        theme.SetColor("font_unselected_color", "TabContainer", controlFontLowColor);
        theme.SetColor("font_disabled_color",   "TabContainer", controlFontDisabledColor);
        theme.SetColor("font_outline_color",    "TabContainer", new Color(1, 1, 1));
        theme.SetColor("drop_mark_color",       "TabContainer", new Color(1, 1, 1));

        theme.SetConstant("side_margin",     "TabContainer", (int)(8 * scale));
        theme.SetConstant("icon_separation", "TabContainer", (int)(4 * scale));
        theme.SetConstant("outline_size",    "TabContainer", 0);

        // TabBar

        theme.SetStylebox("tab_selected",     "TabBar", styleTabSelected);
        theme.SetStylebox("tab_unselected",   "TabBar", styleTabUnselected);
        theme.SetStylebox("tab_disabled",     "TabBar", styleTabDisabled);
        theme.SetStylebox("button_pressed",   "TabBar", buttonPressed);
        theme.SetStylebox("button_highlight", "TabBar", buttonNormal);

        theme.SetIcon("increment",           "TabBar", icons["scroll_button_right"]);
        theme.SetIcon("increment_highlight", "TabBar", icons["scroll_button_right_hl"]);
        theme.SetIcon("decrement",           "TabBar", icons["scroll_button_left"]);
        theme.SetIcon("decrement_highlight", "TabBar", icons["scroll_button_left_hl"]);
        theme.SetIcon("drop_mark",           "TabBar", icons["tabs_drop_mark"]);
        theme.SetIcon("close",               "TabBar", icons["close"]);

        theme.SetFont("font", "TabBar", null);
        theme.SetFontSize("font_size", "TabBar", -1);

        theme.SetColor("font_selected_color",   "TabBar", controlFontHoverColor);
        theme.SetColor("font_unselected_color", "TabBar", controlFontLowColor);
        theme.SetColor("font_disabled_color",   "TabBar", controlFontDisabledColor);
        theme.SetColor("font_outline_color",    "TabBar", new Color(1, 1, 1));
        theme.SetColor("drop_mark_color",       "TabBar", new Color(1, 1, 1));

        theme.SetConstant("h_separation", "TabBar", (int)(4 * scale));
        theme.SetConstant("outline_size", "TabBar", 0);

        // Separators

        theme.SetStylebox("separator", "HSeparator", separatorHorizontal);
        theme.SetStylebox("separator", "VSeparator", separatorVertical);

        theme.SetIcon("close", "Icons", icons["close"]);
        theme.SetFont("normal", "Fonts", null);
        theme.SetFont("large",  "Fonts", null);

        theme.SetConstant("separation", "HSeparator", (int)(4 * scale));
        theme.SetConstant("separation", "VSeparator", (int)(4 * scale));

        // ColorPicker

        theme.SetConstant("margin",      "ColorPicker", (int)(4 * scale));
        theme.SetConstant("sv_width",    "ColorPicker", (int)(256 * scale));
        theme.SetConstant("sv_height",   "ColorPicker", (int)(256 * scale));
        theme.SetConstant("h_width",     "ColorPicker", 30);
        theme.SetConstant("label_width", "ColorPicker", 10);

        theme.SetIcon("folded_arrow",         "ColorPicker", icons["arrow_right"]);
        theme.SetIcon("expanded_arrow",       "ColorPicker", icons["arrow_down"]);
        theme.SetIcon("screen_picker",        "ColorPicker", icons["color_picker_pipette"]);
        theme.SetIcon("shape_circle",         "ColorPicker", icons["picker_shape_circle"]);
        theme.SetIcon("shape_rect",           "ColorPicker", icons["picker_shape_rectangle"]);
        theme.SetIcon("shape_rect_wheel",     "ColorPicker", icons["picker_shape_rectangle_wheel"]);
        theme.SetIcon("add_preset",           "ColorPicker", icons["add"]);
        theme.SetIcon("sample_bg",            "ColorPicker", icons["mini_checkerboard"]);
        theme.SetIcon("overbright_indicator", "ColorPicker", icons["color_picker_overbright"]);
        theme.SetIcon("bar_arrow",            "ColorPicker", icons["color_picker_bar_arrow"]);
        theme.SetIcon("picker_cursor",        "ColorPicker", icons["color_picker_cursor"]);

        {
            const int PRECISION = 7;

            var hueGradient = new Gradient();
            var offsets = new float[PRECISION];
            var colors = new Color[PRECISION];

            for (var i = 0; i < PRECISION; i++)
            {
                var h = i / (float)(PRECISION - 1);
                offsets[i] = h;
                colors[i] = Color.FromHsv(h, 1, 1);
            }
            hueGradient.Offsets = offsets;
            hueGradient.Colors  = colors;

            var hueTexture = new GradientTexture2D
            {
                Width    = 800,
                Height   = 6,
                Gradient = hueGradient
            };

            theme.SetIcon("color_hue", "ColorPicker", hueTexture);
        }

        {
            const int PRECISION = 7;

            var hueGradient = new Gradient();
            var offsets = new float[PRECISION];
            var colors = new Color[PRECISION];

            for (var i = 0; i < PRECISION; i++)
            {
                var h = i / (float)(PRECISION - 1);
                offsets[i] = h;
                colors[i] = Color.FromOkHsl(h, 1, 0.5f);
            }
            hueGradient.Offsets = offsets;
            hueGradient.Colors = colors;

            var hueTexture = new GradientTexture2D
            {
                Width    = 800,
                Height   = 6,
                Gradient = hueGradient
            };

            theme.SetIcon("color_okhsl_hue", "ColorPicker", hueTexture);
        }

        // ColorPickerButton

        theme.SetIcon("bg", "ColorPickerButton", icons["mini_checkerboard"]);
        theme.SetStylebox("normal",   "ColorPickerButton", buttonNormal);
        theme.SetStylebox("pressed",  "ColorPickerButton", buttonPressed);
        theme.SetStylebox("hover",    "ColorPickerButton", buttonHover);
        theme.SetStylebox("disabled", "ColorPickerButton", buttonDisabled);
        theme.SetStylebox("focus",    "ColorPickerButton", focus);

        theme.SetFont("font", "ColorPickerButton", null);
        theme.SetFontSize("font_size", "ColorPickerButton", -1);

        theme.SetColor("font_color",          "ColorPickerButton", new Color(1, 1, 1, 1));
        theme.SetColor("font_pressed_color",  "ColorPickerButton", new Color(0.8f, 0.8f, 0.8f, 1));
        theme.SetColor("font_hover_color",    "ColorPickerButton", new Color(1, 1, 1, 1));
        theme.SetColor("font_focus_color",    "ColorPickerButton", new Color(1, 1, 1, 1));
        theme.SetColor("font_disabled_color", "ColorPickerButton", new Color(0.9f, 0.9f, 0.9f, 0.3f));
        theme.SetColor("font_outline_color",  "ColorPickerButton", new Color(1, 1, 1));

        theme.SetConstant("h_separation", "ColorPickerButton", (int)(2 * scale));
        theme.SetConstant("outline_size", "ColorPickerButton", 0);

        // ColorPresetButton

        var presetSb = MakeFlatStylebox(new Color(1, 1, 1), 2, 2, 2, 2);
        presetSb.SetCornerRadiusAll(2);
        presetSb.CornerDetail = 2;
        presetSb.AntiAliased  = false;

        theme.SetStylebox("preset_fg", "ColorPresetButton", presetSb);
        theme.SetIcon("preset_bg",            "ColorPresetButton", icons["mini_checkerboard"]);
        theme.SetIcon("overbright_indicator", "ColorPresetButton", icons["color_picker_overbright"]);

        // TooltipPanel + TooltipLabel

        theme.SetStylebox(
            "panel",
            "TooltipPanel",
            MakeFlatStylebox(
                new Color(0, 0, 0, 0.5f),
                2 * DEFAULT_MARGIN,
                0.5f * DEFAULT_MARGIN,
                2 * DEFAULT_MARGIN,
                0.5f * DEFAULT_MARGIN
            )
        );

        theme.SetFontSize("font_size", "TooltipLabel", -1);
        theme.SetFont("font", "TooltipLabel", null);

        theme.SetColor("font_color",         "TooltipLabel", controlFontColor);
        theme.SetColor("font_shadow_color",  "TooltipLabel", new Color(0, 0, 0, 0));
        theme.SetColor("font_outline_color", "TooltipLabel", new Color(0, 0, 0, 0));

        theme.SetConstant("shadow_offset_x", "TooltipLabel", 1);
        theme.SetConstant("shadow_offset_y", "TooltipLabel", 1);
        theme.SetConstant("outline_size",    "TooltipLabel", 0);

        // RichTextLabel

        theme.SetStylebox("focus",  "RichTextLabel", focus);
        theme.SetStylebox("normal", "RichTextLabel", MakeEmptyStylebox(0, 0, 0, 0));

        theme.SetFont("normal_font",       "RichTextLabel", null);
        theme.SetFont("bold_font",         "RichTextLabel", boldFont);
        theme.SetFont("italics_font",      "RichTextLabel", italicsFont);
        theme.SetFont("bold_italics_font", "RichTextLabel", boldItalicsFont);
        theme.SetFont("mono_font",         "RichTextLabel", null);
        theme.SetFontSize("normal_font_size",       "RichTextLabel", -1);
        theme.SetFontSize("bold_font_size",         "RichTextLabel", -1);
        theme.SetFontSize("italics_font_size",      "RichTextLabel", -1);
        theme.SetFontSize("bold_italics_font_size", "RichTextLabel", -1);
        theme.SetFontSize("mono_font_size",         "RichTextLabel", -1);

        theme.SetColor("default_color",       "RichTextLabel", new Color(1, 1, 1));
        theme.SetColor("font_selected_color", "RichTextLabel", new Color(0, 0, 0, 0));
        theme.SetColor("selection_color",     "RichTextLabel", new Color(0.1f, 0.1f, 1, 0.8f));

        theme.SetColor("font_shadow_color", "RichTextLabel", new Color(0, 0, 0, 0));

        theme.SetColor("font_outline_color", "RichTextLabel", new Color(1, 1, 1));

        theme.SetConstant("shadow_offset_x",     "RichTextLabel", (int)scale);
        theme.SetConstant("shadow_offset_y",     "RichTextLabel", (int)scale);
        theme.SetConstant("shadow_outline_size", "RichTextLabel", (int)scale);

        theme.SetConstant("line_separation",    "RichTextLabel", 0);
        theme.SetConstant("table_h_separation", "RichTextLabel", (int)(3 * scale));
        theme.SetConstant("table_v_separation", "RichTextLabel", (int)(3 * scale));

        theme.SetConstant("outline_size", "RichTextLabel", 0);

        theme.SetColor("table_odd_row_bg",  "RichTextLabel", new Color(0, 0, 0, 0));
        theme.SetColor("table_even_row_bg", "RichTextLabel", new Color(0, 0, 0, 0));
        theme.SetColor("table_border",      "RichTextLabel", new Color(0, 0, 0, 0));

        theme.SetConstant("text_highlight_h_padding", "RichTextLabel", (int)(3 * scale));
        theme.SetConstant("text_highlight_v_padding", "RichTextLabel", (int)(3 * scale));

        // Containers

        theme.SetIcon("h_grabber", "SplitContainer", icons["hsplitter"]);
        theme.SetIcon("v_grabber", "SplitContainer", icons["vsplitter"]);
        theme.SetIcon("grabber",   "VSplitContainer", icons["vsplitter"]);
        theme.SetIcon("grabber",   "HSplitContainer", icons["hsplitter"]);

        theme.SetConstant("separation",             "BoxContainer", (int)(4 * scale));
        theme.SetConstant("separation",             "HBoxContainer", (int)(4 * scale));
        theme.SetConstant("separation",             "VBoxContainer", (int)(4 * scale));
        theme.SetConstant("margin_left",            "MarginContainer", 0);
        theme.SetConstant("margin_top",             "MarginContainer", 0);
        theme.SetConstant("margin_right",           "MarginContainer", 0);
        theme.SetConstant("margin_bottom",          "MarginContainer", 0);
        theme.SetConstant("h_separation",           "GridContainer", (int)(4 * scale));
        theme.SetConstant("v_separation",           "GridContainer", (int)(4 * scale));
        theme.SetConstant("separation",             "SplitContainer", (int)(12 * scale));
        theme.SetConstant("separation",             "HSplitContainer", (int)(12 * scale));
        theme.SetConstant("separation",             "VSplitContainer", (int)(12 * scale));
        theme.SetConstant("minimum_grab_thickness", "SplitContainer", (int)(6 * scale));
        theme.SetConstant("minimum_grab_thickness", "HSplitContainer", (int)(6 * scale));
        theme.SetConstant("minimum_grab_thickness", "VSplitContainer", (int)(6 * scale));
        theme.SetConstant("autohide",               "SplitContainer", (int)scale);
        theme.SetConstant("autohide",               "HSplitContainer", (int)scale);
        theme.SetConstant("autohide",               "VSplitContainer", (int)scale);
        theme.SetConstant("h_separation",           "FlowContainer", (int)(4 * scale));
        theme.SetConstant("v_separation",           "FlowContainer", (int)(4 * scale));
        theme.SetConstant("h_separation",           "HFlowContainer", (int)(4 * scale));
        theme.SetConstant("v_separation",           "HFlowContainer", (int)(4 * scale));
        theme.SetConstant("h_separation",           "VFlowContainer", (int)(4 * scale));
        theme.SetConstant("v_separation",           "VFlowContainer", (int)(4 * scale));

        theme.SetStylebox("panel", "PanelContainer", MakeFlatStylebox(styleNormalColor, 0, 0, 0, 0));

        theme.SetIcon("minus",   "GraphEdit", icons["zoom_less"]);
        theme.SetIcon("reset",   "GraphEdit", icons["zoom_reset"]);
        theme.SetIcon("more",    "GraphEdit", icons["zoom_more"]);
        theme.SetIcon("snap",    "GraphEdit", icons["grid_snap"]);
        theme.SetIcon("minimap", "GraphEdit", icons["grid_minimap"]);
        theme.SetIcon("layout",  "GraphEdit", icons["grid_layout"]);
        theme.SetStylebox("bg", "GraphEdit", MakeFlatStylebox(styleNormalColor, 4, 4, 4, 5));
        theme.SetColor("grid_minor",       "GraphEdit", new Color(1, 1, 1, 0.05f));
        theme.SetColor("grid_major",       "GraphEdit", new Color(1, 1, 1, 0.2f));
        theme.SetColor("selection_fill",   "GraphEdit", new Color(1, 1, 1, 0.3f));
        theme.SetColor("selection_stroke", "GraphEdit", new Color(1, 1, 1, 0.8f));
        theme.SetColor("activity",         "GraphEdit", new Color(1, 1, 1));

        // Visual Node Ports

        theme.SetConstant("port_hotzone_inner_extent", "GraphEdit", (int)(22 * scale));
        theme.SetConstant("port_hotzone_outer_extent", "GraphEdit", (int)(26 * scale));

        theme.SetStylebox("bg", "GraphEditMinimap", MakeFlatStylebox(new Color(0.24f, 0.24f, 0.24f), 0, 0, 0, 0));
        var styleMinimapCamera = MakeFlatStylebox(new Color(0.65f, 0.65f, 0.65f, 0.2f), 0, 0, 0, 0, 0);
        styleMinimapCamera.BorderColor = new Color(0.65f, 0.65f, 0.65f, 0.45f);
        styleMinimapCamera.SetBorderWidthAll(1);
        theme.SetStylebox("camera", "GraphEditMinimap", styleMinimapCamera);
        theme.SetStylebox("node",   "GraphEditMinimap", MakeFlatStylebox(new Color(1, 1, 1), 0, 0, 0, 0, 2));

        theme.SetIcon("resizer", "GraphEditMinimap", icons["resizer_nw"]);
        theme.SetColor("resizer_color", "GraphEditMinimap", new Color(1, 1, 1, 0.85f));

        // Theme

        defaultIcon = icons["error_icon"];
        // Same color as the error icon.
        defaultStyle = MakeFlatStylebox(new Color(1, 0.365f, 0.365f), 4, 4, 4, 4, 0, false, 2);
    }

    public static ImageTexture GenerateIcon(string source)
    {
        var img = new Image();

        #if MODULE_SVG_ENABLED
        // Upsample icon generation only if the scale isn't an integer multiplier.
        // Generating upsampled icons is slower, and the benefit is hardly visible
        // with integer scales.
        var upsample = !MathX.IsEqualApprox(Math.Round(scale), scale);
        var err = ImageLoaderSVG.CreateImageFromString(img, source, scale, upsample, new Dictionary<Color, Color>());
        if (ERR_FAIL_COND_V_MSG(err != Error.OK, "Failed generating icon, unsupported or invalid SVG data in default theme."))
        {
            return new();
        }
        #endif

        return ImageTexture.CreateFromImage(img);
    }

    public static void MakeDefaultTheme(
        float                          scale,
        Font?                          font,
        TextServer.SubpixelPositioning fontSubpixel,
        TextServer.Hinting             fontHinting,
        TextServer.FontAntialiasing    fontAntialiasing,
        bool                           fontMsdf,
        bool                           fontGenerateMipmaps
    )
    {
        var t = new Theme();

        var boldItalicsFont = default(FontVariation);
        var boldFont        = default(FontVariation);
        var italicsFont     = default(FontVariation);
        var defaultScale    = Math.Clamp(scale, 0.5f, 8.0f);

        Font? defaultFont;
        if (font != null)
        {
            // Use the custom font defined in the Project Settings.
            defaultFont = font;
        }
        else
        {
            // Use the default DynamicFont (separate from the editor font).
            // The default DynamicFont is chosen to have a small file size since it's
            // embedded in both editor and export template binaries.
            var dynamicFont = new FontFile
            {
                Antialiasing                    = fontAntialiasing,
                Data                            = DefaultFont.FontOpenSansSemiBold,
                GenerateMipmaps                 = fontGenerateMipmaps,
                Hinting                         = fontHinting,
                MultichannelSignedDistanceField = fontMsdf,
                SubpixelPositioning             = fontSubpixel,
            };

            defaultFont = dynamicFont;
        }

        if (defaultFont != null)
        {
            boldFont = new()
            {
                BaseFont          = defaultFont,
                VariationEmbolden = 1.2f
            };

            boldItalicsFont = new()
            {
                BaseFont           = defaultFont,
                VariationEmbolden  = 1.2f,
                VariationTransform = new((RealT)1.0, (RealT)0.2, (RealT)0.0, (RealT)1.0, (RealT)0.0, (RealT)0.0)
            };

            italicsFont = new()
            {
                BaseFont           = defaultFont,
                VariationTransform = new((RealT)1.0, (RealT)0.2, (RealT)0.0, (RealT)1.0, (RealT)0.0, (RealT)0.0)
            };
        }

        FillDefaultTheme(
            t,
            defaultFont,
            boldFont,
            boldItalicsFont,
            italicsFont,
            out var defaultIcon,
            out var defaultStyle,
            defaultScale
        );

        ThemeDB.Singleton.DefaultTheme = t;

        ThemeDB.Singleton.FallbackBaseScale = defaultScale;
        ThemeDB.Singleton.FallbackIcon      = defaultIcon;
        ThemeDB.Singleton.FallbackStyleBox  = defaultStyle;
        ThemeDB.Singleton.FallbackFont      = defaultFont;
        ThemeDB.Singleton.FallbackFontSize  = (int)(DEFAULT_FONT_SIZE * defaultScale);
    }

    public static StyleBox MakeEmptyStylebox(
        float marginLeft   = -1,
        float marginTop    = -1,
        float marginRight  = -1,
        float marginBottom = -1
    )
    {
        var style = new StyleBoxEmpty();

        style.SetContentMarginIndividual(marginLeft * scale, marginTop * scale, marginRight * scale, marginBottom * scale);

	    return style;
    }

    public static StyleBoxFlat MakeFlatStylebox(
        Color color,
        float marginLeft   = DEFAULT_MARGIN,
        float marginTop    = DEFAULT_MARGIN,
        float marginRight  = DEFAULT_MARGIN,
        float marginBottom = DEFAULT_MARGIN,
        int   cornerRadius = DEFAULT_CORNER_RADIUS,
        bool  drawCenter   = true,
        int   borderWidth  = 0
    )
    {
        var style = new StyleBoxFlat
        {
            BgColor = color
        };

        style.SetContentMarginIndividual(marginLeft * scale, marginTop * scale, marginRight * scale, marginBottom * scale);

        style.SetCornerRadiusAll(cornerRadius);
        style.AntiAliased = true;
        // Adjust level of detail based on the corners' effective sizes.
        style.CornerDetail = (int)(Math.Min(Math.Ceiling(1.5 * cornerRadius), 6.0) * scale);

        style.DrawCenter = drawCenter;
        style.SetBorderWidthAll(borderWidth);

        return style;
    }

    public static StyleBoxFlat SbExpand(StyleBoxFlat sbox, float left, float top, float right, float bottom)
    {
        sbox.SetExpandMargin(Side.SIDE_LEFT, left * scale);
        sbox.SetExpandMargin(Side.SIDE_TOP, top * scale);
        sbox.SetExpandMargin(Side.SIDE_RIGHT, right * scale);
        sbox.SetExpandMargin(Side.SIDE_BOTTOM, bottom * scale);
        return sbox;
    }
}
