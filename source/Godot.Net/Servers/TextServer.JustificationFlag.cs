namespace Godot.Net.Servers;

public partial class TextServer
{
    [Flags]
    public enum JustificationFlag
    {
		JUSTIFICATION_NONE               = 0,
		JUSTIFICATION_KASHIDA            = 1 << 0,
		JUSTIFICATION_WORD_BOUND         = 1 << 1,
		JUSTIFICATION_TRIM_EDGE_SPACES   = 1 << 2,
		JUSTIFICATION_AFTER_LAST_TAB     = 1 << 3,
		JUSTIFICATION_CONSTRAIN_ELLIPSIS = 1 << 4,
	}
}
