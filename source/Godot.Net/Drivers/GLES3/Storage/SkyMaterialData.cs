namespace Godot.Net.Drivers.GLES3.Storage;

public class SkyMaterialData : MaterialData
{
    public SkyShaderData? ShaderData        { get; set; }
    public bool           UniformSetUpdated { get; set; }

    public override void BindUniforms() => throw new NotImplementedException();
    public override void UpdateParameters(Dictionary<string, object> parameters, bool uniformDirty, bool texturesDirty) => throw new NotImplementedException();
}
