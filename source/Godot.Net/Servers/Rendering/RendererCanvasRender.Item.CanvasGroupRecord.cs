namespace Godot.Net.Servers.Rendering;

public abstract partial class RendererCanvasRender
{
    public partial record Item
    {
        public record CanvasGroupRecord
        {
            public bool               BlurMipmaps { get; set; }
            public float              ClearMargin { get; set; }
            public bool               FitEmpty    { get; set; }
            public float              FitMargin   { get; set; }
            public RS.CanvasGroupMode Mode        { get; set; }
        };
    }
}
