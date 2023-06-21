namespace Godot.Net.Drivers.GLES3.Api.Enums;

using Godot.Net.Drivers.GLES3.Api.Attributes;

#pragma warning disable CA1069

public enum EnableCap
{
    PointSmooth                = 0x0B10,
    LineSmooth                 = 0x0B20,
    LineStipple                = 0x0B24,
    PolygonSmooth              = 0x0B41,
    PolygonStipple             = 0x0B42,
    CullFace                   = 0x0B44,
    Lighting                   = 0x0B50,
    ColorMaterial              = 0x0B57,
    Fog                        = 0x0B60,
    DepthTest                  = 0x0B71,
    StencilTest                = 0x0B90,
    Normalize                  = 0x0BA1,
    AlphaTest                  = 0x0BC0,
    Dither                     = 0x0BD0,
    Blend                      = 0x0BE2,
    IndexLogicOp               = 0x0BF1,
    ColorLogicOp               = 0x0BF2,
    ScissorTest                = 0x0C11,
    TextureGenS                = 0x0C60,
    TextureGenT                = 0x0C61,
    TextureGenR                = 0x0C62,
    TextureGenQ                = 0x0C63,
    AutoNormal                 = 0x0D80,
    Map1Color4                 = 0x0D90,
    Map1Index                  = 0x0D91,
    Map1Normal                 = 0x0D92,
    Map1TextureCoord1          = 0x0D93,
    Map1TextureCoord2          = 0x0D94,
    Map1TextureCoord3          = 0x0D95,
    Map1TextureCoord4          = 0x0D96,
    Map1Vertex3                = 0x0D97,
    Map1Vertex4                = 0x0D98,
    Map2Color4                 = 0x0DB0,
    Map2Index                  = 0x0DB1,
    Map2Normal                 = 0x0DB2,
    Map2TextureCoord1          = 0x0DB3,
    Map2TextureCoord2          = 0x0DB4,
    Map2TextureCoord3          = 0x0DB5,
    Map2TextureCoord4          = 0x0DB6,
    Map2Vertex3                = 0x0DB7,
    Map2Vertex4                = 0x0DB8,
    Texture1D                  = 0x0DE0,
    Texture2D                  = 0x0DE1,
    PolygonOffsetPoint         = 0x2A01,
    PolygonOffsetLine          = 0x2A02,
    ClipPlane0                 = 0x3000,
    ClipDistance0              = 0x3000,
    ClipPlane1                 = 0x3001,
    ClipDistance1              = 0x3001,
    ClipPlane2                 = 0x3002,
    ClipDistance2              = 0x3002,
    ClipPlane3                 = 0x3003,
    ClipDistance3              = 0x3003,
    ClipPlane4                 = 0x3004,
    ClipDistance4              = 0x3004,
    ClipPlane5                 = 0x3005,
    ClipDistance5              = 0x3005,
    ClipDistance6              = 0x3006,
    ClipDistance7              = 0x3007,
    Light0                     = 0x4000,
    Light1                     = 0x4001,
    Light2                     = 0x4002,
    Light3                     = 0x4003,
    Light4                     = 0x4004,
    Light5                     = 0x4005,
    Light6                     = 0x4006,
    Light7                     = 0x4007,

    [GLExtension("GL_EXT_convolution")]
    Convolution1DEXT           = 0x8010,

    [GLExtension("GL_EXT_convolution")]
    Convolution2DEXT           = 0x8011,

    [GLExtension("GL_EXT_convolution")]
    Separable2DEXT             = 0x8012,

    [GLExtension("GL_EXT_histogram")]
    HistogramEXT               = 0x8024,

    [GLExtension("GL_EXT_histogram")]
    MinmaxEXT                  = 0x802E,
    PolygonOffsetFill          = 0x8037,

    [GLExtension("GL_EXT_rescale_normal")]
    RescaleNormalEXT           = 0x803A,

    [GLExtension("GL_EXT_texture3D")]
    Texture3DEXT               = 0x806F,
    VertexArray                = 0x8074,
    NormalArray                = 0x8075,
    ColorArray                 = 0x8076,
    IndexArray                 = 0x8077,
    TextureCoordArray          = 0x8078,
    EdgeFlagArray              = 0x8079,
    Multisample                = 0x809D,
    SampleAlphaToCoverage      = 0x809E,
    SampleAlphaToOne           = 0x809F,
    SampleCoverage             = 0x80A0,
    ColorTable                 = 0x80D0,
    PostConvolutionColorTable  = 0x80D1,
    PostColorMatrixColorTable  = 0x80D2,

    [GLExtension("GL_EXT_shared_texture_palette")]
    SharedTexturePaletteEXT    = 0x81FB,
    DebugOutputSynchronous     = 0x8242,
    TextureRectangle           = 0x84F5,

    [GLExtension("GL_ARB_texture_rectangle")]
    TextureRectangleARB        = 0x84F5,
    TextureCubeMap             = 0x8513,

    [GLExtension("GL_ARB_texture_cube_map")]
    TextureCubeMapARB          = 0x8513,

    [GLExtension("GL_EXT_texture_cube_map")]
    TextureCubeMapEXT          = 0x8513,
    ProgramPointSize           = 0x8642,
    DepthClamp                 = 0x864F,
    TextureCubeMapSeamless     = 0x884F,
    SampleShading              = 0x8C36,
    RasterizerDiscard          = 0x8C89,
    PrimitiveRestartFixedIndex = 0x8D69,
    FramebufferSrgb            = 0x8DB9,
    SampleMask                 = 0x8E51,
    PrimitiveRestart           = 0x8F9D,
    DebugOutput                = 0x92E0,
}
