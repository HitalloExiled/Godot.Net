namespace Godot.Net.ThirdParty.Icu.Native;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct Run
{
    /// <summary>first character of the run; b31 indicates even/odd level</summary>
    public int logicalStart;

    /// <summary>last visual position of the run +1</summary>
    public int visualLimit;

    /// <summary>if >0, flags for inserting LRM/RLM before/after run, if <0, count of bidi controls within run</summary>
    public int insertRemove;
}
