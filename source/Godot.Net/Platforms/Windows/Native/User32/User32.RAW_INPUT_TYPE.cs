namespace Godot.Net.Platforms.Windows.Native;

internal static partial class User32
{
    public enum RAW_INPUT_TYPE
    {
        /// <summary>
        /// Raw input comes from the mouse.
        /// </summary>
        RIM_TYPEMOUSE = 0,
        /// <summary>
        /// Raw input comes from the keyboard.
        /// </summary>
        RIM_TYPEKEYBOARD = 1,
        /// <summary>
        /// Raw input comes from some device that is not a keyboard or a mouse.
        /// </summary>
        RIM_TYPEHID = 2
    }
}
