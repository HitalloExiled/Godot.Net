namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

internal static partial class User32
{
    public record struct TIMERPROC(nint Value = default)
    {
        public delegate void Function(HWND unnamedParam1, UINT unnamedParam2, UINT_PTR unnamedParam3, DWORD unnamedParam4);

        public TIMERPROC(Function value) : this(Marshal.GetFunctionPointerForDelegate(value))
        { }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator nint(TIMERPROC value) => value.Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator Function(TIMERPROC value) => Marshal.GetDelegateForFunctionPointer<Function>(value.Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static implicit operator TIMERPROC(Function value) => new(value);
    }
}
