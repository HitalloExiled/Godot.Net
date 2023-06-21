namespace Godot.Net.Servers.Rendering.RendererRD;

public partial class RendererCompositorRD
{
    public struct Blit
    {
        public Guid             Array         { get; set; }
        public Guid             IndexBuffer   { get; set; }
        public Guid[]           Pipelines     { get; set; } = new Guid[(int)BlitMode.BLIT_MODE_MAX];
        public BlitPushConstant PushConstant  { get; set; }
        public Guid             Sampler       { get; set; }
        public BlitShaderRD     Shader        { get; set; } = null!;
        public Guid             ShaderVersion { get; set; }

        public Blit() { }
    }
}
