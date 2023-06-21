#define VULKAN_ENABLED
#define GLES3_ENABLED

namespace Godot.Net.Platforms.Windows;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Godot.Net.Core.Config;
using Godot.Net.Core.Error;
using Godot.Net.Core.Input;
using Godot.Net.Core.Math;
using Godot.Net.Core.OS;
using Godot.Net.Drivers.GLES3;
using Godot.Net.Drivers.Vulkan;
using Godot.Net.Platforms.Windows.Native;
using Godot.Net.Scene.Main;
using Godot.Net.Servers;
using Godot.Net.Servers.Rendering.RendererRD;

using static Godot.Net.Platforms.Windows.Native.Macros;

#pragma warning disable IDE0051, CS0169, IDE0044, CS0414, CS0649, IDE0052, IDE0060, CS8618 // TODO - Remove

public partial class DisplayServerWindows : DisplayServer
{
    #region public delegates
    private delegate DWORD GetImmersiveColorFromColorSetExPtr(UINT dwImmersiveColorSet, UINT dwImmersiveColorType, bool bIgnoreHighContrast, UINT dwHighContrastCacheMode);
    private delegate int GetImmersiveColorTypeFromNamePtr(in WCHAR name);
    private delegate int GetImmersiveUserColorSetPreferencePtr(bool bForceCheckRegistry, bool bSkipCheckOnFail);
    private delegate bool GetPointerPenInfoPtr(uint id, ref POINTER_PEN_INFO penInfo);
    private delegate bool GetPointerTypePtr(uint id, ref POINTER_INPUT_TYPE type);
    private delegate HRESULT RtlGetVersionPtr(ref OSVERSIONINFOW lpVersionInformation);
    private delegate bool ShouldAppsUseDarkModePtr();
    #endregion public delegates

    #region const
    private const ushort DVC_NPRESSURE                    = 15;
    private const ushort DVC_ORIENTATION                  = 17;
    private const ushort DVC_ROTATION                     = 18;
    private const ushort DVC_TPRESSURE                    = 16;
    private const uint   MI_WP_SIGNATURE                  = 0xFF515700;
    private const ushort PEN_FLAG_ERASER                  = 0x00000004;
    private const ushort PEN_FLAG_INVERTED                = 0x00000002;
    private const ushort PEN_MASK_PRESSURE                = 0x00000001;
    private const ushort PEN_MASK_TILT_X                  = 0x00000004;
    private const ushort PEN_MASK_TILT_Y                  = 0x00000008;
    private const ushort POINTER_MESSAGE_FLAG_FIRSTBUTTON = 0x00000010;
    private const uint   SIGNATURE_MASK                   = 0xFFFFFF00;
    private const ushort TPS_INVERT                       = 0x0010; /* 1.1 */
    private const ushort WM_POINTERENTER                  = 0x0249;
    private const ushort WM_POINTERLEAVE                  = 0x024A;
    private const ushort WM_POINTERUPDATE                 = 0x0245;
    private const ushort WT_CSRCHANGE                     = 0x7FF7;
    private const ushort WT_INFOCHANGE                    = 0x7FF6;
    private const ushort WT_PACKET                        = 0x7FF0;
    private const ushort WT_PROXIMITY                     = 0x7FF5;
    private const ushort WTI_DEFSYSCTX                    = 4;
    private const ushort WTI_DEVICES                      = 100;
    #endregion const

    private static readonly object padlock = new();
    private static readonly User32.IDC_STANDARD_CURSORS[] winCursors =
    {
        User32.IDC_STANDARD_CURSORS.IDC_ARROW,
        User32.IDC_STANDARD_CURSORS.IDC_IBEAM,
        User32.IDC_STANDARD_CURSORS.IDC_HAND, // Finger.
        User32.IDC_STANDARD_CURSORS.IDC_CROSS,
        User32.IDC_STANDARD_CURSORS.IDC_WAIT,
        User32.IDC_STANDARD_CURSORS.IDC_APPSTARTING,
        User32.IDC_STANDARD_CURSORS.IDC_SIZEALL,
        User32.IDC_STANDARD_CURSORS.IDC_ARROW,
        User32.IDC_STANDARD_CURSORS.IDC_NO,
        User32.IDC_STANDARD_CURSORS.IDC_SIZENS,
        User32.IDC_STANDARD_CURSORS.IDC_SIZEWE,
        User32.IDC_STANDARD_CURSORS.IDC_SIZENESW,
        User32.IDC_STANDARD_CURSORS.IDC_SIZENWSE,
        User32.IDC_STANDARD_CURSORS.IDC_SIZEALL,
        User32.IDC_STANDARD_CURSORS.IDC_SIZENS,
        User32.IDC_STANDARD_CURSORS.IDC_SIZEWE,
        User32.IDC_STANDARD_CURSORS.IDC_HELP,
    };

    #region private static fields
    private static GetImmersiveColorFromColorSetExPtr?    getImmersiveColorFromColorSetEx;
    private static GetImmersiveColorTypeFromNamePtr?      getImmersiveColorTypeFromName;
    private static GetImmersiveUserColorSetPreferencePtr? getImmersiveUserColorSetPreference;
    private static bool                                   handleEarlyWindowMessageFirstPrint = true;
    private static int                                    overallX;
    private static int                                    overallY;
    private static ShouldAppsUseDarkModePtr?              shouldAppsUseDarkMode;
    private static GetPointerPenInfoPtr?                  win8pGetPointerPenInfo;
    private static GetPointerTypePtr?                     win8pGetPointerType;
    private static bool                                   wininkAvailable;
    #endregion private static fields

    #region private readonly fields
    private readonly HCURSOR[]                       cursors = new HCURSOR[17];
    private readonly Dictionary<CursorShape, object> cursorsCache = new();
    private readonly bool                            metaMem;
    private readonly MouseMode                       mouseMode;
    private readonly string                          renderingDriver;
    private readonly Dictionary<int, Vector2<RealT>> touchState = new();
    private readonly Dictionary<int, WindowData>     windows = new();
    #endregion private readonly fields

    #region private fields
    private bool                      altMem;
    private bool                      appFocused;
    private Vector2<RealT>            center;
    private VulkanContextWindows?     contextVulkan;
    private bool                      controlMem;
    private CursorShape               cursorShape = CursorShape.CURSOR_ARROW;
    private bool                      darkTitleAvailable;
    private bool                      dropEvents;
    private GLManagerWindows?         glManager;
    private bool                      grMem;
    private HINSTANCE                 hInstance;
    private JoypadWindows             joypad;
    private bool                      keepScreenOn;
    private int                       keyEventPos;
    private MouseButtonMask           lastButtonState;
    private int                       lastFocusedWindow;
    private nint                      mouseMonitor;
    private bool                      oldInvalid = true;
    private RealT                     oldX;
    private RealT                     oldY;
    private Stack<int>                popupList = new();
    private nint                      powerRequest;
    private int                       pressrc;
    private RenderingDeviceVulkan?    renderingDeviceVulkan;
    private bool                      shiftMem;
    private long                      timeSincePopup;
    private bool                      useRawInput;
    private User32.WNDPROC?           userProc;
    private bool                      uxThemeAvailable;
    private HBRUSH                    windowBkgBrush;
    private uint                      windowBkgBrushColor;
    private int                       windowIdCounter;
    private int                       windowMouseoverId;
    private bool                      wintabAvailable;
    #endregion private fields

    #region private properties
    private BOOL IsDarkMode          => this.uxThemeAvailable && (shouldAppsUseDarkMode?.Invoke() ?? false);
    private BOOL IsDarkModeSupported => this.uxThemeAvailable;
    #endregion private properties

    #region public override readonly properties
    public override bool CanAnyWindowDraw => this.windows.Any(x => !x.Value.Minimized);
    public override bool SwapCancelOk     => true;
    #endregion public override readonly properties

    public DisplayServerWindows(string renderingDriver, WindowMode mode, VSyncMode vsyncMode, WindowFlagsBit flags, out Vector2<int> position, Vector2<int> resolution, int screen, out Error error)
    {
        Singleton = this;

        position = default;
        error    = Error.FAILED;

        this.mouseMode = MouseMode.MOUSE_MODE_VISIBLE;

        this.renderingDriver = renderingDriver;
        this.hInstance       = OSWindows.HInstance;

        // TODO - platform\windows\display_server_windows.cpp[3798]

        this.ScreenSetKeepOn(GLOBAL_GET<bool>("display/window/energy_saving/keep_screen_on"));

        // Load Windows version info.
        var osVer = new OSVERSIONINFOW
        {
            dwOSVersionInfoSize = Marshal.SizeOf<OSVERSIONINFOW>()
        };

        if (Kernel32.LoadLibraryW("ntdll.dll") is var ntLib && ntLib != default)
        {
            if (Kernel32.GetProcAddress(ntLib, "RtlGetVersion") is var rtlGetVersionPtr && rtlGetVersionPtr != default)
            {
                var rtlGetVersion = Marshal.GetDelegateForFunctionPointer<RtlGetVersionPtr>(rtlGetVersionPtr);
                rtlGetVersion(ref osVer);
            }
            Kernel32.FreeLibrary(ntLib);
        }

        // // Load UXTheme.
        if (Kernel32.LoadLibraryW("uxtheme.dll") is var uxThemeLib && uxThemeLib != default)
        {
            if (Kernel32.GetProcAddress(uxThemeLib, new(132)) is var shouldAppsUseDarkModePtr && shouldAppsUseDarkModePtr != default)
            {
                shouldAppsUseDarkMode = Marshal.GetDelegateForFunctionPointer<ShouldAppsUseDarkModePtr>(shouldAppsUseDarkModePtr);
            }

            if (Kernel32.GetProcAddress(uxThemeLib, new(95)) is var getImmersiveColorFromColorSetExPtr && getImmersiveColorFromColorSetExPtr != default)
            {
                getImmersiveColorFromColorSetEx = Marshal.GetDelegateForFunctionPointer<GetImmersiveColorFromColorSetExPtr>(getImmersiveColorFromColorSetExPtr);
            }

            if (Kernel32.GetProcAddress(uxThemeLib, new(96)) is var getImmersiveColorTypeFromNamePtr && getImmersiveColorTypeFromNamePtr != default)
            {
                getImmersiveColorTypeFromName = Marshal.GetDelegateForFunctionPointer<GetImmersiveColorTypeFromNamePtr>(getImmersiveColorTypeFromNamePtr);
            }

            if (Kernel32.GetProcAddress(uxThemeLib, new(98)) is var getImmersiveUserColorSetPreferencePtr && getImmersiveUserColorSetPreferencePtr != default)
            {
                getImmersiveUserColorSetPreference = Marshal.GetDelegateForFunctionPointer<GetImmersiveUserColorSetPreferencePtr>(getImmersiveUserColorSetPreferencePtr);
            }

            this.uxThemeAvailable = shouldAppsUseDarkMode != null && getImmersiveColorFromColorSetEx != null && getImmersiveColorTypeFromName != null && getImmersiveUserColorSetPreference != null;
            if (osVer.dwBuildNumber >= 22000)
            {
                this.darkTitleAvailable = true;
            }
        }

        // // Note: Wacom WinTab driver API for pen input, for devices incompatible with Windows Ink.
        // HMODULE wintab_lib = LoadLibraryW(L"wintab32.dll");
        // if (wintab_lib) {
        //     wintab_WTOpen = (WTOpenPtr)GetProcAddress(wintab_lib, "WTOpenW");
        //     wintab_WTClose = (WTClosePtr)GetProcAddress(wintab_lib, "WTClose");
        //     wintab_WTInfo = (WTInfoPtr)GetProcAddress(wintab_lib, "WTInfoW");
        //     wintab_WTPacket = (WTPacketPtr)GetProcAddress(wintab_lib, "WTPacket");
        //     wintab_WTEnable = (WTEnablePtr)GetProcAddress(wintab_lib, "WTEnable");

        //     wintab_available = wintab_WTOpen && wintab_WTClose && wintab_WTInfo && wintab_WTPacket && wintab_WTEnable;
        // }

        // if (wintab_available) {
        //     tablet_drivers.push_back("wintab");
        // }

        // // Note: Windows Ink API for pen input, available on Windows 8+ only.
        // HMODULE user32_lib = LoadLibraryW(L"user32.dll");
        // if (user32_lib) {
        //     win8p_GetPointerType = (GetPointerTypePtr)GetProcAddress(user32_lib, "GetPointerType");
        //     win8p_GetPointerPenInfo = (GetPointerPenInfoPtr)GetProcAddress(user32_lib, "GetPointerPenInfo");

        //     winink_available = win8p_GetPointerType && win8p_GetPointerPenInfo;
        // }

        // if (winink_available) {
        //     tablet_drivers.push_back("winink");
        // }

        // if (OS::get_singleton()->is_hidpi_allowed()) {
        //     HMODULE Shcore = LoadLibraryW(L"Shcore.dll");

        //     if (Shcore != nullptr) {
        //         typedef HRESULT(WINAPI * SetProcessDpiAwareness_t)(SHC_PROCESS_DPI_AWARENESS);

        //         SetProcessDpiAwareness_t SetProcessDpiAwareness = (SetProcessDpiAwareness_t)GetProcAddress(Shcore, "SetProcessDpiAwareness");

        //         if (SetProcessDpiAwareness) {
        //             SetProcessDpiAwareness(SHC_PROCESS_SYSTEM_DPI_AWARE);
        //         }
        //     }
        // }

        unsafe
        {
            using var engine = new LPCWSTR("Engine");

            var wc = new User32.WNDCLASSEXW
            {
                cbSize        = Marshal.SizeOf<User32.WNDCLASSEXW>(),
                style         = User32.CLASS_STYLES.CS_HREDRAW | User32.CLASS_STYLES.CS_VREDRAW | User32.CLASS_STYLES.CS_OWNDC | User32.CLASS_STYLES.CS_DBLCLKS,
                lpfnWndProc   = new(WndProc),
                cbClsExtra    = 0,
                cbWndExtra    = 0,
                hInstance     = this.hInstance != default ? this.hInstance : new(Kernel32.GetModuleHandleW(default)),
                // hIcon         = LoadIcon(nullptr, IDI_WINLOGO),
                hCursor       = default,
                hbrBackground = default,
                lpszMenuName  = default,
                lpszClassName = engine
            };

            if (User32.RegisterClassExW(wc) == 0)
            {
                _ = User32.MessageBoxW(default, "Failed To Register The Window Class.", "ERROR", User32.MESSAGE_BOX_OPTIONS.MB_OK | User32.MESSAGE_BOX_OPTIONS.MB_ICONEXCLAMATION);

                error = Error.ERR_UNAVAILABLE;

                return;
            }
        }


        // TODO - platform\windows\display_server_windows.cpp[3894]

        #if VULKAN_ENABLED
        if (renderingDriver == "vulkan")
        {
            this.contextVulkan = new VulkanContextWindows();

            if (this.contextVulkan.Initialize() != Error.OK)
            {
                this.contextVulkan = null;

                error = Error.ERR_UNAVAILABLE;

                return;
            }
        }
        #endif

        #if GLES3_ENABLED
        if (renderingDriver == "opengl3")
        {
            var openglApiType = GLManagerWindows.ContextType.GLES_3_0_COMPATIBLE;

            this.glManager = new GLManagerWindows(openglApiType);

            if (this.glManager.Initialize() != Error.OK)
            {
                this.glManager = null;

                error = Error.ERR_UNAVAILABLE;

                return;
            }

            RasterizerGLES3.MakeCurrent();
        }
        #endif

        unsafe
        {
            this.mouseMonitor = User32.SetWindowsHookExW(User32.WINDOWS_HOOK_TYPE.WH_MOUSE, new(MouseProcHook), default, Kernel32.GetCurrentThreadId());
        }

        var windowPosition = new Vector2<int>();

        if (position != default)
        {
            windowPosition = position;
        }
        else
        {
            if (screen == SCREEN_OF_MAIN_WINDOW)
            {
                screen = SCREEN_PRIMARY;
            }

            windowPosition = this.ScreenGetPosition(screen) + (this.ScreenGetSize(screen) - resolution) / 2;
        }

        var mainWindow = this.CreateWindow(mode, vsyncMode, 0, new Rect2<int>(windowPosition, resolution));

        if (ERR_FAIL_COND_MSG(mainWindow == INVALID_WINDOW_ID, "Failed to create main window."))
        {
            return;
        }

        this.joypad = new JoypadWindows(this.windows[MAIN_WINDOW_ID].HWnd);

        for (var i = 0; i < (int)WindowFlags.WINDOW_FLAG_MAX; i++)
        {
            if (((int)flags & (1 << i)) == 1)
            {
                this.WindowSetFlag((WindowFlags)i, true, mainWindow);
            }
        }

        this.ShowWindow(MAIN_WINDOW_ID);

        #if VULKAN_ENABLED
        if (renderingDriver == "vulkan")
        {
            this.renderingDeviceVulkan = new RenderingDeviceVulkan();
            this.renderingDeviceVulkan.Initialize(this.contextVulkan!);

            RendererCompositorRD.MakeCurrent();
        }
        #endif

        if (!Engine.Singleton.IsEditorHint && !OS.Singleton.InLowProcessorUsageMode)
        {
            // Increase priority for projects that are not in low-processor mode (typically games)
            // to reduce the risk of frame stuttering.
            // This is not done for the editor to prevent importers or resource bakers
            // from making the system unresponsive.

            Kernel32.SetPriorityClass(new(Kernel32.GetCurrentProcess()), Kernel32.PROCESS_CREATION_FLAGS.ABOVE_NORMAL_PRIORITY_CLASS);

            unsafe
            {
                var handle = AVRT.AvSetMmThreadCharacteristicsW("Games", out var index);

                if (handle != 0)
                {
                    AVRT.AvSetMmThreadPriority(handle, AVRT.AVRT_PRIORITY.AVRT_PRIORITY_CRITICAL);
                }
            }

            // This is needed to make sure that background work does not starve the main thread.
            // This is only setting the priority of this thread, not the whole process.
            Kernel32.SetThreadPriority(Kernel32.GetCurrentThread(), Kernel32.THREAD_PRIORITY.THREAD_PRIORITY_TIME_CRITICAL);
        }

        this.cursorShape = CursorShape.CURSOR_ARROW;

        this.UpdateRealMousePosition(MAIN_WINDOW_ID);

        error = Error.OK;

        ((OSWindows)OS.Singleton).MainWindow = this.windows[MAIN_WINDOW_ID].HWnd;
    }

