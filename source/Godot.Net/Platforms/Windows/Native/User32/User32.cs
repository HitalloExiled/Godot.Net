namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.InteropServices;

internal static partial class User32
{
    public const uint   HOVER_DEFAULT      = 0xFFFFFFFF;
    public const uint   USER_TIMER_MINIMUM = 0x0000000A;
    public const uint   USER_TIMER_MAXIMUM = 0x7FFFFFFF;
    public const ushort WHEEL_DELTA        = 120;

    ///<summary>Deactivated.</summary>
    public const int WA_INACTIVE = 0;

    ///<summary>
    ///Activated by some method other than a mouse click (for example, by a call to
    ///the SetActiveWindow function or by use of the keyboard interface to select the window).
    ///</summary>
    public const int WA_ACTIVE = 1;

    ///<summary>Activated by a mouse click.</summary>
    public const int WA_CLICKACTIVE = 2;

    [LibraryImport(nameof(User32))]
    public static partial BOOL AdjustWindowRectEx(LPRECT lpRect, WINDOW_STYLES dwStyle, BOOL bMenu, WINDOW_STYLES_EX dwExStyle);

    [LibraryImport(nameof(User32))]
    public static partial BOOL AdjustWindowRectEx(ref RECT lpRect, WINDOW_STYLES dwStyle, BOOL bMenu, WINDOW_STYLES_EX dwExStyle);

    [LibraryImport(nameof(User32))]
    public static partial HDC BeginPaint(HWND hWnd, LPPAINTSTRUCT lpPaint);

    [LibraryImport(nameof(User32))]
    public static partial HDC BeginPaint(HWND hWnd, out PAINTSTRUCT lpPaint);

    [LibraryImport(nameof(User32))]
    public static partial LRESULT CallNextHookEx(HHOOK hhk, int nCode, WINDOW_MESSAGE wParam, LPARAM lParam);

    [LibraryImport(nameof(User32))]
    public static partial LRESULT CallWindowProcW(WNDPROC lpPrevWndFunc, HWND hWnd, WINDOW_MESSAGE msg, WPARAM wParam, LPARAM lParam);

    [LibraryImport(nameof(User32))]
    public static partial BOOL ClientToScreen(HWND hWnd, LPPOINT lpPoint);

    [LibraryImport(nameof(User32))]
    public static partial BOOL ClientToScreen(HWND hWnd, ref POINT lpPoint);

    [LibraryImport(nameof(User32))]
    public static unsafe partial BOOL ClipCursor(RECT* lpRect);

    [LibraryImport(nameof(User32))]
    public static unsafe partial BOOL ClipCursor(in RECT lpRect);

    [LibraryImport(nameof(User32))]
    public static unsafe partial HRGN CreatePolygonRgn(POINT* pptl, int cPoint, FILL_MODE iMode);

    public static unsafe HRGN CreatePolygonRgn(POINT[] points, FILL_MODE iMode)
    {
        fixed (POINT* pptl = points)
        {
            return CreatePolygonRgn(pptl, points.Length, iMode);
        }
    }

    [LibraryImport(nameof(User32))]
    public static partial HWND CreateWindowExW(
        WINDOW_STYLES_EX dwExStyle,
        LPCWSTR        lpClassName,
        LPCWSTR        lpWindowName,
        WINDOW_STYLES   dwStyle,
        int            x,
        int            y,
        int            nWidth,
        int            nHeight,
        HWND           hWndParent,
        HMENU          hMenu,
        HINSTANCE      hInstance,
        LPVOID         lpParam
    );

    public static unsafe HWND CreateWindowExW<T>(
        WINDOW_STYLES_EX dwExStyle,
        string?        className,
        string?        windowName,
        WINDOW_STYLES   dwStyle,
        int            x,
        int            y,
        int            nWidth,
        int            nHeight,
        HWND           hWndParent,
        HMENU          hMenu,
        HINSTANCE      hInstance,
        in T           param
    ) where T : unmanaged
    {
        using var lpClassName  = new LPCWSTR(className);
        using var lpWindowName = new LPCWSTR(windowName);

        fixed(void* lpParam = &param)
        {
            return CreateWindowExW(
                dwExStyle,
                lpClassName,
                lpWindowName,
                dwStyle,
                x,
                y,
                nWidth,
                nHeight,
                hWndParent,
                hMenu,
                hInstance,
                lpParam
            );
        }
    }

    [LibraryImport(nameof(User32))]
    public static partial LRESULT DefWindowProcW(HWND hWnd, WINDOW_MESSAGE uMsg, WPARAM wParam, LPARAM lParam);

    [LibraryImport(nameof(User32))]
    public static partial LRESULT DispatchMessageW(in MSG lpMsg);

    [LibraryImport(nameof(User32))]
    public static unsafe partial BOOL EndPaint(HWND hWnd, PAINTSTRUCT* lpPaint);

