namespace Godot.Net.Scene.GUI;

using Godot.Net.Core.Math;
using Godot.Net.Scene.Resources;

#pragma warning disable CS0067 // TODO Remove

public partial class TabContainer
{
    private record ThemeCache
    {
        public Texture2D? DecrementHlIcon     { get; set; }
        public Texture2D? DecrementIcon       { get; set; }
        public Color      DropMarkColor       { get; set; }
        public Texture2D? DropMarkIcon        { get; set; }
        public Color      FontDisabledColor   { get; set; }
        public Color      FontOutlineColor    { get; set; }
        public Color      FontSelectedColor   { get; set; }
        public Color      FontUnselectedColor { get; set; }
        public int        IconSeparation      { get; set; }
        public Texture2D? IncrementHlIcon     { get; set; }
        public Texture2D? IncrementIcon       { get; set; }
        public Texture2D? MenuHlIcon          { get; set; }
        public Texture2D? MenuIcon            { get; set; }
        public int        OutlineSize         { get; set; }
        public StyleBox?  PanelStyle          { get; set; }
        public int        SideMargin          { get; set; }
        public StyleBox?  TabbarStyle         { get; set; }
        public StyleBox?  TabDisabledStyle    { get; set; }
        public Font?      TabFont             { get; set; }
        public int        TabFontSize         { get; set; }
        public StyleBox?  TabSelectedStyle    { get; set; }
        public StyleBox?  TabUnselectedStyle  { get; set; }
    }
}
