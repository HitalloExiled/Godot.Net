namespace Godot.Net.Drivers.GLES3;

using System.Collections.Generic;

public partial class RasterizerCanvasGLES3
{
    private record DataBuffer
    {
        public nint       Fence           { get; set; }
        public List<uint> InstanceBuffers { get; } = new();
        public ulong?     LastFrameUsed   { get; set; }
        public uint       LightUbo        { get; set; }
        public uint       StateUbo        { get; set; }
    };
}
