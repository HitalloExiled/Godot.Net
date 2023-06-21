namespace Godot.Net.Platforms.Windows.Native;

internal static partial class GDI32
{
    public enum PIXEL_TYPE
    {
        /// <summary>
        /// RGBA pixels. Each pixel has four components in this order: red, green, blue, and alpha.
        /// </summary>
        PFD_TYPE_RGBA = 0,

        /// <summary>
        /// Color-index pixels. Each pixel uses a color-index value.
        /// </summary>
        PFD_TYPE_COLORINDEX = 1,
    }
}
