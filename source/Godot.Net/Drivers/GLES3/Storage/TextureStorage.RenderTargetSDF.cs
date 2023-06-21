namespace Godot.Net.Drivers.GLES3.Storage;

using Godot.Net.Drivers.GLES3.Shaders;

#pragma warning disable IDE0044 // TODO Remove

public partial class TextureStorage
{
    private record RenderTargetSDF
    {
        public CanvasSdfShaderGLES3 Shader        { get; } = new();
        public Guid                 ShaderVersion { get; set; }
    }
}
