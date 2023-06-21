namespace Godot.Net.Servers;

public abstract partial class DisplayServer
{
    public enum WindowFlags
    {
        WINDOW_FLAG_RESIZE_DISABLED,
        WINDOW_FLAG_BORDERLESS,
        WINDOW_FLAG_ALWAYS_ON_TOP,
        WINDOW_FLAG_TRANSPARENT,
        WINDOW_FLAG_NO_FOCUS,
        WINDOW_FLAG_POPUP,
        WINDOW_FLAG_EXTEND_TO_TITLE,
        WINDOW_FLAG_MOUSE_PASSTHROUGH,
        WINDOW_FLAG_MAX,
    };
}
