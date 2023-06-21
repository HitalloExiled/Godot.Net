namespace Godot.Net.Servers;
public abstract partial class TextServer
{
    [Flags]
    public enum TextOverrunFlag
    {
		OVERRUN_NO_TRIM             = 0,
		OVERRUN_TRIM                = 1 << 0,
		OVERRUN_TRIM_WORD_ONLY      = 1 << 1,
		OVERRUN_ADD_ELLIPSIS        = 1 << 2,
		OVERRUN_ENFORCE_ELLIPSIS    = 1 << 3,
		OVERRUN_JUSTIFICATION_AWARE = 1 << 4,
	}
}
