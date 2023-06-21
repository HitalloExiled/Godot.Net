namespace Godot.Net.Drivers.GLES3.Storage;

using Godot.Net.Servers.Rendering;

#pragma warning disable IDE0052, IDE0044, IDE0051, CS0169 // TODO Remove

public class MaterialData
{
    private readonly Dictionary<string, ulong> usedGlobalTextures = new();

    private Guid? globalBuffer;
    private Guid? globalTexture;
    private ulong globalTexturesPass;

    protected List<Guid> TextureCache  { get; set; } = new();
    protected List<byte> UboData       { get; set; } = new();
    protected uint       UniformBuffer { get; set; }

    public Guid NextPass       { get; set; }
    public int  RenderPriority { get; set; }
    public Guid Self           { get; set; }

    public void UpdateParametersInternal(Dictionary<string, object> parameters, bool uniformDirty, bool texturesDirty, Dictionary<string, ShaderLanguage.ShaderNode.Uniform> uniforms, uint[] uniformOffsets, List<ShaderCompiler.GeneratedCode.Texture> textureUniforms, Dictionary<string, Dictionary<int, Guid>> defaultTextureParams, uint uboSize) => throw new NotImplementedException();
    public void UpdateTextures(Dictionary<string, object> parameters, Dictionary<string, Dictionary<int, Guid>> defaultTextures, List<ShaderCompiler.GeneratedCode.Texture> textureUniforms, Guid textures, bool useLinearColor) => throw new NotImplementedException();
    public void UpdateUniformBuffer(Dictionary<string, ShaderLanguage.ShaderNode.Uniform> uniforms, uint uniformOffsets, Dictionary<string, object> parameters, byte[] buffer, uint bufferSize, bool useLinearColor) => throw new NotImplementedException();

    public virtual void BindUniforms() => throw new NotImplementedException();
    public virtual void UpdateParameters(Dictionary<string, object> parameters, bool uniformDirty, bool texturesDirty) => throw new NotImplementedException();

    //internally by update_parameters_internal
}
