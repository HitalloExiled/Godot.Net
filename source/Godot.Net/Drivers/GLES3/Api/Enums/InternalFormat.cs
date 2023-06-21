namespace Godot.Net.Drivers.GLES3.Api.Enums;

using Godot.Net.Drivers.GLES3.Api.Attributes;

#pragma warning disable CA1069

public enum InternalFormat
{
    StencilIndex                             = 0x1901,
    DepthComponent                           = 0x1902,
    Red                                      = 0x1903,

    [GLExtension("GL_EXT_texture_rg")]
    RedEXT                                   = 0x1903,
    Rgb                                      = 0x1907,
    Rgba                                     = 0x1908,
    R3G3B2                                   = 0x2A10,
    Alpha4                                   = 0x803B,

    [GLExtension("GL_EXT_texture")]
    Alpha4EXT                                = 0x803B,
    Alpha8                                   = 0x803C,

    [GLExtension("GL_EXT_texture")]
    Alpha8EXT                                = 0x803C,
    Alpha12                                  = 0x803D,

    [GLExtension("GL_EXT_texture")]
    Alpha12EXT                               = 0x803D,
    Alpha16                                  = 0x803E,

    [GLExtension("GL_EXT_texture")]
    Alpha16EXT                               = 0x803E,
    Luminance4                               = 0x803F,

    [GLExtension("GL_EXT_texture")]
    Luminance4EXT                            = 0x803F,
    Luminance8                               = 0x8040,

    [GLExtension("GL_EXT_texture")]
    Luminance8EXT                            = 0x8040,
    Luminance12                              = 0x8041,

    [GLExtension("GL_EXT_texture")]
    Luminance12EXT                           = 0x8041,
    Luminance16                              = 0x8042,

    [GLExtension("GL_EXT_texture")]
    Luminance16EXT                           = 0x8042,
    Luminance4Alpha4                         = 0x8043,

    [GLExtension("GL_EXT_texture")]
    Luminance4Alpha4EXT                      = 0x8043,
    Luminance6Alpha2                         = 0x8044,

    [GLExtension("GL_EXT_texture")]
    Luminance6Alpha2EXT                      = 0x8044,
    Luminance8Alpha8                         = 0x8045,

    [GLExtension("GL_EXT_texture")]
    Luminance8Alpha8EXT                      = 0x8045,
    Luminance12Alpha4                        = 0x8046,

    [GLExtension("GL_EXT_texture")]
    Luminance12Alpha4EXT                     = 0x8046,
    Luminance12Alpha12                       = 0x8047,

    [GLExtension("GL_EXT_texture")]
    Luminance12Alpha12EXT                    = 0x8047,
    Luminance16Alpha16                       = 0x8048,

    [GLExtension("GL_EXT_texture")]
    Luminance16Alpha16EXT                    = 0x8048,
    Intensity                                = 0x8049,
    Intensity4                               = 0x804A,

    [GLExtension("GL_EXT_texture")]
    Intensity4EXT                            = 0x804A,
    Intensity8                               = 0x804B,

    [GLExtension("GL_EXT_texture")]
    Intensity8EXT                            = 0x804B,
    Intensity12                              = 0x804C,

    [GLExtension("GL_EXT_texture")]
    Intensity12EXT                           = 0x804C,
    Intensity16                              = 0x804D,

    [GLExtension("GL_EXT_texture")]
    Intensity16EXT                           = 0x804D,

    [GLExtension("GL_EXT_texture")]
    Rgb2EXT                                  = 0x804E,
    Rgb4                                     = 0x804F,

    [GLExtension("GL_EXT_texture")]
    Rgb4EXT                                  = 0x804F,
    Rgb5                                     = 0x8050,

    [GLExtension("GL_EXT_texture")]
    Rgb5EXT                                  = 0x8050,
    Rgb8                                     = 0x8051,

    [GLExtension("GL_EXT_texture")]
    Rgb8EXT                                  = 0x8051,
    Rgb10                                    = 0x8052,

    [GLExtension("GL_EXT_texture")]
    Rgb10EXT                                 = 0x8052,
    Rgb12                                    = 0x8053,

    [GLExtension("GL_EXT_texture")]
    Rgb12EXT                                 = 0x8053,
    Rgb16                                    = 0x8054,

