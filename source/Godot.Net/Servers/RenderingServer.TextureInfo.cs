namespace Godot.Net.Servers;

using Godot.Net.Core.IO;

public abstract partial class RenderingServer
{
    public record TextureInfo
    {
        public int         Bytes   { get; set; }
        public uint        Depth   { get; set; }
        public ImageFormat Format  { get; set; }
        public uint        Height  { get; set; }
        public string?     Path    { get; set; }
        public Guid        Texture { get; set; }
        public uint        Width   { get; set; }
    };
}
