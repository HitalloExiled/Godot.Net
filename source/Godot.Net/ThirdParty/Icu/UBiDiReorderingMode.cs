namespace Godot.Net.ThirdParty.Icu;

using System;

/// <summary>
/// UBiDiReorderingMode values indicate which variant of the Bidi algorithm to use.
/// </summary>
/// <remarks>See <see cref="UBidi.SetReorderingMode"/>. Stable ICU 3.6.</remarks>
public enum UBiDiReorderingMode
{
    /// <summary>
    /// Regular Logical to Visual Bidi algorithm according to Unicode.
    /// </summary>
    /// <remarks>This is a 0 value. Stable ICU 3.6.</remarks>
    UBIDI_REORDER_DEFAULT = 0,

    /// <summary>
    /// Logical to Visual algorithm which handles numbers in a way which mimics the behavior of Windows XP.
    /// </summary>
    /// <remarks>Stable ICU 3.6.</remarks>
    UBIDI_REORDER_NUMBERS_SPECIAL,

    /// <summary>
    /// Logical to Visual algorithm grouping numbers with adjacent R characters (reversible algorithm).
    /// </summary>
    /// <remarks>Stable ICU 3.6.</remarks>
    UBIDI_REORDER_GROUP_NUMBERS_WITH_R,

    /// <summary>
    /// Reorder runs only to transform a Logical LTR string to the Logical RTL string with the same display, or vice-versa.
    /// </summary>
    /// <remarks>
    /// If this mode is set together with option <see cref="UBidiOption.InsertMarks"/>,
    /// some Bidi controls in the source text may be removed and other controls may be added to produce the minimum combination which has the required display.
    /// Stable ICU 3.6.
    /// </remarks>
    UBIDI_REORDER_RUNS_ONLY,

    /// <summary>
    /// Visual to Logical algorithm which handles numbers like L (same algorithm as selected by <see cref="UBidi.SetInverse"/>).
    /// </summary>
    /// <remarks>See <see cref="UBidi.SetInverse"/>. Stable ICU 3.6.</remarks>
    UBIDI_REORDER_INVERSE_NUMBERS_AS_L,

    /// <summary>
    /// Visual to Logical algorithm equivalent to the regular Logical to Visual algorithm.
    /// </summary>
    /// <remarks>Stable ICU 3.6.</remarks>
    UBIDI_REORDER_INVERSE_LIKE_DIRECT,

    /// <summary>
    /// Inverse Bidi (Visual to Logical) algorithm for the UBIDI_REORDER_NUMBERS_SPECIAL Bidi algorithm.
    /// </summary>
    /// <remarks>Stable ICU 3.6.</remarks>
    UBIDI_REORDER_INVERSE_FOR_NUMBERS_SPECIAL,

    #if !U_HIDE_DEPRECATED_API
    /// <summary>
    /// Number of values for reordering mode.
    /// </summary>
    /// <remarks>Deprecated ICU 58. The numeric value may change over time, see ICU ticket #12420.</remarks>
    [Obsolete("Deprecated ICU 58. The numeric value may change over time, see ICU ticket #12420.")]
    UBIDI_REORDER_COUNT
    #endif // U_HIDE_DEPRECATED_API
}
