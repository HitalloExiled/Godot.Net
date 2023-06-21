namespace Godot.Net.Servers;

using System;
using Godot.Net.Core.Math;

public abstract partial class RenderingServer
{
    public record SurfaceData
    {
        public record LOD
        {
			public List<byte> IndexData  { get; } = new();

			public float EdgeLength { get; set; }
		}

		public List<byte>    AttributeData  { get; } = new(); // Color, UV, UV2, Custom0-3.
		public List<byte>    BlendShapeData { get; } = new();
		public List<AABB>    BoneAabbs      { get; } = new();
		public List<byte>    IndexData      { get; } = new();
		public List<LOD>     Lods           { get; } = new();
		public List<byte>    SkinData       { get; } = new(); // Bone index, Bone weight.
		public List<byte>    VertexData     { get; } = new(); // Vertex, Normal, Tangent (change with skinning, blendshape).

		public AABB          Aabb        { get; set; }
		public uint          Format      { get; set; }
		public uint          IndexCount  { get; set; }
		public Guid          Material    { get; set; }
		public PrimitiveType Primitive   { get; set; } = PrimitiveType.PRIMITIVE_MAX;
		public uint          VertexCount { get; set; }
	};
}
