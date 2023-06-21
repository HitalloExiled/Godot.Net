namespace Godot.Net.Drivers.GLES3.Storage;

public class CanvasMaterialData : MaterialData
{
    public CanvasShaderData? ShaderData { get; set; }

    public override void UpdateParameters(Dictionary<string, object> parameters, bool uniformDirty, bool texturesDirty) => throw new NotImplementedException();
}
