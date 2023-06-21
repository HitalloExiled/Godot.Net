namespace Godot.Net.Editor;

using Godot.Net.Core.Math;

public class EditorColorMap
{
    public static HashSet<string>          ColorConversionExceptions { get; } = new();

    public static Dictionary<Color, Color> ColorConversionMap        { get; } = new();
}
