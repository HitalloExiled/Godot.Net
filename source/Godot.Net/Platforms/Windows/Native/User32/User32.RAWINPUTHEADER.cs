namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.InteropServices;

internal static partial class User32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RAWINPUTHEADER
    {
        public RAW_INPUT_TYPE dwType;
        public DWORD          dwSize;
        public HANDLE         hDevice;
        public WPARAM         wParam;
    }
}
