namespace Godot.Net.Servers.Rendering;
public partial class ShaderLanguage
{
    public partial record ConstantNode
    {
        public record ValueUnion
        {
            public bool  Boolean { get; set; }
            public float Real    { get; set; }
            public int   Sint    { get; set; }
            public uint  Uint    { get; set; }
        }
    }
}
