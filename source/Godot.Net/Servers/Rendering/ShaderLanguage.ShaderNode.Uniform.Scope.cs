namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public partial record ShaderNode
    {
        public partial record Uniform
        {
            public enum ScopeKind
            {
                SCOPE_LOCAL,
                SCOPE_INSTANCE,
                SCOPE_GLOBAL,
            }
        }
    }
}
