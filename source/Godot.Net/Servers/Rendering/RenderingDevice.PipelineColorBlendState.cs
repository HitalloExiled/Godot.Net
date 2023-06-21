namespace Godot.Net.Servers.Rendering;

public abstract partial class RenderingDevice
{
    public record PipelineColorBlendState
    {
        public static PipelineColorBlendState CreateBlend() => throw new NotImplementedException();
        public static PipelineColorBlendState CreateDisabled() => throw new NotImplementedException();
    }
}
