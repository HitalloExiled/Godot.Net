#define MATH_CHECKS

namespace Godot.Net.Core.Math;

using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

// TODO - Analyze whether there is real gain when using generic types in core structures
[DebuggerDisplay("\\{ X: {x}, Y: {y}, Origin: {origin} \\}")]
public record struct Transform2D<T> where T : notnull, INumber<T>
{
    private Vector2<T> x = new(T.One, T.Zero);
    private Vector2<T> y = new(T.Zero, T.One);
    private Vector2<T> origin;

    public Vector2<T> this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        readonly get => index switch
        {
            0 => this.x,
            1 => this.y,
            2 => this.origin,
            _ => throw new IndexOutOfRangeException(),
        };
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        set
        {
            switch (index)
            {
                case 0: this.x      = value; break;
                case 1: this.y      = value; break;
                case 2: this.origin = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public T this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        readonly get => this[x][y];
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        set
        {
            switch (x)
            {
                case 0: this.x[y]      = value; break;
                case 1: this.y[y]      = value; break;
                case 2: this.origin[y] = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public Vector2<T> Origin
    {
        readonly get => this.origin;
        set          => this.origin = value;
    }

    public Transform2D(T xx, T xy, T yx, T yy, T ox, T oy)
    {
		this[0, 0] = xx;
		this[0, 1] = xy;
		this[1, 0] = yx;
		this[1, 1] = yy;
		this[2, 0] = ox;
		this[2, 1] = oy;
	}

    public Transform2D(T rotation, Vector2<T> position)
    {
        var rot = RealT.CreateSaturating(rotation);
        var cr =  T.CreateSaturating(RealT.Cos(rot));
        var sr =  T.CreateSaturating(RealT.Sin(rot));

        this[0, 0] = cr;
        this[0, 1] = sr;
        this[1, 0] = -sr;
        this[1, 1] = cr;
        this[2]    = position;
    }

    public Transform2D() { }

    public Transform2D(Vector2<T> column0, Vector2<T> column1, Vector2<T> column2)
    {
        this.x      = column0;
        this.y      = column1;
        this.origin = column2;
    }

    private void ScaleBasis(Vector2<T> scale)
    {
        this[0, 0] *= scale.X;
        this[0, 1] *= scale.Y;
        this[1, 0] *= scale.X;
        this[1, 1] *= scale.Y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private readonly T TdotX(Vector2<T> vector) =>
        this[0, 0] * vector.X + this[1, 0] * vector.Y;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private readonly T TdotY(Vector2<T> vector) =>
        this[0, 1] * vector.X + this[1, 1] * vector.Y;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void Scale(Vector2<T> scale)
    {
        this.ScaleBasis(scale);
        this[2] *= scale;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Transform2D<T> AffineInverse()
    {
        var inv = this;
        inv.AffineInvert();
        return inv;
    }

    public void AffineInvert()
    {
        var det = this.BasisDeterminant();
        #if MATH_CHECKS
        if (ERR_FAIL_COND(det == T.Zero))
        {
            return;
        }
        #endif
        var idet = T.One / det;

        (this[0, 0], this[1, 1]) = (this[1, 1], this[0, 0]);

        this.x *= new Vector2<T>(idet, -idet);
        this.y *= new Vector2<T>(-idet, idet);

        this.origin = this.BasisXform(-this.origin);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private readonly T BasisDeterminant() => this.x.X * this.y.Y - this.x.Y * this.y.X;
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Vector2<T> BasisXform(Vector2<T> vector) =>
        new(this.TdotX(vector), this.TdotY(vector));

    public readonly Vector2<T> GetScale()
    {
        var detSign = T.CreateSaturating(T.Sign(this.BasisDeterminant()));

        return new(T.CreateSaturating(this.x.Length), detSign * T.CreateSaturating(this.y.Length));
    }

    public void SetRotationAndScale(T rot, Vector2<T> scale)
    {
        var rotation = double.CreateSaturating(rot);
        var cos      = T.CreateSaturating(Math.Cos(rotation));
        var sin      = T.CreateSaturating(Math.Sin(rotation));

        this[0, 0] = cos  * scale.X;
        this[1, 1] = cos  * scale.Y;
        this[1, 0] = -sin * scale.Y;
        this[0, 1] = sin  * scale.X;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void TranslateLocal(Vector2<T> translation) =>
        this[2] += this.BasisXform(translation);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Vector2<T> Xform(Vector2<T> vector) =>
        new Vector2<T>(this.TdotX(vector), this.TdotY(vector)) + this[2];

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Rect2<T> Xform(Rect2<T> rect)
    {
        var x = this[0] * rect.Size.X;
        var y = this[1] * rect.Size.Y;

        var pos = this.Xform(rect.Position);

        var newRect = new Rect2<T>
        {
            Position = pos
        };

        newRect.ExpandTo(pos + x);
        newRect.ExpandTo(pos + y);
        newRect.ExpandTo(pos + x + y);

        return newRect;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Transform2D<T> operator *(Transform2D<T> left, Transform2D<T> right)
    {
        left[2] = left.Xform(right[2]);

        var x0 = left.TdotX(right[0]);
        var x1 = left.TdotY(right[0]);
        var y0 = left.TdotX(right[1]);
        var y1 = left.TdotY(right[1]);

        left[0, 0] = x0;
        left[0, 1] = x1;
        left[1, 0] = y0;
        left[1, 1] = y1;

        return left;
    }
}
