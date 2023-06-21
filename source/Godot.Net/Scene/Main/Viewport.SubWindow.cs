namespace Godot.Net.Scene.Main;

public partial class Viewport
{
    public record SubWindow
    {
        public Window? Window     { get; set; }
        public Guid    CanvasItem { get; set; }
    }
}
