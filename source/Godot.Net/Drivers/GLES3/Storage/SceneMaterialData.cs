namespace Godot.Net.Drivers.GLES3.Storage;

#pragma warning disable IDE0051, IDE0051, IDE0044, CS0414, CS0169, IDE0052 // TODO Remove

public class SceneMaterialData : MaterialData
{
    public uint             Index      { get; set; }
    public ulong            LastPass   { get; set; }
    public SceneShaderData? ShaderData { get; set; }

    public override void BindUniforms() => throw new NotImplementedException();
    public override void UpdateParameters(Dictionary<string, object> parameters, bool uniformDirty, bool texturesDirty) => throw new NotImplementedException();
}
