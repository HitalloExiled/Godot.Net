namespace Godot.Net.Core.Math;

using System.Numerics;
using System.Runtime.CompilerServices;

public record struct Plane<T> where T : notnull, INumber<T>, IRootFunctions<T>
{
    public Vector3<T> Normal   { get; set; }
    public T          Distance { get; set; } = T.Zero;

    public Plane() { }
    public Plane(T a, T b, T c, T d) : this(new Vector3<T>(a, b, c), d) { }
    public Plane(Vector3<T> normal, T distance) => (this.Normal, this.Distance) = (normal, distance);
    public Plane(Vector3<T> normal, Vector3<T> point) : this(normal, normal.Dot(point)) { }
    public Plane(Vector3<T> point1, Vector3<T> point2, Vector3<T> point3) => throw new NotImplementedException();

    public readonly Vector3<T> Center
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        get => this.Normal * this.Distance;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly T DistanceTo(Vector3<T> point) =>
        this.Normal.Dot(point) - this.Distance;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Vector3<T>? Intersect(Plane<T> plane1, Plane<T> plane2)
    {
        var normal0 = this.Normal;
        var normal1 = plane1.Normal;
        var normal2 = plane2.Normal;

        var denom = normal0.Cross(normal1).Dot(normal2);

        return MathX.IsZeroApprox((RealT)(object)denom)
            ? null
            : (normal1.Cross(normal2) * this.Distance + normal2.Cross(normal0) * plane1.Distance + normal0.Cross(normal1) * plane2.Distance) / denom;
    }
}
