namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.InteropServices;

internal static partial class User32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RAWHID
    {
        public DWORD dwSizeHid;
        public DWORD dwCount;
        public BYTE  bRawData;
    }
}
