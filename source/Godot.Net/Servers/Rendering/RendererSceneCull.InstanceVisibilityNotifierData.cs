namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core;

public partial class RendererSceneCull
{
    private record InstanceVisibilityNotifierData
    {
        public Guid                                     Base           { get; set; }
        public bool                                     JustVisible    { get; set; }
        public SelfList<InstanceVisibilityNotifierData> ListElement    { get; }
        public ulong                                    VisibleInFrame { get; set; }

        public InstanceVisibilityNotifierData() => this.ListElement = new(this);
    };
}
