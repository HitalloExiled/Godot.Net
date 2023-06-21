#pragma warning disable IDE0051,CS0169,IDE0044,CS0649 // TODO - Remove

namespace Godot.Net.Core.OS;



public abstract partial class OS
{
    public enum RenderThreadModeType
    {
        NONE = -1,
        RENDER_THREAD_UNSAFE,
        RENDER_THREAD_SAFE,
        RENDER_SEPARATE_THREAD
    };
}
