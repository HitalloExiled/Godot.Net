namespace Godot.Net.Drivers.GLES3;

using Godot.Net.Core.Math;

public record FBOCacheEntry
{
    public List<uint>   AllocatedTextures       { get; set; } = new();
    public int          AllocatedTexturesLength { get; set; }
    public uint         Fbo                     { get; set; }
    public Vector2<int> Size                    { get; set; }
}
