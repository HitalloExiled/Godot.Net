namespace Godot.Net.Servers.Rendering;

using System.Collections.Generic;
using Godot.Net.Core.Math;
using Godot.Net.Servers.Rendering.Storage;

public partial class RendererViewport
{
    public partial record Viewport
    {
        public Guid                            Camera                        { get; set; }
        public uint                            CanvasCullMask                { get; set; } = 0xffffffff;
        public Dictionary<Guid, CanvasData>    CanvasMap                     { get; } = new();
        public RS.ViewportClearMode            ClearMode                     { get; set; } = RS.ViewportClearMode.VIEWPORT_CLEAR_ALWAYS;
        public RS.ViewportDebugDraw            DebugDraw                     { get; set; } = RS.ViewportDebugDraw.VIEWPORT_DEBUG_DRAW_DISABLED;
        public bool                            Disable2D                     { get; set; }
        public bool                            Disable3D                     { get; set; }
        public RS.ViewportEnvironmentMode      DisableEnvironment            { get; set; } = RS.ViewportEnvironmentMode.VIEWPORT_ENVIRONMENT_INHERIT;
        public bool                            FsrEnabled                    { get; set; }
        public float                           FsrSharpness                  { get; set; } = 0.2f;
        public Transform2D<RealT>              GlobalTransform               { get; set; } = new();
        public Vector2<int>                    InternalSize                  { get; set; }
        public long                            LastPass                      { get; set; }
        public bool                            MeasureRenderTime             { get; set; }
        public float                           MeshLodThreshold              { get; set; } = 1.0f;
        public RS.ViewportMSAA                 Msaa2D                        { get; set; } = RS.ViewportMSAA.VIEWPORT_MSAA_DISABLED;
        public RS.ViewportMSAA                 Msaa3D                        { get; set; } = RS.ViewportMSAA.VIEWPORT_MSAA_DISABLED;
        public bool                            OcclusionBufferDirty          { get; set; }
        public Guid                            Parent                        { get; set; }
        public RendererSceneRender.CameraData? PrevCameraData                { get; set; }
        public ulong                           PrevCameraDataFrame           { get; set; }
        public RenderSceneBuffers?             RenderBuffers                 { get; set; }
        public RenderingMethod.RenderInfo      RenderInfo                    { get; } = new();
        public Guid                            RenderTarget                  { get; set; }
        public Guid                            RenderTargetTexture           { get; set; }
        public RS.ViewportScaling3DMode        Scaling3DMode                 { get; set; } = RS.ViewportScaling3DMode.VIEWPORT_SCALING_3D_MODE_BILINEAR;
        public float                           Scaling3DScale                { get; set; } = 1.0f;
        public Guid                            Scenario                      { get; set; }
        public RS.ViewportScreenSpaceAA        ScreenSpaceAA                 { get; set; } = RS.ViewportScreenSpaceAA.VIEWPORT_SCREEN_SPACE_AA_DISABLED;
        public bool                            SdfActive                     { get; set; }
        public Guid                            Self                          { get; set; }
        public Guid                            ShadowAtlas                   { get; set; }
        public bool                            ShadowAtlas16Bits             { get; set; } = true;
        public int                             ShadowAtlasSize               { get; set; } = 2048;
        public Vector2<int>                    Size                          { get; set; }
        public bool                            Snap2DTransformsToPixel       { get; set; }
        public bool                            Snap2DVerticesToPixel         { get; set; }
        public RS.CanvasItemTextureFilter      TextureFilter                 { get; set; } = RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_LINEAR;
        public float                           TextureMipmapBias             { get; set; }
        public RS.CanvasItemTextureRepeat      TextureRepeat                 { get; set; } = RS.CanvasItemTextureRepeat.CANVAS_ITEM_TEXTURE_REPEAT_DISABLED;
        public ulong                           TimeCpuBegin                  { get; set; }
        public ulong                           TimeCpuEnd                    { get; set; }
        public ulong                           TimeGpuBegin                  { get; set; }
        public ulong                           TimeGpuEnd                    { get; set; }
        public bool                            TransparentBg                 { get; set; }
        public RS.ViewportUpdateMode           UpdateMode                    { get; set; } = RS.ViewportUpdateMode.VIEWPORT_UPDATE_WHEN_VISIBLE;
        public bool                            UseDebanding                  { get; set; }
        public bool                            UseOcclusionCulling           { get; set; }
        public bool                            UseTAA                        { get; set; }
        public bool                            UseXr                         { get; set; } // use xr interface to override camera positioning and projection matrices and control output
        public uint                            ViewCount                     { get; set; }
        public bool                            ViewportRenderDirectToScreen  { get; set; }
        public int                             ViewportToScreen              { get; set; }
        public Rect2<RealT>                    ViewportToScreenRect          { get; set; }

        public Viewport()
        {
            this.ViewCount          = 1;
            this.UpdateMode         = RS.ViewportUpdateMode.VIEWPORT_UPDATE_WHEN_VISIBLE;
            this.ClearMode          = RS.ViewportClearMode.VIEWPORT_CLEAR_ALWAYS;
            this.TransparentBg      = false;

            this.ViewportToScreen  = DisplayServer.INVALID_WINDOW_ID;
            this.ShadowAtlasSize   = 0;
            this.MeasureRenderTime = false;

            this.DebugDraw            = RS.ViewportDebugDraw.VIEWPORT_DEBUG_DRAW_DISABLED;
            this.ScreenSpaceAA        = RS.ViewportScreenSpaceAA.VIEWPORT_SCREEN_SPACE_AA_DISABLED;
            this.UseDebanding         = false;
            this.UseOcclusionCulling  = false;
            this.OcclusionBufferDirty = true;

            this.Snap2DTransformsToPixel = false;
            this.Snap2DVerticesToPixel   = false;

            this.UseXr     = false;
            this.SdfActive = false;

            this.TimeCpuBegin = 0;
            this.TimeCpuEnd   = 0;

            this.TimeGpuBegin = 0;
            this.TimeGpuEnd   = 0;
        }
    }
}
