namespace Godot.Net.Platforms.Windows.Native;
internal static partial class User32
{
    public enum FILL_MODE
    {
        /// <summary>
        /// Selects alternate mode (fills area between odd-numbered and even-numbered polygon sides on each scan line).
        /// </summary>
        ALTERNATE,

        /// <summary>
        /// Selects winding mode (fills any region with a nonzero winding value).
        /// </summary>
        WINDING,
    }
}
