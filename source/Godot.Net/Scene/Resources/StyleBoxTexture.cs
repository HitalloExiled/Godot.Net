namespace Godot.Net.Scene.Resources;

using Godot.Net.Core.Enums;
using Godot.Net.Core.Math;

public class StyleBoxTexture : StyleBox
{
    private readonly float[] textureMargin = new float[4];

    private bool       drawCenter;
    private Texture2D? texture;

    public bool DrawCenter
    {
        get => this.drawCenter;
        set
        {
            this.drawCenter = value;
            this.EmitChanged();
        }
    }

    public Texture2D? Texture
    {
        get => throw new NotImplementedException();
        set
        {
            if (this.texture == value)
            {
                return;
            }
            this.texture = value;
            this.EmitChanged();
        }
    }

    public void SetTextureMarginIndividual(float left, float top, float right, float bottom)
    {
        this.textureMargin[(int)Side.SIDE_LEFT]   = left;
        this.textureMargin[(int)Side.SIDE_TOP]    = top;
        this.textureMargin[(int)Side.SIDE_RIGHT]  = right;
        this.textureMargin[(int)Side.SIDE_BOTTOM] = bottom;

        this.EmitChanged();
    }

    public override void Draw(Guid canvasItemId, in Rect2<float> rect) => throw new NotImplementedException();
}
