namespace Godot.Net.Drivers.GLES3.Storage;
using Godot.Net.Core;
using Godot.Net.Servers.Rendering.Storage;

public record Material
{
    public Guid                       Self          { get; set; }
    public MaterialData?              Data          { get; set; }
    public Shader?                    Shader        { get; set; }
    public RS.ShaderMode              ShaderMode    { get; set; } = RS.ShaderMode.SHADER_MAX;
    public Guid                       ShaderId      { get; set; }
    public bool                       UniformDirty  { get; set; }
    public bool                       TextureDirty  { get; set; }
    public Dictionary<string, object> Params        { get; } = new();
    public int                        Priority      { get; set; }
    public Guid                       NextPass      { get; set; }
    public SelfList<Material>         UpdateElement { get; }
    public Dependency                 Dependency    { get; } = new();

    public Material() => this.UpdateElement = new(this);
}
