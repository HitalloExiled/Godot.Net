namespace Godot.Net.Core.Math;

using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

[DebuggerDisplay("\\{ X: {X}, Y: {Y}, Z: {Z} \\}")]
public record struct Vector3<T> where T : notnull, INumber<T>
{
    public static Vector3<T> One =>  new(T.One, T.One, T.One);
    public static Vector3<T> Zero => new(T.Zero, T.Zero, T.Zero);

    public T X { get; set; } = T.Zero;
    public T Y { get; set; } = T.Zero;
    public T Z { get; set; } = T.Zero;

    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        readonly get => index switch
        {
            0 => this.X,
            1 => this.Y,
            2 => this.Z,
            _ => throw new IndexOutOfRangeException(),
        };
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        set
        {
            switch (index)
            {
                case 0: this.X = value; break;
                case 1: this.Y = value; break;
                case 2: this.Z = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public readonly T LengthSquared
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => this.Dot(this);
    }

    public readonly double Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => System.Math.Sqrt(double.CreateChecked(this.LengthSquared));
    }

    public readonly Vector3<T> Normalized
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => T.CreateSaturating(this.Length) is T length && length > T.Zero ? this / length : Zero;
    }

    public Vector3() { }
    public Vector3(T x, T y, T z)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Vector3<T> Cross(Vector3<T> vector) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly T Dot(Vector3<T> other) =>
        this.X * other.X + this.Y * other.Y + this.Z * other.Z;

    public void Normalize() => throw new NotImplementedException();

    public override readonly string ToString()
    {
        var builder = new StringBuilder();

        builder.Append("X = ");
        builder.Append(this.X);
        builder.Append(", Y = ");
        builder.Append(this.Y);
        builder.Append(", Z = ");
        builder.Append(this.Z);
        builder.Append(", LengthSquared = ");
        builder.Append(this.LengthSquared);
        builder.Append(", Length = ");

        return builder.ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator +(Vector3<T> vector, T value) =>
        new(vector.X + value, vector.Y + value, vector.Z + value);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator +(Vector3<T> left, Vector3<T> right) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator -(Vector3<T> left, Vector3<T> right) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator -(Vector3<T> value) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator *(Vector3<T> vector, T scalar) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator *(Vector3<T> left, Vector3<T> right) =>
        new(left.X * right.X, left.Y * right.Y, left.Z * right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator /(Vector3<T> left, T scalar) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector3<T> operator /(Vector3<T> left, Vector3<T> right) => throw new NotImplementedException();
}
