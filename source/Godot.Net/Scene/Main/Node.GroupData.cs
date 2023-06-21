namespace Godot.Net.Scene.Main;

public partial class Node
{
    public record GroupData
    {
        public SceneTree.Group? Group      { get; set; }
        public bool             Persistent { get; set; }
    };
}
