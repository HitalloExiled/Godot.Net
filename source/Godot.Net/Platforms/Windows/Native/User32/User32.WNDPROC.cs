namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal static partial class User32
{
    public record struct WNDPROC(nint Value = default)
    {
        public delegate LRESULT Function(HWND hwnd, WINDOW_MESSAGE msg, WPARAM wParam, LPARAM lParam);

        public WNDPROC(Function value) : this(Marshal.GetFunctionPointerForDelegate(value))
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator nint(WNDPROC value) => value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator Function(WNDPROC value) => Marshal.GetDelegateForFunctionPointer<Function>(value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator WNDPROC(Function value) => new(value);
    }
}
