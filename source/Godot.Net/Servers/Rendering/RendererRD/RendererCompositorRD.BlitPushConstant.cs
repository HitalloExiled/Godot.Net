namespace Godot.Net.Servers.Rendering.RendererRD;

public partial class RendererCompositorRD
{
    public struct BlitPushConstant
    {
        public float   AspectRatio { get; set; }
        public float[] DstRect     { get; set; } = new float[4];
        public float[] EyeCenter   { get; set; } = new float[2];
        public float   K1          { get; set; }
        public float   K2          { get; set; }
        public uint    Layer       { get; set; }
        public uint    Pad1        { get; set; }
        public float[] SrcRect     { get; set; } = new float[4];
        public float   Upscale     { get; set; }

        public BlitPushConstant() { }
    }
}
