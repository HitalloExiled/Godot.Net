namespace Godot.Net.Scene.GUI;

public partial class LineEdit
{
    private record TextOperation
    {
		public int    CaretColumn  { get; set; }
		public float  ScrollOffset { get; set; }
		public string Text         { get; set; } = "";
	}
}
