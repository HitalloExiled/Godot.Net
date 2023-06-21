namespace Godot.Net.Servers;

public abstract partial class TextServer
{
    [Flags]
    public enum Feature
    {
		FEATURE_SIMPLE_LAYOUT                     = 1 << 0,
		FEATURE_BIDI_LAYOUT                       = 1 << 1,
		FEATURE_VERTICAL_LAYOUT                   = 1 << 2,
		FEATURE_SHAPING                           = 1 << 3,
		FEATURE_KASHIDA_JUSTIFICATION             = 1 << 4,
		FEATURE_BREAK_ITERATORS                   = 1 << 5,
		FEATURE_FONT_BITMAP                       = 1 << 6,
		FEATURE_FONT_DYNAMIC                      = 1 << 7,
		FEATURE_FONT_MSDF                         = 1 << 8,
		FEATURE_FONT_SYSTEM                       = 1 << 9,
		FEATURE_FONT_VARIABLE                     = 1 << 10,
		FEATURE_CONTEXT_SENSITIVE_CASE_CONVERSION = 1 << 11,
		FEATURE_USE_SUPPORT_DATA                  = 1 << 12,
		FEATURE_UNICODE_IDENTIFIERS               = 1 << 13,
		FEATURE_UNICODE_SECURITY                  = 1 << 14,
	}
}
