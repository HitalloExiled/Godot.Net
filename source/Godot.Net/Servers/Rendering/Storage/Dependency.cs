namespace Godot.Net.Servers.Rendering.Storage;

public partial class Dependency
{
    public Dictionary<DependencyTracker, uint> Instances { get; } = new();

    public void ChangedNotify(DependencyChangedNotification notification)
    {
        foreach (var e in this.Instances)
        {
            e.Key.ChangedCallback?.Invoke(notification, e.Key);
        }
    }

    public void DeletedNotify(Guid rid)
    {
        foreach (var e in this.Instances)
        {
            e.Key.DeletedCallback?.Invoke(rid, e.Key);
        }

        foreach (var e in this.Instances)
        {
            e.Key.Dependencies.Remove(this);
        }

        this.Instances.Clear();
    }
}
