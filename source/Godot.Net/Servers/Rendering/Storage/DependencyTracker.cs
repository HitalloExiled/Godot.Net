namespace Godot.Net.Servers.Rendering.Storage;

#pragma warning disable IDE0052, CA1822, CS0414 // TODO REMOVE

public class DependencyTracker
{
    public delegate void ChangedCallbackFn(Dependency.DependencyChangedNotification notification, DependencyTracker dependencyTracker);
    public delegate void DeletedCallbackFn(Guid id, DependencyTracker dependencyTracker);

    private uint instanceVersion;

    public HashSet<Dependency> Dependencies { get; } = new();

    public ChangedCallbackFn?  ChangedCallback { get; set; }
    public DeletedCallbackFn?  DeletedCallback { get; set; }
    public object?             Userdata        { get; set; }

    ~DependencyTracker() => this.Clear();

    public void Clear()
    {
        // clear all dependencies
        foreach (var dep in this.Dependencies)
        {
            dep.Instances.Remove(this);
        }

        this.Dependencies.Clear();
    }

    public void UpdateBegin() => this.instanceVersion++; // call before updating dependencies

    public void UpdateDependency(Dependency dependency)
    {
        //called internally, can't be used directly, use update functions in Storage
        this.Dependencies.Add(dependency);

        if (!dependency.Instances.TryAdd(this, this.instanceVersion))
        {
            dependency.Instances[this] = this.instanceVersion;
        }
    }

    public void UpdateEnd()
    {
        //call after updating dependencies
        var toCleanUp = new List<KeyValuePair<Dependency, DependencyTracker>>();

        foreach (var dep in this.Dependencies)
        {
            dep.Instances.TryGetValue(this, out var f);

            if (ERR_CONTINUE(f == default))
            {
                continue;
            }

            if (f != this.instanceVersion)
            {
                var p = new KeyValuePair<Dependency, DependencyTracker>(dep, this);

                toCleanUp.Add(p);
            }
        }

        while (toCleanUp.Count != 0)
        {
            toCleanUp.First().Key.Instances.Remove(toCleanUp.First().Value);

            this.Dependencies.Remove(toCleanUp.First().Key);

            toCleanUp.RemoveAt(0);
        }
    }
}
