namespace Godot.Net.Servers;

public abstract partial class RenderingServer
{
    public enum CanvasOccluderPolygonCullMode
    {
        CANVAS_OCCLUDER_POLYGON_CULL_DISABLED,
        CANVAS_OCCLUDER_POLYGON_CULL_CLOCKWISE,
        CANVAS_OCCLUDER_POLYGON_CULL_COUNTER_CLOCKWISE,
    };
}
