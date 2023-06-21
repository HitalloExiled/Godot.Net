namespace Godot.Net.Scene.GUI;

using Godot.Net.Core.Math;
using Godot.Net.Scene.Resources;

public partial class LineEdit
{
    private record ThemeCache
    {
		public float      BaseScale               { get; set; } = 1;
		public Color      CaretColor              { get; set; }
		public int        CaretWidth              { get; set; }
		public Color      ClearButtonColor        { get; set; }
		public Color      ClearButtonColorPressed { get; set; }
		public Texture2D? ClearIcon               { get; set; }
		public StyleBox?  Focus                   { get; set; }
		public Font?      Font                    { get; set; }
		public Color      FontColor               { get; set; }
		public Color      FontOutlineColor        { get; set; }
		public int        FontOutlineSize         { get; set; }
		public Color      FontPlaceholderColor    { get; set; }
		public Color      FontSelectedColor       { get; set; }
		public int        FontSize                { get; set; }
		public Color      FontUneditableColor     { get; set; }
		public int        MinimumCharacterWidth   { get; set; }
		public StyleBox?  Normal                  { get; set; }
		public StyleBox?  ReadOnly                { get; set; }
		public Color      SelectionColor          { get; set; }
	}
}
