namespace Godot.Net.Modules.SVG;

using Godot.Net.Core.Error;
using Godot.Net.Core.IO;
using Godot.Net.Core.Math;
using Godot.Net.Extensions;
using SkiaSharp;
using SKSvg = SkiaSharp.Extended.Svg.SKSvg;

#pragma warning disable IDE0052 // TODO Remove;

public class ImageLoaderSVG : ImageFormatLoader
{
    private static Dictionary<Color, Color> forcedColorMap = new();

    private static void ReplaceColorProperty(Dictionary<Color, Color> colorMap, string prefix, ref string refString)
    {
        // Replace colors in the SVG based on what is passed in `colorMap`.
        // Used to change the colors of editor icons based on the used theme.
        // The strings being replaced are typically of the form:
        //   fill="#5abbef"
        // But can also be 3-letter codes, include alpha, be "none" or a named color
        // string ("blue"). So we convert to Godot Color to compare with `colorMap`.

        var prefixLen = prefix.Length;
        var pos       = refString.IndexOf(prefix);

        while (pos != -1)
        {
            pos += prefixLen; // Skip prefix.

            var endPos = refString.IndexOf("\"", pos);

            if (ERR_FAIL_COND_MSG(endPos == -1, $"Malformed SVG string after property \"{prefix}\"."))
            {
                return;
            }

            var colorCode = refString[pos..endPos];

            if (colorCode != "none" && !colorCode.StartsWith("url("))
            {
                var color = new Color(colorCode); // Handles both HTML codes and named colors.

                if (colorMap.TryGetValue(color, out var value))
                {
                    refString = refString.Left(pos) + "#" + value.ToHtml(false) + refString[endPos..];
                }
            }

            // Search for other occurrences.
            pos = refString.IndexOf(prefix, pos);
        }
    }

    public static Error CreateImageFromString(
        Image                    image,
        string                   @string,
        float                    scale,
        bool                     upsample,
        Dictionary<Color, Color> colorMap
    )
    {
        if (colorMap.Count != 0)
        {
            ReplaceColorProperty(colorMap, "stop-color=\"", ref @string);
            ReplaceColorProperty(colorMap, "fill=\"", ref @string);
            ReplaceColorProperty(colorMap, "stroke=\"", ref @string);
        }

        var svg = @string.ToUTF8();

        return CreateImageFromUtf8Buffer(image, svg.ToBytes(), scale, upsample);
    }

    public static Error CreateImageFromUtf8Buffer(Image image, byte[] source, float scale, bool _)
    {
        if (ERR_FAIL_COND_V_MSG(MathX.IsZeroApprox(scale), "ImageLoaderSVG: Can't load SVG with a scale of 0."))
        {
            return Error.ERR_INVALID_PARAMETER;
        }

        var svg = new SKSvg();

        using var svgStream = new MemoryStream(source);

        svg.Load(svgStream);

        var fw = svg.CanvasSize.Width;
        var fh = svg.CanvasSize.Height;

        var width  = (int)Math.Round(fw * scale);
        var height = (int)Math.Round(fh * scale);

        const uint MAX_DIMENSION = 16384;

        if (width > MAX_DIMENSION || height > MAX_DIMENSION)
        {
            WARN_PRINT($"ImageLoaderSVG: Target canvas dimensions {width}×{height} (with scale {scale:0.00}) exceed the max supported dimensions {MAX_DIMENSION}×{MAX_DIMENSION}. The target canvas will be scaled down.");
            width  = (int)Math.Min(width, MAX_DIMENSION);
            height = (int)Math.Min(height, MAX_DIMENSION);
        }


        var imageInfo = new SKImageInfo(width, height);

        using var surface = SKSurface.Create(imageInfo);
        using var canvas = surface.Canvas;

        var scaleX = width / svg.Picture.CullRect.Width;
        var scaleY = height / svg.Picture.CullRect.Height;
        var matrix = SKMatrix.CreateScale((float)scaleX, (float)scaleY);

        // draw the svg
        canvas.Clear(SKColors.Transparent);
        canvas.DrawPicture(svg.Picture, ref matrix);
        canvas.Flush();

        using var imageStream = new MemoryStream();

        using var data     = surface.Snapshot();
        using var pngImage = data.Encode(SKEncodedImageFormat.Png, 100);

        pngImage.SaveTo(imageStream);

        var buffer = imageStream.ToArray();

        Array.Resize(ref buffer, width * height * sizeof(uint));

        var imageBuffer = new byte[width * height * sizeof(uint)];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var n = buffer[y * width + x];
                var offset = sizeof(uint) * width * y + sizeof(uint) * x;
                imageBuffer[offset + 0] = (byte)((n >> 16) & 0xff);
                imageBuffer[offset + 1] = (byte)((n >> 8) & 0xff);
                imageBuffer[offset + 2] = (byte)(n & 0xff);
                imageBuffer[offset + 3] = (byte)((n >> 24) & 0xff);
            }
        }

        image.SetData(width, height, false, ImageFormat.FORMAT_RGBA8, imageBuffer);

        return Error.OK;
    }

    public static void SetForcedColorMap(Dictionary<Color, Color> colorMap) =>
        forcedColorMap = colorMap;
}
