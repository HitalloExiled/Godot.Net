namespace Godot.Net.Servers;

public abstract partial class DisplayServer
{
    public enum MouseMode
    {
        MOUSE_MODE_VISIBLE,
        MOUSE_MODE_HIDDEN,
        MOUSE_MODE_CAPTURED,
        MOUSE_MODE_CONFINED,
        MOUSE_MODE_CONFINED_HIDDEN,
    };
}
