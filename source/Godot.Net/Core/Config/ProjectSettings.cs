namespace Godot.Net.Core.Config;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Godot.Net.Core.IO;
using Godot.Net.Core.Math;
using Godot.Net.Extensions;

using Error = Error.Error;
using OS    = OS.OS;

#pragma warning disable IDE0052, CS0414 // TODO Remove

public partial class ProjectSettings
{
    private const int    NO_BUILTIN_ORDER_BASE        = 1 << 16;
    private const string PROJECT_DATA_DIR_NAME_SUFFIX = "godot_net";

    private static readonly object padlock = new();

    private static ProjectSettings? singleton;

    public static ProjectSettings Singleton => singleton ?? throw new NullReferenceException();

    private readonly Dictionary<string, AutoloadInfo>                        autoloads        = new();
    private readonly HashSet<string>                                         customFeatures   = new();
    private readonly Dictionary<string, PropertyInfo>                        customPropInfo   = new();
    private readonly Dictionary<string, List<(string First, string Second)>> featureOverrides = new();
    private readonly Dictionary<string, VariantContainer>                    props            = new();

    private int  lastBuiltinOrder;
    private int  lastOrder = NO_BUILTIN_ORDER_BASE;
    private bool usingDatapack;

    public string ImportedFilesPath  => throw new NotImplementedException();
    public bool   IsProjectLoaded    { get; private set; }
    public string ProjectDataDirName { get; private set; } = "";
    public string ProjectDataPath    => $"res://{this.ProjectDataDirName}";
    public string ResourcePath       { get; private set; } = "";

