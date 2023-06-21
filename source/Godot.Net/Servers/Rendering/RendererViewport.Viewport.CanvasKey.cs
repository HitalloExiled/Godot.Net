namespace Godot.Net.Servers.Rendering;
public partial class RendererViewport
{
    public partial record Viewport
    {
        public record CanvasKey
        {
            public Guid Canvas   { get; set; }
            public long Stacking { get; set; }
            public int  Layer => (int)(this.Stacking >> 32);

            public CanvasKey(Guid canvas, int layer, int sublayer)
            {
                this.Canvas = canvas;

                var sign      = layer < 0 ? -1 : 1;
                this.Stacking = sign * (((long)Math.Abs(layer)) << 32) + sublayer;
            }

            public static bool operator <(CanvasKey left, CanvasKey right) =>
                left.Stacking == right.Stacking ? left.Canvas < right.Canvas : left.Stacking < right.Stacking;

            public static bool operator >(CanvasKey left, CanvasKey right) =>
                left.Stacking == right.Stacking ? left.Canvas > right.Canvas : left.Stacking > right.Stacking;
        }
    }
}
