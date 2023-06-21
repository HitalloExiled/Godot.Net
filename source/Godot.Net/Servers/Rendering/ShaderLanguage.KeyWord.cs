namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    private record KeyWord(TokenType Token, string? Text, ContextFlag Flags, string[]? ExcludedShaderTypes = default, string[]? Functions = default)
    {
        public string[] ExcludedShaderTypes { get; } = ExcludedShaderTypes ?? Array.Empty<string>();
        public string[] Functions           { get; } = Functions ?? Array.Empty<string>();
    }
}
