namespace Godot.Net.Drivers.GLES3.Storage;

public record Shader
{
    public string?                                   Code                    { get; set; }
    public ShaderData?                               Data                    { get; set; }
    public Dictionary<string, Dictionary<int, Guid>> DefaultTextureParameter { get; } = new();
    public RS.ShaderMode                             Mode                    { get; set; }
    public HashSet<Material>                         Owners                  { get; } = new();
    public string?                                   PathHint                { get; set; }
}
