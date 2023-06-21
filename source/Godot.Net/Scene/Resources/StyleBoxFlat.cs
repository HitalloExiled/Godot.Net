namespace Godot.Net.Scene.Resources;

using Godot.Net.Core.Enums;
using Godot.Net.Core.Math;

#pragma warning disable IDE0052, IDE0044, CS0169, CS0649 // TODO Remove

public class StyleBoxFlat : StyleBox
{
    #region private readonly fields
    private readonly RealT[] borderWidth  = new RealT[4];
    private readonly RealT[] cornerRadius = new RealT[4];
    private readonly RealT[] expandMargin = new RealT[4];
    #endregion private readonly fields

    #region private fields
    private RealT          aaSize = 0.625f;
    private bool           antiAliased;
    private Color          bgColor;
    private bool           blendBorder;
    private Color          borderColor;
    private int            cornerDetail;
    private bool           drawCenter;
    private Color          shadowColor = new(0, 0, 0, 0.6f);
    private Vector2<RealT> shadowOffset;
    private int            shadowSize;
    private Vector2<RealT> skew;
    #endregion private fields

    #region public properties
    public bool AntiAliased
    {
        get => this.antiAliased;
        set
        {
            this.antiAliased = value;
            this.EmitChanged();
            this.NotifyPropertyListChanged();
        }
    }

    public Color BgColor
    {
        get => this.bgColor;
        set
        {
            this.bgColor = value;
            this.EmitChanged();
        }
    }

    public Color BorderColor
    {
        get => this.borderColor;
        set
        {
            this.borderColor = value;
            this.EmitChanged();
        }
    }

    public int CornerDetail
    {
        get => this.cornerDetail;
        set
        {
            this.cornerDetail = Math.Clamp(value, 1, 20);
            this.EmitChanged();
        }
    }

    public bool DrawCenter
    {
        get => this.drawCenter;
        set
        {
            this.drawCenter = value;
            this.EmitChanged();
        }
    }

    public Color ShadowColor
    {
        get => this.shadowColor;
        set
        {
            this.shadowColor = value;
            this.EmitChanged();
        }
    }

    public int ShadowSize
    {
        get => this.shadowSize;
        set
        {
            this.shadowSize = value;
            this.EmitChanged();
        }
    }
    #endregion public properties

    #region private static methods
    private static void AdaptValues(
        int     indexA,
        int     indexB,
        RealT[] adaptedValues,
        RealT[] values,
        RealT   width,
        RealT   maxA,
        RealT   maxB
    )
    {
        if (values[indexA] + values[indexB] > width)
        {
            RealT factor;
            RealT newValue;

            factor = (RealT)width / (RealT)(values[indexA] + values[indexB]);

            newValue = values[indexA] * factor;

            if (newValue < adaptedValues[indexA])
            {
                adaptedValues[indexA] = newValue;
            }

            newValue = values[indexB] * factor;
            if (newValue < adaptedValues[indexB])
            {
                adaptedValues[indexB] = newValue;
            }
        }
        else
        {
            adaptedValues[indexA] = Math.Min(values[indexA], adaptedValues[indexA]);
            adaptedValues[indexB] = Math.Min(values[indexB], adaptedValues[indexB]);
        }

        adaptedValues[indexA] = Math.Min(maxA, adaptedValues[indexA]);
        adaptedValues[indexB] = Math.Min(maxB, adaptedValues[indexB]);
    }

