namespace Godot.Net.Servers;
public abstract partial class RenderingServer
{
    public enum DecalFilter
    {
        DECAL_FILTER_NEAREST,
        DECAL_FILTER_LINEAR,
        DECAL_FILTER_NEAREST_MIPMAPS,
        DECAL_FILTER_LINEAR_MIPMAPS,
        DECAL_FILTER_NEAREST_MIPMAPS_ANISOTROPIC,
        DECAL_FILTER_LINEAR_MIPMAPS_ANISOTROPIC,
    };
}
