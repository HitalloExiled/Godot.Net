namespace Godot.Net.ThirdParty.Icu.Native;

using System.Runtime.InteropServices;
using Godot.Net.ThirdParty.Icu;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct InsertPoints
{
    /// <summary>number of points allocated</summary>
    public int capacity;

    /// <summary>number of points used</summary>
    public int size;

    /// <summary>number of points confirmed</summary>
    public int confirmed;

    /// <summary>for eventual memory shortage</summary>

    public UErrorCode errorCode;

    /// <summary>pointer to array of points</summary>
    public Point *points;
}
