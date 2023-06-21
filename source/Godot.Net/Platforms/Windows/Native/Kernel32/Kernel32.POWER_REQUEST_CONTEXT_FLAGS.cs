namespace Godot.Net.Platforms.Windows.Native;

internal static partial class Kernel32
{
    [Flags]
    public enum POWER_REQUEST_CONTEXT_FLAGS : uint
    {
        POWER_REQUEST_CONTEXT_VERSION         = 0,
        POWER_REQUEST_CONTEXT_DETAILED_STRING = 2U,
        POWER_REQUEST_CONTEXT_SIMPLE_STRING   = 1U,
    }
}
