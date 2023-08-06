namespace Godot.Net.Platforms.Windows.Native;

internal static partial class Shcore
{
    public enum MONITOR_DPI_TYPE
    {
        MDT_EFFECTIVE_DPI = 0,
        MDT_ANGULAR_DPI   = 1,
        MDT_RAW_DPI       = 2,
        MDT_DEFAULT       = MDT_EFFECTIVE_DPI
    }
}
