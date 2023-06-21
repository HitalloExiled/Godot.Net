namespace Godot.Net.Servers.XR;

using Godot.Net.Core.Math;
using Godot.Net.Servers.Rendering;

public abstract class XRInterface
{
    public XRInterface() => throw new NotImplementedException();

    public abstract Vector2<RealT> RenderTargetSize { get; }
    public abstract uint           ViewCount        { get; }

    public virtual Guid ColorTexture    { get; }
    public virtual Guid DepthTexture    { get; }
    public virtual Guid VelocityTexture { get; }

    public virtual bool PreDrawViewport(Guid renderTargetId) => true;
    public abstract IList<BlitToScreen> PostDrawViewport(Guid renderTargetId, in Rect2<RealT> screenRect);
}