    [GLExtension("GL_EXT_texture")]
    Rgb16EXT                                 = 0x8054,
    Rgba2                                    = 0x8055,

    [GLExtension("GL_EXT_texture")]
    Rgba2EXT                                 = 0x8055,
    Rgba4                                    = 0x8056,

    [GLExtension("GL_EXT_texture")]
    Rgba4EXT                                 = 0x8056,
    Rgb5A1                                   = 0x8057,

    [GLExtension("GL_EXT_texture")]
    Rgb5A1EXT                                = 0x8057,
    Rgba8                                    = 0x8058,

    [GLExtension("GL_EXT_texture")]
    Rgba8EXT                                 = 0x8058,
    Rgb10A2                                  = 0x8059,

    [GLExtension("GL_EXT_texture")]
    Rgb10A2EXT                               = 0x8059,
    Rgba12                                   = 0x805A,

    [GLExtension("GL_EXT_texture")]
    Rgba12EXT                                = 0x805A,
    Rgba16                                   = 0x805B,

    [GLExtension("GL_EXT_texture")]
    Rgba16EXT                                = 0x805B,
    DepthComponent16                         = 0x81A5,

    [GLExtension("GL_ARB_depth_texture")]
    DepthComponent16ARB                      = 0x81A5,
    DepthComponent24                         = 0x81A6,

    [GLExtension("GL_ARB_depth_texture")]
    DepthComponent24ARB                      = 0x81A6,
    DepthComponent32                         = 0x81A7,

    [GLExtension("GL_ARB_depth_texture")]
    DepthComponent32ARB                      = 0x81A7,
    CompressedRed                            = 0x8225,
    CompressedRg                             = 0x8226,
    Rg                                       = 0x8227,
    R8                                       = 0x8229,

    [GLExtension("GL_EXT_texture_rg")]
    R8EXT                                    = 0x8229,
    R16                                      = 0x822A,

    [GLExtension("GL_EXT_texture_norm16")]
    R16EXT                                   = 0x822A,
    Rg8                                      = 0x822B,

    [GLExtension("GL_EXT_texture_rg")]
    Rg8EXT                                   = 0x822B,
    Rg16                                     = 0x822C,

    [GLExtension("GL_EXT_texture_norm16")]
    Rg16EXT                                  = 0x822C,
    R16f                                     = 0x822D,

    [GLExtension("GL_EXT_color_buffer_half_float")]
    R16fEXT                                  = 0x822D,
    R32f                                     = 0x822E,

    [GLExtension("GL_EXT_texture_storage")]
    R32fEXT                                  = 0x822E,
    Rg16f                                    = 0x822F,

    [GLExtension("GL_EXT_color_buffer_half_float")]
    Rg16fEXT                                 = 0x822F,
    Rg32f                                    = 0x8230,

    [GLExtension("GL_EXT_texture_storage")]
    Rg32fEXT                                 = 0x8230,
    R8i                                      = 0x8231,
    R8ui                                     = 0x8232,
    R16i                                     = 0x8233,
    R16ui                                    = 0x8234,
    R32i                                     = 0x8235,
    R32ui                                    = 0x8236,
    Rg8i                                     = 0x8237,
    Rg8ui                                    = 0x8238,
    Rg16i                                    = 0x8239,
    Rg16ui                                   = 0x823A,
    Rg32i                                    = 0x823B,
    Rg32ui                                   = 0x823C,

    [GLExtension("GL_EXT_texture_compression_dxt1")]
    CompressedRgbS3tcDxt1EXT                 = 0x83F0,

    [GLExtension("GL_EXT_texture_compression_dxt1")]
    CompressedRgbaS3tcDxt1EXT                = 0x83F1,

    [GLExtension("GL_EXT_texture_compression_s3tc")]
    CompressedRgbaS3tcDxt3EXT                = 0x83F2,

    [GLExtension("GL_EXT_texture_compression_s3tc")]
    CompressedRgbaS3tcDxt5EXT                = 0x83F3,
    CompressedRgb                            = 0x84ED,
    CompressedRgba                           = 0x84EE,
    DepthStencil                             = 0x84F9,

    [GLExtension("GL_EXT_packed_depth_stencil")]
    DepthStencilEXT                          = 0x84F9,
    DepthStencilMESA                         = 0x8750,
    Rgba32f                                  = 0x8814,

