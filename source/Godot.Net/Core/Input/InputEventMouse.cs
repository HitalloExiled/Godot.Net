namespace Godot.Net.Core.Input;

using Godot.Net.Core.Math;

public class InputEventMouse : InputEventWithModifiers
{
    private MouseButtonMask buttonMask;

    public MouseButtonMask ButtonMask     { get => this.buttonMask; set => this.SetButtonMask(value); }
    public Vector2<RealT>  GlobalPosition { get; set; }
    public Vector2<RealT>  Position       { get; set; }

    private void SetButtonMask(MouseButtonMask value)
    {
        this.buttonMask = value;
        this.EmitChanged();
    }
}
