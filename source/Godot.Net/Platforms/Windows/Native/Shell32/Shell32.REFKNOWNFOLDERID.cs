namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.CompilerServices;

internal static partial class Shell32
{
    public readonly record struct REFKNOWNFOLDERID(GUID Value = default)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator GUID(REFKNOWNFOLDERID value) => value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator REFKNOWNFOLDERID(GUID value) => new(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static unsafe implicit operator REFKNOWNFOLDERID(Guid value) => new(value);
    }
}
