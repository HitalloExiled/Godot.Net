namespace Godot.Net.ThirdParty.Icu.Native;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct Point
{
    /// <summary>position in text</summary>
    public int pos;

    /// <summary>flag for LRM/RLM, before/after</summary>
    public int flag;
}
