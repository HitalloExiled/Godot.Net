namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public record StructNode : Node
    {
        public override NodeType Type => NodeType.TYPE_STRUCT;

        public List<MemberNode> Members { get; set; } = new();
    }
}
