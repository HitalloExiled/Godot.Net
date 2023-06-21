namespace Godot.Net.Platforms.Windows;

using Godot.Net.Platforms.Windows.Native;

public partial class DisplayServerWindows
{
    private record struct EnumScreenData(int Count = default, int Screen = default, HMONITOR Monitor = default);
}
