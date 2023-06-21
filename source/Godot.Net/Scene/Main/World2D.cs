#pragma warning disable IDE0052 // TODO REMOVE

namespace Godot.Net.Scene.Main;

using Godot.Net.Core.IO;

public class World2D : Resource
{
    public Guid Canvas { get; }

    public World2D() => this.Canvas = RS.Singleton.CanvasCreate();
}
