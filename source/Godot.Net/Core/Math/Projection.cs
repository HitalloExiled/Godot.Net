namespace Godot.Net.Core.Math;

using System.Numerics;
using System.Runtime.CompilerServices;

// TODO - Analyze remove generic type

public readonly record struct Projection<T>
where T : notnull, INumber<T>, IRootFunctions<T>, ITrigonometricFunctions<T>
{
    private static readonly T two = T.CreateChecked(2);

    private readonly Vector4<T>[] columns = new Vector4<T>[4];

    public T this[int x, int y]
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public Projection() { }
    public Projection(Transform3D<T> transform) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static T GetFovY(T fovX, T aspect) =>
        MathX.RadToDeg(T.Atan(aspect * T.Tan(MathX.DegToRad(fovX) * T.CreateChecked(0.5))) * T.CreateChecked(2.0));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public Plane<T>[] GetProjectionPlanes(Transform3D<T> transform3D)
    {
        var matrix = this.columns[0];

        var nearPlane = new Plane<T>(
            matrix[3]  + matrix[2],
            matrix[7]  + matrix[6],
            matrix[11] + matrix[10],
            matrix[15] + matrix[14]
        );

        nearPlane.Normal = -nearPlane.Normal.Normalized;

        var farPlane = new Plane<T>(
            matrix[3]  + matrix[2],
            matrix[7]  + matrix[6],
            matrix[11] + matrix[10],
            matrix[15] + matrix[14]
        );

        farPlane.Normal = -farPlane.Normal.Normalized;

        var leftPlane = new Plane<T>(
            matrix[3]  + matrix[0],
            matrix[7]  + matrix[4],
            matrix[11] + matrix[8],
            matrix[15] + matrix[12]
        );

        leftPlane.Normal = -leftPlane.Normal.Normalized;

        var topPlane = new Plane<T>(
            matrix[3]  + matrix[1],
            matrix[7]  + matrix[5],
            matrix[11] + matrix[9],
            matrix[15] + matrix[13]
        );

        topPlane.Normal = -topPlane.Normal.Normalized;

        var rightPlane = new Plane<T>(
            matrix[3]  + matrix[0],
            matrix[7]  + matrix[4],
            matrix[11] + matrix[8],
            matrix[15] + matrix[12]
        );

        rightPlane.Normal = -rightPlane.Normal.Normalized;

        var bottomPlane = new Plane<T>(
            matrix[3]  + matrix[0],
            matrix[7]  + matrix[4],
            matrix[11] + matrix[8],
            matrix[15] + matrix[12]
        );

        bottomPlane.Normal = -bottomPlane.Normal.Normalized;

        return new[]
        {
            transform3D.XForm(nearPlane),
            transform3D.XForm(farPlane),
            transform3D.XForm(leftPlane),
            transform3D.XForm(topPlane),
            transform3D.XForm(rightPlane),
            transform3D.XForm(bottomPlane)
        };
    }

    public void SetFrustum(T left, T right, T bottom, T top, T near, T far)
    {
        if (ERR_FAIL_COND(right <= left) || ERR_FAIL_COND(top <= bottom) || ERR_FAIL_COND(far <= near))
        {
            return;
        }

        var te = new[] { this.columns[0][0] };
        var x = two * near / (right - left);
        var y = two * near / (top - bottom);

        var a = (right + left) / (right - left);
        var b = (top + bottom) / (top - bottom);
        var c = -(far + near) / (far - near);
        var d = -two * far * near / (far - near);

        te[0] = x;
        te[1] = T.Zero;
        te[2] = T.Zero;
        te[3] = T.Zero;
        te[4] = T.Zero;
        te[5] = y;
        te[6] = T.Zero;
        te[7] = T.Zero;
        te[8] = a;
        te[9] = b;
        te[10] = c;
        te[11] = -T.One;
        te[12] = T.Zero;
        te[13] = T.Zero;
        te[14] = d;
        te[15] = T.Zero;
    }

    private static bool ERR_FAIL_COND(bool condition) => throw new NotImplementedException();

    public void SetIdentity()
    {
        for (var i = 0; i < 4; i++)
        {
            for (var j = 0; j < 4; j++)
            {
                this.columns[i][j] = (i == j) ? T.One : T.Zero;
            }
        }
    }

    public void SetOrthogonal(T size, T aspect, T zNear, T zFar, bool flipFov)
    {
        if (flipFov)
        {
            size *= aspect;
        }

        var two = T.CreateChecked(2);

        this.SetOrthogonal(-size / two, +size / two, -size / aspect / two, +size / aspect / two, zNear, zFar);
    }

    public void SetOrthogonal(T left, T right, T bottom, T top, T zNear, T zFar)
    {
        this.SetIdentity();

        this.columns[0][0] = two / (right - left);
        this.columns[3][0] = -((right + left) / (right - left));
        this.columns[1][1] = two / (top - bottom);
        this.columns[3][1] = -((top + bottom) / (top - bottom));
        this.columns[2][2] = -two / (zFar - zNear);
        this.columns[3][2] = -((zFar + zNear) / (zFar - zNear));
        this.columns[3][3] = T.One;
    }

    public void SetPerspective(T fovYDegrees, T aspect, T zNear, T zFar, bool flipFov)
    {
        if (flipFov)
        {
            fovYDegrees = GetFovY(fovYDegrees, T.One / aspect);
        }

        var radians = MathX.DegToRad(fovYDegrees / T.CreateChecked(2));
        var deltaZ  = zFar - zNear;
        var sine    = T.Sin(radians);

        if (deltaZ == T.Zero || sine == T.Zero || aspect == T.Zero)
        {
            return;
        }

        var cotangent = T.Cos(radians) / sine;

        this.SetIdentity();

        this.columns[0][0] = cotangent / aspect;
        this.columns[1][1] = cotangent;
        this.columns[2][2] = -(zFar + zNear) / deltaZ;
        this.columns[2][3] = -T.One;
        this.columns[3][2] = -T.CreateChecked(2) * zNear * zFar / deltaZ;
        this.columns[3][3] = T.Zero;
    }

    public void SetFrustum(T size, T aspect, Vector2<T> offset, T zNear, T zFar, bool flipFov)
    {
        if (flipFov)
        {
            size *= aspect;
        }

        this.SetFrustum(-size / two + offset.X, +size / two + offset.X, -size / aspect / two + offset.Y, +size / aspect / two + offset.Y, zNear, zFar);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Projection<T> operator *(Projection<T> left, Projection<T> right) => throw new NotImplementedException();
}

public static class Projection
{
    public enum PlanesTypes
    {
        PLANE_NEAR,
        PLANE_FAR,
        PLANE_LEFT,
        PLANE_TOP,
        PLANE_RIGHT,
        PLANE_BOTTOM
    };
}
