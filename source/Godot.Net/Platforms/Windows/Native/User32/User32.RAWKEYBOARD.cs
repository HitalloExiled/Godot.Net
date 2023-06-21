namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.InteropServices;

internal static partial class User32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct RAWKEYBOARD
    {
        public USHORT MakeCode;
        public USHORT Flags;
        public USHORT Reserved;
        public USHORT VKey;
        public UINT   Message;
        public ULONG  ExtraInformation;
    }
}
