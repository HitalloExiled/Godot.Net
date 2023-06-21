namespace Godot.Net.Core;

using System.Runtime.CompilerServices;

public static class InstanceTracker
{
    private record Id(int Value);

    private static readonly ConditionalWeakTable<object, Id> table = new();

    private static int lastId;

    public static int GetId(object instance)
    {
        if (table.TryGetValue(instance, out var id))
        {
            return id.Value;
        }

        Interlocked.Increment(ref lastId);

        table.Add(instance, new(lastId));

        return lastId;
    }

    public static bool HasId(object instance) => table.TryGetValue(instance, out var _);
}
