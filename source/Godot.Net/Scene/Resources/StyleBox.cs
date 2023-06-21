namespace Godot.Net.Scene.Resources;

using Godot.Net.Core.Enums;
using Godot.Net.Core.IO;
using Godot.Net.Core.Math;

public class StyleBox : Resource
{
    private readonly float[] contentMargin = new float[4];

    public Vector2<RealT> Offset => new(this.GetMargin(Side.SIDE_LEFT), this.GetMargin(Side.SIDE_TOP));

    public virtual Vector2<RealT> MinimumSize
    {
        get
        {
            var minSize = new Vector2<RealT>(
                this.GetMargin(Side.SIDE_LEFT) + this.GetMargin(Side.SIDE_RIGHT),
                this.GetMargin(Side.SIDE_TOP) + this.GetMargin(Side.SIDE_BOTTOM)
            );
            #region TODO
            // Size2 custom_size;
            // GDVIRTUAL_CALL(_get_minimum_size, custom_size);

            // if (min_size.x < custom_size.x)
            // {
            //     min_size.x = custom_size.x;
            // }
            // if (min_size.y < custom_size.y)
            // {
            //     min_size.y = custom_size.y;
            // }
            #endregion TODO

            return minSize;
        }
    }

    public StyleBox() => Array.Fill(this.contentMargin, -1);

    #region protected methods
    protected virtual float GetStyleMargin(Side side) => 0;
    #endregion protected methods

    #region public virtual methods
    public virtual void Draw(Guid canvasItemId, in Rect2<RealT> rect)
    {
        // TODO - GDVIRTUAL_REQUIRED_CALL(_draw, p_canvas_item, p_rect);
    }
    #endregion public virtual methods

    #region public methods
    public float GetContentMargin(Side side) =>
        ERR_FAIL_INDEX_V((int)side, 4) ? 0 : this.contentMargin[(int)side];

    public float GetMargin(Side side) =>
        ERR_FAIL_INDEX_V((int)side, 4)
            ? 0
            : this.contentMargin[(int)side] < 0
                ? this.GetStyleMargin(side)
                : this.contentMargin[(int)side];

    public void SetContentMargin(Side side, float value)
    {
        if (ERR_FAIL_INDEX((int)side, 4))
        {
            return;
        }

        this.contentMargin[(int)side] = value;

        this.EmitChanged();
    }

    public void SetContentMarginAll(float value)
    {
        for (var i = 0; i < 4; i++)
        {
            this.contentMargin[i] = value;
        }

        this.EmitChanged();
    }

    public void SetContentMarginIndividual(float left, float top, float right, float bottom)
    {
        this.contentMargin[(int)Side.SIDE_LEFT]   = left;
        this.contentMargin[(int)Side.SIDE_TOP]    = top;
        this.contentMargin[(int)Side.SIDE_RIGHT]  = right;
        this.contentMargin[(int)Side.SIDE_BOTTOM] = bottom;

        this.EmitChanged();
    }
    #endregion public methods
}
