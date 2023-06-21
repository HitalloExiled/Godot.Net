namespace Godot.Net.Drivers.GLES3;

public abstract partial class ShaderGLES3
{
    protected record StageTemplate
    {
        public enum ChunkType
        {
            TYPE_MATERIAL_UNIFORMS,
            TYPE_VERTEX_GLOBALS,
            TYPE_FRAGMENT_GLOBALS,
            TYPE_CODE,
            TYPE_TEXT
        }

        public record Chunk
        {
            public ChunkType  Type { get; set; }
            public string?    Code { get; set; }
            public string?    Text { get; set; }
        }

        public List<Chunk> Chunks { get; } = new();
    };
}
