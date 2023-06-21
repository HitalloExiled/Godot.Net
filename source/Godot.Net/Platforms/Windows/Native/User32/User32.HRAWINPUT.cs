namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.CompilerServices;

internal static partial class User32
{
    public readonly record struct HRAWINPUT(nint Value = default)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator RAWINPUT*(HRAWINPUT value) => (RAWINPUT*)value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator HRAWINPUT(RAWINPUT* value) => new(new(value));
    }
}
