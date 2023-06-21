namespace Godot.Net.Core;

using System.Collections;
using System.Runtime.CompilerServices;

public class GuidOwner<T> : IEnumerable<T> where T : new()
{
    private static readonly object padlock = new();

    private readonly Dictionary<Guid, T> entries = new();
    private readonly bool                threadSafe;

    public T this[Guid key] => this.entries[key];

    public int Count => this.entries.Count;

    public GuidOwner(bool threadSafe = false) => this.threadSafe = threadSafe;

    IEnumerator IEnumerable.GetEnumerator() => this.entries.Values.GetEnumerator();

    public Guid Add(T value)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        Guid action()
        {
            var id = Guid.NewGuid();

            this.entries.Add(id, value);

            return id;
        }

        if (this.threadSafe)
        {
            lock (padlock)
            {
                return action();
            }
        }
        else
        {
            return action();
        }
    }

    public Guid Initialize(T? value = default)
    {
        var id = Guid.NewGuid();

        this.Initialize(id, value);

        return id;
    }

    public T Initialize(Guid id, T? value = default)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        T action()
        {
            var instance = value ?? new T();

            this.entries.Add(id, instance);

            return instance;
        }

        if (this.threadSafe)
        {
            lock (padlock)
            {
                return action();
            }
        }
        else
        {
            return action();
        }
    }

    public IEnumerator<T> GetEnumerator() => this.entries.Values.GetEnumerator();

    public T? GetOrNull(Guid id)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        T? action() => this.entries.TryGetValue(id, out var value) ? value : default;

        if (this.threadSafe)
        {
            lock (padlock)
            {
                return action();
            }
        }
        else
        {
            return action();
        }
    }

    public bool Owns(Guid id)
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        bool action() => this.entries.ContainsKey(id);

        if (this.threadSafe)
        {
            lock (padlock)
            {
                return action();
            }
        }
        else
        {
            return action();
        }
    }
}
