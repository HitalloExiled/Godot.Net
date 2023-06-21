namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core;
using Godot.Net.Core.Generics;
using Godot.Net.Core.Math;

#pragma warning disable CS0649,IDE0044,IDE0052,IDE0059,IDE0060 // TODO - Remove

public partial class RendererSceneCull
{
    public partial record Scenario
    {
        public Guid                       CameraAttributes           { get; set; }
        public List<Instance>             DirectionalLights          { get; } = new();
        public List<Guid>                 DynamicLights              { get; } = new();
        public Guid                       Environment                { get; set; }
        public Guid                       FallbackEnvironment        { get; set; }
        public DynamicBVH[]               Indexers                   { get; } = new DynamicBVH[(int)IndexerType.INDEXER_MAX];
        public PagedArray<InstanceBounds> InstanceAABBs              { get; } = new();
        public PagedArray<InstanceData>   InstanceData               { get; } = new();
        public VisibilityArray            InstanceVisibility         { get; } = new();
        public SelfList<Instance>.List    Instances                  { get; } = new();
        public Guid                       ReflectionAtlas            { get; set; }
        public Guid                       ReflectionProbeShadowAtlas { get; set; }
        public Guid                       Self                       { get; set; }
        public ulong                      UsedViewportVisibilityBits { get; set; }
        public Dictionary<Guid, ulong>    ViewportVisibilityMasks    { get; } = new();

        public Scenario()
        {
            for (var i = 0; i < (int)IndexerType.INDEXER_MAX; i++)
            {
                this.Indexers[i] = new();
            }

            this.Indexers[(int)IndexerType.INDEXER_GEOMETRY].Index = (uint)IndexerType.INDEXER_GEOMETRY;
            this.Indexers[(int)IndexerType.INDEXER_VOLUMES].Index  = (uint)IndexerType.INDEXER_VOLUMES;
            this.UsedViewportVisibilityBits = 0;
        }
    }
}
