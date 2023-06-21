namespace Godot.Net.Scene.Main;

#pragma warning disable IDE0044, CS0649 // TODO Remove

public class CanvasLayer : Node
{
    public Guid Canvas    { get; }
    public bool IsVisible { get; }

    public CanvasLayer() => this.Canvas = RS.Singleton.CanvasCreate();

    public void ResetSortIndex() => throw new NotImplementedException();
}
