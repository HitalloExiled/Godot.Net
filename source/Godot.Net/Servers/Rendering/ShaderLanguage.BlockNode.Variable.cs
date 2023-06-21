namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public partial record BlockNode
    {
        public record Variable
        {
            public DataType                Type       { get; set; }
            public string?                 StructName { get; set; }
            public DataPrecision           Precision  { get; set; }
            public int                     Line       { get; set; } //for completion
            public uint                    ArraySize  { get; set; }
            public bool                    IsConst    { get; set; }
            public ConstantNode.ValueUnion Value      { get; set; } = new();
        };
    }
}
