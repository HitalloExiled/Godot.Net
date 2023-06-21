namespace Godot.Net.Platforms.Windows;

using Godot.Net.Platforms.Windows.Native;

public partial class OSWindows
{
    private record ProcessInfo(Kernel32.STARTUPINFOW Si, Kernel32.PROCESS_INFORMATION Pi);
}
