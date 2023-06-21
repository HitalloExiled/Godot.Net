namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;

public abstract partial class RendererCanvasRender
{
    private static RendererCanvasRender? singleton;
    public static RendererCanvasRender Singleton => singleton ?? throw new NullReferenceException();

    public abstract double Time { get; set; }

    public RendererCanvasRender() => singleton = this;

    public abstract void CanvasRenderItems(
        Guid                       toRenderTarget,
        Item?                      itemList,
        in Color                   modulate,
        Light?                     lightList,
        Light?                     directionalLightList,
        in Transform2D<RealT>      canvasTransform,
        RS.CanvasItemTextureFilter defaultFilter,
        RS.CanvasItemTextureRepeat defaultRepeat,
        bool                       snap2DVerticesToPixel,
        out bool                   sdfUsed
    );

    public abstract bool Free(Guid id);
    public abstract void FreePolygon(ulong polygon);
    public abstract Guid LightCreate();
    public abstract void LightSetTexture(Guid id, Guid textureId);
    public abstract void LightSetUseShadow(Guid id, bool enable);
    public abstract void LightUpdateDirectionalShadow(Guid id, int shadowIndex, in Transform2D<RealT> lightXform, int lightMask, float cullDistance, in Rect2<RealT> clipRect, LightOccluderInstance occluders);
    public abstract void LightUpdateShadow(Guid id, int shadowIndex, in Transform2D<RealT> lightXform, int lightMask, float near, float far, LightOccluderInstance occluders);
    public abstract Guid OccluderPolygonCreate();
    public abstract void OccluderPolygonSetCullMode(Guid occluder, RS.CanvasOccluderPolygonCullMode mode);
    public abstract void OccluderPolygonSetShape(Guid occluder, Vector2<RealT>[] points, bool closed);
    public abstract void RenderSdf(Guid renderTarget, LightOccluderInstance occluders);
    public abstract ulong RequestPolygon(IList<int> indices, IList<Vector2<RealT>> points, IList<Color> colors, IList<Vector2<RealT>>? uvs = default, IList<int>? bones = default, IList<float>? weights = default);
    public abstract void SetShadowTextureSize(int size);
    public abstract void Update();
}
