namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public record OperatorNode : Node
    {
        public override NodeType Type => NodeType.TYPE_OPERATOR;

        public List<Node>    Arguments            { get; set; } = new();
        public Operator      Op                   { get; set; } = Operator.OP_EQUAL;
        public uint          ReturnArraySize      { get; set; }
        public DataType      ReturnCache          { get; set; } = DataType.TYPE_VOID;
        public DataPrecision ReturnPrecisionCache { get; set; } = DataPrecision.PRECISION_DEFAULT;
        public string        StructName           { get; set; } = "";

        public override bool IsIndexed => this.Op == Operator.OP_INDEX;

        public override uint     GetArraySize()    => this.ReturnArraySize;
        public override DataType GetDatatype()     => this.ReturnCache;
        public override string   GetDatatypeName() => this.StructName;
    }
}
