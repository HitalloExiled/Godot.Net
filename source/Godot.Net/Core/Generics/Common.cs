namespace Godot.Net.Core.Generics;

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

public class Common
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T[] FillArray<T>(int size) where T : new() => FillArray<T>((uint)size);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T[] FillArray<T>(uint size) where T : new()
    {
        var elements = new T[size];

        for (var i = 0; i < size; i++)
        {
            elements[i] = new();
        }

        return elements;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T[] FillArray<T>(Func<T> factory, int size) => FillArray(factory, (uint)size);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T[] FillArray<T>(Func<T> factory, uint size)
    {
        var elements = new T[size];

        for (var i = 0; i < size; i++)
        {
            elements[i] = factory();
        }

        return elements;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T[] FillArray<T>(T value, uint size)
    {
        var elements = new T[size];

        for (var i = 0; i < size; i++)
        {
            elements[i] = value;
        }

        return elements;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int GetShiftFromPowerOf2(uint bits)
    {
        for (uint i = 0; i < 32; i++)
        {
            if (bits == (uint)(1 << (int)i))
            {
                return (int)i;
            }
        }

        return -1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T NearestPowerOf2Templated<T>(T value) where T : INumber<T>
    {
        var x = long.CreateSaturating(value) - 1;

        // The number of operations on x is the base two logarithm
        // of the number of bits in the type. Add three to account
        // for sizeof(T) being in bytes.
        var num = GetShiftFromPowerOf2((uint)Marshal.SizeOf<T>()) + 3;

        // If the compiler is smart, it unrolls this loop.
        // If it's dumb, this is a bit slow.
        for (var i = 0; i < num; i++)
        {
            x |= x >> (1 << i);
        }

        return T.CreateSaturating(x + 1);
    }
}
