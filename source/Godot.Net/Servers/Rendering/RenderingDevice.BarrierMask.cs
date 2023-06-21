namespace Godot.Net.Servers.Rendering;

public abstract partial class RenderingDevice
{
    [Flags]
    public enum BarrierMask
    {
        BARRIER_MASK_RASTER = 1,
        BARRIER_MASK_COMPUTE = 2,
        BARRIER_MASK_TRANSFER = 4,
        BARRIER_MASK_ALL_BARRIERS = BARRIER_MASK_RASTER | BARRIER_MASK_COMPUTE | BARRIER_MASK_TRANSFER, // 7
        BARRIER_MASK_NO_BARRIER = 8,
    }
}