    private static void DrawRing(
        List<Vector2<RealT>> verts,
        List<int>            indices,
        List<Color>          colors,
        in Rect2<RealT>      styleRect,
        RealT[]              cornerRadius,
        in Rect2<RealT>      ringRect,
        in Rect2<RealT>      innerRect,
        in Color             innerColor,
        in Color             outerColor,
        int                  cornerDetail,
        in Vector2<RealT>    skew,
        bool                 fillCenter = false
    )
    {
        var vertOffset = verts.Count;

        var adaptedCornerDetail = (cornerRadius[0] == 0 && cornerRadius[1] == 0 && cornerRadius[2] == 0 && cornerRadius[3] == 0) ? 1 : cornerDetail;

        var ringCornerRadius = new RealT[4];
        SetInnerCornerRadius(styleRect, ringRect, cornerRadius, ringCornerRadius);

        // Corner radius center points.
        var outerPoints = new List<Vector2<RealT>>
        {
            ringRect.Position + new Vector2<RealT>(ringCornerRadius[0], ringCornerRadius[0]), //tl
		    new Vector2<RealT>(ringRect.Position.X + ringRect.Size.X - ringCornerRadius[1], ringRect.Position.Y + ringCornerRadius[1]), //tr
		    ringRect.Position + ringRect.Size - new Vector2<RealT>(ringCornerRadius[2], ringCornerRadius[2]), //br
		    new Vector2<RealT>(ringRect.Position.X + ringCornerRadius[3], ringRect.Position.Y + ringRect.Size.Y - ringCornerRadius[3]) //bl
	    };

        var innerCornerRadius = new RealT[4];
        SetInnerCornerRadius(styleRect, innerRect, cornerRadius, innerCornerRadius);

        var innerPoints = new List<Vector2<RealT>>
        {
            innerRect.Position + new Vector2<RealT>(innerCornerRadius[0], innerCornerRadius[0]), //tl
            new Vector2<RealT>(innerRect.Position.X + innerRect.Size.X - innerCornerRadius[1], innerRect.Position.Y + innerCornerRadius[1]), //tr
            innerRect.Position + innerRect.Size - new Vector2<RealT>(innerCornerRadius[2], innerCornerRadius[2]), //br
            new Vector2<RealT>(innerRect.Position.X + innerCornerRadius[3], innerRect.Position.Y + innerRect.Size.Y - innerCornerRadius[3]) //bl
        };

        // Calculate the vertices.
        for (var cornerIndex = 0; cornerIndex < 4; cornerIndex++)
        {
            for (var detail = 0; detail <= adaptedCornerDetail; detail++)
            {
                for (var innerOuter = 0; innerOuter < 2; innerOuter++)
                {
                    RealT          radius;
                    Color          color;
                    Vector2<RealT> cornerPoint;

                    if (innerOuter == 0)
                    {
                        radius      = innerCornerRadius[cornerIndex];
                        color       = innerColor;
                        cornerPoint = innerPoints[cornerIndex];
                    }
                    else
                    {
                        radius      = ringCornerRadius[cornerIndex];
                        color       = outerColor;
                        cornerPoint = outerPoints[cornerIndex];
                    }

                    var x = radius * (RealT)Math.Cos((cornerIndex + detail / (double)adaptedCornerDetail) * (Math.Tau / 4.0) + Math.PI) + cornerPoint.X;
                    var y = radius * (RealT)Math.Sin((cornerIndex + detail / (double)adaptedCornerDetail) * (Math.Tau / 4.0) + Math.PI) + cornerPoint.Y;
                    var xSkew = -skew.X * (y - ringRect.Center.Y);
                    var ySkew = -skew.Y * (x - ringRect.Center.X);
                    verts.Add(new(x + xSkew, y + ySkew));
                    colors.Add(color);
                }
            }
        }

        var ringVertCount = verts.Count - vertOffset;

        // Fill the indices and the colors for the border.
        for (var i = 0; i < ringVertCount; i++)
        {
            indices.Add(vertOffset + (i + 0) % ringVertCount);
            indices.Add(vertOffset + (i + 2) % ringVertCount);
            indices.Add(vertOffset + (i + 1) % ringVertCount);
        }

        if (fillCenter)
        {
            //Fill the indices and the colors for the center.
            for (var index = 0; index < ringVertCount / 2; index += 2)
            {
                var i = index;
                // Polygon 1.
                indices.Add(vertOffset + i);
                indices.Add(vertOffset + ringVertCount - 4 - i);
                indices.Add(vertOffset + i + 2);
                // Polygon 2.
                indices.Add(vertOffset + i);
                indices.Add(vertOffset + ringVertCount - 2 - i);
                indices.Add(vertOffset + ringVertCount - 4 - i);
            }
        }
    }

    private static void SetInnerCornerRadius(in Rect2<RealT> styleRect, in Rect2<RealT> innerRect, RealT[] cornerRadius, RealT[] innerCornerRadius)
    {
        var borderLeft   = innerRect.Position.X - styleRect.Position.X;
        var borderTop    = innerRect.Position.Y - styleRect.Position.Y;
        var borderRight  = styleRect.Size.X - innerRect.Size.X - borderLeft;
        var borderBottom = styleRect.Size.Y - innerRect.Size.Y - borderTop;

        RealT rad;

        // Top left.
        rad = Math.Min(borderTop, borderLeft);
        innerCornerRadius[0] = Math.Max(cornerRadius[0] - rad, 0);

        // Top right;
        rad = Math.Min(borderTop, borderRight);
        innerCornerRadius[1] = Math.Max(cornerRadius[1] - rad, 0);

        // Bottom right.
        rad = Math.Min(borderBottom, borderRight);
        innerCornerRadius[2] = Math.Max(cornerRadius[2] - rad, 0);

        // Bottom left.
        rad = Math.Min(borderBottom, borderLeft);
        innerCornerRadius[3] = Math.Max(cornerRadius[3] - rad, 0);
    }

