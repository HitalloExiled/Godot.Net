namespace Godot.Net.Platforms.Windows;

using Godot.Net.Platforms.Windows.Native;

public partial class GLManagerWindows
{
    public record GLWindow
    {
        public int  Width    { get; set; }
        public int  Height   { get; set; }
        public bool UseVsync { get; set; }

        // windows specific
        public HDC  HDC         { get; set; }
        public HWND Hwnd        { get; set; }
        public int  GldisplayId { get; set; }
    };
}
