namespace Godot.Net.Platforms.Windows.Native;

using System.Diagnostics;
using System.Runtime.CompilerServices;

internal static partial class GDI32
{
    [DebuggerDisplay("{Value}")]
    public readonly unsafe record struct LPCOLORREF(nint Value = default)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator DWORD*(LPCOLORREF value) => (DWORD*)value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator LPCOLORREF(DWORD* value) => new(new(value));
    }
}
