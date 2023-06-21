namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.CompilerServices;

internal static partial class User32
{
    public readonly record struct LPMONITORINFO(nint Value = default)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator MONITORINFO*(LPMONITORINFO value) => (MONITORINFO*)value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator LPMONITORINFO(MONITORINFO* value) => new(new(value));
    }
}
