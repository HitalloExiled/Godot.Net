namespace Godot.Net.Platforms.Windows.Native;

/// <content>
/// Contains the <see cref="MONITORINFO"/> nested type.
/// </content>
internal static partial class User32
{
    /// <summary>
    /// The <see cref="MONITORINFO"/> structure contains information about a display monitor.
    /// The <see cref="GetMonitorInfoW(HMONITOR, LPMONITORINFO)"/> function stores information in a <see cref="MONITORINFO"/> structure or a <see cref="MONITORINFOEX"/> structure.
    /// The <see cref="MONITORINFO"/> structure is a subset of the <see cref="MONITORINFOEX"/> structure. The <see cref="MONITORINFOEX"/> structure adds a string member to contain a name for the display monitor.
    /// </summary>
    public struct MONITORINFO
    {
        /// <summary>
        /// The size of the structure, in bytes.
        /// Set this member to <c>sizeof(MONITORINFO)</c> before calling the <see cref="GetMonitorInfoW(HMONITOR, LPMONITORINFO)"/> function.
        /// Doing so lets the function determine the type of structure you are passing to it.
        /// </summary>
        public DWORD cbSize;

        /// <summary>
        /// A <see cref="RECT"/> structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates. Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public RECT rcMonitor;

        /// <summary>
        /// A <see cref="RECT"/> structure that specifies the work area rectangle of the display monitor, expressed in virtual-screen coordinates. Note that if the monitor is not the primary display monitor, some of the rectangle's coordinates may be negative values.
        /// </summary>
        public RECT rcWork;

        /// <summary>
        /// A set of flags that represent attributes of the display monitor.
        /// </summary>
        public MONITOR_INFO_FLAGS dwFlags;
    }
}
