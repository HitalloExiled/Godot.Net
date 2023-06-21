namespace Godot.Net.Core.IO;

using System.Runtime.InteropServices;
using Godot.Net.Core.Math;
using Error = Error.Error;
using Math  = System.Math;

#pragma warning disable IDE0051, IDE0052 // TODO Remove

public partial class Image : Resource
{
    private delegate Image ImageMemLoadFunc(byte[] png);
    private delegate void RenormalizeFunc<T>(Span<T> value);
    private delegate void AverageFunc<T>(ref T @out, T a, T b, T c, T d);

    private const int MAX_WIDTH = 1 << 24; // force a limit somehow
    private const int MAX_HEIGHT = 1 << 24; // force a limit somehow
    private const int MAX_PIXELS = 268435456;

    private static readonly string[] formatNames =
    {
        "Lum8", //luminance
        "LumAlpha8", //luminance-alpha
        "Red8",
        "RedGreen",
        "RGB8",
        "RGBA8",
        "RGBA4444",
        "RGBA5551",
        "RFloat", //float
        "RGFloat",
        "RGBFloat",
        "RGBAFloat",
        "RHalf", //half float
        "RGHalf",
        "RGBHalf",
        "RGBAHalf",
        "RGBE9995",
        "DXT1 RGB8", //s3tc
        "DXT3 RGBA8",
        "DXT5 RGBA8",
        "RGTC Red8",
        "RGTC RedGreen8",
        "BPTC_RGBA",
        "BPTC_RGBF",
        "BPTC_RGBFU",
        "ETC", //etc1
        "ETC2_R11", //etc2
        "ETC2_R11S", //signed", NOT srgb.
        "ETC2_RG11",
        "ETC2_RG11S",
        "ETC2_RGB8",
        "ETC2_RGBA8",
        "ETC2_RGB8A1",
        "ETC2_RA_AS_RG",
        "FORMAT_DXT5_RA_AS_RG",
        "ASTC_4x4",
        "ASTC_4x4_HDR",
        "ASTC_8x8",
        "ASTC_8x8_HDR"
    };

    private byte[] data = Array.Empty<byte>();

    public byte[]      Data         => this.data;
    public ImageFormat Format       { get; private set; } = ImageFormat.FORMAT_L8;
    public bool        HasMipmaps   { get; private set; }
    public int         Height       { get; private set; }
    public bool        IsCompressed => this.Format > ImageFormat.FORMAT_RGBE9995;
    public int         MipmapCount  =>  this.HasMipmaps ? GetImageRequiredMipmaps(this.Width, this.Height, this.Format) : 0;
    public int         Width        { get; private set; }

    #region private static methods
    private static void Average4Float(ref float @out, float a, float b, float c, float d) =>
        @out = (a + b + c + d) * 0.25f;

    private static void Average4Half(ref ushort @out, ushort a, ushort b, ushort c, ushort d) =>
        @out = (ushort)((a + b + c + d) * 0.25f);

    private static void Average4Rgbe9995(ref uint @out, uint a, uint b, uint c, uint d) =>
        @out = ((Color.FromRgbe9995(a) + Color.FromRgbe9995(b) + Color.FromRgbe9995(c) + Color.FromRgbe9995(d)) * 0.25f).ToRgbe9995();

    private static void Average4Uint8(ref byte @out, byte a, byte b, byte c, byte d) =>
        @out = (byte)((a + b + c + d + 2) >> 2);

    private static bool CanModify(ImageFormat format) =>
        format <= ImageFormat.FORMAT_RGBE9995;

    private static int GetDstImageSize(int width, int height, ImageFormat format, out int outMipmaps, int inMipmaps = -1) =>
        GetDstImageSize(width, height, format, out outMipmaps, inMipmaps, out var _, out var _);

