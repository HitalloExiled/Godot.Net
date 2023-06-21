namespace Godot.Net.Drivers.GLES3;

using Godot.Net.Core.Math;
using Godot.Net.Drivers.GLES3.Shaders;
using Godot.Net.Drivers.GLES3.Storage;

public partial class RasterizerCanvasGLES3
{
    private record Batch
    {
        // Position in the UBO measured in bytes
        public Color                           BlendColor          { get; set; } = new(1, 1, 1, 1);
        public CanvasShaderData.BlendModeType  BlendMode           { get; set; } = CanvasShaderData.BlendModeType.BLEND_MODE_MIX;
        public Item?                           Clip                { get; set; }
        public Item.Command?                   Command             { get; set; }
        public Item.CommandType                CommandType         { get; set; } = Item.CommandType.TYPE_ANIMATION_SLICE; // Can default to any type that doesn't form a batch.
        public RS.CanvasItemTextureFilter      Filter              { get; set; } = RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_MAX;
        public int                             InstanceBufferIndex { get; set; }
        public int                             InstanceCount       { get; set; }
        public bool                            LightsDisabled      { get; set; }
        public Guid                            Material            { get; set; }
        public CanvasMaterialData?             MaterialData        { get; set; }
        public int                             PrimitivePoints     { get; set; }
        public RS.CanvasItemTextureRepeat      Repeat              { get; set; } = RS.CanvasItemTextureRepeat.CANVAS_ITEM_TEXTURE_REPEAT_MAX;
        public CanvasShaderGLES3.ShaderVariant ShaderVariant       { get; set; } = CanvasShaderGLES3.ShaderVariant.MODE_QUAD;
        public int                             Start               { get; set; }
        public Guid                            Tex                 { get; set; }
    }
}
