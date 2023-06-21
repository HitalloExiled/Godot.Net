namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;

public abstract partial class RendererCanvasRender
{
    public partial record Item
    {
        public record ViewportRenderRecord
        {
            public RS?          Owner { get; set; }
            public Rect2<RealT> Rect  { get; set; }
            public object?      Udata { get; set; }
        }
    }
}
