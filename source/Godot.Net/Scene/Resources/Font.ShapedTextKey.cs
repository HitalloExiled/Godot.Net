namespace Godot.Net.Scene.Resources;
using Godot.Net.Servers;

#pragma warning disable IDE0044, CS0649 // TODO Remove

public partial class Font
{
    private record struct ShapedTextKey
    {
		public TextServer.LineBreakFlag     BrkFlags    { get; set; } = TextServer.LineBreakFlag.BREAK_MANDATORY;
		public TextServer.Direction         Direction   { get; set; } = TextServer.Direction.DIRECTION_AUTO;
		public int                          FontSize    { get; set; } = 14;
		public TextServer.JustificationFlag JstFlags    { get; set; } = TextServer.JustificationFlag.JUSTIFICATION_NONE;
		public TextServer.Orientation       Orientation { get; set; } = TextServer.Orientation.ORIENTATION_HORIZONTAL;
		public string?                      Text        { get; set; }
		public float                        Width       { get; set; }

		public ShapedTextKey() { }
		public ShapedTextKey(
            string                       text,
            int                          fontSize,
            float                        width,
            TextServer.JustificationFlag jstFlags,
            TextServer.LineBreakFlag     brkFlags,
            TextServer.Direction         direction,
            TextServer.Orientation       orientation
        )
        {
            this.Text        = text;
            this.FontSize    = fontSize;
            this.Width       = width;
            this.JstFlags    = jstFlags;
            this.BrkFlags    = brkFlags;
            this.Direction   = direction;
            this.Orientation = orientation;
		}
	}
}
