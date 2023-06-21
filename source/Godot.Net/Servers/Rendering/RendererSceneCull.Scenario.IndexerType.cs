namespace Godot.Net.Servers.Rendering;
#pragma warning disable CS0649,IDE0044,IDE0052,IDE0059,IDE0060 // TODO - Remove

public partial class RendererSceneCull
{
    public partial record Scenario
    {
        public enum IndexerType
        {
            INDEXER_GEOMETRY, //for geometry
            INDEXER_VOLUMES, //for everything else
            INDEXER_MAX
        };
    }
}
