namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;

public partial class RendererSceneCull
{
    public record Camera
    {
        public Guid               Attributes    { get; set; }
        public Guid               Env           { get; set; }
        public RealT              Fov           { get; set; }
        public Vector2<RealT>     Offset        { get; set; }
        public RealT              Size          { get; set; }
        public Transform3D<RealT> Transform     { get; set; }
        public CameraType         Type          { get; set; }
        public bool               VAspect       { get; set; }
        public uint               VisibleLayers { get; set; }
        public RealT              ZFar          { get; set; }
        public RealT              ZNear         { get; set; }
    }
}
