namespace Godot.Net.Drivers.GLES3.Api.Enums;

using Godot.Net.Drivers.GLES3.Api.Attributes;

#pragma warning disable CA1069 // TODO Remove

public enum VertexAttribPointerType
{
    Byte                     = 0x1400,
    UnsignedByte             = 0x1401,
    Short                    = 0x1402,
    UnsignedShort            = 0x1403,
    Int                      = 0x1404,
    UnsignedInt              = 0x1405,
    Float                    = 0x1406,
    Double                   = 0x140A,
    HalfFloat                = 0x140B,
    Fixed                    = 0x140C,
    UnsignedInt2101010Rev    = 0x8368,

    [GLExtension("GL_EXT_texture_type_2_10_10_10_REV")]
    UnsignedInt2101010RevEXT = 0x8368,
    UnsignedInt10f11f11fRev  = 0x8C3B,
    Int2101010Rev            = 0x8D9F,
}
