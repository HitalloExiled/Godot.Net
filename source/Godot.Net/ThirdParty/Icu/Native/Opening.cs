namespace Godot.Net.ThirdParty.Icu.Native;

using System.Runtime.InteropServices;
using Godot.Net.ThirdParty.Icu;

/// <summary>
/// Represents an opening bracket in a text.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Opening
{
    /// <summary>
    /// Position of the opening bracket.
    /// </summary>
    public int position;

    /// <summary>
    /// Matching char or -position of the closing bracket.
    /// </summary>
    public int match;

    /// <summary>
    /// Position of the last strong character found before the opening.
    /// </summary>
    public int contextPos;

    /// <summary>
    /// Bits for L or R/AL found within the pair.
    /// </summary>
    public ushort flags;

    /// <summary>
    /// Direction (L or R) according to the last strong character before the opening.
    /// </summary>
    public UBiDiDirection contextDir;

    /// <summary>
    /// To complete a nice multiple of 4 bytes.
    /// </summary>
    public byte filler;
}
