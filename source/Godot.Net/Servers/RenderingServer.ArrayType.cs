namespace Godot.Net.Servers;
public abstract partial class RenderingServer
{
    public enum ArrayType : uint
    {
        ARRAY_VERTEX  = 0, // RG32F or RGB32F (depending on 2D bit)
        ARRAY_NORMAL  = 1, // A2B10G10R10, A is ignored.
        ARRAY_TANGENT = 2, // A2B10G10R10, A flips sign of binormal.
        ARRAY_COLOR   = 3, // RGBA8
        ARRAY_TEX_UV  = 4, // RG32F
        ARRAY_TEX_UV2 = 5, // RG32F
        ARRAY_CUSTOM0 = 6, // Depends on ArrayCustomFormat.
        ARRAY_CUSTOM1 = 7,
        ARRAY_CUSTOM2 = 8,
        ARRAY_CUSTOM3 = 9,
        ARRAY_BONES   = 10, // RGBA16UI (x2 if 8 weights)
        ARRAY_WEIGHTS = 11, // RGBA16UNORM (x2 if 8 weights)
        ARRAY_INDEX   = 12, // 16 or 32 bits depending on length > 0xFFFF.
        ARRAY_MAX     = 13
    }
}