    #region private static
    private static DisplayServer CreateFunc(string renderingDriver, WindowMode mode, VSyncMode vsyncMode, WindowFlagsBit flags, out Vector2<int> position, Vector2<int> resolution, int screen, out Error error)
    {
        var ds = new DisplayServerWindows(renderingDriver, mode, vsyncMode, flags, out position, resolution, screen, out error);

        if (error != Error.OK)
        {
            if (renderingDriver == "vulkan")
            {
                var executableName = Path.Join(AppContext.BaseDirectory, "Godot.Net.exe");

                OS.Singleton.Alert(
                    $"""
                    Your video card driver does not support the selected Vulkan version.
                    Please try updating your GPU driver or try using the OpenGL 3 driver.
                    You can enable the OpenGL 3 driver by starting the engine from the",
                    command line with the command:'.
                    {executableName} --rendering-driver opengl3'.
                    If you have updated your graphics drivers recently, try rebooting.
                    """,
                    "Unable to initialize Video driver"
                );
            }
            else
            {
                OS.Singleton.Alert(
                    """
                    Your video card driver does not support the selected OpenGL version.
                    Please try updating your GPU driver.
                    If you have updated your graphics drivers recently, try rebooting.
                    """,
                    "Unable to initialize Video driver"
                );
            }
        }
        return ds;
    }

    private static Vector2<int> MouseGetPosition()
    {
        User32.GetCursorPos(out var p);

        return new Vector2<int>(p.x, p.y) - GetScreensOrigin();
    }

    private static List<string> GetRenderingDriversFunc()
    {
        var drivers = new List<string>();

        #if VULKAN_ENABLED
        drivers.Add("vulkan");
        #endif

        #if GLES3_ENABLED
        drivers.Add("opengl3");
        #endif

        return drivers;
    }

    private static Vector2<int> GetScreensOrigin()
    {
        lock (padlock)
        {
            var data = new EnumPosData();

            User32.EnumDisplayMonitors(default, default, new(MonitorEnumProcOrigin), ref data);

            return data.Position;
        }
    }

