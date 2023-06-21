namespace Godot.Net.Servers.Rendering;

public abstract partial class RendererCanvasRender
{
    public partial record Item
    {
        public record CommandPolygon : Command
        {
            public Polygon Polygon { get; } = new();

            public RS.PrimitiveType Primitive { get; set; }
            public Guid             Texture   { get; set; }

            public CommandPolygon() => this.Type = CommandType.TYPE_POLYGON;
        }
    }
}
