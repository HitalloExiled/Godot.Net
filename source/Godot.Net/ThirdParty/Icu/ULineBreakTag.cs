namespace Godot.Net.ThirdParty.Icu;

/// <summary>
/// Enum constants for the line break tags returned by getRuleStatus().
/// A range of values is defined for each category of word,
/// to allow for further subdivisions of a category in future releases.
/// Applications should check for tag values falling within the range,
/// rather than for single individual values.
/// </summary>
/// <remarks>Stable ICU 2.8</remarks>
public enum ULineBreakTag
{
    /// <summary>
    /// Tag value for soft line breaks, positions at which a line break
    /// is acceptable but not required.
    /// </summary>
    /// <remarks>Stable ICU 2.8</remarks>
    UBRK_LINE_SOFT = 0,

    /// <summary>
    /// Upper bound for soft line breaks.
    /// </summary>
    /// <remarks>Stable ICU 2.8</remarks>
    UBRK_LINE_SOFT_LIMIT = 100,

    /// <summary>
    /// Tag value for a hard, or mandatory line break.
    /// </summary>
    /// <remarks>Stable ICU 2.8</remarks>
    UBRK_LINE_HARD = UBRK_LINE_SOFT_LIMIT,

    /// <summary>
    /// Upper bound for hard line breaks.
    /// </summary>
    /// <remarks>Stable ICU 2.8</remarks>
    UBRK_LINE_HARD_LIMIT = 200
}
