namespace Godot.Net.Scene.Resources;

public class GradientTexture2D : Texture2D
{
    public Gradient? Gradient { get; set; }

    public override int Height { get; set; }
    public override int Width  { get; set; }
}
