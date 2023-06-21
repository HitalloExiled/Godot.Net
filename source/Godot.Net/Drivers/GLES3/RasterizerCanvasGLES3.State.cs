namespace Godot.Net.Drivers.GLES3;

using System.Collections.Generic;

public partial class RasterizerCanvasGLES3
{
    private record State
    {
        public List<Batch>      CanvasInstanceBatches      { get; } = new();
        public List<DataBuffer> CanvasInstanceDataBuffers  { get; } = new();

        public int                        CurrentBatchIndex          { get; set; }
        public int                        CurrentDataBufferIndex     { get; set; }
        public RS.CanvasItemTextureFilter CurrentFilterMode          { get; set; } = RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_MAX;
        public int                        CurrentInstanceBufferIndex { get; set; }
        public RS.CanvasItemTextureRepeat CurrentRepeatMode          { get; set; } = RS.CanvasItemTextureRepeat.CANVAS_ITEM_TEXTURE_REPEAT_MAX;
        public Guid                       CurrentTex                 { get; set; }
        public RS.CanvasItemTextureFilter DefaultFilter              { get; set; } = RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_DEFAULT;
        public RS.CanvasItemTextureRepeat DefaultRepeat              { get; set; } = RS.CanvasItemTextureRepeat.CANVAS_ITEM_TEXTURE_REPEAT_DEFAULT;
        public InstanceData[]             InstanceDataArray          { get; set; } = Array.Empty<InstanceData>();
        public int                        LastItemIndex              { get; set; }
        public LightUniform[]             LightUniforms              { get; set; } = Array.Empty<LightUniform>();
        public uint                       ShadowDepthBuffer          { get; set; }
        public uint                       ShadowFb                   { get; set; }
        public uint                       ShadowTexture              { get; set; }
        public int                        ShadowTextureSize          { get; set; } = 2048;
        public double                     Time                       { get; set; }
        public bool                       TransparentRenderTarget    { get; set; }
        public bool                       UsingDirectionalLights     { get; set; }
    }
}
