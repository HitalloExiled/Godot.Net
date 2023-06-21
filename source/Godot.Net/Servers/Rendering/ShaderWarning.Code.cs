namespace Godot.Net.Servers.Rendering;

public partial class ShaderWarning
{
    public enum Code
    {
        FLOAT_COMPARISON,
        UNUSED_CONSTANT,
        UNUSED_FUNCTION,
        UNUSED_STRUCT,
        UNUSED_UNIFORM,
        UNUSED_VARYING,
        UNUSED_LOCAL_VARIABLE,
        FORMATTING_ERROR,
        DEVICE_LIMIT_EXCEEDED,
        WARNING_MAX,
    }
}
