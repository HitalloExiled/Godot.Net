namespace Godot.Net.Drivers.GLES3;

public abstract partial class ShaderGLES3
{
    public record Version
    {
        public Dictionary<string, string> CodeSections    { get; } = new();
        public List<string>               CustomDefines   { get; } = new();
        public string?                    FragmentGlobals { get; set; }
        public List<string>               TextureUniforms { get; set; } = new();
        public string?                    Uniforms        { get; set; }
        public string?                    VertexGlobals   { get; set; }

        public record Specialization
        {
            public bool                    BuildQueued             { get; set; }
            public Dictionary<string, int> CustomUniformLocations  { get; } = new();
            public uint                    FragId                  { get; set; }
            public uint                    Id                      { get; set; }
            public bool                    Ok                      { get; set; }
            public List<int>               TextureUniformLocations { get; } = new();
            public List<int>               UniformLocation         { get; } = new();
            public uint                    VertId                  { get; set; }
        };

        public List<Dictionary<ulong, Specialization>> Variants { get; } = new();
    };


}
