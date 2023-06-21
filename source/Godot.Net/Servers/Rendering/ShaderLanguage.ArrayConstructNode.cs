namespace Godot.Net.Servers.Rendering;

#pragma warning disable CA1721 // TODO Remove;

public partial class ShaderLanguage
{
    public record ArrayConstructNode : Node
    {
        public override NodeType Type => NodeType.TYPE_ARRAY_CONSTRUCT;

        public DataType   Datatype    { get; set; } = DataType.TYPE_VOID;
        public List<Node> Initializer { get; set; } = new();
        public string     StructName  { get; set; } = "";

        public override uint GetArraySize() => (uint)this.Initializer.Count;
        public override DataType GetDatatype() => this.Datatype;
        public override string GetDatatypeName() => this.StructName;
    }
}
