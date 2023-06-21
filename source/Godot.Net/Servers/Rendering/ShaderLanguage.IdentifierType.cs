namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    private enum IdentifierType
    {
        IDENTIFIER_FUNCTION,
        IDENTIFIER_UNIFORM,
        IDENTIFIER_VARYING,
        IDENTIFIER_FUNCTION_ARGUMENT,
        IDENTIFIER_LOCAL_VAR,
        IDENTIFIER_BUILTIN_VAR,
        IDENTIFIER_CONSTANT,
        IDENTIFIER_MAX,
    }
}
