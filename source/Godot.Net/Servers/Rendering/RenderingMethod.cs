namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;
using Godot.Net.Servers.Rendering.Storage;

public abstract class RenderingMethod
{
    public record RenderInfo
    {
        public int[,] Info { get; } = new int[RS.VIEWPORT_RENDER_INFO_TYPE_MAX, RS.VIEWPORT_RENDER_INFO_MAX];
    }

    public static RendererSceneCull Singleton { get; protected set; } = null!;

    public virtual RS.ViewportDebugDraw DebugDrawMode { get; set; }

    public RenderingMethod()
    { }

    public abstract RS.EnvironmentBG EnvironmentGetBackground(Guid environmentId);
    public abstract int EnvironmentGetCanvasMaxLayer(Guid environmentId);
    public abstract bool IsCamera(Guid cameraId);
    public abstract bool IsEnvironment(Guid environmentId);
    public abstract bool IsScenario(Guid scenarioId);
    public abstract RenderSceneBuffers RenderBuffersCreate();
    public abstract void RenderCamera(
        RenderSceneBuffers          renderBuffers,
        Guid                        cameraId,
        Guid                        scenarioId,
        Guid                        viewportId,
        Vector2<RealT>              viewPortSize,
        bool                        useTaa,
        RealT                       screenMeshLodThreshold,
        Guid                        shadowAtlas,
        object?                     xrInterface, // TODO - XRInterface xrInterface,
        RenderInfo                  renderInfo
    );
    public abstract void RenderEmptyScene(RenderSceneBuffers? renderBuffers, Guid scenarioId, Guid shadowAtlas);
    public abstract Guid ScenarioGetEnvironment(Guid scenarioId);
    public abstract void ScenarioInitialize(Guid scenarioId);
    public abstract void ScenarioRemoveViewportVisibilityMask(Guid scenarioId, Guid viewportId);
    public abstract void Update();
    public abstract void UpdateVisibilityNotifiers();
}
