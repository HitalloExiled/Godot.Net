namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;

public abstract partial class RendererCanvasRender
{
    public partial record Item
    {
        public record CommandPrimitive : Command
        {
            public Color[]          Colors     { get; } = new Color[4];
            public int              PointCount { get; set; }
            public Vector2<RealT>[] Points     { get; } = new Vector2<RealT>[4];
            public Guid             Texture    { get; set; }
            public Vector2<RealT>[] Uvs        { get; } = new Vector2<RealT>[4];

            public CommandPrimitive() => this.Type = CommandType.TYPE_PRIMITIVE;
        }
    }
}
