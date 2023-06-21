namespace Godot.Net.Core.Math;

using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

// TODO - Analyze whether there is real gain when using generic types in core structures
[DebuggerDisplay("\\{ Basis: {Basis}, Origin: {Origin} \\}")]
public record struct Transform3D<T> where T : notnull, INumber<T>, IRootFunctions<T>
{
    private Vector3<T> origin = new();
    private Basis<T>   basis = new();

    public Basis<T> Basis
    {
        readonly get => this.basis;
        set          => this.basis = value;
    }

    public Vector3<T> Origin
    {
        readonly get => this.origin;
        set          => this.origin = value;
    }

    public Transform3D() { }

    public Transform3D(Basis<T> basis, Vector3<T> origin)
    {
        this.Basis  = basis;
        this.Origin = origin;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private readonly Plane<T> XFormFast(Plane<T> plane, Basis<T> basisInverseTranspose)
    {
        var point    = this.XForm(plane.Normal * plane.Distance);
        var normal   = basisInverseTranspose.XForm(plane.Normal).Normalized;
        var distance = normal.Dot(point);

        return new Plane<T>(normal, distance);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Transform3D<T> Inverse() => this with
    {
        origin = this.basis.Transposed().XForm(-this.Origin)
    };

    public void Scale(Vector3<T> scale)
    {
        this.basis.Scale(scale);
        this.origin *= scale;
    }

    public void TranslateLocal(T tx, T ty, T tz) => this.TranslateLocal(new(tx, ty, tz));

    public void TranslateLocal(Vector3<T> translation)
    {
        for (var i = 0; i < 3; i++)
        {
            this.origin[i] += this.Basis[i].Dot(translation);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Plane<T> XForm(Plane<T> plane)
    {
        var basis = this.basis.Inverse().Transposed();

        return this.XFormFast(plane, basis);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Vector3<T> XForm(Vector3<T> vector)
    {
        var origin = this.origin;
        var basis  = this.basis;
        return new(
            basis[0].Dot(vector) + origin.X,
            basis[1].Dot(vector) + origin.Y,
            basis[2].Dot(vector) + origin.Z
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Transform3D<T> operator *(Transform3D<T> left, Transform3D<T> right) => throw new NotImplementedException();
}
