namespace Godot.Net.Extensions;

using System.Runtime.CompilerServices;
using System.Text;

public static class ByteExtension
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static string ConvertToString(this byte[] source) => Encoding.UTF8.GetString(source);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static string ConvertToString(this byte[] source, Encoding encoding) => encoding.GetString(source);
}
