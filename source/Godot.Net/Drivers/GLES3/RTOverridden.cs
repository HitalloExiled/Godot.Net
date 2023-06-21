namespace Godot.Net.Drivers.GLES3;

public record RTOverridden
{
    public Guid                            Color        { get; set; }
    public Guid                            Depth        { get; set; }
    public Dictionary<uint, FBOCacheEntry> FboCache     { get; } = new();
    public bool                            IsOverridden { get; set; }
    public Guid                            Velocity     { get; set; }
}
