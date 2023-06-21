namespace Godot.Net.Servers;
public abstract partial class TextServer
{
    public enum VisibleCharactersBehavior
    {
		VC_CHARS_BEFORE_SHAPING,
		VC_CHARS_AFTER_SHAPING,
		VC_GLYPHS_AUTO,
		VC_GLYPHS_LTR,
		VC_GLYPHS_RTL,
	}
}
