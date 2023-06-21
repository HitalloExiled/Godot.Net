namespace Godot.Net.Servers.Rendering;

public partial class ShaderCompiler
{
    public partial record GeneratedCode
    {
        public Dictionary<string, string> Code                       { get; } = new();
        public List<string>               Defines                    { get; } = new();
        public string?[]                  StageGlobals               { get; set; } = new string[(int)Stage.STAGE_MAX];
        public List<Texture>              TextureUniforms            { get; } = new();
        public List<uint>                 UniformOffsets             { get; } = new();
        public string?                    Uniforms                   { get; set; }
        public uint                       UniformTotalSize           { get; set; }
        public bool                       UsesDepthTexture           { get; set; }
        public bool                       UsesFragmentTime           { get; set; }
        public bool                       UsesGlobalTextures         { get; set; }
        public bool                       UsesNormalRoughnessTexture { get; set; }
        public bool                       UsesScreenTexture          { get; set; }
        public bool                       UsesScreenTextureMipmaps   { get; set; }
        public bool                       UsesVertexTime             { get; set; }
    }
}
