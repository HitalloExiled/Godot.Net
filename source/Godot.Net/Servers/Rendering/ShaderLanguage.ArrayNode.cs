namespace Godot.Net.Servers.Rendering;

#pragma warning disable CA1721 // TODO Remove

public partial class ShaderLanguage
{
    public record ArrayNode : Node
    {
        public override NodeType Type => NodeType.TYPE_ARRAY;

        public uint     ArraySize        { get; set; }
        public Node?    AssignExpression { get; set; }
        public Node?    CallExpression   { get; set; }
        public DataType DatatypeCache    { get; set; } = DataType.TYPE_VOID;
        public Node?    IndexExpression  { get; set; }
        public bool     IsConst          { get; set; }
        public bool     IsLocal          { get; set; }
        public string   Name             { get; set; } = "";
        public string   StructName       { get; set; } = "";

        public override bool IsIndexed => this.IndexExpression != null;

        public override uint GetArraySize() => (this.IndexExpression != null || this.CallExpression != null) ? 0 : this.ArraySize;
        public override DataType GetDatatype() => this.CallExpression != null ? this.CallExpression.GetDatatype() : this.DatatypeCache;
        public override string GetDatatypeName() => this.CallExpression != null ? this.CallExpression.GetDatatypeName() : this.StructName;
    }
}
