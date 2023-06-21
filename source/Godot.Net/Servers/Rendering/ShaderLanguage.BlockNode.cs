namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public partial record BlockNode : Node
    {
        public override NodeType Type => NodeType.TYPE_BLOCK;

        public SubClassTag                  BlockTag                  { get; set; } = SubClassTag.TAG_GLOBAL;
        public BlockTypeKind                BlockType                 { get; set; } = BlockTypeKind.BLOCK_TYPE_STANDARD;
        public BlockNode?                   ParentBlock               { get; set; }
        public FunctionNode?                ParentFunction            { get; set; }
        public bool                         SingleStatement           { get; set; }
        public List<Node>                   Statements                { get; init; } = new();
        public bool                         UseCommaBetweenStatements { get; set; }
        public Dictionary<string, Variable> Variables                 { get; init; } = new();
    }
}
