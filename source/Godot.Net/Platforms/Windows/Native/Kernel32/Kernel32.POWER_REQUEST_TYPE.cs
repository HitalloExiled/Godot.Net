namespace Godot.Net.Platforms.Windows.Native;

internal static partial class Kernel32
{
    public enum POWER_REQUEST_TYPE
    {
        POWER_REQUEST_DISPLAY_REQUIRED,
        POWER_REQUEST_SYSTEM_REQUIRED,
        POWER_REQUEST_AWAYMODE_REQUIRED,
        POWER_REQUEST_EXECUTION_REQUIRED,
    }
}
