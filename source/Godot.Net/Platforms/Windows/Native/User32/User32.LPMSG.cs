namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.CompilerServices;

internal static partial class User32
{
    public readonly record struct LPMSG(nint Value = default)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator MSG*(LPMSG value) => (MSG*)value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator LPMSG(MSG* value) => new(new(value));
    }
}
