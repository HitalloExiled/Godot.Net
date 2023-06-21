namespace Godot.Net.Servers.Rendering;
public partial class ShaderLanguage
{
    public record Expression
    {
        public bool     IsOp { get; set; }
        public Node?    Node { get; set; }
        public Operator Op   { get; set; }
    }
}
