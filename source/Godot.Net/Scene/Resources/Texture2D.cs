namespace Godot.Net.Scene.Resources;

using Godot.Net.Core.Math;

public class Texture2D : Texture
{
    public virtual int Height
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public virtual Vector2<RealT> Size
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public virtual int Width
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public Texture2D()
    { }

    public virtual void Draw(Guid canvasItemId, in Vector2<RealT> pos) =>
        this.Draw(canvasItemId, pos, new(1, 1, 1, 1));

    public virtual void Draw(Guid canvasItemId, in Vector2<RealT> pos, in Color modulate, bool transpose = false) => throw new NotImplementedException();
}
