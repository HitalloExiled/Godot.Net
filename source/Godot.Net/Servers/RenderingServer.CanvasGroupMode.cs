namespace Godot.Net.Servers;

public abstract partial class RenderingServer
{
    public enum CanvasGroupMode
    {
        CANVAS_GROUP_MODE_DISABLED,
        CANVAS_GROUP_MODE_CLIP_ONLY,
        CANVAS_GROUP_MODE_CLIP_AND_DRAW,
        CANVAS_GROUP_MODE_TRANSPARENT,
    };
}
