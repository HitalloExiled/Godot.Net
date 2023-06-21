namespace Godot.Net.Scene.GUI;

public partial class LineEdit
{
    private record ClearButtonStatus
    {
        public bool PressAttempt   { get; set; }
		public bool PressingInside { get; set; }
    }
}
