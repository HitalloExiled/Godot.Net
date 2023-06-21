namespace Godot.Net.Core.Input;

#pragma warning disable IDE0044, CS0649 // TODO Remove;

public class InputEventWithModifiers : InputEventFromWindow
{
    private bool altPressed;
    private bool commandOrControlAutoremap;
    private bool ctrlPressed;
    private bool shiftPressed;

    public bool AltPressed   { get => this.altPressed;   set => this.SetAltPressed(value); }
    public bool CtrlPressed  { get => this.ctrlPressed;  set => this.SetCtrlPressed(value); }
    public bool ShiftPressed { get => this.shiftPressed; set => this.SetShiftPressed(value); }

    private void SetAltPressed(bool value)
    {
        if (ERR_FAIL_COND_MSG(this.commandOrControlAutoremap, "Command/Control autoremaping is enabled, cannot set Control directly!"))
        {
            return;
        }

        this.altPressed = value;
        this.EmitChanged();
    }

    private void SetCtrlPressed(bool value)
    {
        if (ERR_FAIL_COND_MSG(this.commandOrControlAutoremap, "Command/Control autoremaping is enabled, cannot set Control directly!"))
        {
            return;
        }

        this.ctrlPressed = value;
        this.EmitChanged();
    }

    private void SetShiftPressed(bool value)
    {
        if (ERR_FAIL_COND_MSG(this.commandOrControlAutoremap, "Command/Control autoremaping is enabled, cannot set Control directly!"))
        {
            return;
        }

        this.shiftPressed = value;
        this.EmitChanged();
    }
}
