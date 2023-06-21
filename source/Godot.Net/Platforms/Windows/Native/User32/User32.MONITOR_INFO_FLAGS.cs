namespace Godot.Net.Platforms.Windows.Native;

using System;

/// <summary>
/// Contains the <see cref="MONITOR_INFO_FLAGS"/> nested type.
/// </summary>
internal static partial class User32
{
    /// <summary>
    /// Flags that may be defined on the <see cref="MONITORINFO.dwFlags"/> field.
    /// </summary>
    [Flags]
    public enum MONITOR_INFO_FLAGS
    {
        /// <summary>
        /// No flags.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// This is the primary display monitor.
        /// </summary>
        MONITORINFOF_PRIMARY = 0x1,
    }
}