    /// Get mipmap size and offset.
    private static int GetDstImageSize(int width, int height, ImageFormat format, out int outMipmaps, int inMipmaps, out int mmWidth, out int mmHeight)
    {
        mmWidth = default;
        mmHeight = default;

        // Data offset in mipmaps (including the original texture).
        var size = 0;

        var w = width;
        var h = height;

        // Current mipmap index in the loop below. mipmaps is the target mipmap index.
        // In this function, mipmap 0 represents the first mipmap instead of the original texture.
        var mm = 0;

        var pixsize  = GetFormatPixelSize(format);
        var pixshift = GetFormatPixelRshift(format);
        var block    = GetFormatBlockSize(format);

        // Technically, you can still compress up to 1 px no matter the format, so commenting this.
        //int minw, minh;
        //get_format_min_pixelSize(format, minw, minh);
        var minw = 1;
        var minh = 1;

        while (true)
        {
            var bw = w % block != 0 ? w + (block - w % block) : w;
            var bh = h % block != 0 ? h + (block - h % block) : h;

            var s = bw * bh;

            s *= pixsize;
            s >>= pixshift;

            size += s;

            if (inMipmaps >= 0)
            {
                w = Math.Max(minw, w >> 1);
                h = Math.Max(minh, h >> 1);
            }
            else
            {
                if (w == minw && h == minh)
                {
                    break;
                }
                w = Math.Max(minw, w >> 1);
                h = Math.Max(minh, h >> 1);
            }

            // Set mipmap size.
            mmWidth  = w;
            mmHeight = h;

            // Reach target mipmap.
            if (inMipmaps >= 0 && mm == inMipmaps)
            {
                break;
            }

            mm++;
        }

        outMipmaps = mm;

        return size;
    }

    private static void RenormalizeFloat(Span<float> rgb)
    {
        var n = new Vector3<RealT>(rgb[0], rgb[1], rgb[2]);

        n.Normalize();

        rgb[0] = n.X;
        rgb[1] = n.Y;
        rgb[2] = n.Z;
    }

    private static void RenormalizeHalf(Span<ushort> rgb)
    {
        var n = new Vector3<RealT>(rgb[0], rgb[1], rgb[2]);

        n.Normalize();

        rgb[0] = (ushort)n.X;
        rgb[1] = (ushort)n.Y;
        rgb[2] = (ushort)n.Z;
    }

    private static void RenormalizeRgbe9995(Span<uint> rgb)
    {
        // Never used
    }

    private static void RenormalizeUint8(Span<byte> rgb)
    {
        var n = new Vector3<RealT>(rgb[0] / 255.0f, rgb[1] / 255.0f, rgb[2] / 255.0f);
        n *= 2.0f;
        n -= new Vector3<RealT>(1, 1, 1);
        n.Normalize();
        n += new Vector3<RealT>(1, 1, 1);
        n *= 0.5f;
        n *= 255;

        rgb[0] = (byte)Math.Clamp(n.X, 0, 255);
        rgb[1] = (byte)Math.Clamp(n.Y, 0, 255);
        rgb[2] = (byte)Math.Clamp(n.Z, 0, 255);
    }

    // Repeats `pixel` `count` times in consecutive memory.
    // Results in the original pixel and `count - 1` subsequent copies of it.
    private static void RepeatPixelOverSubsequentMemory(byte[] pixel, int pixelSize, int count)
    {
        var offset = 1;

        for (var stride = 1; offset + stride <= count; stride *= 2)
        {
            Buffer.BlockCopy(pixel, 0, pixel, offset * pixelSize, stride * pixelSize);
            offset += stride;
        }

        if (offset < count)
        {
            Buffer.BlockCopy(pixel, 0, pixel, offset * pixelSize, (count - offset) * pixelSize);
        }
    }
    #endregion private static methods

    #region private methods
    private void CopyInternalsFrom(Image image) => throw new NotImplementedException();
    private void GetClippedSrcAndDestRects(Image src, in Rect2<int> srcRect, in Vector2<int> dest, out Rect2<int> clippedSrcRect, out Rect2<int> clippedDestRect) => throw new NotImplementedException();
    private Dictionary<string, object> GetData() => throw new NotImplementedException();

