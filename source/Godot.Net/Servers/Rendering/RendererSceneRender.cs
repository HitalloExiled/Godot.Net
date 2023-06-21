namespace Godot.Net.Servers.Rendering;

using Godot.Net.Servers.Rendering.Storage;

public abstract partial class RendererSceneRender
{
    public const int MAX_DIRECTIONAL_LIGHT_CASCADES = 4;
    public const int MAX_DIRECTIONAL_LIGHTS         = 8;
    public const int MAX_RENDER_VIEWS               = 2;

    private readonly RendererEnvironmentStorage environmentStorage = new();

    public RS.ViewportDebugDraw DebugDrawMode { get; set; }
    public double               Time          { get; set; }
    public double               TimeStep      { get; set; }

    public bool IsEnvironment(Guid environment) =>
        this.environmentStorage.IsEnvironment(environment);

    #pragma warning disable CA1822
    public uint GeometryInstanceGetPairMask() => 0;
    #pragma warning restore CA1822
    public void SetScenePass(int renderPass) => throw new NotImplementedException();

    public abstract void Update();
}
