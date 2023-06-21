namespace Godot.Net.Drivers.GLES3.Api.Enums;

using Godot.Net.Drivers.GLES3.Api.Attributes;

#pragma warning disable CA1069

[Flags]
public enum MapBufferAccessMask : uint
{
    MapReadBit                = 0x0001,

    [GLExtension("GL_EXT_map_buffer_range")]
    MapReadBitEXT             = 0x0001,
    MapWriteBit               = 0x0002,

    [GLExtension("GL_EXT_map_buffer_range")]
    MapWriteBitEXT            = 0x0002,
    MapInvalidateRangeBit     = 0x0004,

    [GLExtension("GL_EXT_map_buffer_range")]
    MapInvalidateRangeBitEXT  = 0x0004,
    MapInvalidateBufferBit    = 0x0008,

    [GLExtension("GL_EXT_map_buffer_range")]
    MapInvalidateBufferBitEXT = 0x0008,
    MapFlushExplicitBit       = 0x0010,

    [GLExtension("GL_EXT_map_buffer_range")]
    MapFlushExplicitBitEXT    = 0x0010,
    MapUnsynchronizedBit      = 0x0020,

    [GLExtension("GL_EXT_map_buffer_range")]
    MapUnsynchronizedBitEXT   = 0x0020,
    MapPersistentBit          = 0x0040,

    [GLExtension("GL_EXT_buffer_storage")]
    MapPersistentBitEXT       = 0x0040,
    MapCoherentBit            = 0x0080,

    [GLExtension("GL_EXT_buffer_storage")]
    MapCoherentBitEXT         = 0x0080,
}
