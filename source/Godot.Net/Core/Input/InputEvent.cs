namespace Godot.Net.Core.Input;

using Godot.Net.Core.IO;
using Godot.Net.Core.Math;

public abstract class InputEvent : Resource
{
    public const int DEVICE_ID_EMULATION = -1;
	public const int DEVICE_ID_INTERNAL  = -2;

	public float GetActionRawStrength(string action, bool exactMatch = false) => throw new NotImplementedException();
	public float GetActionStrength(string action, bool exactMatch = false) => throw new NotImplementedException();
	public int GetDevice() => throw new NotImplementedException();
	public bool IsAction(string action, bool exactMatch = false) => throw new NotImplementedException();
	public bool IsActionPressed(string action, bool allowEcho = false, bool exactMatch = false) => throw new NotImplementedException();
	public bool IsActionReleased(string action, bool exactMatch = false) => throw new NotImplementedException();
	public void SetDevice(int device) => throw new NotImplementedException();

    public virtual bool Accumulate(InputEvent @event) => false;
	public virtual bool ActionMatch(InputEvent @event, bool exactMatch, float deadzone, out bool pressed, out float strength, out float rawStrength) => throw new NotImplementedException();
	public virtual bool IsActionType() => throw new NotImplementedException();
	public virtual bool IsEcho() => throw new NotImplementedException();
	public virtual bool IsMatch(InputEvent @event, bool exactMatch = true) => throw new NotImplementedException();
	public virtual bool IsPressed() => throw new NotImplementedException();
	public virtual InputEvent XformedBy(in Transform2D<RealT> xform, in Vector2<RealT> localOfs = default) => throw new NotImplementedException();

    public abstract string AsText();
}
