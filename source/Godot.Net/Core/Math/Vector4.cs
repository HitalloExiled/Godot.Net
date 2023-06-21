namespace Godot.Net.Core.Math;

using System.Numerics;
using System.Runtime.CompilerServices;

public record struct Vector4<T> where T : notnull, INumber<T>
{
    public T X { get; set; } = T.Zero;
    public T Y { get; set; } = T.Zero;
    public T Z { get; set; } = T.Zero;
    public T W { get; set; } = T.Zero;

    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        readonly get => index switch
        {
            0 => this.X,
            1 => this.Y,
            2 => this.Z,
            3 => this.W,
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
                case 3: this.W = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public Vector4() { }

    public Vector4(T x, T y, T z, T w)
    {
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.W = w;
    }

    public Vector4(Vector3<T> vector, T w) : this(vector.X, vector.Y, vector.Z, w) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly T Dot(Vector4<T> other) =>
        this.X * other.X + this.Y * other.X + this.Z * other.X;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector4<T> operator +(Vector4<T> left, Vector4<T> right) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector4<T> operator -(Vector4<T> left, Vector4<T> right) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector4<T> operator -(Vector4<T> value) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector4<T> operator *(Vector4<T> left, Vector4<T> right) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector4<T> operator /(Vector4<T> left, Vector4<T> right) => throw new NotImplementedException();
}
