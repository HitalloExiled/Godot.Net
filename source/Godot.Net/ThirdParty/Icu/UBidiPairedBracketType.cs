namespace Godot.Net.ThirdParty.Icu;

public enum UBidiPairedBracketType
{
    /// <summary>
    /// Not a paired bracket. @stable ICU 52
    /// </summary>
    NONE,

    /// <summary>
    /// Open paired bracket. @stable ICU 52
    /// </summary>
    OPEN,

    /// <summary>
    /// Close paired bracket. @stable ICU 52
    /// </summary>
    CLOSE,

    [Obsolete("ICU 58 The numeric value may change over time")]
    COUNT /* 3 */
}
