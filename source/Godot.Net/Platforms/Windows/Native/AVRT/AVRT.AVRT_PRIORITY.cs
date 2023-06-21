namespace Godot.Net.Platforms.Windows.Native;

internal static partial class AVRT
{
    public enum AVRT_PRIORITY
    {
        AVRT_PRIORITY_CRITICAL = 2,
        AVRT_PRIORITY_HIGH     = 1,
        AVRT_PRIORITY_LOW      = -1,
        AVRT_PRIORITY_NORMAL   = 0,
    }
}
