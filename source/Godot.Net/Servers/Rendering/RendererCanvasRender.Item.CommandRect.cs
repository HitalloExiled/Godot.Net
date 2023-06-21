namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;

public abstract partial class RendererCanvasRender
{
    public partial record Item
    {
        public record CommandRect : Command
        {
            public CanvasRectFlags Flags     { get; set; }
            public Color           Modulate  { get; set; }
            public float           Outline   { get; set; }
            public float           PxRange   { get; set; }
            public Rect2<RealT>    Rect      { get; set; }
            public Rect2<RealT>    Source    { get; set; }
            public Guid            Texture   { get; set; }

            public CommandRect()
            {
                this.Flags   = 0;
                this.Outline = 0;
                this.PxRange = 1;
                this.Type    = CommandType.TYPE_RECT;
            }
        }
    }
}
