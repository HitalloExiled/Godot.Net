namespace Godot.Net.Core.Math;

using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Interfaces;
using MathS = System.Math;

[DebuggerDisplay("\\{ X: {X}, Y: {Y} \\}")]
public record struct Vector2<T> : IPoint2<T>, ISize2<T>
where T : notnull, INumber<T>
{
    public static Vector2<T> One  => new(T.One, T.One);
    public static Vector2<T> Zero => new(T.Zero, T.Zero);

    public T X { get; set; } = T.Zero;
    public T Y { get; set; } = T.Zero;

    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        readonly get => index switch
        {
            0 => this.X,
            1 => this.Y,
            _ => throw new IndexOutOfRangeException(),
        };
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        set
        {
            switch (index)
            {
                case 0: this.X = value; break;
                case 1: this.Y = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public readonly bool IsZeroApprox => MathX.IsZeroApprox(this.X) || MathX.IsZeroApprox(this.Y);

    public readonly T LengthSquared
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => this.Dot(this);
    }

    public readonly double Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => MathS.Sqrt(double.CreateChecked(this.LengthSquared));
    }

    public readonly Vector2<T> Normalized
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => T.CreateSaturating(this.Length) is T length && length > T.Zero ? this / length : Zero;
    }

    T ISize2<T>.Height
    {
        readonly get => this.Y;
        set          => this.Y = value;
    }

    T ISize2<T>.Width
    {
        readonly get => this.X;
        set          => this.X = value;
    }

    public Vector2() { }

    public Vector2(T x, T y)
    {
        this.X = x;
        this.Y = y;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Vector2<TOther> As<TOther>() where TOther : notnull, INumber<TOther> =>
        new(TOther.CreateSaturating(this.X), TOther.CreateSaturating(this.Y));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly T Aspect() => this.X / this.Y;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Vector2<T> Ceil() =>
        new(
            T.CreateSaturating(MathS.Ceiling(double.CreateChecked(this.X))),
            T.CreateSaturating(MathS.Ceiling(double.CreateChecked(this.Y)))
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Vector2<T> Clamp(Vector2<T> min, Vector2<T> max) =>
        new(T.Clamp(this.X, min.X, max.X), T.Clamp(this.Y, min.Y, max.Y));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly T Dot(Vector2<T> other) =>
        this.X * other.X + this.Y * other.Y;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Vector2<T> Floor() =>
        new(
            T.CreateSaturating(MathS.Floor(double.CreateChecked(this.X))),
            T.CreateSaturating(MathS.Floor(double.CreateChecked(this.Y)))
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Vector2<T> Min(Vector2<T> vector) =>
        new(T.Min(this.X, vector.X), T.Min(this.Y, vector.Y));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Vector2<T> Max(Vector2<T> vector) =>
        new(T.Max(this.X, vector.X), T.Max(this.Y, vector.Y));

    public void Normalize() => throw new NotImplementedException();

    public readonly Vector2<T> Round() =>
        new(
            T.CreateSaturating(double.Round(double.CreateSaturating(this.X))),
            T.CreateSaturating(double.Round(double.CreateSaturating(this.Y)))
        );

    public override readonly string ToString()
    {
        var builder = new StringBuilder();

        builder.Append("X = ");
        builder.Append(this.X);
        builder.Append(", Y = ");
        builder.Append(this.Y);
        builder.Append(", LengthSquared = ");
        builder.Append(this.LengthSquared);
        builder.Append(", Length = ");

        return builder.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2<T> operator +(Vector2<T> vector) => new(+vector.X, +vector.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2<T> operator +(Vector2<T> left, Vector2<T> right) => new(left.X + right.X, left.Y + right.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2<T> operator -(Vector2<T> vector) => new(-vector.X, -vector.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2<T> operator -(Vector2<T> left, Vector2<T> right) => new(left.X - right.X, left.Y - right.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2<T> operator *(Vector2<T> vector, T scalar) => new(vector.X * scalar, vector.Y * scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2<T> operator *(Vector2<T> left, Vector2<T> right) => new(left.X * right.X, left.Y * right.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2<T> operator /(Vector2<T> vector, T scalar) => new(vector.X / scalar, vector.Y / scalar);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector2<T> operator /(Vector2<T> left, Vector2<T> right) => new(left.X / right.X, left.Y / right.Y);
}
