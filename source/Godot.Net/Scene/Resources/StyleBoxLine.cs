namespace Godot.Net.Scene.Resources;

using Godot.Net.Core.Math;

public class StyleBoxLine : StyleBox
{
    private Color color;
    private float growBegin;
    private float growEnd;
    private int   thickness;
    private bool  vertical;

    public Color Color
    {
        get => this.color;
        set
        {
            this.color = value;
            this.EmitChanged();
        }
    }

    public float GrowBegin
    {
        get => this.growBegin;
        set
        {
            this.growBegin = value;
            this.EmitChanged();
        }
    }

	public float GrowEnd
    {
        get => this.growEnd;
        set
        {
            this.growEnd = value;
            this.EmitChanged();
        }
    }

	public int Thickness
    {
        get => this.thickness;
        set
        {
            this.thickness = value;
            this.EmitChanged();
        }
    }

	public bool Vertical
    {
        get => this.vertical;
        set
        {
            this.vertical = value;
            this.EmitChanged();
        }
    }

    public override void Draw(Guid canvasItemId, in Rect2<RealT> rect)
    {
        var vs = RS.Singleton;

        var r = this.vertical
            ? new Rect2<RealT>(
                new(rect.Position.X, rect.Position.Y - this.GrowBegin),
                new(rect.Size.Y + (this.GrowBegin + this.GrowEnd), this.thickness)
            )
            : new Rect2<RealT>(
                new(rect.Position.X - this.GrowBegin, rect.Position.Y),
                new(rect.Size.X + (this.GrowBegin + this.GrowEnd), this.thickness)
            );

        vs.CanvasItemAddRect(canvasItemId, r, this.color);
    }
}
