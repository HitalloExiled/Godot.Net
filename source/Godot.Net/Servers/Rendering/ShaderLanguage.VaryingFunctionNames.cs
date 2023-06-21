namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public record VaryingFunctionNames
    {
        public string Fragment { get; set; } = "fragment";
        public string Vertex   { get; set; } = "vertex";
        public string Light    { get; set; } = "light";
    }
}
