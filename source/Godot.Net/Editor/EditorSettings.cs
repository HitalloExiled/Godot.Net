namespace Godot.Net.Editor;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Godot.Net.Core;
using Godot.Net.Core.Config;
using Godot.Net.Core.Error;
using Godot.Net.Core.Input;
using Godot.Net.Core.IO;
using Godot.Net.Core.Math;
using Godot.Net.Core.String;
using Godot.Net.Servers;

using IOPath = Path;
using OS     = Core.OS.OS;

#pragma warning disable IDE0052, CS0414, IDE0044, IDE0051, CS0169 // TODO Remove

public partial class EditorSettings : Resource
{
    public event Action? SettingsChanged;

    private static readonly object padlock = new();

    private static EditorSettings? singleton;

    public static float AutoDisplayScale
    {
        get
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) /* || ANDROID_ENABLED */)
            {
                return DisplayServer.Singleton.ScreenGetMaxScale();
            }
            else
            {
                var screen = DisplayServer.Singleton.WindowGetCurrentScreen();

                if (DisplayServer.Singleton.ScreenGetSize(screen) == default)
                {
                    // Invalid screen size, skip.
                    return 1;
                }

                // Use the smallest dimension to use a correct display scale on portrait displays.
                var smallestDimension = Math.Min(DisplayServer.Singleton.ScreenGetSize(screen).X, DisplayServer.Singleton.ScreenGetSize(screen).Y);
                if (DisplayServer.Singleton.ScreenGetDpi(screen) >= 192 && smallestDimension >= 1400)
                {
                    // hiDPI display.
                    return 2;
                }
                else if (smallestDimension >= 1700)
                {
                    // Likely a hiDPI display, but we aren't certain due to the returned DPI.
                    // Use an intermediate scale to handle this situation.
                    return 1.5f;
                }
                else if (smallestDimension <= 800)
                {
                    // Small loDPI display. Use a smaller display scale so that editor elements fit more easily.
                    // Icons won't look great, but this is better than having editor elements overflow from its window.
                    return 0.75f;
                }
                return 1;
            }
        }
    }

    private readonly Dictionary<string, PropertyInfo> hints = new();

    private int lastOrder;

    public static bool           IsInitialized => singleton != null;
    public static EditorSettings Singleton     => singleton ?? throw new NullReferenceException();

    private readonly Dictionary<string, List<InputEvent>> builtinActionOverrides = new();
    private readonly HashSet<string>                      changedSettings        = new();
    private readonly Dictionary<string, VariantContainer> props                  = new();
    private readonly Dictionary<string, Shortcut>         shortcuts              = new();

    private bool saveChangedSetting;

    public bool IsDarkTheme
    {
        get
        {
            const int AUTO_COLOR  = 0;
            const int LIGHT_COLOR = 2;

            var baseColor            = this.GetSetting<Color>("interface/theme/base_color");
            var iconFontColorSetting = this.GetSetting<int>("interface/theme/icon_and_font_color");

            return iconFontColorSetting == AUTO_COLOR && baseColor.Luminance < 0.5f || iconFontColorSetting == LIGHT_COLOR;
        }
    }

    public bool OptimizeSave { get; set; }

    public EditorSettings() => this.LoadDefaults();

    private static bool IsDefaultTextEditorTheme(string themeName) =>
        themeName is "default" or "godot 2" or "custom";

    public static void Create()
    {
        // IMPORTANT: create() *must* create a valid EditorSettings singleton,
        // as the rest of the engine code will assume it. As such, it should never
        // return (incl. via ERR_FAIL) without initializing the singleton member.

        if (singleton != null)
        {
            ERR_PRINT("Can't recreate EditorSettings as it already exists.");
        }

        // GDREGISTER_CLASS(EditorSettings); // Otherwise it can't be unserialized.

        var configFilePath = "";
        var extraConfig    = new ConfigFile();

        if (!EditorPaths.IsInitialized)
        {
            ERR_PRINT("Bug (please report): EditorPaths haven't been initialized, EditorSettings cannot be created properly.");
            goto fail;
        }

        if (EditorPaths.Singleton.IsSelfContained)
        {
            var err = extraConfig.Load(EditorPaths.Singleton.SelfContainedFile);
            if (err != Error.OK)
            {
                ERR_PRINT($"Can't load extra config from path: {EditorPaths.Singleton.SelfContainedFile}");
            }
        }

        if (EditorPaths.Singleton.ArePathsValid)
        {
            // Validate editor config file.
            var configFileName = $"editor_settings-{GodotVersion.VERSION_MAJOR}.tres";
            configFilePath     = IOPath.Join(EditorPaths.Singleton.ConfigDir, configFileName);

            if (!Directory.Exists(configFilePath))
            {
                goto fail;
            }

            singleton = (EditorSettings?)ResourceLoader.Load(configFilePath, "EditorSettings");

            if (singleton == null)
            {
                ERR_PRINT($"Could not load editor settings from path: {configFilePath}");
                goto fail;
            }

            singleton.saveChangedSetting = true;

            PrintVerbose("EditorSettings: Load OK!");

            #region TODO
            // Singleton.SetupLanguage();
            // Singleton.SetupNetwork();
            // Singleton.LoadFavoritesAndRecentDirs();
            #endregion TODO
            singleton.ListTextEditorThemes();

            return;
        }

    fail:
        // patch init projects
        var exePath = AppContext.BaseDirectory;

        if (extraConfig.HasSection("init_projects"))
        {
            var list = extraConfig.GetValue<string[]>("init_projects", "list")!;

            for (var i = 0; i < list.Length; i++)
            {
                list[i] = IOPath.Join(exePath, list[i]);
            }

            extraConfig.SetValue("init_projects", "list", list);
        }

        singleton = new EditorSettings();
        singleton.SetPath(configFilePath, true);
        singleton.saveChangedSetting = true;
        singleton.LoadDefaults(extraConfig);
        #region TODO
        // singleton.SetupLanguage();
        // singleton.SetupNetwork();
        #endregion TODO
        singleton.ListTextEditorThemes();
    }

    private bool Get(string p_name, out object? ret)
    {
        lock (padlock)
        {
            if (p_name == "shortcuts")
            {
                var saveArray = new List<Dictionary<string, object>>();

                var builtinList = InputMap.Singleton.GetBuiltins();

                foreach (var shortcutDefinition in this.shortcuts)
                {
                    var sc = shortcutDefinition.Value;

                    if (builtinList.ContainsKey(shortcutDefinition.Key))
                    {
                        // This shortcut was auto-generated from built in actions: don't save.
                        // If the builtin is overridden, it will be saved in the "builtin_action_overrides" section below.
                        continue;
                    }

                    var shortcutEvents = sc.Events;

                    var dict = new Dictionary<string, object>
                    {
                        ["name"]      = shortcutDefinition.Key,
                        ["shortcuts"] = shortcutEvents
                    };

                    if (!sc.HasMeta("original"))
                    {
                        // Getting the meta when it doesn't exist will return an empty array. If the 'shortcutEvents' have been cleared,
                        // we still want save the shortcut in this case so that shortcuts that the user has customized are not reset,
                        // even if the 'original' has not been populated yet. This can happen when calling save() from the Project Manager.
                        saveArray.Add(dict);
                        continue;
                    }

                    var originalEvents = (InputEvent[])sc.GetMeta("original");

                    var isSame = Shortcut.IsEventArrayEqual(originalEvents, shortcutEvents);
                    if (isSame)
                    {
                        continue; // Not changed from default; don't save.
                    }

                    saveArray.Add(dict);
                }

                ret = saveArray;

                return true;
            }
            else if (p_name == "builtin_action_overrides")
            {
                var actionsArr = new List<Dictionary<string, object>>();

                foreach (var actionOverride in this.builtinActionOverrides)
                {
                    var events = actionOverride.Value;

                    var actionDict = new Dictionary<string, object>
                    {
                        ["name"] = actionOverride.Key
                    };

                    // Convert the list to an array, and only keep key events as this is for the editor.
                    var eventsArr = new List<InputEvent>();

                    foreach (var ie in events)
                    {
                        if (ie is InputEventKey iek)
                        {
                            eventsArr.Add(iek);
                        }
                    }

                    var defaultsArr = new List<InputEvent>();
                    var defaults    = InputMap.Singleton.GetBuiltins()[actionOverride.Key];
                    foreach (var default_input_event in defaults)
                    {
                        defaultsArr.Add(default_input_event);
                    }

                    var same = Shortcut.IsEventArrayEqual(eventsArr, defaultsArr);

                    // Don't save if same as default.
                    if (same)
                    {
                        continue;
                    }

                    actionDict["events"] = eventsArr;
                    actionsArr.Add(actionDict);
                }

                ret = actionsArr;
                return true;
            }

            if (!this.props.TryGetValue(p_name, out var v))
            {
                WARN_PRINT($"EditorSettings::_get - Property not found: {p_name}");
                ret = null;

                return false;
            }
            ret = v.Variant;
            return true;
        }
    }

    private void InitialSet(string name, object value)
    {
        this.Set(name, value);

        var prop = this.props[name];

        prop.Initial         = value;
        prop.HasDefaultValue = true;
    }

    private void LoadDefaults(ConfigFile? extraConfig = null)
    {
        lock (padlock)
        {
            extraConfig ??= new();

            #pragma warning disable IDE1006
            // Sets up the editor setting with a default value and hint PropertyInfo.
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            void EDITOR_SETTING(VariantType type, PropertyHint propertyHint, string name, object defaultValue, string hintString)
            {
                this.InitialSet(name, defaultValue);
                this.hints[name] = new PropertyInfo(type, name, propertyHint, hintString);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            void EDITOR_SETTING_USAGE(VariantType type, PropertyHint propertyHint, string name, object defaultValue, string hintString, PropertyUsageFlags usage)
            {
                this.InitialSet(name, defaultValue);
                this.hints[name] = new PropertyInfo(type, name, propertyHint, hintString, usage);
            }
            #pragma warning restore IDE1006

            /* Languages */
            {
                var langHint = "en";
                var hostLang = OS.Singleton.Locale;

                // Skip locales if Text server lack required features.
                var localesToSkip = new List<string>();
                if (!TextServerManager.Singleton.PrimaryInterface.HasFeature(TextServer.Feature.FEATURE_BIDI_LAYOUT) || !TextServerManager.Singleton.PrimaryInterface.HasFeature(TextServer.Feature.FEATURE_SHAPING))
                {
                    localesToSkip.Add("ar"); // Arabic
                    localesToSkip.Add("fa"); // Persian
                    localesToSkip.Add("ur"); // Urdu
                }
                if (!TextServerManager.Singleton.PrimaryInterface.HasFeature(TextServer.Feature.FEATURE_BIDI_LAYOUT))
                {
                    localesToSkip.Add("he"); // Hebrew
                }
                if (!TextServerManager.Singleton.PrimaryInterface.HasFeature(TextServer.Feature.FEATURE_SHAPING))
                {
                    localesToSkip.Add("bn"); // Bengali
                    localesToSkip.Add("hi"); // Hindi
                    localesToSkip.Add("ml"); // Malayalam
                    localesToSkip.Add("si"); // Sinhala
                    localesToSkip.Add("ta"); // Tamil
                    localesToSkip.Add("te"); // Telugu
                }

                if (localesToSkip.Count != 0)
                {
                    WARN_PRINT("Some locales are not properly supported by selected Text Server and are disabled.");
                }

                var best      = "";
                var bestScore = 0;

                foreach (var locale in this.GetEditorLocales())
                {
                    // Skip locales which we can't render properly (see above comment).
                    // Test against language code without regional variants (e.g. ur_PK).
                    var langCode = locale.Split('_')[0];
                    if (localesToSkip.Contains(langCode))
                    {
                        continue;
                    }

                    langHint += ",";
                    langHint += locale;

                    var score = TranslationServer.Singleton.CompareLocales(hostLang, locale);
                    if (score > 0 && score >= bestScore)
                    {
                        best      = locale;
                        bestScore = score;
                    }
                }

                if (bestScore == 0)
                {
                    best = "en";
                }

                EDITOR_SETTING_USAGE(VariantType.STRING, PropertyHint.PROPERTY_HINT_ENUM, "interface/editor/editor_language", best, langHint, PropertyUsageFlags.PROPERTY_USAGE_DEFAULT | PropertyUsageFlags.PROPERTY_USAGE_RESTART_IF_CHANGED);
            }

            /* Interface */

            // Editor
            // Display what the Auto display scale setting effectively corresponds to.
            var displayScaleHintString = $"Auto ({Math.Round(AutoDisplayScale * 100)}%),75%,100%,125%,150%,175%,200%,Custom";
            EDITOR_SETTING_USAGE(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "interface/editor/display_scale", 0, displayScaleHintString, PropertyUsageFlags.PROPERTY_USAGE_DEFAULT | PropertyUsageFlags.PROPERTY_USAGE_RESTART_IF_CHANGED);

            this.InitialSet("interface/editor/debug/enable_pseudolocalization", false);
            this.SetRestartIfChanged("interface/editor/debug/enable_pseudolocalization", true);

            // Use pseudolocalization in editor.
            EDITOR_SETTING_USAGE(VariantType.BOOL, PropertyHint.PROPERTY_HINT_NONE, "interface/editor/use_embedded_menu", false, "", PropertyUsageFlags.PROPERTY_USAGE_DEFAULT | PropertyUsageFlags.PROPERTY_USAGE_RESTART_IF_CHANGED);
            EDITOR_SETTING_USAGE(VariantType.BOOL, PropertyHint.PROPERTY_HINT_NONE, "interface/editor/expand_to_title", true, "", PropertyUsageFlags.PROPERTY_USAGE_DEFAULT | PropertyUsageFlags.PROPERTY_USAGE_RESTART_IF_CHANGED);

            EDITOR_SETTING_USAGE(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "interface/editor/custom_display_scale", 1.0, "0.5,3,0.01", PropertyUsageFlags.PROPERTY_USAGE_DEFAULT | PropertyUsageFlags.PROPERTY_USAGE_RESTART_IF_CHANGED);
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "interface/editor/main_font_size", 14, "8,48,1");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "interface/editor/code_font_size", 14, "8,48,1");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "interface/editor/code_font_contextual_ligatures", 1, "Enabled,Disable Contextual Alternates (Coding Ligatures),Use Custom OpenType Feature Set");

            this.InitialSet("interface/editor/code_font_custom_opentype_features", "");
            this.InitialSet("interface/editor/code_font_custom_variations", "");

            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "interface/editor/font_antialiasing", 1, "None,Grayscale,LCD Subpixel");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "interface/editor/font_hinting", 0, "Auto (None),None,Light,Normal");
            }
            else
            {
                EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "interface/editor/font_hinting", 0, "Auto (Light),None,Light,Normal");
            }

            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "interface/editor/font_subpixel_positioning", 1, "Disabled,Auto,One Half of a Pixel,One Quarter of a Pixel");

            EDITOR_SETTING(VariantType.STRING, PropertyHint.PROPERTY_HINT_GLOBAL_FILE, "interface/editor/main_font", "", "*.ttf,*.otf,*.woff,*.woff2,*.pfb,*.pfm");
            EDITOR_SETTING(VariantType.STRING, PropertyHint.PROPERTY_HINT_GLOBAL_FILE, "interface/editor/main_font_bold", "", "*.ttf,*.otf,*.woff,*.woff2,*.pfb,*.pfm");
            EDITOR_SETTING(VariantType.STRING, PropertyHint.PROPERTY_HINT_GLOBAL_FILE, "interface/editor/code_font", "", "*.ttf,*.otf,*.woff,*.woff2,*.pfb,*.pfm");
            EDITOR_SETTING_USAGE(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "interface/editor/low_processor_mode_sleep_usec", 6900, "1,100000,1", PropertyUsageFlags.PROPERTY_USAGE_DEFAULT | PropertyUsageFlags.PROPERTY_USAGE_RESTART_IF_CHANGED);
            // Default unfocused usec sleep is for 10 FPS. Allow an unfocused FPS limit
            // as low as 1 FPS for those who really need low power usage (but don't need
            // to preview particles or shaders while the editor is unfocused). With very
            // low FPS limits, the editor can take a small while to become usable after
            // being focused again, so this should be used at the user's discretion.
            EDITOR_SETTING_USAGE(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "interface/editor/unfocused_low_processor_mode_sleep_usec", 100000, "1,1000000,1", PropertyUsageFlags.PROPERTY_USAGE_DEFAULT | PropertyUsageFlags.PROPERTY_USAGE_RESTART_IF_CHANGED);
            this.InitialSet("interface/editor/separate_distraction_mode", false);
            this.InitialSet("interface/editor/automatically_open_screenshots", true);
            EDITOR_SETTING_USAGE(VariantType.BOOL, PropertyHint.PROPERTY_HINT_NONE, "interface/editor/single_window_mode", false, "", PropertyUsageFlags.PROPERTY_USAGE_DEFAULT | PropertyUsageFlags.PROPERTY_USAGE_RESTART_IF_CHANGED);
            this.InitialSet("interface/editor/mouse_extra_buttons_navigate_history", true);
            this.InitialSet("interface/editor/save_each_scene_on_quit", true); // Regression
            EDITOR_SETTING_USAGE(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "interface/editor/accept_dialog_cancel_ok_buttons", 0, $"Auto ({(DisplayServer.Singleton.SwapCancelOk ? "OK First" : "Cancel First")}),Cancel First,OK First", PropertyUsageFlags.PROPERTY_USAGE_DEFAULT | PropertyUsageFlags.PROPERTY_USAGE_RESTART_IF_CHANGED);
            #if DEV_ENABLED
            EDITOR_SETTING(Variant.INT, PropertyHint.PROPERTY_HINT_ENUM, "interface/editor/show_internal_errors_in_toast_notifications", 0, "Auto (Enabled),Enabled,Disabled");
            #else
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "interface/editor/show_internal_errors_in_toast_notifications", 0, "Auto (Disabled),Enabled,Disabled");
            #endif

            // Inspector
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "interface/inspector/max_array_dictionary_items_per_page", 20, "10,100,1");
            EDITOR_SETTING(VariantType.BOOL, PropertyHint.PROPERTY_HINT_NONE, "interface/inspector/show_low_level_opentype_features", false, "");

            // Theme
            EDITOR_SETTING(VariantType.STRING, PropertyHint.PROPERTY_HINT_ENUM, "interface/theme/preset", "Default", "Default,Breeze Dark,Godot 2,Gray,Light,Solarized (Dark),Solarized (Light),Black (OLED),Custom");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "interface/theme/icon_and_font_color", 0, "Auto,Dark,Light");
            EDITOR_SETTING(VariantType.COLOR, PropertyHint.PROPERTY_HINT_NONE, "interface/theme/base_color", new Color(0.2f, 0.23f, 0.31f), "");
            EDITOR_SETTING(VariantType.COLOR, PropertyHint.PROPERTY_HINT_NONE, "interface/theme/accent_color", new Color(0.41f, 0.61f, 0.91f), "");
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "interface/theme/contrast", 0.3, "-1,1,0.01");
            EDITOR_SETTING(VariantType.BOOL, PropertyHint.PROPERTY_HINT_NONE, "interface/theme/draw_extra_borders", false, "");
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "interface/theme/icon_saturation", 1.0, "0,2,0.01");
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "interface/theme/relationship_line_opacity", 0.1, "0.00,1,0.01");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "interface/theme/border_size", 0, "0,2,1");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "interface/theme/corner_radius", 3, "0,6,1");
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "interface/theme/additional_spacing", 0.0, "0,5,0.1");
            EDITOR_SETTING_USAGE(VariantType.STRING, PropertyHint.PROPERTY_HINT_GLOBAL_FILE, "interface/theme/custom_theme", "", "*.res,*.tres,*.theme", PropertyUsageFlags.PROPERTY_USAGE_DEFAULT | PropertyUsageFlags.PROPERTY_USAGE_RESTART_IF_CHANGED);

            // Touchscreen
            var hasTouchscreenUI = DisplayServer.Singleton.IsTouchscreenAvailable;
            EDITOR_SETTING(VariantType.BOOL, PropertyHint.PROPERTY_HINT_NONE, "interface/touchscreen/increase_scrollbar_touch_area", hasTouchscreenUI, "");
            EDITOR_SETTING(VariantType.BOOL, PropertyHint.PROPERTY_HINT_NONE, "interface/touchscreen/enable_long_press_as_right_click", hasTouchscreenUI, "");
            this.SetRestartIfChanged("interface/touchscreen/enable_long_press_as_right_click", true);
            EDITOR_SETTING(VariantType.BOOL, PropertyHint.PROPERTY_HINT_NONE, "interface/touchscreen/enable_pan_and_scale_gestures", hasTouchscreenUI, "");
            this.SetRestartIfChanged("interface/touchscreen/enable_pan_and_scale_gestures", true);

            // Scene tabs
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "interface/scene_tabs/display_close_button", 1, "Never,If Tab Active,Always"); // TabBar::CloseButtonDisplayPolicy
            this.InitialSet("interface/scene_tabs/show_thumbnail_on_hover", true);
            EDITOR_SETTING_USAGE(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "interface/scene_tabs/maximum_width", 350, "0,9999,1", PropertyUsageFlags.PROPERTY_USAGE_DEFAULT);
            this.InitialSet("interface/scene_tabs/show_script_button", false);

            /* Filesystem */

            // External Programs
            EDITOR_SETTING(VariantType.STRING, PropertyHint.PROPERTY_HINT_GLOBAL_FILE, "filesystem/external_programs/raster_image_editor", "", "");
            EDITOR_SETTING(VariantType.STRING, PropertyHint.PROPERTY_HINT_GLOBAL_FILE, "filesystem/external_programs/vector_image_editor", "", "");
            EDITOR_SETTING(VariantType.STRING, PropertyHint.PROPERTY_HINT_GLOBAL_FILE, "filesystem/external_programs/audio_editor", "", "");
            EDITOR_SETTING(VariantType.STRING, PropertyHint.PROPERTY_HINT_GLOBAL_FILE, "filesystem/external_programs/3d_model_editor", "", "");

            // Directories
            EDITOR_SETTING(VariantType.STRING, PropertyHint.PROPERTY_HINT_GLOBAL_DIR, "filesystem/directories/autoscan_project_path", "", "");
            var fsDirDefaultProjectPath = Environment.GetEnvironmentVariable("HOME") ?? OS.Singleton.GetSystemDir(OS.SystemDir.SYSTEM_DIR_DOCUMENTS)!;
            EDITOR_SETTING(VariantType.STRING, PropertyHint.PROPERTY_HINT_GLOBAL_DIR, "filesystem/directories/default_project_path", fsDirDefaultProjectPath, "");

            // On save
            this.InitialSet("filesystem/on_save/compress_binary_resources", true);
            this.InitialSet("filesystem/on_save/safe_save_on_backup_then_rename", true);

            // File dialog
            this.InitialSet("filesystem/file_dialog/show_hidden_files", false);
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "filesystem/file_dialog/display_mode", 0, "Thumbnails,List");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "filesystem/file_dialog/thumbnail_size", 64, "32,128,16");

            /* Docks */

            // SceneTree
            this.InitialSet("docks/scene_tree/start_create_dialog_fully_expanded", false);
            this.InitialSet("docks/scene_tree/auto_expand_to_selected", true);

            // FileSystem
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "docks/filesystem/thumbnail_size", 64, "32,128,16");
            this.InitialSet("docks/filesystem/always_show_folders", true);
            this.InitialSet("docks/filesystem/textfile_extensions", "txt,md,cfg,ini,log,json,yml,yaml,toml");

            // Property editor
            this.InitialSet("docks/property_editor/auto_refresh_interval", 0.2); //update 5 times per second by default
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "docks/property_editor/subresource_hue_tint", 0.75, "0,1,0.01");

            /* Text editor */

            // Theme
            EDITOR_SETTING(VariantType.STRING, PropertyHint.PROPERTY_HINT_ENUM, "text_editor/theme/color_theme", "Default", "Default,Godot 2,Custom");

            // Theme: Highlighting
            this.LoadGodot2TextEditorTheme();

            // Appearance
            // Appearance: Caret
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "text_editor/appearance/caret/type", 0, "Line,Block");
            this.InitialSet("text_editor/appearance/caret/caret_blink", true);
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "text_editor/appearance/caret/caret_blink_interval", 0.5, "0.1,10,0.01");
            this.InitialSet("text_editor/appearance/caret/highlight_current_line", true);
            this.InitialSet("text_editor/appearance/caret/highlight_all_occurrences", true);

            // Appearance: Guidelines
            this.InitialSet("text_editor/appearance/guidelines/show_line_length_guidelines", true);
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "text_editor/appearance/guidelines/line_length_guideline_soft_column", 80, "20,160,1");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "text_editor/appearance/guidelines/line_length_guideline_hard_column", 100, "20,160,1");

            // Appearance: Gutters
            this.InitialSet("text_editor/appearance/gutters/show_line_numbers", true);
            this.InitialSet("text_editor/appearance/gutters/line_numbers_zero_padded", false);
            this.InitialSet("text_editor/appearance/gutters/highlight_type_safe_lines", true);
            this.InitialSet("text_editor/appearance/gutters/show_bookmark_gutter", true);
            this.InitialSet("text_editor/appearance/gutters/show_info_gutter", true);

            // Appearance: Minimap
            this.InitialSet("text_editor/appearance/minimap/show_minimap", true);
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "text_editor/appearance/minimap/minimap_width", 80, "50,250,1");

            // Appearance: Lines
            this.InitialSet("text_editor/appearance/lines/code_folding", true);
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "text_editor/appearance/lines/word_wrap", 0, "None,Boundary");

            // Appearance: Whitespace
            this.InitialSet("text_editor/appearance/whitespace/draw_tabs", true);
            this.InitialSet("text_editor/appearance/whitespace/draw_spaces", false);
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "text_editor/appearance/whitespace/line_spacing", 4, "0,50,1");

            // Behavior
            // Behavior: Navigation
            this.InitialSet("text_editor/behavior/navigation/move_caret_on_right_click", true);
            this.InitialSet("text_editor/behavior/navigation/scroll_past_end_of_file", false);
            this.InitialSet("text_editor/behavior/navigation/smooth_scrolling", true);
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "text_editor/behavior/navigation/v_scroll_speed", 80, "1,10000,1");
            this.InitialSet("text_editor/behavior/navigation/drag_and_drop_selection", true);
            this.InitialSet("text_editor/behavior/navigation/stay_in_script_editor_on_node_selected", true);

            // Behavior: Indent
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "text_editor/behavior/indent/type", 0, "Tabs,Spaces");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "text_editor/behavior/indent/size", 4, "1,64,1"); // size of 0 crashes.
            this.InitialSet("text_editor/behavior/indent/auto_indent", true);

            // Behavior: Files
            this.InitialSet("text_editor/behavior/files/trim_trailing_whitespace_on_save", false);
            this.InitialSet("text_editor/behavior/files/autosave_interval_secs", 0);
            this.InitialSet("text_editor/behavior/files/restore_scripts_on_load", true);
            this.InitialSet("text_editor/behavior/files/convert_indent_on_save", true);
            this.InitialSet("text_editor/behavior/files/auto_reload_scripts_on_external_change", false);

            // Script list
            this.InitialSet("text_editor/script_list/show_members_overview", true);
            this.InitialSet("text_editor/script_list/sort_members_outline_alphabetically", false);

            // Completion
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "text_editor/completion/idle_parse_delay", 2.0, "0.1,10,0.01");
            this.InitialSet("text_editor/completion/auto_brace_complete", true);
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "text_editor/completion/code_complete_delay", 0.3, "0.01,5,0.01");
            this.InitialSet("text_editor/completion/put_callhint_tooltip_below_current_line", true);
            this.InitialSet("text_editor/completion/complete_file_paths", true);
            this.InitialSet("text_editor/completion/add_type_hints", false);
            this.InitialSet("text_editor/completion/use_single_quotes", false);

            // Help
            this.InitialSet("text_editor/help/show_help_index", true);
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "text_editor/help/help_font_size", 16, "8,48,1");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "text_editor/help/help_source_font_size", 15, "8,48,1");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "text_editor/help/help_title_font_size", 23, "8,64,1");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "text_editor/help/class_reference_examples", 0, "GDScript,C#,GDScript and C#");

            /* Editors */

            // GridMap
            this.InitialSet("editors/grid_map/pick_distance", 5000.0);

            // 3D
            EDITOR_SETTING(VariantType.COLOR, PropertyHint.PROPERTY_HINT_NONE, "editors/3d/primary_grid_color", new Color(0.56f, 0.56f, 0.56f, 0.5f), "");
            EDITOR_SETTING(VariantType.COLOR, PropertyHint.PROPERTY_HINT_NONE, "editors/3d/secondary_grid_color", new Color(0.38f, 0.38f, 0.38f, 0.5f), "");

            // Use a similar color to the 2D editor selection.
            EDITOR_SETTING_USAGE(VariantType.COLOR, PropertyHint.PROPERTY_HINT_NONE, "editors/3d/selection_box_color", new Color(1, 0.5f, 0), "", PropertyUsageFlags.PROPERTY_USAGE_DEFAULT | PropertyUsageFlags.PROPERTY_USAGE_RESTART_IF_CHANGED);
            this.InitialSet("editors/3d_gizmos/gizmo_colors/instantiated", new Color(0.7f, 0.7f, 0.7f, 0.6f));
            this.InitialSet("editors/3d_gizmos/gizmo_colors/joint", new Color(0.5f, 0.8f, 1));
            this.InitialSet("editors/3d_gizmos/gizmo_colors/shape", new Color(0.5f, 0.7f, 1));

            // If a line is a multiple of this, it uses the primary grid color.
            // Use a power of 2 value by default as it's more common to use powers of 2 in level design.
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "editors/3d/primary_grid_steps", 8, "1,100,1");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "editors/3d/grid_size", 200, "1,2000,1");

            // Higher values produce graphical artifacts when far away unless View Z-Far
            // is increased significantly more than it really should need to be.
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "editors/3d/grid_division_level_max", 2, "-1,3,1");

            // Lower values produce graphical artifacts regardless of view clipping planes, so limit to -2 as a lower bound.
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "editors/3d/grid_division_level_min", 0, "-2,2,1");

            // -0.2 seems like a sensible default. -1.0 gives Blender-like behavior, 0.5 gives huge grids.
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "editors/3d/grid_division_level_bias", -0.2, "-1.0,0.5,0.1");

            this.InitialSet("editors/3d/grid_xz_plane", true);
            this.InitialSet("editors/3d/grid_xy_plane", false);
            this.InitialSet("editors/3d/grid_yz_plane", false);

            // Use a lower default FOV for the 3D camera compared to the
            // Camera3D node as the 3D viewport doesn't span the whole screen.
            // This means it's technically viewed from a further distance, which warrants a narrower FOV.
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "editors/3d/default_fov", 70.0, "1,179,0.1,degrees");
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "editors/3d/default_z_near", 0.05, "0.01,10,0.01,or_greater,suffix:m");
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "editors/3d/default_z_far", 4000.0, "0.1,4000,0.1,or_greater,suffix:m");

            // 3D: Navigation
            this.InitialSet("editors/3d/navigation/invert_x_axis", false);
            this.InitialSet("editors/3d/navigation/invert_y_axis", false);
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "editors/3d/navigation/navigation_scheme", 0, "Godot,Maya,Modo");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "editors/3d/navigation/zoom_style", 0, "Vertical,Horizontal");

            this.InitialSet("editors/3d/navigation/emulate_numpad", false);
            this.InitialSet("editors/3d/navigation/emulate_3_button_mouse", false);
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "editors/3d/navigation/orbit_modifier", 0, "None,Shift,Alt,Meta,Ctrl");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "editors/3d/navigation/pan_modifier", 1, "None,Shift,Alt,Meta,Ctrl");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "editors/3d/navigation/zoom_modifier", 4, "None,Shift,Alt,Meta,Ctrl");
            this.InitialSet("editors/3d/navigation/warped_mouse_panning", true);

            // 3D: Navigation feel
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "editors/3d/navigation_feel/orbit_sensitivity", 0.25, "0.01,2,0.001");
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "editors/3d/navigation_feel/orbit_inertia", 0.0, "0,1,0.001");
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "editors/3d/navigation_feel/translation_inertia", 0.05, "0,1,0.001");
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "editors/3d/navigation_feel/zoom_inertia", 0.05, "0,1,0.001");

            // 3D: Freelook
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "editors/3d/freelook/freelook_navigation_scheme", 0, "Default,Partially Axis-Locked (id Tech),Fully Axis-Locked (Minecraft)");
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "editors/3d/freelook/freelook_sensitivity", 0.25, "0.01,2,0.001");
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "editors/3d/freelook/freelook_inertia", 0.0, "0,1,0.001");
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "editors/3d/freelook/freelook_base_speed", 5.0, "0,10,0.01");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "editors/3d/freelook/freelook_activation_modifier", 0, "None,Shift,Alt,Meta,Ctrl");
            this.InitialSet("editors/3d/freelook/freelook_speed_zoom_link", false);

            // 2D
            this.InitialSet("editors/2d/grid_color", new Color(1, 1, 1, 0.07f));
            this.InitialSet("editors/2d/guides_color", new Color(0.6f, 0, 0.8f));
            this.InitialSet("editors/2d/smart_snapping_line_color", new Color(0.9f, 0.1f, 0.1f));
            this.InitialSet("editors/2d/bone_width", 5);
            this.InitialSet("editors/2d/bone_color1", new Color(1, 1, 1, 0.7f));
            this.InitialSet("editors/2d/bone_color2", new Color(0.6f, 0.6f, 0.6f, 0.7f));
            this.InitialSet("editors/2d/bone_selected_color", new Color(0.9f, 0.45f, 0.45f, 0.7f));
            this.InitialSet("editors/2d/bone_ik_color", new Color(0.9f, 0.9f, 0.45f, 0.7f));
            this.InitialSet("editors/2d/bone_outline_color", new Color(0.35f, 0.35f, 0.35f, 0.5f));
            this.InitialSet("editors/2d/bone_outline_size", 2);
            this.InitialSet("editors/2d/viewport_border_color", new Color(0.4f, 0.4f, 1, 0.4f));
            this.InitialSet("editors/2d/constrain_editor_view", true);

            // Panning
            // Enum should be in sync with ControlScheme in ViewPanner.
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "editors/panning/2d_editor_panning_scheme", 0, "Scroll Zooms,Scroll Pans");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "editors/panning/sub_editors_panning_scheme", 0, "Scroll Zooms,Scroll Pans");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "editors/panning/animation_editors_panning_scheme", 1, "Scroll Zooms,Scroll Pans");

            this.InitialSet("editors/panning/simple_panning", false);
            this.InitialSet("editors/panning/warped_mouse_panning", true);
            this.InitialSet("editors/panning/2d_editor_pan_speed", 20);

            // Tiles editor
            this.InitialSet("editors/tiles_editor/display_grid", true);
            this.InitialSet("editors/tiles_editor/grid_color", new Color(1, 0.5f, 0.2f, 0.5f));

            // Polygon editor
            this.InitialSet("editors/polygon_editor/point_grab_radius", 8);
            this.InitialSet("editors/polygon_editor/show_previous_outline", true);

            // Animation
            this.InitialSet("editors/animation/autorename_animation_tracks", true);
            this.InitialSet("editors/animation/confirm_insert_track", true);
            this.InitialSet("editors/animation/default_create_bezier_tracks", false);
            this.InitialSet("editors/animation/default_create_reset_tracks", true);
            this.InitialSet("editors/animation/onion_layers_past_color", new Color(1, 0, 0));
            this.InitialSet("editors/animation/onion_layers_future_color", new Color(0, 1, 0));

            // Visual editors
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "editors/visual_editors/minimap_opacity", 0.85, "0.0,1.0,0.01");
            EDITOR_SETTING(VariantType.FLOAT, PropertyHint.PROPERTY_HINT_RANGE, "editors/visual_editors/lines_curvature", 0.5, "0.0,1.0,0.01");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "editors/visual_editors/visual_shader/port_preview_size", 160, "100,400,0.01");

            /* Run */

            // Window placement
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "run/window_placement/rect", 1, "Top Left,Centered,Custom Position,Force Maximized,Force Fullscreen");
            // Keep the enum values in sync with the `DisplayServer::SCREEN_` enum.
            var screenHints = "Same as Editor:-5,Previous Monitor:-4,Next Monitor:-3,Primary Monitor:-2"; // Note: Main Window Screen:-1 is not used for the main window.
            for (var i = 0; i < DisplayServer.Singleton.GetScreenCount(); i++) {
                screenHints += ",Monitor " + (i + 1) + ":" + i;
            }
            this.InitialSet("run/window_placement/rect_custom_position", new Vector2<RealT>());
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "run/window_placement/screen", -5, screenHints);

            // Auto save
            this.InitialSet("run/auto_save/save_before_running", true);

            // Output
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "run/output/font_size", 13, "8,48,1");
            this.InitialSet("run/output/always_clear_output_on_play", true);
            this.InitialSet("run/output/always_open_output_on_play", true);
            this.InitialSet("run/output/always_close_output_on_stop", false);

            /* Network */

            // Debug
            this.InitialSet("network/debug/remote_host", "127.0.0.1"); // Hints provided in setup_network

            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "network/debug/remote_port", 6007, "1,65535,1");

            #region TODO
            // // SSL
            // EDITOR_SETTING_USAGE(Variant.STRING, PropertyHint.PROPERTY_HINT_GLOBAL_FILE, "network/tls/editor_tls_certificates", _SYSTEM_CERTS_PATH, "*.crt,*.pem", PropertyUsageFlags.PROPERTY_USAGE_DEFAULT | PropertyUsageFlags.PROPERTY_USAGE_RESTART_IF_CHANGED);
            #endregion TODO

            // Profiler
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "debugger/profiler_frame_history_size", 3600, "60,10000,1");

            // HTTP Proxy
            this.InitialSet("network/http_proxy/host", "");
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_RANGE, "network/http_proxy/port", 8080, "1,65535,1");

            /* Extra config */

            // TRANSLATORS: Project Manager here refers to the tool used to create/manage Godot projects.
            EDITOR_SETTING(VariantType.INT, PropertyHint.PROPERTY_HINT_ENUM, "project_manager/sorting_order", 0, "Last Edited,Name,Path");

            #if WEB_ENABLED
            // Web platform only supports `gl_compatibility`.
            EDITOR_SETTING(Variant.STRING, PropertyHint.PROPERTY_HINT_NONE, "project_manager/default_renderer", "gl_compatibility", "forward_plus,mobile,gl_compatibility");
            #elif ANDROID_ENABLED
            // Use more suitable rendering method by default.
            EDITOR_SETTING(Variant.STRING, PropertyHint.PROPERTY_HINT_NONE, "project_manager/default_renderer", "mobile", "forward_plus,mobile,gl_compatibility");
            #else
            EDITOR_SETTING(VariantType.STRING, PropertyHint.PROPERTY_HINT_NONE, "project_manager/default_renderer", "forward_plus", "forward_plus,mobile,gl_compatibility");
            #endif

            if (extraConfig.HasSection("init_projects") && extraConfig.HasSectionKey("init_projects", "list"))
            {
                var list = extraConfig.GetValue<List<string>>("init_projects", "list")!;
                foreach (var item in list)
                {
                    var projName = item.Replace("/", "::");
                    GLOBAL_DEF("projects/" + projName, item);
                }
            }

            if (extraConfig.HasSection("presets"))
            {
                extraConfig.GetSectionKeys("presets", out var keys);

                foreach (var key in keys)
                {
                    var val = extraConfig.GetValue("presets", key);
                    GLOBAL_DEF(key, val);
                }
            }
        }
    }

    private void LoadGodot2TextEditorTheme()
    {
        // Godot 2 is only a dark theme; it doesn't have a light theme counterpart.
        this.InitialSet("text_editor/theme/highlighting/symbol_color",                    new Color(0.73f, 0.87f, 1f));
        this.InitialSet("text_editor/theme/highlighting/keyword_color",                   new Color(1, 1, 0.7f));
        this.InitialSet("text_editor/theme/highlighting/control_flow_keyword_color",      new Color(1, 0.85f, 0.7f));
        this.InitialSet("text_editor/theme/highlighting/base_type_color",                 new Color(0.64f, 1, 0.83f));
        this.InitialSet("text_editor/theme/highlighting/engine_type_color",               new Color(0.51f, 0.83f, 1));
        this.InitialSet("text_editor/theme/highlighting/user_type_color",                 new Color(0.42f, 0.67f, 0.93f));
        this.InitialSet("text_editor/theme/highlighting/comment_color",                   new Color(0.4f, 0.4f, 0.4f));
        this.InitialSet("text_editor/theme/highlighting/string_color",                    new Color(0.94f, 0.43f, 0.75f));
        this.InitialSet("text_editor/theme/highlighting/background_color",                new Color(0.13f, 0.12f, 0.15f));
        this.InitialSet("text_editor/theme/highlighting/completion_background_color",     new Color(0.17f, 0.16f, 0.2f));
        this.InitialSet("text_editor/theme/highlighting/completion_selected_color",       new Color(0.26f, 0.26f, 0.27f));
        this.InitialSet("text_editor/theme/highlighting/completion_existing_color",       new Color(0.87f, 0.87f, 0.87f, 0.13f));
        this.InitialSet("text_editor/theme/highlighting/completion_scroll_color",         new Color(1, 1, 1, 0.29f));
        this.InitialSet("text_editor/theme/highlighting/completion_scroll_hovered_color", new Color(1, 1, 1, 0.4f));
        this.InitialSet("text_editor/theme/highlighting/completion_font_color",           new Color(0.67f, 0.67f, 0.67f));
        this.InitialSet("text_editor/theme/highlighting/text_color",                      new Color(0.67f, 0.67f, 0.67f));
        this.InitialSet("text_editor/theme/highlighting/line_number_color",               new Color(0.67f, 0.67f, 0.67f, 0.4f));
        this.InitialSet("text_editor/theme/highlighting/safe_line_number_color",          new Color(0.67f, 0.78f, 0.67f, 0.6f));
        this.InitialSet("text_editor/theme/highlighting/caret_color",                     new Color(0.67f, 0.67f, 0.67f));
        this.InitialSet("text_editor/theme/highlighting/caret_background_color",          new Color(0, 0, 0));
        this.InitialSet("text_editor/theme/highlighting/text_selected_color",             new Color(0, 0, 0, 0));
        this.InitialSet("text_editor/theme/highlighting/selection_color",                 new Color(0.41f, 0.61f, 0.91f, 0.35f));
        this.InitialSet("text_editor/theme/highlighting/brace_mismatch_color",            new Color(1, 0.2f, 0.2f));
        this.InitialSet("text_editor/theme/highlighting/current_line_color",              new Color(0.3f, 0.5f, 0.8f, 0.15f));
        this.InitialSet("text_editor/theme/highlighting/line_length_guideline_color",     new Color(0.3f, 0.5f, 0.8f, 0.1f));
        this.InitialSet("text_editor/theme/highlighting/word_highlighted_color",          new Color(0.8f, 0.9f, 0.9f, 0.15f));
        this.InitialSet("text_editor/theme/highlighting/number_color",                    new Color(0.92f, 0.58f, 0.2f));
        this.InitialSet("text_editor/theme/highlighting/function_color",                  new Color(0.4f, 0.64f, 0.81f));
        this.InitialSet("text_editor/theme/highlighting/member_variable_color",           new Color(0.9f, 0.31f, 0.35f));
        this.InitialSet("text_editor/theme/highlighting/mark_color",                      new Color(1, 0.4f, 0.4f, 0.4f));
        this.InitialSet("text_editor/theme/highlighting/bookmark_color",                  new Color(0.08f, 0.49f, 0.98f));
        this.InitialSet("text_editor/theme/highlighting/breakpoint_color",                new Color(0.9f, 0.29f, 0.3f));
        this.InitialSet("text_editor/theme/highlighting/executing_line_color",            new Color(0.98f, 0.89f, 0.27f));
        this.InitialSet("text_editor/theme/highlighting/code_folding_color",              new Color(0.8f, 0.8f, 0.8f, 0.83f));
        this.InitialSet("text_editor/theme/highlighting/search_result_color",             new Color(0.05f, 0.25f, 0.05f, 1));
        this.InitialSet("text_editor/theme/highlighting/search_result_border_color",      new Color(0.41f, 0.61f, 0.91f, 0.38f));
    }

    private bool Set(string name, object? value)
    {
        lock (padlock)
        {
            var changed = this.SetOnly(name, value);
            if (changed)
            {
                this.changedSettings.Add(name);

                SettingsChanged?.Invoke();

            }
            return true;
        }
    }

    private bool SetOnly(string name, object? value)
    {
        lock (padlock)
        {
            if (value != null)
            {
                if (name == "shortcuts")
                {
                    var arr = (Array)value;
                    for (var i = 0; i < arr.Length; i++)
                    {
                        var dict         = (Dictionary<string, object>)arr.GetValue(i)!;
                        var shortcutName = (string)dict["name"];

                        var shortcutEvents = (InputEvent[])dict["shortcuts"];

                        var sc = new Shortcut
                        {
                            Events = shortcutEvents
                        };

                        this.AddShortcut(shortcutName, sc);
                    }

                    return false;
                }
                else if (name == "builtin_action_overrides")
                {
                    var actionsArr = (Array)value;
                    for (var i = 0; i < actionsArr.Length; i++)
                    {
                        var actionDict = (Dictionary<string, object>)actionsArr.GetValue(i)!;

                        var actionName = (string)actionDict["name"];
                        var events     = (InputEvent[])actionDict["events"];

                        var im = InputMap.Singleton;
                        im.ActionEraseEvents(actionName);

                        if (!this.builtinActionOverrides.TryGetValue(actionName, out var overrides))
                        {
                            this.builtinActionOverrides[actionName] = overrides = new();
                        }

                        overrides.Clear();
                        for (var evIdx = 0; evIdx < events.Length; evIdx++)
                        {
                            im.ActionAddEvent(actionName, events[evIdx]);
                            overrides.Add(events[evIdx]);
                        }
                    }
                    return false;
                }
            }

            var changed = false;

            if (value == null)
            {
                changed = this.props.Remove(name);
            }
            else
            {
                if (this.props.TryGetValue(name, out var container))
                {
                    if (value != container.Variant)
                    {
                        container.Variant = value;
                        changed = true;
                    }
                }
                else
                {
                    this.props[name] = new VariantContainer(value, this.lastOrder++);
                    changed = true;
                }

                if (this.saveChangedSetting)
                {
                    if (!this.props[name].Save)
                    {
                        this.props[name].Save = true;
                        changed = true;
                    }
                }
            }

            return changed;
        }
    }

    public void AddPropertyHint(PropertyInfo hint)
    {
        lock (padlock)
        {
            this.hints[hint.Name] = hint;
        }
    }

    public void AddShortcut(string name, Shortcut shortcut) => throw new NotImplementedException();

    #pragma warning disable CA1822
    public IEnumerable<string> GetEditorLocales()
    {
        yield return "en";
        // TODO
    }
    #pragma warning restore CA1822

    public T? GetSetting<T>(string setting)
    {
        lock (padlock)
        {
            var type = typeof(T);

            return this.props.TryGetValue(setting, out var entry) && entry.Variant != default
                ? type.IsEnum || type == typeof(object)
                    ? (T?)entry.Variant
                    : (T?)Convert.ChangeType(entry.Variant, type)
                : default;
        }
    }

    public bool HasSetting(string setting)
    {
        lock (padlock)
        {
            return this.props.ContainsKey(setting);
        }
    }

    public void ListTextEditorThemes()
    {
        var themes = "Default,Godot 2,Custom";

        var d = new DirectoryInfo(EditorPaths.Singleton.TextEditorThemesDir);
        if (d != null)
        {
            var customThemes = new List<string>();

            foreach (var file in d.GetFiles())
            {
                if (file.Extension == "tet" && !IsDefaultTextEditorTheme(file.DirectoryName!))
                {
                    customThemes.Add(file.Name);
                }
            }

            customThemes.Sort();

            themes += string.Join(',', customThemes);
        }

        this.AddPropertyHint(new PropertyInfo(VariantType.STRING, "text_editor/theme/color_theme", PropertyHint.PROPERTY_HINT_ENUM, themes));
    }

    public void LoadTextEditorTheme() => throw new NotImplementedException();

    public void SetInitialValue<T>(string setting, T value, bool updateCurrent = false)
    {
        lock (padlock)
        {
            if (this.props.TryGetValue(setting, out var prop))
            {
                prop.Initial         = value;
                prop.HasDefaultValue = true;

                if (updateCurrent)
                {
                    this.Set(setting, value);
                }
            }
        }
    }

    public void SetManually<T>(string setting, T value, bool emitSignal = false)
    {
        if (emitSignal)
        {
            this.Set(setting, value);
        }
        else
        {
            this.SetOnly(setting, value);
        }
    }

    public void SetRestartIfChanged(string setting, bool restart)
    {
        lock (padlock)
        {
            if (this.props.TryGetValue(setting, out var value))
            {
                value.RestartIfChanged = restart;
            }
        }
    }

    public void SetSetting<T>(string setting, T value)
    {
        lock (padlock)
        {
            this.Set(setting, value);
        }
    }

    public static class Macros
    {
        public static T? EDITOR_GET<T>(string setting) =>
            ERR_FAIL_COND_V(Singleton == null || !Singleton.HasSetting(setting))
                ? default
                : Singleton!.GetSetting<T>(setting);
    }
}
