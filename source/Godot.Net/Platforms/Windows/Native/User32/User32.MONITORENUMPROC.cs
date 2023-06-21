namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal static partial class User32
{
    public record struct MONITORENUMPROC(nint Value = default)
    {
        public delegate BOOL Function(HMONITOR hMonitor, HDC hdc, LPRECT lpRect, LPARAM lParam);

        public MONITORENUMPROC(Function value) : this(Marshal.GetFunctionPointerForDelegate(value))
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator nint(MONITORENUMPROC value) => value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator Function(MONITORENUMPROC value) => Marshal.GetDelegateForFunctionPointer<Function>(value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator MONITORENUMPROC(Function value) => new(value);
    }
}
