namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public record ShaderCompileInfo
    {
        public Dictionary<string, FunctionInfo> Functions                   { get; init; } = new();
        public GlobalShaderUniformGetTypeFunc?  GlobalShaderUniformTypeFunc { get; set; }
        public bool                             IsInclude                   { get; set; }
        public ModeInfo[]                       RenderModes                 { get; init; } = Array.Empty<ModeInfo>();
        public HashSet<string>                  ShaderTypes                 { get; init; } = new();
        public VaryingFunctionNames             VaryingFunctionNames        { get; init; } = new VaryingFunctionNames();
    }
}