    [LibraryImport(nameof(User32))]
    public static partial BOOL EndPaint(HWND hWnd, in PAINTSTRUCT lpPaint);

    [LibraryImport(nameof(User32))]
    public static partial BOOL EnumDisplayMonitors(HDC hdc, LPCRECT lprcClip, MONITORENUMPROC lpfnEnum, LPARAM dwData);

    public static unsafe BOOL EnumDisplayMonitors<T>(HDC hdc, in RECT clip, MONITORENUMPROC lpfnEnum, ref T data) where T : unmanaged
    {
        fixed(RECT* lprcClip = &clip)
        fixed(T*    dwData   = &data)
        {
            return EnumDisplayMonitors(hdc, clip == default ? default : lprcClip, lpfnEnum, dwData);
        }
    }

    [LibraryImport(nameof(User32))]
    public static unsafe partial int FillRect(HDC hDC, RECT* lprc, HBRUSH hbr);

    [LibraryImport(nameof(User32))]
    public static partial int FillRect(HDC hDC, in RECT lprc, HBRUSH hbr);

    [LibraryImport(nameof(User32))]
    public static partial BOOL GetClientRect(HWND hWnd, LPRECT lpRect);

    [LibraryImport(nameof(User32))]
    public static partial BOOL GetClientRect(HWND hWnd, out RECT lpRect);

    [LibraryImport(nameof(User32))]
    public static partial BOOL GetCursorPos(LPPOINT lpPoint);

    [LibraryImport(nameof(User32))]
    public static partial BOOL GetCursorPos(out POINT lpPoint);

    [LibraryImport(nameof(User32))]
    public static partial HDC GetDC(HWND hWnd);

    [LibraryImport(nameof(User32))]
    public static partial SHORT GetKeyState(VIRTUAL_KEYS virtualKey);

    [LibraryImport(nameof(User32))]
    public static partial LPARAM GetMessageExtraInfo();

    [LibraryImport(nameof(User32))]
    public static partial BOOL GetMonitorInfoW(HMONITOR hMonitor, LPMONITORINFO lpmi);

    [LibraryImport(nameof(User32))]
    public static partial UINT GetRawInputData(HRAWINPUT hRawInput, RAW_INPUT_DATA_COMMAND_FLAGS uiCommand, LPVOID pData, PUINT pcbSize, UINT cbSizeHeader);

    public static unsafe uint GetRawInputData<T>(ref RAWINPUT rawInput, RAW_INPUT_DATA_COMMAND_FLAGS uiCommand, ref T data, ref UINT size, UINT cbSizeHeader) where T : unmanaged
    {
        fixed (RAWINPUT* pRawInput = &rawInput)
        fixed (void*     pdata     = &data)
        fixed (UINT*     pcbSize   = &size)
        {
            return GetRawInputData(pRawInput, uiCommand, pdata, pcbSize, cbSizeHeader);
        }
    }

    [LibraryImport(nameof(User32))]
    public static partial int GetSystemMetrics(SYSTEM_METRIC smIndex);

    [LibraryImport(nameof(User32))]
    public static partial BOOL GetUpdateRect(HWND hWnd, LPRECT lpRect, BOOL bErase);

    [LibraryImport(nameof(User32))]
    public static partial BOOL GetUpdateRect(HWND hWnd, out RECT rect, BOOL bErase);

    [LibraryImport(nameof(User32))]
    public static partial LONG_PTR GetWindowLongPtrW(HWND hWnd, WINDOW_LONG_INDEX nIndex);

    [LibraryImport(nameof(User32))]
    public static partial BOOL GetWindowRect(HWND hWnd, LPRECT lpRect);

    [LibraryImport(nameof(User32))]
    public static partial BOOL GetWindowRect(HWND hWnd, out RECT lpRect);

    [LibraryImport(nameof(User32))]
    public static partial BOOL IsWindow(HWND hWnd);

    [LibraryImport(nameof(User32))]
    public static partial BOOL IsWindowVisible(HWND hWnd);

    [LibraryImport(nameof(User32))]
    public static partial BOOL KillTimer(HWND hWnd, UINT_PTR uIDEvent);

    [LibraryImport(nameof(User32))]
    public static partial BOOL KillTimer(HWND hWnd, ref UINT uIDEvent);

    [LibraryImport(nameof(User32))]
    public static partial HCURSOR LoadCursorW(HINSTANCE hInstance, LPCWSTR lpCursorName);

    [LibraryImport(nameof(User32))]
    public static partial HCURSOR LoadCursorW(HINSTANCE hInstance, IDC_STANDARD_CURSORS lpCursorName);

    [LibraryImport(nameof(User32))]
    public static partial int MessageBoxW(HWND hWnd, LPCWSTR lpText, LPCWSTR lpCaption, MESSAGE_BOX_OPTIONS uType);

