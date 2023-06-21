namespace Godot.Net.Extensions;

using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

public static class StringExtensions
{
    public static long HexToInt(this string value)
    {
        var len = value.Length;

        if (len == 0)
        {
            return 0;
        }

        var s = value;

        var sign = s[0] == '-' ? -1L : 1L;

        if (sign < 0)
        {
            s = s[1..];
        }

        if (len > 2 && s[0] == '0' && char.ToLower(s[1]) == 'x')
        {
            s = s[2..];
        }

        var hex = 0L;

        foreach (var uc in s)
        {
            var c = char.ToLower(uc);
            var n = 0L;

            if (char.IsDigit(c))
            {
                n = c - '0';
            }
            else if (c is >= 'a' and <= 'f')
            {
                n = c - 'a' + 10;
            }
            else
            {
                if (ERR_FAIL_COND_V_MSG(true, $"Invalid hexadecimal notation character \"{c}\" in string \"{value}\"."))
                {
                    return 0;
                }
            }
            // Check for overflow/underflow, with special case to ensure INT64_MIN does not result in error
            var overflow = hex > long.MaxValue / 16 && (sign == 1 || sign == -1 && hex != (long.MaxValue >> 4) + 1) || sign == -1 && hex == (long.MaxValue >> 4) + 1 && c > '0';
            if (ERR_FAIL_COND_V_MSG(overflow, $"Cannot represent {value} as a 64-bit signed integer, since the value is {(sign == 1 ? "too large." : "too small.")}"))
            {
                return sign == 1 ? long.MaxValue : long.MinValue;
            }
            hex *= 16;
            hex += n;
        }

        return hex * sign;
    }

    public static bool IsValidHexNumber(this string value, bool withPrefix)
    {
        var len = value.Length;

        if (len == 0)
        {
            return false;
        }

        var from = 0;

        if (len != 1 && (value[0] == '+' || value[0] == '-'))
        {
            from++;
        }

        if (withPrefix)
        {
            if (len < 3)
            {
                return false;
            }

            if (value[from] != '0' || value[from + 1] != 'x')
            {
                return false;
            }
            from += 2;
        }

        for (var i = from; i < len; i++)
        {
            var c = value[i];

            if (char.IsAsciiHexDigit(c))
            {
                continue;
            }

            return false;
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static string Left(this string value, int len)
    {
        if (len < 0)
        {
            len = value.Length + len;
        }

        return len <= 0 ? "" : len >= value.Length ? value : value[..len];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static string ReplaceFirst(this string text, string search, string replace)
    {
        var pos = text.IndexOf(search);

        return pos < 0 ? text : string.Concat(text.AsSpan(0, pos), replace, text.AsSpan(pos + search.Length));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static byte[] ToBytes(this string source) => Encoding.UTF8.GetBytes(source);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static byte[] ToBytes(this string source, Encoding encoding) => encoding.GetBytes(source);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static string ToSHA256(this string source) => ToSHA256(source, Encoding.UTF8);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static string ToSHA256(this string source, Encoding encoding)
    {
        var builder = new StringBuilder();

        foreach (var @byte in SHA256.HashData(encoding.GetBytes(source)))
        {
            builder.Append(@byte.ToString("x2"));
        }

        return builder.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static string ToASCII(this string source) =>
        Encoding.ASCII.GetString(Encoding.Default.GetBytes(source));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static string ToUTF8(this string source) =>
        Encoding.UTF8.GetString(Encoding.Default.GetBytes(source));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static string ToUTF16(this string source) =>
        Encoding.Unicode.GetString(Encoding.Unicode.GetBytes(source));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static byte[] ToUTF16Buffer(this string source) =>
        Encoding.Unicode.GetBytes(source);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static byte[] ToUTF8Buffer(this string source) =>
        Encoding.Default.GetBytes(source);
}
