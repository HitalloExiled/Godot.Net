namespace Godot.Net.Servers;

public abstract partial class RenderingServer
{
    public enum CanvasItemTextureRepeat
    {
        CANVAS_ITEM_TEXTURE_REPEAT_DEFAULT, // Uses canvas item setting for draw command, uses global setting for canvas item.
        CANVAS_ITEM_TEXTURE_REPEAT_DISABLED,
        CANVAS_ITEM_TEXTURE_REPEAT_ENABLED,
        CANVAS_ITEM_TEXTURE_REPEAT_MIRROR,
        CANVAS_ITEM_TEXTURE_REPEAT_MAX,
    }
}
