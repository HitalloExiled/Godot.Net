namespace Godot.Net.Drivers.GLES3.Api;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[DebuggerDisplay("{Value}")]
public readonly record struct StringW(nint Value = default) : IDisposable
{
    public StringW(string? value) : this(Marshal.StringToHGlobalUni(value))
    { }

    public void Dispose() => Marshal.FreeHGlobal(this.Value);

    public override string? ToString() => (string?)this;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator char*(StringW value) => (char*)value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator StringW(char* value) => new(new nint(value));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static implicit operator string?(StringW value) => Marshal.PtrToStringUni(value.Value);
}
