namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;

public abstract partial class RendererCanvasRender
{
    public partial record Item
    {
        public record CommandNinePatch : Command
        {
            public RS.NinePatchAxisMode AxisX      { get; set; }
            public RS.NinePatchAxisMode AxisY      { get; set; }
            public Color                Color      { get; set; }
            public bool                 DrawCenter { get; set; }
            public float[]              Margin     { get; } = new float[4];
            public Rect2<RealT>         Rect       { get; set; }
            public Rect2<RealT>         Source     { get; set; }
            public Guid                 Texture    { get; set; }

            public CommandNinePatch()
            {
                this.DrawCenter = true;
                this.Type       = CommandType.TYPE_NINEPATCH;
            }
        }
    }
}
