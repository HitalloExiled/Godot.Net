namespace Godot.Net.Drivers.GLES3;
public partial class RasterizerCanvasGLES3
{
    public record Data
    {
        public uint CanvasQuadArray            { get; set; }
        public uint CanvasQuadVertices         { get; set; }
        public Guid CanvasShaderDefaultVersion { get; set; }
        public uint IndexedQuadArray           { get; set; }
        public uint IndexedQuadBuffer          { get; set; }
        public uint MaxInstanceBufferSize      { get; set; } = 16384 * 128;
        public uint MaxInstancesPerBuffer      { get; set; } = 16384;
        public uint MaxLightsPerItem           { get; set; } = 16;
        public uint MaxLightsPerRender         { get; set; } = 256;
        public uint NinepatchElements          { get; set; }
        public uint NinepatchVertices          { get; set; }
        public uint ParticleQuadArray          { get; set; }
        public uint ParticleQuadVertices       { get; set; }
    }
}
