namespace Godot.Net.Modules.RayCast;

using Godot.Net.Core.Math;
using Godot.Net.Servers.Rendering;

#pragma warning disable IDE0044,IDE0052 // TODO Remove

public partial class RaycastOcclusionCull : RendererSceneOcclusionCull
{
    private static RaycastOcclusionCull? raycastSingleton;

    private readonly Dictionary<Guid, Scenario> scenarios = new();

    private RS.ViewportOcclusionCullingBuildQuality buildQuality;

    public static RaycastOcclusionCull RaycastSingleton => raycastSingleton ?? throw new NullReferenceException();

    public RaycastOcclusionCull()
    {
        raycastSingleton = this;

        var defaultQuality = GLOBAL_GET<int>("rendering/occlusion_culling/bvh_build_quality");

        this.buildQuality = (RS.ViewportOcclusionCullingBuildQuality)defaultQuality;
    }

    public override void AddBuffer(Guid viewportId) => throw new NotImplementedException();

    // void RaycastOcclusionCull::add_scenario(RID p_scenario)
    public override void AddScenario(Guid scenarioId)
    {
        if (this.scenarios.TryGetValue(scenarioId, out var value))
        {
            value.Removed = false;
        }
        else
        {
            this.scenarios.Add(scenarioId, new());
        }
    }

    public override void BufferSetScenario(Guid viewportId, Guid scenario) => throw new NotImplementedException();
    public override void BufferSetSize(Guid id, Vector2<int> newSize) => throw new NotImplementedException();
    public override void BufferUpdate(Guid viewportId, Transform3D<RealT> mainTransform, Projection<RealT> mainProjection, bool isOrthogonal) => throw new NotImplementedException();
    public override void RemoveBuffer(Guid viewportId) => throw new NotImplementedException();
}
