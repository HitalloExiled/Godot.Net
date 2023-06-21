namespace Godot.Net.Servers;

public abstract partial class RenderingServer
{
    public enum EnvironmentBG
    {
        ENV_BG_CLEAR_COLOR,
        ENV_BG_COLOR,
        ENV_BG_SKY,
        ENV_BG_CANVAS,
        ENV_BG_KEEP,
        ENV_BG_CAMERA_FEED,
        ENV_BG_MAX
    };
}
