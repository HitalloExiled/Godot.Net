namespace Godot.Net.Servers.Rendering;
public abstract partial class RendererCanvasRender
{
    [Flags]
    public enum CanvasRectFlags : ushort
    {
        CANVAS_RECT_REGION    = 1,
        CANVAS_RECT_TILE      = 2,
        CANVAS_RECT_FLIP_H    = 4,
        CANVAS_RECT_FLIP_V    = 8,
        CANVAS_RECT_TRANSPOSE = 16,
        CANVAS_RECT_CLIP_UV   = 32,
        CANVAS_RECT_IS_GROUP  = 64,
        CANVAS_RECT_MSDF      = 128,
        CANVAS_RECT_LCD       = 256,
    };
}
