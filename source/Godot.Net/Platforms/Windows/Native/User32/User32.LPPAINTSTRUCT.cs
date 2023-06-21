namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.CompilerServices;

internal static partial class User32
{
    public readonly record struct LPPAINTSTRUCT(nint Value = default)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator PAINTSTRUCT*(LPPAINTSTRUCT value) => (PAINTSTRUCT*)value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator LPPAINTSTRUCT(PAINTSTRUCT* value) => new(new(value));
    }
}
