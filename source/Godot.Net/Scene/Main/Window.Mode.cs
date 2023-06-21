namespace Godot.Net.Scene.Main;

using Godot.Net.Servers;

public partial class Window
{
    public enum Mode
    {
        MODE_WINDOWED             = DisplayServer.WindowMode.WINDOW_MODE_WINDOWED,
        MODE_MINIMIZED            = DisplayServer.WindowMode.WINDOW_MODE_MINIMIZED,
        MODE_MAXIMIZED            = DisplayServer.WindowMode.WINDOW_MODE_MAXIMIZED,
        MODE_FULLSCREEN           = DisplayServer.WindowMode.WINDOW_MODE_FULLSCREEN,
        MODE_EXCLUSIVE_FULLSCREEN = DisplayServer.WindowMode.WINDOW_MODE_EXCLUSIVE_FULLSCREEN,
    };
}
