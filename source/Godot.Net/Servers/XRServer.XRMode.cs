namespace Godot.Net.Servers;

public partial class XRServer
{
    public enum XRMode
    {
        XRMODE_DEFAULT, /* Default behavior, means we check project settings */
        XRMODE_OFF, /* Ignore project settings, disable OpenXR, disable shaders */
        XRMODE_ON, /* Ignore project settings, enable OpenXR, enable shaders, run editor in VR (if applicable) */
    }
}
