namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;

public class RendererSceneOcclusionCull
{
    private static bool warning;

    public static RendererSceneOcclusionCull Singleton { get; private set; } = null!;

    public RendererSceneOcclusionCull() => Singleton = this;

    private static void PrintWarning() => WARN_PRINT_ONCE("Occlusion culling is disabled at build-time.", ref warning);

    public virtual void AddBuffer(Guid viewportId) => PrintWarning();
    public virtual void AddScenario(Guid scenarioId) { /* Must be overrided */ }
    public virtual void BufferSetScenario(Guid viewportId, Guid scenario) => PrintWarning();
    public virtual void BufferSetSize(Guid id, Vector2<int> newSize) => PrintWarning();
    public virtual void BufferUpdate(Guid viewportId, Transform3D<RealT> mainTransform, Projection<RealT> mainProjection, bool isOrthogonal) { /* Must be overrided */ }
    public virtual void RemoveBuffer(Guid viewportId) => PrintWarning();
}
