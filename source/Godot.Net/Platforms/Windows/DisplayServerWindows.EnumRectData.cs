namespace Godot.Net.Platforms.Windows;

using Godot.Net.Core.Math;

public partial class DisplayServerWindows
{
    private record struct EnumRectData(int Count = default, int Screen = default, Rect2<int> Rect = default);
}
