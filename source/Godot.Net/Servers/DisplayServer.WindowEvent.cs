namespace Godot.Net.Servers;

public abstract partial class DisplayServer
{
    public enum WindowEvent
    {
        WINDOW_EVENT_MOUSE_ENTER,
        WINDOW_EVENT_MOUSE_EXIT,
        WINDOW_EVENT_FOCUS_IN,
        WINDOW_EVENT_FOCUS_OUT,
        WINDOW_EVENT_CLOSE_REQUEST,
        WINDOW_EVENT_GO_BACK_REQUEST,
        WINDOW_EVENT_DPI_CHANGE,
        WINDOW_EVENT_TITLEBAR_CHANGE,
    };
}
