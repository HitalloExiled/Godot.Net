namespace Godot.Net.Servers.Rendering;
public abstract partial class RendererCanvasRender
{
    public partial record Item
    {
        public record CommandAnimationSlice : Command
        {
            public double AnimationLength { get; set; }
            public double Offset          { get; set; }
            public double SliceBegin      { get; set; }
            public double SliceEnd        { get; set; }

            public CommandAnimationSlice() => this.Type = CommandType.TYPE_ANIMATION_SLICE;
        };
    }
}
