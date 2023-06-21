namespace Godot.Net.Servers.Rendering;

public abstract partial class RenderingDevice
{
    [Flags]
    public enum PipelineDynamicStateFlags
    {
        DYNAMIC_STATE_LINE_WIDTH = 1 << 0,
        DYNAMIC_STATE_DEPTH_BIAS = 1 << 1,
        DYNAMIC_STATE_BLEND_CONSTANTS = 1 << 2,
        DYNAMIC_STATE_DEPTH_BOUNDS = 1 << 3,
        DYNAMIC_STATE_STENCIL_COMPARE_MASK = 1 << 4,
        DYNAMIC_STATE_STENCIL_WRITE_MASK = 1 << 5,
        DYNAMIC_STATE_STENCIL_REFERENCE = 1 << 6,
    }
}
