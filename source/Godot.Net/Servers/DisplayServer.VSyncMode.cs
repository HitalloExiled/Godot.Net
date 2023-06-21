namespace Godot.Net.Servers;

public abstract partial class DisplayServer
{
    public enum VSyncMode
    {
        VSYNC_DISABLED,
        VSYNC_ENABLED,
        VSYNC_ADAPTIVE,
        VSYNC_MAILBOX
    };
}
