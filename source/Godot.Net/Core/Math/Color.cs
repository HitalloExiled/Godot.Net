namespace Godot.Net.Core.Math;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Godot.Net.ThirdParty.Misc;
using Math = System.Math;

[DebuggerDisplay("\\{ R: {R}, G: {G}, B: {B}, A: {A} \\}")]
public record struct Color
{
    private static Dictionary<string, Color> namedColors = new()
    {
        ["ALICEBLUE"]          = Hex(0xF0F8FFFF),
        ["ANTIQUEWHITE"]       = Hex(0xFAEBD7FF),
        ["AQUA"]               = Hex(0x00FFFFFF),
        ["AQUAMARINE"]         = Hex(0x7FFFD4FF),
        ["AZURE"]              = Hex(0xF0FFFFFF),
        ["BEIGE"]              = Hex(0xF5F5DCFF),
        ["BISQUE"]             = Hex(0xFFE4C4FF),
        ["BLACK"]              = Hex(0x000000FF),
        ["BLANCHEDALMOND"]     = Hex(0xFFEBCDFF),
        ["BLUE"]               = Hex(0x0000FFFF),
        ["BLUEVIOLET"]         = Hex(0x8A2BE2FF),
        ["BROWN"]              = Hex(0xA52A2AFF),
        ["BURLYWOOD"]          = Hex(0xDEB887FF),
        ["CADETBLUE"]          = Hex(0x5F9EA0FF),
        ["CHARTREUSE"]         = Hex(0x7FFF00FF),
        ["CHOCOLATE"]          = Hex(0xD2691EFF),
        ["CORAL"]              = Hex(0xFF7F50FF),
        ["CORNFLOWERBLUE"]     = Hex(0x6495EDFF),
        ["CORNSILK"]           = Hex(0xFFF8DCFF),
        ["CRIMSON"]            = Hex(0xDC143CFF),
        ["CYAN"]               = Hex(0x00FFFFFF),
        ["DARKBLUE"]           = Hex(0x00008BFF),
        ["DARKCYAN"]           = Hex(0x008B8BFF),
        ["DARKGOLDENROD"]      = Hex(0xB8860BFF),
        ["DARKRAY"]            = Hex(0xA9A9A9FF),
        ["DARKGREEN"]          = Hex(0x006400FF),
        ["DARKKHAKI"]          = Hex(0xBDB76BFF),
        ["DARKMAGENTA"]        = Hex(0x8B008BFF),
        ["DARKOLIVEGREEN"]     = Hex(0x556B2FFF),
        ["DARKORANGE"]         = Hex(0xFF8C00FF),
        ["DARKORCHID"]         = Hex(0x9932CCFF),
        ["DARKRED"]            = Hex(0x8B0000FF),
        ["DARKSALMON"]         = Hex(0xE9967AFF),
        ["DARKSEAGREEN"]       = Hex(0x8FBC8FFF),
        ["DARKSLATEBLUE"]      = Hex(0x483D8BFF),
        ["DARKSLATEGRAY"]      = Hex(0x2F4F4FFF),
        ["DARKTURQUOISE"]      = Hex(0x00CED1FF),
        ["DARKVIOLET"]         = Hex(0x9400D3FF),
        ["DEEPPINK"]           = Hex(0xFF1493FF),
        ["DEEPSKYBLUE"]        = Hex(0x00BFFFFF),
        ["DIMGRAY"]            = Hex(0x696969FF),
        ["DODGERBLUE"]         = Hex(0x1E90FFFF),
        ["FIREBRICK"]          = Hex(0xB22222FF),
        ["FLORALWHITE"]        = Hex(0xFFFAF0FF),
        ["FORESTGREEN"]        = Hex(0x228B22FF),
        ["FUCHSIA"]            = Hex(0xFF00FFFF),
        ["GAINSBORO"]          = Hex(0xDCDCDCFF),
        ["GHOSTWHITE"]         = Hex(0xF8F8FFFF),
        ["GOLD"]               = Hex(0xFFD700FF),
        ["GOLDENROD"]          = Hex(0xDAA520FF),
        ["GRAY"]               = Hex(0xBEBEBEFF),
        ["GREEN"]              = Hex(0x00FF00FF),
        ["GREENYELLOW"]        = Hex(0xADFF2FFF),
        ["HONEYDEW"]           = Hex(0xF0FFF0FF),
        ["HOTPINK"]            = Hex(0xFF69B4FF),
        ["INDIANRED"]          = Hex(0xCD5C5CFF),
        ["INDIGO"]             = Hex(0x4B0082FF),
        ["IVORY"]              = Hex(0xFFFFF0FF),
        ["KHAKI"]              = Hex(0xF0E68CFF),
        ["LAVENDER"]           = Hex(0xE6E6FAFF),
        ["LAVENDERBLUSH"]      = Hex(0xFFF0F5FF),
        ["LAWNGREEN"]          = Hex(0x7CFC00FF),
        ["LEMONCHIFFON"]       = Hex(0xFFFACDFF),
        ["LIGHTBLUE"]          = Hex(0xADD8E6FF),
        ["LIGHTCORAL"]         = Hex(0xF08080FF),
        ["LIGHTCYAN"]          = Hex(0xE0FFFFFF),
        ["LIGHTGOLDENROD"]     = Hex(0xFAFAD2FF),
        ["LIGHTGRAY"]          = Hex(0xD3D3D3FF),
        ["LIGHTGREEN"]         = Hex(0x90EE90FF),
        ["LIGHTPINK"]          = Hex(0xFFB6C1FF),
        ["LIGHTSALMON"]        = Hex(0xFFA07AFF),
        ["LIGHTSEAGREEN"]      = Hex(0x20B2AAFF),
        ["LIGHTSKYBLUE"]       = Hex(0x87CEFAFF),
        ["LIGHTSLATEGRAY"]     = Hex(0x778899FF),
        ["LIGHTSTEELBLUE"]     = Hex(0xB0C4DEFF),
        ["LIGHTYELLOW"]        = Hex(0xFFFFE0FF),
        ["LIME"]               = Hex(0x00FF00FF),
        ["LIMEGREEN"]          = Hex(0x32CD32FF),
        ["LINEN"]              = Hex(0xFAF0E6FF),
        ["MAGENTA"]            = Hex(0xFF00FFFF),
        ["MAROON"]             = Hex(0xB03060FF),
        ["MEDIUMAQUAMARINE"]   = Hex(0x66CDAAFF),
        ["MEDIUMBLUE"]         = Hex(0x0000CDFF),
        ["MEDIUMORCHID"]       = Hex(0xBA55D3FF),
        ["MEDIUMPURPLE"]       = Hex(0x9370DBFF),
        ["MEDIUMSEAGREEN"]     = Hex(0x3CB371FF),
        ["MEDIUMSLATEBLUE"]    = Hex(0x7B68EEFF),
        ["MEDIUMSPRINGGREEN"]  = Hex(0x00FA9AFF),
        ["MEDIUMTURQUOISE"]    = Hex(0x48D1CCFF),
        ["MEDIUMVIOLETRED"]    = Hex(0xC71585FF),
        ["MIDNIGHTBLUE"]       = Hex(0x191970FF),
        ["MINTCREAM"]          = Hex(0xF5FFFAFF),
        ["MISTYROSE"]          = Hex(0xFFE4E1FF),
        ["MOCCASIN"]           = Hex(0xFFE4B5FF),
        ["NAVAJOWHITE"]        = Hex(0xFFDEADFF),
        ["NAVYBLUE"]           = Hex(0x000080FF),
        ["OLDLACE"]            = Hex(0xFDF5E6FF),
        ["OLIVE"]              = Hex(0x808000FF),
        ["OLIVEDRAB"]          = Hex(0x6B8E23FF),
        ["ORANGE"]             = Hex(0xFFA500FF),
        ["ORANGERED"]          = Hex(0xFF4500FF),
        ["ORCHID"]             = Hex(0xDA70D6FF),
        ["PALEGOLDENROD"]      = Hex(0xEEE8AAFF),
        ["PALEGREEN"]          = Hex(0x98FB98FF),
        ["PALETURQUOISE"]      = Hex(0xAFEEEEFF),
        ["PALEVIOLETRED"]      = Hex(0xDB7093FF),
        ["PAPAYAWHIP"]         = Hex(0xFFEFD5FF),
        ["PEACHPUFF"]          = Hex(0xFFDAB9FF),
        ["PERU"]               = Hex(0xCD853FFF),
        ["PINK"]               = Hex(0xFFC0CBFF),
        ["PLUM"]               = Hex(0xDDA0DDFF),
        ["POWDERBLUE"]         = Hex(0xB0E0E6FF),
        ["PURPLE"]             = Hex(0xA020F0FF),
        ["REBECCAPURPLE"]      = Hex(0x663399FF),
        ["RED"]                = Hex(0xFF0000FF),
        ["ROSYBROWN"]          = Hex(0xBC8F8FFF),
        ["ROYALBLUE"]          = Hex(0x4169E1FF),
        ["SADDLEBROWN"]        = Hex(0x8B4513FF),
        ["SALMON"]             = Hex(0xFA8072FF),
        ["SANDYBROWN"]         = Hex(0xF4A460FF),
        ["SEAGREEN"]           = Hex(0x2E8B57FF),
        ["SEASHELL"]           = Hex(0xFFF5EEFF),
        ["SIENNA"]             = Hex(0xA0522DFF),
        ["SILVER"]             = Hex(0xC0C0C0FF),
        ["SKYBLUE"]            = Hex(0x87CEEBFF),
        ["SLATEBLUE"]          = Hex(0x6A5ACDFF),
        ["SLATEGRAY"]          = Hex(0x708090FF),
        ["SNOW"]               = Hex(0xFFFAFAFF),
        ["SPRINGGREEN"]        = Hex(0x00FF7FFF),
        ["STEELBLUE"]          = Hex(0x4682B4FF),
        ["TAN"]                = Hex(0xD2B48CFF),
        ["TEAL"]               = Hex(0x008080FF),
        ["THISTLE"]            = Hex(0xD8BFD8FF),
        ["TOMATO"]             = Hex(0xFF6347FF),
        ["TRANSPARENT"]        = Hex(0xFFFFFF00),
        ["TURQUOISE"]          = Hex(0x40E0D0FF),
        ["VIOLET"]             = Hex(0xEE82EEFF),
        ["WEBGRAY"]            = Hex(0x808080FF),
        ["WEBGREEN"]           = Hex(0x008000FF),
        ["WEBMAROON"]          = Hex(0x800000FF),
        ["WEBPURPLE"]          = Hex(0x800080FF),
        ["WHEAT"]              = Hex(0xF5DEB3FF),
        ["WHITE"]              = Hex(0xFFFFFFFF),
        ["WHITESMOKE"]         = Hex(0xF5F5F5FF),
        ["YELLOW"]             = Hex(0xFFFF00FF),
        ["YELLOWGREEN"]        = Hex(0x9ACD32FF),
    };

