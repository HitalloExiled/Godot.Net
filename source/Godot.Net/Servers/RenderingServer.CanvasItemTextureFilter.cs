namespace Godot.Net.Servers;

public abstract partial class RenderingServer
{
    public enum CanvasItemTextureFilter
    {
        CANVAS_ITEM_TEXTURE_FILTER_DEFAULT, // Uses canvas item setting for draw command, uses global setting for canvas item.
        CANVAS_ITEM_TEXTURE_FILTER_NEAREST,
        CANVAS_ITEM_TEXTURE_FILTER_LINEAR,
        CANVAS_ITEM_TEXTURE_FILTER_NEAREST_WITH_MIPMAPS,
        CANVAS_ITEM_TEXTURE_FILTER_LINEAR_WITH_MIPMAPS,
        CANVAS_ITEM_TEXTURE_FILTER_NEAREST_WITH_MIPMAPS_ANISOTROPIC,
        CANVAS_ITEM_TEXTURE_FILTER_LINEAR_WITH_MIPMAPS_ANISOTROPIC,
        CANVAS_ITEM_TEXTURE_FILTER_MAX
    };
}
