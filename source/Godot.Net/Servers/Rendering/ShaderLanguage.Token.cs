namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public record struct Token(TokenType Type, string Text, int Line)
    {
        public float         Constant          { get; set; }
        public readonly bool IsIntegerConstant => this.Type is TokenType.TK_INT_CONSTANT or TokenType.TK_UINT_CONSTANT;
    }
}
