namespace Godot.Net.Drivers.GLES3.Storage;
public partial record GlobalShaderUniforms
{
    public record ValueUsage
    {
        public uint Elements { get; set; }
    };
}