    [GLExtension("GL_ARB_texture_float")]
    Rgba32fARB                               = 0x8814,

    [GLExtension("GL_EXT_texture_storage")]
    Rgba32fEXT                               = 0x8814,
    Rgb32f                                   = 0x8815,

    [GLExtension("GL_ARB_texture_float")]
    Rgb32fARB                                = 0x8815,

    [GLExtension("GL_EXT_texture_storage")]
    Rgb32fEXT                                = 0x8815,
    Rgba16f                                  = 0x881A,

    [GLExtension("GL_ARB_texture_float")]
    Rgba16fARB                               = 0x881A,

    [GLExtension("GL_EXT_color_buffer_half_float")]
    Rgba16fEXT                               = 0x881A,
    Rgb16f                                   = 0x881B,

    [GLExtension("GL_ARB_texture_float")]
    Rgb16fARB                                = 0x881B,

    [GLExtension("GL_EXT_color_buffer_half_float")]
    Rgb16fEXT                                = 0x881B,
    Depth24Stencil8                          = 0x88F0,

    [GLExtension("GL_EXT_packed_depth_stencil")]
    Depth24Stencil8EXT                       = 0x88F0,
    R11fG11fB10f                             = 0x8C3A,

    [GLExtension("GL_EXT_packed_float")]
    R11fG11fB10fEXT                          = 0x8C3A,
    Rgb9E5                                   = 0x8C3D,

    [GLExtension("GL_EXT_texture_shared_exponent")]
    Rgb9E5EXT                                = 0x8C3D,
    Srgb                                     = 0x8C40,

    [GLExtension("GL_EXT_sRGB")]
    SrgbEXT                                  = 0x8C40,
    Srgb8                                    = 0x8C41,

    [GLExtension("GL_EXT_texture_sRGB")]
    Srgb8EXT                                 = 0x8C41,
    SrgbAlpha                                = 0x8C42,

    [GLExtension("GL_EXT_sRGB")]
    SrgbAlphaEXT                             = 0x8C42,
    Srgb8Alpha8                              = 0x8C43,

    [GLExtension("GL_EXT_sRGB")]
    Srgb8Alpha8EXT                           = 0x8C43,
    CompressedSrgb                           = 0x8C48,
    CompressedSrgbAlpha                      = 0x8C49,

    [GLExtension("GL_EXT_texture_compression_s3tc_srgb")]
    CompressedSrgbS3tcDxt1EXT                = 0x8C4C,

    [GLExtension("GL_EXT_texture_compression_s3tc_srgb")]
    CompressedSrgbAlphaS3tcDxt1EXT           = 0x8C4D,

    [GLExtension("GL_EXT_texture_compression_s3tc_srgb")]
    CompressedSrgbAlphaS3tcDxt3EXT           = 0x8C4E,

    [GLExtension("GL_EXT_texture_compression_s3tc_srgb")]
    CompressedSrgbAlphaS3tcDxt5EXT           = 0x8C4F,
    DepthComponent32f                        = 0x8CAC,
    Depth32fStencil8                         = 0x8CAD,
    StencilIndex1                            = 0x8D46,

    [GLExtension("GL_EXT_framebuffer_object")]
    StencilIndex1EXT                         = 0x8D46,
    StencilIndex4                            = 0x8D47,

    [GLExtension("GL_EXT_framebuffer_object")]
    StencilIndex4EXT                         = 0x8D47,
    StencilIndex8                            = 0x8D48,

    [GLExtension("GL_EXT_framebuffer_object")]
    StencilIndex8EXT                         = 0x8D48,
    StencilIndex16                           = 0x8D49,

    [GLExtension("GL_EXT_framebuffer_object")]
    StencilIndex16EXT                        = 0x8D49,
    Rgb565                                   = 0x8D62,
    Rgba32ui                                 = 0x8D70,

    [GLExtension("GL_EXT_texture_integer")]
    Rgba32uiEXT                              = 0x8D70,
    Rgb32ui                                  = 0x8D71,

    [GLExtension("GL_EXT_texture_integer")]
    Rgb32uiEXT                               = 0x8D71,

    [GLExtension("GL_EXT_texture_integer")]
    Alpha32uiEXT                             = 0x8D72,

    [GLExtension("GL_EXT_texture_integer")]
    Intensity32uiEXT                         = 0x8D73,

