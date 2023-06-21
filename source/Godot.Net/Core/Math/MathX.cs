namespace Godot.Net.Core.Math;

using System.Numerics;
using System.Runtime.CompilerServices;
using MathS = System.Math;


public static class MathX
{
    public const RealT CMP_EPSILON = 0.00001f;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T DegToRad<T>(T y) where T : notnull, INumber<T> => y * T.CreateChecked(MathS.PI / 180.0);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool IsEqualApprox<T>(T a, T b) where T : notnull, INumber<T>
    {
        // Check for exact equality first, required to handle "infinity" values.
        if (a == b)
        {
            return true;
        }

        var cmpEpsilon = T.CreateSaturating(CMP_EPSILON);

        // Then check for approximate equality.
        var tolerance = cmpEpsilon * T.Abs(a);

        if (tolerance < cmpEpsilon)
        {
            tolerance = cmpEpsilon;
        }

        return T.Abs(a - b) < tolerance;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static bool IsZeroApprox<T>(T value) where T : notnull, INumber<T> =>
        T.Abs(value) < T.CreateSaturating(CMP_EPSILON);

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T Lerp<T>(T from, T to, T weight) where T : notnull, INumber<T> =>
        from + (to - from) * weight;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T RadToDeg<T>(T radians) where T : notnull, INumber<T> =>
        radians * T.CreateChecked(180.0 / MathS.PI);
}
