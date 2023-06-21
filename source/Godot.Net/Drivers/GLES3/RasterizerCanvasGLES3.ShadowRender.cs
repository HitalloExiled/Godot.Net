namespace Godot.Net.Drivers.GLES3;

using Godot.Net.Drivers.GLES3.Shaders;

public partial class RasterizerCanvasGLES3
{
    private record ShadowRender
    {
        public CanvasOcclusionShaderGLES3 Shader        { get; } = new();
        public Guid                       ShaderVersion { get; set; }
    }
}
