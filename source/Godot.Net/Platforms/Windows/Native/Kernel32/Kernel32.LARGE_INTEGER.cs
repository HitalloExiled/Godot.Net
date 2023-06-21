
namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.InteropServices;

internal static partial class Kernel32
{
    [StructLayout(LayoutKind.Explicit)]
    public struct LARGE_INTEGER
    {
        [FieldOffset(0)]
        public int LowPart;

        [FieldOffset(4)]
        public int HighPart;

        [FieldOffset(0)]
        public long QuadPart;

        public static implicit operator long(LARGE_INTEGER value) =>
            value.QuadPart;
    }
}
