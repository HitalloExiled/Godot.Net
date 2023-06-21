namespace Godot.Net.Scene.Main;

public partial class Viewport
{
    public enum MSAA
    {
        MSAA_DISABLED,
        MSAA_2X,
        MSAA_4X,
        MSAA_8X,
        // 16x MSAA is not supported due to its high cost and driver bugs.
        MSAA_MAX
    }
}
