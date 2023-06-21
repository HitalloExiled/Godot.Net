namespace Godot.Net.Scene.Main;
using Godot.Net.Scene.Resources;
#pragma warning disable IDE0052, CA1822, IDE0021, IDE0044, CS0649 // TODO REMOVE

public class ViewportTexture : Texture2D
{
    public Viewport Viewport { get; }
    public Guid     Proxy    { get; set; }

    public ViewportTexture(Viewport viewport) => this.Viewport = viewport;
}
