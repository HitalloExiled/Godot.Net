namespace Godot.Net.Drivers.GLES3;
using System.Collections.Generic;
using Godot.Net.Core.Math;

public partial class RasterizerCanvasGLES3
{
    private record PolygonBuffers
    {
		public Color Color         { get; set; }
		public bool  ColorDisabled { get; set; }
		public int   Count         { get; set; }
		public uint  IndexBuffer   { get; set; }
		public uint  VertexArray   { get; set; }
		public uint  VertexBuffer  { get; set; }
	}

    private record PolygonBuffersData
    {
        public ulong                             LastId   { get; set; }
        public Dictionary<ulong, PolygonBuffers> Polygons { get; } = new();
    }
}