    [GLExtension("GL_EXT_texture_integer")]
    Luminance32uiEXT                         = 0x8D74,

    [GLExtension("GL_EXT_texture_integer")]
    LuminanceAlpha32uiEXT                    = 0x8D75,
    Rgba16ui                                 = 0x8D76,

    [GLExtension("GL_EXT_texture_integer")]
    Rgba16uiEXT                              = 0x8D76,
    Rgb16ui                                  = 0x8D77,

    [GLExtension("GL_EXT_texture_integer")]
    Rgb16uiEXT                               = 0x8D77,

    [GLExtension("GL_EXT_texture_integer")]
    Alpha16uiEXT                             = 0x8D78,

    [GLExtension("GL_EXT_texture_integer")]
    Intensity16uiEXT                         = 0x8D79,

    [GLExtension("GL_EXT_texture_integer")]
    Luminance16uiEXT                         = 0x8D7A,

    [GLExtension("GL_EXT_texture_integer")]
    LuminanceAlpha16uiEXT                    = 0x8D7B,
    Rgba8ui                                  = 0x8D7C,

    [GLExtension("GL_EXT_texture_integer")]
    Rgba8uiEXT                               = 0x8D7C,
    Rgb8ui                                   = 0x8D7D,

    [GLExtension("GL_EXT_texture_integer")]
    Rgb8uiEXT                                = 0x8D7D,

    [GLExtension("GL_EXT_texture_integer")]
    Alpha8uiEXT                              = 0x8D7E,

    [GLExtension("GL_EXT_texture_integer")]
    Intensity8uiEXT                          = 0x8D7F,

    [GLExtension("GL_EXT_texture_integer")]
    Luminance8uiEXT                          = 0x8D80,

    [GLExtension("GL_EXT_texture_integer")]
    LuminanceAlpha8uiEXT                     = 0x8D81,
    Rgba32i                                  = 0x8D82,

    [GLExtension("GL_EXT_texture_integer")]
    Rgba32iEXT                               = 0x8D82,
    Rgb32i                                   = 0x8D83,

    [GLExtension("GL_EXT_texture_integer")]
    Rgb32iEXT                                = 0x8D83,

    [GLExtension("GL_EXT_texture_integer")]
    Alpha32iEXT                              = 0x8D84,

    [GLExtension("GL_EXT_texture_integer")]
    Intensity32iEXT                          = 0x8D85,

    [GLExtension("GL_EXT_texture_integer")]
    Luminance32iEXT                          = 0x8D86,

    [GLExtension("GL_EXT_texture_integer")]
    LuminanceAlpha32iEXT                     = 0x8D87,
    Rgba16i                                  = 0x8D88,

    [GLExtension("GL_EXT_texture_integer")]
    Rgba16iEXT                               = 0x8D88,
    Rgb16i                                   = 0x8D89,

    [GLExtension("GL_EXT_texture_integer")]
    Rgb16iEXT                                = 0x8D89,

    [GLExtension("GL_EXT_texture_integer")]
    Alpha16iEXT                              = 0x8D8A,

    [GLExtension("GL_EXT_texture_integer")]
    Intensity16iEXT                          = 0x8D8B,

    [GLExtension("GL_EXT_texture_integer")]
    Luminance16iEXT                          = 0x8D8C,

    [GLExtension("GL_EXT_texture_integer")]
    LuminanceAlpha16iEXT                     = 0x8D8D,
    Rgba8i                                   = 0x8D8E,

    [GLExtension("GL_EXT_texture_integer")]
    Rgba8iEXT                                = 0x8D8E,
    Rgb8i                                    = 0x8D8F,

    [GLExtension("GL_EXT_texture_integer")]
    Rgb8iEXT                                 = 0x8D8F,

    [GLExtension("GL_EXT_texture_integer")]
    Alpha8iEXT                               = 0x8D90,

    [GLExtension("GL_EXT_texture_integer")]
    Intensity8iEXT                           = 0x8D91,

    [GLExtension("GL_EXT_texture_integer")]
    Luminance8iEXT                           = 0x8D92,

    [GLExtension("GL_EXT_texture_integer")]
    LuminanceAlpha8iEXT                      = 0x8D93,
    CompressedRedRgtc1                       = 0x8DBB,

    [GLExtension("GL_EXT_texture_compression_rgtc")]
    CompressedRedRgtc1EXT                    = 0x8DBB,
    CompressedSignedRedRgtc1                 = 0x8DBC,

