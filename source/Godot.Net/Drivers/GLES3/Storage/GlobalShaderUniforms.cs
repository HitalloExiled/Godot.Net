namespace Godot.Net.Drivers.GLES3.Storage;
public partial record GlobalShaderUniforms
{
    public const ushort BUFFER_DIRTY_REGION_SIZE = 1024;

    public Dictionary<string, Variable> Variables { get; } = new();

    public uint                  Buffer                     { get; set; }
    public uint                  BufferDirtyRegionCount     { get; set; }
    public bool[]?               BufferDirtyRegions         { get; set; }
    public uint                  BufferSize                 { get; set; }
    public ValueUsage[]?         BufferUsage                { get; set; }
    public Value[]?              BufferValues               { get; set; }
    public Dictionary<Guid, int> InstanceBufferPos          { get; } = new();
    public List<Guid>            MaterialsUsingBuffer       { get; } = new();
    public List<Guid>            MaterialsUsingTexture      { get; } = new();
    public bool                  MustUpdateBufferMaterials  { get; set; }
    public bool                  MustUpdateTextureMaterials { get; set; }
}
