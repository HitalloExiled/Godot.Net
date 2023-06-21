namespace Godot.Net.Drivers.GLES3.Api.Enums;

using Godot.Net.Drivers.GLES3.Api.Attributes;

#pragma warning disable CA1069

public enum BlendEquationModeEXT
{
    FuncAdd                = 0x8006,

    [GLExtension("GL_EXT_blend_minmax")]
    FuncAddEXT             = 0x8006,
    Min                    = 0x8007,

    [GLExtension("GL_EXT_blend_minmax")]
    MinEXT                 = 0x8007,
    Max                    = 0x8008,

    [GLExtension("GL_EXT_blend_minmax")]
    MaxEXT                 = 0x8008,
    FuncSubtract           = 0x800A,

    [GLExtension("GL_EXT_blend_subtract")]
    FuncSubtractEXT        = 0x800A,
    FuncReverseSubtract    = 0x800B,

    [GLExtension("GL_EXT_blend_subtract")]
    FuncReverseSubtractEXT = 0x800B,
}
