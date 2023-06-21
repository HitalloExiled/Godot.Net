namespace Godot.Net.Drivers.GLES3.Storage;

using Godot.Net.Core.Math;
using Godot.Net.Servers.Rendering.Storage;

#pragma warning disable IDE0052 // TODO Remove

public class RenderSceneBuffersGLES3 : RenderSceneBuffers
{
    private int height;
    private bool isTransparent;
    private Guid renderTarget;
    private uint viewCount;
    private int width;

    public override void Configure(
        Guid                     renderTargetId,
        Vector2<int>             internalSize,
        Vector2<int>             targetSize,
        RealT                    fsrSharpness,
        double                   textureMipmapBias,
        RS.ViewportMSAA          msaa3D,
        RS.ViewportScreenSpaceAA screenSpaceAA,
        bool                     useTaa,
        bool                     useDebanding,
        uint                     viewCount
    )
    {
        var textureStorage = TextureStorage.Singleton;

        this.width        = targetSize.X;
        this.height       = targetSize.Y;
        this.renderTarget = renderTargetId;
        this.viewCount    = viewCount;

        // this.FreeRenderBufferData(); Do nothing

        var renderTarget = textureStorage.GetRenderTarget(renderTargetId)!;

        this.isTransparent = renderTarget.IsTransparent;
    }
}
