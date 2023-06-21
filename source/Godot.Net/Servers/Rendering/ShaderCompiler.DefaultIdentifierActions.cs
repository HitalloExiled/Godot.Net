namespace Godot.Net.Servers.Rendering;

public partial class ShaderCompiler
{
    public record DefaultIdentifierActions
    {
        public bool                          ApplyLuminanceMultiplier     { get; set; }
        public int                           BaseTextureBindingIndex      { get; set; }
        public string                        BaseUniformString            { get; set; } = "";
        public uint                          BaseVaryingIndex             { get; set; }
        public bool                          CheckMultiviewSamplers       { get; set; }
        public Dictionary<string, string>    CustomSamplers               { get; } = new();
        public ShaderLanguage.TextureFilter  DefaultFilter                { get; set; }
        public ShaderLanguage.TextureRepeat  DefaultRepeat                { get; set; }
        public string                        GlobalBufferArrayVariable    { get; set; } = "";
        public string                        InstanceUniformIndexVariable { get; set; } = "";
        public Dictionary<string, string>    Renames                      { get; } = new();
        public Dictionary<string, string>    RenderModeDefines            { get; } = new();
        public string                        SamplerArrayName             { get; set; } = "";
        public int                           TextureLayoutSet             { get; set; }
        public Dictionary<string, string>    UsageDefines                 { get; } = new();
    }
}