    private static void GeneratePo2Mipmap<T>(Span<byte> src, Span<byte> dst, uint width, uint height, int cc, bool renormalize, AverageFunc<T> averageFunc, RenormalizeFunc<T> renormalizeFunc) where T : struct
    {
        //fast power of 2 mipmap generation
        var dstW = Math.Max(width >> 1, 1u);
        var dstH = Math.Max(height >> 1, 1u);

        var rightStep = (width == 1) ? 0 : cc;
        var downStep  = (height == 1) ? 0 : (width * cc);

        for (var i = 0u; i < dstH; i++)
        {
            var rupPtr   = MemoryMarshal.Cast<byte, T>(src[(int)(i * 2 * downStep)..]);
            var rdownPtr = rupPtr[(int)downStep..];
            var dstPtr   = MemoryMarshal.Cast<byte, T>(dst[(int)(i * dstW * cc)..]);
            var count    = dstW;

            while (count > 0)
            {
                count--;
                for (var j = 0; j < cc; j++)
                {
                    averageFunc(ref dstPtr[j], rupPtr[j], rupPtr[j + rightStep], rdownPtr[j], rdownPtr[j + rightStep]);
                }

                if (renormalize)
                {
                    renormalizeFunc(dstPtr);
                }

                dstPtr   = dstPtr[cc..];
                rupPtr   = rupPtr[(rightStep * 2)..];
                rdownPtr = rdownPtr[(rightStep * 2)..];
            }
        }
    }

    private void GetPixelb(int x, int y, uint pixelSize, byte data, byte pixel) => throw new NotImplementedException();
    private Error LoadFromBuffer(byte[] array, ImageMemLoadFunc loader) => throw new NotImplementedException();
    private void PutPixelb(int x, int y, uint pixelSize, byte data, byte pixel) => throw new NotImplementedException();

    private void SetColorAtOfs(byte[] bytes, uint ofs, in Color color)
    {
        switch (this.Format)
        {
            case ImageFormat.FORMAT_L8:
                bytes[ofs] = (byte)Math.Clamp(color.V * 255.0, 0, 255);
                break;
            case ImageFormat.FORMAT_LA8:
                bytes[ofs * 2 + 0] = (byte)Math.Clamp(color.V * 255.0, 0, 255);
                bytes[ofs * 2 + 1] = (byte)Math.Clamp(color.A * 255.0, 0, 255);

                break;
            case ImageFormat.FORMAT_R8:
                bytes[ofs] = (byte)Math.Clamp(color.R * 255.0, 0, 255);

                break;
            case ImageFormat.FORMAT_RG8:
                bytes[ofs * 2 + 0] = (byte)Math.Clamp(color.R * 255.0, 0, 255);
                bytes[ofs * 2 + 1] = (byte)Math.Clamp(color.G * 255.0, 0, 255);

                break;
            case ImageFormat.FORMAT_RGB8:
                bytes[ofs * 3 + 0] = (byte)Math.Clamp(color.R * 255.0, 0, 255);
                bytes[ofs * 3 + 1] = (byte)Math.Clamp(color.G * 255.0, 0, 255);
                bytes[ofs * 3 + 2] = (byte)Math.Clamp(color.B * 255.0, 0, 255);

                break;
            case ImageFormat.FORMAT_RGBA8:
                bytes[ofs * 4 + 0] = (byte)Math.Clamp(color.R * 255.0, 0, 255);
                bytes[ofs * 4 + 1] = (byte)Math.Clamp(color.G * 255.0, 0, 255);
                bytes[ofs * 4 + 2] = (byte)Math.Clamp(color.B * 255.0, 0, 255);
                bytes[ofs * 4 + 3] = (byte)Math.Clamp(color.A * 255.0, 0, 255);

                break;
            case ImageFormat.FORMAT_RGBA4444:
                {
                    var rgba = (ushort)Math.Clamp(color.R * 15.0, 0, 15) << 12;

                    rgba |= (ushort)Math.Clamp(color.G * 15.0, 0, 15) << 8;
                    rgba |= (ushort)Math.Clamp(color.B * 15.0, 0, 15) << 4;
                    rgba |= (ushort)Math.Clamp(color.A * 15.0, 0, 15);

                    bytes[ofs] = (byte)rgba;
                }

                break;
            case ImageFormat.FORMAT_RGB565:
                {
                    var rgba = (ushort)Math.Clamp(color.R * 31.0, 0, 31);
                    rgba = (ushort)(rgba | ((int)Math.Clamp(color.G * 63.0f, 0, 33) << 5));
                    rgba = (ushort)(rgba | ((int)Math.Clamp(color.B * 31.0f, 0, 31) << 11));

                    bytes[ofs] = (byte)rgba;
                }

                break;
            case ImageFormat.FORMAT_RF:
                bytes[ofs] = (byte)color.R;

                break;
            case ImageFormat.FORMAT_RGF:
                bytes[ofs * 2 + 0] = (byte)color.R;
                bytes[ofs * 2 + 1] = (byte)color.G;

                break;
            case ImageFormat.FORMAT_RGBF:
                bytes[ofs * 3 + 0] = (byte)color.R;
                bytes[ofs * 3 + 1] = (byte)color.G;
                bytes[ofs * 3 + 2] = (byte)color.B;

                break;
            case ImageFormat.FORMAT_RGBAF:
                bytes[ofs * 4 + 0] = (byte)color.R;
                bytes[ofs * 4 + 1] = (byte)color.G;
                bytes[ofs * 4 + 2] = (byte)color.B;
                bytes[ofs * 4 + 3] = (byte)color.A;

                break;
            case ImageFormat.FORMAT_RH:
                bytes[ofs] = (byte)(Half)color.R;
                break;
            case ImageFormat.FORMAT_RGH:
                bytes[ofs * 2 + 0] = (byte)(Half)color.R;
                bytes[ofs * 2 + 1] = (byte)(Half)color.G;

                break;
            case ImageFormat.FORMAT_RGBH:
                bytes[ofs * 3 + 0] = (byte)(Half)color.R;
                bytes[ofs * 3 + 1] = (byte)(Half)color.G;
                bytes[ofs * 3 + 2] = (byte)(Half)color.B;

                break;
            case ImageFormat.FORMAT_RGBAH:
                bytes[ofs * 4 + 0] = (byte)(Half)color.R;
                bytes[ofs * 4 + 1] = (byte)(Half)color.G;
                bytes[ofs * 4 + 2] = (byte)(Half)color.B;
                bytes[ofs * 4 + 3] = (byte)(Half)color.A;

                break;
            case ImageFormat.FORMAT_RGBE9995:
                bytes[ofs] = (byte)color.ToRgbe9995();

                break;
            default:
                ERR_FAIL_MSG("Can't set_pixel() on compressed image, sorry.");
                break;
        }
    }

