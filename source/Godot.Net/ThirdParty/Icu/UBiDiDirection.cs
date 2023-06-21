namespace Godot.Net.ThirdParty.Icu;

/// <summary>
/// <code>UBiDiDirection</code> values indicate the text direction.
/// </summary>
/// <stable>ICU 2.0</stable>
public enum UBiDiDirection
{
    /// <summary>
    /// Left-to-right text. This is a 0 value.
    /// <para>
    /// As a return value for <code>Bidi.GetDirection()</code>, it means
    /// that the source string contains no right-to-left characters, or
    /// that the source string is empty and the paragraph level is even.
    /// </para>
    /// <para>
    /// As a return value for <code>Bidi.GetBaseDirection()</code>, it
    /// means that the first strong character of the source string has
    /// a left-to-right direction.
    /// </para>
    /// </summary>
    /// <stable>ICU 2.0</stable>
    UBIDI_LTR,

    /// <summary>
    /// Right-to-left text. This is a 1 value.
    /// <para>
    /// As a return value for <code>Bidi.GetDirection()</code>, it means
    /// that the source string contains no left-to-right characters, or
    /// that the source string is empty and the paragraph level is odd.
    /// </para>
    /// <para>
    /// As a return value for <code>Bidi.GetBaseDirection()</code>, it
    /// means that the first strong character of the source string has
    /// a right-to-left direction.
    /// </para>
    /// </summary>
    /// <stable>ICU 2.0</stable>
    UBIDI_RTL,

    /// <summary>
    /// Mixed-directional text.
    /// <para>As a return value for <code>Bidi.GetDirection()</code>, it means
    /// that the source string contains both left-to-right and
    /// right-to-left characters.
    /// </para>
    /// </summary>
    /// <stable>ICU 2.0</stable>
    UBIDI_MIXED,

    /// <summary>
    /// No strongly directional text.
    /// <para>
    /// As a return value for <code>Bidi.GetBaseDirection()</code>, it means
    /// that the source string is missing or empty, or contains neither left-to-right
    /// nor right-to-left characters.
    /// </para>
    /// </summary>
    /// <stable>ICU 4.6</stable>
    UBIDI_NEUTRAL
}
