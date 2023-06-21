namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public partial record FunctionNode : Node
    {
        public override NodeType Type => NodeType.TYPE_FUNCTION;

        public Dictionary<string, Argument> Arguments        { get; } = new();
        public BlockNode?                   Body             { get; set; }
        public bool                         CanDiscard       { get; set; }
        public string                       Name             { get; set; } = "";
        public uint                         ReturnArraySize  { get; set; }
        public DataPrecision                ReturnPrecision  { get; set; } = DataPrecision.PRECISION_DEFAULT;
        public string                       ReturnStructName { get; set; } = "";
        public DataType                     ReturnType       { get; set; } = DataType.TYPE_VOID;

        public override DataType GetDatatype()     => this.ReturnType;
        public override string   GetDatatypeName() => this.ReturnStructName;
        public override uint     GetArraySize()    => this.ReturnArraySize;
    }
}
