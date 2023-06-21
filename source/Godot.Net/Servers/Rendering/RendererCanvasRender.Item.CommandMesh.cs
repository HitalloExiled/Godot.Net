namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;

public abstract partial class RendererCanvasRender
{
    public partial record Item
    {
        public record CommandMesh : Command
        {
            public Guid               Mesh         { get; set; }
            public Guid               MeshInstance { get; set; }
            public Color              Modulate     { get; set; }
            public Guid               Texture      { get; set; }
            public Transform2D<RealT> Transform    { get; set; } = new();

            public CommandMesh() => this.Type = CommandType.TYPE_MESH;
        };
    }
}
