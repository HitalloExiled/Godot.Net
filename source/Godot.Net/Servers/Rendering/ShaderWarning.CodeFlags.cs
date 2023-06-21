namespace Godot.Net.Servers.Rendering;

public partial class ShaderWarning
{
    [Flags]
    public enum CodeFlags : uint
    {
        NONE_FLAG                  = 0U,
        FLOAT_COMPARISON_FLAG      = 1U,
        UNUSED_CONSTANT_FLAG       = 2U,
        UNUSED_FUNCTION_FLAG       = 4U,
        UNUSED_STRUCT_FLAG         = 8U,
        UNUSED_UNIFORM_FLAG        = 16U,
        UNUSED_VARYING_FLAG        = 32U,
        UNUSED_LOCAL_VARIABLE_FLAG = 64U,
        FORMATTING_ERROR_FLAG      = 128U,
        DEVICE_LIMIT_EXCEEDED_FLAG = 256U,
    };
}
