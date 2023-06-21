namespace Godot.Net.Platforms.Windows.Native;

using System.Diagnostics;
using System.Runtime.CompilerServices;

internal static partial class GDI32
{
    [DebuggerDisplay("{Value}")]
    public readonly record struct COLORREF(DWORD Value = default)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator DWORD(COLORREF value) => value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator COLORREF(DWORD value) => new(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator uint(COLORREF value) => (uint)value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator COLORREF(uint value) => new(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator int(COLORREF value) => (int)(uint)value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator COLORREF(int value) => new(value);
    }
}
