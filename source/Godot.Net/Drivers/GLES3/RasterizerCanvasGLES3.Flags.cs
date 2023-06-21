namespace Godot.Net.Drivers.GLES3;
public partial class RasterizerCanvasGLES3
{
    [Flags]
    private enum Flags
    {
        FLAGS_INSTANCING_MASK            = 0x7F,
        FLAGS_INSTANCING_HAS_COLORS      = 1 << 7,
        FLAGS_INSTANCING_HAS_CUSTOM_DATA = 1 << 8,
        FLAGS_CLIP_RECT_UV               = 1 << 9,
        FLAGS_TRANSPOSE_RECT             = 1 << 10,
        FLAGS_NINEPACH_DRAW_CENTER       = 1 << 12,
        FLAGS_USING_PARTICLES            = 1 << 13,
        FLAGS_USE_SKELETON               = 1 << 15,
        FLAGS_NINEPATCH_H_MODE_SHIFT     = 16,
        FLAGS_NINEPATCH_V_MODE_SHIFT     = 18,
        FLAGS_LIGHT_COUNT_SHIFT          = 20,
        FLAGS_DEFAULT_NORMAL_MAP_USED    = 1 << 26,
        FLAGS_DEFAULT_SPECULAR_MAP_USED  = 1 << 27,
        FLAGS_USE_MSDF                   = 1 << 28,
        FLAGS_USE_LCD                    = 1 << 29,
        FLAGS_FLIP_H                     = 1 << 30,
        FLAGS_FLIP_V                     = 1 << 31,
    }
}
