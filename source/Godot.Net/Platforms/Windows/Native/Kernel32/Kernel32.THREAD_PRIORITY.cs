namespace Godot.Net.Platforms.Windows.Native;

#pragma warning disable CA1069

internal static partial class Kernel32
{
    public enum THREAD_PRIORITY
    {
        THREAD_MODE_BACKGROUND_BEGIN  = 65536,
        THREAD_MODE_BACKGROUND_END    = 131072,
        THREAD_PRIORITY_ABOVE_NORMAL  = 1,
        THREAD_PRIORITY_BELOW_NORMAL  = -1,
        THREAD_PRIORITY_HIGHEST       = 2,
        THREAD_PRIORITY_IDLE          = -15,
        THREAD_PRIORITY_MIN           = -2,
        THREAD_PRIORITY_LOWEST        = -2,
        THREAD_PRIORITY_NORMAL        = 0,
        THREAD_PRIORITY_TIME_CRITICAL = 15,
    }
}
