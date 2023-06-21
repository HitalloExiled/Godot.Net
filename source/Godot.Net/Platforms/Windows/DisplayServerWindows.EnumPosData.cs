namespace Godot.Net.Platforms.Windows;

using Godot.Net.Core.Math;

public partial class DisplayServerWindows
{
    private record struct EnumPosData(int Count = default, int Screen = default, Vector2<int> Position = default);
}
