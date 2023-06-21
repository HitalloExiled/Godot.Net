namespace Godot.Net.Scene.Main;

using Godot.Net.Servers;

public partial class Window
{
    public enum Flags
    {
        FLAG_RESIZE_DISABLED   = DisplayServer.WindowFlags.WINDOW_FLAG_RESIZE_DISABLED,
        FLAG_BORDERLESS        = DisplayServer.WindowFlags.WINDOW_FLAG_BORDERLESS,
        FLAG_ALWAYS_ON_TOP     = DisplayServer.WindowFlags.WINDOW_FLAG_ALWAYS_ON_TOP,
        FLAG_TRANSPARENT       = DisplayServer.WindowFlags.WINDOW_FLAG_TRANSPARENT,
        FLAG_NO_FOCUS          = DisplayServer.WindowFlags.WINDOW_FLAG_NO_FOCUS,
        FLAG_POPUP             = DisplayServer.WindowFlags.WINDOW_FLAG_POPUP,
        FLAG_EXTEND_TO_TITLE   = DisplayServer.WindowFlags.WINDOW_FLAG_EXTEND_TO_TITLE,
        FLAG_MOUSE_PASSTHROUGH = DisplayServer.WindowFlags.WINDOW_FLAG_MOUSE_PASSTHROUGH,
        FLAG_MAX               = DisplayServer.WindowFlags.WINDOW_FLAG_MAX,
    };
}
