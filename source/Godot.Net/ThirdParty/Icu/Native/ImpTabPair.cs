namespace Godot.Net.ThirdParty.Icu.Native;

using System.Runtime.InteropServices;

/// <summary>
/// FOOD FOR THOUGHT: each ImpTab should have its associated ImpAct,
/// instead of having a pair of ImpTab and a pair of ImpAct.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ImpTabPair
{
    public nint pImpTab; // fixed 2;
    public nint pImpAct; // fixed 2;
}
