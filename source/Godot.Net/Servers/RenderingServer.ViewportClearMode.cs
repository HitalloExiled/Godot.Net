namespace Godot.Net.Servers;

public abstract partial class RenderingServer
{
    public enum ViewportClearMode
    {
        VIEWPORT_CLEAR_ALWAYS,
        VIEWPORT_CLEAR_NEVER,
        VIEWPORT_CLEAR_ONLY_NEXT_FRAME
    };
}
