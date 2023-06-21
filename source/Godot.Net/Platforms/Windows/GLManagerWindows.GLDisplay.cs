namespace Godot.Net.Platforms.Windows;

using Godot.Net.Platforms.Windows.Native;

public partial class GLManagerWindows
{
    public record GLDisplay
    {
        public HGLRC HRc { get; set; }
    }
}
