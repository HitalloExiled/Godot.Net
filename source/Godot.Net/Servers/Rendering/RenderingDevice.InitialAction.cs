namespace Godot.Net.Servers.Rendering;

public abstract partial class RenderingDevice
{
    public enum InitialAction
    {
        INITIAL_ACTION_CLEAR, //start rendering and clear the whole framebuffer (region or not) (supply params)
        INITIAL_ACTION_CLEAR_REGION, //start rendering and clear the framebuffer in the specified region (supply params)
        INITIAL_ACTION_CLEAR_REGION_CONTINUE, //continue rendering and clear the framebuffer in the specified region (supply params)
        INITIAL_ACTION_KEEP, //start rendering, but keep attached color texture contents (depth will be cleared)
        INITIAL_ACTION_DROP, //start rendering, ignore what is there, just write above it
        INITIAL_ACTION_CONTINUE, //continue rendering (framebuffer must have been left in "continue" state as final action previously)
        INITIAL_ACTION_MAX
    }
}
