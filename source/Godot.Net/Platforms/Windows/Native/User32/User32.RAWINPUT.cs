namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.InteropServices;

internal static partial class User32
{
    [StructLayout(LayoutKind.Explicit)]
    public struct RAWINPUT
    {
        [FieldOffset(0)]
        public RAWINPUTHEADER header;

        [FieldOffset(16)]
        public RAWMOUSE mouse;

        [FieldOffset(16)]
        public RAWKEYBOARD keyboard;

        [FieldOffset(16)]
        public RAWHID hid;
    }
}
