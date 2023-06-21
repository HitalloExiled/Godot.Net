#define MATH_CHECKS

namespace Godot.Net.Core.Math;

using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Godot.Net.Core.Enums;

[DebuggerDisplay("\\{ Position: {Position}, Size: {Size} \\}")]
public record struct Rect2<T> where T : notnull, INumber<T>
{
    public readonly Vector2<T> Center => this.Position + this.Size * T.CreateSaturating(0.5);
    public readonly Vector2<T> End    => this.Position + this.Size;

    public Vector2<T> Position { get; set; }
    public Vector2<T> Size     { get; set; }

    public Rect2()
    { }

    public Rect2(T x, T y, T width, T height) : this(new(x, y), new(width, height))
    { }

    public Rect2(Vector2<T> position, Vector2<T> size)
    {
        this.Position = position;
        this.Size     = size;
    }

    public readonly Rect2<TOther> As<TOther>() where TOther : notnull, INumber<TOther> =>
        new (this.Position.As<TOther>(), this.Size.As<TOther>());

    // inline void expand_to(const Vector2 &vector)
    public void ExpandTo(Vector2<T> vector)
    {
        // In place function for speed.
        #if MATH_CHECKS
        if (this.Size.X < T.Zero || this.Size.Y < T.Zero)
        {
            ERR_PRINT("Rect2 size is negative, this is not supported. Use Rect2.abs() to get a Rect2 with a positive size.");
        }
        #endif
        var begin = this.Position;
        var end   = this.Position + this.Size;

        if (vector.X < begin.X)
        {
            begin.X = vector.X;
        }

        if (vector.Y < begin.Y)
        {
            begin.Y = vector.Y;
        }

        if (vector.X > end.X)
        {
            end.X = vector.X;
        }

        if (vector.Y > end.Y)
        {
            end.Y = vector.Y;
        }

        this.Position = begin;
        this.Size     = end - begin;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Rect2<T> GrowIndividual(T left, T top, T right, T bottom) =>
        new(
            new(this.Position.X - left, this.Position.Y - top),
            new(this.Size.X + left + right, this.Size.Y + top + bottom)
        );

    public readonly bool HasPoint(Vector2<T> point)
    {
        #if MATH_CHECKS
        if (this.Size.X < T.Zero || this.Size.Y < T.Zero)
        {
            ERR_PRINT("Rect2i size is negative, this is not supported. Use Rect2i.abs() to get a Rect2i with a positive size.");
        }
        #endif

        return point.X >= this.Position.X && point.Y >= this.Position.Y && point.X < this.Position.X + this.Size.X && point.Y < this.Position.Y + this.Size.Y;
    }

    public readonly Rect2<T> Intersection(Rect2<T> rect)
    {
        var newRect = rect;

        if (!this.Intersects(newRect))
        {
            return new();
        }

        newRect.Position = new(T.Max(rect.Position.X, this.Position.X), T.Max(rect.Position.Y, this.Position.Y));

        var rectEnd = rect.Position + rect.Size;
        var end     = this.Position + this.Size;

        newRect.Size = new(T.Min(rectEnd.X, end.X) - newRect.Position.X, T.Min(rectEnd.Y, end.Y) - newRect.Position.Y);

        return newRect;
    }

    public readonly bool Intersects(Rect2<T> rect, bool includeBorders = false)
    {
        #if MATH_CHECKS
        if (this.Size.X < T.Zero || this.Size.Y < T.Zero || rect.Size.X < T.Zero || rect.Size.Y < T.Zero)
        {
            ERR_PRINT("Rect2 size is negative, this is not supported. Use Rect2i.abs() to get a Rect2i with a positive size.");
        }
        #endif

        if (includeBorders)
        {
            if (this.Position.X > rect.Position.X + rect.Size.Y)
            {
                return false;
            }
            if (this.Position.X + this.Size.X < rect.Position.X)
            {
                return false;
            }
            if (this.Position.Y > rect.Position.Y + rect.Size.Y)
            {
                return false;
            }
            if (this.Position.Y + this.Size.Y < rect.Position.Y)
            {
                return false;
            }
        }
        else
        {
            if (this.Position.X >= rect.Position.X + rect.Size.X)
            {
                return false;
            }
            if (this.Position.X + this.Size.X <= rect.Position.X)
            {
                return false;
            }
            if (this.Position.Y >= rect.Position.Y + rect.Size.Y)
            {
                return false;
            }
            if (this.Position.Y + this.Size.Y <= rect.Position.Y)
            {
                return false;
            }
        }

        return true;
    }

    public readonly bool IntersectsTransformed(Transform2D<T> xformCache, Rect2<T> aabbCache) => throw new NotImplementedException();
    public readonly Rect2<T> Merge(Rect2<T> rect2) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Rect2<T> Grow(T amount)
    {
        var g = this;

        g.GrowBy(amount);

        return g;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public void GrowBy(T amount)
    {
        var two = T.CreateSaturating(2);

        this.Position = new(this.Position.X - amount, this.Position.Y - amount);
        this.Size     = new(this.Size.X + amount * two, this.Size.Y + amount * two);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Rect2<T> GrowSide(Side side, T amount) =>
        this.GrowIndividual(
            (Side.SIDE_LEFT   == side) ? amount : T.Zero,
            (Side.SIDE_TOP    == side) ? amount : T.Zero,
            (Side.SIDE_RIGHT  == side) ? amount : T.Zero,
            (Side.SIDE_BOTTOM == side) ? amount : T.Zero
        );
}
