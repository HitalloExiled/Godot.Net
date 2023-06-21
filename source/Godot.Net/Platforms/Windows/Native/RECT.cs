namespace Godot.Net.Platforms.Windows.Native;

/// <summary>
/// The RECT structure defines the coordinates of the upper-left and lower-right corners of a rectangle.
/// </summary>
/// <remarks>
/// By convention, the right and bottom edges of the rectangle are normally considered exclusive.
/// In other words, the pixel whose coordinates are ( right, bottom ) lies immediately outside of the rectangle.
/// For example, when RECT is passed to the FillRect function, the rectangle is filled up to, but not including,
/// the right column and bottom row of pixels. This structure is identical to the RECTL structure.
/// </remarks>
public struct RECT
{
    /// <summary>
    /// The x-coordinate of the upper-left corner of the rectangle.
    /// </summary>
    public int left;

    /// <summary>
    /// The y-coordinate of the upper-left corner of the rectangle.
    /// </summary>
    public int top;

    /// <summary>
    /// The x-coordinate of the lower-right corner of the rectangle.
    /// </summary>
    public int right;

    /// <summary>
    /// The y-coordinate of the lower-right corner of the rectangle.
    /// </summary>
    public int bottom;

    public override readonly bool Equals(object? obj) =>
        obj is RECT rect
        && this.left   == rect.left
        && this.top    == rect.top
        && this.right  == rect.right
        && this.bottom == rect.bottom;

    public override readonly int GetHashCode() =>
        this.left.GetHashCode()
        ^ this.top.GetHashCode()
        ^ this.right.GetHashCode()
        ^ this.bottom.GetHashCode();

    public static bool operator ==(RECT left, RECT right) =>
        left.Equals(right);

    public static bool operator !=(RECT left, RECT right) =>
        !left.Equals(right);
}
