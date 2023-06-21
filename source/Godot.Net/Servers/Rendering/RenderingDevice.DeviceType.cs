namespace Godot.Net.Servers.Rendering;
public abstract partial class RenderingDevice
{
    public enum DeviceType
    {
        DEVICE_TYPE_OTHER,
        DEVICE_TYPE_INTEGRATED_GPU,
        DEVICE_TYPE_DISCRETE_GPU,
        DEVICE_TYPE_VIRTUAL_GPU,
        DEVICE_TYPE_CPU,
        DEVICE_TYPE_MAX,
    }
}
