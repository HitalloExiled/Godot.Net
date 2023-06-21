namespace Godot.Net.Servers;
public abstract partial class RenderingServer
{
    public enum ViewportRenderInfo
    {
		VIEWPORT_RENDER_INFO_OBJECTS_IN_FRAME,
		VIEWPORT_RENDER_INFO_PRIMITIVES_IN_FRAME,
		VIEWPORT_RENDER_INFO_DRAW_CALLS_IN_FRAME,
		VIEWPORT_RENDER_INFO_MAX,
	}
}
