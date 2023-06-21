namespace Godot.Net.Servers;

public abstract partial class RenderingServer
{
    public enum ViewportMSAA
    {
        VIEWPORT_MSAA_DISABLED,
        VIEWPORT_MSAA_2X,
        VIEWPORT_MSAA_4X,
        VIEWPORT_MSAA_8X,
        VIEWPORT_MSAA_MAX,
    };
}
