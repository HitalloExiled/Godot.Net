#define GLES3_ENABLED
#define MINIZIP_ENABLED
#define TOOLS_ENABLED
#define TOOLS_ENABLED
#define VULKAN_ENABLED

namespace Godot.Net.Main;

using System.Globalization;
using System.Runtime.CompilerServices;
using Godot.Net.Core;
using Godot.Net.Core.Config;
using Godot.Net.Core.Error;
using Godot.Net.Core.Input;
using Godot.Net.Core.IO;
using Godot.Net.Core.Math;
using Godot.Net.Core.Object;
using Godot.Net.Core.OS;
using Godot.Net.Core.String;
using Godot.Net.Editor;
using Godot.Net.Scene.Main;
using Godot.Net.Scene.Theme;
using Godot.Net.Servers;
using Godot.Net.Servers.Rendering;
using Godot.Net.Servers.Text;

using static Godot.Net.Core.RegisterCoreTypesModule;
using static Godot.Net.Drivers.RegisterDriverTypesModule;
using static Godot.Net.Modules.RegisterModulesTypesModule;
using static Godot.Net.Scene.RegisterSceneTypesModule;
using static Godot.Net.Servers.RegisterServerTypesModule;

#pragma warning disable IDE0044,IDE0059,IDE0052,CS0219,CS0414,IDE0051,CS0649,IDE0060 // TODO - Remove

public class Main
{
    #region  private static readonly fields
    private static readonly ProjectSettings globals       = new();
    private static readonly MainTimerSync   mainTimerSync = new();
    #endregion  private static readonly fields

    #region private static fields
    private static bool                            autoQuit;
    private static CameraServer                    cameraServer         = null!;
    private static bool                            disableRenderLoop;
    private static bool                            disableVsync;
    private static int                             displayDriverIdx     = -1;
    private static DisplayServer?                  displayServer;
    private static bool                            editor;
    private static Engine                          engine               = null!;
    private static int                             fixedFps             = -1;
    private static bool                            forceRedrawRequested;
    private static bool                            foundProject;
    private static uint                            frame;
    private static int                             frameDelay;
    private static uint                            frames;
    private static uint                            hidePrintFpsAttempts = 3;
    private static bool                            initAlwaysOnTop;
    private static Vector2<int>                    initCustomPos;
    private static bool                            initFullscreen;
    private static bool                            initMaximized;
    private static int                             initScreen           = DisplayServer.SCREEN_PRIMARY;
    private static bool                            initUseCustomPos;
    private static bool                            initWindowed;
    private static Input                           input                = null!;
    private static int                             iterating;
    private static uint                            lastTicks;
    private static MessageQueue                    messageQueue         = null!;
    private static PackedData                      packedData           = null!;
    private static Performance                     performance          = null!;
    private static long                            physicsProcessMax;
    private static bool                            printFps;
    private static long                            processMax;
    private static bool                            profileGpu;
    private static bool                            projectManager;
    private static string?                         renderingDriver;
    private static string?                         renderingMethod;
    private static RenderingServerDefault          renderingServer      = null!;
    private static bool                            singleWindow;
    private static bool                            startSuccess;
    private static string?                         startupBenchmarkFile;
    private static string?                         textDriver;
    private static int                             textDriverIdx        = -1;
    private static ThemeDB                         themeDB              = null!;
    private static TranslationServer               translationServer    = null!;
    private static TextServerManager               tsman                = null!;
    private static bool                            useStartupBenchmark;
    private static DisplayServer.WindowFlagsBit    windowFlags;
    private static DisplayServer.WindowMode        windowMode;
    private static DisplayServer.ScreenOrientation windowOrientation;
    private static Vector2<int>                    windowSize           = new(1152, 648);
    private static DisplayServer.VSyncMode         windowVsyncMode;
    private static ZipArchive                      zipPackedData        = null!;
    #endregion private static fields

    #region public static properties
    public static bool IsCmdlineTool { get; }
    #endregion public static properties

    #region private static macros
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void MAIN_PRINT(string message) => PrintLine(message);
    #endregion private static macros

    #region private static methods
    private static void InitializeThemeDB()
    {
        themeDB = new ThemeDB();
	    themeDB.InitializeTheme();
    }

    private static void RegisterCoreDriverTypes()
    {
        // Todo
    }

