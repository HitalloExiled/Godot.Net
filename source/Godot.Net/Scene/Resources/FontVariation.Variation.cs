namespace Godot.Net.Scene.Resources;

using Godot.Net.Core.Math;

public partial class FontVariation
{
    private record Variation
    {
		public RealT                      Embolden  { get; set; }
		public int                        FaceIndex { get; set; }
		public Dictionary<object, object> Opentype  { get; } = new();
		public Transform2D<RealT>         Transform { get; set; }
	}
}