    [GLExtension("GL_EXT_texture_compression_rgtc")]
    CompressedSignedRedRgtc1EXT              = 0x8DBC,

    [GLExtension("GL_EXT_texture_compression_rgtc")]
    CompressedRedGreenRgtc2EXT               = 0x8DBD,
    CompressedRgRgtc2                        = 0x8DBD,

    [GLExtension("GL_EXT_texture_compression_rgtc")]
    CompressedSignedRedGreenRgtc2EXT         = 0x8DBE,
    CompressedSignedRgRgtc2                  = 0x8DBE,
    CompressedRgbaBptcUnorm                  = 0x8E8C,

    [GLExtension("GL_ARB_texture_compression_bptc")]
    CompressedRgbaBptcUnormARB               = 0x8E8C,

    [GLExtension("GL_EXT_texture_compression_bptc")]
    CompressedRgbaBptcUnormEXT               = 0x8E8C,
    CompressedSrgbAlphaBptcUnorm             = 0x8E8D,

    [GLExtension("GL_ARB_texture_compression_bptc")]
    CompressedSrgbAlphaBptcUnormARB          = 0x8E8D,

    [GLExtension("GL_EXT_texture_compression_bptc")]
    CompressedSrgbAlphaBptcUnormEXT          = 0x8E8D,
    CompressedRgbBptcSignedFloat             = 0x8E8E,

    [GLExtension("GL_ARB_texture_compression_bptc")]
    CompressedRgbBptcSignedFloatARB          = 0x8E8E,

    [GLExtension("GL_EXT_texture_compression_bptc")]
    CompressedRgbBptcSignedFloatEXT          = 0x8E8E,
    CompressedRgbBptcUnsignedFloat           = 0x8E8F,

    [GLExtension("GL_ARB_texture_compression_bptc")]
    CompressedRgbBptcUnsignedFloatARB        = 0x8E8F,

    [GLExtension("GL_EXT_texture_compression_bptc")]
    CompressedRgbBptcUnsignedFloatEXT        = 0x8E8F,
    R8Snorm                                  = 0x8F94,
    Rg8Snorm                                 = 0x8F95,
    Rgb8Snorm                                = 0x8F96,
    Rgba8Snorm                               = 0x8F97,
    R16Snorm                                 = 0x8F98,

    [GLExtension("GL_EXT_render_snorm")]
    R16SnormEXT                              = 0x8F98,
    Rg16Snorm                                = 0x8F99,

    [GLExtension("GL_EXT_render_snorm")]
    Rg16SnormEXT                             = 0x8F99,
    Rgb16Snorm                               = 0x8F9A,

    [GLExtension("GL_EXT_texture_norm16")]
    Rgb16SnormEXT                            = 0x8F9A,
    Rgba16Snorm                              = 0x8F9B,

    [GLExtension("GL_EXT_render_snorm")]
    Rgba16SnormEXT                           = 0x8F9B,

    [GLExtension("GL_EXT_texture_sRGB_R8")]
    Sr8EXT                                   = 0x8FBD,

