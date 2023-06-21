namespace Godot.Net.Core.Object;

using Godot.Net.Scene.Main;

public class GodotObject
{
    public event Action? PropertyListChanged;

    public int InstanceId { get; }

    public GodotObject() => this.InstanceId = InstanceTracker.GetId(this);

    public void NotifyPropertyListChanged() => PropertyListChanged?.Invoke();

    public virtual void Notification(NotificationKind notification, bool reversed = false)
    {
        //
    }
}
