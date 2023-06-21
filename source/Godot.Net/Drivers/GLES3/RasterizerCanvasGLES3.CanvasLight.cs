namespace Godot.Net.Drivers.GLES3;

using Godot.Net.Core.Math;

public partial class RasterizerCanvasGLES3
{
    public record CanvasLight
    {
        public record ShadowRecord
        {
            public Transform2D<RealT> DirectionalXform { get; set; } = new();
            public bool               Enabled          { get; set; }
            public float              YOffset          { get; set; }
            public float              ZFar             { get; set; }
        }

        public ShadowRecord Shadow  { get; } = new();
        public Guid         Texture { get; set; }
    };
}
