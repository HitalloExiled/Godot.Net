namespace Godot.Net.Extensions;
using System.Globalization;
using System.Runtime.CompilerServices;

public static class CharExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool IsAsciiIdentifierChar(this char character) =>
        char.IsAsciiLetterOrDigit(character) || character == '_';

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool IsLinebreak(this char character) =>
	    character >= 0x000a && character <= 0x000d || character == 0x0085 || character == 0x2028 || character == 0x2029;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool IsUnderscore(this char character) =>
        character == '_';

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool IsUnicodeIdentifierStart(this char character) =>
        char.GetUnicodeCategory(character) is
            UnicodeCategory.UppercaseLetter or
            UnicodeCategory.LowercaseLetter or
            UnicodeCategory.TitlecaseLetter or
            UnicodeCategory.ModifierLetter  or
            UnicodeCategory.OtherLetter     or
            UnicodeCategory.LetterNumber;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool IsUnicodeIdentifierContinue(this char character) =>
        CharUnicodeInfo.GetUnicodeCategory(character) is
            UnicodeCategory.UppercaseLetter      or
            UnicodeCategory.LowercaseLetter      or
            UnicodeCategory.TitlecaseLetter      or
            UnicodeCategory.ModifierLetter       or
            UnicodeCategory.OtherLetter          or
            UnicodeCategory.LetterNumber         or
            UnicodeCategory.NonSpacingMark       or
            UnicodeCategory.SpacingCombiningMark or
            UnicodeCategory.DecimalDigitNumber   or
            UnicodeCategory.ConnectorPunctuation;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool IsWhiteSpace(this char c) =>
        char.IsWhiteSpace(c);
}

