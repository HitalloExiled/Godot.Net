namespace Godot.Net.Modules.TextServerAdv;

using System;
using System.Collections.Generic;
using Godot.Net.Core.Math;

public partial class TextServerAdvanced
{
    private record SystemFontKey
    {
		public FontAntialiasing        Antialiasing         { get; set; } = FontAntialiasing.FONT_ANTIALIASING_GRAY;
		public double                  Embolden             { get; set; }
		public int                     FixedSize            { get; set; }
		public string                  FontName             { get; set; } = "";
		public bool                    ForceAutohinter      { get; set; }
		public Hinting                 Hinting              { get; set; } = Hinting.HINTING_LIGHT;
		public bool                    Italic               { get; set; }
		public bool                    Mipmaps              { get; set; }
		public bool                    Msdf                 { get; set; }
		public int                     MsdfRange            { get; set; } = 14;
		public int                     MsdfSourceSize       { get; set; } = 48;
		public double                  Oversampling         { get; set; }
		public int                     Stretch              { get; set; } = 100;
		public SubpixelPositioning     SubpixelPositioning  { get; set; } = SubpixelPositioning.SUBPIXEL_POSITIONING_AUTO;
		public Transform2D<RealT>      Transform            { get; set; } = new();
		public Dictionary<uint, int>   VariationCoordinates { get; set; } = new();
		public int                     Weight               { get; set; } = 400;

        public SystemFontKey() { }

		public SystemFontKey(string fontName, bool italic, int weight, int stretch, Guid font, TextServerAdvanced fb)
        {
            this.FontName             = fontName;
            this.Italic               = italic;
            this.Weight               = weight;
            this.Stretch              = stretch;
            this.Antialiasing         = fb.FontGetAntialiasing(font);
            this.Mipmaps              = fb.FontGetGenerateMipmaps(font);
            this.Msdf                 = fb.FontIsMultichannelSignedDistanceField(font);
            this.MsdfRange            = fb.FontGetMsdfPixelRange(font);
            this.MsdfSourceSize       = fb.FontGetMsdfSize(font);
            this.FixedSize            = fb.FontGetFixedSize(font);
            this.ForceAutohinter      = fb.FontIsForceAutohinter(font);
            this.Hinting              = fb.FontGetHinting(font);
            this.SubpixelPositioning  = fb.FontGetSubpixelPositioning(font);
            this.VariationCoordinates = fb.FontGetVariationCoordinates(font);
            this.Oversampling         = fb.FontGetOversampling(font);
            this.Embolden             = fb.FontGetEmbolden(font);
            this.Transform            = fb.FontGetTransform(font);
		}
	};
}
