namespace Godot.Net.Scene.Main;

public partial class SceneTree
{
    public record Group
    {
        public bool       Changed { get; set; }
        public List<Node> Nodes   { get; set; } = new();
    };
}
