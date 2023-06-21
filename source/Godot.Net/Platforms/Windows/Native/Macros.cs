namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.CompilerServices;

public static class Macros
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int GET_X_LPARAM(LPARAM value) => LOWORD(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int GET_Y_LPARAM(LPARAM value) => HIWORD(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int LOWORD(nint value) => value.ToInt32() & 0xffff;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int LOWORD(nuint value) => LOWORD((nint)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int HIWORD(nint value) => (value.ToInt32() >> 16) & 0xffff;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int HIWORD(nuint value) => HIWORD((nint)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static long MAKELANGID(int hi, int lo) => (((long)hi) << 32) | ((uint)lo);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static uint RGB(int r, int g, int b) => (uint)(r | (g << 8) | (b << 16));
}