    private static bool GetWindowEarlyClearOverride(out Color color)
    {
        if (WindowEarlyClearOverrideEnabled)
        {
            color = WindowEarlyClearOverrideColor;
            return true;
        }
        else if (RS.Singleton != null)
        {
            color = RS.Singleton.DefaultClearColor;
            return true;
        }
        else
        {
            color = default;
            return false;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool IsPenEvent(WORD dw) => (dw & SIGNATURE_MASK) != 0;
    // This one tells whether the event comes from touchscreen (and not from pen).

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool IsTouchEvent(WORD dw) => IsPenEvent(dw) && (dw & 0x80) != 0;

    private static unsafe BOOL MonitorEnumProcCount(HMONITOR hMonitor, HDC hdcMonitor, LPRECT lprcMonitor, LPARAM dwData)
    {
        var data = (int*)dwData;

        (*data)++;

        return true;
    }

    private static unsafe BOOL MonitorEnumProcDpi(HMONITOR hMonitor, HDC hdcMonitor, LPRECT lprcMonitor, LPARAM dwData)
    {
        var data = (EnumDpiData*)dwData;

        if (data->Count == data->Screen)
        {
            data->Dpi = QueryDpiForMonitor(hMonitor);
        }

        data->Count++;

        return true;
    }

    private static unsafe BOOL MonitorEnumProcOrigin(HMONITOR hMonitor, HDC hdcMonitor, LPRECT lprcMonitor, LPARAM dwData)
    {
        var data = (EnumPosData*)dwData;
        var rect = (RECT*)lprcMonitor;

        data->Position = new(Math.Min(data->Position.X, rect->left), Math.Min(data->Position.Y, rect->top));

        return true;
    }

    private static unsafe BOOL MonitorEnumProcPos(HMONITOR hMonitor, HDC hdcMonitor, LPRECT lprcMonitor, LPARAM dwData)
    {
        var data = (EnumPosData*)dwData;
        var rect = (RECT*)lprcMonitor;

        if (data->Count == data->Screen)
        {
            data->Position = new(rect->left, rect->top);
        }

        data->Count++;

        return true;
    }

    private static unsafe BOOL MonitorEnumProcPrim(HMONITOR hMonitor, HDC hdcMonitor, LPRECT lprcMonitor, LPARAM dwData)
    {
        var data = (EnumScreenData*)dwData;
        var rect = (RECT*)lprcMonitor;

        if (rect->left == 0 && rect->top == 0)
        {
            data->Screen = data->Count;

            return false;
        }

        data->Count++;

        return true;
    }

    private static unsafe BOOL MonitorEnumProcScreen(HMONITOR hMonitor, HDC hdcMonitor, LPRECT lprcMonitor, LPARAM dwData)
    {
        var data = (EnumScreenData*)dwData;

        if (data->Monitor == hMonitor)
        {
            data->Screen = data->Count;
        }

        data->Count++;

        return true;
    }

    private static unsafe BOOL MonitorEnumProcSize(HMONITOR hMonitor, HDC hdcMonitor, LPRECT lprcMonitor, LPARAM dwData)
    {
        var data = (EnumSizeData*)dwData;
        var rect = (RECT*)lprcMonitor;

        if (data->Count == data->Screen)
        {
            data->Rect = data->Rect with
            {
                Size = new(rect->right - rect->left, rect->bottom - rect->top)
            };
        }

        data->Count++;

        return true;
    }

    private static unsafe BOOL MonitorEnumProcUsableSize(HMONITOR hMonitor, HDC hdcMonitor, LPRECT lprcMonitor, LPARAM dwData)
    {
        var data = (EnumRectData*)dwData;

        if (data->Count == data->Screen)
        {
            var mInfo = new User32.MONITORINFO
            {
                cbSize = Marshal.SizeOf<User32.MONITORINFO>()
            };

            User32.GetMonitorInfoW(hMonitor, &mInfo);

            data->Rect = new(
                new(mInfo.rcWork.left, mInfo.rcWork.top),
                new(mInfo.rcWork.right - mInfo.rcWork.left, mInfo.rcWork.bottom - mInfo.rcWork.top)
            );
        }

        data->Count++;

        return true;
    }

    private static LRESULT MouseProcHook(int code, User32.WINDOW_MESSAGE wParam, LPARAM lParam) =>
        Singleton is DisplayServerWindows dsWin
            ? (LRESULT)dsWin.MouseProc(code, wParam, lParam)
            : User32.CallNextHookEx(default, code, wParam, lParam);

    private static int QueryDpiForMonitor(HMONITOR hmon, Shcore.MONITOR_DPI_TYPE dpiType = Shcore.MONITOR_DPI_TYPE.MDT_DEFAULT)
    {
        int dpiX = 96, dpiY = 96;

        if (hmon != default)
        {
            var hr = Shcore.GetDpiForMonitor(hmon, dpiType /*MDT_Effective_DPI*/, out var x, out var y);
            if (hr >= 0 && x > 0 && y > 0)
            {
                dpiX = (int)x.Value;
                dpiY = (int)y.Value;
            }
        }
        else
        {

            if (overallX <= 0 || overallY <= 0)
            {
                var hdc = User32.GetDC(default);
                if (hdc != default)
                {
                    overallX = GDI32.GetDeviceCaps(hdc, GDI32.DEVICE_CAP.LOGPIXELSX);
                    overallY = GDI32.GetDeviceCaps(hdc, GDI32.DEVICE_CAP.LOGPIXELSY);

                    _ = User32.ReleaseDC(default, hdc);
                }
            }
            if (overallX > 0 && overallY > 0)
            {
                dpiX = overallX;
                dpiY = overallY;
            }
        }

        return (dpiX + dpiY) / 2;
    }

    private static void SendWindowEvent(WindowData wd, WindowEvent windowEvent) => wd.EventCallback?.Invoke(windowEvent);

    private static void TrackMouseLeaveEvent(HWND hWnd)
    {
        var tme = new User32.TRACKMOUSEEVENT
        {
            cbSize      = Marshal.SizeOf<User32.TRACKMOUSEEVENT>(),
            dwFlags     = User32.TRACKMOUSEEVENT_FLAGS.TME_LEAVE,
            hwndTrack   = hWnd,
            dwHoverTime = User32.HOVER_DEFAULT
        };

        User32.TrackMouseEvent(tme);
    }

    private static unsafe LRESULT WndProc(HWND hwnd, User32.WINDOW_MESSAGE msg, WPARAM wParam, LPARAM lParam) =>
        Singleton is DisplayServerWindows dsWin
            ? dsWin.WndProcHandler(hwnd, msg, wParam, lParam, dsWin.GetCenter())
            : User32.DefWindowProcW(hwnd, msg, wParam, lParam);
    #endregion private static

    #region public static methods
    public static void RegisterWindowsDriver() => RegisterCreateFunction("windows", CreateFunc, GetRenderingDriversFunc);
    #endregion public static methods

    #region private methods
    private unsafe int CreateWindow(WindowMode mode, VSyncMode vsyncMode, WindowFlagsBit flags, in Rect2<int> rect)
    {
        GetWindowStyle(
            this.windowIdCounter == MAIN_WINDOW_ID,
            mode is WindowMode.WINDOW_MODE_FULLSCREEN or WindowMode.WINDOW_MODE_EXCLUSIVE_FULLSCREEN,
            mode == WindowMode.WINDOW_MODE_EXCLUSIVE_FULLSCREEN,
            flags.HasFlag(WindowFlagsBit.WINDOW_FLAG_BORDERLESS_BIT),
            !flags.HasFlag(WindowFlagsBit.WINDOW_FLAG_RESIZE_DISABLED_BIT),
            mode == WindowMode.WINDOW_MODE_MAXIMIZED,
            flags.HasFlag(WindowFlagsBit.WINDOW_FLAG_NO_FOCUS_BIT) || flags.HasFlag(WindowFlagsBit.WINDOW_FLAG_POPUP_BIT),
            out var dwStyle,
            out var dwExStyle
        );

        var windowRect = new RECT
        {
            left   = rect.Position.X,
            right  = rect.Position.X + rect.Size.X,
            top    = rect.Position.Y,
            bottom = rect.Position.Y + rect.Size.Y
        };

        var rqScreen = this.GetScreenFromRect(rect);

        if (rqScreen < 0)
        {
            rqScreen = GetPrimaryScreen(); // Requested window rect is outside any screen bounds.
        }

        if (mode is WindowMode.WINDOW_MODE_FULLSCREEN or WindowMode.WINDOW_MODE_EXCLUSIVE_FULLSCREEN)
        {
            var screenRect = new Rect2<int>(this.ScreenGetPosition(rqScreen), this.ScreenGetSize(rqScreen));

            windowRect.left   = screenRect.Position.X;
            windowRect.right  = screenRect.Position.X + screenRect.Size.X;
            windowRect.top    = screenRect.Position.Y;
            windowRect.bottom = screenRect.Position.Y + screenRect.Size.Y;
        }
        else
        {
            var sRect = this.ScreenGetUsableRect(rqScreen);
            var wPos = rect.Position;

            if (sRect != default)
            {
                wPos.X = Math.Clamp(wPos.X, sRect.Position.X, sRect.Position.X + sRect.Size.X - rect.Size.X / 3);
                wPos.Y = Math.Clamp(wPos.Y, sRect.Position.Y, sRect.Position.Y + sRect.Size.Y - rect.Size.Y / 3);
            }

            windowRect.left   = wPos.X;
            windowRect.right  = wPos.X + rect.Size.X;
            windowRect.top    = wPos.Y;
            windowRect.bottom = wPos.Y + rect.Size.Y;
        }

        var offset = GetScreensOrigin();

        windowRect.left   += offset.X;
        windowRect.right  += offset.X;
        windowRect.top    += offset.Y;
        windowRect.bottom += offset.Y;

        User32.AdjustWindowRectEx(ref windowRect, dwStyle, false, dwExStyle);

        var id = this.windowIdCounter;

        var wd = new WindowData();

        this.windows.Add(id, wd);

        wd.HWnd = User32.CreateWindowExW(
            dwExStyle,
            "Engine",
            default,
            dwStyle,
            // (GetSystemMetrics(SM_CXSCREEN) - WindowRect.right) / 2,
            // (GetSystemMetrics(SM_CYSCREEN) - WindowRect.bottom) / 2,
            windowRect.left,
            windowRect.top,
            windowRect.right - windowRect.left,
            windowRect.bottom - windowRect.top,
            default,
            default,
            this.hInstance,
            // tunnel the WindowData we need to handle creation message
            // lifetime is ensured because we are still on the stack when this is
            // processed in the window proc
            id
        );

        if (wd.HWnd == default)
        {
            _ = User32.MessageBoxW(0, "Window Creation Error.", "ERROR", User32.MESSAGE_BOX_OPTIONS.MB_OK | User32.MESSAGE_BOX_OPTIONS.MB_ICONEXCLAMATION);

            this.windows.Remove(id);
            return ERR_FAIL_V_MSG(INVALID_WINDOW_ID, "Failed to create Windows OS window.");
        }

        if (mode is WindowMode.WINDOW_MODE_FULLSCREEN or WindowMode.WINDOW_MODE_EXCLUSIVE_FULLSCREEN)
        {
            wd.Fullscreen = true;

            if (mode == WindowMode.WINDOW_MODE_FULLSCREEN)
            {
                wd.MultiwindowFs = true;
            }
        }

        if (mode is not WindowMode.WINDOW_MODE_FULLSCREEN and not WindowMode.WINDOW_MODE_EXCLUSIVE_FULLSCREEN)
        {
            wd.PreFsValid = true;
        }

        if (this.IsDarkModeSupported && this.darkTitleAvailable)
        {
            unsafe
            {
                var value = this.IsDarkMode;

                DwmApi.DwmSetWindowAttribute(wd.HWnd, DwmApi.DWM_WINDOW_ATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, (uint)Marshal.SizeOf(value));
            }
        }

        #if VULKAN_ENABLED
        if (this.contextVulkan != null)
        {
            if (this.contextVulkan.WindowCreate(id, vsyncMode, wd.HWnd, this.hInstance, windowRect.right - windowRect.left, windowRect.bottom - windowRect.top) != Error.OK)
            {
                this.contextVulkan = null;
                this.windows.Remove(id);

                return ERR_FAIL_V_MSG(INVALID_WINDOW_ID, "Failed to create Vulkan Window.");
            }

            wd.ContextCreated = true;
        }
        #endif

        #if GLES3_ENABLED
        if (this.glManager != null)
        {
            if (this.glManager.WindowCreate(id, wd.HWnd, this.hInstance, windowRect.right - windowRect.left, windowRect.bottom - windowRect.top) != Error.OK)
            {
                this.glManager = null;
                this.windows.Remove(id);

                return ERR_FAIL_V_MSG(INVALID_WINDOW_ID, "Failed to create an OpenGL window.");
            }

            this.WindowSetVsyncMode(vsyncMode, id);
        }
        #endif

        // TODO - platform\windows\display_server_windows.cpp[3751:3778]

        if (mode == WindowMode.WINDOW_MODE_MAXIMIZED)
        {
            wd.Maximized = true;
            wd.Minimized = false;
        }

        if (mode == WindowMode.WINDOW_MODE_MINIMIZED)
        {
            wd.Maximized = false;
            wd.Minimized = true;
        }

        wd.LastPressure       = 0;
        wd.LastPressureUpdate = 0;
        wd.LastTilt           = new();

        // TODO - platform\windows\display_server_windows.cpp[3794:3796]

        wd.ImPosition = new();

        // FIXME this is wrong in cases where the window coordinates were changed due to full screen mode; use WindowRect
        wd.LastPos = rect.Position;
        wd.Width   = rect.Size.X;
        wd.Height  = rect.Size.Y;

        this.windowIdCounter++;

        return id;
    }

    private void CursorSetShape(CursorShape shape)
    {
        lock (padlock)
        {
            if (ERR_FAIL_INDEX((int)shape, (int)CursorShape.CURSOR_MAX))
            {
                return;
            }

            if (this.cursorShape == shape)
            {
                return;
            }

            if (this.mouseMode is not MouseMode.MOUSE_MODE_VISIBLE and not MouseMode.MOUSE_MODE_CONFINED)
            {
                this.cursorShape = shape;
                return;
            }

            if (this.cursorsCache.ContainsKey(shape))
            {
                User32.SetCursor(this.cursors[(int)shape]);
            }
            else
            {
                User32.SetCursor(User32.LoadCursorW(this.hInstance, winCursors[(int)shape]));
            }

            this.cursorShape = shape;
        }
    }

    private Vector2<float> GetCenter() => this.center;

    private unsafe void GetCursorPos(ref POINT v) => throw new NotImplementedException();

    private int GetFocusedWindowOrPopup() => this.popupList.Count > 0 ? this.popupList.Pop() : this.lastFocusedWindow;

    private static int GetPrimaryScreen()
    {
        var data = new EnumScreenData();

        User32.EnumDisplayMonitors(default, default, new(MonitorEnumProcPrim), ref data);

        return data.Screen;
    }

    private int GetScreenFromRect(Rect2<int> rect)
    {
        var nearestArea = 0;
        var posScreen  = -1;

        var screenCount = this.GetScreenCount();

        for (var i = 0; i < screenCount; i++)
        {
            var r = new Rect2<int>(this.ScreenGetPosition(i), this.ScreenGetSize(i));

            var inters = r.Intersection(rect);
            var area   = inters.Size.X * inters.Size.Y;

            if (area > nearestArea)
            {
                posScreen   = i;
                nearestArea = area;
            }
        }

        return posScreen;
    }

    private int GetWindowAtScreenPosition(Vector2<int> position)
    {
        var offset = GetScreensOrigin();
        var p = new POINT
        {
            x = position.X + offset.X,
            y = position.Y + offset.Y
        };

        var hwnd = User32.WindowFromPoint(p);

        foreach (var item in this.windows)
        {
            if (item.Value.HWnd == hwnd)
            {
                return item.Key;
            }
        }

        return INVALID_WINDOW_ID;
    }

    private static void GetWindowStyle(
        bool mainWindow,
        bool fullscreen,
        bool multiwindowFs,
        bool borderless,
        bool resizable,
        bool maximized,
        bool noActivateFocus,
        out User32.WINDOW_STYLES style,
        out User32.WINDOW_STYLES_EX styleEx
    )
    {
        // Windows docs for window styles:
        // https://docs.microsoft.com/en-us/windows/win32/winmsg/window-styles
        // https://docs.microsoft.com/en-us/windows/win32/winmsg/extended-window-styles

        style = 0;
        styleEx = User32.WINDOW_STYLES_EX.WS_EX_WINDOWEDGE;
        if (mainWindow)
        {
            styleEx |= User32.WINDOW_STYLES_EX.WS_EX_APPWINDOW;
            style |= User32.WINDOW_STYLES.WS_VISIBLE;
        }

        if (fullscreen || borderless)
        {
            style |= User32.WINDOW_STYLES.WS_POPUP; // p_borderless was WS_EX_TOOLWINDOW in the past.
            if (fullscreen && multiwindowFs)
            {
                style |= User32.WINDOW_STYLES.WS_BORDER; // Allows child windows to be displayed on top of full screen.
            }
        }
        else
        {
            style = resizable
                ? maximized
                    ? User32.WINDOW_STYLES.WS_OVERLAPPEDWINDOW | User32.WINDOW_STYLES.WS_MAXIMIZE
                    : User32.WINDOW_STYLES.WS_OVERLAPPEDWINDOW
                : User32.WINDOW_STYLES.WS_OVERLAPPED | User32.WINDOW_STYLES.WS_CAPTION | User32.WINDOW_STYLES.WS_SYSMENU;
        }

        if (noActivateFocus)
        {
            styleEx |= User32.WINDOW_STYLES_EX.WS_EX_TOPMOST | User32.WINDOW_STYLES_EX.WS_EX_NOACTIVATE;
        }

        if (!borderless && !noActivateFocus)
        {
            style |= User32.WINDOW_STYLES.WS_VISIBLE;
        }

        style |= User32.WINDOW_STYLES.WS_CLIPCHILDREN | User32.WINDOW_STYLES.WS_CLIPSIBLINGS;
        styleEx |= User32.WINDOW_STYLES_EX.WS_EX_ACCEPTFILES;
    }

    private unsafe LRESULT HandleEarlyWindowMessage(HWND hWnd, User32.WINDOW_MESSAGE uMsg, WPARAM wParam, LPARAM lParam)
    {
        switch (uMsg)
        {
            case User32.WINDOW_MESSAGE.WM_GETMINMAXINFO:
                // We receive this during CreateWindowEx and we haven't initialized the window
                // struct, so let Windows figure out the maximized size.
                // Silently forward to user/default.
                break;
            case User32.WINDOW_MESSAGE.WM_NCCREATE:
                {
                    // We tunnel an unowned pointer to our window context (WindowData) through the
                    // first possible message (WM_NCCREATE) to fix up our window context collection.
                    var pCreate  = (User32.CREATESTRUCTW*)lParam;
                    var windowId = *(int*)pCreate->lpCreateParams;

                    // Fix this up so we can recognize the remaining messages.
                    this.windows[windowId].HWnd = hWnd;
                }
                break;
            default:
                // Additional messages during window creation should happen after we fixed
                // up the data structures on WM_NCCREATE, but this might change in the future,
                // so report an error here and then we can implement them.
                ERR_PRINT_ONCE($"Unexpected window message 0x{(uint)uMsg} received for window we cannot recognize in our collection; sequence error.", ref handleEarlyWindowMessageFirstPrint);
                break;
        }

        return this.userProc != null
            ? User32.CallWindowProcW(this.userProc.Value, hWnd, uMsg, wParam, lParam)
            : User32.DefWindowProcW(hWnd, uMsg, wParam, lParam);
    }

    private unsafe nint MouseProc(int code, User32.WINDOW_MESSAGE wParam, nint lParam)
    {
        var delta = OS.Singleton.TicksUsec - this.timeSincePopup;

        if (delta > 250)
        {
            switch (wParam)
            {
                case User32.WINDOW_MESSAGE.WM_NCLBUTTONDOWN:
                case User32.WINDOW_MESSAGE.WM_NCRBUTTONDOWN:
                case User32.WINDOW_MESSAGE.WM_NCMBUTTONDOWN:
                case User32.WINDOW_MESSAGE.WM_LBUTTONDOWN:
                case User32.WINDOW_MESSAGE.WM_RBUTTONDOWN:
                case User32.WINDOW_MESSAGE.WM_MBUTTONDOWN:
                    {
                        var ms = (User32.MOUSEHOOKSTRUCT*)lParam;

                        var pos = new Vector2<int>(ms->pt.x, ms->pt.y) - GetScreensOrigin();

                        // // Find top popup to close.
                        var i = this.popupList.Count;
                        foreach (var item in this.popupList)
                        {
                            i--;
                            // Popup window area.
                            var winRect = new Rect2<int>(this.WindowGetPosition(item), this.WindowGetSize(item));
                            // Area of the parent window, which responsible for opening sub-menu.
                            var safeRect = this.WindowGetPopupSafeRect(item);
                            if (winRect.HasPoint(pos))
                            {
                                break;
                            }
                            else if (safeRect != default && safeRect.HasPoint(pos))
                            {
                                break;
                            }
                        }

                        if (i > -1)
                        {
                            SendWindowEvent(this.windows[i], WindowEvent.WINDOW_EVENT_CLOSE_REQUEST);

                            return 1;
                        }
                    }
                    break;
            }
        }

        return User32.CallNextHookEx(this.mouseMonitor, code, wParam, lParam);
    }

    private void PopupOpen(int window)
    {
        lock (padlock)
        {
            var wd = this.windows[window];

            if (wd.IsPopup)
            {
                // Find current popup parent, or root popup if new window is not transient.
                var i = this.popupList.Count;

                foreach (var item in this.popupList)
                {
                    i--;

                    if (wd.TransientParent == item && wd.TransientParent != INVALID_WINDOW_ID)
                    {
                        break;
                    }
                }

                if (i > -1)
                {
                    SendWindowEvent(this.windows[i], WindowEvent.WINDOW_EVENT_CLOSE_REQUEST);
                }

                this.timeSincePopup = OS.Singleton.TicksUsec;
                this.popupList.Push(window);
            }
        }
    }

    private void PrintError(string message) => throw new NotImplementedException();

    private void ProcessActivateEvent(int windowId, WPARAM wParam, LPARAM lParam)
    {
        if (LOWORD(wParam) is User32.WA_ACTIVE or User32.WA_ACTIVE)
        {
            // <TODO> this.SendWindowEvent(this.windows[windowId], WindowEvent.WINDOW_EVENT_FOCUS_IN);
            this.windows[windowId].WindowFocused = true;

            this.altMem     = false;
            this.controlMem = false;
            this.shiftMem   = false;

            // Restore mouse mode.
            // <TODO> this.SetMouseModeImpl(this.mouseMode);
        }
        else
        { // WM_INACTIVE.
            // <TODO> Input.Singleton.ReleasePressedEvents();
            // <TODO> this.SendWindowEvent(this.windows[windowId], WindowEvent.WINDOW_EVENT_FOCUS_OUT);
            this.windows[windowId].WindowFocused = false;
            this.altMem = false;
        }

        // <TODO>
        // if ((tablet_get_current_driver() == "wintab") && wintab_available && windows[windowId].wtctx)
        // {
        //     wintab_WTEnable(windows[windowId].wtctx, GET_WM_ACTIVATE_STATE(wParam, lParam));
        // }
        // </TODO>
    }

    #pragma warning disable CA1822 // TODO - REMOVE
    private void ProcessKeyEventsInternal()
    {
        // TODO
    }
    #pragma warning restore CA1822 // TODO - REMOVE

    private unsafe void ScreenToClient(HWND hWnd, ref POINT v) => throw new NotImplementedException();

    private void SetMouseModeImpl(MouseMode mouse_mode) => throw new NotImplementedException();

    private void ScreenSetKeepOn(bool enable)
    {
        if (this.keepScreenOn == enable)
        {
            return;
        }

        if (enable)
        {
            using var simpleReasonString = new LPWSTR("Godot running with display/window/energy_saving/keescreen_on = true");

            var context = new Kernel32.REASON_CONTEXT
            {
                Version = Kernel32.POWER_REQUEST_CONTEXT_FLAGS.POWER_REQUEST_CONTEXT_VERSION,
                Flags   = Kernel32.POWER_REQUEST_CONTEXT_FLAGS.POWER_REQUEST_CONTEXT_SIMPLE_STRING,
                Reason  = new()
                {
                    SimpleReasonString = simpleReasonString,
                }
            };

            this.powerRequest = Kernel32.PowerCreateRequest(context);

            if (this.powerRequest == Kernel32.INVALID_HANDLE_VALUE)
            {
                this.PrintError("Failed to enable screen_keep_on.");
                return;
            }

            if (Kernel32.PowerSetRequest(this.powerRequest, Kernel32.POWER_REQUEST_TYPE.POWER_REQUEST_SYSTEM_REQUIRED))
            {
                this.PrintError("Failed to request system sleep override.");
                return;
            }

            if (Kernel32.PowerSetRequest(this.powerRequest, Kernel32.POWER_REQUEST_TYPE.POWER_REQUEST_DISPLAY_REQUIRED))
            {
                this.PrintError("Failed to request display timeout override.");
                return;
            }
        }
        else
        {
            Kernel32.PowerClearRequest(this.powerRequest, Kernel32.POWER_REQUEST_TYPE.POWER_REQUEST_SYSTEM_REQUIRED);
            Kernel32.PowerClearRequest(this.powerRequest, Kernel32.POWER_REQUEST_TYPE.POWER_REQUEST_DISPLAY_REQUIRED);
            Kernel32.CloseHandle(this.powerRequest);

            this.powerRequest = -1;
        }

        this.keepScreenOn = enable;
    }

    private void SetFocus(HWND hWnd) => throw new NotImplementedException();
    private void SetForegroundWindow(HWND hWnd) => throw new NotImplementedException();


    #pragma warning disable CA1822
    private string? TabletGetCurrentDriver() => null; // TODO
    #pragma warning restore CA1822

    private void TouchEvent(int windowId, bool v, float x, float y, int key) => throw new NotImplementedException();

    private void UpdateRealMousePosition(int window)
    {
        if (ERR_FAIL_COND(!this.windows.ContainsKey(window)))
        {
            return;
        }

        if (User32.GetCursorPos(out var mousePos) && User32.ScreenToClient(this.windows[window].HWnd, mousePos))
        {
            if (mousePos.x > 0 && mousePos.y > 0 && mousePos.x <= this.windows[window].Width && mousePos.y <= this.windows[window].Height)
            {
                this.oldX       = mousePos.x;
                this.oldY       = mousePos.y;
                this.oldInvalid = false;

                // TODO Input.Singleton.MousePosition = new(mousePos.x, mousePos.y);
            }
        }
    }

    private unsafe void UpdateWindowMousePassthrough(int window)
    {
        if (ERR_FAIL_COND(!this.windows.ContainsKey(window)))
        {
            return;
        }

        if (this.windows[window].Mpath.Count == 0)
        {
            _ = User32.SetWindowRgn(this.windows[window].HWnd, default, true);
        }
        else
        {
            var points = new POINT[this.windows[window].Mpath.Count];

            for (var i = 0; i < this.windows[window].Mpath.Count; i++)
            {
                if (this.windows[window].Borderless)
                {
                    points[i].x = (int)this.windows[window].Mpath[i].X;
                    points[i].y = (int)this.windows[window].Mpath[i].Y;
                }
                else
                {
                    points[i].x = (int)this.windows[window].Mpath[i].X + User32.GetSystemMetrics(User32.SYSTEM_METRIC.SM_CXSIZEFRAME);
                    points[i].y = (int)this.windows[window].Mpath[i].Y + User32.GetSystemMetrics(User32.SYSTEM_METRIC.SM_CYSIZEFRAME) + User32.GetSystemMetrics(User32.SYSTEM_METRIC.SM_CYCAPTION);
                }
            }

            var region = User32.CreatePolygonRgn(points, User32.FILL_MODE.ALTERNATE);

            _ = User32.SetWindowRgn(this.windows[window].HWnd, region, true);
            GDI32.DeleteObject((HGDIOBJ)region.Value);
        }
    }

    private unsafe void UpdateWindowStyle(int window, bool repaint = true)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND(!this.windows.ContainsKey(window)))
            {
                return;
            }

            var wd = this.windows[window];

            GetWindowStyle(
                window == MAIN_WINDOW_ID,
                wd.Fullscreen,
                wd.MultiwindowFs,
                wd.Borderless,
                wd.Resizable,
                wd.Maximized,
                wd.NoFocus || wd.IsPopup,
                out var style,
                out var styleEx
            );

            var uStyle   = (LONG)(long)style;
            var uStyleEx = (LONG)(long)styleEx;

            User32.SetWindowLongPtrW(wd.HWnd, User32.WINDOW_LONG_INDEX.GWL_STYLE, uStyle);
            User32.SetWindowLongPtrW(wd.HWnd, User32.WINDOW_LONG_INDEX.GWL_EXSTYLE, uStyleEx);

            // TODO
            // if (icon.is_valid())
            // {
            //     set_icon(icon);
            // }

            User32.SetWindowPos(
                wd.HWnd,
                wd.AlwaysOnTop ? User32.WINDOW_ZORDER.HWND_TOPMOST : User32.WINDOW_ZORDER.HWND_NOTOPMOST,
                0,
                0,
                0,
                0,
                User32.WINDOW_POS_FLAGS.SWP_FRAMECHANGED
                | User32.WINDOW_POS_FLAGS.SWP_NOMOVE
                | User32.WINDOW_POS_FLAGS.SWP_NOSIZE
                | ((wd.NoFocus || wd.IsPopup) ? User32.WINDOW_POS_FLAGS.SWP_NOACTIVATE : 0)
            );

            if (repaint)
            {
                User32.GetWindowRect(wd.HWnd, out var rect);
                User32.MoveWindow(wd.HWnd, rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top, true);
            }
        }
    }

    private int WindowGetActivePopup() => throw new NotImplementedException();



    private Rect2<int> WindowGetPopupSafeRect(int item) => throw new NotImplementedException();

    private Vector2<int> WindowGetSizeWithDecorations(int windowId) => throw new NotImplementedException();

    private void WindowSetVsyncMode(VSyncMode vsyncMode, int window)
    {
        lock (padlock)
        {
            #if VULKAN_ENABLED
            this.contextVulkan?.SetVsyncMode(window, vsyncMode);
            #endif

            #if GLES3_ENABLED
            this.glManager?.SetUseVsync(window, vsyncMode != VSyncMode.VSYNC_DISABLED);
            #endif
        }
    }

    private unsafe bool WintabWtInfo(uint v1, ushort dVC_NPRESSURE, AXIS[] v2) => throw new NotImplementedException();

    private unsafe bool WintabWtPacket(bool wtctx, WPARAM wParam, PACKET* v) => throw new NotImplementedException();

    private unsafe LRESULT WndProcHandler(HWND hWnd, User32.WINDOW_MESSAGE uMsg, WPARAM wParam, LPARAM lParam, Vector2<float> center)
    {
        if (this.dropEvents)
        {
            return this.userProc != null
                ? User32.CallWindowProcW(this.userProc.Value, hWnd, uMsg, wParam, lParam)
                : User32.DefWindowProcW(hWnd, uMsg, wParam, lParam);
        }

        var windowId      = INVALID_WINDOW_ID;
        var windowCreated = false;

        // Check whether window exists
        // FIXME this is O(n), where n is the set of currently open windows and subwindows
        // we should have a secondary map from HWND to WindowID or even WindowData* alias, if we want to eliminate all the map lookups below
        foreach (var item in this.windows)
        {
            if (item.Value.HWnd == hWnd)
            {
                windowId = item.Key;
                windowCreated = true;
                break;
            }
        }

        // WARNING: we get called with events before the window is registered in our collection
        // specifically, even the call to CreateWindowEx already calls here while still on the stack,
        // so there is no way to store the window handle in our collection before we get here
        if (!windowCreated)
        {
            // don't let code below operate on incompletely initialized window objects or missing windowId
            return this.HandleEarlyWindowMessage(hWnd, uMsg, wParam, lParam);
        }

        var windowData = this.windows[windowId]!;

        // Process window messages.
        switch (uMsg)
        {
            case User32.WINDOW_MESSAGE.WM_MOUSEACTIVATE:
                if (windowData.NoFocus)
                {
                    return new((int)User32.MOUSE_ACTIVATE.MA_NOACTIVATEANDEAT); // Do not activate, and discard mouse messages.
                }
                else if (windowData.IsPopup)
                {
                    return new((int)User32.MOUSE_ACTIVATE.MA_NOACTIVATE); // Do not activate, but process mouse messages.
                }
                break;
            case User32.WINDOW_MESSAGE.WM_SETFOCUS:
                windowData.WindowHasFocus = true;
                this.lastFocusedWindow = windowId;

                // Restore mouse mode.
                // <TODO> this.SetMouseModeImpl(this.mouseMode);

                if (!this.appFocused)
                {
                    OS.Singleton.MainLoop?.Notification(NotificationKind.MAIN_LOOP_NOTIFICATION_APPLICATION_FOCUS_IN);
                    this.appFocused = true;
                }
                break;
            case User32.WINDOW_MESSAGE.WM_KILLFOCUS:
                windowData.WindowHasFocus = false;
                this.lastFocusedWindow = windowId;

                // Release capture unconditionally because it can be set due to dragging, in addition to captured mode.
                User32.ReleaseCapture();

                // Release every touch to avoid sticky points.
                foreach (var e in this.touchState)
                {
                    this.TouchEvent(windowId, false, e.Value.X, e.Value.Y, e.Key);
                }

                this.touchState.Clear();

                var selfSteal = false;
                if (User32.IsWindow((nint)wParam.Value))
                {
                    selfSteal = true;
                }

                if (!selfSteal)
                {
                    OS.Singleton.MainLoop?.Notification(NotificationKind.MAIN_LOOP_NOTIFICATION_APPLICATION_FOCUS_OUT);
                    this.appFocused = false;
                }
                break;
            case User32.WINDOW_MESSAGE.WM_ACTIVATE:
                // Watch for window activate message.
                if (!windowData.WindowFocused)
                {
                    this.ProcessActivateEvent(windowId, wParam, lParam);
                }
                else
                {
                    windowData.SavedWParam  = wParam;
                    windowData.SavedLParam  = lParam;
                    windowData.FocusTimerId = User32.SetTimer(windowData.HWnd, 2u, User32.USER_TIMER_MINIMUM, default);
                }

                // Run a timer to prevent event catching warning if the focused window is closing.
                if (wParam.Value.ToUInt32() != User32.WA_INACTIVE)
                {
                    // <TODO> this.TrackMouseLeaveEvent(hwnd);
                }
                return default; // Return to the message loop.

            case User32.WINDOW_MESSAGE.WM_GETMINMAXINFO:
                if (windowData.Resizable && !windowData.Fullscreen)
                {
                    // Size of window decorations.
                    var decor = (this.WindowGetSizeWithDecorations(windowId) - this.WindowGetSize(windowId)).As<RealT>();

                    var minMaxInfo = (User32.MINMAXINFO*)lParam;
                    if (windowData.MinSize != default)
                    {
                        minMaxInfo->ptMinTrackSize.x = (int)(windowData.MinSize.X + decor.X);
                        minMaxInfo->ptMinTrackSize.y = (int)(windowData.MinSize.Y + decor.Y);
                    }
                    if (windowData.MaxSize != default)
                    {
                        minMaxInfo->ptMaxTrackSize.x = (int)(windowData.MaxSize.X + decor.X);
                        minMaxInfo->ptMaxTrackSize.y = (int)(windowData.MaxSize.Y + decor.Y);
                    }
                    return default;
                }
                break;
            case User32.WINDOW_MESSAGE.WM_ERASEBKGND:
                {
                    if (!GetWindowEarlyClearOverride(out var earlyColor))
                    {
                        break;
                    }
                    var mustRecreateBrush = this.windowBkgBrush == default || this.windowBkgBrushColor != earlyColor.ToArgb32();
                    if (mustRecreateBrush)
                    {
                        if (this.windowBkgBrush != default)
                        {
                            GDI32.DeleteObject(new(this.windowBkgBrush));
                        }
                        this.windowBkgBrush = GDI32.CreateSolidBrush(RGB(earlyColor.R8, earlyColor.G8, earlyColor.B8));
                    }
                    var hdc = (HDC)(nint)wParam.Value;

                    if (User32.GetUpdateRect(hWnd, out var rect, true))
                    {
                        User32.FillRect(hdc, rect, this.windowBkgBrush);
                    }

                }
                return 1;
            case User32.WINDOW_MESSAGE.WM_PAINT:
                Main.Main.ForceRedraw();
                break;
            case User32.WINDOW_MESSAGE.WM_SETTINGCHANGE:
                if (lParam != default && Kernel32.CompareStringOrdinal(((LPCWCH)(void*)lParam)!, -1, "ImmersiveColorSet", -1, true) == Kernel32.CSTR_EQUAL)
                {
                    if (this.IsDarkModeSupported && this.darkTitleAvailable)
                    {
                        var value = this.IsDarkMode;

                        DwmApi.DwmSetWindowAttribute(windowData.HWnd, DwmApi.DWM_WINDOW_ATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, (uint)Marshal.SizeOf(value));
                    }
                }
                break;
            case User32.WINDOW_MESSAGE.WM_THEMECHANGED:
                if (this.IsDarkModeSupported && this.darkTitleAvailable)
                {
                    var value = this.IsDarkMode;

                    DwmApi.DwmSetWindowAttribute(windowData.HWnd, DwmApi.DWM_WINDOW_ATTRIBUTE.DWMWA_USE_IMMERSIVE_DARK_MODE, ref value, (uint)Marshal.SizeOf(value));
                }
                break;
            case User32.WINDOW_MESSAGE.WM_SYSCOMMAND: // Intercept system commands.
                switch ((SYS_COMMANDS)wParam.Value.ToUInt32()) // Check system calls.
                {
                    case SYS_COMMANDS.SC_SCREENSAVE: // Screensaver trying to start?
                    case SYS_COMMANDS.SC_MONITORPOWER: // Monitor trying to enter powersave?
                        return 0; // Prevent from happening.
                    case SYS_COMMANDS.SC_KEYMENU:
                        if (lParam >> 16 <= 0)
                        {
                            return 0;
                        }
                        break;
                }
                break;
            case User32.WINDOW_MESSAGE.WM_CLOSE: // Did we receive a close message?
                if (windowData.FocusTimerId != 0U)
                {
                    User32.KillTimer(windowData.HWnd, windowData.FocusTimerId);
                }

                SendWindowEvent(windowData, WindowEvent.WINDOW_EVENT_CLOSE_REQUEST);

                return 0; // Jump back.
            case User32.WINDOW_MESSAGE.WM_MOUSELEAVE:
                if (this.windowMouseoverId == windowId)
                {
                    this.oldInvalid = true;
                    this.windowMouseoverId = INVALID_WINDOW_ID;

                    SendWindowEvent(windowData, WindowEvent.WINDOW_EVENT_MOUSE_EXIT);
                }
                else if (this.windowMouseoverId != INVALID_WINDOW_ID && this.windows.TryGetValue(this.windowMouseoverId, out var value))
                {
                    // This is reached during drag and drop, after dropping in a different window.
                    // Once-off notification, must call again.
                    TrackMouseLeaveEvent(value.HWnd);
                }

                break;
            case User32.WINDOW_MESSAGE.WM_INPUT:
                if (this.mouseMode != MouseMode.MOUSE_MODE_CAPTURED || !this.useRawInput)
                {
                    break;
                }

                var dwSize = new PUINT();

                User32.GetRawInputData((User32.HRAWINPUT)(void*)lParam, User32.RAW_INPUT_DATA_COMMAND_FLAGS.RID_INPUT, default, dwSize, Marshal.SizeOf<User32.RAWINPUTHEADER>());

                if (dwSize == 0)
                {
                    return 0;
                }

                LPBYTE lpb = (byte)(uint)dwSize;

                if (User32.GetRawInputData((User32.HRAWINPUT)(void*)lParam, User32.RAW_INPUT_DATA_COMMAND_FLAGS.RID_INPUT, &lpb, dwSize, Marshal.SizeOf<User32.RAWINPUTHEADER>()) != dwSize)
                {
                    Kernel32.OutputDebugStringW("GetRawInputData does not return correct size !\n");
                }

                var raw = (User32.RAWINPUT*)(void*)lpb;

                if (raw->header.dwType == User32.RAW_INPUT_TYPE.RIM_TYPEMOUSE)
                {
                    var mm = new InputEventMouseMotion
                    {
                        AltPressed   = this.altMem,
                        ButtonMask   = this.lastButtonState,
                        CtrlPressed  = this.controlMem,
                        Pressure     = raw->mouse.ulButtons.HasFlag(User32.MOUSE_BUTTON_STATE.RI_MOUSE_LEFT_BUTTON_DOWN) ? 1.0f : 0.0f,
                        ShiftPressed = this.shiftMem,
                        WindowId     = windowId,
                    };

                    var c = new Vector2<RealT>(windowData.Width / 2, windowData.Height / 2);

                    // Centering just so it works as before.
                    var pos = new POINT { x = (int)c.X, y = (int)c.Y };

                    User32.ClientToScreen(windowData.HWnd, &pos);
                    User32.SetCursorPos(pos.x, pos.y);

                    mm.Position       = c;
                    mm.GlobalPosition = c;
                    mm.Velocity       = new();

                    if (raw->mouse.usFlags == User32.MOUSE_STATE.MOUSE_MOVE_RELATIVE)
                    {
                        mm.Relative = new Vector2<RealT>(raw->mouse.lLastX, raw->mouse.lLastY);

                    }
                    else if (raw->mouse.usFlags == User32.MOUSE_STATE.MOUSE_MOVE_ABSOLUTE)
                    {
                        var nScreenWidth = User32.GetSystemMetrics(User32.SYSTEM_METRIC.SM_CXVIRTUALSCREEN);
                        var nScreenHeight = User32.GetSystemMetrics(User32.SYSTEM_METRIC.SM_CYVIRTUALSCREEN);
                        var nScreenLeft = User32.GetSystemMetrics(User32.SYSTEM_METRIC.SM_XVIRTUALSCREEN);
                        var nScreenTop = User32.GetSystemMetrics(User32.SYSTEM_METRIC.SM_YVIRTUALSCREEN);

                        var absPos = new Vector2<RealT>(
                            (raw->mouse.lLastX - (RealT)65536.0 / nScreenWidth) * nScreenWidth / (RealT)65536.0 + nScreenLeft,
                            (raw->mouse.lLastY - (RealT)65536.0 / nScreenHeight) * nScreenHeight / (RealT)65536.0 + nScreenTop
                        );

                        var coords = new POINT
                        {
                            x = (int)absPos.X,
                            y = (int)absPos.Y
                        }; // Client coords.

                        User32.ScreenToClient(hWnd, coords);

                        mm.Relative = new Vector2<RealT>(coords.x - this.oldX, coords.y - this.oldY);
                        this.oldX = coords.x;
                        this.oldY = coords.y;
                    }

                    if ((windowData.WindowHasFocus || windowData.IsPopup) && mm.Relative != default)
                    {
                        Input.Singleton.ParseInputEvent(mm);
                    }
                }
                break;
            case (User32.WINDOW_MESSAGE)WT_CSRCHANGE:
            case (User32.WINDOW_MESSAGE)WT_PROXIMITY:
                if (this.TabletGetCurrentDriver() == "wintab" && this.wintabAvailable && windowData.Wtctx)
                {
                    var pressure = new AXIS[1];

                    if (this.WintabWtInfo(WTI_DEVICES + windowData.Wtlc.lcDevice, DVC_NPRESSURE, pressure))
                    {
                        windowData.MinPressure = (int)pressure[0].axMin;
                        windowData.MaxPressure = (int)pressure[0].axMax;
                    }

                    var orientation = new AXIS[3];

                    if (this.WintabWtInfo(WTI_DEVICES + windowData.Wtlc.lcDevice, DVC_ORIENTATION, orientation))
                    {
                        windowData.TiltSupported = orientation[0].axResolution != default && orientation[1].axResolution != default;
                    }

                    return 0;
                }
                break;
            case (User32.WINDOW_MESSAGE)WT_PACKET:
                if (this.TabletGetCurrentDriver() == "wintab" && this.wintabAvailable && windowData.Wtctx)
                {
                    var packet = new PACKET();
                    if (this.WintabWtPacket(windowData.Wtctx, wParam, &packet))
                    {
                        var coords = new POINT();
                        this.GetCursorPos(ref coords);
                        this.ScreenToClient(windowData.HWnd, ref coords);

                        windowData.LastPressureUpdate = 0;

                        var pressure = (packet.pkNormalPressure - windowData.MinPressure) / (float)(windowData.MaxPressure - windowData.MinPressure);
                        var azim = packet.pkOrientation.orAzimuth / 10.0f * (Math.PI / 180);
                        var alt = Math.Tan(Math.Abs(packet.pkOrientation.orAltitude / 10.0f) * (Math.PI / 180));
                        var inverted = (packet.pkStatus & TPS_INVERT) != 0;

                        var tilt = windowData.TiltSupported
                            ? new Vector2<RealT>(
                                (RealT)Math.Atan(Math.Sin(azim) / alt),
                                (RealT)Math.Atan(Math.Cos(azim) / alt)
                            )
                            : new Vector2<RealT>();

                        // Nothing changed, ignore event.
                        if (!this.oldInvalid && coords.x == this.oldX && coords.y == this.oldY && windowData.LastPressure == pressure && windowData.LastTilt == tilt && windowData.LastPenInverted == inverted)
                        {
                            break;
                        }

                        windowData.LastPressure = pressure;
                        windowData.LastTilt = tilt;
                        windowData.LastPenInverted = inverted;

                        // Don't calculate relative mouse movement if we don't have focus in CAPTURED mode.
                        if (!windowData.WindowHasFocus && this.mouseMode == MouseMode.MOUSE_MODE_CAPTURED)
                        {
                            break;
                        }

                        var mm = new InputEventMouseMotion
                        {
                            AltPressed = this.altMem,
                            ButtonMask = this.lastButtonState,
                            CtrlPressed = User32.GetKeyState(User32.VIRTUAL_KEYS.VK_CONTROL) < 0,
                            GlobalPosition = new(coords.x, coords.y),
                            PenInverted = windowData.LastPenInverted,
                            Position = new(coords.x, coords.y),
                            Pressure = windowData.LastPressure,
                            ShiftPressed = User32.GetKeyState(User32.VIRTUAL_KEYS.VK_SHIFT) < 0,
                            Tilt = windowData.LastTilt,
                            WindowId = windowId,
                        };

                        if (this.mouseMode == MouseMode.MOUSE_MODE_CAPTURED)
                        {
                            var c = new Vector2<RealT>(windowData.Width / 2, windowData.Height / 2);
                            this.oldX = c.X;
                            this.oldY = c.Y;

                            if (mm.Position == c)
                            {
                                this.center = c;
                                return 0;
                            }

                            var nCenter = mm.Position;
                            this.center = nCenter;
                            var pos = new POINT { x = (int)c.X, y = (int)c.Y };
                            User32.ClientToScreen(windowData.HWnd, &pos);
                            User32.SetCursorPos(pos.x, pos.y);
                        }

                        mm.Velocity = Input.Singleton.LastMouseVelocity;

                        if (this.oldInvalid != default)
                        {
                            this.oldX = mm.Position.X;
                            this.oldY = mm.Position.Y;
                            this.oldInvalid = false;
                        }

                        mm.Relative = mm.Position - new Vector2<RealT>(this.oldX, this.oldY);
                        this.oldX = mm.Position.X;
                        this.oldY = mm.Position.Y;
                        if (windowData.WindowHasFocus || this.WindowGetActivePopup() == windowId)
                        {
                            Input.Singleton.ParseInputEvent(mm);
                        }
                    }
                    return 0;
                }
                break;
            case (User32.WINDOW_MESSAGE)WM_POINTERENTER:
                {
                    if (this.mouseMode == MouseMode.MOUSE_MODE_CAPTURED && this.useRawInput)
                    {
                        break;
                    }

                    if (this.TabletGetCurrentDriver() != "winink" || !wininkAvailable)
                    {
                        break;
                    }

                    var pointerId   = (uint)wParam;
                    var pointerType = POINTER_INPUT_TYPE.PT_POINTER;

                    if (!win8pGetPointerType!(pointerId, ref pointerType))
                    {
                        break;
                    }

                    if (pointerType != POINTER_INPUT_TYPE.PT_PEN)
                    {
                        break;
                    }
                    windowData.BlockMm = true;
                }
                return 0;
            case (User32.WINDOW_MESSAGE)WM_POINTERLEAVE:
                windowData.BlockMm = false;
                return 0;
            case (User32.WINDOW_MESSAGE)WM_POINTERUPDATE:
                {
                    if (this.mouseMode == MouseMode.MOUSE_MODE_CAPTURED && this.useRawInput)
                    {
                        break;
                    }

                    if (this.TabletGetCurrentDriver() != "winink" || !wininkAvailable) {
                        break;
                    }

                    var pointerId   = (uint)wParam;
                    var pointerType = POINTER_INPUT_TYPE.PT_POINTER;
                    if (!win8pGetPointerType!(pointerId, ref pointerType))
                    {
                        break;
                    }

                    if (pointerType != POINTER_INPUT_TYPE.PT_PEN)
                    {
                        break;
                    }

                    var penInfo = default(POINTER_PEN_INFO);
                    if (!win8pGetPointerPenInfo!(pointerId, ref penInfo))
                    {
                        break;
                    }

                    if (Input.Singleton.EmulatingMouseFromTouch)
                    {
                        // Universal translation enabled; ignore OS translation.
                        var extra = User32.GetMessageExtraInfo();
                        if (IsTouchEvent((int)extra))
                        {
                            break;
                        }
                    }

                    if (this.windowMouseoverId != windowId) {
                        // Mouse enter.

                        if (this.mouseMode != MouseMode.MOUSE_MODE_CAPTURED) {
                            if (this.windowMouseoverId != INVALID_WINDOW_ID && this.windows.TryGetValue(this.windowMouseoverId, out var value)) {
                                // Leave previous window.
                                SendWindowEvent(value, WindowEvent.WINDOW_EVENT_MOUSE_EXIT);
                            }
                            SendWindowEvent(windowData, WindowEvent.WINDOW_EVENT_MOUSE_ENTER);
                        }

                        var c = this.cursorShape;
                        this.cursorShape = CursorShape.CURSOR_MAX;
                        this.CursorSetShape(c);
                        this.windowMouseoverId = windowId;

                        // Once-off notification, must call again.
                        TrackMouseLeaveEvent(hWnd);
                    }

                    // Don't calculate relative mouse movement if we don't have focus in CAPTURED mode.
                    if (!windowData.WindowHasFocus && this.mouseMode == MouseMode.MOUSE_MODE_CAPTURED)
                    {
                        break;
                    }

                    var mm = new InputEventMouseMotion
                    {
                        WindowId = windowId,
                        Pressure = (penInfo.penMask & PEN_MASK_PRESSURE) != 0
                        ? (float)penInfo.pressure / 1024
                        : (((uint)wParam & POINTER_MESSAGE_FLAG_FIRSTBUTTON) != 0) ? 1.0f : 0.0f
                    };
                    if ((penInfo.penMask & PEN_MASK_TILT_X) != 0 && (penInfo.penMask & PEN_MASK_TILT_Y) != 0)
                    {
                        mm.Tilt = new((RealT)penInfo.tiltX / 90, (RealT)penInfo.tiltY / 90);
                    }
                    mm.PenInverted = (penInfo.penFlags & (PEN_FLAG_INVERTED | PEN_FLAG_ERASER)) == 1u;

                    mm.CtrlPressed  = User32.GetKeyState(User32.VIRTUAL_KEYS.VK_CONTROL) < 0;
                    mm.ShiftPressed = User32.GetKeyState(User32.VIRTUAL_KEYS.VK_SHIFT) < 0;
                    mm.AltPressed   = this.altMem;

                    mm.ButtonMask = this.lastButtonState;

                    var coords = new POINT
                    {
                        x = GET_X_LPARAM(lParam),
                        y = GET_Y_LPARAM(lParam)
                    }; // Client coords.

                    User32.ScreenToClient(windowData.HWnd, &coords);

                    mm.Position       = new(coords.x, coords.y);
                    mm.GlobalPosition = new(coords.x, coords.y);

                    if (this.mouseMode == MouseMode.MOUSE_MODE_CAPTURED)
                    {
                        var c = new Vector2<RealT>(windowData.Width / 2, windowData.Height / 2);
                        this.oldX = c.X;
                        this.oldY = c.Y;

                        if (mm.Position == c)
                        {
                            center = c;
                            return 0;
                        }

                        var nCenter = mm.Position;
                        center = nCenter;
                        var pos = new POINT { x = (int)c.X, y = (int)c.Y };

                        User32.ClientToScreen(hWnd, &pos);
                        User32.SetCursorPos(pos.x, pos.y);
                    }

                    mm.Velocity = Input.Singleton.LastMouseVelocity;

                    if (this.oldInvalid)
                    {
                        this.oldX = mm.Position.X;
                        this.oldY = mm.Position.Y;
                        this.oldInvalid = false;
                    }

                    mm.Relative = mm.Position - new Vector2<RealT>(this.oldX, this.oldY);
                    this.oldX = mm.Position.X;
                    this.oldY = mm.Position.Y;
                    if (windowData.WindowHasFocus || this.WindowGetActivePopup() == windowId)
                    {
                        Input.Singleton.ParseInputEvent(mm);
                    }
                }
                return 0; // Pointer event handled return 0 to avoid duplicate WM_MOUSEMOVE event.
            case User32.WINDOW_MESSAGE.WM_MOUSEMOVE:
                {
                    if (windowData.BlockMm)
                    {
                        break;
                    }

                    if (this.mouseMode == MouseMode.MOUSE_MODE_CAPTURED && this.useRawInput)
                    {
                        break;
                    }

                    if (Input.Singleton.EmulatingMouseFromTouch)
                    {
                        // Universal translation enabled; ignore OS translation.
                        var extra = User32.GetMessageExtraInfo();
                        if (IsTouchEvent((int)extra))
                        {
                            break;
                        }
                    }

                    var overId = this.GetWindowAtScreenPosition(MouseGetPosition());

                    if (!new Rect2<RealT>(this.WindowGetPosition(overId).As<RealT>(), new Vector2<RealT>(windowData.Width, windowData.Height)).HasPoint(MouseGetPosition().As<RealT>()))
                    {
                        // Don't consider the windowborder as part of the window.
                        overId = INVALID_WINDOW_ID;
                    }

                    if (this.windowMouseoverId != overId)
                    {
                        // Mouse enter.

                        if (this.mouseMode != MouseMode.MOUSE_MODE_CAPTURED)
                        {
                            if (this.windowMouseoverId != INVALID_WINDOW_ID && windowData != null)
                            {
                                // Leave previous window.
                                SendWindowEvent(windowData, WindowEvent.WINDOW_EVENT_MOUSE_EXIT);
                            }

                            if (overId != INVALID_WINDOW_ID && windowData != null)
                            {
                                SendWindowEvent(windowData, WindowEvent.WINDOW_EVENT_MOUSE_ENTER);
                            }
                        }

                        var c = this.cursorShape;
                        this.cursorShape = CursorShape.CURSOR_MAX;
                        this.CursorSetShape(c);
                        this.windowMouseoverId = overId;

                        // Once-off notification, must call again.
                        TrackMouseLeaveEvent(hWnd);
                    }

                    // Don't calculate relative mouse movement if we don't have focus in CAPTURED mode.
                    if (!(windowData?.WindowHasFocus ?? false) && this.mouseMode == MouseMode.MOUSE_MODE_CAPTURED)
                    {
                        break;
                    }

                    var receivingWindowId = this.GetFocusedWindowOrPopup();
                    if (receivingWindowId == INVALID_WINDOW_ID)
                    {
                        receivingWindowId = windowId;
                    }
                    var mm = new InputEventMouseMotion
                    {
                        WindowId     = receivingWindowId,
                        CtrlPressed  = ((User32.MOUSE_KEY_STATES)(uint)wParam).HasFlag(User32.MOUSE_KEY_STATES.MK_CONTROL),
                        ShiftPressed = ((User32.MOUSE_KEY_STATES)(uint)wParam).HasFlag(User32.MOUSE_KEY_STATES.MK_SHIFT),
                        AltPressed   = this.altMem
                    };

                    if (this.TabletGetCurrentDriver() == "wintab" && this.wintabAvailable && windowData!.Wtctx)
                    {
                        // Note: WinTab sends both WT_PACKET and WM_xBUTTONDOWN/UP/MOUSEMOVE events, use mouse 1/0 pressure only when last_pressure was not updated recently.
                        if (windowData.LastPressureUpdate < 10)
                        {
                            windowData.LastPressureUpdate++;
                        }
                        else
                        {
                            windowData.LastTilt        = new();
                            windowData.LastPressure    = ((User32.MOUSE_KEY_STATES)(uint)wParam).HasFlag(User32.MOUSE_KEY_STATES.MK_LBUTTON) ? 1.0f : 0.0f;
                            windowData.LastPenInverted = false;
                        }
                    }
                    else
                    {
                        windowData!.LastTilt       = new();
                        windowData.LastPressure    = ((User32.MOUSE_KEY_STATES)(uint)wParam).HasFlag(User32.MOUSE_KEY_STATES.MK_LBUTTON) ? 1.0f : 0.0f;
                        windowData.LastPenInverted = false;
                    }

                    mm.Pressure    = windowData.LastPressure;
                    mm.Tilt        = windowData.LastTilt;
                    mm.PenInverted = windowData.LastPenInverted;
                    mm.ButtonMask  = this.lastButtonState;

                    mm.Position       = new Vector2<RealT>(GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam));
                    mm.GlobalPosition = new Vector2<RealT>(GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam));

                    if (this.mouseMode == MouseMode.MOUSE_MODE_CAPTURED)
                    {
                        var c = new Vector2<RealT>(windowData.Width / 2, windowData.Height / 2);
                        this.oldX = c.X;
                        this.oldY = c.Y;

                        if (mm.Position == c)
                        {
                            center = c;
                            return 0;
                        }

                        var ncenter = mm.Position;
                        center = ncenter;
                        var pos = new POINT{ x = (int)c.X, y = (int)c.Y };
                        User32.ClientToScreen(windowData.HWnd, ref pos);
                        User32.SetCursorPos(pos.x, pos.y);
                    }

                    mm.Velocity = Input.Singleton.LastMouseVelocity;

                    if (this.oldInvalid)
                    {
                        this.oldX = mm.Position.X;
                        this.oldY = mm.Position.Y;
                        this.oldInvalid = false;
                    }

                    mm.Relative = mm.Position - new Vector2<RealT>(this.oldX, this.oldY);

                    this.oldX = mm.Position.X;
                    this.oldY = mm.Position.Y;

                    if (receivingWindowId != windowId)
                    {
                        // Adjust event position relative to window distance when event is sent to a different window.
                        mm.Position       -= (this.WindowGetPosition(receivingWindowId) + this.WindowGetPosition(windowId)).As<RealT>();
                        mm.GlobalPosition = mm.Position;
                    }
                    Input.Singleton.ParseInputEvent(mm);
                }
                break;
            case User32.WINDOW_MESSAGE.WM_LBUTTONDOWN:
            case User32.WINDOW_MESSAGE.WM_LBUTTONUP:
                if (Input.Singleton.EmulatingMouseFromTouch)
                {
                    // Universal translation enabled; ignore OS translations for left button.
                    var extra = User32.GetMessageExtraInfo();
                    if (IsTouchEvent((int)extra))
                    {
                        break;
                    }
                }
                goto case User32.WINDOW_MESSAGE.WM_MBUTTONDOWN;
            case User32.WINDOW_MESSAGE.WM_MBUTTONDOWN:
            case User32.WINDOW_MESSAGE.WM_MBUTTONUP:
            case User32.WINDOW_MESSAGE.WM_RBUTTONDOWN:
            case User32.WINDOW_MESSAGE.WM_RBUTTONUP:
            case User32.WINDOW_MESSAGE.WM_MOUSEWHEEL:
            case User32.WINDOW_MESSAGE.WM_MOUSEHWHEEL:
            case User32.WINDOW_MESSAGE.WM_LBUTTONDBLCLK:
            case User32.WINDOW_MESSAGE.WM_MBUTTONDBLCLK:
            case User32.WINDOW_MESSAGE.WM_RBUTTONDBLCLK:
            case User32.WINDOW_MESSAGE.WM_XBUTTONDBLCLK:
            case User32.WINDOW_MESSAGE.WM_XBUTTONDOWN:
            case User32.WINDOW_MESSAGE.WM_XBUTTONUP:
                {
                    var mb = new InputEventMouseButton
                    {
                        WindowId = windowId
                    };

                    switch (uMsg)
                    {
                        case User32.WINDOW_MESSAGE.WM_LBUTTONDOWN:
                            mb.Pressed = true;
                            mb.ButtonIndex = MouseButton.LEFT;
                            break;
                        case User32.WINDOW_MESSAGE.WM_LBUTTONUP:
                            mb.Pressed = false;
                            mb.ButtonIndex = MouseButton.LEFT;
                            break;
                        case User32.WINDOW_MESSAGE.WM_MBUTTONDOWN:
                            mb.Pressed = true;
                            mb.ButtonIndex = MouseButton.MIDDLE;
                            break;
                        case User32.WINDOW_MESSAGE.WM_MBUTTONUP:
                            mb.Pressed = false;
                            mb.ButtonIndex = MouseButton.MIDDLE;
                            break;
                        case User32.WINDOW_MESSAGE.WM_RBUTTONDOWN:
                            mb.Pressed = true;
                            mb.ButtonIndex = MouseButton.RIGHT;
                            break;
                        case User32.WINDOW_MESSAGE.WM_RBUTTONUP:
                            mb.Pressed = false;
                            mb.ButtonIndex = MouseButton.RIGHT;
                            break;
                        case User32.WINDOW_MESSAGE.WM_LBUTTONDBLCLK:
                            mb.Pressed = true;
                            mb.ButtonIndex = MouseButton.LEFT;
                            mb.DoubleClick = true;
                            break;
                        case User32.WINDOW_MESSAGE.WM_RBUTTONDBLCLK:
                            mb.Pressed = true;
                            mb.ButtonIndex = MouseButton.RIGHT;
                            mb.DoubleClick = true;
                            break;
                        case User32.WINDOW_MESSAGE.WM_MBUTTONDBLCLK:
                            mb.Pressed = true;
                            mb.ButtonIndex = MouseButton.MIDDLE;
                            mb.DoubleClick = true;
                            break;
                        case User32.WINDOW_MESSAGE.WM_MOUSEWHEEL:
                            {
                                mb.Pressed = true;
                                var motion = (short)HIWORD(wParam);
                                if (motion == default)
                                {
                                    return 0;
                                }

                                mb.ButtonIndex = motion > 0 ? MouseButton.WHEEL_UP : MouseButton.WHEEL_DOWN;
                                mb.Factor = Math.Abs(motion / (double)User32.WHEEL_DELTA);
                            }
                            break;
                        case User32.WINDOW_MESSAGE.WM_MOUSEHWHEEL:
                            {
                                mb.Pressed = true;
                                var motion = (short)HIWORD(wParam);

                                if (motion == default)
                                {
                                    return 0;
                                }

                                mb.ButtonIndex = motion < 0 ? MouseButton.WHEEL_LEFT : MouseButton.WHEEL_RIGHT;
                                mb.Factor = Math.Abs(motion / (double)User32.WHEEL_DELTA);
                            }
                            break;
                        case User32.WINDOW_MESSAGE.WM_XBUTTONDOWN:
                            mb.Pressed = true;
                            mb.ButtonIndex = HIWORD(wParam) == (int)User32.MOUSE_BUTTON.XBUTTON1
                                ? MouseButton.MB_XBUTTON1
                                : MouseButton.MB_XBUTTON2;
                            break;
                        case User32.WINDOW_MESSAGE.WM_XBUTTONUP:
                            mb.Pressed = false;
                            mb.ButtonIndex = HIWORD(wParam) == (int)User32.MOUSE_BUTTON.XBUTTON1
                                ? MouseButton.MB_XBUTTON1
                                : MouseButton.MB_XBUTTON2;
                            break;
                        case User32.WINDOW_MESSAGE.WM_XBUTTONDBLCLK:
                            mb.Pressed = true;
                            mb.ButtonIndex = HIWORD(wParam) == (int)User32.MOUSE_BUTTON.XBUTTON1
                                ? MouseButton.MB_XBUTTON1
                                : MouseButton.MB_XBUTTON2;
                            mb.DoubleClick = true;
                            break;
                        default:
                            return 0;
                    }

                    mb.CtrlPressed  = ((User32.MOUSE_KEY_STATES)wParam.Value).HasFlag(User32.MOUSE_KEY_STATES.MK_CONTROL);
                    mb.ShiftPressed = ((User32.MOUSE_KEY_STATES)wParam.Value).HasFlag(User32.MOUSE_KEY_STATES.MK_SHIFT);
                    mb.AltPressed   = this.altMem;

                    // mb->is_alt_pressed()=(wParam&MK_MENU)!=0;
                    if (mb.Pressed)
                    {
                        this.lastButtonState |= (MouseButtonMask)mb.ButtonIndex;
                    }
                    else
                    {
                        this.lastButtonState &= ~(MouseButtonMask)mb.ButtonIndex;
                    }

                    mb.ButtonMask = this.lastButtonState;

                    mb.Position = new(GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam));

                    if (this.mouseMode == MouseMode.MOUSE_MODE_CAPTURED && !this.useRawInput)
                    {
                        mb.Position = new(this.oldX, this.oldY);
                    }

                    if (uMsg is not User32.WINDOW_MESSAGE.WM_MOUSEWHEEL and not User32.WINDOW_MESSAGE.WM_MOUSEHWHEEL)
                    {
                        if (mb.Pressed)
                        {
                            if (++this.pressrc > 0 && this.mouseMode != MouseMode.MOUSE_MODE_CAPTURED)
                            {
                                User32.SetCapture(hWnd);
                            }
                        }
                        else
                        {
                            if (--this.pressrc <= 0)
                            {
                                if (this.mouseMode != MouseMode.MOUSE_MODE_CAPTURED)
                                {
                                    User32.ReleaseCapture();
                                }
                                this.pressrc = 0;
                            }
                        }
                    }
                    else
                    {
                        // For reasons unknown to humanity, wheel comes in screen coordinates.
                        var coords = new POINT
                        {
                            x = (int)mb.Position.X,
                            y = (int)mb.Position.Y,
                        };

                        User32.ScreenToClient(hWnd, coords);

                        mb.Position = new(coords.x, coords.y);
                    }

                    mb.GlobalPosition = mb.Position;

                    Input.Singleton.ParseInputEvent(mb);
                    if (mb.Pressed && mb.ButtonIndex >= MouseButton.WHEEL_UP && mb.ButtonIndex <= MouseButton.WHEEL_RIGHT)
                    {
                        // Send release for mouse wheel.
                        var mbd = (InputEventMouseButton)mb.Duplicate();
                        mbd.WindowId = windowId;
                        this.lastButtonState &= ~(MouseButtonMask)mbd.ButtonIndex;
                        mbd.ButtonMask = this.lastButtonState;
                        mbd.Pressed = false;
                        Input.Singleton.ParseInputEvent(mbd);
                    }
                }
                break;
            case User32.WINDOW_MESSAGE.WM_WINDOWPOSCHANGED:
                return 0;
            //{
            //             Rect2i window_client_rect;
            //             Rect2i window_rect;
            //             {
            //                 RECT rect;
            //                 GetClientRect(hWnd, &rect);
            //                 ClientToScreen(hWnd, (POINT *)&rect.left);
            //                 ClientToScreen(hWnd, (POINT *)&rect.right);
            //                 window_client_rect = Rect2i(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
            //                 window_client_rect.position -= _get_screens_origin();

            //                 RECT wrect;
            //                 GetWindowRect(hWnd, &wrect);
            //                 window_rect = Rect2i(wrect.left, wrect.top, wrect.right - wrect.left, wrect.bottom - wrect.top);
            //                 window_rect.position -= _get_screens_origin();
            //             }

            //             WINDOWPOS *window_pos_params = (WINDOWPOS *)lParam;
            //             WindowData &window = this.windows[windowId];

            //             bool rect_changed = false;
            //             if (!(window_pos_params->flags & SWP_NOSIZE) || window_pos_params->flags & SWP_FRAMECHANGED) {
            //                 int screen_id = window_get_current_screen(windowId);
            //                 Size2i screen_size = screen_get_size(screen_id);
            //                 Point2i screen_position = screen_get_position(screen_id);

            //                 window.maximized = false;
            //                 window.minimized = false;
            //                 window.fullscreen = false;

            //                 if (IsIconic(hWnd)) {
            //                     window.minimized = true;
            //                 } else if (IsZoomed(hWnd)) {
            //                     window.maximized = true;
            //                 } else if (window_rect.position == screen_position && window_rect.size == screen_size) {
            //                     window.fullscreen = true;
            //                 }

            //                 if (!window.minimized) {
            //                     window.width = window_client_rect.size.width;
            //                     window.height = window_client_rect.size.height;

            //                     rect_changed = true;
            //                 }
            // #if defined(VULKAN_ENABLED)
            //                 if (context_vulkan && window.context_created) {
            //                     // Note: Trigger resize event to update swapchains when window is minimized/restored, even if size is not changed.
            //                     context_vulkan->window_resize(windowId, window.width, window.height);
            //                 }
            // #endif
            //             }

            //             if (!window.minimized && (!(window_pos_params->flags & SWP_NOMOVE) || window_pos_params->flags & SWP_FRAMECHANGED)) {
            //                 window.last_pos = window_client_rect.position;
            //                 rect_changed = true;
            //             }

            //             if (rect_changed) {
            //                 if (!window.rect_changed_callback.is_null()) {
            //                     Variant size = Rect2i(window.last_pos.x, window.last_pos.y, window.width, window.height);
            //                     const Variant *args[] = { &size };
            //                     Variant ret;
            //                     Callable::CallError ce;
            //                     window.rect_changed_callback.callp(args, 1, ret, ce);
            //                 }
            //             }

            //             // Return here to prevent WM_MOVE and WM_SIZE from being sent
            //             // See: https://docs.microsoft.com/en-us/windows/win32/winmsg/wm-windowposchanged#remarks
            //             return 0;

            //         } break;

            //         case User32.WindowMessage.WM_ENTERSIZEMOVE: {
            //             Input.Singleton.ReleasePressedEvents();
            //             this.windows[windowId].move_timer_id = SetTimer(this.windows[windowId].hWnd, 1, USER_TIMER_MINIMUM, (TIMERPROC) nullptr);
            //         } break;
            //         case User32.WindowMessage.WM_EXITSIZEMOVE: {
            //             KillTimer(this.windows[windowId].hWnd, this.windows[windowId].move_timer_id);
            //         } break;
            case User32.WINDOW_MESSAGE.WM_TIMER:
                return 0;
            // {
            //             if (wParam == this.windows[windowId].move_timer_id) {
            //                 _process_key_events();
            //                 if (!Main::is_iterating()) {
            //                     Main::iteration();
            //                 }
            //             } else if (wParam == this.windows[windowId].focus_timer_id) {
            //                 _process_activate_event(windowId, this.windows[windowId].saved_wparam, this.windows[windowId].saved_lparam);
            //                 KillTimer(this.windows[windowId].hWnd, wParam);
            //                 this.windows[windowId].focus_timer_id = 0U;
            //             }
            //         } break;
            case User32.WINDOW_MESSAGE.WM_SYSKEYDOWN:
            case User32.WINDOW_MESSAGE.WM_SYSKEYUP:
            case User32.WINDOW_MESSAGE.WM_KEYUP:
            case User32.WINDOW_MESSAGE.WM_KEYDOWN:
                return 0;
            //{
            //             if (wParam == VK_SHIFT) {
            //                 shift_mem = (uMsg == WM_KEYDOWN || uMsg == WM_SYSKEYDOWN);
            //             }
            //             if (wParam == VK_CONTROL) {
            //                 control_mem = (uMsg == WM_KEYDOWN || uMsg == WM_SYSKEYDOWN);
            //             }
            //             if (wParam == VK_MENU) {
            //                 alt_mem = (uMsg == WM_KEYDOWN || uMsg == WM_SYSKEYDOWN);
            //                 if (lParam & (1 << 24)) {
            //                     gr_mem = alt_mem;
            //                 }
            //             }

            //             if (this.mouseMode == MouseMode.MOUSE_MODE_CAPTURED) {
            //                 // When SetCapture is used, ALT+F4 hotkey is ignored by Windows, so handle it ourselves
            //                 if (wParam == VK_F4 && alt_mem && (uMsg == WM_KEYDOWN || uMsg == WM_SYSKEYDOWN)) {
            //                     _send_window_event(this.windows[windowId], WINDOW_EVENT_CLOSE_REQUEST);
            //                 }
            //             }
            //             [[fallthrough]];
            //         }
            case User32.WINDOW_MESSAGE.WM_CHAR:
                return 0;
            //{
            //             ERR_BREAK(key_event_pos >= KEY_EVENT_BUFFER_SIZE);

            //             // Make sure we don't include modifiers for the modifier key itself.
            //             KeyEvent ke;
            //             ke.shift = (wParam != VK_SHIFT) ? shift_mem : false;
            //             ke.alt = (!(wParam == VK_MENU && (uMsg == WM_KEYDOWN || uMsg == WM_SYSKEYDOWN))) ? alt_mem : false;
            //             ke.control = (wParam != VK_CONTROL) ? control_mem : false;
            //             ke.meta = meta_mem;
            //             ke.uMsg = uMsg;
            //             ke.windowId = windowId;

            //             if (ke.uMsg == WM_SYSKEYDOWN) {
            //                 ke.uMsg = WM_KEYDOWN;
            //             }
            //             if (ke.uMsg == WM_SYSKEYUP) {
            //                 ke.uMsg = WM_KEYUP;
            //             }

            //             ke.wParam = wParam;
            //             ke.lParam = lParam;
            //             key_event_buffer[key_event_pos++] = ke;

            //         } break;
            case User32.WINDOW_MESSAGE.WM_INPUTLANGCHANGEREQUEST:
                return 0;
            // {
            //             // FIXME: Do something?
            //         } break;
            // case User32.WindowMessage.WM_TOUCH:
            // {
            //             BOOL bHandled = FALSE;
            //             UINT cInputs = LOWORD(wParam);
            //             PTOUCHINPUT pInputs = memnew_arr(TOUCHINPUT, cInputs);
            //             if (pInputs) {
            //                 if (GetTouchInputInfo((HTOUCHINPUT)lParam, cInputs, pInputs, sizeof(TOUCHINPUT))) {
            //                     for (UINT i = 0; i < cInputs; i++) {
            //                         TOUCHINPUT ti = pInputs[i];
            //                         POINT touch_pos = {
            //                             TOUCH_COORD_TO_PIXEL(ti.x),
            //                             TOUCH_COORD_TO_PIXEL(ti.y),
            //                         };
            //                         ScreenToClient(hWnd, &touch_pos);
            //                         // Do something with each touch input entry.
            //                         if (ti.dwFlags & TOUCHEVENTF_MOVE) {
            //                             _drag_event(windowId, touch_pos.x, touch_pos.y, ti.dwID);
            //                         } else if (ti.dwFlags & (TOUCHEVENTF_UP | TOUCHEVENTF_DOWN)) {
            //                             _touch_event(windowId, ti.dwFlags & TOUCHEVENTF_DOWN, touch_pos.x, touch_pos.y, ti.dwID);
            //                         }
            //                     }
            //                     bHandled = TRUE;
            //                 } else {
            //                     // TODO: Handle the error here.
            //                 }
            //                 memdelete_arr(pInputs);
            //             } else {
            //                 // TODO: Handle the error here, probably out of memory.
            //             }
            //             if (bHandled) {
            //                 CloseTouchInputHandle((HTOUCHINPUT)lParam);
            //                 return 0;
            //             }

            //         } break;
            case User32.WINDOW_MESSAGE.WM_DEVICECHANGE:
                return 0;
                // {
            //             joypad->probe_joypads();
            //         } break;
            //         case User32.WindowMessage.WM_DESTROY: {
            //             Input.Singleton.FlushBufferedEvents();
            //         } break;
            //         case User32.WindowMessage.WM_SETCURSOR: {
            //             if (LOWORD(lParam) == HTCLIENT) {
            //                 if (this.windows[windowId].window_has_focus && (mouse_mode == MOUSE_MODE_HIDDEN || this.mouseMode == MouseMode.MOUSE_MODE_CAPTURED || mouse_mode == MOUSE_MODE_CONFINED_HIDDEN)) {
            //                     // Hide the cursor.
            //                     if (hCursor == nullptr) {
            //                         hCursor = SetCursor(nullptr);
            //                     } else {
            //                         SetCursor(nullptr);
            //                     }
            //                 } else {
            //                     if (hCursor != nullptr) {
            //                         CursorShape c = cursor_shape;
            //                         cursor_shape = CURSOR_MAX;
            //                         cursor_set_shape(c);
            //                         hCursor = nullptr;
            //                     }
            //                 }
            //             }
            //         } break;
            case User32.WINDOW_MESSAGE.WM_DROPFILES:
                return 0;
            // {
            //             HDROP hDropInfo = (HDROP)wParam;
            //             const int buffsize = 4096;
            //             WCHAR buf[buffsize];

            //             int fcount = DragQueryFileW(hDropInfo, 0xFFFFFFFF, nullptr, 0);

            //             Vector<String> files;

            //             for (int i = 0; i < fcount; i++) {
            //                 DragQueryFileW(hDropInfo, i, buf, buffsize);
            //                 String file = String::utf16((const char16_t *)buf);
            //                 files.push_back(file);
            //             }

            //             if (files.size() && !this.windows[windowId].drop_files_callback.is_null()) {
            //                 Variant v = files;
            //                 Variant *vp = &v;
            //                 Variant ret;
            //                 Callable::CallError ce;
            //                 this.windows[windowId].drop_files_callback.callp((const Variant **)&vp, 1, ret, ce);
            //             }
            //         } break;
            default:
                if (this.userProc.HasValue)
                {
                    return User32.CallWindowProcW(this.userProc.Value, hWnd, uMsg, wParam, lParam);
                }
                break;
        }

        return User32.DefWindowProcW(hWnd, uMsg, wParam, lParam);
    }

    #endregion private methods

    #region public methods
    public bool WindowCanDraw(int window)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND_V(!this.windows.ContainsKey(window)))
            {
                return false;
            }

            var wd = this.windows[window];
            return !wd.Minimized;
        }
    }
    #endregion public methods

    #region public override methods
    public override int CreateSubWindow(WindowMode mode, VSyncMode vsyncMode, int flags, in Rect2<int> windowRect) => throw new NotImplementedException();

    public override bool HasFeature(Feature feature) => feature switch
    {
        Feature.FEATURE_SUBWINDOWS
        or Feature.FEATURE_TOUCHSCREEN
        or Feature.FEATURE_MOUSE
        or Feature.FEATURE_MOUSE_WARP
        or Feature.FEATURE_CLIPBOARD
        or Feature.FEATURE_CURSOR_SHAPE
        or Feature.FEATURE_CUSTOM_CURSOR_SHAPE
        or Feature.FEATURE_IME
        or Feature.FEATURE_WINDOW_TRANSPARENCY
        or Feature.FEATURE_HIDPI
        or Feature.FEATURE_ICON
        or Feature.FEATURE_NATIVE_ICON
        or Feature.FEATURE_SWAP_BUFFERS
        or Feature.FEATURE_KEEP_SCREEN_ON
        or Feature.FEATURE_TEXT_TO_SPEECH => true,
        _ => false,
    };

    public override int GetScreenCount()
    {
        lock (padlock)
        {
            var data = 0;

            User32.EnumDisplayMonitors(default, default, new(MonitorEnumProcCount), ref data);

            return data;
        }
    }

    #pragma warning disable IDE0022
    public override void GlWindowMakeCurrent(int window)
    {
        #if GLES3_ENABLED
        this.glManager?.WindowMakeCurrent(window);
        #endif
    }
    #pragma warning restore IDE0022

    public override void MakeRenderingThread()
    {
        // Does nothing
    }

    public override void ProcessEvents()
    {
        lock (padlock)
        {
            var msg = default(User32.MSG);

            if (!this.dropEvents)
            {
                this.joypad.ProcessJoypads();
            }

            while (User32.PeekMessageW(ref msg, default, 0, 0, User32.PEEK_MESSAGE.PM_REMOVE))
            {
                User32.TranslateMessage(msg);
                User32.DispatchMessageW(msg);
            }

            if (!this.dropEvents)
            {
                this.ProcessKeyEventsInternal();
                // TODO Input.Singleton.FlushBufferedEvents();
            }
        }
    }

    public override void ReleaseRenderingThread() => throw new NotImplementedException();

    public override Vector2<int> ScreenGetPosition(int window)
    {
        lock (padlock)
        {
            switch (window)
            {
                case SCREEN_PRIMARY:
                    {
                        window = GetPrimaryScreen();
                    }
                    break;
                case SCREEN_OF_MAIN_WINDOW:
                    {
                        window = this.WindowGetCurrentScreen(MAIN_WINDOW_ID);
                    }
                    break;
                default:
                    break;
            }

            var data = new EnumPosData(0, window, new());

            User32.EnumDisplayMonitors(default, default, new(MonitorEnumProcPos), ref data);

            return data.Position - GetScreensOrigin();
        }
    }

    public override Vector2<int> ScreenGetSize(int window)
    {
        lock (padlock)
        {
            switch (window)
            {
                case SCREEN_PRIMARY:
                    {
                        window = GetPrimaryScreen();
                    }
                    break;
                case SCREEN_OF_MAIN_WINDOW:
                    {
                        window = this.WindowGetCurrentScreen(MAIN_WINDOW_ID);
                    }
                    break;
                default:
                    break;
            }

            var data  = new EnumSizeData(0, window, new());

            User32.EnumDisplayMonitors(default, default, new(MonitorEnumProcSize), ref data);

            return data.Rect.Size;
        }
    }

    public override Rect2<int> ScreenGetUsableRect(int screen)
    {
        lock (padlock)
        {
            switch (screen)
            {
                case SCREEN_PRIMARY:
                    {
                        screen = GetPrimaryScreen();
                    }
                    break;
                case SCREEN_OF_MAIN_WINDOW:
                    {
                        screen = this.WindowGetCurrentScreen(MAIN_WINDOW_ID);
                    }
                    break;
                default:
                    break;
            }

            var data = new EnumRectData(0, screen);

            User32.EnumDisplayMonitors(default, default, new(MonitorEnumProcUsableSize), ref data);

            data.Rect = data.Rect with { Position = data.Rect.Position - GetScreensOrigin() };

            return data.Rect;
        }
    }

    public override void ScreenSetOrientation(ScreenOrientation orientation) => throw new NotImplementedException();

    public override void ShowWindow(int window)
    {
        if (ERR_FAIL_COND(!this.windows.ContainsKey(window)))
        {
            return;
        }

        var wd = this.windows[window];

        this.PopupOpen(window);

        if (window != MAIN_WINDOW_ID)
        {
            this.UpdateWindowStyle(window);
        }

        if (wd.Maximized)
        {
            User32.ShowWindow(wd.HWnd, User32.SHOW_WINDOW_COMMANDS.SW_SHOWMAXIMIZED);
            User32.SetForegroundWindow(wd.HWnd); // Slightly higher priority.
            User32.SetFocus(wd.HWnd); // Set keyboard focus.
        }
        else if (wd.Minimized)
        {
            User32.ShowWindow(wd.HWnd, User32.SHOW_WINDOW_COMMANDS.SW_SHOWMINIMIZED);
        }
        else if (wd.NoFocus || wd.IsPopup)
        {
            // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow
            User32.ShowWindow(wd.HWnd, User32.SHOW_WINDOW_COMMANDS.SW_SHOWNA);
        }
        else
        {
            User32.ShowWindow(wd.HWnd, User32.SHOW_WINDOW_COMMANDS.SW_SHOW);
            User32.SetForegroundWindow(wd.HWnd); // Slightly higher priority.
            User32.SetFocus(wd.HWnd); // Set keyboard focus.
        }

        if (wd.AlwaysOnTop)
        {
            User32.SetWindowPos(
                wd.HWnd,
                User32.WINDOW_ZORDER.HWND_TOPMOST,
                0,
                0,
                0,
                0,
                User32.WINDOW_POS_FLAGS.SWP_FRAMECHANGED
                | User32.WINDOW_POS_FLAGS.SWP_NOMOVE
                | User32.WINDOW_POS_FLAGS.SWP_NOSIZE
                | ((wd.NoFocus || wd.IsPopup) ? User32.WINDOW_POS_FLAGS.SWP_NOACTIVATE : 0)
            );
        }
    }

    #pragma warning disable IDE0022
    public override void SwapBuffers()
    {
        #if GLES3_ENABLED
        this.glManager?.SwapBuffers();
        #endif
    }
    #pragma warning restore IDE0022

    public override void WindowAttachInstanceId(int instance, int window)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND(!this.windows.ContainsKey(window)))
            {
                return;
            }

            this.windows[window].InstanceId = instance;
        }
    }

    public override bool WindowCanDraw() => this.WindowCanDraw(MAIN_WINDOW_ID);

    public override int WindowGetCurrentScreen(int window)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND_V(!this.windows.ContainsKey(window)))
            {
                return -1;
            }

            var data = new EnumScreenData(0, 0, User32.MonitorFromWindow(this.windows[window].HWnd, User32.MONITOR.MONITOR_DEFAULTTONEAREST));

            User32.EnumDisplayMonitors(default, default, new(MonitorEnumProcScreen), ref data);

            return data.Screen;

        }
    }

    public override int ScreenGetDpi(int screen = SCREEN_OF_MAIN_WINDOW)
    {
        lock (padlock)
        {
            switch (screen)
            {
                case SCREEN_PRIMARY:
                    screen = GetPrimaryScreen();
                    break;
                case SCREEN_OF_MAIN_WINDOW:
                    screen = this.WindowGetCurrentScreen(MAIN_WINDOW_ID);
                    break;
                default:
                    break;
            }

            var data = new EnumDpiData(0, screen, 72);

            User32.EnumDisplayMonitors(default, default, new(MonitorEnumProcDpi), ref data);

            return data.Dpi;
        }
    }

    public override bool WindowGetFlag(WindowFlags windowFlags, int window)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND_V(!this.windows.ContainsKey(window)))
            {
                return false;
            }

            var wd = this.windows[window];

            return windowFlags switch
            {
                WindowFlags.WINDOW_FLAG_RESIZE_DISABLED   => !wd.Resizable,
                WindowFlags.WINDOW_FLAG_BORDERLESS        => wd.Borderless,
                WindowFlags.WINDOW_FLAG_ALWAYS_ON_TOP     => wd.AlwaysOnTop,
                WindowFlags.WINDOW_FLAG_TRANSPARENT       => wd.LayeredWindow,
                WindowFlags.WINDOW_FLAG_NO_FOCUS          => wd.NoFocus,
                WindowFlags.WINDOW_FLAG_MOUSE_PASSTHROUGH => wd.Mpass,
                WindowFlags.WINDOW_FLAG_POPUP             => wd.IsPopup,
                _ => false,
            };
        }
    }

    public override float ScreenGetMaxScale() => throw new NotImplementedException();

    public override WindowMode WindowGetMode(int window)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND_V(!this.windows.ContainsKey(window)))
            {
                return WindowMode.WINDOW_MODE_WINDOWED;
            }

            var wd = this.windows[window];

            return wd.Fullscreen
                ? wd.MultiwindowFs ? WindowMode.WINDOW_MODE_FULLSCREEN : WindowMode.WINDOW_MODE_EXCLUSIVE_FULLSCREEN
                : wd.Minimized
                    ? WindowMode.WINDOW_MODE_MINIMIZED
                    : wd.Maximized ? WindowMode.WINDOW_MODE_MAXIMIZED : WindowMode.WINDOW_MODE_WINDOWED;
        }

    }

    public override Vector2<int> WindowGetPosition(int window)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND_V(!this.windows.ContainsKey(window)))
            {
                return default;
            }

            var wd = this.windows[window];

            if (wd.Minimized)
            {
                return wd.LastPos;
            }

            var point = new POINT
            {
                x = 0,
                y = 0
            };

            User32.ClientToScreen(wd.HWnd, ref point);

            return new Vector2<int>(point.x, point.y) - GetScreensOrigin();
        }
    }

    public override Vector2<int> WindowGetSize(int window)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND_V(!this.windows.ContainsKey(window)))
            {
                return default;
            }

            var wd = this.windows[window];

            // GetClientRect() returns a zero rect for a minimized window, so we need to get the size in another way.
            return wd.Minimized
                ? new(wd.Width, wd.Height)
                : User32.GetClientRect(wd.HWnd, out var r) ? new(r.right - r.left, r.bottom - r.top) : default;
        }
    }

    public override VSyncMode WindowGetVsyncMode(int window) => throw new NotImplementedException();

    public override void WindowSetCurrentScreen(int screen, int window = default)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND(!this.windows.ContainsKey(window)))
            {
                return;
            }

            if (ERR_FAIL_INDEX(screen, this.GetScreenCount()))
            {
                return;
            }

            if (this.WindowGetCurrentScreen(window) == screen)
            {
                return;
            }
            var wd = this.windows[window];

            if (wd.Fullscreen)
            {
                var pos = this.ScreenGetPosition(screen) + GetScreensOrigin();
                var size = this.ScreenGetSize(screen);

                User32.MoveWindow(wd.HWnd, pos.X, pos.Y, size.X, size.Y, true);
            }
            else
            {
                var srect = this.ScreenGetUsableRect(screen);
                var wpos  = this.WindowGetPosition(window) - this.ScreenGetPosition(this.WindowGetCurrentScreen(window));
                var wsize = this.WindowGetSize(window);

                wpos += srect.Position;

                wpos.X = Math.Clamp(wpos.X, srect.Position.X, srect.Position.X + srect.Size.X - wsize.X / 3);
                wpos.Y = Math.Clamp(wpos.Y, srect.Position.Y, srect.Position.Y + srect.Size.Y - wsize.Y / 3);

                this.WindowSetPosition(wpos, window);
            }

            // Don't let the mouse leave the window when resizing to a smaller resolution.
            if (this.mouseMode is MouseMode.MOUSE_MODE_CONFINED or MouseMode.MOUSE_MODE_CONFINED_HIDDEN)
            {
                User32.GetClientRect(wd.HWnd, out var crect);
                User32.ClientToScreen(wd.HWnd, crect.left);
                User32.ClientToScreen(wd.HWnd, crect.right);
                User32.ClipCursor(crect);
            }
        }
    }

    public override void WindowSetDropFilesCallback(Action<string[]> callback, int window)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND(!this.windows.ContainsKey(window)))
            {
                return;
            }

            this.windows[window].DropFilesCallback = callback;
        }
    }

    public override void WindowSetExclusive(int window, bool exclusive) => throw new NotImplementedException();

    public override void WindowSetFlag(WindowFlags flag, bool enabled, int window = 0)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND(!this.windows.ContainsKey(window)))
            {
                return;
            }

            var wd = this.windows[window];

            switch (flag)
            {
                case WindowFlags.WINDOW_FLAG_RESIZE_DISABLED:
                    {
                        wd.Resizable = !enabled;
                        this.UpdateWindowStyle(window);
                    }
                    break;
                case WindowFlags.WINDOW_FLAG_BORDERLESS:
                    {
                        wd.Borderless = enabled;
                        this.UpdateWindowStyle(window);
                        this.UpdateWindowMousePassthrough(window);
                        User32.ShowWindow(wd.HWnd, (wd.NoFocus || wd.IsPopup) ? User32.SHOW_WINDOW_COMMANDS.SW_SHOWNOACTIVATE : User32.SHOW_WINDOW_COMMANDS.SW_SHOW); // Show the window.
                    }
                    break;
                case WindowFlags.WINDOW_FLAG_ALWAYS_ON_TOP:
                    {
                        if (ERR_FAIL_COND_MSG(wd.TransientParent != INVALID_WINDOW_ID && enabled, "Transient windows can't become on top"))
                        {
                            return;
                        }

                        wd.AlwaysOnTop = enabled;
                        this.UpdateWindowStyle(window);
                    }
                    break;
                case WindowFlags.WINDOW_FLAG_TRANSPARENT:
                    {
                        if (enabled)
                        {
                            //enable per-pixel alpha

                            var hRgn = GDI32.CreateRectRgn(0, 0, -1, -1);

                            var bb = new DwmApi.DWM_BLURBEHIND
                            {
                                dwFlags  = DwmApi.DWM_BB_ENABLE | DwmApi.DWM_BB_BLURREGION,
                                hRgnBlur = hRgn,
                                fEnable  = true
                            };

                            DwmApi.DwmEnableBlurBehindWindow(wd.HWnd, ref bb);

                            wd.LayeredWindow = true;
                        }
                        else
                        {
                            //disable per-pixel alpha
                            wd.LayeredWindow = false;

                            var hRgn = GDI32.CreateRectRgn(0, 0, -1, -1);

                            var bb = new DwmApi.DWM_BLURBEHIND
                            {
                                dwFlags  = DwmApi.DWM_BB_ENABLE | DwmApi.DWM_BB_BLURREGION,
                                hRgnBlur = hRgn,
                                fEnable  = false
                            };

                            DwmApi. DwmEnableBlurBehindWindow(wd.HWnd, ref bb);
                        }
                    }
                    break;
                case WindowFlags.WINDOW_FLAG_NO_FOCUS:
                    {
                        wd.NoFocus = enabled;
                        this.UpdateWindowStyle(window);
                    }
                    break;
                case WindowFlags.WINDOW_FLAG_POPUP:
                    {
                        if (ERR_FAIL_COND_MSG(window == MAIN_WINDOW_ID, "Main window can't be popup."))
                        {
                            return;
                        }

                        if (ERR_FAIL_COND_MSG(User32.IsWindowVisible(wd.HWnd) && wd.IsPopup != enabled, "Popup flag can't changed while window is opened."))
                        {
                            return;
                        }

                        wd.IsPopup = enabled;
                    }
                    break;
                default:
                    break;
            }

            this.windows[window] = wd;
        }
    }

    public override void WindowSetInputEventCallback(Action<InputEvent> callback, int window)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND(!this.windows.ContainsKey(window)))
            {
                return;
            }

            this.windows[window].InputEventCallback = callback;
        }
    }

    public override void WindowSetInputTextCallback(Action<string> callback, int window)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND(!this.windows.ContainsKey(window)))
            {
                return;
            }

            this.windows[window].InputTextCallback = callback;
        }
    }

    public override void WindowSetMaxSize(in Vector2<int> size, int window)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND(!this.windows.ContainsKey(window)))
            {
                return;
            }

            var wd = this.windows[window];

            if (size != default && (size.X < wd.MinSize.X || size.Y < wd.MinSize.Y))
            {
                ERR_PRINT("Maximum window size can't be smaller than minimum window size!");
                return;
            }

            wd.MaxSize = size.As<RealT>();
        }
    }

    public override void WindowSetMinSize(in Vector2<int> size, int window)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND(!this.windows.ContainsKey(window)))
            {
                return;
            }

            var wd = this.windows[window];

            if (size != default && wd.MaxSize != default && (size.X > wd.MaxSize.X || size.Y > wd.MaxSize.Y))
            {
                ERR_PRINT("Maximum window size can't be smaller than minimum window size!");
                return;
            }

            wd.MaxSize = size.As<RealT>();
        }
    }

    public override void WindowSetMode(WindowMode windowMode) => throw new NotImplementedException();

    public override void WindowSetMousePassthrough(in Vector2<float>[] mpath, int window) => throw new NotImplementedException();

    public override void WindowSetPosition(in Vector2<int> position, int window = default)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND(!this.windows.ContainsKey(window)))
            {
                return;
            }

            var wd = this.windows[window];

            if (wd.Fullscreen || wd.Maximized) {
                return;
            }

            var offset = GetScreensOrigin();

            var rc = new RECT
            {
                left   = position.X + offset.X,
                right  = position.X + wd.Width + offset.X,
                bottom = position.Y + wd.Height + offset.Y,
                top    = position.Y + offset.Y
            };

            var style   = (User32.WINDOW_STYLES)(int)User32.GetWindowLongPtrW(wd.HWnd, User32.WINDOW_LONG_INDEX.GWL_STYLE);
            var exStyle = (User32.WINDOW_STYLES_EX)(int)User32.GetWindowLongPtrW(wd.HWnd, User32.WINDOW_LONG_INDEX.GWL_EXSTYLE);

            User32.AdjustWindowRectEx(ref rc, style, false, exStyle);
            User32.MoveWindow(wd.HWnd, rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top, true);

            // Don't let the mouse leave the window when moved.
            if (this.mouseMode is MouseMode.MOUSE_MODE_CONFINED or MouseMode.MOUSE_MODE_CONFINED_HIDDEN)
            {
                User32.GetClientRect(wd.HWnd, out var rect);
                User32.ClientToScreen(wd.HWnd, rect.left);
                User32.ClientToScreen(wd.HWnd, rect.right);
                User32.ClipCursor(rect);
            }

            wd.LastPos = position;
            this.UpdateRealMousePosition(window);
        }

    }

    public override void WindowSetRectChangedCallback(Action<Rect2<int>> callback, int window)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND(!this.windows.ContainsKey(window)))
            {
                return;
            }

            this.windows[window].RectChangedCallback = callback;
        }
    }

    public override void WindowSetSize(in Vector2<int> size, int window)
    {
        lock (padlock)
        {

            if (ERR_FAIL_COND(!this.windows.ContainsKey(window)))
            {
                return;
            }

            var wd = this.windows[window];

            if (wd.Fullscreen || wd.Maximized)
            {
                return;
            }

            var w = size.X;
            var h = size.Y;

            wd.Width  = w;
            wd.Height = h;

            #if VULKAN_ENABLED
            this.contextVulkan?.WindowResize(window, w, h);
            #endif
            #if GLES3_ENABLED
            this.glManager?.WindowResize(window, w, h);
            #endif

            User32.GetWindowRect(wd.HWnd, out var rect);

            if (!wd.Borderless)
            {
                User32.GetClientRect(wd.HWnd, out var crect);

                w += rect.right - rect.left - (crect.right - crect.left);
                h += rect.bottom - rect.top - (crect.bottom - crect.top);
            }

            User32.MoveWindow(wd.HWnd, rect.left, rect.top, w, h, true);
        }
    }

    // void DisplayServerWindows::window_set_title(const String &p_title, WindowID window)
    public override void WindowSetTitle(string? title, int window)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND(!this.windows.ContainsKey(window)))
            {

            }

            User32.SetWindowTextW(this.windows[window].HWnd, title);
        }
    }

    public override void WindowSetTransient(int window, int parent)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND(window == parent) || ERR_FAIL_COND(!this.windows.ContainsKey(window)))
            {
                return;
            }

            var wdWindow = this.windows[window];

            if (ERR_FAIL_COND(wdWindow.TransientParent == parent) || ERR_FAIL_COND_MSG(wdWindow.AlwaysOnTop, "Windows with the 'on top' can't become transient."))
            {
                return;
            }

            if (parent == INVALID_WINDOW_ID)
            {
                // Remove transient.

                if (ERR_FAIL_COND(wdWindow.TransientParent == INVALID_WINDOW_ID) || ERR_FAIL_COND(!this.windows.ContainsKey(wdWindow.TransientParent)))
                {
                    return;
                }

                var wdParent = this.windows[wdWindow.TransientParent];

                wdWindow.TransientParent = INVALID_WINDOW_ID;
                wdParent.TransientChildren.Remove(window);

                if (wdWindow.Exclusive)
                {
                    User32.SetWindowLongPtrW(wdWindow.HWnd, User32.WINDOW_LONG_INDEX.GWLP_HWNDPARENT, default);
                }
            }
            else
            {
                if (ERR_FAIL_COND(!this.windows.ContainsKey(parent)) || ERR_FAIL_COND_MSG(wdWindow.TransientParent != INVALID_WINDOW_ID, "Window already has a transient parent"))
                {
                    return;
                }

                var wdParent = this.windows[parent];

                wdWindow.TransientParent = parent;
                wdParent.TransientChildren.Add(window);

                if (wdWindow.Exclusive)
                {
                    User32.SetWindowLongPtrW(wdWindow.HWnd, User32.WINDOW_LONG_INDEX.GWLP_HWNDPARENT, new(wdParent.HWnd.Value));
                }
            }
        }
    }

    public override void WindowSetWindowEventCallback(Action<WindowEvent> callback, int window)
    {
        lock (padlock)
        {
            if (ERR_FAIL_COND(!this.windows.ContainsKey(window)))
            {
                return;
            }

            this.windows[window].WindowEventCallback = callback;
        }
    }



    #endregion public override methods
}
