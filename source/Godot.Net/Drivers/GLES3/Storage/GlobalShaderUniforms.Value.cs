namespace Godot.Net.Drivers.GLES3.Storage;
public partial record GlobalShaderUniforms
{
    public record struct Value
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
    }
}
