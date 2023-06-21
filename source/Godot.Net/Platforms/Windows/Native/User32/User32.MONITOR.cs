namespace Godot.Net.Platforms.Windows.Native;
internal static partial class User32
{
    public enum MONITOR : int
    {
        /// <summary>
        /// Returns NULL.
        /// </summary>
        MONITOR_DEFAULTTONULL = 0x00000000,

        /// <summary>
        /// Returns a handle to the primary display monitor.
        /// </summary>
        MONITOR_DEFAULTTOPRIMARY = 0x00000001,

        /// <summary>
        /// Returns a handle to the nearest display monitor.
        /// </summary>
        MONITOR_DEFAULTTONEAREST = 0x00000002
    }
}
