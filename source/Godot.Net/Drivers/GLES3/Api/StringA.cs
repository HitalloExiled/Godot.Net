namespace Godot.Net.Drivers.GLES3.Api;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[DebuggerDisplay("{Value}")]
public readonly record struct StringA(nint Value = default) : IDisposable
{
    public StringA(string? value) : this(Marshal.StringToHGlobalAnsi(value))
    { }

    public void Dispose() => Marshal.FreeHGlobal(this.Value);

    public override string? ToString() => (string?)this;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator char*(StringA value) => (char*)value.Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator StringA(char* value) => new((nint)value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator string?(StringA value) => Marshal.PtrToStringAnsi(value.Value);
}
