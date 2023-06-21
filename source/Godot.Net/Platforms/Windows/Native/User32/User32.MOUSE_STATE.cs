namespace Godot.Net.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// Enumeration of mouse move type.
    /// </summary>
    public enum MOUSE_STATE : ushort
    {
        /// <summary>
        /// Mouse movement data is relative to the last mouse position.
        /// </summary>
        MOUSE_MOVE_RELATIVE = 0x00,

        /// <summary>
        /// Mouse movement data is based on absolute position.
        /// </summary>
        MOUSE_MOVE_ABSOLUTE = 0x01,

        /// <summary>
        /// Mouse coordinates are mapped to the virtual desktop (for a multiple monitor system).
        /// </summary>
        MOUSE_VIRTUAL_DESKTOP = 0x02,

        /// <summary>
        /// Mouse attributes changed; application needs to query the mouse attributes.
        /// </summary>
        MOUSE_ATTRIBUTES_CHANGED = 0x04,

        /// <summary>
        /// This mouse movement event was not coalesced. Mouse movement events can be coalesced by default.
        /// Note: This value is not supported on Windows XP/2000.
        /// </summary>
        MOUSE_MOVE_NOCOALESCE = 0x08
    }
}
