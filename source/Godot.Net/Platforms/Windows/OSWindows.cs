#define WINDOWS_DEBUG_OUTPUT_ENABLED
#define WINDOWS_SUBSYSTEM_CONSOLE

namespace Godot.Net.Platforms.Windows;

using System.Diagnostics;
using Godot.Net.Core.Error;
using Godot.Net.Core.IO;
using Godot.Net.Core.OS;
using Godot.Net.Extensions;
using Godot.Net.Platforms.Windows.Native;
using Godot.Net.Platforms.Windows.Native.Winmm;
using Godot.Net.Servers;
using Main = Main.Main;

#pragma warning disable IDE0052, IDE0044, CS0649 // TODO - Remove

public partial class OSWindows : OS
{
    #region private static fields
    private static string? cachePathCache;
    #endregion private static fields

    #region public static readonly properties
    public static HINSTANCE HInstance => Process.GetCurrentProcess().Handle;
    public static new OS    Singleton { get; private set; } = null!;
    #endregion public static readonly properties

    #region private readonly fields
    private readonly List<Logger>                   loggers    = new();
    private readonly Dictionary<DWORD, ProcessInfo> processMap = new();
    #endregion private readonly fields

    #region private fields
    private bool             dwriteInit;
    private ErrorHandlerList errorHandlerList = null!;
    private CompositeLogger? logger;
    private long             ticksPerSecond;
    private long             ticksStart;
    #endregion private fields

    #region protected override readonly properties
    protected override CompositeLogger? Logger => this.logger;
    #endregion protected override readonly properties

    #region public properties
    public HWND MainWindow { get; set; }
    #endregion public properties

    #region public override readonly properties
    public override string CachePath
    {
        get
        {
            if (string.IsNullOrEmpty(cachePathCache))
            {
                if (Environment.GetEnvironmentVariable("LOCALAPPDATA") is string localAppData)
                {
                    cachePathCache = localAppData;
                }

                if (string.IsNullOrEmpty(cachePathCache) && Environment.GetEnvironmentVariable("TEMP") is string temp)
                {
                    cachePathCache = temp;
                }

                if (string.IsNullOrEmpty(cachePathCache))
                {
                    cachePathCache = ConfigPath;
                }
            }
            return cachePathCache;
        }
    }
    public override long TicksUsec
    {
        get
        {
            // This is the number of clock ticks since start
            Kernel32.QueryPerformanceCounter(out var ticks);
            // Subtract the ticks at game start to get
            // the ticks since the game started
            ticks -= this.ticksStart;

            // Divide by frequency to get the time in seconds
            // original calculation shown below is subject to overflow
            // with high ticks_per_second and a number of days since the last reboot.
            // time = ticks * 1000000L / ticks_per_second;

            // we can prevent this by either using 128 bit math
            // or separating into a calculation for seconds, and the fraction
            var seconds = ticks / this.ticksPerSecond;

            // compiler will optimize these two into one divide
            var leftover = ticks % this.ticksPerSecond;

            // remainder
            var time = leftover * 1000000L / this.ticksPerSecond;

            // seconds
            time += seconds * 1000000L;

            return time;
        }
    }

    public override string UserDataDir
    {
        get
        {
            var appname = GetSafeDirName(GLOBAL_GET<string>("application/config/name") ?? "");
            if (!string.IsNullOrEmpty(appname))
            {
                var useCustomDir = GLOBAL_GET<bool>("application/config/use_custom_user_dir");
                if (useCustomDir)
                {
                    var customDir = GetSafeDirName(GLOBAL_GET<string>("application/config/custom_user_dir_name") ?? "", true);

                    if (string.IsNullOrEmpty(customDir))
                    {
                        customDir = appname;
                    }
                    return Path.Join(DataPath, customDir).Replace("\\", "/");
                }
                else
                {
                    return Path.Join(DataPath, GodotDirName, "app_userdata", appname).Replace("\\", "/");
                }
            }

            return Path.Join(DataPath, GodotDirName, "app_userdata", "[unnamed project]").Replace("\\", "/");
        }
    }
    #endregion public override readonly properties

