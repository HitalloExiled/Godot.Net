namespace Godot.Net.Servers.Rendering;

#pragma warning disable CA1721 // TODO Remove;

public partial class ShaderLanguage
{
    public record MemberNode : Node
    {
        public override NodeType Type => NodeType.TYPE_MEMBER;

        public uint          ArraySize              { get; set; }
        public Node?         AssignExpression       { get; set; }
        public string?       BaseStructName         { get; set; }
        public DataType      Basetype               { get; set; } = DataType.TYPE_VOID;
        public bool          BasetypeConst          { get; set; }
        public Node?         CallExpression         { get; set; }
        public DataType      Datatype               { get; set; }
        public bool          HasSwizzlingDuplicates { get; set; }
        public Node?         IndexExpression        { get; set; }
        public string        Name                   { get; set; } = string.Empty;
        public Node?         Owner                  { get; set; }
        public DataPrecision Precision              { get; set; } = DataPrecision.PRECISION_DEFAULT;
        public string        StructName             { get; set; } = "";

        public override bool IsIndexed => this.IndexExpression != null || this.CallExpression != null;

        public override uint     GetArraySize()    => (this.IndexExpression != null || this.CallExpression != null) ? 0u : this.ArraySize;
        public override DataType GetDatatype()     => this.CallExpression != null ? this.CallExpression.GetDatatype() : this.Datatype;
        public override string   GetDatatypeName() => this.CallExpression != null ? this.CallExpression.GetDatatypeName() : this.StructName;
    }
}
