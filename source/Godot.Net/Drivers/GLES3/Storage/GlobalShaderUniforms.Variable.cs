namespace Godot.Net.Drivers.GLES3.Storage;

public partial record GlobalShaderUniforms
{
    public record Variable
    {
        public int                          BufferElements   { get; set; } //for vectors
        public int                          BufferIndex      { get; set; } //for vectors
        public object?                      OverGuide        { get; set; }
        public HashSet<Guid>                TextureMaterials { get; } = new();// materials using this
        public RS.GlobalShaderParameterType Type             { get; set; }
        public object?                      Value            { get; set; }
    };
}
