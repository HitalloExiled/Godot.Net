namespace Godot.Net.Scene.GUI;

public partial class LineEdit
{
    private record Selection
    {
		public int  Begin       { get; set; }
		public bool Creating    { get; set; }
		public bool DoubleClick { get; set; }
		public bool DragAttempt { get; set; }
		public bool Enabled     { get; set; }
		public int  End         { get; set; }
		public int  StartColumn { get; set; }
	} ;
}
