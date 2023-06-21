namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;

public abstract partial class RendererCanvasRender
{
    public partial record Item
    {
        public record CopyBackBufferRecord
        {
            public bool         Full       { get; set; }
            public Rect2<RealT> Rect       { get; set; }
            public Rect2<RealT> ScreenRect { get; set; }
        }
    }
}
