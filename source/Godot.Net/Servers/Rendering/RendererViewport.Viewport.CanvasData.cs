namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;

public partial class RendererViewport
{
    public partial record Viewport
    {
        public record CanvasData
        {
            public CanvasBase?        Canvas    { get; set; }
            public int                Layer     { get; set; }
            public int                Sublayer  { get; set; }
            public Transform2D<RealT> Transform { get; set; } = new();
        };
    }
}
