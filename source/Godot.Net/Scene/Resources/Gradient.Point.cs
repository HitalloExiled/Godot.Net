namespace Godot.Net.Scene.Resources;

using Godot.Net.Core.Math;

public partial class Gradient
{
    public record Point
    {
		public Color Color  { get; set; }
		public float Offset { get; set; }
	}
}
