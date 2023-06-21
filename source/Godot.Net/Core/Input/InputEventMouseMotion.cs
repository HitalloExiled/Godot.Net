namespace Godot.Net.Core.Input;

using Godot.Net.Core.Math;

public class InputEventMouseMotion : InputEventMouse
{
    public bool           PenInverted { get; set; }
    public RealT          Pressure    { get; set; }
    public Vector2<RealT> Relative    { get; set; }
    public Vector2<RealT> Tilt        { get; set; }
    public Vector2<RealT> Velocity    { get; set; }
}
