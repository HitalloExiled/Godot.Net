namespace Godot.Net.Core.Input;

public class InputEventMouseButton : InputEventMouse
{
    private MouseButton buttonIndex;

    public MouseButton ButtonIndex { get => this.buttonIndex; set => this.SetButtonIndex(value); }
    public bool        DoubleClick { get; set; }
    public double      Factor      { get; set; }
    public bool        Pressed     { get; set; }

    private void SetButtonIndex(MouseButton value)
    {
        this.buttonIndex = value;
        this.EmitChanged();
    }
}
