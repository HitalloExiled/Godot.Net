namespace Godot.Net.Servers.Rendering;

public abstract partial class RenderingDevice
{
    public enum TextureUsageBits
    {
        TEXTURE_USAGE_SAMPLING_BIT                 = 1 << 0,
        TEXTURE_USAGE_COLOR_ATTACHMENT_BIT         = 1 << 1,
        TEXTURE_USAGE_DEPTH_STENCIL_ATTACHMENT_BIT = 1 << 2,
        TEXTURE_USAGE_STORAGE_BIT                  = 1 << 3,
        TEXTURE_USAGE_STORAGE_ATOMIC_BIT           = 1 << 4,
        TEXTURE_USAGE_CPU_READ_BIT                 = 1 << 5,
        TEXTURE_USAGE_CAN_UPDATE_BIT               = 1 << 6,
        TEXTURE_USAGE_CAN_COPY_FROM_BIT            = 1 << 7,
        TEXTURE_USAGE_CAN_COPY_TO_BIT              = 1 << 8,
        TEXTURE_USAGE_INPUT_ATTACHMENT_BIT         = 1 << 9,
        TEXTURE_USAGE_VRS_ATTACHMENT_BIT           = 1 << 10,
    }
}
