namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public abstract record Node
    {
        public enum NodeType
        {
            TYPE_SHADER,
            TYPE_FUNCTION,
            TYPE_BLOCK,
            TYPE_VARIABLE,
            TYPE_VARIABLE_DECLARATION,
            TYPE_CONSTANT,
            TYPE_OPERATOR,
            TYPE_CONTROL_FLOW,
            TYPE_MEMBER,
            TYPE_ARRAY,
            TYPE_ARRAY_CONSTRUCT,
            TYPE_STRUCT,
        }

        public abstract NodeType Type { get; }

        public Node? Next { get; set; }

        public virtual bool    IsIndexed => default;

        public virtual uint     GetArraySize() => default;
        public virtual DataType GetDatatype()  => DataType.TYPE_VOID;
        public virtual string   GetDatatypeName() => "";
    }
}
