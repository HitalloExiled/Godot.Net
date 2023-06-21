namespace Godot.Net.ThirdParty.Misc;

public class OkColor
{
    public record struct Cs(float C0, float CMid, float CMax);
    public record struct HSL(float H, float S, float L);
    public record struct Lab(float L, float A, float B);
    public record struct RGB(float R, float G, float B);
    public record struct LC(float L, float C);
    public record struct ST(float S, float T);

    ///<summary>
    /// Finds the maximum saturation possible for a given hue that fits in sRGB
    /// Saturation here is defined as S = C/L
    /// a and b must be normalized so a^2 + b^2 == 1
    ///</summary>
    public static float ComputeMaxSaturation(float a, float b)
    {
        // Max saturation will be when one of r, g or b goes below zero.

        // Select different coefficients depending on which component goes below zero first
        float k0, k1, k2, k3, k4, wl, wm, ws;

        if (-1.88170328f * a - 0.80936493f * b > 1)
        {
            // Red component
            k0 = +1.19086277f; k1 = +1.76576728f; k2 = +0.59662641f; k3 = +0.75515197f; k4 = +0.56771245f;
            wl = +4.0767416621f; wm = -3.3077115913f; ws = +0.2309699292f;
        }
        else if (1.81444104f * a - 1.19445276f * b > 1)
        {
            // Green component
            k0 = +0.73956515f; k1 = -0.45954404f; k2 = +0.08285427f; k3 = +0.12541070f; k4 = +0.14503204f;
            wl = -1.2684380046f; wm = +2.6097574011f; ws = -0.3413193965f;
        }
        else
        {
            // Blue component
            k0 = +1.35733652f; k1 = -0.00915799f; k2 = -1.15130210f; k3 = -0.50559606f; k4 = +0.00692167f;
            wl = -0.0041960863f; wm = -0.7034186147f; ws = +1.7076147010f;
        }

        // Approximate max saturation using a polynomial:
        var uS = k0 + k1 * a + k2 * b + k3 * a * a + k4 * a * b;

        // Do one step Halley's method to get closer
        // this gives an error less than 10e6, except for some blue hues where the dS/dh is close to infinite
        // this should be sufficient for most applications, otherwise do two/three steps

        var kL = +0.3963377774f * a + 0.2158037573f * b;
        var kM = -0.1055613458f * a - 0.0638541728f * b;
        var kS = -0.0894841775f * a - 1.2914855480f * b;

        {
            var l_ = 1 + uS * kL;
            var m_ = 1 + uS * kM;
            var s_ = 1 + uS * kS;

            var l = l_ * l_ * l_;
            var m = m_ * m_ * m_;
            var s = s_ * s_ * s_;

            var lDs = 3 * kL * l_ * l_;
            var mDs = 3 * kM * m_ * m_;
            var sDs = 3 * kS * s_ * s_;

            var lDs2 = 6 * kL * kL * l_;
            var mDs2 = 6 * kM * kM * m_;
            var sDs2 = 6 * kS * kS * s_;

            var f = wl * l + wm * m + ws * s;
            var f1 = wl * lDs + wm * mDs + ws * sDs;
            var f2 = wl * lDs2 + wm * mDs2 + ws * sDs2;

            uS -= f * f1 / (f1 * f1 - 0.5f * f * f2);
        }

        return uS;
    }

    public static Cs GetCs(float uL, float a, float b)
    {
        var cusp = FindCusp(a, b);

        var cMax = FindGamutIntersection(a, b, uL, 1, uL, cusp);
        var stMax = ToSt(cusp);

        // Scale factor to compensate for the curved part of gamut shape:
        var k = cMax / Math.Min(uL * stMax.S, (1 - uL) * stMax.T);

        float cMid;
        {
            var stMid = GetStMid(a, b);

            // Use a soft minimum function, instead of a sharp triangle shape to get a smooth value for chroma.
            var cA = uL * stMid.S;
            var cB = (1 - uL) * stMid.T;
            cMid = 0.9f * k * MathF.Sqrt(MathF.Sqrt(1 / (1 / (cA * cA * cA * cA) + 1 / (cB * cB * cB * cB))));
        }

        float c0;
        {
            // for C_0, the shape is independent of hue, so ST are constant. Values picked to roughly be the average values of ST.
            var cA = uL * 0.4f;
            var cB = (1 - uL) * 0.8f;

            // Use a soft minimum function, instead of a sharp triangle shape to get a smooth value for chroma.
            c0 = MathF.Sqrt(1 / (1 / (cA * cA) + 1 / (cB * cB)));
        }

        return new(c0, cMid, cMax);
    }

