namespace Godot.Net.Servers;

public abstract partial class RenderingServer
{
    public enum ViewportOcclusionCullingBuildQuality
    {
        VIEWPORT_OCCLUSION_BUILD_QUALITY_LOW = 0,
        VIEWPORT_OCCLUSION_BUILD_QUALITY_MEDIUM = 1,
        VIEWPORT_OCCLUSION_BUILD_QUALITY_HIGH = 2,
    };
}
