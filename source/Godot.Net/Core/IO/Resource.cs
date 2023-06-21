#define TOOLS_ENABLED

namespace Godot.Net.Core.IO;

using Godot.Net.Core.Object;
using Godot.Net.Extensions;

#pragma warning disable IDE0052, IDE0051 // TODO Remove

public partial class Resource : GodotObject
{
    public event Action? Changed;

    private readonly SelfList<Resource> remappedList;

    private string? pathCache;

    public string? Path { get; }

    public Resource() => this.remappedList = new(this);


    protected virtual void OnResourcePathChanged() { }

    public virtual Resource Duplicate(bool subresources = false) => this.Clone(subresources);

    public void EmitChanged() => Changed?.Invoke();

    public void SetPath(string path, bool takeOver = false)
    {
        if (this.pathCache == path)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(path))
        {
            takeOver = false; // Can't take over an empty path
        }

        ResourceCache.Lock.WaitOne();

        if (!string.IsNullOrEmpty(this.pathCache))
        {
            ResourceCache.Resources.Remove(this.pathCache);
        }

        this.pathCache = "";

        var existing = ResourceCache.GetRef(path);

        if (existing != null)
        {
            if (takeOver)
            {
                existing.pathCache = null;
                ResourceCache.Resources.Remove(path);
            }
            else
            {
                ResourceCache.Lock.ReleaseMutex();

                if (ERR_FAIL_MSG("Another resource is loaded from path '" + path + "' (possible cyclic resource inclusion)."))
                {
                    return;
                }
            }
        }

        this.pathCache = path;

        if (!string.IsNullOrEmpty(this.pathCache))
        {
            ResourceCache.Resources[this.pathCache] = this;
        }

        ResourceCache.Lock.ReleaseMutex();

        this.OnResourcePathChanged();
    }
}