    #endregion private static methods

    #region public methods
    public void SetBorderWidth(Side side, int width)
    {
        if (ERR_FAIL_INDEX((int)side, 4))
        {
            return;
        }

        this.borderWidth[(int)side] = width;

        this.EmitChanged();
    }

    public void SetBorderWidthAll(int size)
    {
        this.borderWidth[0] = size;
        this.borderWidth[1] = size;
        this.borderWidth[2] = size;
        this.borderWidth[3] = size;

        this.EmitChanged();
    }

    public void SetCornerRadius(Corner corner, int radius)
    {
        if (ERR_FAIL_INDEX((int)corner, 4))
        {
            return;
        }

        this.cornerRadius[(int)corner] = radius;

        this.EmitChanged();
    }

    public void SetCornerRadiusAll(int radius)
    {
        for (var i = 0; i < 4; i++)
        {
            this.cornerRadius[i] = radius;
        }

        this.EmitChanged();
    }

    public void SetExpandMargin(Side side, float size)
    {
        if (ERR_FAIL_INDEX((int)side, 4))
        {
            return;
        }

        this.expandMargin[(int)side] = size;

        this.EmitChanged();
    }

    public void SetExpandMarginAll(float expandMarginSize)
    {
        for (var i = 0; i < 4; i++)
        {
            this.expandMargin[i] = expandMarginSize;
        }

        this.EmitChanged();
    }
    #endregion public methods

