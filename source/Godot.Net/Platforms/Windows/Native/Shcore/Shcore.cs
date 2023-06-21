namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.InteropServices;

internal static partial class Shcore
{
    [LibraryImport(nameof(Shcore))]
    public static unsafe partial HRESULT GetDpiForMonitor(HMONITOR hmonitor, MONITOR_DPI_TYPE dpiType, UINT* dpiX, UINT* dpiY);

    [LibraryImport(nameof(Shcore))]
    public static partial HRESULT GetDpiForMonitor(HMONITOR hmonitor, MONITOR_DPI_TYPE dpiType, out UINT dpiX, out UINT dpiY);
}
