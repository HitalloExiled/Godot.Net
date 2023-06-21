namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.InteropServices;

internal static partial class DwmApi
{
    /// <summary>
    /// A value for the fEnable member has been specified.
    /// </summary>
    public const int DWM_BB_ENABLE = 0x00000001;

    /// <summary>
    /// A value for the hRgnBlur member has been specified.
    /// </summary>
    public const int DWM_BB_BLURREGION = 0x00000002;

    /// <summary>
    /// A value for the fTransitionOnMaximized member has been specified.
    /// </summary>
    public const int DWM_BB_TRANSITIONONMAXIMIZED = 0x00000004;


    [LibraryImport(nameof(DwmApi))]
    public static partial HRESULT DwmSetWindowAttribute(HWND hwnd, DWM_WINDOW_ATTRIBUTE attr, LPCVOID attrValue, DWORD attrSize);

    public static unsafe HRESULT DwmSetWindowAttribute<T>(HWND hwnd, DWM_WINDOW_ATTRIBUTE attr, ref T attrValue, DWORD attrSize) where T : unmanaged
    {
        fixed (void* pAttrValue = &attrValue)
        {
            return DwmSetWindowAttribute(hwnd, attr, pAttrValue, attrSize);
        }
    }

    [LibraryImport(nameof(DwmApi))]
    public static partial HRESULT DwmEnableBlurBehindWindow(HWND hWnd, ref DWM_BLURBEHIND pBlurBehind);
}
