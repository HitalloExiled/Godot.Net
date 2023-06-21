namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;

public partial class RendererCanvasCull
{
    public partial record Canvas : RendererViewport.CanvasBase
    {
        public List<ChildItem>                                     ChildItems         { get; } = new();
        public bool                                                ChildrenOrderDirty { get; set; }
        public HashSet<RendererCanvasRender.Light>                 DirectionalLights  { get; } = new();
        public HashSet<RendererCanvasRender.Light>                 Lights             { get; } = new();
        public Color                                               Modulate           { get; set; }
        public HashSet<RendererCanvasRender.LightOccluderInstance> Occluders          { get; } = new();
        public Guid                                                Parent             { get; set; }
        public float                                               ParentScale        { get; set; }
        public HashSet<Guid>                                       Viewports          { get; } = new();

        public Canvas()
        {
            this.Modulate           = new(1, 1, 1, 1);
            this.ChildrenOrderDirty = true;
            this.ParentScale        = 1.0f;
        }

        public void EraseItem(Item item)
        {
            var idx = this.FindItem(item);

            if (idx >= 0)
            {
                this.ChildItems.RemoveAt(idx);
            }
        }

        public int FindItem(Item item)
        {
            for (var i = 0; i < this.ChildItems.Count; i++)
            {
                if (this.ChildItems[i].Item == item)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
