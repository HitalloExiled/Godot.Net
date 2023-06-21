namespace Godot.Net.Core.Input;

public class InputEventFromWindow : InputEvent
{
    private long windowId;

    public long WindowId
    {
        get => this.windowId;
        set
        {
            this.windowId = value;
            this.EmitChanged();
        }
    }

    public override string AsText() => throw new NotImplementedException();
}
