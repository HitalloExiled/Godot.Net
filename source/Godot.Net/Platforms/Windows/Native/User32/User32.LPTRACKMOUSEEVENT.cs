namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.CompilerServices;

internal static partial class User32
{
    public readonly record struct LPTRACKMOUSEEVENT(nint Value = default)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator TRACKMOUSEEVENT*(LPTRACKMOUSEEVENT value) => (TRACKMOUSEEVENT*)value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator LPTRACKMOUSEEVENT(MSG* value) => new(new(value));
    }
}
