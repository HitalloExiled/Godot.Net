namespace Godot.Net.Servers.Rendering;
public abstract partial class RendererCanvasRender
{
    public partial record Item
    {
        public record CommandParticles : Command
        {
            public Guid Particles { get; set; }
            public Guid Texture   { get; set; }

            public CommandParticles() => this.Type = CommandType.TYPE_PARTICLES;
        };
    }
}
