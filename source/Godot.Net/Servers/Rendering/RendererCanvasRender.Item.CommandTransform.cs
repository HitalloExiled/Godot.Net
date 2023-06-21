namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;

public abstract partial class RendererCanvasRender
{
    public partial record Item
    {
        public record CommandTransform : Command
        {
            public Transform2D<RealT> Xform { get; set; } = new();

            public CommandTransform() => this.Type = CommandType.TYPE_TRANSFORM;
        };
    }
}
