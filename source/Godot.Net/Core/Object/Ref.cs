namespace Godot.Net.Core.Object;

using System.Diagnostics;
using System.Runtime.CompilerServices;

[DebuggerDisplay("{Value}")]
public record Ref<T>(T? Value = default)
{
    public T? Value { get; set; } = Value;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator Ref<T>(T value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe implicit operator T?(Ref<T> value) => value.Value;
}