    public OSWindows()
    {
        // TODO - platform\windows\os_windows.cpp[1485:1490]

        DisplayServerWindows.RegisterWindowsDriver();

        // TODO - platform\windows\os_windows.cpp[1499:1504]

        this.logger = new CompositeLogger(new() { new TerminalLogger() });
    }

    #region private methods
    #if WINDOWS_DEBUG_OUTPUT_ENABLED
    private static void ErrorHandler(object userData, string function, string file, int line, string error, string message, bool editorNotify, ErrorHandlerType type)
    {
        var output = !string.IsNullOrEmpty(message) ? message : $"{file}:{line} - {error}{(editorNotify ? " (User)" : "")}\n";

        Debug.Write(output);
    }
    #endif

    private static void RedirectIOToConsole()
    {
        if (Kernel32.AttachConsole(Kernel32.ATTACH_PARENT_PROCESS))
        {
            // TODO - platform\windows\os_windows.cpp[103:105]
        }
    }
    #endregion private methods

    #region public override methods
    public override void Alert(string alert, string title) =>
        Console.WriteLine($"{title}: {alert}");

    public override bool CheckInternalFeatureSupport(string feature) =>
        feature == "system_fonts" ? this.dwriteInit : feature == "pc";

    public override string? GetSystemDir(SystemDir dir, bool sharedStorage = true)
    {
        var id = dir switch
        {
            SystemDir.SYSTEM_DIR_DESKTOP   => Shell32.KNOWN_FOLDERS.Desktop,
            SystemDir.SYSTEM_DIR_DCIM      => Shell32.KNOWN_FOLDERS.Pictures,
            SystemDir.SYSTEM_DIR_DOCUMENTS => Shell32.KNOWN_FOLDERS.Documents,
            SystemDir.SYSTEM_DIR_DOWNLOADS => Shell32.KNOWN_FOLDERS.Downloads,
            SystemDir.SYSTEM_DIR_MOVIES    => Shell32.KNOWN_FOLDERS.Videos,
            SystemDir.SYSTEM_DIR_MUSIC     => Shell32.KNOWN_FOLDERS.Music,
            SystemDir.SYSTEM_DIR_PICTURES  => Shell32.KNOWN_FOLDERS.Pictures,
            SystemDir.SYSTEM_DIR_RINGTONES => Shell32.KNOWN_FOLDERS.Music,
            _ => default,
        };

        var res = Shell32.SHGetKnownFolderPath(id, 0, default, out var path);

        return ERR_FAIL_COND_V(res != 0) ? null : path;
    }

    public override void DelayUsec(uint usec)
    {
        if (usec < 1000)
        {
            Thread.Sleep(1);
        }
        else
        {
            Thread.Sleep((int)(usec / 1000));
        }
    }

    public override void FinalizeCore() => throw new NotImplementedException();

    public override int GetEmbeddedPckOffset()
    {
        using var file   = File.OpenRead(Environment.ProcessPath!);
        using var reader = new BinaryReader(file);

        // Process header.
        file.Seek(0x3c, SeekOrigin.Begin);

        var pePos = reader.ReadUInt32();

        file.Seek(pePos, SeekOrigin.Begin);

        var magic = reader.ReadUInt32();

        if (magic != 0x00004550)
        {
            return 0;
        }

        var headerPos = file.Position;

        file.Seek(headerPos + 2, SeekOrigin.Begin);

        int numSections = reader.ReadUInt16(); // 236 Remove Comment

        file.Seek(headerPos + 16, SeekOrigin.Begin);

        var optHeaderSize = reader.ReadUInt16(); // 240 Remove Comment

        // Skip rest of header + optional header to go to the section headers.
        file.Seek(file.Position + 2 + optHeaderSize, SeekOrigin.Begin);
        var sectionTablePos = file.Position; // 492 Remove Comment

        // Search for the "pck" section.
        var off = 0u;

        for (var i = 0; i < numSections; ++i)
        {
            var sectionHeaderPos = sectionTablePos + i * 40;

            file.Seek(sectionHeaderPos, SeekOrigin.Begin);

            var sectionName = new byte[9];

            file.Read(sectionName, 0, sectionName.Length);

            sectionName[8] = (byte)'\0';

            if (string.Compare(sectionName.ConvertToString().TrimEnd('\0'), "pck") == 0)
            {
                file.Seek(sectionHeaderPos + 20, SeekOrigin.Begin);
                off = reader.ReadUInt32();
                break;
            }
        }

        return (int)off;
    }