    #region protected overrided methods
    public override void Draw(Guid canvasItemId, in Rect2<RealT> rect)
    {
        var drawBorder = this.borderWidth[0] > 0 || this.borderWidth[1] > 0 || this.borderWidth[2] > 0 || this.borderWidth[3] > 0;
        var drawShadow = this.shadowSize > 0;
        if (!drawBorder && !this.drawCenter && !drawShadow)
        {
            return;
        }

        var styleRect = rect.GrowIndividual(
            this.expandMargin[(int)Side.SIDE_LEFT],
            this.expandMargin[(int)Side.SIDE_TOP],
            this.expandMargin[(int)Side.SIDE_RIGHT],
            this.expandMargin[(int)Side.SIDE_BOTTOM]
        );

        if (MathX.IsZeroApprox(styleRect.Size.X) || MathX.IsZeroApprox(styleRect.Size.Y))
        {
            return;
        }

        var roundedCorners = this.cornerRadius[0] > 0 || this.cornerRadius[1] > 0 || this.cornerRadius[2] > 0 || this.cornerRadius[3] > 0;
        // Only enable antialiasing if it is actually needed. This improve performances
        // and maximizes sharpness for non-skewed StyleBoxes with sharp corners.
        var aaOn = (roundedCorners || !this.skew.IsZeroApprox) && this.antiAliased;

        var blendOn = this.blendBorder && drawBorder;

        var borderColorAlpha = new Color(this.borderColor.R, this.borderColor.G, this.borderColor.B, 0);
        var borderColorBlend = this.drawCenter ? this.bgColor : borderColorAlpha;
        var borderColorInner = blendOn ? borderColorBlend : this.BorderColor;

        // Adapt borders (prevent weird overlapping/glitchy drawings).
        var width         = Math.Max(styleRect.Size.X, 0);
        var height        = Math.Max(styleRect.Size.Y, 0);
        var adaptedBorder = new[] { (RealT)1000000.0, (RealT)1000000.0, (RealT)1000000.0, (RealT)1000000.0 };

        AdaptValues(
            (int)Side.SIDE_TOP,
            (int)Side.SIDE_BOTTOM,
            adaptedBorder,
            this.borderWidth,
            height,
            height,
            height
        );

        AdaptValues(
            (int)Side.SIDE_LEFT,
            (int)Side.SIDE_RIGHT,
            adaptedBorder,
            this.borderWidth,
            width,
            width,
            width
        );

        // Adapt corners (prevent weird overlapping/glitchy drawings).
        var adaptedCorner = new[] { (RealT)1000000.0, (RealT)1000000.0, (RealT)1000000.0, (RealT)1000000.0 };

        AdaptValues(
            (int)Corner.CORNER_TOP_RIGHT,
            (int)Corner.CORNER_BOTTOM_RIGHT,
            adaptedCorner,
            this.cornerRadius,
            height,
            height - adaptedBorder[(int)Side.SIDE_BOTTOM],
            height - adaptedBorder[(int)Side.SIDE_TOP]
        );

        AdaptValues(
            (int)Corner.CORNER_TOP_LEFT,
            (int)Corner.CORNER_BOTTOM_LEFT,
            adaptedCorner,
            this.cornerRadius,
            height,
            height - adaptedBorder[(int)Side.SIDE_BOTTOM],
            height - adaptedBorder[(int)Side.SIDE_TOP]
        );

        AdaptValues(
            (int)Corner.CORNER_TOP_LEFT,
            (int)Corner.CORNER_TOP_RIGHT,
            adaptedCorner,
            this.cornerRadius,
            width,
            width - adaptedBorder[(int)Side.SIDE_RIGHT],
            width - adaptedBorder[(int)Side.SIDE_LEFT]
        );

        AdaptValues(
            (int)Corner.CORNER_BOTTOM_LEFT,
            (int)Corner.CORNER_BOTTOM_RIGHT,
            adaptedCorner,
            this.cornerRadius,
            width,
            width - adaptedBorder[(int)Side.SIDE_RIGHT],
            width - adaptedBorder[(int)Side.SIDE_LEFT]
        );

        var infillRect = styleRect.GrowIndividual(
            -adaptedBorder[(int)Side.SIDE_LEFT],
            -adaptedBorder[(int)Side.SIDE_TOP],
            -adaptedBorder[(int)Side.SIDE_RIGHT],
            -adaptedBorder[(int)Side.SIDE_BOTTOM]
        );

        var borderStyleRect = styleRect;
        if (aaOn)
        {
            for (var i = 0; i < 4; i++)
            {
                if (this.borderWidth[i] > 0)
                {
                    borderStyleRect = borderStyleRect.GrowSide((Side)i, this.aaSize);
                }
            }
        }

        var verts   = new List<Vector2<RealT>>();
        var indices = new List<int>();
        var colors  = new List<Color>();
        var uvs     = new List<Vector2<RealT>>();

        // Create shadow
        if (drawShadow)
        {
            var shadowInnerRect = styleRect;
            shadowInnerRect.Position += this.shadowOffset;

            var shadowRect = styleRect.Grow(this.shadowSize);
            shadowRect.Position += this.shadowOffset;

            var shadowColorTransparent = new Color(this.shadowColor.R, this.shadowColor.G, this.shadowColor.B, 0);

            DrawRing(
                verts,
                indices,
                colors,
                shadowInnerRect,
                adaptedCorner,
                shadowRect,
                shadowInnerRect,
                this.shadowColor,
                shadowColorTransparent,
                this.cornerDetail,
                this.skew
            );

            if (this.drawCenter)
            {
                DrawRing(
                    verts,
                    indices,
                    colors,
                    shadowInnerRect,
                    adaptedCorner,
                    shadowInnerRect,
                    shadowInnerRect,
                    this.shadowColor,
                    this.shadowColor,
                    this.cornerDetail,
                    this.skew,
                    true
                );
            }
        }

        // Create border (no AA).
        if (drawBorder && !aaOn)
        {
            DrawRing(
                verts,
                indices,
                colors,
                borderStyleRect,
                adaptedCorner,
                borderStyleRect,
                infillRect,
                borderColorInner,
                this.borderColor,
                this.cornerDetail,
                this.skew
            );
        }

        // Create infill (no AA).
        if (this.drawCenter && (!aaOn || blendOn || !drawBorder))
        {
            DrawRing(
                verts,
                indices,
                colors,
                borderStyleRect,
                adaptedCorner,
                infillRect,
                infillRect,
                this.BgColor,
                this.BgColor,
                this.CornerDetail,
                this.skew,
                true
            );
        }

        if (aaOn)
        {
            var aaBorderWidth = new RealT[4];
            var aaFillWidth   = new RealT[4];
            if (drawBorder)
            {
                for (var i = 0; i < 4; i++)
                {
                    if (this.borderWidth[i] > 0)
                    {
                        aaBorderWidth[i] = this.aaSize;
                        aaFillWidth[i] = 0;
                    }
                    else
                    {
                        aaBorderWidth[i] = 0;
                        aaFillWidth[i] = this.aaSize;
                    }
                }
            }
            else
            {
                for (var i = 0; i < 4; i++)
                {
                    aaBorderWidth[i] = 0;
                    aaFillWidth[i] = this.aaSize;
                }
            }

            var infillInnerRect = infillRect.GrowIndividual(
                -aaBorderWidth[(int)Side.SIDE_LEFT],
                -aaBorderWidth[(int)Side.SIDE_TOP],
                -aaBorderWidth[(int)Side.SIDE_RIGHT],
                -aaBorderWidth[(int)Side.SIDE_BOTTOM]
            );

            if (this.drawCenter)
            {
                if (!blendOn && drawBorder)
                {
                    var infillInnerRectAA = infillInnerRect.GrowIndividual(
                        aaBorderWidth[(int)Side.SIDE_LEFT],
                        aaBorderWidth[(int)Side.SIDE_TOP],
                        aaBorderWidth[(int)Side.SIDE_RIGHT],
                        aaBorderWidth[(int)Side.SIDE_BOTTOM]
                    );
                    // Create infill within AA border.
                    DrawRing(
                        verts,
                        indices,
                        colors,
                        borderStyleRect,
                        adaptedCorner,
                        infillInnerRectAA,
                        infillInnerRectAA,
                        this.bgColor,
                        this.bgColor,
                        this.cornerDetail,
                        this.skew,
                        true
                    );
                }

                if (!blendOn || !drawBorder)
                {
                    var infillRect_aa = infillRect.GrowIndividual(
                        aaFillWidth[(int)Side.SIDE_LEFT],
                        aaFillWidth[(int)Side.SIDE_TOP],
                        aaFillWidth[(int)Side.SIDE_RIGHT],
                        aaFillWidth[(int)Side.SIDE_BOTTOM]
                    );

                    var alphaBg = new Color(this.bgColor.R, this.bgColor.G, this.bgColor.B, 0);

                    // Create infill fake AA gradient.
                    DrawRing(
                        verts,
                        indices,
                        colors,
                        styleRect,
                        adaptedCorner,
                        infillRect_aa,
                        infillRect,
                        this.bgColor,
                        alphaBg,
                        this.cornerDetail,
                        this.skew
                    );
                }
            }

            if (drawBorder)
            {
                var infillRect_aa = infillRect.GrowIndividual(
                    aaBorderWidth[(int)Side.SIDE_LEFT],
                    aaBorderWidth[(int)Side.SIDE_TOP],
                    aaBorderWidth[(int)Side.SIDE_RIGHT],
                    aaBorderWidth[(int)Side.SIDE_BOTTOM]
                );

                var styleRectAa = styleRect.GrowIndividual(
                    aaBorderWidth[(int)Side.SIDE_LEFT],
                    aaBorderWidth[(int)Side.SIDE_TOP],
                    aaBorderWidth[(int)Side.SIDE_RIGHT],
                    aaBorderWidth[(int)Side.SIDE_BOTTOM]
                );

                var borderStyleRectAA = borderStyleRect.GrowIndividual(
                    aaBorderWidth[(int)Side.SIDE_LEFT],
                    aaBorderWidth[(int)Side.SIDE_TOP],
                    aaBorderWidth[(int)Side.SIDE_RIGHT],
                    aaBorderWidth[(int)Side.SIDE_BOTTOM]
                );

                // Create border.
                DrawRing(
                    verts,
                    indices,
                    colors,
                    borderStyleRect,
                    adaptedCorner,
                    borderStyleRectAA,
                    blendOn ? infillRect : infillRect_aa,
                    borderColorInner,
                    this.borderColor,
                    this.cornerDetail,
                    this.skew
                );

                if (!blendOn)
                {
                    // Create inner border fake AA gradient.
                    DrawRing(
                        verts,
                        indices,
                        colors,
                        borderStyleRect,
                        adaptedCorner,
                        infillRect_aa,
                        infillRect,
                        borderColorBlend,
                        this.borderColor,
                        this.cornerDetail,
                        this.skew
                    );
                }

                // Create outer border fake AA gradient.
                DrawRing(
                    verts,
                    indices,
                    colors,
                    borderStyleRect,
                    adaptedCorner,
                    styleRectAa,
                    borderStyleRectAA,
                    this.borderColor,
                    borderColorAlpha,
                    this.cornerDetail,
                    this.skew
                );
            }
        }

        // Compute UV coordinates.
        var uvRect = styleRect.Grow(aaOn ? this.aaSize : 0);

        uvs.Capacity = verts.Count;

        for (var i = verts.Count - 1; i >= 0; i--)
        {
            uvs.Add(
                new(
                    (verts[i].X - uvRect.Position.X) / uvRect.Size.X,
                    (verts[i].Y - uvRect.Position.Y) / uvRect.Size.Y
                )
            );
        }

        // Draw stylebox.
        RS.Singleton.CanvasItemAddTriangleArray(canvasItemId, indices, verts, colors, uvs);
    }
    #endregion protected overrided methods
}
