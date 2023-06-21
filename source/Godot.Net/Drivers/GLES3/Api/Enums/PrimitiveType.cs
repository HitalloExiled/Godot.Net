namespace Godot.Net.Drivers.GLES3.Api.Enums;

using Godot.Net.Drivers.GLES3.Api.Attributes;

#pragma warning disable CA1069

public enum PrimitiveType
{
    Points                    = 0x0000,
    Lines                     = 0x0001,
    LineLoop                  = 0x0002,
    LineStrip                 = 0x0003,
    Triangles                 = 0x0004,
    TriangleStrip             = 0x0005,
    TriangleFan               = 0x0006,
    Quads                     = 0x0007,

    [GLExtension("GL_EXT_tessellation_shader")]
    QuadsEXT                  = 0x0007,
    QuadStrip                 = 0x0008,
    Polygon                   = 0x0009,
    LinesAdjacency            = 0x000A,

    [GLExtension("GL_ARB_geometry_shader4")]
    LinesAdjacencyARB         = 0x000A,

    [GLExtension("GL_EXT_geometry_shader")]
    LinesAdjacencyEXT         = 0x000A,
    LineStripAdjacency        = 0x000B,

    [GLExtension("GL_ARB_geometry_shader4")]
    LineStripAdjacencyARB     = 0x000B,

    [GLExtension("GL_EXT_geometry_shader")]
    LineStripAdjacencyEXT     = 0x000B,
    TrianglesAdjacency        = 0x000C,

    [GLExtension("GL_ARB_geometry_shader4")]
    TrianglesAdjacencyARB     = 0x000C,

    [GLExtension("GL_EXT_geometry_shader")]
    TrianglesAdjacencyEXT     = 0x000C,
    TriangleStripAdjacency    = 0x000D,

    [GLExtension("GL_ARB_geometry_shader4")]
    TriangleStripAdjacencyARB = 0x000D,

    [GLExtension("GL_EXT_geometry_shader")]
    TriangleStripAdjacencyEXT = 0x000D,
    Patches                   = 0x000E,

    [GLExtension("GL_EXT_tessellation_shader")]
    PatchesEXT                = 0x000E,
}
