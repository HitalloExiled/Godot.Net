namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public partial record ShaderNode : Node
    {
        public override NodeType Type => NodeType.TYPE_SHADER;

        public Dictionary<string, Constant>       Constants   { get; } = new();
        public Dictionary<string, Function>       Functions   { get; } = new();
        public List<string>                       RenderModes { get; } = new();
        public Dictionary<string, Struct>         Structs     { get; } = new();
        public Dictionary<string, Uniform>        Uniforms    { get; } = new();
        public Dictionary<string, Varying>        Varyings    { get; } = new();
        public Dictionary<string, Constant>       Vconstants  { get; } = new();
        public List<Struct>                       Vstructs    { get; } = new();
    }
}