    ///<summary>
    /// finds L_cusp and C_cusp for a given hue
    /// a and b must be normalized so a^2 + b^2 == 1
    ///</summary>
    public static LC FindCusp(float a, float b)
    {
        // First, find the maximum saturation (saturation S = C/L)
        var sCusp = ComputeMaxSaturation(a, b);

        // Convert to linear sRGB to find the first point where at least one of r,g or b >= 1:
        var rgbAtMax = OklabToLinearSrgb(new(1, sCusp * a, sCusp * b));
        var lCusp    = MathF.Cbrt(1 / Math.Max(Math.Max(rgbAtMax.R, rgbAtMax.G), rgbAtMax.B));
        var cCusp    = lCusp * sCusp;

        return new(lCusp , cCusp);
    }

    ///<summary>
    /// Finds intersection of the line defined by
    /// L = L0 * (1 - t) + t * L1;
    /// C = t * C1;
    /// a and b must be normalized so a^2 + b^2 == 1
    ///</summary>
    public static float FindGamutIntersection(float a, float b, float l1, float c1, float l0, LC cusp)
    {
        // Find the intersection for upper and lower half seprately
        float t;
        if ((l1 - l0) * cusp.C - (cusp.L - l0) * c1 <= 0)
        {
            // Lower half

            t = cusp.C * l0 / (c1 * cusp.L + cusp.C * (l0 - l1));
        }
        else
        {
            // Upper half

            // First intersect with triangle
            t = cusp.C * (l0 - 1) / (c1 * (cusp.L - 1) + cusp.C * (l0 - l1));

            // Then one step Halley's method
            {
                var dL = l1 - l0;
                var dC = c1;

                var kL = +0.3963377774f * a + 0.2158037573f * b;
                var kM = -0.1055613458f * a - 0.0638541728f * b;
                var kS = -0.0894841775f * a - 1.2914855480f * b;

                var lDt = dL + dC * kL;
                var mDt = dL + dC * kM;
                var sDt = dL + dC * kS;


                // If higher accuracy is required, 2 or 3 iterations of the following block can be used:
                {
                    var uL = l0 * (1 - t) + t * l1;
                    var uC = t * c1;

                    var l_ = uL + uC * kL;
                    var m_ = uL + uC * kM;
                    var s_ = uL + uC * kS;

                    var l = l_ * l_ * l_;
                    var m = m_ * m_ * m_;
                    var s = s_ * s_ * s_;

                    var ldt = 3 * lDt * l_ * l_;
                    var mdt = 3 * mDt * m_ * m_;
                    var sdt = 3 * sDt * s_ * s_;

                    var ldt2 = 6 * lDt * lDt * l_;
                    var mdt2 = 6 * mDt * mDt * m_;
                    var sdt2 = 6 * sDt * sDt * s_;

                    var r = 4.0767416621f * l - 3.3077115913f * m + 0.2309699292f * s - 1;
                    var r1 = 4.0767416621f * ldt - 3.3077115913f * mdt + 0.2309699292f * sdt;
                    var r2 = 4.0767416621f * ldt2 - 3.3077115913f * mdt2 + 0.2309699292f * sdt2;

                    var uR = r1 / (r1 * r1 - 0.5f * r * r2);
                    var tR = -r * uR;

                    var g = -1.2684380046f * l + 2.6097574011f * m - 0.3413193965f * s - 1;
                    var g1 = -1.2684380046f * ldt + 2.6097574011f * mdt - 0.3413193965f * sdt;
                    var g2 = -1.2684380046f * ldt2 + 2.6097574011f * mdt2 - 0.3413193965f * sdt2;

                    var uG = g1 / (g1 * g1 - 0.5f * g * g2);
                    var tG = -g * uG;

                    b = -0.0041960863f * l - 0.7034186147f * m + 1.7076147010f * s - 1;
                    var b1 = -0.0041960863f * ldt - 0.7034186147f * mdt + 1.7076147010f * sdt;
                    var b2 = -0.0041960863f * ldt2 - 0.7034186147f * mdt2 + 1.7076147010f * sdt2;

                    var uB = b1 / (b1 * b1 - 0.5f * b * b2);
                    var tB = -b * uB;

                    tR = uR >= 0 ? tR : float.MaxValue;
                    tG = uG >= 0 ? tG : float.MaxValue;
                    tB = uB >= 0 ? tB : float.MaxValue;

                    t += Math.Min(tR, Math.Min(tG, tB));
                }
            }
        }

        return t;
    }

