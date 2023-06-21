namespace Godot.Net.Core.Input;

[Flags]
public enum MouseButtonMask
{
    LEFT        = 1 << (MouseButton.LEFT - 1),
    RIGHT       = 1 << (MouseButton.RIGHT - 1),
    MIDDLE      = 1 << (MouseButton.MIDDLE - 1),
    MB_XBUTTON1 = 1 << (MouseButton.MB_XBUTTON1 - 1),
    MB_XBUTTON2 = 1 << (MouseButton.MB_XBUTTON2 - 1),
}
