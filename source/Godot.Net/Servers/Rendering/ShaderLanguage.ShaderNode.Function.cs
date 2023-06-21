namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public partial record ShaderNode
    {
        public record Function
        {
            public required bool         Callable     { get; set; }
            public required FunctionNode FunctionNode { get; set; }
            public required string       Name         { get; set; }
            public required int          Index        { get; set; }

            public HashSet<string> UsesFunction { get; set; } = new();
        }
    }
}
