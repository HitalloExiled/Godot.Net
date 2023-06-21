namespace Godot.Net.ThirdParty.Icu.Native;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct Isolate
{
    public int startON;
    public int start1;
    public int state;
    public short stateImp;
}
