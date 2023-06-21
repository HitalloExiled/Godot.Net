namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal static partial class User32
{
    public record struct HOOKPROC(nint Value = default)
    {
        public delegate LRESULT Function(int code, WINDOW_MESSAGE wParam, LPARAM lParam);

        public HOOKPROC(Function value) : this(Marshal.GetFunctionPointerForDelegate(value))
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator nint(HOOKPROC value) => value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator Function(HOOKPROC value) => Marshal.GetDelegateForFunctionPointer<Function>(value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator HOOKPROC(Function value) => new(value);
    }
}
