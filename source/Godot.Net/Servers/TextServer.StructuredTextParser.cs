namespace Godot.Net.Servers;

public partial class TextServer
{
    public enum StructuredTextParser
    {
		STRUCTURED_TEXT_DEFAULT,
		STRUCTURED_TEXT_URI,
		STRUCTURED_TEXT_FILE,
		STRUCTURED_TEXT_EMAIL,
		STRUCTURED_TEXT_LIST,
		STRUCTURED_TEXT_GDSCRIPT,
		STRUCTURED_TEXT_CUSTOM
	}
}
