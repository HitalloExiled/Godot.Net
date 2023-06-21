namespace Godot.Net.Servers.Rendering;

public abstract partial class RendererCanvasRender
{
    public partial record Item
    {
        public record Command
        {
            public Command?    Next { get; set; }
            public CommandType Type { get; set; }
        }
    }
}
