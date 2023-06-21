namespace Godot.Net.Servers.Rendering;
public abstract partial class RendererCanvasRender
{
    public partial record Item
    {
        public record CommandClipIgnore : Command
        {
            public bool Ignore { get; set; }

            public CommandClipIgnore() => this.Type = CommandType.TYPE_CLIP_IGNORE;
        };
    }
}
