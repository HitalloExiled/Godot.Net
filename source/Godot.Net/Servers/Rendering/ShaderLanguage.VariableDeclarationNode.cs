namespace Godot.Net.Servers.Rendering;

#pragma warning disable CA1721 // TODO Remove;

public partial class ShaderLanguage
{
    public partial record VariableDeclarationNode : Node
    {
        public override NodeType Type => NodeType.TYPE_VARIABLE_DECLARATION;

        public DataType          Datatype     { get; set; } = DataType.TYPE_VOID;
        public List<Declaration> Declarations { get; set; } = new();
        public bool              IsConst      { get; set; }
        public DataPrecision     Precision    { get; set; } = DataPrecision.PRECISION_DEFAULT;
        public string            StructName   { get; set; } = "";

        public override DataType GetDatatype() => this.Datatype;
    }
}
