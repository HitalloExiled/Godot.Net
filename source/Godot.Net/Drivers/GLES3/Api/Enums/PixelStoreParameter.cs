namespace Godot.Net.Drivers.GLES3.Api.Enums;

using Godot.Net.Drivers.GLES3.Api.Attributes;

#pragma warning disable CA1069

public enum PixelStoreParameter
{
    UnpackSwapBytes      = 0x0CF0,
    UnpackLsbFirst       = 0x0CF1,
    UnpackRowLength      = 0x0CF2,

    [GLExtension("GL_EXT_unpack_subimage")]
    UnpackRowLengthEXT   = 0x0CF2,
    UnpackSkipRows       = 0x0CF3,

    [GLExtension("GL_EXT_unpack_subimage")]
    UnpackSkipRowsEXT    = 0x0CF3,
    UnpackSkipPixels     = 0x0CF4,

    [GLExtension("GL_EXT_unpack_subimage")]
    UnpackSkipPixelsEXT  = 0x0CF4,
    UnpackAlignment      = 0x0CF5,
    PackSwapBytes        = 0x0D00,
    PackLsbFirst         = 0x0D01,
    PackRowLength        = 0x0D02,
    PackSkipRows         = 0x0D03,
    PackSkipPixels       = 0x0D04,
    PackAlignment        = 0x0D05,
    PackSkipImages       = 0x806B,

    [GLExtension("GL_EXT_texture3D")]
    PackSkipImagesEXT    = 0x806B,
    PackImageHeight      = 0x806C,

    [GLExtension("GL_EXT_texture3D")]
    PackImageHeightEXT   = 0x806C,
    UnpackSkipImages     = 0x806D,

    [GLExtension("GL_EXT_texture3D")]
    UnpackSkipImagesEXT  = 0x806D,
    UnpackImageHeight    = 0x806E,

    [GLExtension("GL_EXT_texture3D")]
    UnpackImageHeightEXT = 0x806E,
}
