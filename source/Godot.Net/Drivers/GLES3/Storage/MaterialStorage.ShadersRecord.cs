namespace Godot.Net.Drivers.GLES3.Storage;

using Godot.Net.Drivers.GLES3.Shaders;
using Godot.Net.Servers.Rendering;

public partial class MaterialStorage
{
    public record ShadersRecord
    {
        public CanvasShaderGLES3        CanvasShader           { get; } = new();
        public ShaderCompiler           CompilerCanvas         { get; } = new();
        public ShaderCompiler           CompilerParticles      { get; } = new();
        public ShaderCompiler           CompilerScene          { get; } = new();
        public ShaderCompiler           CompilerSky            { get; } = new();
        public CubemapFilterShaderGLES3 CubemapFilterShader    { get; } = new();
        public ParticlesShaderGLES3     ParticlesProcessShader { get; } = new();
        public SceneShaderGLES3         SceneShader            { get; } = new();
        public SkyShaderGLES3           SkyShader              { get; } = new();
        public CopyShaderGLES3          CopyShaderGLES3        { get; } = new();
    }
}
