namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public partial record ShaderNode
    {
        public record Constant
        {
            public required string   Name  { get; init; }
            public required int      Index { get; init; }
            public required DataType Type  { get; set; }

            public uint          ArraySize   { get; set; }
            public ConstantNode? Initializer { get; set; }
            public DataPrecision Precision   { get; set; }
            public string        TypeStr     { get; set; } = "";
        }
    }
}
