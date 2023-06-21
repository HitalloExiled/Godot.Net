namespace Godot.Net.Servers.Rendering.Storage;

using Godot.Net.Core.Config;

public abstract partial class RendererMaterialStorage
{
    public record InstanceShaderParam
    {
        public object?       DefaultValue { get; set; }
        public int           Index        { get; set; }
        public PropertyInfo? Info         { get; set; }
    };
}
