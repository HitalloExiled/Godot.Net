namespace Godot.Net.Scene.GUI;

using Godot.Net.Core.Math;
using Godot.Net.Scene.Resources;

#pragma warning disable IDE0044, IDE0052, IDE0051, CS0169, CS0414 // TODO Remove

public partial class Label
{
    private record ThemeCache
    {
        public Font?          Font                  { get; set; }
        public Color          FontColor             { get; set; }
        public Color          FontOutlineColor      { get; set; }
        public int            FontOutlineSize       { get; set; }
        public Color          FontShadowColor       { get; set; }
        public Vector2<RealT> FontShadowOffset      { get; set; }
        public int            FontShadowOutlineSize { get; set; }
        public int            FontSize              { get; set; }
        public int            LineSpacing           { get; set; }
        public StyleBox       NormalStyle           { get; set; } = new();
    }
}