    [GLExtension("GL_EXT_texture_sRGB_RG8")]
    Srg8EXT                                  = 0x8FBE,
    Rgb10A2ui                                = 0x906F,
    CompressedR11Eac                         = 0x9270,
    CompressedR11EacOES                      = 0x9270,
    CompressedSignedR11Eac                   = 0x9271,
    CompressedSignedR11EacOES                = 0x9271,
    CompressedRg11Eac                        = 0x9272,
    CompressedRg11EacOES                     = 0x9272,
    CompressedSignedRg11Eac                  = 0x9273,
    CompressedSignedRg11EacOES               = 0x9273,
    CompressedRgb8Etc2                       = 0x9274,
    CompressedRgb8Etc2OES                    = 0x9274,
    CompressedSrgb8Etc2                      = 0x9275,
    CompressedSrgb8Etc2OES                   = 0x9275,
    CompressedRgb8PunchthroughAlpha1Etc2     = 0x9276,
    CompressedRgb8PunchthroughAlpha1Etc2OES  = 0x9276,
    CompressedSrgb8PunchthroughAlpha1Etc2    = 0x9277,
    CompressedSrgb8PunchthroughAlpha1Etc2OES = 0x9277,
    CompressedRgba8Etc2Eac                   = 0x9278,
    CompressedRgba8Etc2EacOES                = 0x9278,
    CompressedSrgb8Alpha8Etc2Eac             = 0x9279,
    CompressedSrgb8Alpha8Etc2EacOES          = 0x9279,
    CompressedRgbaAstc4x4                    = 0x93B0,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedRgbaAstc4x4Khr                 = 0x93B0,
    CompressedRgbaAstc5x4                    = 0x93B1,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedRgbaAstc5x4Khr                 = 0x93B1,
    CompressedRgbaAstc5x5                    = 0x93B2,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedRgbaAstc5x5Khr                 = 0x93B2,
    CompressedRgbaAstc6x5                    = 0x93B3,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedRgbaAstc6x5Khr                 = 0x93B3,
    CompressedRgbaAstc6x6                    = 0x93B4,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedRgbaAstc6x6Khr                 = 0x93B4,
    CompressedRgbaAstc8x5                    = 0x93B5,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedRgbaAstc8x5Khr                 = 0x93B5,
    CompressedRgbaAstc8x6                    = 0x93B6,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedRgbaAstc8x6Khr                 = 0x93B6,
    CompressedRgbaAstc8x8                    = 0x93B7,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedRgbaAstc8x8Khr                 = 0x93B7,
    CompressedRgbaAstc10x5                   = 0x93B8,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedRgbaAstc10x5Khr                = 0x93B8,
    CompressedRgbaAstc10x6                   = 0x93B9,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedRgbaAstc10x6Khr                = 0x93B9,
    CompressedRgbaAstc10x8                   = 0x93BA,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedRgbaAstc10x8Khr                = 0x93BA,
    CompressedRgbaAstc10x10                  = 0x93BB,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedRgbaAstc10x10Khr               = 0x93BB,
    CompressedRgbaAstc12x10                  = 0x93BC,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedRgbaAstc12x10Khr               = 0x93BC,
    CompressedRgbaAstc12x12                  = 0x93BD,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedRgbaAstc12x12Khr               = 0x93BD,
    CompressedSrgb8Alpha8Astc4x4             = 0x93D0,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedSrgb8Alpha8Astc4x4Khr          = 0x93D0,
    CompressedSrgb8Alpha8Astc5x4             = 0x93D1,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedSrgb8Alpha8Astc5x4Khr          = 0x93D1,
    CompressedSrgb8Alpha8Astc5x5             = 0x93D2,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedSrgb8Alpha8Astc5x5Khr          = 0x93D2,
    CompressedSrgb8Alpha8Astc6x5             = 0x93D3,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedSrgb8Alpha8Astc6x5Khr          = 0x93D3,
    CompressedSrgb8Alpha8Astc6x6             = 0x93D4,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedSrgb8Alpha8Astc6x6Khr          = 0x93D4,
    CompressedSrgb8Alpha8Astc8x5             = 0x93D5,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedSrgb8Alpha8Astc8x5Khr          = 0x93D5,
    CompressedSrgb8Alpha8Astc8x6             = 0x93D6,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedSrgb8Alpha8Astc8x6Khr          = 0x93D6,
    CompressedSrgb8Alpha8Astc8x8             = 0x93D7,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedSrgb8Alpha8Astc8x8Khr          = 0x93D7,
    CompressedSrgb8Alpha8Astc10x5            = 0x93D8,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedSrgb8Alpha8Astc10x5Khr         = 0x93D8,
    CompressedSrgb8Alpha8Astc10x6            = 0x93D9,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedSrgb8Alpha8Astc10x6Khr         = 0x93D9,
    CompressedSrgb8Alpha8Astc10x8            = 0x93DA,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedSrgb8Alpha8Astc10x8Khr         = 0x93DA,
    CompressedSrgb8Alpha8Astc10x10           = 0x93DB,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedSrgb8Alpha8Astc10x10Khr        = 0x93DB,
    CompressedSrgb8Alpha8Astc12x10           = 0x93DC,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedSrgb8Alpha8Astc12x10Khr        = 0x93DC,
    CompressedSrgb8Alpha8Astc12x12           = 0x93DD,

    [GLExtension("GL_KHR_texture_compression_astc_hdr")]
    CompressedSrgb8Alpha8Astc12x12Khr        = 0x93DD,
}