    ///<summary>
    /// Returns a smooth approximation of the location of the cusp
    /// This polynomial was created by an optimization process
    /// It has been designed so that S_mid < S_max and T_mid < T_max
    ///</summary>
    public static ST GetStMid(float a, float b)
    {
        var s = 0.11516993f + 1 /
        (
            7.44778970f + 4.15901240f * b
            + a *
            (
                -2.19557347f + 1.75198401f * b
                + a *
                (
                    -2.13704948f - 10.02301043f * b
                    + a * (-4.24894561f + 5.38770819f * b + 4.69891013f * a)
                )
            )
        );

        var t = 0.11239642f + 1 /
        (
            1.61320320f - 0.68124379f * b
            + a *
            (
                +0.40370612f + 0.90148123f * b
                + a *
                (
                    -0.27087943f + 0.61223990f * b
                    + a * (+0.00299215f - 0.45399568f * b - 0.14661872f * a)
                )
            )
        );

        return new(s, t);
    }

    public static RGB OkHslToSrgb(HSL hsl)
    {
        var h = hsl.H;
        var s = hsl.S;
        var l = hsl.L;

        if (l == 1.0f)
        {
            return new(1, 1, 1);
        }

        else if (l == 0)
        {
            return new(0, 0, 0);
        }

        var a  = MathF.Cos((float)(2 * Math.PI * h));
        var b  = MathF.Sin((float)(2 * Math.PI * h));
        var lI = ToeInv(l);

        var cs   = GetCs(lI, a, b);
        var c0   = cs.C0;
        var cMid = cs.CMid;
        var cMax = cs.CMax;

        var mid    = 0.8f;
        var midInv = 1.25f;

        float c, t, k0, k1, k2;

        if (s < mid)
        {
            t = midInv * s;

            k1 = mid * c0;
            k2 = 1 - k1 / cMid;

            c = t * k1 / (1 - k2 * t);
        }
        else
        {
            t = (s - mid)/ (1 - mid);

            k0 = cMid;
            k1 = (1 - mid) * cMid * cMid * midInv * midInv / c0;
            k2 = 1 - k1 / (cMax - cMid);

            c = k0 + t * k1 / (1 - k2 * t);
        }

        var rgb = OklabToLinearSrgb(new(lI, c * a, c * b));
        return new(
            SrgbTransferFunction(rgb.R),
            SrgbTransferFunction(rgb.G),
            SrgbTransferFunction(rgb.B)
        );
    }

    public static RGB OklabToLinearSrgb(Lab c)
    {
        var l_ = c.L + 0.3963377774f * c.A + 0.2158037573f * c.B;
        var m_ = c.L - 0.1055613458f * c.A - 0.0638541728f * c.B;
        var s_ = c.L - 0.0894841775f * c.A - 1.2914855480f * c.B;

        var l = l_ * l_ * l_;
        var m = m_ * m_ * m_;
        var s = s_ * s_ * s_;

        return new(
             4.0767416621f * l - 3.3077115913f * m + 0.2309699292f * s,
            -1.2684380046f * l + 2.6097574011f * m - 0.3413193965f * s,
            -0.0041960863f * l - 0.7034186147f * m + 1.7076147010f * s
        );
    }

    public static float SrgbTransferFunction(float a) =>
        0.0031308f >= a ? 12.92f * a : 1.055f * MathF.Pow(a, .4166666666666667f) - .055f;

    public static float ToeInv(float x)
    {
        var k1 = 0.206f;
        var k2 = 0.03f;
        var k3 = (1 + k1) / (1 + k2);
        return (x * x + k1 * x) / (k3 * (x + k2));
    }

    public static ST ToSt(LC cusp)
    {
        var l = cusp.L;
        var c = cusp.C;
        return new(c / l, c / (1 - l));
    }
}
