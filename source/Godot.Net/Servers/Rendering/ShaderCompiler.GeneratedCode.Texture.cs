namespace Godot.Net.Servers.Rendering;

public partial class ShaderCompiler
{
    public partial record GeneratedCode
    {
        public record Texture
        {
            public uint                                       ArraySize { get; set; }
            public ShaderLanguage.TextureFilter               Filter    { get; set; }
            public bool                                       Global    { get; set; }
            public ShaderLanguage.ShaderNode.Uniform.HintKind Hint      { get; set; }
            public string?                                    Name      { get; set; }
            public ShaderLanguage.TextureRepeat               Repeat    { get; set; }
            public ShaderLanguage.DataType                    Type      { get; set; }
            public bool                                       UseColor  { get; set; }
        }
    }
}
