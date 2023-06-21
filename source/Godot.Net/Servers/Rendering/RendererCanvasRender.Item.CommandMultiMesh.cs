namespace Godot.Net.Servers.Rendering;
public abstract partial class RendererCanvasRender
{
    public partial record Item
    {
        public record CommandMultiMesh : Command
        {
            public Guid Multimesh { get; set; }
            public Guid Texture   { get; set; }

            public CommandMultiMesh() => this.Type = CommandType.TYPE_MULTIMESH;
        };
    }
}
