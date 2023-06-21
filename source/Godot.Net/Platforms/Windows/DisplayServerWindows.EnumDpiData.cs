namespace Godot.Net.Platforms.Windows;

public partial class DisplayServerWindows
{
    private record struct EnumDpiData(int Count, int Screen, int Dpi);
}
