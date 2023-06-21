#define MODULE_FREETYPE_ENABLED

namespace Godot.Net.Modules.TextServerAdv;

using System.Collections.Generic;
using Godot.Net.Core.Math;

using HfFont = HarfBuzzSharp.Font;

public partial class TextServerAdvanced
{
    private record FontForSizeAdvanced
    {
		public Dictionary<int, FontGlyph>               GlyphMap   = new();
		public Dictionary<Vector2<int>, Vector2<RealT>> KerningMap = new();
		public List<ShelfPackTexture>                   Textures   = new();

		public double       Ascent0             { get; set; }
		public double       Descent0            { get; set; }
		public HfFont?      HbHandle            { get; set; }
		public double       Oversampling        { get; set; } = 1.0;
		public double       Scale               { get; set; } = 1.0;
		public Vector2<int> Size                { get; set; }
		public double       UnderlinePosition0  { get; set; }
		public double       UnderlineThickness0 { get; set; }

        #if MODULE_FREETYPE_ENABLED
        public object? Face    { get; set; }
        public object? Stream  { get; set; }
        #endif
	}
}
