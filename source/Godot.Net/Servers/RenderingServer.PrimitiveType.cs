namespace Godot.Net.Servers;

public abstract partial class RenderingServer
{
    public enum PrimitiveType
    {
        PRIMITIVE_POINTS,
        PRIMITIVE_LINES,
        PRIMITIVE_LINE_STRIP,
        PRIMITIVE_TRIANGLES,
        PRIMITIVE_TRIANGLE_STRIP,
        PRIMITIVE_MAX,
    }
}
