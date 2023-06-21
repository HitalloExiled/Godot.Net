namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public record FilePosition
    {
        public string? File { get; set; }
        public int     Line { get; set; }
    }
}