    public readonly float Luminance => 0.2126f * this.R + 0.7152f * this.G + 0.0722f * this.B;

    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }
    public float A { get; set; }

    public float this[int index]
    {
        readonly get => index switch
        {
            0 => this.R,
            1 => this.G,
            2 => this.B,
            3 => this.A,
            _ => throw new IndexOutOfRangeException(),
        };
        set
        {
            switch (index)
            {
                case 0: this.R = value; break;
                case 1: this.G = value; break;
                case 2: this.B = value; break;
                case 3: this.A = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public float H
    {
        set => this.SetHsv(value, this.S, this.V, this.A);
        readonly get
        {
            var min = Math.Min(this.R, this.G);
            min = Math.Min(min, this.B);

            var max = Math.Max(this.R, this.G);
            max = Math.Max(max, this.B);

            var delta = max - min;

            if (delta == 0.0f)
            {
                return 0.0f;
            }

            float h;

            if (this.R == max)
            {
                h = (this.G - this.B) / delta; // between yellow & magenta
            }
            else if (this.G == max)
            {
                h = 2 + (this.B - this.R) / delta; // between cyan & yellow
            }
            else
            {
                h = 4 + (this.R - this.G) / delta; // between magenta & cyan
            }

            h /= 6.0f;
            if (h < 0.0f)
            {
                h += 1.0f;
            }

            return h;
        }
    }

    public float S
    {
        set => this.SetHsv(this.H, value, this.V, this.A);
        readonly get
        {
            var min = Math.Min(this.R, this.G);
            min = Math.Min(min, this.B);

            var max = Math.Max(this.R, this.G);
            max = Math.Max(max, this.B);

            var delta = max - min;

            return (max != 0.0f) ? (delta / max) : 0.0f;
        }
    }

    public float V
    {
        set => this.SetHsv(this.H, this.S, value, this.A);
        readonly get
        {
            var max = Math.Max(this.R, this.G);
            max = Math.Max(max, this.B);

            return max;
        }
    }

    public int A8
    {
        readonly get => (int)Math.Clamp(Math.Round(this.A * 255.0f), 0.0f, 255.0f);
        set => this.A = Math.Clamp(value, 0, 255) / 255.0f;
    }

    public int B8
    {
        readonly get => (int)Math.Clamp(Math.Round(this.B * 255.0f), 0.0f, 255.0f);
        set => this.B = Math.Clamp(value, 0, 255) / 255.0f;
    }

    public int G8
    {
        readonly get => (int)Math.Clamp(Math.Round(this.G * 255.0f), 0.0f, 255.0f);
        set => this.G = Math.Clamp(value, 0, 255) / 255.0f;
    }

    public int R8
    {
        readonly get => (int)Math.Clamp(Math.Round(this.R * 255.0f), 0.0f, 255.0f);
        set => this.R = Math.Clamp(value, 0, 255) / 255.0f;
    }

    public Color(float r, float g, float b, float a = 1)
    {
        this.R = r;
        this.G = g;
        this.B = b;
        this.A = a;
    }

    public Color(in Color c, float a)
    {
        this.R = c.R;
        this.G = c.G;
        this.B = c.B;
        this.A = a;
    }

    public Color(string code) =>
        this = HtmlIsValid(code) ? Html(code) : Named(code);

    private static bool HtmlIsValid(string color)
    {
        var source = color;

        if (source.Length == 0)
        {
            return false;
        }
        if (source[0] == '#')
        {
            source = source[1..];
        }

        // Check if the amount of hex digits is valid.
        var len = source.Length;
        if (!(len == 3 || len == 4 || len == 6 || len == 8))
        {
            return false;
        }

        // Check if each hex digit is valid.
        for (var i = 0; i < len; i++)
        {
            if (ParseCol4(source, i) == -1)
            {
                return false;
            }
        }

        return true;
    }

    private static Color Named(string name)
    {
        var normalizedName = name
            .Replace(" ", "")
            .Replace("-", "")
            .Replace("_", "")
            .Replace("'", "")
            .Replace(".", "")
            .ToUpper();

        return namedColors.TryGetValue(normalizedName, out var color)
            ? color
            : ERR_FAIL_V_MSG(new Color(), "Invalid color name: " + name + ".");
    }

    private static int ParseCol4(string str, int ofs)
    {
        var character = str[ofs];

        if (character >= '0' && character <= '9')
        {
            return character - '0';
        }
        else if (character >= 'a' && character <= 'f')
        {
            return character + (10 - 'a');
        }
        else if (character >= 'A' && character <= 'F')
        {
            return character + (10 - 'A');
        }
        return -1;
    }

    private static int ParseCol8(string str, int ofs) =>
        ParseCol4(str, ofs) * 16 + ParseCol4(str, ofs + 1);

    public static Color FromHsv(float h, float s, float v, float alpha = 1)
    {
        var c = new Color();
        c.SetHsv(h, s, v, alpha);
        return c;
    }

    public static Color FromOkHsl(float h, float s, float l, float alpha = 1)
    {
        var c = new Color();
        c.SetOkHsl(h, s, l, alpha);
        return c;
    }

    public static Color FromRgbe9995(uint rgbe)
    {
        var r = (float)(rgbe & 0x1ff);
        var g = (float)((rgbe >> 9) & 0x1ff);
        var b = (float)((rgbe >> 18) & 0x1ff);
        var e = (float)(rgbe >> 27);
        var m = (float)Math.Pow(2.0f, e - 15.0f - 9.0f);

        var rd = r * m;
        var gd = g * m;
        var bd = b * m;

        return new(rd, gd, bd, 1.0f);
    }

    public static Color Hex(uint hex)
    {
        var a = (hex & 0xFF) / 255.0f;
        hex >>= 8;

        var b = (hex & 0xFF) / 255.0f;
        hex >>= 8;

        var g = (hex & 0xFF) / 255.0f;
        hex >>= 8;

        var r = (hex & 0xFF) / 255.0f;

        return new(r, g, b, a);
    }

    public static Color Html(string rgba)
    {
        var color = rgba;
        if (color.Length == 0)
        {
            return new();
        }
        if (color[0] == '#')
        {
            color = color[1..];
        }

        // If enabled, use 1 hex digit per channel instead of 2.
        // Other sizes aren't in the HTML/CSS spec but we could add them if desired.
        var isShorthand = color.Length < 5;
        bool alpha;
        if (color.Length == 8)
        {
            alpha = true;
        }
        else if (color.Length == 6)
        {
            alpha = false;
        }
        else if (color.Length == 4)
        {
            alpha = true;
        }
        else if (color.Length == 3)
        {
            alpha = false;
        }
        else
        {
            return ERR_FAIL_V_MSG(new Color(), $"Invalid color code: {rgba}.");
        }

        float r, g, b, a = 1;

        if (isShorthand)
        {
            r = ParseCol4(color, 0) / 15.0f;
            g = ParseCol4(color, 1) / 15.0f;
            b = ParseCol4(color, 2) / 15.0f;
            if (alpha)
            {
                a = ParseCol4(color, 3) / 15.0f;
            }
        }
        else
        {
            r = ParseCol8(color, 0) / 255.0f;
            g = ParseCol8(color, 2) / 255.0f;
            b = ParseCol8(color, 4) / 255.0f;
            if (alpha)
            {
                a = ParseCol8(color, 6) / 255.0f;
            }
        }

        return ERR_FAIL_COND_V_MSG(r < 0.0f, "Invalid color code: " + rgba + ".")
            || ERR_FAIL_COND_V_MSG(g < 0.0f, "Invalid color code: " + rgba + ".")
            || ERR_FAIL_COND_V_MSG(b < 0.0f, "Invalid color code: " + rgba + ".")
            || ERR_FAIL_COND_V_MSG(a < 0.0f, "Invalid color code: " + rgba + ".")
                ? new()
                : new(r, g, b, a);
    }

    public readonly Color Clamp() => this.Clamp(new(), new(1, 1, 1, 1));

    public readonly Color Clamp(in Color min, in Color max) =>
        new(
            Math.Clamp(this.R, min.R, max.R),
            Math.Clamp(this.G, min.G, max.G),
            Math.Clamp(this.B, min.B, max.B),
            Math.Clamp(this.R, min.R, max.A)
        );

    public readonly Color Darkened(float amount) =>
        new(
            this.R * (1.0f - amount),
            this.G * (1.0f - amount),
            this.B * (1.0f - amount),
            this.A
        );

    public void Invert()
    {
        this.R = 1 - this.R;
        this.G = 1 - this.G;
        this.B = 1 - this.B;
    }

    public readonly Color Inverted()
    {
        var c = this;

        c.Invert();

        return c;
    }

    public readonly Color Lerp(in Color to, float weight) =>
        new(
            MathX.Lerp(this.R, to.R, weight),
            MathX.Lerp(this.G, to.G, weight),
            MathX.Lerp(this.B, to.B, weight),
            MathX.Lerp(this.A, to.A, weight)
        );

    public void SetHsv(float h, float s, float v, float alpha = 1)
    {
        int i;
        float f, p, q, t;

        this.A = alpha;

        if (s == 0.0f)
        {
            // Achromatic (gray)
            this.R = this.G = this.B = v;
            return;
        }

        h *= 6.0f;
        h %= 6f;
        i = (int)Math.Floor(h);

        f = h - i;
        p = v * (1.0f - s);
        q = v * (1.0f - s * f);
        t = v * (1.0f - s * (1.0f - f));

        switch (i)
        {
            case 0: // Red is the dominant color
                this.R = v;
                this.G = t;
                this.B = p;
                break;
            case 1: // Green is the dominant color
                this.R = q;
                this.G = v;
                this.B = p;
                break;
            case 2:
                this.R = p;
                this.G = v;
                this.B = t;
                break;
            case 3: // Blue is the dominant color
                this.R = p;
                this.G = q;
                this.B = v;
                break;
            case 4:
                this.R = t;
                this.G = p;
                this.B = v;
                break;
            default: // (5) Red is the dominant color
                this.R = v;
                this.G = p;
                this.B = q;
                break;
        }
    }

    public void SetOkHsl(float h, float s, float l, float alpha)
    {
        var hsl = new OkColor.HSL(h, s, l);

        var rgb = OkColor.OkHslToSrgb(hsl);
        var c = new Color(rgb.R, rgb.G, rgb.B, alpha).Clamp();

        this.R = c.R;
        this.G = c.G;
        this.B = c.B;
        this.A = c.A;
    }

    public readonly uint ToArgb32()
    {
        var c = (uint)(ushort)Math.Round(this.A * 255.0f);
        c <<= 8;
        c |= (ushort)Math.Round(this.R * 255.0f);
        c <<= 8;
        c |= (ushort)Math.Round(this.G * 255.0f);
        c <<= 8;
        c |= (ushort)Math.Round(this.B * 255.0f);

        return c;
    }

    public readonly string ToHtml(bool alpha)
    {
        var txt = "";

        txt += ((int)this.R).ToString("x").PadLeft(2, '0');
        txt += ((int)this.G).ToString("x").PadLeft(2, '0');
        txt += ((int)this.B).ToString("x").PadLeft(2, '0');

        if (alpha)
        {
            txt += ((int)this.A).ToString("x").PadLeft(2, '0');
        }

        return txt;
    }

    public readonly uint ToRgbe9995() => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Color operator +(Color left, Color rigth) =>
        new(
            left.R + rigth.R,
            left.G + rigth.G,
            left.B + rigth.B,
            left.A + rigth.A
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Color operator -(Color left, Color rigth) =>
        new(
            left.R - rigth.R,
            left.G - rigth.G,
            left.B - rigth.B,
            left.A - rigth.A
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Color operator *(Color left, Color rigth) =>
        new(
            left.R * rigth.R,
            left.G * rigth.G,
            left.B * rigth.B,
            left.A * rigth.A
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Color operator *(Color color, float scalar) =>
        new(
            color.R * scalar,
            color.G * scalar,
            color.B * scalar,
            color.A * scalar
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Color operator /(Color left, Color rigth) =>
        new(
            left.R / rigth.R,
            left.G / rigth.G,
            left.B / rigth.B,
            left.A / rigth.A
        );

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Color operator /(Color color, float scalar) =>
        new(
            color.R / scalar,
            color.G / scalar,
            color.B / scalar,
            color.A / scalar
        );
}