    private void SetData(Dictionary<string, object> data) => throw new NotImplementedException();

    private void InitializeData(int width, int height, bool useMipmaps, ImageFormat format)
    {
        if (ERR_FAIL_COND_MSG(width <= 0, $"The Image width specified ({width} pixels) must be greater than 0 pixels."))
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(height <= 0, $"The Image height specified ({height} pixels) must be greater than 0 pixels."))
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(width > MAX_WIDTH, $"The Image width specified ({width} pixels) cannot be greater than {MAX_WIDTH}pixels."))
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(height > MAX_HEIGHT, $"The Image height specified ({height} pixels) cannot be greater than {MAX_HEIGHT}pixels."))
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(width * height > MAX_PIXELS, $"Too many pixels for Image. Maximum is {MAX_WIDTH}x{MAX_HEIGHT} = {MAX_PIXELS}pixels."))
        {
            return;
        }

        if (ERR_FAIL_INDEX_MSG(format, ImageFormat.FORMAT_MAX, $"The Image format specified ({format}) is out of range. See Image's Format enum."))
        {
            return;
        }

        var size = GetDstImageSize(width, height, format, out var _, useMipmaps ? -1 : 0);
        this.data = new byte[size];

        this.Width   = width;
        this.Height  = height;
        this.HasMipmaps = useMipmaps;
        this.Format  = format;
    }

    private void InitializeData(int width, int height, bool useMipmaps, ImageFormat format, byte[] data)
    {
        if (ERR_FAIL_COND_MSG(width <= 0, $"The Image width specified ({width} pixels) must be greater than 0 pixels."))
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(height <= 0, $"The Image height specified ({height} pixels) must be greater than 0 pixels."))
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(width > MAX_WIDTH, $"The Image width specified ({width} pixels) cannot be greater than {MAX_WIDTH} pixels."))
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(height > MAX_HEIGHT, $"The Image height specified ({height} pixels) cannot be greater than {MAX_HEIGHT} pixels."))
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(width * height > MAX_PIXELS, $"Too many pixels for Image. Maximum is {MAX_WIDTH}x{MAX_HEIGHT} = {MAX_PIXELS}pixels ."))
        {
            return;
        }

        if (ERR_FAIL_INDEX_MSG(format, ImageFormat.FORMAT_MAX, $"The Image format specified ({format}) is out of range. See Image's ImageFormat enum."))
        {
            return;
        }

        var size = GetDstImageSize(width, height, format, out var _, useMipmaps ? -1 : 0);

        if (data.Length != size)
        {
            var descriptionMipmaps = GetFormatName(format) + " ";
            if (useMipmaps)
            {
                var numMipmaps = GetImageRequiredMipmaps(width, height, format);

                if (numMipmaps != 1)
                {
                    descriptionMipmaps += $"with {numMipmaps} mipmaps";
                }
                else
                {
                    descriptionMipmaps += "with 1 mipmap";
                }
            }
            else
            {
                descriptionMipmaps += "without mipmaps";
            }

            var description = $"{width}x{height}x{GetFormatPixelSize(format)} ({descriptionMipmaps})";

            if (ERR_FAIL_MSG($"Expected Image data size of {description} = {size} bytes, got {data.Length} bytes instead."))
            {
                return;
            }
        }

        this.Height = height;
        this.Width  = width;
        this.Format = format;
        this.data   = data;

        this.HasMipmaps = useMipmaps;
    }
    #endregion private methods

    #region public static methods
    public static Image CreateEmpty(int width, int height, bool useMipmaps, ImageFormat format)
    {
        var image = new Image();

        image.InitializeData(width, height, useMipmaps, format);

        return image;
    }

    public static int GetFormatBlockSize(ImageFormat format) =>
        format switch
        {
            ImageFormat.FORMAT_DXT1          or // s3tc bc1
            ImageFormat.FORMAT_DXT3          or // bc2
            ImageFormat.FORMAT_DXT5          or // bc3
            ImageFormat.FORMAT_RGTC_R        or // bc4
            ImageFormat.FORMAT_RGTC_RG       or // bc5
            ImageFormat.FORMAT_ETC           or
            ImageFormat.FORMAT_BPTC_RGBA     or
            ImageFormat.FORMAT_BPTC_RGBF     or
            ImageFormat.FORMAT_BPTC_RGBFU    or
            ImageFormat.FORMAT_ETC2_R11      or // etc2
            ImageFormat.FORMAT_ETC2_R11S     or // signed: NOT srgb.
            ImageFormat.FORMAT_ETC2_RG11     or
            ImageFormat.FORMAT_ETC2_RG11S    or
            ImageFormat.FORMAT_ETC2_RGB8     or
            ImageFormat.FORMAT_ETC2_RGBA8    or
            ImageFormat.FORMAT_ETC2_RGB8A1   or
            ImageFormat.FORMAT_ETC2_RA_AS_RG or // used to make basis universal happy
            ImageFormat.FORMAT_DXT5_RA_AS_RG or // used to make basis universal happy
            ImageFormat.FORMAT_ASTC_4x4      or
            ImageFormat.FORMAT_ASTC_4x4_HDR  => 4,
            ImageFormat.FORMAT_ASTC_8x8      or
            ImageFormat.FORMAT_ASTC_8x8_HDR  => 8,
            _                                => 1,
        };

    public static void GetFormatMinPixelSize(ImageFormat format, out int w, out int h)
    {
        switch (format)
        {
            case ImageFormat.FORMAT_DXT1: //s3tc bc1
            case ImageFormat.FORMAT_DXT3: //bc2
            case ImageFormat.FORMAT_DXT5: //bc3
            case ImageFormat.FORMAT_RGTC_R: //bc4
            case ImageFormat.FORMAT_RGTC_RG:
                //bc5        case case ImageFormat.FORMAT_DXT1:
                w = 4;
                h = 4;

                break;
            case ImageFormat.FORMAT_ETC:
                w = 4;
                h = 4;

                break;
            case ImageFormat.FORMAT_BPTC_RGBA:
            case ImageFormat.FORMAT_BPTC_RGBF:
            case ImageFormat.FORMAT_BPTC_RGBFU:
                w = 4;
                h = 4;

                break;
            case ImageFormat.FORMAT_ETC2_R11: //etc2
            case ImageFormat.FORMAT_ETC2_R11S: //signed: NOT srgb.
            case ImageFormat.FORMAT_ETC2_RG11:
            case ImageFormat.FORMAT_ETC2_RG11S:
            case ImageFormat.FORMAT_ETC2_RGB8:
            case ImageFormat.FORMAT_ETC2_RGBA8:
            case ImageFormat.FORMAT_ETC2_RGB8A1:
            case ImageFormat.FORMAT_ETC2_RA_AS_RG:
            case ImageFormat.FORMAT_DXT5_RA_AS_RG:
                w = 4;
                h = 4;

                break;
            case ImageFormat.FORMAT_ASTC_4x4:
            case ImageFormat.FORMAT_ASTC_4x4_HDR:
                w = 4;
                h = 4;

                break;
            case ImageFormat.FORMAT_ASTC_8x8:
            case ImageFormat.FORMAT_ASTC_8x8_HDR:
                w = 8;
                h = 8;

                break;
            default:
                w = 1;
                h = 1;

                break;
        }
    }

    public static int GetFormatPixelRshift(ImageFormat format) =>
        format == ImageFormat.FORMAT_ASTC_8x8
            ? 2
            : format is ImageFormat.FORMAT_DXT1 or ImageFormat.FORMAT_RGTC_R or ImageFormat.FORMAT_ETC or ImageFormat.FORMAT_ETC2_R11 or ImageFormat.FORMAT_ETC2_R11S or ImageFormat.FORMAT_ETC2_RGB8 or ImageFormat.FORMAT_ETC2_RGB8A1
                ? 1
                : 0;

    public static int GetFormatPixelSize(ImageFormat format) =>
        format switch
        {
            ImageFormat.FORMAT_ETC2_RG11     or
            ImageFormat.FORMAT_ETC2_RG11S    or
            ImageFormat.FORMAT_ETC2_RGB8     or
            ImageFormat.FORMAT_ETC2_RGBA8    or
            ImageFormat.FORMAT_ETC2_RGB8A1   or
            ImageFormat.FORMAT_ETC2_RA_AS_RG or
            ImageFormat.FORMAT_DXT5_RA_AS_RG or
            ImageFormat.FORMAT_ASTC_4x4      or
            ImageFormat.FORMAT_ASTC_4x4_HDR  or
            ImageFormat.FORMAT_ASTC_8x8      or
            ImageFormat.FORMAT_ASTC_8x8_HDR  or
            ImageFormat.FORMAT_R8            or
            ImageFormat.FORMAT_DXT3          or    // bc2
            ImageFormat.FORMAT_DXT5          or    // bc3
            ImageFormat.FORMAT_RGTC_R        or    // bc4
            ImageFormat.FORMAT_RGTC_RG       or    // bc5
            ImageFormat.FORMAT_BPTC_RGBA     or    // btpc bc6h
            ImageFormat.FORMAT_ETC           or    // etc1
            ImageFormat.FORMAT_ETC2_R11      or    // etc2
            ImageFormat.FORMAT_BPTC_RGBF     or    // float /
            ImageFormat.FORMAT_L8            or    // luminance
            ImageFormat.FORMAT_DXT1          or    // s3tc bc1
            ImageFormat.FORMAT_ETC2_R11S     or    // signed: return 1; NOT srgb.
            ImageFormat.FORMAT_BPTC_RGBFU    => 1, // unsigned float
            ImageFormat.FORMAT_RG8           or
            ImageFormat.FORMAT_RGBA4444      or
            ImageFormat.FORMAT_RGB565        or
            ImageFormat.FORMAT_RH            or    // half float
            ImageFormat.FORMAT_LA8           => 2, // luminance-alpha
            ImageFormat.FORMAT_RGB8          => 3,
            ImageFormat.FORMAT_RGBA8         or
            ImageFormat.FORMAT_RGH           or
            ImageFormat.FORMAT_RGBE9995      or
            ImageFormat.FORMAT_RF            => 4, // float
            ImageFormat.FORMAT_RGBH          => 6,
            ImageFormat.FORMAT_RGF           or
            ImageFormat.FORMAT_RGBAH         => 8,
            ImageFormat.FORMAT_RGBF          => 12,
            ImageFormat.FORMAT_RGBAF         => 16,
            _                                => 0,
        };

    public static string? GetFormatName(ImageFormat format) =>
        ERR_FAIL_INDEX_V(format, ImageFormat.FORMAT_MAX) ? default : formatNames[(int)format];

    public static int GetImageRequiredMipmaps(int width, int height, ImageFormat format)
    {
        GetDstImageSize(width, height, format, out var mm);

        return mm;
    }
    #endregion public static methods

    #region private methods
    #endregion private methods

    #region public methods
    public void AdjustBcs(float p_brightness, float p_contrast, float p_saturation) => throw new NotImplementedException();

    public void Convert(ImageFormat newFormat) => throw new NotImplementedException();

    public void ConvertRaRgba8ToRg() => throw new NotImplementedException();

    public Error Decompress() => throw new NotImplementedException();

    public void Fill(in Color color)
    {
        if (this.data.Length == 0)
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(!CanModify(this.Format), "Cannot fill in compressed or custom image formats."))
        {
            return;
        }

        var dstDataPtr = this.data;

        var pixelSize = GetFormatPixelSize(this.Format);

        // Put first pixel with the format-aware API.
        this.SetColorAtOfs(dstDataPtr, 0, color);

        RepeatPixelOverSubsequentMemory(dstDataPtr, pixelSize, this.Width * this.Height);
    }

    public Error GenerateMipmaps(bool renormalize = default)
    {
        if (ERR_FAIL_COND_V_MSG(!CanModify(this.Format), "Cannot generate mipmaps in compressed or custom image formats."))
        {
            return Error.ERR_UNAVAILABLE;
        }

        if (ERR_FAIL_COND_V_MSG(this.Format == ImageFormat.FORMAT_RGBA4444, "Cannot generate mipmaps from RGBA4444 format."))
        {
            return Error.ERR_UNAVAILABLE;
        }

        if (ERR_FAIL_COND_V_MSG(this.Width == 0 || this.Height == 0, "Cannot generate mipmaps with width or height equal to 0."))
        {
            return Error.ERR_UNCONFIGURED;
        }

        var size = GetDstImageSize(this.Width, this.Height, this.Format, out var mmcount);

        Array.Resize(ref this.data, size);

        var wp = this.data.AsSpan();

        var prevOfs = 0;
        var prevH   = this.Height;
        var prevW   = this.Width;

        for (var i = 1; i <= mmcount; i++)
        {
            this.GetMipmapOffsetAndSize(i, out var ofs, out var w, out var h);

            switch (this.Format)
            {
                case ImageFormat.FORMAT_L8:
                case ImageFormat.FORMAT_R8:
                    GeneratePo2Mipmap<byte>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 1, false, Average4Uint8, RenormalizeUint8);
                    break;
                case ImageFormat.FORMAT_LA8:
                case ImageFormat.FORMAT_RG8:
                    GeneratePo2Mipmap<byte>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 2, false, Average4Uint8, RenormalizeUint8);
                    break;
                case ImageFormat.FORMAT_RGB8:
                    if (renormalize)
                    {
                        GeneratePo2Mipmap<byte>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 3, true, Average4Uint8, RenormalizeUint8);
                    }
                    else
                    {
                        GeneratePo2Mipmap<byte>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 3, false, Average4Uint8, RenormalizeUint8);
                    }

                    break;
                case ImageFormat.FORMAT_RGBA8:
                    if (renormalize)
                    {
                        GeneratePo2Mipmap<byte>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 4, true, Average4Uint8, RenormalizeUint8);
                    }
                    else
                    {
                        GeneratePo2Mipmap<byte>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 4, false, Average4Uint8, RenormalizeUint8);
                    }
                    break;
                case ImageFormat.FORMAT_RF:
                    GeneratePo2Mipmap<float>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 1, false, Average4Float, RenormalizeFloat);
                    break;
                case ImageFormat.FORMAT_RGF:
                    GeneratePo2Mipmap<float>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 2, false, Average4Float, RenormalizeFloat);
                    break;
                case ImageFormat.FORMAT_RGBF:
                    if (renormalize)
                    {
                        GeneratePo2Mipmap<float>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 3, true, Average4Float, RenormalizeFloat);
                    }
                    else
                    {
                        GeneratePo2Mipmap<float>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 3, false, Average4Float, RenormalizeFloat);
                    }

                    break;
                case ImageFormat.FORMAT_RGBAF:
                    if (renormalize)
                    {
                        GeneratePo2Mipmap<float>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 4, true, Average4Float, RenormalizeFloat);
                    }
                    else
                    {
                        GeneratePo2Mipmap<float>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 4, false, Average4Float, RenormalizeFloat);
                    }

                    break;
                case ImageFormat.FORMAT_RH:
                    GeneratePo2Mipmap<ushort>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 1, false, Average4Half, RenormalizeHalf);
                    break;
                case ImageFormat.FORMAT_RGH:
                    GeneratePo2Mipmap<ushort>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 2, false, Average4Half, RenormalizeHalf);
                    break;
                case ImageFormat.FORMAT_RGBH:
                    if (renormalize)
                    {
                        GeneratePo2Mipmap<ushort>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 3, true, Average4Half, RenormalizeHalf);
                    }
                    else
                    {
                        GeneratePo2Mipmap<ushort>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 3, false, Average4Half, RenormalizeHalf);
                    }

                    break;
                case ImageFormat.FORMAT_RGBAH:
                    if (renormalize)
                    {
                        GeneratePo2Mipmap<ushort>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 4, true, Average4Half, RenormalizeHalf);
                    }
                    else
                    {
                        GeneratePo2Mipmap<ushort>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 4, false, Average4Half, RenormalizeHalf);
                    }

                    break;
                case ImageFormat.FORMAT_RGBE9995:
                    if (renormalize)
                    {
                        GeneratePo2Mipmap<uint>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 1, true, Average4Rgbe9995, RenormalizeRgbe9995);
                    }
                    else
                    {
                        GeneratePo2Mipmap<uint>(wp[prevOfs..], wp[ofs..], (uint)prevW, (uint)prevH, 1, false, Average4Rgbe9995, RenormalizeRgbe9995);
                    }

                    break;
                default:
                    break;
            }

            prevOfs = ofs;
            prevW   = w;
            prevH   = h;
        }

        this.HasMipmaps = true;

        return Error.OK;
    }

    public void GetMipmapOffsetAndSize(int mipmap, out int offset, out int width, out int height) //get where the mipmap begins in data
    {
        var w   = this.Width;
        var h   = this.Height;
        var ofs = 0;

        var pixelSize   = GetFormatPixelSize(this.Format);
        var pixelRshift = GetFormatPixelRshift(this.Format);
        var block       = GetFormatBlockSize(this.Format);

        GetFormatMinPixelSize(this.Format, out var minw, out var minh);

        for (var i = 0; i < mipmap; i++)
        {
            var bw = w % block != 0 ? w + (block - w % block) : w;
            var bh = h % block != 0 ? h + (block - h % block) : h;

            var s = bw * bh;

            s   *=  pixelSize;
            s   >>= pixelRshift;
            ofs +=  s;
            w   =   Math.Max(minw, w >> 1);
            h   =   Math.Max(minh, h >> 1);
        }

        offset = ofs;
        width  = w;
        height = h;
    }

    #pragma warning disable IDE0060

    public void SetData(int width, int height, bool useMipmaps, ImageFormat format, byte[] data) =>
        this.InitializeData(width, height, useMipmaps, format, data);

    public void ResizeToPo2(bool square, Interpolation interpolation = Interpolation.INTERPOLATE_BILINEAR) => throw new NotImplementedException();
    #pragma warning restore IDE0060
    #endregion public methods

    #region public overrided methods
    public override Resource Duplicate(bool subresources = false)
    {
        var copy = new Image();

        copy.CopyInternalsFrom(this);

        return copy;
    }
    #endregion public overrided methods
}