    public static int MessageBoxW(HWND hWnd, string text, string caption, MESSAGE_BOX_OPTIONS type)
    {
        using var lpText    = new LPCWSTR(text);
        using var lpCaption = new LPCWSTR(caption);

        return MessageBoxW(hWnd, lpText, lpCaption, type);
    }

    [LibraryImport(nameof(User32))]
    public static partial HMONITOR MonitorFromWindow(HWND hwnd, DWORD dwFlags);

    [LibraryImport(nameof(User32))]
    public static partial HMONITOR MonitorFromWindow(HWND hwnd, MONITOR dwFlags);

    [LibraryImport(nameof(User32))]
    public static partial BOOL MoveWindow(HWND hWnd, int x, int y, int nWidth, int nHeight, BOOL bRepaint);

    [LibraryImport(nameof(User32))]
    public static partial BOOL PeekMessageW(LPMSG lpMsg, HWND hWnd, UINT wMsgFilterMin, UINT wMsgFilterMax, PEEK_MESSAGE wRemoveMsg);

    [LibraryImport(nameof(User32))]
    public static partial BOOL PeekMessageW(ref MSG lpMsg, HWND hWnd, UINT wMsgFilterMin, UINT wMsgFilterMax, PEEK_MESSAGE wRemoveMsg);

    [LibraryImport(nameof(User32))]
    public static partial ATOM RegisterClassExW(in WNDCLASSEXW lpwcx);

    [LibraryImport(nameof(User32))]
    public static partial BOOL ReleaseCapture();

    [LibraryImport(nameof(User32))]
    public static partial int ReleaseDC(HWND hWnd, HDC hDC);

    [LibraryImport(nameof(User32))]
    public static partial BOOL ScreenToClient(HWND hWnd, LPPOINT lpPoint);

    [LibraryImport(nameof(User32))]
    public static partial BOOL ScreenToClient(HWND hWnd, in POINT lpPoint);

    [LibraryImport(nameof(User32))]
    public static partial HWND SetCapture(HWND hWnd);

    [LibraryImport(nameof(User32))]
    public static partial HCURSOR SetCursor(HCURSOR handle);

    [LibraryImport(nameof(User32))]
    public static partial BOOL SetCursorPos(int x, int y);

    [LibraryImport(nameof(User32))]
    public static partial HWND SetFocus(HWND hWnd);

    [LibraryImport(nameof(User32))]
    public static partial BOOL SetForegroundWindow(HWND hWnd);

    [LibraryImport(nameof(User32))]
    public static partial UINT_PTR SetTimer(HWND hWnd, UINT_PTR nIDEvent, UINT uElapse, TIMERPROC lpTimerFunc);

    [LibraryImport(nameof(User32))]
    public static partial UINT_PTR SetTimer(HWND hWnd, in UINT nIDEvent, UINT uElapse, TIMERPROC lpTimerFunc);

    [LibraryImport(nameof(User32))]
    public static partial LONG_PTR SetWindowLongPtrW(HWND hWnd, WINDOW_LONG_INDEX nIndex, LONG_PTR dwNewLong);

    [LibraryImport(nameof(User32))]
    public static partial LONG_PTR SetWindowLongPtrW(HWND hWnd, WINDOW_LONG_INDEX nIndex, in LONG dwNewLong);

    [LibraryImport(nameof(User32))]
    public static partial BOOL SetWindowPos(HWND hWnd, WINDOW_ZORDER hWndInsertAfter, int x, int y, int cx, int cy, WINDOW_POS_FLAGS uFlags);

    [LibraryImport(nameof(User32))]
    public static partial int SetWindowRgn(HWND hWnd, HRGN hRgn, BOOL bRedraw);

    [LibraryImport(nameof(User32))]
    public static partial HHOOK SetWindowsHookExW(WINDOWS_HOOK_TYPE hookType, HOOKPROC lpfn, HINSTANCE hMod, DWORD dwThreadId);

    [LibraryImport(nameof(User32))]
    public static partial BOOL SetWindowTextW(HWND hWnd, LPCWSTR lpString);

    public static BOOL SetWindowTextW(HWND hWnd, string? text)
    {
        using var lpString = new LPCWSTR(text);

        return SetWindowTextW(hWnd, lpString);
    }

    [LibraryImport(nameof(User32))]
    public static partial BOOL ShowWindow(HWND hWnd, SHOW_WINDOW_COMMANDS nCmdShow);

    [LibraryImport(nameof(User32))]
    public static partial BOOL TrackMouseEvent(LPTRACKMOUSEEVENT lpEventTrack);

    [LibraryImport(nameof(User32))]
    public static partial BOOL TrackMouseEvent(in TRACKMOUSEEVENT eventTrack);

    [LibraryImport(nameof(User32))]
    public static partial BOOL TranslateMessage(in MSG lpMsg);

    [LibraryImport(nameof(User32))]
    public static partial HWND WindowFromPoint(POINT point);
}
