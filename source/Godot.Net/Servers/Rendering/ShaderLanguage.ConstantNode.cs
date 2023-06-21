namespace Godot.Net.Servers.Rendering;

#pragma warning disable CA1721 // TODO Remove

public partial class ShaderLanguage
{
    public partial record ConstantNode : Node
    {
        public override NodeType Type => NodeType.TYPE_CONSTANT;

        public VariableDeclarationNode.Declaration[] ArrayDeclarations { get; set; } = Array.Empty<VariableDeclarationNode.Declaration>();
        public uint                                  ArraySize         { get; set; }
        public DataType                              Datatype          { get; set; } = DataType.TYPE_VOID;
        public string                                StructName        { get; set; } = "";
        public ValueUnion                            Value             { get; set; } = new();
        public List<ValueUnion>                      Values            { get; set; } = new();

        public override uint     GetArraySize()    => this.ArraySize;
        public override DataType GetDatatype()     => this.Datatype;
        public override string   GetDatatypeName() => this.StructName;
    }
}
