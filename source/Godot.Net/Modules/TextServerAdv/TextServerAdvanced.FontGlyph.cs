namespace Godot.Net.Modules.TextServerAdv;

using Godot.Net.Core.Math;

public partial class TextServerAdvanced
{
    private record FontGlyph
    {
		public Vector2<RealT> Advance    { get; set; }
		public bool           Found      { get; set; }
		public Rect2<RealT>   Rect       { get; set; }
		public int            TextureIdx { get; set; } = -1;
		public Rect2<RealT>   UvRect     { get; set; }
	}
}
