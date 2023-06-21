namespace Godot.Net.Servers;

public abstract partial class RenderingServer
{
    public enum ViewportUpdateMode
    {
        VIEWPORT_UPDATE_DISABLED,
        VIEWPORT_UPDATE_ONCE, // Then goes to disabled, must be manually updated.
        VIEWPORT_UPDATE_WHEN_VISIBLE, // Default
        VIEWPORT_UPDATE_WHEN_PARENT_VISIBLE,
        VIEWPORT_UPDATE_ALWAYS
    };
}
