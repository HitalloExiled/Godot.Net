namespace Godot.Net.Servers.Rendering;

public abstract partial class RendererCanvasRender
{
    public partial record Item
    {
        public enum CommandType
        {
            TYPE_RECT,
            TYPE_NINEPATCH,
            TYPE_POLYGON,
            TYPE_PRIMITIVE,
            TYPE_MESH,
            TYPE_MULTIMESH,
            TYPE_PARTICLES,
            TYPE_TRANSFORM,
            TYPE_CLIP_IGNORE,
            TYPE_ANIMATION_SLICE,
        }
    }
}
