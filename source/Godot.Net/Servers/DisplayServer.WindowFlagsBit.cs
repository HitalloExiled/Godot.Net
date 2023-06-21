namespace Godot.Net.Servers;

public abstract partial class DisplayServer
{
    [Flags]
    public enum WindowFlagsBit
    {
        WINDOW_FLAG_RESIZE_DISABLED_BIT = 1,
        WINDOW_FLAG_BORDERLESS_BIT      = 2,
        WINDOW_FLAG_ALWAYS_ON_TOP_BIT   = 4,
        WINDOW_FLAG_TRANSPARENT_BIT     = 8,
        WINDOW_FLAG_NO_FOCUS_BIT        = 16,
        WINDOW_FLAG_POPUP_BIT           = 32,
        WINDOW_FLAG_EXTEND_TO_TITLE_BIT = 64,
    };
}
