namespace Godot.Net.Platforms.Windows.Native;

internal static partial class User32
{
    public enum WINDOW_LONG_INDEX
    {
        /// <summary>
        /// Sets a new extended window style.
        /// </summary>
        GWL_EXSTYLE = -20,

        /// <summary>
        /// Sets a new application instance handle.
        /// </summary>
        GWLP_HINSTANCE = -6,

        /// <summary>
        /// Retrieves a handle to the parent window, if there is one.
        /// </summary>
        GWLP_HWNDPARENT = -8,

        /// <summary>
        /// Sets a new identifier of the child window. The window cannot be a top-level window.
        /// </summary>
        GWLP_ID = -12,

        /// <summary>
        /// Sets a new window style.
        /// </summary>
        GWL_STYLE = -16,

        /// <summary>
        /// Sets the user data associated with the window. This data is intended for use by the application that created the window. Its value is initially zero.
        /// </summary>
        GWLP_USERDATA = -21,

        /// <summary>
        /// Sets a new address for the window procedure.
        /// </summary>
        GWLP_WNDPROC = -4
    }
}
