namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;

public abstract partial class RendererCanvasRender
{
    public record LightOccluderInstance
    {
        public Rect2<RealT>                     AabbCache     { get; set; }
        public Guid                             Canvas        { get; set; }
        public RS.CanvasOccluderPolygonCullMode CullCache     { get; set; }
        public bool                             Enabled       { get; set; }
        public int                              LightMask     { get; set; }
        public LightOccluderInstance?           Next          { get; set; }
        public Guid                             Occluder      { get; set; }
        public Guid                             Polygon       { get; set; }
        public bool                             SdfCollision  { get; set; }
        public Transform2D<RealT>               Xform         { get; set; } = new();
        public Transform2D<RealT>               XformCache    { get; set; } = new();

        public LightOccluderInstance()
        {
            this.Enabled      = true;
            this.SdfCollision = false;
            this.Next         = null;
            this.LightMask    = 1;
            this.CullCache    = RS.CanvasOccluderPolygonCullMode.CANVAS_OCCLUDER_POLYGON_CULL_DISABLED;
        }
    }
}