    private static void RegisterCoreSettings()
    {
        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "network/limits/tcp/connect_timeout_seconds", PropertyHint.PROPERTY_HINT_RANGE, "1,1800,1"), 30);
        GLOBAL_DEF_RST(new PropertyInfo(VariantType.INT, "network/limits/packet_peer_stream/max_buffer_po2", PropertyHint.PROPERTY_HINT_RANGE, "0,64,1,or_greater"), 16);
        GLOBAL_DEF(new PropertyInfo(VariantType.STRING, "network/tls/certificate_bundle_override", PropertyHint.PROPERTY_HINT_FILE, "*.crt"), "");

        var workerThreads               = GLOBAL_DEF("threading/worker_pool/max_threads", -1);
        var lowPriorityUseSystemThreads = GLOBAL_DEF("threading/worker_pool/use_system_threads_for_low_priority_tasks", true);
        var lowPropertyRatio            = GLOBAL_DEF("threading/worker_pool/low_priority_thread_ratio", 0.3f);

        // Todo - core\register_core_types.cpp[307:310] WorkerThreadPool possibly managed by dotnet
    }

    private static void RegisterCoreSingletons() => throw new NotImplementedException();

    private static void RegisterCoreTypes()
    {
        // Todo
    }

    private static void RegisterDriverTypes()
    {
        // Do nothing
    }

    private static Error Setup2()
    {
        // print_line(String(VERSION_NAME) + " v" + get_full_version_string() + " - " + String(VERSION_WEBSITE));

        engine.StartupBenchmarkBeginMeasure("servers");

        tsman = new TextServerManager();

        tsman.AddInterface(new TextServerDummy());

        // physics_server_3d_manager = memnew(PhysicsServer3DManager);
        // physics_server_2d_manager = memnew(PhysicsServer2DManager);

        RegisterServerTypes();
        InitializeModules(ModuleInitializationLevel.MODULE_INITIALIZATION_LEVEL_SERVERS);

        //     GDExtensionManager::get_singleton()->initialize_extensions(GDExtension::INITIALIZATION_LEVEL_SERVERS);
        //     if (p_main_tid_override) {
        //         Thread::main_thread_id = p_main_tid_override;
        //     }

        #if TOOLS_ENABLED
        if (editor || projectManager || IsCmdlineTool)
        {
            EditorPaths.Create();
            if (foundProject && EditorPaths.Singleton.IsSelfContained)
            {
                if (ProjectSettings.Singleton.ResourcePath == AppContext.BaseDirectory)
                {
                    ERR_PRINT("You are trying to run a self-contained editor at the same location as a project. This is not allowed, since editor files will mix with project files.");
                    Environment.ExitCode = 1;
                    return Error.FAILED;
                }
            }
        }
        #endif

        /* Initialize Input */

        input = new Input();

        /* Initialize Display Server */

        var displayDriver = DisplayServer.ServerCreateFunctions[displayDriverIdx].Name;

        var windowPosition = new Vector2<int>();
        var position = initCustomPos;

        if (initUseCustomPos)
        {
            windowPosition = position;
        }

        var bootBgColor = GLOBAL_DEF_BASIC("application/boot_splash/bg_color", Splash.BootSplashBgColor);
        DisplayServer.SetEarlyWindowClearColorOverride(true, bootBgColor);

        var err = Error.OK;

        displayServer = DisplayServer.Create(
            displayDriverIdx,
            renderingDriver!,
            windowMode,
            windowVsyncMode,
            windowFlags,
            out windowPosition,
            windowSize,
            initScreen,
            out err
        );

        if (err != Error.OK || displayServer == null)
        {
            for (var i = 0; i < DisplayServer.ServerCreateFunctions.Count - 1; i++)
            {
                if (i == displayDriverIdx)
                {
                    continue; // Don't try the same twice.
                }

                displayServer = DisplayServer.Create(
                    i,
                    renderingDriver!,
                    windowMode,
                    windowVsyncMode,
                    windowFlags,
                    out windowPosition,
                    windowSize,
                    initScreen,
                    out err
                );

                if (err == Error.OK && displayServer != null)
                {
                    break;
                }
            }
        }

        if (err != Error.OK || displayServer == null)
        {
            ERR_PRINT("Unable to create DisplayServer, all display drivers failed.");
            return err;
        }

        if (displayServer.HasFeature(DisplayServer.Feature.FEATURE_ORIENTATION))
        {
            displayServer.ScreenSetOrientation(windowOrientation);
        }

        if (GLOBAL_GET<bool>("debug/settings/stdout/print_fps") || printFps)
        {
            switch (windowVsyncMode)
            {
                case DisplayServer.VSyncMode.VSYNC_DISABLED:
                    PrintLine("Requested V-Sync mode: Disabled");
                    break;
                case DisplayServer.VSyncMode.VSYNC_ENABLED:
                    PrintLine("Requested V-Sync mode: Enabled - FPS will likely be capped to the monitor refresh rate.");
                    break;
                case DisplayServer.VSyncMode.VSYNC_ADAPTIVE:
                    PrintLine("Requested V-Sync mode: Adaptive");
                    break;
                case DisplayServer.VSyncMode.VSYNC_MAILBOX:
                    PrintLine("Requested V-Sync mode: Mailbox");
                    break;
            }
        }

        GLOBAL_DEF_RST_NOVAL("input_devices/pen_tablet/driver", "");
        GLOBAL_DEF_RST_NOVAL(new PropertyInfo(VariantType.STRING, "input_devices/pen_tablet/driver.windows", PropertyHint.PROPERTY_HINT_ENUM, "wintab,winink"), "");

        // TODO - main\main.cpp[2053:2071]

        renderingServer = new RenderingServerDefault(OS.Singleton.RenderThreadMode == OS.RenderThreadModeType.RENDER_SEPARATE_THREAD);

        renderingServer.Init();

        renderingServer.RenderLoopEnabled = !disableRenderLoop;

        if (profileGpu || !editor && GLOBAL_GET<bool>("debug/settings/stdout/print_gpu_profile"))
        {
            renderingServer.PrintGpuProfile = true;
        }

        // TODO - main\main.cpp[2081:2106]

        if (initUseCustomPos)
        {
            displayServer.WindowSetPosition(initCustomPos);
        }

        // TODO - main\main.cpp[2114:2118]

        // <TODO> RegisterCoreSingletons();

        MAIN_PRINT("Main: Setup Logo");

        #if !TOOLS_ENABLED && (WEB_ENABLED || ANDROID_ENABLED)
        var showLogo = false;
        #else
        var showLogo = true;
        #endif

        if (initWindowed)
        {
            //do none..
        }
        else if (initMaximized)
        {
            DisplayServer.Singleton.WindowSetMode(DisplayServer.WindowMode.WINDOW_MODE_MAXIMIZED);
        }
        else if (initFullscreen)
        {
            DisplayServer.Singleton.WindowSetMode(DisplayServer.WindowMode.WINDOW_MODE_FULLSCREEN);
        }
        if (initAlwaysOnTop)
        {
            DisplayServer.Singleton.WindowSetFlag(DisplayServer.WindowFlags.WINDOW_FLAG_ALWAYS_ON_TOP, true);
        }

        MAIN_PRINT("Main: Load Boot Image");

        var clear = GLOBAL_DEF_BASIC("rendering/environment/defaults/default_clear_color", new Color(0.3f, 0.3f, 0.3f));
        RS.Singleton.SetDefaultClearColor(clear);

        //     if (show_logo) { //boot logo!
        //         const bool boot_logo_image = GLOBAL_DEF_BASIC("application/boot_splash/show_image", true);
        //         const String boot_logo_path = String(GLOBAL_DEF_BASIC(PropertyInfo(Variant::STRING, "application/boot_splash/image", PROPERTY_HINT_FILE, "*.png"), String())).strip_edges();
        //         const bool boot_logo_scale = GLOBAL_DEF_BASIC("application/boot_splash/fullsize", true);
        //         const bool boot_logo_filter = GLOBAL_DEF_BASIC("application/boot_splash/use_filter", true);

        //         Ref<Image> boot_logo;

        //         if (boot_logo_image) {
        //             if (!boot_logo_path.is_empty()) {
        //                 boot_logo.instantiate();
        //                 Error load_err = ImageLoader::load_image(boot_logo_path, boot_logo);
        //                 if (load_err) {
        //                     ERR_PRINT("Non-existing or invalid boot splash at '" + boot_logo_path + "'. Loading default splash.");
        //                 }
        //             }
        //         } else {
        //             // Create a 1×1 transparent image. This will effectively hide the splash image.
        //             boot_logo.instantiate();
        //             boot_logo->initialize_data(1, 1, false, Image::FORMAT_RGBA8);
        //             boot_logo->set_pixel(0, 0, Color(0, 0, 0, 0));
        //         }

        //         Color boot_bg_color = GLOBAL_GET("application/boot_splash/bg_color");

        // #if defined(TOOLS_ENABLED) && !defined(NO_EDITOR_SPLASH)
        //         boot_bg_color =
        //                 GLOBAL_DEF_BASIC("application/boot_splash/bg_color",
        //                         (editor || project_manager) ? boot_splash_editor_bg_color : boot_splash_bg_color);
        // #endif
        //         if (boot_logo.is_valid()) {
        //             RenderingServer::get_singleton()->set_boot_image(boot_logo, boot_bg_color, boot_logo_scale,
        //                     boot_logo_filter);

        //         } else {
        // #ifndef NO_DEFAULT_BOOT_LOGO
        //             MAIN_PRINT("Main: Create bootsplash");
        // #if defined(TOOLS_ENABLED) && !defined(NO_EDITOR_SPLASH)
        //             Ref<Image> splash = (editor || project_manager) ? memnew(Image(boot_splash_editor_png)) : memnew(Image(boot_splash_png));
        // #else
        //             Ref<Image> splash = memnew(Image(boot_splash_png));
        // #endif

        //             MAIN_PRINT("Main: ClearColor");
        //             RenderingServer::get_singleton()->set_default_clear_color(boot_bg_color);
        //             MAIN_PRINT("Main: Image");
        //             RenderingServer::get_singleton()->set_boot_image(splash, boot_bg_color, false);
        // #endif
        //         }

        // #if defined(TOOLS_ENABLED) && defined(MACOS_ENABLED)
        //         if (OS::get_singleton()->get_bundle_icon_path().is_empty()) {
        //             Ref<Image> icon = memnew(Image(app_icon_png));
        //             DisplayServer::get_singleton()->set_icon(icon);
        //         }
        // #endif
        //     }

        //     DisplayServer::set_early_window_clear_color_override(false);

        //     MAIN_PRINT("Main: DCC");
        //     RenderingServer::get_singleton()->set_default_clear_color(
        //             GLOBAL_GET("rendering/environment/defaults/default_clear_color"));

        //     GLOBAL_DEF_BASIC(PropertyInfo(Variant::STRING, "application/config/icon", PROPERTY_HINT_FILE, "*.png,*.webp,*.svg"), String());
        //     GLOBAL_DEF(PropertyInfo(Variant::STRING, "application/config/macos_native_icon", PROPERTY_HINT_FILE, "*.icns"), String());
        //     GLOBAL_DEF(PropertyInfo(Variant::STRING, "application/config/windows_native_icon", PROPERTY_HINT_FILE, "*.ico"), String());

        //     Input *id = Input::get_singleton();
        //     if (id) {
        //         agile_input_event_flushing = GLOBAL_DEF("input_devices/buffering/agile_event_flushing", false);

        //         if (bool(GLOBAL_DEF_BASIC("input_devices/pointing/emulate_touch_from_mouse", false)) &&
        //                 !(editor || project_manager)) {
        //             if (!DisplayServer::get_singleton()->is_touchscreen_available()) {
        //                 //only if no touchscreen ui hint, set emulation
        //                 id->set_emulate_touch_from_mouse(true);
        //             }
        //         }

        //         id->set_emulate_mouse_from_touch(bool(GLOBAL_DEF_BASIC("input_devices/pointing/emulate_mouse_from_touch", true)));
        //     }

        //     MAIN_PRINT("Main: Load Translations and Remaps");

        //     translation_server->setup(); //register translations, load them, etc.
        //     if (!locale.is_empty()) {
        //         translation_server->set_locale(locale);
        //     }
        //     translation_server->load_translations();
        //     ResourceLoader::load_translation_remaps(); //load remaps for resources

        //     ResourceLoader::load_path_remaps();

        MAIN_PRINT("Main: Load TextServer");

        /* Enum text drivers */
        GLOBAL_DEF_RST("internationalization/rendering/text_driver", "");

        var textDriverOptions = "";

        foreach (var @interface in TextServerManager.Singleton.Interfaces)
        {
            var driverName = @interface.Name;

            if (driverName == "Dummy")
            {
                // Dummy text driver cannot draw any text, making the editor unusable if selected.
                continue;
            }

            if (!string.IsNullOrEmpty(textDriverOptions) && !textDriverOptions.Contains(',', StringComparison.CurrentCulture))
            {
                // Not the first option; add a comma before it as a separator for the property hint.
                textDriverOptions += ",";
            }

            textDriverOptions += driverName;
        }

        GLOBAL_DEF_RST(new PropertyInfo(VariantType.STRING, "internationalization/rendering/text_driver", PropertyHint.PROPERTY_HINT_ENUM, textDriverOptions), textDriverOptions);

        /* Determine text driver */
        if (string.IsNullOrEmpty(textDriver))
        {
            textDriver = GLOBAL_GET<string>("internationalization/rendering/text_driver");
        }

        if (!string.IsNullOrEmpty(textDriver))
        {
            var i = 0;
            /* Load user selected text server. */
            foreach (var @interface in TextServerManager.Singleton.Interfaces)
            {
                if (@interface.Name == textDriver)
                {
                    textDriverIdx = i;
                    break;
                }
                i++;
            }
        }

        if (textDriverIdx < 0)
        {
            /* If not selected, use one with the most features available. */
            var maxFeatures = 0;
            var i           = 0;

            foreach (var @interface in TextServerManager.Singleton.Interfaces)
            {
                var features      = (int)@interface.Features;
                var featureNumber = 0;

                while (features != default)
                {
                    featureNumber += features & 1;
                    features >>= 1;
                }
                if (featureNumber >= maxFeatures)
                {
                    maxFeatures   = featureNumber;
                    textDriverIdx = i;
                }
                i++;
            }
        }

        if (textDriverIdx >= 0)
        {
            var ts = TextServerManager.Singleton.Interfaces[textDriverIdx];

            TextServerManager.Singleton.PrimaryInterface = ts;

            // Godot.Net - No needed on .net
            // if (ts.HasFeature(TextServer.Feature.FEATURE_USE_SUPPORT_DATA))
            // {
            //     ts.LoadSupportData($"res://{ts.GetSupportDataFilename()}");
            // }
        }
        else
        {
            return ERR_FAIL_V_MSG(Error.ERR_CANT_CREATE, "TextServer: Unable to create TextServer interface.");
        }

        engine.StartupBenchmarkEndMeasure();

        MAIN_PRINT("Main: Load Scene Types");

        engine.StartupBenchmarkBeginMeasure("scene");

        RegisterSceneTypes();
        RegisterDriverTypes();

        InitializeModules(ModuleInitializationLevel.MODULE_INITIALIZATION_LEVEL_SCENE);

        // TODO - main\main.cpp[2324:2334]

        MAIN_PRINT("Main: Load Modules");

        // <TODO> - RegisterPlatformApis();

        // Theme needs modules to be initialized so that sub-resources can be loaded.
        InitializeThemeDB();
        RegisterSceneSingletons();

        GLOBAL_DEF_BASIC("display/mouse_cursor/custom_image", "");
        GLOBAL_DEF_BASIC("display/mouse_cursor/custom_image_hotspot", new Vector2<RealT>());
        GLOBAL_DEF_BASIC("display/mouse_cursor/tooltip_position_offset", new Vector2<RealT>(10, 10));
        GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.STRING, "display/mouse_cursor/custom_image", PropertyHint.PROPERTY_HINT_FILE, "*.png,*.webp"), "");

        // TODO - main\main.cpp[2352:2359]

        cameraServer = CameraServer.Create();

        // TODO - main\main.cpp[2363:2366]

        RegisterServerSingletons();

        // TODO - main\main.cpp[2369:2386]

        startSuccess = true;

        // TODO - main\main.cpp[2390:2393]

        MAIN_PRINT("Main: Done");

        engine.StartupBenchmarkEndMeasure(); // scene

        return Error.OK;
    }

    private static void UnregisterCoreExtensions()
    {
        // TODO
    }

    private static void UnregisterCoreTypes()
    {
        // TODO
    }
    #endregion private static methods

    #region public static methods
    public static void Cleanup() => throw new NotImplementedException();

    public static void ForceRedraw() => forceRedrawRequested = true;

    public static bool Iteration()
    {
        iterating++;

        var ticks = (uint)OS.Singleton.TicksUsec;

        Engine.Singleton.FrameTicks = ticks;

        mainTimerSync.CpuTicksUsec = ticks;
        mainTimerSync.FixedFps     = fixedFps;

        var ticksElapsed = ticks - lastTicks;

        var physicsStep = 1.0 / Engine.Singleton.PhysicsTicksPerSecond;
        var timeScale   = Engine.Singleton.TimeScale;

        var advance = mainTimerSync.Advance(physicsStep, Engine.Singleton.PhysicsTicksPerSecond);

        var processStep = advance.ProcessStep;
        var scaledStep  = advance.ProcessStep * timeScale;

        Engine.Singleton.ProcessStep                  = processStep;
        Engine.Singleton.PhysicsInterpolationFraction = advance.InterpolationFraction;

        // var physicsProcessTicks = 0uL;
        var processTicks = 0L;

        frame += ticksElapsed;

        lastTicks = ticks;

        var maxPhysicsSteps = Engine.Singleton.MaxPhysicsStepsPerFrame;

        if (fixedFps == -1 && advance.PhysicsSteps > maxPhysicsSteps)
        {
            processStep -= (advance.PhysicsSteps - maxPhysicsSteps) * physicsStep;
            advance.PhysicsSteps = maxPhysicsSteps;
        }

        var exit = false;

        // TODO - main\main.cpp[3146:3189]

        var processBegin = OS.Singleton.TicksUsec;

        if (OS.Singleton.MainLoop!.Process(processStep * timeScale))
        {
            exit = true;
        }

        // TODO - main\main.cpp[3146:3196]

        RS.Singleton.Sync();

        if (DisplayServer.Singleton.CanAnyWindowDraw && RS.Singleton.RenderLoopEnabled)
        {
            if (!forceRedrawRequested && OS.Singleton.InLowProcessorUsageMode)
            {
                if (RS.Singleton.HasChanged)
                {
                    RS.Singleton.Draw(true, scaledStep);
                    Engine.Singleton.FramesDrawn++;
                }
            }
            else
            {
                RS.Singleton.Draw(true, scaledStep);
                Engine.Singleton.FramesDrawn++;
                forceRedrawRequested = false;
            }
        }

        ticks = (uint)OS.Singleton.TicksUsec;

        processTicks = OS.Singleton.TicksUsec - processBegin;
        processMax   = Math.Max(processTicks, processMax);

        var frameTime = OS.Singleton.TicksUsec - ticks;

        // TODO - main\main.cpp[3218:3226]

        frames++;

        Engine.Singleton.ProcessFrames++;

        if (frame > 1000000)
        {
            if (hidePrintFpsAttempts == 0)
            {
                if (editor || projectManager)
                {
                    if (printFps)
                    {
                        PrintLine($"Editor FPS: {frames} ({(1000.0 / frames).ToString("0.00", CultureInfo.InvariantCulture)} mspf)");
                    }
                }
                else if (printFps || GLOBAL_GET<bool>("debug/settings/stdout/print_fps"))
                {
                    PrintLine($"Project FPS: {frames} ({(1000.0 / frames).ToString("0.00", CultureInfo.InvariantCulture)} mspf)");
                }
            }
            else
            {
                hidePrintFpsAttempts--;
            }

            Engine.Singleton.Fps = frames;

            performance.ProcessTime        = processMax / TimeSpan.TicksPerSecond;
            performance.PhysicsProcessTime = physicsProcessMax / TimeSpan.TicksPerSecond;

            processMax        = 0;
            physicsProcessMax = 0;

            frame %= 1000000;
            frames = 0;
        }

        iterating--;

        // TODO - main\main.cpp[3258:3267]

        if (fixedFps != -1)
        {
            return exit;
        }

        OS.Singleton.AddFrameDelay(DisplayServer.Singleton.WindowCanDraw());

        // TODO - main\main.cpp[3275:3288]

        return exit || autoQuit;
    }

    public static Error Setup(string execpath, string[] args, bool secondPhase)
    {
        ClassDB.Initialize();
        // TODO - Review non GLOBAL_DEF

        OS.Singleton.Initialize();

        engine = new Engine();

        MAIN_PRINT("Main: Initialize CORE");

        engine.StartupBegin();
        engine.StartupBenchmarkBeginMeasure("core");

        RegisterCoreTypes();
        RegisterCoreDriverTypes();

        MAIN_PRINT("Main: Initialize Globals");

        #region TODO
        // input_map = memnew(InputMap);
        // time_singleton = memnew(Time);
        // globals = memnew(ProjectSettings);
        #endregion TODO

        RegisterCoreSettings(); //here globals are present

        translationServer = new TranslationServer();

        performance = new();

        #region TODO
        // GDREGISTER_CLASS(Performance);
        #endregion TODO

        engine.AddSingleton(performance);

        GLOBAL_DEF("application/run/flush_stdout_on_print", false);
        GLOBAL_DEF("application/run/flush_stdout_on_print.debug", true);

        MAIN_PRINT("Main: Parse CMDLine");

        /* argument parsing and main creation */
        var mainArgs       = new List<string>();
        var userArgs       = new List<string>();
        var addingUserArgs = false;

        var displayDriver   = "";
        var audioDriver     = "";
        var projectPath     = ".";
        var upwards         = false;
        var debugUri        = "";
        var skipBreakpoints = false;
        var mainPack        = default(string);
        var quietStdout     = false;
        var rtm             = -1;

        var remotefs     = default(string);
        var remotefsPass = default(string);

        var breakpoints  = default(string[]);
        var useCustomRes = true;
        var forceRes     = false;

        var defaultRenderer       = "";
        var defaultRendererMobile = "";
        var rendererHints         = "";

        packedData = new PackedData();

        #if MINIZIP_ENABLED
        zipPackedData = new ZipArchive();
        packedData.AddPackSource(zipPackedData);
        #endif

        // Default exit code, can be modified for certain errors.
        var exitCode = Error.ERR_INVALID_PARAMETER;

        // I = args.front();
        // while (I) {
        //     #ifdef MACOS_ENABLED
        // 	// Ignore the process serial number argument passed by macOS Gatekeeper.
        // 	// Otherwise, Godot would try to open a non-existent project on the first start and abort.
        // 	if (I->get().begins_with("-psn_")) {
        // 		I = I->next();
        // 		continue;
        // 	}
        //     #endif

        // 	List<String>::Element *N = I->next();

        //     #ifdef TOOLS_ENABLED
        // 	if (I->get() == "--debug" ||
        // 			I->get() == "--verbose" ||
        // 			I->get() == "--disable-crash-handler") {
        // 		forwardable_cli_arguments[CLI_SCOPE_TOOL].push_back(I->get());
        // 		forwardable_cli_arguments[CLI_SCOPE_PROJECT].push_back(I->get());
        // 	}
        // 	if (I->get() == "--single-window") {
        // 		forwardable_cli_arguments[CLI_SCOPE_TOOL].push_back(I->get());
        // 	}
        // 	if (I->get() == "--audio-driver" ||
        // 			I->get() == "--display-driver" ||
        // 			I->get() == "--rendering-method" ||
        // 			I->get() == "--rendering-driver") {
        // 		if (I->next()) {
        // 			forwardable_cli_arguments[CLI_SCOPE_TOOL].push_back(I->get());
        // 			forwardable_cli_arguments[CLI_SCOPE_TOOL].push_back(I->next()->get());
        // 		}
        // 	}
        //     #endif

        // 	if (adding_user_args) {
        // 		user_args.push_back(I->get());
        // 	} else if (I->get() == "-h" || I->get() == "--help" || I->get() == "/?") { // display help

        // 		show_help = true;
        // 		exit_code = ERR_HELP; // Hack to force an early exit in `main()` with a success code.
        // 		goto error;

        // 	} else if (I->get() == "--version") {
        // 		print_line(get_full_version_string());
        // 		exit_code = ERR_HELP; // Hack to force an early exit in `main()` with a success code.
        // 		goto error;

        // 	} else if (I->get() == "-v" || I->get() == "--verbose") { // verbose output

        // 		OS::get_singleton()->_verbose_stdout = true;
        // 	} else if (I->get() == "-q" || I->get() == "--quiet") { // quieter output

        // 		quiet_stdout = true;

        // 	} else if (I->get() == "--audio-driver") { // audio driver

        // 		if (I->next()) {
        // 			audio_driver = I->next()->get();

        // 			bool found = false;
        // 			for (int i = 0; i < AudioDriverManager::get_driver_count(); i++) {
        // 				if (audio_driver == AudioDriverManager::get_driver(i)->get_name()) {
        // 					found = true;
        // 				}
        // 			}

        // 			if (!found) {
        // 				OS::get_singleton()->print("Unknown audio driver '%s', aborting.\nValid options are ",
        // 						audio_driver.utf8().get_data());

        // 				for (int i = 0; i < AudioDriverManager::get_driver_count(); i++) {
        // 					if (i == AudioDriverManager::get_driver_count() - 1) {
        // 						OS::get_singleton()->print(" and ");
        // 					} else if (i != 0) {
        // 						OS::get_singleton()->print(", ");
        // 					}

        // 					OS::get_singleton()->print("'%s'", AudioDriverManager::get_driver(i)->get_name());
        // 				}

        // 				OS::get_singleton()->print(".\n");

        // 				goto error;
        // 			}

        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing audio driver argument, aborting.\n");
        // 			goto error;
        // 		}
        // 	} else if (I->get() == "--text-driver") {
        // 		if (I->next()) {
        // 			text_driver = I->next()->get();
        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing text driver argument, aborting.\n");
        // 			goto error;
        // 		}

        // 	} else if (I->get() == "--display-driver") { // force video driver

        // 		if (I->next()) {
        // 			display_driver = I->next()->get();

        // 			bool found = false;
        // 			for (int i = 0; i < DisplayServer::get_create_function_count(); i++) {
        // 				if (display_driver == DisplayServer::get_create_function_name(i)) {
        // 					found = true;
        // 				}
        // 			}

        // 			if (!found) {
        // 				OS::get_singleton()->print("Unknown display driver '%s', aborting.\nValid options are ",
        // 						display_driver.utf8().get_data());

        // 				for (int i = 0; i < DisplayServer::get_create_function_count(); i++) {
        // 					if (i == DisplayServer::get_create_function_count() - 1) {
        // 						OS::get_singleton()->print(" and ");
        // 					} else if (i != 0) {
        // 						OS::get_singleton()->print(", ");
        // 					}

        // 					OS::get_singleton()->print("'%s'", DisplayServer::get_create_function_name(i));
        // 				}

        // 				OS::get_singleton()->print(".\n");

        // 				goto error;
        // 			}

        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing display driver argument, aborting.\n");
        // 			goto error;
        // 		}
        // 	} else if (I->get() == "--rendering-method") {
        // 		if (I->next()) {
        // 			rendering_method = I->next()->get();
        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing renderer name argument, aborting.\n");
        // 			goto error;
        // 		}
        // 	} else if (I->get() == "--rendering-driver") {
        // 		if (I->next()) {
        // 			rendering_driver = I->next()->get();
        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing rendering driver argument, aborting.\n");
        // 			goto error;
        // 		}
        // 	} else if (I->get() == "-f" || I->get() == "--fullscreen") { // force fullscreen

        // 		init_fullscreen = true;
        // 	} else if (I->get() == "-m" || I->get() == "--maximized") { // force maximized window

        // 		init_maximized = true;
        // 		window_mode = DisplayServer::WINDOW_MODE_MAXIMIZED;

        // 	} else if (I->get() == "-w" || I->get() == "--windowed") { // force windowed window

        // 		init_windowed = true;
        // 	} else if (I->get() == "--gpu-index") {
        // 		if (I->next()) {
        // 			Engine::singleton->gpu_idx = I->next()->get().to_int();
        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing GPU index argument, aborting.\n");
        // 			goto error;
        // 		}
        // 	} else if (I->get() == "--gpu-validation") {
        // 		Engine::singleton->use_validation_layers = true;
        //     #ifdef DEBUG_ENABLED
        // 	} else if (I->get() == "--gpu-abort") {
        // 		Engine::singleton->abort_on_gpu_errors = true;
        //     #endif
        // 	} else if (I->get() == "--tablet-driver") {
        // 		if (I->next()) {
        // 			tablet_driver = I->next()->get();
        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing tablet driver argument, aborting.\n");
        // 			goto error;
        // 		}
        // 	} else if (I->get() == "--single-window") { // force single window

        // 		single_window = true;
        // 	} else if (I->get() == "-t" || I->get() == "--always-on-top") { // force always-on-top window

        // 		init_always_on_top = true;
        // 	} else if (I->get() == "--resolution") { // force resolution

        // 		if (I->next()) {
        // 			String vm = I->next()->get();

        // 			if (!vm.contains("x")) { // invalid parameter format

        // 				OS::get_singleton()->print("Invalid resolution '%s', it should be e.g. '1280x720'.\n",
        // 						vm.utf8().get_data());
        // 				goto error;
        // 			}

        // 			int w = vm.get_slice("x", 0).to_int();
        // 			int h = vm.get_slice("x", 1).to_int();

        // 			if (w <= 0 || h <= 0) {
        // 				OS::get_singleton()->print("Invalid resolution '%s', width and height must be above 0.\n",
        // 						vm.utf8().get_data());
        // 				goto error;
        // 			}

        // 			window_size.width = w;
        // 			window_size.height = h;
        // 			force_res = true;

        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing resolution argument, aborting.\n");
        // 			goto error;
        // 		}

        // 	} else if (I->get() == "--screen") { // set window screen

        // 		if (I->next()) {
        // 			init_screen = I->next()->get().to_int();
        // 			init_use_custom_screen = true;

        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing screen argument, aborting.\n");
        // 			goto error;
        // 		}

        // 	} else if (I->get() == "--position") { // set window position

        // 		if (I->next()) {
        // 			String vm = I->next()->get();

        // 			if (!vm.contains(",")) { // invalid parameter format

        // 				OS::get_singleton()->print("Invalid position '%s', it should be e.g. '80,128'.\n",
        // 						vm.utf8().get_data());
        // 				goto error;
        // 			}

        // 			int x = vm.get_slice(",", 0).to_int();
        // 			int y = vm.get_slice(",", 1).to_int();

        // 			init_custom_pos = Point2(x, y);
        // 			init_use_custom_pos = true;

        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing position argument, aborting.\n");
        // 			goto error;
        // 		}

        // 	} else if (I->get() == "--headless") { // enable headless mode (no audio, no rendering).

        // 		audio_driver = NULL_AUDIO_DRIVER;
        // 		display_driver = NULL_DISPLAY_DRIVER;

        // 	} else if (I->get() == "--profiling") { // enable profiling

        // 		use_debug_profiler = true;

        // 	} else if (I->get() == "-l" || I->get() == "--language") { // language

        // 		if (I->next()) {
        // 			locale = I->next()->get();
        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing language argument, aborting.\n");
        // 			goto error;
        // 		}

        // 	} else if (I->get() == "--remote-fs") { // remote filesystem

        // 		if (I->next()) {
        // 			remotefs = I->next()->get();
        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing remote filesystem address, aborting.\n");
        // 			goto error;
        // 		}
        // 	} else if (I->get() == "--remote-fs-password") { // remote filesystem password

        // 		if (I->next()) {
        // 			remotefs_pass = I->next()->get();
        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing remote filesystem password, aborting.\n");
        // 			goto error;
        // 		}
        // 	} else if (I->get() == "--render-thread") { // render thread mode

        // 		if (I->next()) {
        // 			if (I->next()->get() == "safe") {
        // 				rtm = OS::RENDER_THREAD_SAFE;
        // 			} else if (I->next()->get() == "unsafe") {
        // 				rtm = OS::RENDER_THREAD_UNSAFE;
        // 			} else if (I->next()->get() == "separate") {
        // 				rtm = OS::RENDER_SEPARATE_THREAD;
        // 			}

        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing render thread mode argument, aborting.\n");
        // 			goto error;
        // 		}
        //     #ifdef TOOLS_ENABLED
        // 	} else if (I->get() == "-e" || I->get() == "--editor") { // starts editor

        // 		editor = true;
        // 	} else if (I->get() == "-p" || I->get() == "--project-manager") { // starts project manager
        // 		project_manager = true;
        // 	} else if (I->get() == "--debug-server") {
        // 		if (I->next()) {
        // 			debug_server_uri = I->next()->get();
        // 			if (!debug_server_uri.contains("://")) { // wrong address
        // 				OS::get_singleton()->print("Invalid debug server uri. It should be of the form <protocol>://<bind_address>:<port>.\n");
        // 				goto error;
        // 			}
        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing remote debug server uri, aborting.\n");
        // 			goto error;
        // 		}
        // 	} else if (I->get() == "--build-solutions") { // Build the scripting solution such C#

        // 		auto_build_solutions = true;
        // 		editor = true;
        // 		cmdline_tool = true;
        // 	} else if (I->get() == "--dump-gdextension-interface") {
        // 		// Register as an editor instance to use low-end fallback if relevant.
        // 		editor = true;
        // 		cmdline_tool = true;
        // 		dump_gdextension_interface = true;
        // 		print_line("Dumping GDExtension interface header file");
        // 		// Hack. Not needed but otherwise we end up detecting that this should
        // 		// run the project instead of a cmdline tool.
        // 		// Needs full refactoring to fix properly.
        // 		main_args.push_back(I->get());
        // 	} else if (I->get() == "--dump-extension-api") {
        // 		// Register as an editor instance to use low-end fallback if relevant.
        // 		editor = true;
        // 		cmdline_tool = true;
        // 		dump_extension_api = true;
        // 		print_line("Dumping Extension API");
        // 		// Hack. Not needed but otherwise we end up detecting that this should
        // 		// run the project instead of a cmdline tool.
        // 		// Needs full refactoring to fix properly.
        // 		main_args.push_back(I->get());
        // 	} else if (I->get() == "--export-release" || I->get() == "--export-debug" ||
        // 			I->get() == "--export-pack") { // Export project
        // 		// Actually handling is done in start().
        // 		editor = true;
        // 		cmdline_tool = true;
        // 		main_args.push_back(I->get());
        //     #ifndef DISABLE_DEPRECATED
        // 	} else if (I->get() == "--export") { // For users used to 3.x syntax.
        // 		OS::get_singleton()->print("The Godot 3 --export option was changed to more explicit --export-release / --export-debug / --export-pack options.\nSee the --help output for details.\n");
        // 		goto error;
        // 	} else if (I->get() == "--convert-3to4") {
        // 		// Actually handling is done in start().
        // 		cmdline_tool = true;
        // 		main_args.push_back(I->get());

        // 		if (I->next() && !I->next()->get().begins_with("-")) {
        // 			if (itos(I->next()->get().to_int()) == I->next()->get()) {
        // 				converter_max_kb_file = I->next()->get().to_int();
        // 			}
        // 			if (I->next()->next() && !I->next()->next()->get().begins_with("-")) {
        // 				if (itos(I->next()->next()->get().to_int()) == I->next()->next()->get()) {
        // 					converter_max_line_length = I->next()->next()->get().to_int();
        // 				}
        // 			}
        // 		}
        // 	} else if (I->get() == "--validate-conversion-3to4") {
        // 		// Actually handling is done in start().
        // 		cmdline_tool = true;
        // 		main_args.push_back(I->get());

        // 		if (I->next() && !I->next()->get().begins_with("-")) {
        // 			if (itos(I->next()->get().to_int()) == I->next()->get()) {
        // 				converter_max_kb_file = I->next()->get().to_int();
        // 			}
        // 			if (I->next()->next() && !I->next()->next()->get().begins_with("-")) {
        // 				if (itos(I->next()->next()->get().to_int()) == I->next()->next()->get()) {
        // 					converter_max_line_length = I->next()->next()->get().to_int();
        // 				}
        // 			}
        // 		}
        //     #endif // DISABLE_DEPRECATED
        // 	} else if (I->get() == "--doctool") {
        // 		// Actually handling is done in start().
        // 		cmdline_tool = true;

        // 		// `--doctool` implies `--headless` to avoid spawning an unnecessary window
        // 		// and speed up class reference generation.
        // 		audio_driver = NULL_AUDIO_DRIVER;
        // 		display_driver = NULL_DISPLAY_DRIVER;
        // 		main_args.push_back(I->get());
        //     #endif // TOOLS_ENABLED
        // 	} else if (I->get() == "--path") { // set path of project to start or edit

        // 		if (I->next()) {
        // 			String p = I->next()->get();
        // 			if (OS::get_singleton()->set_cwd(p) != OK) {
        // 				OS::get_singleton()->print("Invalid project path specified: \"%s\", aborting.\n", p.utf8().get_data());
        // 				goto error;
        // 			}
        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing relative or absolute path, aborting.\n");
        // 			goto error;
        // 		}
        // 	} else if (I->get() == "-u" || I->get() == "--upwards") { // scan folders upwards
        // 		upwards = true;
        // 	} else if (I->get() == "--quit") { // Auto quit at the end of the first main loop iteration
        // 		auto_quit = true;
        // 	} else if (I->get().ends_with("project.godot")) {
        // 		String path;
        // 		String file = I->get();
        // 		int sep = MAX(file.rfind("/"), file.rfind("\\"));
        // 		if (sep == -1) {
        // 			path = ".";
        // 		} else {
        // 			path = file.substr(0, sep);
        // 		}
        // 		if (OS::get_singleton()->set_cwd(path) == OK) {
        // 			// path already specified, don't override
        // 		} else {
        // 			project_path = path;
        // 		}
        //     #ifdef TOOLS_ENABLED
        // 		editor = true;
        //     #endif
        // 	} else if (I->get() == "-b" || I->get() == "--breakpoints") { // add breakpoints

        // 		if (I->next()) {
        // 			String bplist = I->next()->get();
        // 			breakpoints = bplist.split(",");
        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing list of breakpoints, aborting.\n");
        // 			goto error;
        // 		}

        // 	} else if (I->get() == "--frame-delay") { // force frame delay

        // 		if (I->next()) {
        // 			frame_delay = I->next()->get().to_int();
        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing frame delay argument, aborting.\n");
        // 			goto error;
        // 		}

        // 	} else if (I->get() == "--time-scale") { // force time scale

        // 		if (I->next()) {
        // 			Engine::get_singleton()->set_time_scale(I->next()->get().to_float());
        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing time scale argument, aborting.\n");
        // 			goto error;
        // 		}

        // 	} else if (I->get() == "--main-pack") {
        // 		if (I->next()) {
        // 			main_pack = I->next()->get();
        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing path to main pack file, aborting.\n");
        // 			goto error;
        // 		};

        // 	} else if (I->get() == "-d" || I->get() == "--debug") {
        // 		debug_uri = "local://";
        // 		OS::get_singleton()->_debug_stdout = true;
        //     #if defined(DEBUG_ENABLED)
        // 	} else if (I->get() == "--debug-collisions") {
        // 		debug_collisions = true;
        // 	} else if (I->get() == "--debug-paths") {
        // 		debug_paths = true;
        // 	} else if (I->get() == "--debug-navigation") {
        // 		debug_navigation = true;
        // 	} else if (I->get() == "--debug-stringnames") {
        // 		StringName::set_debug_stringnames(true);
        //     #endif
        // 	} else if (I->get() == "--remote-debug") {
        // 		if (I->next()) {
        // 			debug_uri = I->next()->get();
        // 			if (!debug_uri.contains("://")) { // wrong address
        // 				OS::get_singleton()->print(
        // 						"Invalid debug host address, it should be of the form <protocol>://<host/IP>:<port>.\n");
        // 				goto error;
        // 			}
        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing remote debug host address, aborting.\n");
        // 			goto error;
        // 		}
        // 	} else if (I->get() == "--editor-pid") { // not exposed to user
        // 		if (I->next()) {
        // 			editor_pid = I->next()->get().to_int();
        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing editor PID argument, aborting.\n");
        // 			goto error;
        // 		}
        // 	} else if (I->get() == "--disable-render-loop") {
        // 		disable_render_loop = true;
        // 	} else if (I->get() == "--fixed-fps") {
        // 		if (I->next()) {
        // 			fixed_fps = I->next()->get().to_int();
        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing fixed-fps argument, aborting.\n");
        // 			goto error;
        // 		}
        // 	} else if (I->get() == "--write-movie") {
        // 		if (I->next()) {
        // 			Engine::get_singleton()->set_write_movie_path(I->next()->get());
        // 			N = I->next()->next();
        // 			if (fixed_fps == -1) {
        // 				fixed_fps = 60;
        // 			}
        // 			OS::get_singleton()->_writing_movie = true;
        // 		} else {
        // 			OS::get_singleton()->print("Missing write-movie argument, aborting.\n");
        // 			goto error;
        // 		}
        // 	} else if (I->get() == "--disable-vsync") {
        // 		disable_vsync = true;
        // 	} else if (I->get() == "--print-fps") {
        // 		print_fps = true;
        // 	} else if (I->get() == "--profile-gpu") {
        // 		profile_gpu = true;
        // 	} else if (I->get() == "--disable-crash-handler") {
        // 		OS::get_singleton()->disable_crash_handler();
        // 	} else if (I->get() == "--skip-breakpoints") {
        // 		skip_breakpoints = true;
        // 	} else if (I->get() == "--xr-mode") {
        // 		if (I->next()) {
        // 			String xr_mode = I->next()->get().to_lower();
        // 			N = I->next()->next();
        // 			if (xr_mode == "default") {
        // 				XRServer::set_xr_mode(XRServer::XRMODE_DEFAULT);
        // 			} else if (xr_mode == "off") {
        // 				XRServer::set_xr_mode(XRServer::XRMODE_OFF);
        // 			} else if (xr_mode == "on") {
        // 				XRServer::set_xr_mode(XRServer::XRMODE_ON);
        // 			} else {
        // 				OS::get_singleton()->print("Unknown --xr-mode argument \"%s\", aborting.\n", xr_mode.ascii().get_data());
        // 				goto error;
        // 			}
        // 		} else {
        // 			OS::get_singleton()->print("Missing --xr-mode argument, aborting.\n");
        // 			goto error;
        // 		}

        // 	} else if (I->get() == "--startup-benchmark") {
        // 		use_startup_benchmark = true;
        // 	} else if (I->get() == "--startup-benchmark-file") {
        // 		if (I->next()) {
        // 			use_startup_benchmark = true;
        // 			startup_benchmark_file = I->next()->get();
        // 			N = I->next()->next();
        // 		} else {
        // 			OS::get_singleton()->print("Missing <path> argument for --startup-benchmark-file <path>.\n");
        // 			goto error;
        // 		}

        // 	} else if (I->get() == "--" || I->get() == "++") {
        // 		adding_user_args = true;
        // 	} else {
        // 		main_args.push_back(I->get());
        // 	}

        // 	I = N;
        // }

        // #ifdef TOOLS_ENABLED
        // if (editor && project_manager) {
        // 	OS::get_singleton()->print(
        // 			"Error: Command line arguments implied opening both editor and project manager, which is not possible. Aborting.\n");
        // 	goto error;
        // }
        // #endif

        // // Network file system needs to be configured before globals, since globals are based on the
        // // 'project.godot' file which will only be available through the network if this is enabled
        // FileAccessNetwork::configure();
        // if (!remotefs.is_empty()) {
        // 	file_access_network_client = memnew(FileAccessNetworkClient);
        // 	int port;
        // 	if (remotefs.contains(":")) {
        // 		port = remotefs.get_slicec(':', 1).to_int();
        // 		remotefs = remotefs.get_slicec(':', 0);
        // 	} else {
        // 		port = 6010;
        // 	}

        // 	Error err = file_access_network_client->connect(remotefs, port, remotefs_pass);
        // 	if (err) {
        // 		OS::get_singleton()->printerr("Could not connect to remotefs: %s:%i.\n", remotefs.utf8().get_data(), port);
        // 		goto error;
        // 	}

        // 	FileAccess::make_default<FileAccessNetwork>(FileAccess::ACCESS_RESOURCES);
        // }

        if (globals.Setup(projectPath, mainPack, upwards, editor) == Error.OK)
        {
            #if TOOLS_ENABLED
            foundProject = true;
            #endif
        }
        else
        {
            #if TOOLS_ENABLED
            editor = false;
            #else
            var errorMsg = $"Error: Couldn't load project data at path \"{projectPath}\". Is the .pck file missing?\nIf you've renamed the executable, the associated .pck file should also be renamed to match the executable's name (without the extension).\n";

            OS.Singleton.Print(errorMsg);
            OS.Singleton.Alert(errorMsg);

            goto error;
            #endif
        }

	    // Initialize user data dir.
        OS.Singleton.EnsureUserDataDir();

        InitializeModules(ModuleInitializationLevel.MODULE_INITIALIZATION_LEVEL_CORE);
        RegisterCoreExtensions(); // core extensions must be registered after globals setup and before display

        // TODO - main\main.cpp[1372]

        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "network/limits/debugger/max_chars_per_second",    PropertyHint.PROPERTY_HINT_RANGE, "0, 4096, 1, or_greater"), 32768);
        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "network/limits/debugger/max_queued_messages",     PropertyHint.PROPERTY_HINT_RANGE, "0, 8192, 1, or_greater"), 2048);
        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "network/limits/debugger/max_errors_per_second",   PropertyHint.PROPERTY_HINT_RANGE, "0, 200, 1, or_greater"), 400);
        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "network/limits/debugger/max_warnings_per_second", PropertyHint.PROPERTY_HINT_RANGE, "0, 200, 1, or_greater"), 400);

        // TODO - main\main.cpp[1379:1395]

        if (!projectManager && !editor)
        {
            projectManager = !foundProject && !IsCmdlineTool;
        }

        if (projectManager)
        {
            Engine.Singleton.ProjectManagerHint = true;
        }

        GLOBAL_DEF("debug/file_logging/enable_file_logging", false);
        // Only file logging by default on desktop platforms as logs can't be
        // accessed easily on mobile/Web platforms (if at all).
        // This also prevents logs from being created for the editor instance, as feature tags
        // are disabled while in the editor (even if they should logically apply).
        GLOBAL_DEF("debug/file_logging/enable_file_logging.pc", true);
        GLOBAL_DEF("debug/file_logging/log_path", "user://logs/godot.log");
        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "debug/file_logging/max_log_files", PropertyHint.PROPERTY_HINT_RANGE, "0,20,1,or_greater"), 5);

        #region TODO
        //     if (!project_manager && !editor && FileAccess::get_create_func(FileAccess::ACCESS_USERDATA) &&
        //     GLOBAL_GET("debug/file_logging/enable_file_logging")) {
        //         // Don't create logs for the project manager as they would be written to
        //         // the current working directory, which is inconvenient.
        //         String base_path = GLOBAL_GET("debug/file_logging/log_path");
        //         int max_files = GLOBAL_GET("debug/file_logging/max_log_files");
        //         OS::get_singleton()->add_logger(memnew(RotatedFileLogger(base_path, max_files)));
        //     }

        //     if (main_args.size() == 0 && String(GLOBAL_GET("application/run/main_scene")) == "") {
        // #ifdef TOOLS_ENABLED
        //         if (!editor && !project_manager) {
        // #endif
        //             const String error_msg = "Error: Can't run project: no main scene defined in the project.\n";
        //             OS::get_singleton()->print("%s", error_msg.utf8().get_data());
        //             OS::get_singleton()->alert(error_msg);
        //             goto error;
        // #ifdef TOOLS_ENABLED
        //         }
        // #endif
        //     }
        #endregion TODO

        if (editor || projectManager)
        {
            Engine.Singleton.EditorHint = true;
            useCustomRes = false;
            // TODO - input_map->load_default(); //keys for editor
        }
        else
        {
            // TODO - input_map->load_from_project_settings(); //keys for game
        }

        #region TODO
        //     if (bool(GLOBAL_GET("application/run/disable_stdout"))) {
        //         quiet_stdout = true;
        //     }
        //     if (bool(GLOBAL_GET("application/run/disable_stderr"))) {
        //         CoreGlobals::print_error_enabled = false;
        //     };

        //     if (quiet_stdout) {
        //         CoreGlobals::print_line_enabled = false;
        //     }

        //     Logger::set_flush_stdout_on_print(GLOBAL_GET("application/run/flush_stdout_on_print"));
        #endregion TODO

        OS.Singleton.SetCmdline(execpath, mainArgs, userArgs);

        var driverHints = "";

        #if VULKAN_ENABLED
        driverHints = "vulkan";
        #endif

        var defaultDriver = driverHints.Split(',').FirstOrDefault() ?? "";

        GLOBAL_DEF("rendering/rendering_device/driver", defaultDriver);

        GLOBAL_DEF(new PropertyInfo(VariantType.STRING, "rendering/rendering_device/driver.windows",  PropertyHint.PROPERTY_HINT_ENUM, driverHints), defaultDriver);
        GLOBAL_DEF(new PropertyInfo(VariantType.STRING, "rendering/rendering_device/driver.linuxbsd", PropertyHint.PROPERTY_HINT_ENUM, driverHints), defaultDriver);
        GLOBAL_DEF(new PropertyInfo(VariantType.STRING, "rendering/rendering_device/driver.android",  PropertyHint.PROPERTY_HINT_ENUM, driverHints), defaultDriver);
        GLOBAL_DEF(new PropertyInfo(VariantType.STRING, "rendering/rendering_device/driver.ios",      PropertyHint.PROPERTY_HINT_ENUM, driverHints), defaultDriver);
        GLOBAL_DEF(new PropertyInfo(VariantType.STRING, "rendering/rendering_device/driver.macos",    PropertyHint.PROPERTY_HINT_ENUM, driverHints), defaultDriver);

        driverHints = "";

        #if GLES3_ENABLED
        driverHints = "opengl3";
        #endif

        defaultDriver = driverHints.Split(',').FirstOrDefault() ?? "";

        GLOBAL_DEF("rendering/gl_compatibility/driver", defaultDriver);
        GLOBAL_DEF(new PropertyInfo(VariantType.STRING, "rendering/gl_compatibility/driver.windows",  PropertyHint.PROPERTY_HINT_ENUM, driverHints), defaultDriver);
        GLOBAL_DEF(new PropertyInfo(VariantType.STRING, "rendering/gl_compatibility/driver.linuxbsd", PropertyHint.PROPERTY_HINT_ENUM, driverHints), defaultDriver);
        GLOBAL_DEF(new PropertyInfo(VariantType.STRING, "rendering/gl_compatibility/driver.web",      PropertyHint.PROPERTY_HINT_ENUM, driverHints), defaultDriver);
        GLOBAL_DEF(new PropertyInfo(VariantType.STRING, "rendering/gl_compatibility/driver.android",  PropertyHint.PROPERTY_HINT_ENUM, driverHints), defaultDriver);
        GLOBAL_DEF(new PropertyInfo(VariantType.STRING, "rendering/gl_compatibility/driver.ios",      PropertyHint.PROPERTY_HINT_ENUM, driverHints), defaultDriver);
        GLOBAL_DEF(new PropertyInfo(VariantType.STRING, "rendering/gl_compatibility/driver.macos",    PropertyHint.PROPERTY_HINT_ENUM, driverHints), defaultDriver);

        #if VULKAN_ENABLED
        rendererHints = "forward_plus,mobile";
        defaultRendererMobile = "mobile";
        #endif

        #if GLES3_ENABLED
        if (!string.IsNullOrEmpty(rendererHints))
        {
            rendererHints += ",";
        }

        rendererHints += "gl_compatibility";
        defaultRendererMobile ??= "gl_compatibility";

        if (renderingDriver == null && renderingMethod == null && projectManager)
        {
            renderingDriver       = "opengl3";
            renderingMethod       = "gl_compatibility";
            defaultRendererMobile = "gl_compatibility";
        }
        #endif

        if (string.IsNullOrEmpty(rendererHints))
        {
            ERR_PRINT("No renderers available.");
        }

        if (!string.IsNullOrEmpty(renderingMethod))
        {
            if (renderingMethod is not "forward_plus" and not "mobile" and not "gl_compatibility")
            {
                OS.Singleton.Print($"Unknown renderer name '{renderingMethod}', aborting. Valid options are: {rendererHints}\n");
                goto error;
            }
        }

        if (!string.IsNullOrEmpty(renderingDriver))
        {
            var found = DisplayServer.ServerCreateFunctions.SelectMany(x => x.GetRenderingDriversFunction()).Any(x => renderingDriver == x);

            if (!found)
            {
                OS.Singleton.Print($"Unknown rendering driver '{renderingDriver}', aborting.\nValid options are ");

                foreach (var driver in DisplayServer.ServerCreateFunctions.SelectMany(x => x.GetRenderingDriversFunction()))
                {
                    OS.Singleton.Print($"'{driver}', ");
                }

                OS.Singleton.Print(".\n");

                goto error;
            }

            if (string.IsNullOrEmpty(renderingMethod))
            {
                renderingMethod = renderingDriver == "opengl3" ? "gl_compatibility" : "forward_plus";
            }

            var validCombination = false;
            var availableDrivers = new List<string>();
            #if VULKAN_ENABLED
            if (renderingMethod is "forward_plus" or "mobile")
            {
                availableDrivers.Add("vulkan");
            }
            #endif

            #if GLES3_ENABLED
            if (renderingMethod == "gl_compatibility")
            {
                availableDrivers.Add("opengl3");
            }
            #endif

            if (availableDrivers.Count == 0)
            {
                OS.Singleton.Print($"Unknown renderer name '{renderingMethod}', aborting.\n");

                goto error;
            }

            for (var i = 0; i < availableDrivers.Count; i++)
            {
                if (renderingDriver == availableDrivers[i])
                {
                    validCombination = true;
                    break;
                }
            }

            if (!validCombination)
            {
                OS.Singleton.Print($"Invalid renderer/driver combination '{renderingMethod}' and '{renderingDriver}', aborting. {renderingMethod} only supports the following drivers ");

                for (var d = 0; d < availableDrivers.Count; d++)
                {
                    OS.Singleton.Print($"'{availableDrivers[d]}', ");
                }

                OS.Singleton.Print(".\n");

                goto error;
            }
        }

        defaultRenderer = rendererHints.Split(',').FirstOrDefault() ?? "";

        GLOBAL_DEF_RST_BASIC(new PropertyInfo(VariantType.STRING, "rendering/renderer/rendering_method", PropertyHint.PROPERTY_HINT_ENUM, rendererHints), defaultRenderer);
        GLOBAL_DEF_RST_BASIC("rendering/renderer/rendering_method.mobile", defaultRendererMobile);
        GLOBAL_DEF_RST_BASIC("rendering/renderer/rendering_method.web",    "gl_compatibility");


        if (string.IsNullOrEmpty(renderingMethod))
        {
            renderingMethod = GLOBAL_GET<string>("rendering/renderer/rendering_method");
        }

        if (string.IsNullOrEmpty(renderingDriver))
        {
            renderingDriver = renderingMethod == "gl_compatibility"
                ? GLOBAL_GET<string>("rendering/gl_compatibility/driver")
                : GLOBAL_GET<string>("rendering/rendering_device/driver");
        }

        OS.Singleton.CurrentRenderingDriverName = renderingDriver;
        OS.Singleton.CurrentRenderingMethod     = renderingMethod;

        renderingDriver = renderingDriver!.ToLower();

        if (useCustomRes)
        {
            if (!forceRes)
            {
                windowSize.X = GLOBAL_GET<int>("display/window/size/viewport_width");
                windowSize.Y = GLOBAL_GET<int>("display/window/size/viewport_height");

                if (globals.HasSetting("display/window/size/window_width_override") && ProjectSettings.Singleton.HasSetting("display/window/size/window_height_override"))
                {
                    var desiredWidth = globals.Get<int>("display/window/size/window_width_override");

                    if (desiredWidth > 0)
                    {
                        windowSize.X = desiredWidth;
                    }

                    var desiredHeight = globals.Get<int>("display/window/size/window_height_override");

                    if (desiredHeight > 0)
                    {
                        windowSize.Y = desiredHeight;
                    }
                }
            }

            if (!GLOBAL_GET<bool>("display/window/size/resizable"))
            {
                windowFlags |= DisplayServer.WindowFlagsBit.WINDOW_FLAG_RESIZE_DISABLED_BIT;
            }
            if (GLOBAL_GET<bool>("display/window/size/borderless"))
            {
                windowFlags |= DisplayServer.WindowFlagsBit.WINDOW_FLAG_BORDERLESS_BIT;
            }
            if (GLOBAL_GET<bool>("display/window/size/always_on_top"))
            {
                windowFlags |= DisplayServer.WindowFlagsBit.WINDOW_FLAG_ALWAYS_ON_TOP_BIT;
            }
            if (GLOBAL_GET<bool>("display/window/size/transparent"))
            {
                windowFlags |= DisplayServer.WindowFlagsBit.WINDOW_FLAG_TRANSPARENT_BIT;
            }
            if (GLOBAL_GET<bool>("display/window/size/extend_to_title"))
            {
                windowFlags |= DisplayServer.WindowFlagsBit.WINDOW_FLAG_EXTEND_TO_TITLE_BIT;
            }
            if (GLOBAL_GET<bool>("display/window/size/no_focus"))
            {
                windowFlags |= DisplayServer.WindowFlagsBit.WINDOW_FLAG_NO_FOCUS_BIT;
            }

            windowMode = GLOBAL_GET<DisplayServer.WindowMode>("display/window/size/mode");
        }

        GLOBAL_DEF("internationalization/locale/include_text_server_data", false);

        OS.Singleton.AllowHidpi   = GLOBAL_DEF("display/window/dpi/allow_hidpi",                true);
        OS.Singleton.AllowLayered = GLOBAL_DEF("display/window/per_pixel_transparency/allowed", false);

        if (editor || projectManager)
        {
            // The editor and project manager always detect and use hiDPI if needed
            OS.Singleton.AllowHidpi = true;
        }

        if (rtm == -1)
        {
            rtm = GLOBAL_DEF("rendering/driver/threads/thread_model", (int)OS.RenderThreadModeType.RENDER_THREAD_SAFE);
        }

        if (rtm is >= 0 and < 3)
        {
            if (editor)
            {
                rtm = (int)OS.RenderThreadModeType.RENDER_THREAD_SAFE;
            }

            OS.Singleton.RenderThreadMode = (OS.RenderThreadModeType)rtm;
        }

        // Make sure that headless is the last one, which it is assumed to be by design.
        //DEV_ASSERT(String("headless") == DisplayServer::get_create_function_name(DisplayServer::get_create_function_count() - 1));

        for (var i = 0; i < DisplayServer.ServerCreateFunctions.Count; i++)
        {
            var name = DisplayServer.ServerCreateFunctions[i].Name;

            if (displayDriver == name)
            {
                displayDriverIdx = i;
                break;
            }
        }

        if (displayDriverIdx < 0)
        {
            displayDriverIdx = 0;
        }

        OS.Singleton.DisplayDriverId = displayDriverIdx;

        // TODO - main\main.cpp[1763:1787]

        windowOrientation = GLOBAL_DEF_BASIC("display/window/handheld/orientation", DisplayServer.ScreenOrientation.SCREEN_LANDSCAPE);
        windowVsyncMode   = GLOBAL_DEF("display/window/vsync/vsync_mode",           DisplayServer.VSyncMode.VSYNC_ENABLED);

        if (disableVsync)
        {
            windowVsyncMode = DisplayServer.VSyncMode.VSYNC_DISABLED;
        }

        engine.PhysicsTicksPerSecond   = GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.INT, "physics/common/physics_ticks_per_second",    PropertyHint.PROPERTY_HINT_RANGE, "1,1000,1"), 60);
        engine.MaxPhysicsStepsPerFrame = GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.INT, "physics/common/max_physics_steps_per_frame", PropertyHint.PROPERTY_HINT_RANGE, "1,100,1"), 8);
        engine.PhysicsJitterFix        = GLOBAL_DEF("physics/common/physics_jitter_fix", 0.5);
        engine.MaxFps                  = GLOBAL_DEF(new PropertyInfo(VariantType.INT, "application/run/max_fps", PropertyHint.PROPERTY_HINT_RANGE, "0,1000,1"), 0);

        GLOBAL_DEF("debug/settings/stdout/print_fps", false);
        GLOBAL_DEF("debug/settings/stdout/print_gpu_profile", false);
        GLOBAL_DEF("debug/settings/stdout/verbose_stdout", false);

        if (!OS.Singleton.VerboseStdout)
        {
            OS.Singleton.VerboseStdout = GLOBAL_GET<bool>("debug/settings/stdout/verbose_stdout");
        }

        if (frameDelay == 0)
        {
            frameDelay = GLOBAL_DEF(new PropertyInfo(VariantType.INT, "application/run/frame_delay_msec", PropertyHint.PROPERTY_HINT_RANGE, "0,100,1,or_greater"), 0); // No negative numbers
        }

        OS.Singleton.LowProcessorUsageMode          = GLOBAL_DEF("application/run/low_processor_mode", false);
        OS.Singleton.LowProcessorUsageModeSleepUsec = GLOBAL_DEF(new PropertyInfo(VariantType.INT, "application/run/low_processor_mode_sleep_usec", PropertyHint.PROPERTY_HINT_RANGE, "0,33200,1,or_greater"), 6900);  // Roughly 144 FPS

        GLOBAL_DEF("display/window/ios/allow_high_refresh_rate", true);
        GLOBAL_DEF("display/window/ios/hide_home_indicator", true);
        GLOBAL_DEF("display/window/ios/hide_status_bar", true);
        GLOBAL_DEF("display/window/ios/suppress_ui_gesture", true);

        // XR project settings.
        GLOBAL_DEF_RST_BASIC("xr/openxr/enabled", false);

        GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.STRING, "xr/openxr/default_action_map", PropertyHint.PROPERTY_HINT_FILE, "*.tres"), "res://openxr_action_map.tres");
        GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.INT,    "xr/openxr/form_factor",        PropertyHint.PROPERTY_HINT_ENUM, "Head Mounted,Handheld"), "0");
        GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.INT,    "xr/openxr/view_configuration", PropertyHint.PROPERTY_HINT_ENUM, "Mono,Stereo"), "1"); // "Mono,Stereo,Quad,Observer"
        GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.INT,    "xr/openxr/reference_space",    PropertyHint.PROPERTY_HINT_ENUM, "Local,Stage"), "1");

        GLOBAL_DEF_BASIC("xr/openxr/submit_depth_buffer", false);

        engine.FrameDelay = frameDelay;

        messageQueue = new MessageQueue();

        engine.StartupBenchmarkEndMeasure(); // core

        return secondPhase ? Setup2() : Error.OK;
    error:
        // Todo - main\main.cpp[1889:1928]

        UnregisterCoreDriverTypes();
        UnregisterCoreExtensions();
        UnregisterCoreTypes();

        // Todo - main\main.cpp[1937:1939]

        OS.Singleton.FinalizeCore();

        // Todo - main\main.cpp[1941]

        return exitCode;
    }

    public static bool Start()
    {
        if (ERR_FAIL_COND_V(!startSuccess))
        {
            return false;
        }

        var hasicon       = false;
        var positionalArg = default(string);
        var gamePath      = default(string);
        var script        = default(string);
        var checkOnly     = false;

        #if TOOLS_ENABLED
        var docToolPath                 = default(string);
        var docBase                     = true;
        var exportPreset                = default(string);
        var exportDebug                 = false;
        var exportPackOnly              = false;
        var convertingProject           = false;
        var validatingConvertingProject = false;
        #endif

        mainTimerSync.Init(OS.Singleton.TicksUsec);
        var args = OS.Singleton.CmdlineArgs;

        for (var i = 0; i < args.Count; i++)
        {
            // First check parameters that do not have an argument to the right.

            // Doctest Unit Testing Handler
            // Designed to override and pass arguments to the unit test handler.
            if (args[i] == "--check-only")
            {
                checkOnly = true;
            #if TOOLS_ENABLED
            }
            else if (args[i] == "--no-docbase")
            {
                docBase = false;
            }
            else if (args[i] == "--convert-3to4")
            {
                convertingProject = true;
            }
            else if (args[i] == "--validate-conversion-3to4")
            {
                validatingConvertingProject = true;
            }
            else if (args[i] is "-e" or "--editor")
            {
                editor = true;
            }
            else if (args[i] is "-p" or "--project-manager")
            {
                projectManager = true;
            #endif
            }
            else if (args[i].Length > 0 && args[i][0] != '-' && string.IsNullOrEmpty(positionalArg))
            {
                positionalArg = args[i];

                if (
                    args[i].EndsWith(".scn") ||
                    args[i].EndsWith(".tscn") ||
                    args[i].EndsWith(".escn") ||
                    args[i].EndsWith(".res") ||
                    args[i].EndsWith(".tres")
                )
                {
                    // Only consider the positional argument to be a scene path if it ends with
                    // a file extension associated with godot scenes. This makes it possible
                    // for projects to parse command-line arguments for custom CLI arguments
                    // or other file extensions without trouble. This can be used to implement
                    // "drag-and-drop onto executable" logic, which can prove helpful
                    // for non-game applications.
                    gamePath = args[i];
                }
            }
            // Then parameters that have an argument to the right.
            else if (i < args.Count - 1)
            {
                var parsedPair = true;
                if (args[i] is "-s" or "--script")
                {
                    script = args[i + 1];
                #if TOOLS_ENABLED
                }
                else if (args[i] == "--doctool")
                {
                    docToolPath = args[i + 1];
                    if (docToolPath.StartsWith("-"))
                    {
                        // Assuming other command line arg, so default to cwd.
                        docToolPath = ".";
                        parsedPair = false;
                    }
                }
                else if (args[i] == "--export-release")
                {
                    editor = true; //needs editor
                    exportPreset = args[i + 1];
                }
                else if (args[i] == "--export-debug")
                {
                    editor = true; //needs editor
                    exportPreset = args[i + 1];
                    exportDebug = true;
                }
                else if (args[i] == "--export-pack")
                {
                    editor = true;
                    exportPreset = args[i + 1];
                    exportPackOnly = true;
                #endif
                }
                else
                {
                    // The parameter does not match anything known, don't skip the next argument
                    parsedPair = false;
                }
                if (parsedPair)
                {
                    i++;
                }
            }
            #if TOOLS_ENABLED
            // Handle case where no path is given to --doctool.
            else if (args[i] == "--doctool")
            {
                docToolPath = ".";
            }
            #endif
        }

        var minimumTimeMsec = GLOBAL_DEF(new PropertyInfo(VariantType.INT, "application/boot_splash/minimum_display_time", PropertyHint.PROPERTY_HINT_RANGE, "0,100,1,or_greater,suffix:ms"), 0);

        // TODO main\main.cpp[2404:2506]

        #if TOOLS_ENABLED
        if (!editor && !projectManager && !IsCmdlineTool && string.IsNullOrEmpty(script) && string.IsNullOrEmpty(gamePath))
        {
            // If we end up here, it means we didn't manage to detect what we want to run.
            // Let's throw an error gently. The code leading to this is pretty brittle so
            // this might end up triggered by valid usage, in which case we'll have to
            // fine-tune further.
            OS.Singleton.Alert("Couldn't detect whether to run the editor, the project manager or a specific project. Aborting.");
            return ERR_FAIL_V_MSG(false, "Couldn't detect whether to run the editor, the project manager or a specific project. Aborting.");
        }
        #endif

        var mainLoop = default(MainLoop);

        if (editor)
        {
            mainLoop = new SceneTree();
        }

        var mainLoopType = GLOBAL_DEF("application/run/main_loop_type", nameof(SceneTree));

        // TODO main\main.cpp[2525:2576]

        if (mainLoop == null && mainLoopType == null)
        {
            mainLoopType = typeof(SceneTree).FullName!;
        }

        if (mainLoop == null)
        {
            if (ClassDB.GetType(mainLoopType) is Type type)
            {
                #pragma warning disable IL2072, IL2057
                var ml = Activator.CreateInstance(type);
                #pragma warning restore IL2072, IL2057

                if (ERR_FAIL_COND_V_MSG(ml == null, "Can't instance MainLoop type."))
                {
                    return false;
                }

                mainLoop = ml as MainLoop;

                if (mainLoop == null)
                {
                    return ERR_FAIL_V_MSG(false, "Invalid MainLoop type.");
                }
            }
            else
            {
                OS.Singleton.Alert("Error: MainLoop type doesn't exist: " + mainLoopType);

                return false;
            }
        }

        if (mainLoop is SceneTree sml)
        {
            // TODO main\main.cpp[2600:2612]

            var embedSubwindows = GLOBAL_DEF("display/window/subwindows/embed_subwindows", true);

            if (singleWindow || !projectManager && !editor && embedSubwindows || !DisplayServer.Singleton.HasFeature(DisplayServer.Feature.FEATURE_SUBWINDOWS))
            {
                sml.Root.EmbeddingSubwindows = true;
            }

            // TODO main\main.cpp[2620:2694]

            #if TOOLS_ENABLED
            var editorNode = default(EditorNode);
            if (editor)
            {
                Engine.Singleton.StartupBenchmarkBeginMeasure("editor");
                editorNode = new EditorNode() { PostInitialize = true };
                sml.Root.AddChild(editorNode);

                if (!string.IsNullOrEmpty(exportPreset))
                {
                    editorNode.ExportPreset(exportPreset, positionalArg, exportDebug, exportPackOnly);
                    gamePath = ""; // Do not load anything.
                }

                Engine.Singleton.StartupBenchmarkEndMeasure();

                editorNode.SetUseStartupBenchmark(useStartupBenchmark, startupBenchmarkFile!);
                // Editor takes over
                useStartupBenchmark  = false;
                startupBenchmarkFile = null;
            }
            #endif
            GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.STRING, "display/window/stretch/mode",   PropertyHint.PROPERTY_HINT_ENUM, "disabled,canvas_items,viewport"), "disabled");
            GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.STRING, "display/window/stretch/aspect", PropertyHint.PROPERTY_HINT_ENUM, "ignore,keep,keep_width,keep_height,expand"), "keep");
            GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.FLOAT, "display/window/stretch/scale",   PropertyHint.PROPERTY_HINT_RANGE, "0.5,8.0,0.01"), 1.0);

            sml.AutoAcceptQuit = GLOBAL_DEF("application/config/auto_accept_quit", true);
            sml.QuitOnGoBack   = GLOBAL_DEF("application/config/quit_on_go_back", true);

            GLOBAL_DEF_BASIC("gui/common/snap_controls_to_pixels", true);
            GLOBAL_DEF_BASIC("gui/fonts/dynamic_fonts/use_oversampling", true);

            GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.INT, "rendering/textures/canvas_textures/default_texture_filter", PropertyHint.PROPERTY_HINT_ENUM, "Nearest,Linear,Linear Mipmap,Nearest Mipmap"), 1);
            GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.INT, "rendering/textures/canvas_textures/default_texture_repeat", PropertyHint.PROPERTY_HINT_ENUM, "Disable,Enable,Mirror"), 0);

            // TODO main\main.cpp[2727:2784]

            #if TOOLS_ENABLED
            if (editor)
            {
                var editorEmbedSubwindows = EditorSettings.Singleton.GetSetting<bool>("interface/editor/single_window_mode");

                if (editorEmbedSubwindows)
                {
                    sml.Root.EmbeddingSubwindows = true;
                }
            }
            #endif
            // TODO main\main.cpp[2797:2847]

            #if TOOLS_ENABLED
            if (projectManager)
            {
                Engine.Singleton.StartupBenchmarkBeginMeasure("project_manager");
                Engine.Singleton.EditorHint = true;

                var pmanager       = new ProjectManager() { PostInitialize = true };
                var progressDialog = new ProgressDialog() { PostInitialize = true };

                pmanager.AddChild(progressDialog);
                sml.Root.AddChild(pmanager);

                DisplayServer.Singleton.Context = DisplayServer.ContextType.CONTEXT_PROJECTMAN;
                Engine.Singleton.StartupBenchmarkEndMeasure();
            }

            if (projectManager || editor)
            {
                // Load SSL Certificates from Editor Settings (or builtin)
                // TODO Crypto::load_default_certificates(EditorSettings.Singleton.get_setting("network/tls/editor_tls_certificates").operator String());
            }
            #endif
        }

        #region TODO
        //         if (!hasicon && OS.Singleton.get_bundle_icon_path().is_empty())
        //         {
        //             Ref<Image> icon = memnew(Image(app_icon_png));
        //             DisplayServer.Singleton.set_icon(icon);
        //         }
        #endregion TODO

        OS.Singleton.MainLoop = mainLoop;

        #region TODO
        //         if (movie_writer)
        //         {
        //             movie_writer->begin(DisplayServer.Singleton.window_get_size(), fixed_fps, Engine.Singleton.get_write_movie_path());
        //         }

        //         if (minimumTimeMsec)
        //         {
        //             uint64_t minimum_time = 1000 * minimumTimeMsec;
        //             uint64_t elapsed_time = OS.Singleton.get_ticks_usec();
        //             if (elapsed_time < minimum_time)
        //             {
        //                 OS.Singleton.delay_usec(minimum_time - elapsed_time);
        //             }
        //         }

        //         if (use_startup_benchmark)
        //         {
        //             Engine.Singleton.startup_dump(startup_benchmark_file);
        //             startup_benchmark_file = String();
        //         }
        #endregion TODO

        return true;
    }
    #endregion public static methods
}
