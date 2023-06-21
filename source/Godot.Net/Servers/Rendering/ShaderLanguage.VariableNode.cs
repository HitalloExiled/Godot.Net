namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public record VariableNode : Node
    {
        public override NodeType Type => NodeType.TYPE_VARIABLE;

        public DataType DatatypeCache { get; set; } = DataType.TYPE_VOID;
        public bool     IsConst       { get; set; }
        public bool     IsLocal       { get; set; }
        public string   Name          { get; set; } = "";
        public string   StructName    { get; set; } = "";

        public override DataType GetDatatype()     => this.DatatypeCache;
        public override string   GetDatatypeName() => this.StructName;
    }
}
