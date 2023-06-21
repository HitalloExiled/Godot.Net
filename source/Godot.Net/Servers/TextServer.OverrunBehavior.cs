namespace Godot.Net.Servers;
public abstract partial class TextServer
{
    public enum OverrunBehavior
    {
		OVERRUN_NO_TRIMMING,
		OVERRUN_TRIM_CHAR,
		OVERRUN_TRIM_WORD,
		OVERRUN_TRIM_ELLIPSIS,
		OVERRUN_TRIM_WORD_ELLIPSIS,
	}
}
