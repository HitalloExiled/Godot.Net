// TODO - Analyze whether there is real gain when using generic types in core structures

namespace Godot.Net.Core.Math;

using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

[DebuggerDisplay("[{row0}, {row1}, {row2}]")]
public record struct Basis<T> where T : notnull, INumber<T>, IRootFunctions<T>
{
    private Vector3<T> row0 = new(T.One, T.Zero, T.Zero);
    private Vector3<T> row1 = new(T.Zero, T.One, T.Zero);
    private Vector3<T> row2 = new(T.Zero, T.Zero, T.One);

    public Vector3<T> this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        readonly get => index switch
        {
            0 => this.row0,
            1 => this.row1,
            2 => this.row2,
            _ => throw new IndexOutOfRangeException(),
        };
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        set
        {
            switch (index)
            {
                case 0: this.row0 = value; break;
                case 1: this.row1 = value; break;
                case 2: this.row2 = value; break;
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
                case 0: this.row0[y] = value; break;
                case 1: this.row1[y] = value; break;
                case 2: this.row2[y] = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public Basis() { }

    public Basis(Vector3<T> x, Vector3<T> y, Vector3<T> z)
    {
        this.row0 = x;
        this.row1 = y;
        this.row2 = z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Basis<T> Inverse()
    {
        var rows = new[]
        {
            this.row0,
            this.row1,
            this.row2,
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        T factory(int row1, int col1, int row2, int col2) =>
            rows[row1][col1] * rows[row2][col2] - rows[row1][col2] * rows[row2][col1];

        var co = new[] { factory(1, 1, 2, 2), factory(1, 2, 2, 0), factory(1, 0, 2, 1) };
        var det =
            this[0, 0] * co[0] +
            this[0, 1] * co[1] +
            this[0, 2] * co[2];

        // #ifdef MATH_CHECKS
        //     ERR_FAIL_COND(det == 0);
        // #endif

        var s = (T)(object)1.0f / det;

        return new
        (
            new Vector3<T>(co[0] * s, factory(0, 2, 2, 1) * s, factory(0, 1, 1, 2) * s),
            new Vector3<T>(co[1] * s, factory(0, 0, 2, 2) * s, factory(0, 2, 1, 0) * s),
            new Vector3<T>(co[2] * s, factory(0, 1, 2, 0) * s, factory(0, 0, 1, 1) * s)
        );
    }

    public void Scale(Vector3<T> scale)
    {
        this[0, 0] *= scale.X;
        this[0, 1] *= scale.X;
        this[0, 2] *= scale.X;
        this[1, 0] *= scale.Y;
        this[1, 1] *= scale.Y;
        this[1, 2] *= scale.Y;
        this[2, 0] *= scale.Z;
        this[2, 1] *= scale.Z;
        this[2, 2] *= scale.Z;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Basis<T> Transposed()
    {
        var rows = new[,]
        {
            { this[0, 0], this[0, 1], this[0, 2] },
            { this[1, 0], this[1, 1], this[1, 2] },
            { this[2, 0], this[2, 1], this[2, 2] },
        };

        (rows[0, 1], rows[1, 0]) = (rows[1, 0], rows[0, 1]);
        (rows[0, 2], rows[2, 0]) = (rows[2, 0], rows[0, 2]);
        (rows[1, 2], rows[2, 1]) = (rows[2, 1], rows[1, 2]);

        return new(
            new Vector3<T>(rows[0, 0], rows[0, 1], rows[0, 2]),
            new Vector3<T>(rows[1, 0], rows[1, 1], rows[1, 2]),
            new Vector3<T>(rows[2, 0], rows[2, 1], rows[2, 2])
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public readonly Vector3<T> XForm(Vector3<T> vector) =>
        new(
            this.row0.Dot(vector),
            this.row1.Dot(vector),
            this.row2.Dot(vector)
        );
}
