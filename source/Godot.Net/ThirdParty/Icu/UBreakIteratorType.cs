namespace Godot.Net.ThirdParty.Icu;

/// <summary>
/// UBreakIteratorType represents the type of break iterator.
/// </summary>
/// <remarks> Stable ICU 2.0</remarks>
public enum UBreakIteratorType
{
    UBRK_DONE = -1,

    /// <summary>
    /// Character breaks.
    /// </summary>
    /// <remarks> Stable ICU 2.0</remarks>
    UBRK_CHARACTER = 0,

    /// <summary>
    /// Word breaks.
    /// </summary>
    /// <remarks> Stable ICU 2.0</remarks>
    UBRK_WORD = 1,

    /// <summary>
    /// Line breaks.
    /// </summary>
    /// <remarks> Stable ICU 2.0</remarks>
    UBRK_LINE = 2,

    /// <summary>
    /// Sentence breaks.
    /// </summary>
    /// <remarks> Stable ICU 2.0</remarks>
    UBRK_SENTENCE = 3,

    #if !U_HIDE_DEPRECATED_API
    /// <summary>
    /// Title Case breaks.
    /// The iterator created using this type locates title boundaries as described for
    /// Unicode 3.2 only. For Unicode 4.0 and above title boundary iteration,
    /// please use Word Boundary iterator.
    /// </summary>
    /// <deprecated>ICU 2.8 Use the word break iterator for titlecasing for Unicode 4 and later.</deprecated>
    UBRK_TITLE = 4,

    /// <summary>
    /// One more than the highest normal UBreakIteratorType value.
    /// </summary>
    /// <deprecated>ICU 58 The numeric value may change over time, see ICU ticket #12420.</deprecated>
    UBRK_COUNT = 5,
    #endif  // U_HIDE_DEPRECATED_API
}
