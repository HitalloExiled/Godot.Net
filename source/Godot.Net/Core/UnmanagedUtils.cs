namespace Godot.Net.Core;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Godot.Net.Extensions;

public static class UnmanagedUtils
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe string BytesPrToString(byte* values, int size, Encoding encoding)
    {
        var buffer = new byte[size];

        for (var i = 0; i < buffer.Length; i++)
        {
            buffer[i] = values[i];
        }

        return encoding.GetString(buffer).TrimEnd('\0');
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe void Copy<T>(T* source, T[] destination, int length) where T : unmanaged
    {
        for (var i = 0; i < length; i++)
        {
            destination[i] = source[i];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe void Copy<T>(nint source, T[] destination, int length) where T : unmanaged =>
        Copy((T*)source, destination, length);


    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe void Copy<T>(T[] source, T* destination, int length) where T : unmanaged
    {
        for (var i = 0; i < length; i++)
        {
            destination[i] = source[i];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe void Copy<T>(T[] source, nint destination, int length) where T : unmanaged =>
        Copy(source, (T*)destination, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe string BytesPrToString(byte* values, int size = 255) =>
        BytesPrToString(values, size, Encoding.UTF8);

    public static unsafe T[] PointerToArray<T>(T* source, int length) where T : unmanaged
    {
        var result = new T[length];

        Copy(source, result, length);

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe T[] PointerToArray<T>(nint source, int length) where T : unmanaged =>
        PointerToArray((T*)source, length);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe void StringToBytesPr(IEnumerable<string?> values, byte** pEnabledExtensionNamesPointer, Encoding encoding) // FIXME
    {
        var i = 0;

        foreach (var value in values)
        {
            if (value != null)
            {
                fixed (byte* pValue = value.ToBytes(encoding))
                {
                    pEnabledExtensionNamesPointer[i++] = pValue;
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe void StringToBytesPr(IEnumerable<string> values, byte** pEnabledExtensionNamesPointer) =>
        StringToBytesPr(values, pEnabledExtensionNamesPointer, Encoding.UTF8);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe void ZeroFill(nint pointer, int size)
    {
        for (var i = 0; i < size; i++)
        {
            Marshal.WriteByte(pointer + i, 0);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe void ZeroFill<T>(T* pointer, int size) where T : unmanaged =>
        ZeroFill((nint)pointer, size);
}