    public override void Initialize()
    {
        // crash_handler.initialize();

        #if WINDOWS_DEBUG_OUTPUT_ENABLED
        this.errorHandlerList = new(this, ErrorHandler);
        AddErrorHandler(this.errorHandlerList);
        #endif

        #if WINDOWS_SUBSYSTEM_CONSOLE
        RedirectIOToConsole();
        #endif

        // FileAccess::make_default<FileAccessWindows>(FileAccess::ACCESS_RESOURCES);
        // FileAccess::make_default<FileAccessWindows>(FileAccess::ACCESS_USERDATA);
        // FileAccess::make_default<FileAccessWindows>(FileAccess::ACCESS_FILESYSTEM);
        // DirAccess::make_default<DirAccessWindows>(DirAccess::ACCESS_RESOURCES);
        // DirAccess::make_default<DirAccessWindows>(DirAccess::ACCESS_USERDATA);
        // DirAccess::make_default<DirAccessWindows>(DirAccess::ACCESS_FILESYSTEM);

        // NetSocketPosix::make_default();

        // // We need to know how often the clock is updated
        Kernel32.QueryPerformanceFrequency(out this.ticksPerSecond);
	    Kernel32.QueryPerformanceCounter(out this.ticksStart);

        // // set minimum resolution for periodic timers, otherwise Sleep(n) may wait at least as
        // //  long as the windows scheduler resolution (~16-30ms) even for calls like Sleep(1)
        Winmm.TimeBeginPeriod(1);

        // process_map = memnew((HashMap<ProcessID, ProcessInfo>));

        // Add current Godot PID to the list of known PIDs
        Kernel32.PROCESS_INFORMATION currentPiPi = new()
        {
            hProcess = Kernel32.GetCurrentProcess()
        };

        var currentPi = new ProcessInfo(default, currentPiPi);

        this.processMap.Add(Kernel32.GetCurrentProcessId(), currentPi);

        // IPUnix::make_default();
        // main_loop = nullptr;

        // CoInitialize(nullptr);
        // HRESULT hr = DWriteCreateFactory(DWRITE_FACTORY_TYPE_SHARED, __uuidof(IDWriteFactory), reinterpret_cast<IUnknown **>(&dwrite_factory));
        // if (SUCCEEDED(hr)) {
        //     hr = dwrite_factory->GetSystemFontCollection(&font_collection, false);
        //     if (SUCCEEDED(hr)) {
        //         dwrite_init = true;
        //         hr = dwrite_factory->QueryInterface(&dwrite_factory2);
        //         if (SUCCEEDED(hr)) {
        //             hr = dwrite_factory2->GetSystemFontFallback(&system_font_fallback);
        //             if (SUCCEEDED(hr)) {
        //                 dwrite2_init = true;
        //             }
        //         }
        //     }
        // }
        // if (!dwrite_init) {
        //     print_verbose("Unable to load IDWriteFactory, system font support is disabled.");
        // } else if (!dwrite2_init) {
        //     print_verbose("Unable to load IDWriteFactory2, automatic system font fallback is disabled.");
        // }

        // FileAccessWindows::initialize();
    }

    public override void Run()
    {
        if (this.MainLoop == null)
        {
            return;
        }

        this.MainLoop.Initialize();

        while (true)
        {
            DisplayServer.Singleton.ProcessEvents(); // get rid of pending events
            if (Main.Iteration())
            {
                break;
            }
        }

        this.MainLoop.Complete();
    }
    #endregion public override methods
}
