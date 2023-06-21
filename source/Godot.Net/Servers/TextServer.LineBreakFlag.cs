namespace Godot.Net.Servers;

public partial class TextServer
{
    [Flags]
    public enum LineBreakFlag
    {
		BREAK_NONE             = 0,
		BREAK_MANDATORY        = 1 << 0,
		BREAK_WORD_BOUND       = 1 << 1,
		BREAK_GRAPHEME_BOUND   = 1 << 2,
		BREAK_ADAPTIVE         = 1 << 3,
		BREAK_TRIM_EDGE_SPACES = 1 << 4,
	}
}
