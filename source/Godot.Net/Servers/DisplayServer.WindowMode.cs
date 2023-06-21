namespace Godot.Net.Servers;

public abstract partial class DisplayServer
{
    public enum WindowMode
    {
        WINDOW_MODE_WINDOWED,
        WINDOW_MODE_MINIMIZED,
        WINDOW_MODE_MAXIMIZED,
        WINDOW_MODE_FULLSCREEN,
        WINDOW_MODE_EXCLUSIVE_FULLSCREEN,
    };
}
