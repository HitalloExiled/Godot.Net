namespace Godot.Net.Servers;

using Godot.Net.Servers.Rendering;
using Godot.Net.Servers.Rendering.Environment;
using Godot.Net.Servers.Rendering.Storage;

public static class RenderingServerGlobals
{
    public static RendererCameraAttributes CameraAttributes { get; set; } = null!;
    public static RendererCanvasCull       Canvas           { get; set; } = null!;
    public static RendererCanvasRender     CanvasRender     { get; set; } = null!;
    public static RendererFog              Fog              { get; set; } = null!;
    public static RendererGI               Gi               { get; set; } = null!;
    public static RendererLightStorage     LightStorage     { get; set; } = null!;
    public static RendererMaterialStorage  MaterialStorage  { get; set; } = null!;
    public static RendererMeshStorage      MeshStorage      { get; set; } = null!;
    public static RendererParticlesStorage ParticlesStorage { get; set; } = null!;
    public static RendererCompositor       Rasterizer       { get; set; } = null!;
    public static RenderingMethod          Scene            { get; set; } = null!;
    public static RendererTextureStorage   TextureStorage   { get; set; } = null!;
    public static bool                     Threaded         { get; set; }
    public static RendererUtilities        Utilities        { get; set; } = null!;
    public static RendererViewport         Viewport         { get; set; } = null!;
}
