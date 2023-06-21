namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    private record BuiltinFuncOutArgs(string? Name, int[] Arguments)
    {
        public const int MAX_ARGS = 2;
    }
}
