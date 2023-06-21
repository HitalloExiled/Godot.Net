namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public partial record ShaderNode
    {
        public record Struct
        {
            public required string Name { get; init; }

            public StructNode? ShaderStruct { get; set; }
        }
    }
}
