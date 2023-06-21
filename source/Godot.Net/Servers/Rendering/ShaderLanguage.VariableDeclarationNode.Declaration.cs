namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public partial record VariableDeclarationNode
    {
        public record Declaration
        {
            public List<Node> Initializer      { get; set; } = new();
            public string     Name             { get; set; } = "";
            public bool       SingleExpression { get; set; }
            public uint       Size             { get; set; }
            public Node?      SizeExpression   { get; set; }
        }
    }
}
