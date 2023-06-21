namespace Godot.Net.Servers;
public abstract partial class RenderingServer
{
    public record ShaderNativeSourceCode
    {
        public record Version
        {
            public record Stage
            {
                public string? Name { get; set; }
                public string? Code { get; set; }
            }

            public List<Stage> Stages { get; set; } = new();
        }

        public List<Version> Versions { get; set; } = new();
    }
}
