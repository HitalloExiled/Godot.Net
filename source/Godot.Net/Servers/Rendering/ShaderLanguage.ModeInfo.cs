namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public record ModeInfo
    {
        public string?      Name    { get; set; }
        public List<string> Options { get; } = new();

        public ModeInfo()
        { }

        public ModeInfo(string name, params string[] args)
        {
            this.Name = name;

            this.Options.AddRange(args);
        }
    };
}