    public ProjectSettings()
    {
        // Initialization of engine variables should be done in the setup() method,
        // so that the values can be overridden from project.godot or project.binary.

        CRASH_COND_MSG(singleton != null, "Instantiating a new ProjectSettings singleton is not supported.");
        singleton = this;

        GLOBAL_DEF_BASIC("application/config/name", "");
        GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.DICTIONARY, "application/config/name_localized", PropertyHint.PROPERTY_HINT_LOCALIZABLE_STRING), new Dictionary<string, object>());
        GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.STRING,     "application/config/description",    PropertyHint.PROPERTY_HINT_MULTILINE_TEXT), "");
        GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.STRING,     "application/run/main_scene",        PropertyHint.PROPERTY_HINT_FILE, "*.tscn,*.scn,*.res"), "");
        GLOBAL_DEF("application/run/disable_stdout", false);
        GLOBAL_DEF("application/run/disable_stderr", false);
        GLOBAL_DEF_RST("application/config/use_hidden_project_data_directory", true);
        GLOBAL_DEF("application/config/use_custom_user_dir",       false);
        GLOBAL_DEF("application/config/custom_user_dir_name",      "");
        GLOBAL_DEF("application/config/project_settings_override", "");

        // The default window size is tuned to:
        // - Have a 16:9 aspect ratio,
        // - Have both dimensions divisible by 8 to better play along with video recording,
        // - Be displayable correctly in windowed mode on a 1366×768 display (tested on Windows 10 with default settings).
        GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.INT, "display/window/size/viewport_width",  PropertyHint.PROPERTY_HINT_RANGE, "0,7680,1,or_greater"), 1152); // 8K resolution
        GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.INT, "display/window/size/viewport_height", PropertyHint.PROPERTY_HINT_RANGE, "0,4320,1,or_greater"), 648); // 8K resolution

        GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.INT, "display/window/size/mode", PropertyHint.PROPERTY_HINT_ENUM, "Windowed,Minimized,Maximized,Fullscreen,Exclusive Fullscreen"), 0);

        // Keep the enum values in sync with the `DisplayServer::SCREEN_` enum.
        GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.INT,      "display/window/size/initial_position_type", PropertyHint.PROPERTY_HINT_ENUM, "Absolute,Primary Screen Center,Other Screen Center"), 1);
        GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.VECTOR2I, "display/window/size/initial_position"), new Vector2<int>());
        GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.INT,      "display/window/size/initial_screen", PropertyHint.PROPERTY_HINT_RANGE, "0,64,1,or_greater"), 0);

        GLOBAL_DEF_BASIC("display/window/size/resizable", true);
        GLOBAL_DEF_BASIC("display/window/size/borderless", false);
        GLOBAL_DEF("display/window/size/always_on_top",   false);
        GLOBAL_DEF("display/window/size/transparent",     false);
        GLOBAL_DEF("display/window/size/extend_to_title", false);
        GLOBAL_DEF("display/window/size/no_focus",        false);

        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "display/window/size/window_width_override",  PropertyHint.PROPERTY_HINT_RANGE, "0,7680,1,or_greater"), 0); // 8K resolution
        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "display/window/size/window_height_override", PropertyHint.PROPERTY_HINT_RANGE, "0,4320,1,or_greater"), 0); // 8K resolution

        GLOBAL_DEF("display/window/energy_saving/keep_screen_on", true);
        GLOBAL_DEF("display/window/energy_saving/keep_screen_on.editor", false);

        GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.STRING, "audio/buses/default_bus_layout", PropertyHint.PROPERTY_HINT_FILE, "*.tres"), "res://default_bus_layout.tres");
        GLOBAL_DEF_RST(new PropertyInfo(VariantType.FLOAT, "audio/general/2d_panning_strength", PropertyHint.PROPERTY_HINT_RANGE, "0,2,0.01"), 0.5f);
        GLOBAL_DEF_RST(new PropertyInfo(VariantType.FLOAT, "audio/general/3d_panning_strength", PropertyHint.PROPERTY_HINT_RANGE, "0,2,0.01"), 0.5f);

        var extensions = new List<string>
        {
            "cs",
            "godotshader"
        };

        GLOBAL_DEF("editor/run/main_run_args", "");

        GLOBAL_DEF(new PropertyInfo(VariantType.PACKED_STRING_ARRAY, "editor/script/search_in_file_extensions"), extensions);
        GLOBAL_DEF(new PropertyInfo(VariantType.STRING,              "editor/script/templates_search_path", PropertyHint.PROPERTY_HINT_DIR), "res://script_templates");

        // For correct doc generation.
        GLOBAL_DEF("editor/naming/default_signal_callback_name",         "_on_{node_name}_{signal_name}");
        GLOBAL_DEF("editor/naming/default_signal_callback_to_self_name", "_on_{signal_name}");

        // TODO this.AddBuiltinInputMap();

        // Keep the enum values in sync with the `DisplayServer::ScreenOrientation` enum.
        this.customPropInfo["display/window/handheld/orientation"] = new PropertyInfo(VariantType.INT, "display/window/handheld/orientation", PropertyHint.PROPERTY_HINT_ENUM, "Landscape,Portrait,Reverse Landscape,Reverse Portrait,Sensor Landscape,Sensor Portrait,Sensor");
        // Keep the enum values in sync with the `DisplayServer::VSyncMode` enum.
        this.customPropInfo["display/window/vsync/vsync_mode"]       = new PropertyInfo(VariantType.INT, "display/window/vsync/vsync_mode", PropertyHint.PROPERTY_HINT_ENUM, "Disabled,Enabled,Adaptive,Mailbox");
        this.customPropInfo["rendering/driver/threads/thread_model"] = new PropertyInfo(VariantType.INT, "rendering/driver/threads/thread_model", PropertyHint.PROPERTY_HINT_ENUM, "Single-Unsafe,Single-Safe,Multi-Threaded");

        GLOBAL_DEF("physics/2d/run_on_separate_thread", false);
        GLOBAL_DEF("physics/3d/run_on_separate_thread", false);

        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "debug/settings/profiler/max_functions", PropertyHint.PROPERTY_HINT_RANGE, "128,65535,1"), 16384);

        GLOBAL_DEF(new PropertyInfo(VariantType.BOOL, "compression/formats/zstd/long_distance_matching"), Compression.ZstdLongDistanceMatching);
        GLOBAL_DEF(new PropertyInfo(VariantType.INT,  "compression/formats/zstd/compression_level", PropertyHint.PROPERTY_HINT_RANGE, "1,22,1"), Compression.ZstdLevel);
        GLOBAL_DEF(new PropertyInfo(VariantType.INT,  "compression/formats/zstd/window_log_size",   PropertyHint.PROPERTY_HINT_RANGE, "10,30,1"), Compression.ZstdWindowLogSize);
        GLOBAL_DEF(new PropertyInfo(VariantType.INT,  "compression/formats/zlib/compression_level", PropertyHint.PROPERTY_HINT_RANGE, "-1,9,1"), Compression.ZlibLevel);
        GLOBAL_DEF(new PropertyInfo(VariantType.INT,  "compression/formats/gzip/compression_level", PropertyHint.PROPERTY_HINT_RANGE, "-1,9,1"), Compression.GzipLevel);

        GLOBAL_DEF("debug/settings/crash_handler/message",        "Please include this when reporting the bug to the project developer.");
        GLOBAL_DEF("debug/settings/crash_handler/message.editor", "Please include this when reporting the bug on: https://github.com/godotengine/godot/issues");
        GLOBAL_DEF_RST(new PropertyInfo(VariantType.INT, "rendering/occlusion_culling/bvh_build_quality", PropertyHint.PROPERTY_HINT_ENUM, "Low,Medium,High"), 2);
        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "memory/limits/multithreaded_server/rid_pool_prealloc", PropertyHint.PROPERTY_HINT_RANGE, "0,500,1"), 60); // No negative and limit to 500 due to crashes.
        GLOBAL_DEF_RST("internationalization/rendering/force_right_to_left_layout_direction", false);

        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "gui/timers/incremental_search_max_interval_msec", PropertyHint.PROPERTY_HINT_RANGE, "0,10000,1,or_greater"), 2000);

        GLOBAL_DEF("rendering/rendering_device/staging_buffer/block_size_kb",                 256);
        GLOBAL_DEF("rendering/rendering_device/staging_buffer/max_size_mb",                   128);
        GLOBAL_DEF("rendering/rendering_device/staging_buffer/texture_upload_region_size_px", 64);
        GLOBAL_DEF("rendering/rendering_device/vulkan/max_descriptors_per_pool",              64);

        // These properties will not show up in the dialog nor in the documentation. If you want to exclude whole groups, see _get_property_list() method.
        GLOBAL_DEF_INTERNAL("application/config/features",                    Array.Empty<string>());
        GLOBAL_DEF_INTERNAL("internationalization/locale/translation_remaps", Array.Empty<string>());
        GLOBAL_DEF_INTERNAL("internationalization/locale/translations",       Array.Empty<string>());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void SetInitialValue(VariantContainer container, object? value) =>
        // Duplicate so that if value is array or dictionary, changing the setting will not change the stored initial value.
        container.Variant = value?.Clone();

    private bool InternalSet(string name, object? value)
    {
        lock (padlock)
        {

            if (value == null)
            {
                this.props.Remove(name);
                if (name.StartsWith("autoload/"))
                {
                    var nodeName = name.Split("/")[1];
                    if (this.autoloads.ContainsKey(nodeName))
                    {
                        this.RemoveAutoload(nodeName);
                    }
                }
            }
            else
            {
                if (name == "_custom_features")
                {
                    foreach (var feature in ((string)value).Split(","))
                    {
                        this.customFeatures.Add(feature);
                    }

                    return true;
                }

                {
                    // Feature overrides.
                    var dot = name.IndexOf(".");
                    if (dot != -1)
                    {
                        var s = name.Split(".");

                        for (var i = 1; i < s.Length; i++)
                        {
                            var feature         = s[i].Trim();
                            var featureOverride = (feature, name);

                            if (!this.featureOverrides.TryGetValue(s[0], out var overrides))
                            {
                                this.featureOverrides[s[0]] = overrides = new();
                            }

                            overrides.Add(featureOverride);
                        }
                    }
                }

                if (this.props.TryGetValue(name, out var container))
                {
                    container.Variant = value;
                }
                else
                {
                    this.props[name] = new VariantContainer { Variant = value, Order = this.lastOrder++ };
                }

                if (name.StartsWith("autoload/"))
                {
                    var node_name = name.Split("/")[1];

                    var autoload = new AutoloadInfo
                    {
                        Name = node_name
                    };

                    var path = (string)value;

                    if (path.StartsWith("*"))
                    {
                        autoload.IsSingleton = true;
                        autoload.Path = path[1..];
                    }
                    else
                    {
                        autoload.Path = path;
                    }

                    this.AddAutoload(autoload);
                }
            }

            return true;
        }
    }

    private Error InternalSetup(string path, string? mainPack, bool upwards, bool ignoreOverride)
    {
        if (!string.IsNullOrEmpty(OS.Singleton.ResourceDir))
        {
            // OS will call ProjectSettings->get_resource_path which will be empty if not overridden!
            // If the OS would rather use a specific location, then it will not be empty.
            this.ResourcePath = OS.Singleton.ResourceDir;
            if (!string.IsNullOrEmpty(this.ResourcePath) && this.ResourcePath[^1] == '/')
            {
                this.ResourcePath = this.ResourcePath[..^1]; // Chop end.
            }
        }

        #region TODO
        // // If looking for files in a network client, use it directly

        // if (FileAccessNetworkClient::get_singleton())
        // {
        //     Error err = _load_settings_text_or_binary("res://project.godot", "res://project.binary");
        //     if (err == OK && !p_ignore_override)
        //     {
        //         // Optional, we don't mind if it fails
        //         _load_settings_text("res://override.cfg");
        //     }
        //     return err;
        // }
        #endregion TODO

        // Attempt with a user-defined main pack first

        var found = false;
        Error err;

        if (!string.IsNullOrEmpty(mainPack))
        {
            var ok = this.LoadResourcePack(mainPack);
            if (ERR_FAIL_COND_V_MSG(!ok, $"Cannot open resource pack '{mainPack}'."))
            {
                return Error.ERR_CANT_OPEN;
            }

            err = this.LoadSettingsTextOrBinary("res://project.godot", "res://project.binary");
            if (err == Error.OK && !ignoreOverride)
            {
                // Load override from location of the main pack
                // Optional, we don't mind if it fails
                this.LoadSettingsText(Path.Join(Path.GetDirectoryName(mainPack), "override.cfg"));
            }
            return err;
        }

        var execPath = Environment.ProcessPath;

        if (!string.IsNullOrEmpty(execPath))
        {
            // We do several tests sequentially until one succeeds to find a PCK,
            // and if so, we attempt loading it at the end.

            // Attempt with PCK bundled into executable.
            found = this.LoadResourcePack(execPath);

            // Attempt with exec_name.pck.
            // (This is the usual case when distributing a Godot game.)
            var execDir      = Path.GetDirectoryName(execPath)!;
            var execFilename = Path.GetFileName(execPath)!;
            var execBasename = Path.GetFileNameWithoutExtension(execPath)!;

            // Based on the OS, it can be the exec path + '.pck' (Linux w/o extension, macOS in .app bundle)
            // or the exec path's basename + '.pck' (Windows).
            // We need to test both possibilities as extensions for Linux binaries are optional
            // (so both 'mygame.bin' and 'mygame' should be able to find 'mygame.pck').


            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && !found)
            {
                // Attempt to load PCK from macOS .app bundle resources.
                found = this.LoadResourcePack(Path.Join(OS.Singleton.BundleResourceDir, execBasename + ".pck")) || this.LoadResourcePack(Path.Join(OS.Singleton.BundleResourceDir, execFilename + ".pck"));
            }

            if (!found)
            {
                // Try to load data pack at the location of the executable.
                // As mentioned above, we have two potential names to attempt.
                found = this.LoadResourcePack(Path.Join(execDir, execBasename + ".pck")) || this.LoadResourcePack(Path.Join(execDir, execFilename + ".pck"));
            }

            if (!found)
            {
                // If we couldn't find them next to the executable, we attempt
                // the current working directory. Same story, two tests.
                found = this.LoadResourcePack(execBasename + ".pck") || this.LoadResourcePack(execFilename + ".pck");
            }

            // If we opened our package, try and load our project.
            if (found)
            {
                err = this.LoadSettingsTextOrBinary("res://project.godot", "res://project.binary");
                if (err == Error.OK && !ignoreOverride)
                {
                    // Load overrides from the PCK and the executable location.
                    // Optional, we don't mind if either fails.
                    this.LoadSettingsText("res://override.cfg");
                    this.LoadSettingsText(Path.Join(Path.GetDirectoryName(execPath), "override.cfg"));
                }
                return err;
            }
        }

        // Try to use the filesystem for files, according to OS.
        // (Only Android -when reading from pck- and iOS use this.)

        if (!string.IsNullOrEmpty(OS.Singleton.ResourceDir))
        {
            err = this.LoadSettingsTextOrBinary("res://project.godot", "res://project.binary");
            if (err == Error.OK && !ignoreOverride)
            {
                // Optional, we don't mind if it fails.
                this.LoadSettingsText("res://override.cfg");
            }
            return err;
        }

        // Nothing was found, try to find a project file in provided path (`p_path`)
        // or, if requested (`p_upwards`) in parent directories.

        var currentDir = Path.GetFullPath(path);

        while (true)
        {
            // Set the resource path early so things can be resolved when loading.
            this.ResourcePath = currentDir;
            this.ResourcePath = this.ResourcePath.Replace("\\", "/"); // Windows path to Unix path just in case.
            err = this.LoadSettingsTextOrBinary(Path.Join(currentDir, "project.godot"), Path.Join(currentDir, "project.binary"));
            if (err == Error.OK && !ignoreOverride)
            {
                // Optional, we don't mind if it fails.
                this.LoadSettingsText(Path.Join(currentDir, "override.cfg"));
                found = true;
                break;
            }

            if (upwards)
            {
                // Try to load settings ascending through parent directories
                var parentDir = Path.GetDirectoryName(currentDir);
                if (parentDir == currentDir)
                {
                    break; // not doing anything useful
                }
                currentDir = parentDir!;
            }
            else
            {
                break;
            }
        }

        if (!found)
        {
            return err;
        }

        if (this.ResourcePath.Length > 0 && this.ResourcePath[^1] == '/')
        {
            this.ResourcePath = this.ResourcePath[..^1]; // Chop end.
        }

        return Error.OK;
    }

    private bool LoadResourcePack(string pack, bool replaceFiles = default, ulong offset = default)
    {
        if (PackedData.Singleton.IsDisabled)
        {
            return false;
        }

        var ok = PackedData.Singleton.AddPack(pack, replaceFiles, offset) == Error.OK;

        if (!ok)
        {
            return false;
        }

        //if data.pck is found, all directory access will be from here
        this.usingDatapack = true;

        return true;
    }

    #pragma warning disable CA1822 // TODO Remove
    private Error LoadSettingsBinary(string path)
    {
        if (!File.Exists(path))
        {
            return Error.ERR_FILE_NOT_FOUND;
        }

        #region TODO
        // uint8_t hdr[4];
        // f->get_buffer(hdr, 4);
        // ERR_FAIL_COND_V_MSG((hdr[0] != 'E' || hdr[1] != 'C' || hdr[2] != 'F' || hdr[3] != 'G'), ERR_FILE_CORRUPT, "Corrupted header in binary project.binary (not ECFG).");

        // uint32_t count = f->get_32();

        // for (uint32_t i = 0; i < count; i++)
        // {
        //     uint32_t slen = f->get_32();
        //     CharString cs;
        //     cs.resize(slen + 1);
        //     cs[slen] = 0;
        //     f->get_buffer((uint8_t*)cs.ptr(), slen);
        //     String key;
        //     key.parse_utf8(cs.ptr());

        //     uint32_t vlen = f->get_32();
        //     Vector<uint8_t> d;
        //     d.resize(vlen);
        //     f->get_buffer(d.ptrw(), vlen);
        //     Variant value;
        //     err = decode_variant(value, d.ptr(), d.size(), nullptr, true);
        //     ERR_CONTINUE_MSG(err != OK, "Error decoding property: " + key + ".");
        //     set(key, value);
        // }
        #endregion TODO

        return Error.OK;
    }

    private Error LoadSettingsText(string path)
    {
        if (!File.Exists(path))
        {
            // FIXME: Above 'err' error code is ERR_FILE_CANT_OPEN if the file is missing
            // This needs to be streamlined if we want decent error reporting
            return Error.ERR_FILE_NOT_FOUND;
        }

        #region TODO
        // VariantParser::StreamFile stream;
        // stream.f = f;

        // String assign;
        // Variant value;
        // VariantParser::Tag next_tag;

        // int lines = 0;
        // String error_text;
        // String section;
        // int config_version = 0;

        // while (true) {
        //     assign = Variant();
        //     next_tag.fields.clear();
        //     next_tag.name = String();

        //     err = VariantParser::parse_tag_assign_eof(&stream, lines, error_text, next_tag, assign, value, nullptr, true);
        //     if (err == ERR_FILE_EOF) {
        //         // If we're loading a project.godot from source code, we can operate some
        //         // ProjectSettings conversions if need be.
        //         _convert_to_last_version(config_version);
        //         last_save_time = FileAccess::get_modified_time(get_resource_path().path_join("project.godot"));
        //         return OK;
        //     }
        //     ERR_FAIL_COND_V_MSG(err != OK, err, "Error parsing " + p_path + " at line " + itos(lines) + ": " + error_text + " File might be corrupted.");

        //     if (!assign.is_empty()) {
        //         if (section.is_empty() && assign == "config_version") {
        //             config_version = value;
        //             ERR_FAIL_COND_V_MSG(config_version > CONFIG_VERSION, ERR_FILE_CANT_OPEN, vformat("Can't open project at '%s', its `config_version` (%d) is from a more recent and incompatible version of the engine. Expected config version: %d.", p_path, config_version, CONFIG_VERSION));
        //         } else {
        //             if (section.is_empty()) {
        //                 set(assign, value);
        //             } else {
        //                 set(section + "/" + assign, value);
        //             }
        //         }
        //     } else if (!next_tag.name.is_empty()) {
        //         section = next_tag.name;
        //     }
        // }
        #endregion TODO

        return default;
    }
    #pragma warning restore CA1822 // TODO Remove

    private Error LoadSettingsTextOrBinary(string textPath, string binPath)
    {
        // Attempt first to load the binary project.godot file.
        var err = this.LoadSettingsBinary(binPath);

        if (err == Error.OK)
        {
            return err;
        }
        else if (err != Error.ERR_FILE_NOT_FOUND)
        {
            // If the file exists but can't be loaded, we want to know it.
            ERR_PRINT($"Couldn't load file '{binPath}', error code {err}.");
        }

        // Fallback to text-based project.godot file if binary was not found.
        err = this.LoadSettingsText(textPath);

        if (err == Error.OK)
        {
            return err;
        }
        else if (err != Error.ERR_FILE_NOT_FOUND)
        {
            ERR_PRINT($"Couldn't load file '{textPath}', error code {err}.");
        }

        return err;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void SetBuiltinOrder(VariantContainer container)
    {
        if (container.Order >= NO_BUILTIN_ORDER_BASE)
        {
            container.Order = this.lastBuiltinOrder++;
        }
    }

    public void AddAutoload(AutoloadInfo autoload) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public object? Get(string key) => this.props.TryGetValue(key, out var value) ? value.Initial : default;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public T? Get<T>(string key) => this.Get(key) is T value ? value : default;

    public T? GetSettingWithOverride<T>(string name)
    {
        lock (padlock)
        {
            if (this.featureOverrides.TryGetValue(name, out var overrides))
            {
                foreach (var (first, second) in overrides)
                {
                    if (OS.Singleton.HasFeature(first))
                    {
                        // Custom features are checked in OS.has_feature() already. No need to check twice.
                        if (this.props.ContainsKey(second))
                        {
                            name = second;
                            break;
                        }
                    }
                }
            }

            if (this.props.TryGetValue(name, out var value))
            {
                var type = typeof(T);

                return type.IsEnum || type == typeof(object)
                    ? (T?)value.Variant
                    : (T?)Convert.ChangeType(value?.Variant, type);
            }

            WARN_PRINT($"Property not found: {name}");
            return default;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool HasCustomFeature(string feature) =>
        this.customFeatures.Contains(feature);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool HasSetting(string key)
    {
        lock (padlock)
        {
            return this.props.ContainsKey(key);
        }
    }

    public void RemoveAutoload(string autoload) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Set(string name, object? value) =>
        this.InternalSet(name, value);

    public void SetAsBasic(string name, bool basic)
    {
        if (ERR_FAIL_COND_MSG(!this.props.TryGetValue(name, out var prop), $"Request for nonexistent project setting: {name}."))
        {
            return;
        }

        prop!.Basic = basic;
    }

    public void SetAsInternal(string name, bool @internal)
    {
        if (ERR_FAIL_COND_MSG(!this.props.TryGetValue(name, out var prop), $"Request for nonexistent project setting: {name}."))
        {
            return;
        }

        prop!.Internal = @internal;
    }

    public void SetBuiltinOrder(string name)
    {
        if (ERR_FAIL_COND_MSG(!this.props.TryGetValue(name, out var variant), $"Request for nonexistent project setting: {name}."))
        {
            return;
        }

        this.SetBuiltinOrder(variant!);
    }

    public void SetCustomPropertyInfo(PropertyInfo info)
    {
        if (ERR_FAIL_COND(!this.props.ContainsKey(info.Name)))
        {
            return;
        }

        this.customPropInfo[info.Name] = info;
    }

    public void SetIgnoreValueInDocs(string name, bool ignore)
    {
        if (ERR_FAIL_COND_MSG(!this.props.TryGetValue(name, out var prop), $"Request for nonexistent project setting: {name}."))
        {
            return;
        }

        prop!.IgnoreValueInDocs = ignore;
    }

    public void SetInitialValue(string name, object? value)
    {
        if (ERR_FAIL_COND_MSG(!this.props.TryGetValue(name, out var prop), $"Request for nonexistent project setting: {name}."))
        {
            return;
        }


        SetInitialValue(prop!, value);
    }

    public void SetRestartIfChanged(string name, bool restart)
    {
        if (ERR_FAIL_COND_MSG(!this.props.TryGetValue(name, out var prop), $"Request for nonexistent project setting: {name}."))
        {
            return;
        }

        prop!.RestartIfChanged = restart;
    }

    public Error Setup(string path, string? mainPack, bool upwards, bool ignoreOverride)
    {
        var err = this.InternalSetup(path, mainPack, upwards, ignoreOverride);

        if (err == Error.OK)
        {
            var customSettings = GLOBAL_GET<string>("application/config/project_settings_override");

            if (!string.IsNullOrEmpty(customSettings))
            {
                this.LoadSettingsText(customSettings);
            }
        }

        // Updating the default value after the project settings have loaded.
        var useHiddenDirectory = GLOBAL_GET<bool>("application/config/use_hidden_project_data_directory");
        this.ProjectDataDirName = (useHiddenDirectory ? "." : "") + PROJECT_DATA_DIR_NAME_SUFFIX;

        // Using GLOBAL_GET on every block for compressing can be slow, so assigning here.
        Compression.ZstdLongDistanceMatching = GLOBAL_GET<bool>("compression/formats/zstd/long_distance_matching");
        Compression.ZstdLevel                = GLOBAL_GET<int>("compression/formats/zstd/compression_level");
        Compression.ZstdWindowLogSize        = GLOBAL_GET<int>("compression/formats/zstd/window_log_size");
        Compression.ZlibLevel                = GLOBAL_GET<int>("compression/formats/zlib/compression_level");
        Compression.GzipLevel                = GLOBAL_GET<int>("compression/formats/gzip/compression_level");

        this.IsProjectLoaded = err == Error.OK;
        return err;
    }
}
