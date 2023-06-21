namespace Godot.Net.Modules.TextServerAdv;

using System;
using System.Collections.Generic;
using Godot.Net.Core.Math;

public partial class TextServerAdvanced
{
    private record FontAdvanced
    {
		public Dictionary<Vector2<int>, FontForSizeAdvanced> Cache                    { get; } = new();
		public Dictionary<string, object>                    FeatureOverrides         { get; } = new();
		public Dictionary<string, bool>                      LanguageSupportOverrides { get; } = new();
		public Mutex                                         Mutex                    { get; } = new();
		public Dictionary<string, bool>                      ScriptSupportOverrides   { get; } = new();
		public Dictionary<string, object>                    SupportedFeatures        { get; } = new();
		public HashSet<uint>                                 SupportedScripts         { get; } = new();
		public Dictionary<string, object>                    SupportedVaraitions      { get; } = new();
		public Transform2D<RealT>                            Transform                { get; } = new();
		public Dictionary<string, object>                    VariationCoordinates     { get; } = new();

		public bool                                          AllowSystemFallback      { get; set; } = true;
		public FontAntialiasing                              Antialiasing             { get; set; } = FontAntialiasing.FONT_ANTIALIASING_GRAY;
		public byte[]                                        Data                     { get; set; } = Array.Empty<byte>();
		public double                                        Embolden                 { get; set; }
		public int                                           FaceIndex                { get; set; }
		public bool                                          FaceInit                 { get; set; }
		public int                                           FixedSize                { get; set; }
		public string                                        FontName                 { get; set; } = "";
		public bool                                          ForceAutohinter          { get; set; }
		public Hinting                                       Hinting                  { get; set; } = Hinting.HINTING_LIGHT;
		public bool                                          Mipmaps                  { get; set; }
		public bool                                          Msdf                     { get; set; }
		public int                                           MsdfRange                { get; set; } = 14;
		public int                                           MsdfSourceSize           { get; set; } = 48;
		public double                                        Oversampling             { get; set; }
		public int                                           Stretch                  { get; set; } = 100;
		public FontStyle                                     StyleFlags               { get; set; }
		public string                                        StyleName                { get; set; } = "";
		public SubpixelPositioning                           SubpixelPositioning      { get; set; } = SubpixelPositioning.SUBPIXEL_POSITIONING_AUTO;
		public int                                           Weight                   { get; set; } = 400;
	}
}
