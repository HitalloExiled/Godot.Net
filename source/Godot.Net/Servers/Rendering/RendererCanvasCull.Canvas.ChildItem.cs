namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;

public partial class RendererCanvasCull
{
    public partial record Canvas : RendererViewport.CanvasBase
    {
        public record ChildItem
        {
            public Item?          Item   { get; set; }
            public Vector2<RealT> Mirror { get; set; }

            public static bool operator <(ChildItem left, ChildItem right) => left.Item?.Index < right.Item?.Index;
            public static bool operator >(ChildItem left, ChildItem right) => left.Item?.Index > right.Item?.Index;
        };
    }
}
