namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core;
using Godot.Net.Core.Math;

public partial class RendererCanvasCull
{
    public partial record Item
    {
        public record VisibilityNotifierData
        {
            public Rect2<RealT>                     Area            { get; set; }
            public Action?                          EnterCallable   { get; set; }
            public Action?                          ExitCallable    { get; set; }
            public bool                             JustVisible     { get; set; }
            public ulong                            VisibleInFrame  { get; set; }
            public SelfList<VisibilityNotifierData> VisibleElement  { get; set; }

            public VisibilityNotifierData() => this.VisibleElement = new(this);
        }
    }
}
