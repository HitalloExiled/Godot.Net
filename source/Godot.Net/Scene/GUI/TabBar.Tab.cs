namespace Godot.Net.Scene.GUI;

using Godot.Net.Core.Math;
using Godot.Net.Scene.Resources;

public partial class TabBar
{
    private record Tab
    {
        public Rect2<RealT>  CbRect        { get; set; } = new();
        public bool          Disabled      { get; set; }
        public bool          Hidden        { get; set; }
        public Texture2D?    Icon          { get; set; }
        public string        Language      { get; set; } = "";
        public int           OfsCache      { get; set; }
        public Rect2<RealT>  RbRect        { get; set; }
        public Texture2D?    RightButton   { get; set; }
        public int           SizeCache     { get; set; }
        public int           SizeText      { get; set; }
        public string        Text          { get; set; } = "";
        public TextLine?     TextBuf       { get; set; }
        public TextDirection TextDirection { get; set; } = TextDirection.TEXT_DIRECTION_INHERITED;
        public string        XlText        { get; set; } = "";
    };
}
