namespace Godot.Net.Servers;
public abstract partial class TextServer
{
    [Flags]
    public enum GraphemeFlag : ushort
    {
		GRAPHEME_IS_VALID                  = 1 << 0, // Glyph is valid.
		GRAPHEME_IS_RTL                    = 1 << 1, // Glyph is right-to-left.
		GRAPHEME_IS_VIRTUAL                = 1 << 2, // Glyph is not part of source string (added by fit_to_width function, do not affect caret movement).
		GRAPHEME_IS_SPACE                  = 1 << 3, // Is whitespace (for justification and word breaks).
		GRAPHEME_IS_BREAK_HARD             = 1 << 4, // Is line break (mandatory break, e.g. "\n").
		GRAPHEME_IS_BREAK_SOFT             = 1 << 5, // Is line break (optional break, e.g. space).
		GRAPHEME_IS_TAB                    = 1 << 6, // Is tab or vertical tab.
		GRAPHEME_IS_ELONGATION             = 1 << 7, // Elongation (e.g. kashida), glyph can be duplicated or truncated to fit line to width.
		GRAPHEME_IS_PUNCTUATION            = 1 << 8, // Punctuation, except underscore (can be used as word break, but not line break or justifiction).
		GRAPHEME_IS_UNDERSCORE             = 1 << 9, // Underscore (can be used as word break).
		GRAPHEME_IS_CONNECTED              = 1 << 10, // Connected to previous grapheme.
		GRAPHEME_IS_SAFE_TO_INSERT_TATWEEL = 1 << 11, // It is safe to insert a U+0640 before this grapheme for elongation.
	}
}
