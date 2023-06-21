namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    [Flags]
    private enum ContextFlag : uint
    {
        CF_UNSPECIFIED             = 0U,
        CF_BLOCK                   = 1U, // "void test() { <x> }"
        CF_FUNC_DECL_PARAM_SPEC    = 2U, // "void test(<x> int param) {}"
        CF_FUNC_DECL_PARAM_TYPE    = 4U, // "void test(<x> param) {}"
        CF_IF_DECL                 = 8U, // "if(<x>) {}"
        CF_BOOLEAN                 = 16U, // "bool t = <x>;"
        CF_GLOBAL_SPACE            = 32U, // "struct", "const", "void" etc.
        CF_DATATYPE                = 64U, // "<x> value;"
        CF_UNIFORM_TYPE            = 128U, // "uniform <x> myUniform;"
        CF_VARYING_TYPE            = 256U, // "varying <x> myVarying;"
        CF_PRECISION_MODIFIER      = 512U, // "<x> vec4 a = vec4(0.0, 1.0, 2.0, 3.0);"
        CF_INTERPOLATION_QUALIFIER = 1024U, // "varying <x> vec3 myColor;"
        CF_UNIFORM_KEYWORD         = 2048U, // "uniform"
        CF_CONST_KEYWORD           = 4096U, // "const"
        CF_UNIFORM_QUALIFIER       = 8192U, // "<x> uniform float t;"
        CF_SHADER_TYPE             = 16384U, // "shader_type"
    }
}
