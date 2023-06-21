namespace Godot.Net.Scene.GUI;

using Godot.Net.Core.Input;
using Godot.Net.Scene.Main;

public class Popup : Window
{
    public Popup()
    {
        this.WrapControls = true;
        this.Visible      = false;
        this.Transient    = true;
        this.SetFlag(Flags.FLAG_BORDERLESS, true);
        this.SetFlag(Flags.FLAG_RESIZE_DISABLED, true);
        this.SetFlag(Flags.FLAG_POPUP, true);

        // connect("window_input", callable_mp(this, &Popup::_input_from_window));
        WindowInput += this.InputFromWindow;
    }

    private void InputFromWindow(InputEvent @event)
    {
        // Ref<InputEventKey> key = p_event;
        // if (get_flag(FLAG_POPUP) && key.is_valid() && key->is_pressed() && key->get_keycode() == Key::ESCAPE) {
        //     _close_pressed();
        // }
    }
}
