namespace Godot.Net.Servers.Rendering.Storage;

using Godot.Net.Core.Math;

public abstract class RendererUtilities
{
    public abstract Vector2<int> MaximumViewportSize { get; }
    public abstract string       VideoAdapterName    { get; }

    public Color DefaultClearColor   { get; set; }
    public bool  CapturingTimestamps { get; set; }

    public abstract void CaptureTimestamp(string name);
    public abstract void CaptureTimestampsBegin(string? name = default);
    public abstract void UpdateDirtyResources();
    public abstract void VisibilityNotifierCall(Guid notifier, bool enter, bool deferred);
}

public static class Macros
{
    public static void RENDER_TIMESTAMP(string text)
    {
        if (RSG.Utilities.CapturingTimestamps)
        {
            RSG.Utilities.CaptureTimestamp(text);
        }
    }

    public static void TIMESTAMP_BEGIN(string? text = default)
    {
        if (RSG.Utilities.CapturingTimestamps)
        {
            RSG.Utilities.CaptureTimestampsBegin(text);
        }
    }
}
