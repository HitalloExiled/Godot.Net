namespace Godot.Net.Servers.Rendering;

public partial class ShaderCompiler
{
    public record IdentifierActions
    {
        public Dictionary<string, Stage>                             EntryPointStages  { get; } = new();
        public Dictionary<string, bool>                              RenderModeFlags   { get; } = new();
        public Dictionary<string, (int, int)>                        RenderModeValues  { get; } = new();
        public Dictionary<string, ShaderLanguage.ShaderNode.Uniform> Uniforms          { get; set; } = new();
        public Dictionary<string, bool>                              UsageFlagPointers { get; } = new();
        public Dictionary<string, bool>                              WriteFlagPointers { get; } = new();
    };
}
