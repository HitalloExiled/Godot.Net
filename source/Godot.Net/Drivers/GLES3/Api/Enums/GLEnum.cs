namespace Godot.Net.Drivers.GLES3.Api.Enums;

using Godot.Net.Drivers.GLES3.Api.Attributes;

#pragma warning disable CA1069

public enum GLEnum
{

    [GLExtension("GL_EXT_blend_func_extended")]
    SrcAlphaSaturateEXT                                            = 0x0308,

    [GLExtension("GL_KHR_debug")]
    StackOverflowKhr                                               = 0x0503,

    [GLExtension("GL_KHR_debug")]
    StackUnderflowKhr                                              = 0x0504,
    ContextLost                                                    = 0x0507,

    [GLExtension("GL_KHR_robustness")]
    ContextLostKhr                                                 = 0x0507,

    [GLExtension("GL_EXT_clip_cull_distance")]
    MaxClipDistancesEXT                                            = 0x0D32,

    [GLExtension("GL_EXT_texture_border_clamp")]
    TextureBorderColorEXT                                          = 0x1004,
    TextureTarget                                                  = 0x1006,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview0ARB                                                  = 0x1700,

    [GLExtension("GL_EXT_clip_cull_distance")]
    ClipDistance0EXT                                               = 0x3000,

    [GLExtension("GL_EXT_clip_cull_distance")]
    ClipDistance1EXT                                               = 0x3001,

    [GLExtension("GL_EXT_clip_cull_distance")]
    ClipDistance2EXT                                               = 0x3002,

    [GLExtension("GL_EXT_clip_cull_distance")]
    ClipDistance3EXT                                               = 0x3003,

    [GLExtension("GL_EXT_clip_cull_distance")]
    ClipDistance4EXT                                               = 0x3004,

    [GLExtension("GL_EXT_clip_cull_distance")]
    ClipDistance5EXT                                               = 0x3005,

    [GLExtension("GL_EXT_clip_cull_distance")]
    ClipDistance6EXT                                               = 0x3006,

    [GLExtension("GL_EXT_clip_cull_distance")]
    ClipDistance7EXT                                               = 0x3007,

    [GLExtension("GL_EXT_blend_color")]
    ConstantColorEXT                                               = 0x8001,

    [GLExtension("GL_EXT_blend_color")]
    OneMinusConstantColorEXT                                       = 0x8002,

    [GLExtension("GL_EXT_blend_color")]
    ConstantAlphaEXT                                               = 0x8003,

    [GLExtension("GL_EXT_blend_color")]
    OneMinusConstantAlphaEXT                                       = 0x8004,

    [GLExtension("GL_EXT_blend_equation_separate")]
    BlendEquationRgbEXT                                            = 0x8009,

    [GLExtension("GL_EXT_polygon_offset")]
    PolygonOffsetEXT                                               = 0x8037,

    [GLExtension("GL_EXT_polygon_offset")]
    PolygonOffsetFactorEXT                                         = 0x8038,
    RescaleNormal                                                  = 0x803A,

    [GLExtension("GL_EXT_texture")]
    IntensityEXT                                                   = 0x8049,

    [GLExtension("GL_EXT_texture")]
    TextureRedSizeEXT                                              = 0x805C,

    [GLExtension("GL_EXT_texture")]
    TextureGreenSizeEXT                                            = 0x805D,

    [GLExtension("GL_EXT_texture")]
    TextureBlueSizeEXT                                             = 0x805E,

    [GLExtension("GL_EXT_texture")]
    TextureAlphaSizeEXT                                            = 0x805F,

    [GLExtension("GL_EXT_texture")]
    TextureLuminanceSizeEXT                                        = 0x8060,

    [GLExtension("GL_EXT_texture")]
    TextureIntensitySizeEXT                                        = 0x8061,

    [GLExtension("GL_EXT_texture_object")]
    TextureResidentEXT                                             = 0x8067,

    [GLExtension("GL_EXT_texture_object")]
    Texture1DBindingEXT                                            = 0x8068,

    [GLExtension("GL_EXT_texture_object")]
    Texture2DBindingEXT                                            = 0x8069,
    Texture3DBindingOES                                            = 0x806A,
    TextureDepth                                                   = 0x8071,

    [GLExtension("GL_EXT_vertex_array")]
    VertexArrayEXT                                                 = 0x8074,

    [GLExtension("GL_KHR_debug")]
    VertexArrayKhr                                                 = 0x8074,

    [GLExtension("GL_EXT_vertex_array")]
    NormalArrayEXT                                                 = 0x8075,

    [GLExtension("GL_EXT_vertex_array")]
    ColorArrayEXT                                                  = 0x8076,

    [GLExtension("GL_EXT_vertex_array")]
    IndexArrayEXT                                                  = 0x8077,

    [GLExtension("GL_EXT_vertex_array")]
    TextureCoordArrayEXT                                           = 0x8078,

    [GLExtension("GL_EXT_vertex_array")]
    EdgeFlagArrayEXT                                               = 0x8079,

    [GLExtension("GL_EXT_vertex_array")]
    VertexArraySizeEXT                                             = 0x807A,

    [GLExtension("GL_EXT_vertex_array")]
    VertexArrayTypeEXT                                             = 0x807B,

    [GLExtension("GL_EXT_vertex_array")]
    VertexArrayStrideEXT                                           = 0x807C,

    [GLExtension("GL_EXT_vertex_array")]
    NormalArrayTypeEXT                                             = 0x807E,

    [GLExtension("GL_EXT_vertex_array")]
    NormalArrayStrideEXT                                           = 0x807F,

    [GLExtension("GL_EXT_vertex_array")]
    ColorArraySizeEXT                                              = 0x8081,

    [GLExtension("GL_EXT_vertex_array")]
    ColorArrayTypeEXT                                              = 0x8082,

    [GLExtension("GL_EXT_vertex_array")]
    ColorArrayStrideEXT                                            = 0x8083,

    [GLExtension("GL_EXT_vertex_array")]
    IndexArrayTypeEXT                                              = 0x8085,

    [GLExtension("GL_EXT_vertex_array")]
    IndexArrayStrideEXT                                            = 0x8086,

    [GLExtension("GL_EXT_vertex_array")]
    TextureCoordArraySizeEXT                                       = 0x8088,

    [GLExtension("GL_EXT_vertex_array")]
    TextureCoordArrayTypeEXT                                       = 0x8089,

    [GLExtension("GL_EXT_vertex_array")]
    TextureCoordArrayStrideEXT                                     = 0x808A,

    [GLExtension("GL_EXT_vertex_array")]
    EdgeFlagArrayStrideEXT                                         = 0x808C,

    [GLExtension("GL_ARB_multisample")]
    MultisampleARB                                                 = 0x809D,

    [GLExtension("GL_EXT_multisample")]
    MultisampleEXT                                                 = 0x809D,

    [GLExtension("GL_ARB_multisample")]
    SampleAlphaToCoverageARB                                       = 0x809E,

    [GLExtension("GL_EXT_multisample")]
    SampleAlphaToMaskEXT                                           = 0x809E,

    [GLExtension("GL_ARB_multisample")]
    SampleAlphaToOneARB                                            = 0x809F,

    [GLExtension("GL_EXT_multisample")]
    SampleAlphaToOneEXT                                            = 0x809F,

    [GLExtension("GL_ARB_multisample")]
    SampleCoverageARB                                              = 0x80A0,

    [GLExtension("GL_EXT_multisample")]
    SampleMaskEXT                                                  = 0x80A0,

    [GLExtension("GL_ARB_multisample")]
    SampleBuffersARB                                               = 0x80A8,

    [GLExtension("GL_EXT_multisample")]
    SampleBuffersEXT                                               = 0x80A8,

    [GLExtension("GL_ARB_multisample")]
    SamplesARB                                                     = 0x80A9,

    [GLExtension("GL_EXT_multisample")]
    SamplesEXT                                                     = 0x80A9,

    [GLExtension("GL_ARB_multisample")]
    SampleCoverageValueARB                                         = 0x80AA,

    [GLExtension("GL_EXT_multisample")]
    SampleMaskValueEXT                                             = 0x80AA,

    [GLExtension("GL_ARB_multisample")]
    SampleCoverageInvertARB                                        = 0x80AB,

    [GLExtension("GL_EXT_multisample")]
    SampleMaskInvertEXT                                            = 0x80AB,

    [GLExtension("GL_EXT_multisample")]
    SamplePatternEXT                                               = 0x80AC,

    [GLExtension("GL_ARB_imaging")]
    ColorMatrix                                                    = 0x80B1,

    [GLExtension("GL_ARB_imaging")]
    ColorMatrixStackDepth                                          = 0x80B2,

    [GLExtension("GL_ARB_imaging")]
    MaxColorMatrixStackDepth                                       = 0x80B3,

    [GLExtension("GL_ARB_shadow_ambient")]
    TextureCompareFailValueARB                                     = 0x80BF,

    [GLExtension("GL_EXT_blend_func_separate")]
    BlendDstRgbEXT                                                 = 0x80C8,

    [GLExtension("GL_EXT_blend_func_separate")]
    BlendSrcRgbEXT                                                 = 0x80C9,

    [GLExtension("GL_EXT_blend_func_separate")]
    BlendDstAlphaEXT                                               = 0x80CA,

    [GLExtension("GL_EXT_blend_func_separate")]
    BlendSrcAlphaEXT                                               = 0x80CB,

    [GLExtension("GL_EXT_422_pixels")]
    _422EXT                                                        = 0x80CC,

    [GLExtension("GL_EXT_422_pixels")]
    _422RevEXT                                                     = 0x80CD,

    [GLExtension("GL_EXT_422_pixels")]
    _422AverageEXT                                                 = 0x80CE,

    [GLExtension("GL_EXT_422_pixels")]
    _422RevAverageEXT                                              = 0x80CF,

    [GLExtension("GL_EXT_paletted_texture")]
    ColorIndex1EXT                                                 = 0x80E2,

    [GLExtension("GL_EXT_paletted_texture")]
    ColorIndex2EXT                                                 = 0x80E3,

    [GLExtension("GL_EXT_paletted_texture")]
    ColorIndex4EXT                                                 = 0x80E4,

    [GLExtension("GL_EXT_paletted_texture")]
    ColorIndex8EXT                                                 = 0x80E5,

    [GLExtension("GL_EXT_paletted_texture")]
    ColorIndex12EXT                                                = 0x80E6,

    [GLExtension("GL_EXT_paletted_texture")]
    ColorIndex16EXT                                                = 0x80E7,

    [GLExtension("GL_EXT_draw_range_elements")]
    MaxElementsVerticesEXT                                         = 0x80E8,

    [GLExtension("GL_EXT_draw_range_elements")]
    MaxElementsIndicesEXT                                          = 0x80E9,

    [GLExtension("GL_EXT_paletted_texture")]
    TextureIndexSizeEXT                                            = 0x80ED,

    [GLExtension("GL_ARB_indirect_parameters")]
    ParameterBufferARB                                             = 0x80EE,
    ParameterBufferBinding                                         = 0x80EF,

    [GLExtension("GL_ARB_indirect_parameters")]
    ParameterBufferBindingARB                                      = 0x80EF,

    [GLExtension("GL_EXT_texture_border_clamp")]
    ClampToBorderEXT                                               = 0x812D,

    [GLExtension("GL_ARB_imaging")]
    ConstantBorder                                                 = 0x8151,

    [GLExtension("GL_ARB_imaging")]
    ReplicateBorder                                                = 0x8153,

    [GLExtension("GL_EXT_compiled_vertex_array")]
    ArrayElementLockFirstEXT                                       = 0x81A8,

    [GLExtension("GL_EXT_compiled_vertex_array")]
    ArrayElementLockCountEXT                                       = 0x81A9,

    [GLExtension("GL_EXT_cull_vertex")]
    CullVertexEXT                                                  = 0x81AA,

    [GLExtension("GL_EXT_index_array_formats")]
    IuiV2fEXT                                                      = 0x81AD,

    [GLExtension("GL_EXT_index_array_formats")]
    IuiV3fEXT                                                      = 0x81AE,

    [GLExtension("GL_EXT_index_array_formats")]
    IuiN3fV2fEXT                                                   = 0x81AF,

    [GLExtension("GL_EXT_index_array_formats")]
    IuiN3fV3fEXT                                                   = 0x81B0,

    [GLExtension("GL_EXT_index_array_formats")]
    T2fIuiV2fEXT                                                   = 0x81B1,

    [GLExtension("GL_EXT_index_array_formats")]
    T2fIuiV3fEXT                                                   = 0x81B2,

    [GLExtension("GL_EXT_index_array_formats")]
    T2fIuiN3fV2fEXT                                                = 0x81B3,

    [GLExtension("GL_EXT_index_array_formats")]
    T2fIuiN3fV3fEXT                                                = 0x81B4,

    [GLExtension("GL_EXT_index_func")]
    IndexTestEXT                                                   = 0x81B5,

    [GLExtension("GL_EXT_index_func")]
    IndexTestFuncEXT                                               = 0x81B6,

    [GLExtension("GL_EXT_index_func")]
    IndexTestRefEXT                                                = 0x81B7,

    [GLExtension("GL_EXT_index_material")]
    IndexMaterialEXT                                               = 0x81B8,

    [GLExtension("GL_EXT_index_material")]
    IndexMaterialParameterEXT                                      = 0x81B9,

    [GLExtension("GL_EXT_index_material")]
    IndexMaterialFaceEXT                                           = 0x81BA,
    FramebufferDefault                                             = 0x8218,

    [GLExtension("GL_EXT_buffer_storage")]
    BufferImmutableStorageEXT                                      = 0x821F,

    [GLExtension("GL_EXT_buffer_storage")]
    BufferStorageFlagsEXT                                          = 0x8220,
    PrimitiveRestartForPatchesSupported                            = 0x8221,
    Index                                                          = 0x8222,

    [GLExtension("GL_EXT_texture_rg")]
    RgEXT                                                          = 0x8227,

    [GLExtension("GL_ARB_cl_event")]
    SyncClEventARB                                                 = 0x8240,

    [GLExtension("GL_ARB_cl_event")]
    SyncClEventCompleteARB                                         = 0x8241,

    [GLExtension("GL_ARB_debug_output")]
    DebugOutputSynchronousARB                                      = 0x8242,

    [GLExtension("GL_KHR_debug")]
    DebugOutputSynchronousKhr                                      = 0x8242,
    DebugNextLoggedMessageLength                                   = 0x8243,

    [GLExtension("GL_ARB_debug_output")]
    DebugNextLoggedMessageLengthARB                                = 0x8243,

    [GLExtension("GL_KHR_debug")]
    DebugNextLoggedMessageLengthKhr                                = 0x8243,

    [GLExtension("GL_ARB_debug_output")]
    DebugCallbackFunctionARB                                       = 0x8244,

    [GLExtension("GL_KHR_debug")]
    DebugCallbackFunctionKhr                                       = 0x8244,

    [GLExtension("GL_ARB_debug_output")]
    DebugCallbackUserParamARB                                      = 0x8245,

    [GLExtension("GL_KHR_debug")]
    DebugCallbackUserParamKhr                                      = 0x8245,

    [GLExtension("GL_ARB_debug_output")]
    DebugSourceApiARB                                              = 0x8246,

    [GLExtension("GL_KHR_debug")]
    DebugSourceApiKhr                                              = 0x8246,

    [GLExtension("GL_ARB_debug_output")]
    DebugSourceWindowSystemARB                                     = 0x8247,

    [GLExtension("GL_KHR_debug")]
    DebugSourceWindowSystemKhr                                     = 0x8247,

    [GLExtension("GL_ARB_debug_output")]
    DebugSourceShaderCompilerARB                                   = 0x8248,

    [GLExtension("GL_KHR_debug")]
    DebugSourceShaderCompilerKhr                                   = 0x8248,

    [GLExtension("GL_ARB_debug_output")]
    DebugSourceThirdPartyARB                                       = 0x8249,

    [GLExtension("GL_KHR_debug")]
    DebugSourceThirdPartyKhr                                       = 0x8249,

    [GLExtension("GL_ARB_debug_output")]
    DebugSourceApplicationARB                                      = 0x824A,

    [GLExtension("GL_KHR_debug")]
    DebugSourceApplicationKhr                                      = 0x824A,

    [GLExtension("GL_ARB_debug_output")]
    DebugSourceOtherARB                                            = 0x824B,

    [GLExtension("GL_KHR_debug")]
    DebugSourceOtherKhr                                            = 0x824B,

    [GLExtension("GL_ARB_debug_output")]
    DebugTypeErrorARB                                              = 0x824C,

    [GLExtension("GL_KHR_debug")]
    DebugTypeErrorKhr                                              = 0x824C,

    [GLExtension("GL_ARB_debug_output")]
    DebugTypeDeprecatedBehaviorARB                                 = 0x824D,

    [GLExtension("GL_KHR_debug")]
    DebugTypeDeprecatedBehaviorKhr                                 = 0x824D,

    [GLExtension("GL_ARB_debug_output")]
    DebugTypeUndefinedBehaviorARB                                  = 0x824E,

    [GLExtension("GL_KHR_debug")]
    DebugTypeUndefinedBehaviorKhr                                  = 0x824E,

    [GLExtension("GL_ARB_debug_output")]
    DebugTypePortabilityARB                                        = 0x824F,

    [GLExtension("GL_KHR_debug")]
    DebugTypePortabilityKhr                                        = 0x824F,

    [GLExtension("GL_ARB_debug_output")]
    DebugTypePerformanceARB                                        = 0x8250,

    [GLExtension("GL_KHR_debug")]
    DebugTypePerformanceKhr                                        = 0x8250,

    [GLExtension("GL_ARB_debug_output")]
    DebugTypeOtherARB                                              = 0x8251,

    [GLExtension("GL_KHR_debug")]
    DebugTypeOtherKhr                                              = 0x8251,
    LoseContextOnReset                                             = 0x8252,

    [GLExtension("GL_ARB_robustness")]
    LoseContextOnResetARB                                          = 0x8252,

    [GLExtension("GL_EXT_robustness")]
    LoseContextOnResetEXT                                          = 0x8252,

    [GLExtension("GL_KHR_robustness")]
    LoseContextOnResetKhr                                          = 0x8252,

    [GLExtension("GL_ARB_robustness")]
    GuiltyContextResetARB                                          = 0x8253,

    [GLExtension("GL_EXT_robustness")]
    GuiltyContextResetEXT                                          = 0x8253,

    [GLExtension("GL_KHR_robustness")]
    GuiltyContextResetKhr                                          = 0x8253,

    [GLExtension("GL_ARB_robustness")]
    InnocentContextResetARB                                        = 0x8254,

    [GLExtension("GL_EXT_robustness")]
    InnocentContextResetEXT                                        = 0x8254,

    [GLExtension("GL_KHR_robustness")]
    InnocentContextResetKhr                                        = 0x8254,

    [GLExtension("GL_ARB_robustness")]
    UnknownContextResetARB                                         = 0x8255,

    [GLExtension("GL_EXT_robustness")]
    UnknownContextResetEXT                                         = 0x8255,

    [GLExtension("GL_KHR_robustness")]
    UnknownContextResetKhr                                         = 0x8255,
    ResetNotificationStrategy                                      = 0x8256,

    [GLExtension("GL_ARB_robustness")]
    ResetNotificationStrategyARB                                   = 0x8256,

    [GLExtension("GL_EXT_robustness")]
    ResetNotificationStrategyEXT                                   = 0x8256,

    [GLExtension("GL_KHR_robustness")]
    ResetNotificationStrategyKhr                                   = 0x8256,

    [GLExtension("GL_EXT_separate_shader_objects")]
    ProgramSeparableEXT                                            = 0x8258,

    [GLExtension("GL_EXT_separate_shader_objects")]
    ActiveProgramEXT                                               = 0x8259,

    [GLExtension("GL_EXT_separate_shader_objects")]
    ProgramPipelineBindingEXT                                      = 0x825A,
    ViewportSubpixelBitsEXT                                        = 0x825C,
    ViewportBoundsRangeEXT                                         = 0x825D,

    [GLExtension("GL_EXT_geometry_shader")]
    LayerProvokingVertexEXT                                        = 0x825E,
    ViewportIndexProvokingVertexEXT                                = 0x825F,
    UndefinedVertex                                                = 0x8260,

    [GLExtension("GL_EXT_geometry_shader")]
    UndefinedVertexEXT                                             = 0x8260,
    NoResetNotification                                            = 0x8261,

    [GLExtension("GL_ARB_robustness")]
    NoResetNotificationARB                                         = 0x8261,

    [GLExtension("GL_EXT_robustness")]
    NoResetNotificationEXT                                         = 0x8261,

    [GLExtension("GL_KHR_robustness")]
    NoResetNotificationKhr                                         = 0x8261,
    MaxComputeSharedMemorySize                                     = 0x8262,

    [GLExtension("GL_KHR_debug")]
    DebugTypeMarkerKhr                                             = 0x8268,

    [GLExtension("GL_KHR_debug")]
    DebugTypePushGroupKhr                                          = 0x8269,

    [GLExtension("GL_KHR_debug")]
    DebugTypePopGroupKhr                                           = 0x826A,

    [GLExtension("GL_KHR_debug")]
    DebugSeverityNotificationKhr                                   = 0x826B,

    [GLExtension("GL_KHR_debug")]
    MaxDebugGroupStackDepthKhr                                     = 0x826C,

    [GLExtension("GL_KHR_debug")]
    DebugGroupStackDepthKhr                                        = 0x826D,
    MaxCombinedDimensions                                          = 0x8282,
    DepthComponents                                                = 0x8284,
    StencilComponents                                              = 0x8285,
    ManualGenerateMipmap                                           = 0x8294,

    [GLExtension("GL_ARB_internalformat_query2")]
    SrgbDecodeARB                                                  = 0x8299,
    FullSupport                                                    = 0x82B7,
    CaveatSupport                                                  = 0x82B8,
    ImageClass4X32                                                 = 0x82B9,
    ImageClass2X32                                                 = 0x82BA,
    ImageClass1X32                                                 = 0x82BB,
    ImageClass4X16                                                 = 0x82BC,
    ImageClass2X16                                                 = 0x82BD,
    ImageClass1X16                                                 = 0x82BE,
    ImageClass4X8                                                  = 0x82BF,
    ImageClass2X8                                                  = 0x82C0,
    ImageClass1X8                                                  = 0x82C1,
    ImageClass111110                                               = 0x82C2,
    ImageClass1010102                                              = 0x82C3,
    ViewClass128Bits                                               = 0x82C4,
    ViewClass96Bits                                                = 0x82C5,
    ViewClass64Bits                                                = 0x82C6,
    ViewClass48Bits                                                = 0x82C7,
    ViewClass32Bits                                                = 0x82C8,
    ViewClass24Bits                                                = 0x82C9,
    ViewClass16Bits                                                = 0x82CA,
    ViewClass8Bits                                                 = 0x82CB,
    ViewClassS3tcDxt1Rgb                                           = 0x82CC,
    ViewClassS3tcDxt1Rgba                                          = 0x82CD,
    ViewClassS3tcDxt3Rgba                                          = 0x82CE,
    ViewClassS3tcDxt5Rgba                                          = 0x82CF,
    ViewClassRgtc1Red                                              = 0x82D0,
    ViewClassRgtc2Rg                                               = 0x82D1,
    ViewClassBptcUnorm                                             = 0x82D2,
    ViewClassBptcFloat                                             = 0x82D3,
    TextureViewMinLevel                                            = 0x82DB,

    [GLExtension("GL_EXT_texture_view")]
    TextureViewMinLevelEXT                                         = 0x82DB,
    TextureViewNumLevels                                           = 0x82DC,

    [GLExtension("GL_EXT_texture_view")]
    TextureViewNumLevelsEXT                                        = 0x82DC,
    TextureViewMinLayer                                            = 0x82DD,

    [GLExtension("GL_EXT_texture_view")]
    TextureViewMinLayerEXT                                         = 0x82DD,
    TextureViewNumLayers                                           = 0x82DE,

    [GLExtension("GL_EXT_texture_view")]
    TextureViewNumLayersEXT                                        = 0x82DE,
    TextureImmutableLevels                                         = 0x82DF,

    [GLExtension("GL_KHR_debug")]
    BufferKhr                                                      = 0x82E0,

    [GLExtension("GL_KHR_debug")]
    ShaderKhr                                                      = 0x82E1,

    [GLExtension("GL_KHR_debug")]
    ProgramKhr                                                     = 0x82E2,

    [GLExtension("GL_KHR_debug")]
    QueryKhr                                                       = 0x82E3,

    [GLExtension("GL_KHR_debug")]
    ProgramPipelineKhr                                             = 0x82E4,
    MaxVertexAttribStride                                          = 0x82E5,

    [GLExtension("GL_KHR_debug")]
    SamplerKhr                                                     = 0x82E6,
    DisplayList                                                    = 0x82E7,

    [GLExtension("GL_KHR_debug")]
    MaxLabelLengthKhr                                              = 0x82E8,
    NumShadingLanguageVersions                                     = 0x82E9,

    [GLExtension("GL_ARB_transform_feedback_overflow_query")]
    TransformFeedbackOverflowARB                                   = 0x82EC,
    TransformFeedbackStreamOverflow                                = 0x82ED,

    [GLExtension("GL_ARB_transform_feedback_overflow_query")]
    TransformFeedbackStreamOverflowARB                             = 0x82ED,

    [GLExtension("GL_ARB_pipeline_statistics_query")]
    VerticesSubmittedARB                                           = 0x82EE,

    [GLExtension("GL_ARB_pipeline_statistics_query")]
    PrimitivesSubmittedARB                                         = 0x82EF,

    [GLExtension("GL_ARB_pipeline_statistics_query")]
    VertexShaderInvocationsARB                                     = 0x82F0,
    TessControlShaderPatches                                       = 0x82F1,

    [GLExtension("GL_ARB_pipeline_statistics_query")]
    TessControlShaderPatchesARB                                    = 0x82F1,
    TessEvaluationShaderInvocations                                = 0x82F2,

    [GLExtension("GL_ARB_pipeline_statistics_query")]
    TessEvaluationShaderInvocationsARB                             = 0x82F2,
    GeometryShaderPrimitivesEmitted                                = 0x82F3,

    [GLExtension("GL_ARB_pipeline_statistics_query")]
    GeometryShaderPrimitivesEmittedARB                             = 0x82F3,
    FragmentShaderInvocations                                      = 0x82F4,

    [GLExtension("GL_ARB_pipeline_statistics_query")]
    FragmentShaderInvocationsARB                                   = 0x82F4,
    ComputeShaderInvocations                                       = 0x82F5,

    [GLExtension("GL_ARB_pipeline_statistics_query")]
    ComputeShaderInvocationsARB                                    = 0x82F5,
    ClippingInputPrimitives                                        = 0x82F6,

    [GLExtension("GL_ARB_pipeline_statistics_query")]
    ClippingInputPrimitivesARB                                     = 0x82F6,
    ClippingOutputPrimitives                                       = 0x82F7,

    [GLExtension("GL_ARB_pipeline_statistics_query")]
    ClippingOutputPrimitivesARB                                    = 0x82F7,

    [GLExtension("GL_ARB_sparse_buffer")]
    SparseBufferPageSizeARB                                        = 0x82F8,
    MaxCullDistances                                               = 0x82F9,

    [GLExtension("GL_EXT_clip_cull_distance")]
    MaxCullDistancesEXT                                            = 0x82F9,
    MaxCombinedClipAndCullDistances                                = 0x82FA,

    [GLExtension("GL_EXT_clip_cull_distance")]
    MaxCombinedClipAndCullDistancesEXT                             = 0x82FA,
    ContextReleaseBehavior                                         = 0x82FB,

    [GLExtension("GL_KHR_context_flush_control")]
    ContextReleaseBehaviorKhr                                      = 0x82FB,
    ContextReleaseBehaviorFlush                                    = 0x82FC,

    [GLExtension("GL_KHR_context_flush_control")]
    ContextReleaseBehaviorFlushKhr                                 = 0x82FC,
    RobustGpuTimeoutMsKhr                                          = 0x82FD,
    DepthPassInstrumentSGIX                                        = 0x8310,
    DepthPassInstrumentCountersSGIX                                = 0x8311,
    DepthPassInstrumentMaxSGIX                                     = 0x8312,
    FragmentsInstrumentSGIX                                        = 0x8313,
    FragmentsInstrumentCountersSGIX                                = 0x8314,
    FragmentsInstrumentMaxSGIX                                     = 0x8315,
    UnpackCompressedSizeSGIX                                       = 0x831A,
    PackMaxCompressedSizeSGIX                                      = 0x831B,
    PackCompressedSizeSGIX                                         = 0x831C,
    Slim8uSGIX                                                     = 0x831D,
    Slim10uSGIX                                                    = 0x831E,
    Slim12sSGIX                                                    = 0x831F,

    [GLExtension("GL_EXT_pixel_transform")]
    CubicEXT                                                       = 0x8334,

    [GLExtension("GL_EXT_pixel_transform")]
    AverageEXT                                                     = 0x8335,

    [GLExtension("GL_EXT_pixel_transform")]
    PixelTransform2DStackDepthEXT                                  = 0x8336,

    [GLExtension("GL_EXT_pixel_transform")]
    MaxPixelTransform2DStackDepthEXT                               = 0x8337,

    [GLExtension("GL_EXT_pixel_transform")]
    PixelTransform2DMatrixEXT                                      = 0x8338,

    [GLExtension("GL_EXT_light_texture")]
    TextureApplicationModeEXT                                      = 0x834F,

    [GLExtension("GL_EXT_light_texture")]
    TextureLightEXT                                                = 0x8350,

    [GLExtension("GL_EXT_light_texture")]
    TextureMaterialFaceEXT                                         = 0x8351,

    [GLExtension("GL_EXT_light_texture")]
    TextureMaterialParameterEXT                                    = 0x8352,

    [GLExtension("GL_ARB_texture_mirrored_repeat")]
    MirroredRepeatARB                                              = 0x8370,

    [GLExtension("GL_EXT_coordinate_frame")]
    TangentArrayEXT                                                = 0x8439,

    [GLExtension("GL_EXT_coordinate_frame")]
    BinormalArrayEXT                                               = 0x843A,

    [GLExtension("GL_EXT_coordinate_frame")]
    CurrentTangentEXT                                              = 0x843B,

    [GLExtension("GL_EXT_coordinate_frame")]
    CurrentBinormalEXT                                             = 0x843C,

    [GLExtension("GL_EXT_coordinate_frame")]
    TangentArrayTypeEXT                                            = 0x843E,

    [GLExtension("GL_EXT_coordinate_frame")]
    TangentArrayStrideEXT                                          = 0x843F,

    [GLExtension("GL_EXT_coordinate_frame")]
    BinormalArrayTypeEXT                                           = 0x8440,

    [GLExtension("GL_EXT_coordinate_frame")]
    BinormalArrayStrideEXT                                         = 0x8441,

    [GLExtension("GL_EXT_coordinate_frame")]
    TangentArrayPointerEXT                                         = 0x8442,

    [GLExtension("GL_EXT_coordinate_frame")]
    BinormalArrayPointerEXT                                        = 0x8443,

    [GLExtension("GL_EXT_coordinate_frame")]
    Map1TangentEXT                                                 = 0x8444,

    [GLExtension("GL_EXT_coordinate_frame")]
    Map2TangentEXT                                                 = 0x8445,

    [GLExtension("GL_EXT_coordinate_frame")]
    Map1BinormalEXT                                                = 0x8446,

    [GLExtension("GL_EXT_coordinate_frame")]
    Map2BinormalEXT                                                = 0x8447,
    FogCoordinateSource                                            = 0x8450,

    [GLExtension("GL_EXT_fog_coord")]
    FogCoordinateSourceEXT                                         = 0x8450,
    CurrentFogCoordinate                                           = 0x8453,
    CurrentFogCoord                                                = 0x8453,

    [GLExtension("GL_EXT_fog_coord")]
    CurrentFogCoordinateEXT                                        = 0x8453,
    FogCoordinateArrayType                                         = 0x8454,

    [GLExtension("GL_EXT_fog_coord")]
    FogCoordinateArrayTypeEXT                                      = 0x8454,
    FogCoordArrayType                                              = 0x8454,
    FogCoordinateArrayStride                                       = 0x8455,

    [GLExtension("GL_EXT_fog_coord")]
    FogCoordinateArrayStrideEXT                                    = 0x8455,
    FogCoordArrayStride                                            = 0x8455,
    FogCoordinateArrayPointer                                      = 0x8456,

    [GLExtension("GL_EXT_fog_coord")]
    FogCoordinateArrayPointerEXT                                   = 0x8456,
    FogCoordArrayPointer                                           = 0x8456,
    FogCoordinateArray                                             = 0x8457,

    [GLExtension("GL_EXT_fog_coord")]
    FogCoordinateArrayEXT                                          = 0x8457,
    FogCoordArray                                                  = 0x8457,
    ColorSum                                                       = 0x8458,

    [GLExtension("GL_ARB_vertex_program")]
    ColorSumARB                                                    = 0x8458,

    [GLExtension("GL_EXT_secondary_color")]
    ColorSumEXT                                                    = 0x8458,
    CurrentSecondaryColor                                          = 0x8459,

    [GLExtension("GL_EXT_secondary_color")]
    CurrentSecondaryColorEXT                                       = 0x8459,
    SecondaryColorArraySize                                        = 0x845A,

    [GLExtension("GL_EXT_secondary_color")]
    SecondaryColorArraySizeEXT                                     = 0x845A,
    SecondaryColorArrayType                                        = 0x845B,

    [GLExtension("GL_EXT_secondary_color")]
    SecondaryColorArrayTypeEXT                                     = 0x845B,
    SecondaryColorArrayStride                                      = 0x845C,

    [GLExtension("GL_EXT_secondary_color")]
    SecondaryColorArrayStrideEXT                                   = 0x845C,
    SecondaryColorArrayPointer                                     = 0x845D,

    [GLExtension("GL_EXT_secondary_color")]
    SecondaryColorArrayPointerEXT                                  = 0x845D,
    SecondaryColorArray                                            = 0x845E,

    [GLExtension("GL_EXT_secondary_color")]
    SecondaryColorArrayEXT                                         = 0x845E,
    CurrentRasterSecondaryColor                                    = 0x845F,

    [GLExtension("GL_ARB_multitexture")]
    Texture2ARB                                                    = 0x84C2,

    [GLExtension("GL_ARB_multitexture")]
    Texture3ARB                                                    = 0x84C3,

    [GLExtension("GL_ARB_multitexture")]
    Texture4ARB                                                    = 0x84C4,

    [GLExtension("GL_ARB_multitexture")]
    Texture5ARB                                                    = 0x84C5,

    [GLExtension("GL_ARB_multitexture")]
    Texture6ARB                                                    = 0x84C6,

    [GLExtension("GL_ARB_multitexture")]
    Texture7ARB                                                    = 0x84C7,

    [GLExtension("GL_ARB_multitexture")]
    Texture8ARB                                                    = 0x84C8,

    [GLExtension("GL_ARB_multitexture")]
    Texture9ARB                                                    = 0x84C9,

    [GLExtension("GL_ARB_multitexture")]
    Texture10ARB                                                   = 0x84CA,

    [GLExtension("GL_ARB_multitexture")]
    Texture11ARB                                                   = 0x84CB,

    [GLExtension("GL_ARB_multitexture")]
    Texture12ARB                                                   = 0x84CC,

    [GLExtension("GL_ARB_multitexture")]
    Texture13ARB                                                   = 0x84CD,

    [GLExtension("GL_ARB_multitexture")]
    Texture14ARB                                                   = 0x84CE,

    [GLExtension("GL_ARB_multitexture")]
    Texture15ARB                                                   = 0x84CF,

    [GLExtension("GL_ARB_multitexture")]
    Texture16ARB                                                   = 0x84D0,

    [GLExtension("GL_ARB_multitexture")]
    Texture17ARB                                                   = 0x84D1,

    [GLExtension("GL_ARB_multitexture")]
    Texture18ARB                                                   = 0x84D2,

    [GLExtension("GL_ARB_multitexture")]
    Texture19ARB                                                   = 0x84D3,

    [GLExtension("GL_ARB_multitexture")]
    Texture20ARB                                                   = 0x84D4,

    [GLExtension("GL_ARB_multitexture")]
    Texture21ARB                                                   = 0x84D5,

    [GLExtension("GL_ARB_multitexture")]
    Texture22ARB                                                   = 0x84D6,

    [GLExtension("GL_ARB_multitexture")]
    Texture23ARB                                                   = 0x84D7,

    [GLExtension("GL_ARB_multitexture")]
    Texture24ARB                                                   = 0x84D8,

    [GLExtension("GL_ARB_multitexture")]
    Texture25ARB                                                   = 0x84D9,

    [GLExtension("GL_ARB_multitexture")]
    Texture26ARB                                                   = 0x84DA,

    [GLExtension("GL_ARB_multitexture")]
    Texture27ARB                                                   = 0x84DB,

    [GLExtension("GL_ARB_multitexture")]
    Texture28ARB                                                   = 0x84DC,

    [GLExtension("GL_ARB_multitexture")]
    Texture29ARB                                                   = 0x84DD,

    [GLExtension("GL_ARB_multitexture")]
    Texture30ARB                                                   = 0x84DE,

    [GLExtension("GL_ARB_multitexture")]
    Texture31ARB                                                   = 0x84DF,

    [GLExtension("GL_ARB_multitexture")]
    ActiveTextureARB                                               = 0x84E0,
    ClientActiveTexture                                            = 0x84E1,

    [GLExtension("GL_ARB_multitexture")]
    ClientActiveTextureARB                                         = 0x84E1,
    MaxTextureUnits                                                = 0x84E2,

    [GLExtension("GL_ARB_multitexture")]
    MaxTextureUnitsARB                                             = 0x84E2,
    TransposeModelviewMatrix                                       = 0x84E3,

    [GLExtension("GL_ARB_transpose_matrix")]
    TransposeModelviewMatrixARB                                    = 0x84E3,
    TransposeProjectionMatrix                                      = 0x84E4,

    [GLExtension("GL_ARB_transpose_matrix")]
    TransposeProjectionMatrixARB                                   = 0x84E4,
    TransposeTextureMatrix                                         = 0x84E5,

    [GLExtension("GL_ARB_transpose_matrix")]
    TransposeTextureMatrixARB                                      = 0x84E5,
    TransposeColorMatrix                                           = 0x84E6,

    [GLExtension("GL_ARB_transpose_matrix")]
    TransposeColorMatrixARB                                        = 0x84E6,
    Subtract                                                       = 0x84E7,

    [GLExtension("GL_ARB_texture_env_combine")]
    SubtractARB                                                    = 0x84E7,

    [GLExtension("GL_EXT_framebuffer_object")]
    MaxRenderbufferSizeEXT                                         = 0x84E8,
    CompressedAlpha                                                = 0x84E9,

    [GLExtension("GL_ARB_texture_compression")]
    CompressedAlphaARB                                             = 0x84E9,
    CompressedLuminance                                            = 0x84EA,

    [GLExtension("GL_ARB_texture_compression")]
    CompressedLuminanceARB                                         = 0x84EA,
    CompressedLuminanceAlpha                                       = 0x84EB,

    [GLExtension("GL_ARB_texture_compression")]
    CompressedLuminanceAlphaARB                                    = 0x84EB,
    CompressedIntensity                                            = 0x84EC,

    [GLExtension("GL_ARB_texture_compression")]
    CompressedIntensityARB                                         = 0x84EC,

    [GLExtension("GL_ARB_texture_compression")]
    CompressedRgbARB                                               = 0x84ED,

    [GLExtension("GL_ARB_texture_compression")]
    CompressedRgbaARB                                              = 0x84EE,

    [GLExtension("GL_ARB_texture_rectangle")]
    MaxRectangleTextureSizeARB                                     = 0x84F8,

    [GLExtension("GL_EXT_texture_lod_bias")]
    MaxTextureLodBiasEXT                                           = 0x84FD,

    [GLExtension("GL_EXT_texture_filter_anisotropic")]
    TextureMaxAnisotropyEXT                                        = 0x84FE,
    MaxTextureMaxAnisotropy                                        = 0x84FF,

    [GLExtension("GL_EXT_texture_filter_anisotropic")]
    MaxTextureMaxAnisotropyEXT                                     = 0x84FF,

    [GLExtension("GL_EXT_texture_lod_bias")]
    TextureFilterControlEXT                                        = 0x8500,

    [GLExtension("GL_EXT_texture_lod_bias")]
    TextureLodBiasEXT                                              = 0x8501,

    [GLExtension("GL_EXT_vertex_weighting")]
    Modelview1StackDepthEXT                                        = 0x8502,

    [GLExtension("GL_EXT_vertex_weighting")]
    Modelview1MatrixEXT                                            = 0x8506,

    [GLExtension("GL_EXT_stencil_wrap")]
    IncrWrapEXT                                                    = 0x8507,

    [GLExtension("GL_EXT_stencil_wrap")]
    DecrWrapEXT                                                    = 0x8508,

    [GLExtension("GL_EXT_vertex_weighting")]
    VertexWeightingEXT                                             = 0x8509,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview1ARB                                                  = 0x850A,

    [GLExtension("GL_EXT_vertex_weighting")]
    Modelview1EXT                                                  = 0x850A,

    [GLExtension("GL_EXT_vertex_weighting")]
    CurrentVertexWeightEXT                                         = 0x850B,

    [GLExtension("GL_EXT_vertex_weighting")]
    VertexWeightArrayEXT                                           = 0x850C,

    [GLExtension("GL_EXT_vertex_weighting")]
    VertexWeightArraySizeEXT                                       = 0x850D,

    [GLExtension("GL_EXT_vertex_weighting")]
    VertexWeightArrayTypeEXT                                       = 0x850E,

    [GLExtension("GL_EXT_vertex_weighting")]
    VertexWeightArrayStrideEXT                                     = 0x850F,

    [GLExtension("GL_EXT_vertex_weighting")]
    VertexWeightArrayPointerEXT                                    = 0x8510,

    [GLExtension("GL_EXT_texture_perturb_normal")]
    TextureNormalEXT                                               = 0x85AF,

    [GLExtension("GL_ARB_vertex_program")]
    VertexAttribArrayEnabledARB                                    = 0x8622,

    [GLExtension("GL_ARB_vertex_program")]
    VertexAttribArraySizeARB                                       = 0x8623,

    [GLExtension("GL_ARB_vertex_program")]
    VertexAttribArrayStrideARB                                     = 0x8624,

    [GLExtension("GL_ARB_vertex_program")]
    VertexAttribArrayTypeARB                                       = 0x8625,

    [GLExtension("GL_ARB_vertex_program")]
    CurrentVertexAttribARB                                         = 0x8626,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramLengthARB                                               = 0x8627,

    [GLExtension("GL_ARB_fragment_program")]
    MaxProgramMatrixStackDepthARB                                  = 0x862E,

    [GLExtension("GL_ARB_fragment_program")]
    MaxProgramMatricesARB                                          = 0x862F,

    [GLExtension("GL_ARB_fragment_program")]
    CurrentMatrixStackDepthARB                                     = 0x8640,

    [GLExtension("GL_ARB_fragment_program")]
    CurrentMatrixARB                                               = 0x8641,
    VertexProgramPointSize                                         = 0x8642,

    [GLExtension("GL_ARB_vertex_program")]
    VertexProgramPointSizeARB                                      = 0x8642,

    [GLExtension("GL_ARB_geometry_shader4")]
    ProgramPointSizeARB                                            = 0x8642,

    [GLExtension("GL_EXT_geometry_shader4")]
    ProgramPointSizeEXT                                            = 0x8642,
    VertexProgramTwoSide                                           = 0x8643,

    [GLExtension("GL_ARB_vertex_program")]
    VertexProgramTwoSideARB                                        = 0x8643,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramErrorPositionARB                                        = 0x864B,

    [GLExtension("GL_EXT_depth_clamp")]
    DepthClampEXT                                                  = 0x864F,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramBindingARB                                              = 0x8677,
    TextureCompressedImageSize                                     = 0x86A0,

    [GLExtension("GL_ARB_texture_compression")]
    TextureCompressedImageSizeARB                                  = 0x86A0,

    [GLExtension("GL_ARB_texture_compression")]
    TextureCompressedARB                                           = 0x86A1,

    [GLExtension("GL_ARB_texture_compression")]
    NumCompressedTextureFormatsARB                                 = 0x86A2,

    [GLExtension("GL_ARB_texture_compression")]
    CompressedTextureFormatsARB                                    = 0x86A3,

    [GLExtension("GL_ARB_vertex_blend")]
    MaxVertexUnitsARB                                              = 0x86A4,

    [GLExtension("GL_ARB_vertex_blend")]
    ActiveVertexUnitsARB                                           = 0x86A5,

    [GLExtension("GL_ARB_vertex_blend")]
    WeightSumUnityARB                                              = 0x86A6,

    [GLExtension("GL_ARB_vertex_blend")]
    VertexBlendARB                                                 = 0x86A7,

    [GLExtension("GL_ARB_vertex_blend")]
    CurrentWeightARB                                               = 0x86A8,

    [GLExtension("GL_ARB_vertex_blend")]
    WeightArrayTypeARB                                             = 0x86A9,

    [GLExtension("GL_ARB_vertex_blend")]
    WeightArrayStrideARB                                           = 0x86AA,

    [GLExtension("GL_ARB_vertex_blend")]
    WeightArraySizeARB                                             = 0x86AB,

    [GLExtension("GL_ARB_vertex_blend")]
    WeightArrayPointerARB                                          = 0x86AC,

    [GLExtension("GL_ARB_vertex_blend")]
    WeightArrayARB                                                 = 0x86AD,
    Dot3Rgb                                                        = 0x86AE,

    [GLExtension("GL_ARB_texture_env_dot3")]
    Dot3RgbARB                                                     = 0x86AE,
    Dot3Rgba                                                       = 0x86AF,

    [GLExtension("GL_ARB_texture_env_dot3")]
    Dot3RgbaARB                                                    = 0x86AF,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview2ARB                                                  = 0x8722,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview3ARB                                                  = 0x8723,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview4ARB                                                  = 0x8724,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview5ARB                                                  = 0x8725,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview6ARB                                                  = 0x8726,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview7ARB                                                  = 0x8727,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview8ARB                                                  = 0x8728,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview9ARB                                                  = 0x8729,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview10ARB                                                 = 0x872A,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview11ARB                                                 = 0x872B,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview12ARB                                                 = 0x872C,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview13ARB                                                 = 0x872D,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview14ARB                                                 = 0x872E,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview15ARB                                                 = 0x872F,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview16ARB                                                 = 0x8730,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview17ARB                                                 = 0x8731,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview18ARB                                                 = 0x8732,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview19ARB                                                 = 0x8733,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview20ARB                                                 = 0x8734,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview21ARB                                                 = 0x8735,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview22ARB                                                 = 0x8736,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview23ARB                                                 = 0x8737,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview24ARB                                                 = 0x8738,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview25ARB                                                 = 0x8739,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview26ARB                                                 = 0x873A,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview27ARB                                                 = 0x873B,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview28ARB                                                 = 0x873C,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview29ARB                                                 = 0x873D,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview30ARB                                                 = 0x873E,

    [GLExtension("GL_ARB_vertex_blend")]
    Modelview31ARB                                                 = 0x873F,

    [GLExtension("GL_EXT_texture_env_dot3")]
    Dot3RgbEXT                                                     = 0x8740,

    [GLExtension("GL_EXT_texture_env_dot3")]
    Dot3RgbaEXT                                                    = 0x8741,

    [GLExtension("GL_EXT_texture_mirror_clamp")]
    MirrorClampEXT                                                 = 0x8742,
    MirrorClampToEdge                                              = 0x8743,

    [GLExtension("GL_EXT_texture_mirror_clamp")]
    MirrorClampToEdgeEXT                                           = 0x8743,
    UnsignedInt248MESA                                             = 0x8751,
    UnsignedInt824RevMESA                                          = 0x8752,
    UnsignedShort151MESA                                           = 0x8753,
    UnsignedShort115RevMESA                                        = 0x8754,
    TraceMaskMESA                                                  = 0x8755,
    TraceNameMESA                                                  = 0x8756,
    DebugObjectMESA                                                = 0x8759,
    DebugPrintMESA                                                 = 0x875A,
    DebugAssertMESA                                                = 0x875B,

    [GLExtension("GL_EXT_vertex_shader")]
    VertexShaderEXT                                                = 0x8780,

    [GLExtension("GL_EXT_vertex_shader")]
    VertexShaderBindingEXT                                         = 0x8781,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputVertexEXT                                                = 0x879A,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputColor0EXT                                                = 0x879B,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputColor1EXT                                                = 0x879C,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord0EXT                                         = 0x879D,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord1EXT                                         = 0x879E,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord2EXT                                         = 0x879F,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord3EXT                                         = 0x87A0,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord4EXT                                         = 0x87A1,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord5EXT                                         = 0x87A2,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord6EXT                                         = 0x87A3,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord7EXT                                         = 0x87A4,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord8EXT                                         = 0x87A5,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord9EXT                                         = 0x87A6,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord10EXT                                        = 0x87A7,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord11EXT                                        = 0x87A8,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord12EXT                                        = 0x87A9,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord13EXT                                        = 0x87AA,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord14EXT                                        = 0x87AB,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord15EXT                                        = 0x87AC,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord16EXT                                        = 0x87AD,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord17EXT                                        = 0x87AE,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord18EXT                                        = 0x87AF,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord19EXT                                        = 0x87B0,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord20EXT                                        = 0x87B1,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord21EXT                                        = 0x87B2,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord22EXT                                        = 0x87B3,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord23EXT                                        = 0x87B4,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord24EXT                                        = 0x87B5,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord25EXT                                        = 0x87B6,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord26EXT                                        = 0x87B7,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord27EXT                                        = 0x87B8,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord28EXT                                        = 0x87B9,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord29EXT                                        = 0x87BA,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord30EXT                                        = 0x87BB,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputTextureCoord31EXT                                        = 0x87BC,

    [GLExtension("GL_EXT_vertex_shader")]
    OutputFogEXT                                                   = 0x87BD,

    [GLExtension("GL_EXT_vertex_shader")]
    MaxVertexShaderInstructionsEXT                                 = 0x87C5,

    [GLExtension("GL_EXT_vertex_shader")]
    MaxVertexShaderVariantsEXT                                     = 0x87C6,

    [GLExtension("GL_EXT_vertex_shader")]
    MaxVertexShaderInvariantsEXT                                   = 0x87C7,

    [GLExtension("GL_EXT_vertex_shader")]
    MaxVertexShaderLocalConstantsEXT                               = 0x87C8,

    [GLExtension("GL_EXT_vertex_shader")]
    MaxVertexShaderLocalsEXT                                       = 0x87C9,

    [GLExtension("GL_EXT_vertex_shader")]
    MaxOptimizedVertexShaderInstructionsEXT                        = 0x87CA,

    [GLExtension("GL_EXT_vertex_shader")]
    MaxOptimizedVertexShaderVariantsEXT                            = 0x87CB,

    [GLExtension("GL_EXT_vertex_shader")]
    MaxOptimizedVertexShaderLocalConstantsEXT                      = 0x87CC,

    [GLExtension("GL_EXT_vertex_shader")]
    MaxOptimizedVertexShaderInvariantsEXT                          = 0x87CD,

    [GLExtension("GL_EXT_vertex_shader")]
    MaxOptimizedVertexShaderLocalsEXT                              = 0x87CE,

    [GLExtension("GL_EXT_vertex_shader")]
    VertexShaderInstructionsEXT                                    = 0x87CF,

    [GLExtension("GL_EXT_vertex_shader")]
    VertexShaderVariantsEXT                                        = 0x87D0,

    [GLExtension("GL_EXT_vertex_shader")]
    VertexShaderInvariantsEXT                                      = 0x87D1,

    [GLExtension("GL_EXT_vertex_shader")]
    VertexShaderLocalConstantsEXT                                  = 0x87D2,

    [GLExtension("GL_EXT_vertex_shader")]
    VertexShaderLocalsEXT                                          = 0x87D3,

    [GLExtension("GL_EXT_vertex_shader")]
    VertexShaderOptimizedEXT                                       = 0x87D4,

    [GLExtension("GL_EXT_vertex_shader")]
    VariantArrayPointerEXT                                         = 0x87E9,

    [GLExtension("GL_EXT_vertex_shader")]
    InvariantValueEXT                                              = 0x87EA,

    [GLExtension("GL_EXT_vertex_shader")]
    InvariantDatatypeEXT                                           = 0x87EB,

    [GLExtension("GL_EXT_vertex_shader")]
    LocalConstantValueEXT                                          = 0x87EC,

    [GLExtension("GL_EXT_vertex_shader")]
    LocalConstantDatatypeEXT                                       = 0x87ED,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramAluInstructionsARB                                      = 0x8805,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramTexInstructionsARB                                      = 0x8806,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramTexIndirectionsARB                                      = 0x8807,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramNativeAluInstructionsARB                                = 0x8808,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramNativeTexInstructionsARB                                = 0x8809,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramNativeTexIndirectionsARB                                = 0x880A,

    [GLExtension("GL_ARB_fragment_program")]
    MaxProgramAluInstructionsARB                                   = 0x880B,

    [GLExtension("GL_ARB_fragment_program")]
    MaxProgramTexInstructionsARB                                   = 0x880C,

    [GLExtension("GL_ARB_fragment_program")]
    MaxProgramTexIndirectionsARB                                   = 0x880D,

    [GLExtension("GL_ARB_fragment_program")]
    MaxProgramNativeAluInstructionsARB                             = 0x880E,

    [GLExtension("GL_ARB_fragment_program")]
    MaxProgramNativeTexInstructionsARB                             = 0x880F,

    [GLExtension("GL_ARB_fragment_program")]
    MaxProgramNativeTexIndirectionsARB                             = 0x8810,

    [GLExtension("GL_ARB_texture_float")]
    Alpha32fARB                                                    = 0x8816,

    [GLExtension("GL_EXT_texture_storage")]
    Alpha32fEXT                                                    = 0x8816,

    [GLExtension("GL_ARB_texture_float")]
    Intensity32fARB                                                = 0x8817,

    [GLExtension("GL_ARB_texture_float")]
    Luminance32fARB                                                = 0x8818,

    [GLExtension("GL_EXT_texture_storage")]
    Luminance32fEXT                                                = 0x8818,

    [GLExtension("GL_ARB_texture_float")]
    LuminanceAlpha32fARB                                           = 0x8819,

    [GLExtension("GL_EXT_texture_storage")]
    LuminanceAlpha32fEXT                                           = 0x8819,

    [GLExtension("GL_ARB_texture_float")]
    Alpha16fARB                                                    = 0x881C,

    [GLExtension("GL_EXT_texture_storage")]
    Alpha16fEXT                                                    = 0x881C,

    [GLExtension("GL_ARB_texture_float")]
    Intensity16fARB                                                = 0x881D,

    [GLExtension("GL_ARB_texture_float")]
    Luminance16fARB                                                = 0x881E,

    [GLExtension("GL_EXT_texture_storage")]
    Luminance16fEXT                                                = 0x881E,

    [GLExtension("GL_ARB_texture_float")]
    LuminanceAlpha16fARB                                           = 0x881F,

    [GLExtension("GL_EXT_texture_storage")]
    LuminanceAlpha16fEXT                                           = 0x881F,

    [GLExtension("GL_ARB_color_buffer_float")]
    RgbaFloatModeARB                                               = 0x8820,

    [GLExtension("GL_ARB_draw_buffers")]
    MaxDrawBuffersARB                                              = 0x8824,

    [GLExtension("GL_EXT_draw_buffers")]
    MaxDrawBuffersEXT                                              = 0x8824,
    DrawBuffer0                                                    = 0x8825,

    [GLExtension("GL_ARB_draw_buffers")]
    DrawBuffer0ARB                                                 = 0x8825,

    [GLExtension("GL_EXT_draw_buffers")]
    DrawBuffer0EXT                                                 = 0x8825,
    DrawBuffer1                                                    = 0x8826,

    [GLExtension("GL_ARB_draw_buffers")]
    DrawBuffer1ARB                                                 = 0x8826,

    [GLExtension("GL_EXT_draw_buffers")]
    DrawBuffer1EXT                                                 = 0x8826,
    DrawBuffer2                                                    = 0x8827,

    [GLExtension("GL_ARB_draw_buffers")]
    DrawBuffer2ARB                                                 = 0x8827,

    [GLExtension("GL_EXT_draw_buffers")]
    DrawBuffer2EXT                                                 = 0x8827,
    DrawBuffer3                                                    = 0x8828,

    [GLExtension("GL_ARB_draw_buffers")]
    DrawBuffer3ARB                                                 = 0x8828,

    [GLExtension("GL_EXT_draw_buffers")]
    DrawBuffer3EXT                                                 = 0x8828,
    DrawBuffer4                                                    = 0x8829,

    [GLExtension("GL_ARB_draw_buffers")]
    DrawBuffer4ARB                                                 = 0x8829,

    [GLExtension("GL_EXT_draw_buffers")]
    DrawBuffer4EXT                                                 = 0x8829,
    DrawBuffer5                                                    = 0x882A,

    [GLExtension("GL_ARB_draw_buffers")]
    DrawBuffer5ARB                                                 = 0x882A,

    [GLExtension("GL_EXT_draw_buffers")]
    DrawBuffer5EXT                                                 = 0x882A,
    DrawBuffer6                                                    = 0x882B,

    [GLExtension("GL_ARB_draw_buffers")]
    DrawBuffer6ARB                                                 = 0x882B,

    [GLExtension("GL_EXT_draw_buffers")]
    DrawBuffer6EXT                                                 = 0x882B,
    DrawBuffer7                                                    = 0x882C,

    [GLExtension("GL_ARB_draw_buffers")]
    DrawBuffer7ARB                                                 = 0x882C,

    [GLExtension("GL_EXT_draw_buffers")]
    DrawBuffer7EXT                                                 = 0x882C,
    DrawBuffer8                                                    = 0x882D,

    [GLExtension("GL_ARB_draw_buffers")]
    DrawBuffer8ARB                                                 = 0x882D,

    [GLExtension("GL_EXT_draw_buffers")]
    DrawBuffer8EXT                                                 = 0x882D,
    DrawBuffer9                                                    = 0x882E,

    [GLExtension("GL_ARB_draw_buffers")]
    DrawBuffer9ARB                                                 = 0x882E,

    [GLExtension("GL_EXT_draw_buffers")]
    DrawBuffer9EXT                                                 = 0x882E,
    DrawBuffer10                                                   = 0x882F,

    [GLExtension("GL_ARB_draw_buffers")]
    DrawBuffer10ARB                                                = 0x882F,

    [GLExtension("GL_EXT_draw_buffers")]
    DrawBuffer10EXT                                                = 0x882F,
    DrawBuffer11                                                   = 0x8830,

    [GLExtension("GL_ARB_draw_buffers")]
    DrawBuffer11ARB                                                = 0x8830,

    [GLExtension("GL_EXT_draw_buffers")]
    DrawBuffer11EXT                                                = 0x8830,
    DrawBuffer12                                                   = 0x8831,

    [GLExtension("GL_ARB_draw_buffers")]
    DrawBuffer12ARB                                                = 0x8831,

    [GLExtension("GL_EXT_draw_buffers")]
    DrawBuffer12EXT                                                = 0x8831,
    DrawBuffer13                                                   = 0x8832,

    [GLExtension("GL_ARB_draw_buffers")]
    DrawBuffer13ARB                                                = 0x8832,

    [GLExtension("GL_EXT_draw_buffers")]
    DrawBuffer13EXT                                                = 0x8832,
    DrawBuffer14                                                   = 0x8833,

    [GLExtension("GL_ARB_draw_buffers")]
    DrawBuffer14ARB                                                = 0x8833,

    [GLExtension("GL_EXT_draw_buffers")]
    DrawBuffer14EXT                                                = 0x8833,
    DrawBuffer15                                                   = 0x8834,

    [GLExtension("GL_ARB_draw_buffers")]
    DrawBuffer15ARB                                                = 0x8834,

    [GLExtension("GL_EXT_draw_buffers")]
    DrawBuffer15EXT                                                = 0x8834,
    CompressedLuminanceAlpha3dcATI                                 = 0x8837,

    [GLExtension("GL_EXT_blend_equation_separate")]
    BlendEquationAlphaEXT                                          = 0x883D,

    [GLExtension("GL_ARB_matrix_palette")]
    MatrixPaletteARB                                               = 0x8840,

    [GLExtension("GL_ARB_matrix_palette")]
    MaxMatrixPaletteStackDepthARB                                  = 0x8841,

    [GLExtension("GL_ARB_matrix_palette")]
    MaxPaletteMatricesARB                                          = 0x8842,

    [GLExtension("GL_ARB_matrix_palette")]
    CurrentPaletteMatrixARB                                        = 0x8843,

    [GLExtension("GL_ARB_matrix_palette")]
    MatrixIndexArrayARB                                            = 0x8844,

    [GLExtension("GL_ARB_matrix_palette")]
    CurrentMatrixIndexARB                                          = 0x8845,

    [GLExtension("GL_ARB_matrix_palette")]
    MatrixIndexArraySizeARB                                        = 0x8846,

    [GLExtension("GL_ARB_matrix_palette")]
    MatrixIndexArrayTypeARB                                        = 0x8847,

    [GLExtension("GL_ARB_matrix_palette")]
    MatrixIndexArrayStrideARB                                      = 0x8848,

    [GLExtension("GL_ARB_matrix_palette")]
    MatrixIndexArrayPointerARB                                     = 0x8849,
    TextureDepthSize                                               = 0x884A,

    [GLExtension("GL_ARB_depth_texture")]
    TextureDepthSizeARB                                            = 0x884A,
    DepthTextureMode                                               = 0x884B,

    [GLExtension("GL_ARB_depth_texture")]
    DepthTextureModeARB                                            = 0x884B,

    [GLExtension("GL_ARB_shadow")]
    TextureCompareModeARB                                          = 0x884C,

    [GLExtension("GL_EXT_shadow_samplers")]
    TextureCompareModeEXT                                          = 0x884C,

    [GLExtension("GL_ARB_shadow")]
    TextureCompareFuncARB                                          = 0x884D,

    [GLExtension("GL_EXT_shadow_samplers")]
    TextureCompareFuncEXT                                          = 0x884D,

    [GLExtension("GL_ARB_shadow")]
    CompareRToTextureARB                                           = 0x884E,

    [GLExtension("GL_EXT_texture_array")]
    CompareRefDepthToTextureEXT                                    = 0x884E,

    [GLExtension("GL_EXT_shadow_samplers")]
    CompareRefToTextureEXT                                         = 0x884E,

    [GLExtension("GL_ARB_point_sprite")]
    PointSpriteARB                                                 = 0x8861,

    [GLExtension("GL_ARB_point_sprite")]
    CoordReplaceARB                                                = 0x8862,

    [GLExtension("GL_ARB_occlusion_query")]
    QueryCounterBitsARB                                            = 0x8864,

    [GLExtension("GL_EXT_disjoint_timer_query")]
    QueryCounterBitsEXT                                            = 0x8864,

    [GLExtension("GL_ARB_occlusion_query")]
    CurrentQueryARB                                                = 0x8865,

    [GLExtension("GL_EXT_disjoint_timer_query")]
    CurrentQueryEXT                                                = 0x8865,

    [GLExtension("GL_ARB_occlusion_query")]
    QueryResultARB                                                 = 0x8866,

    [GLExtension("GL_EXT_disjoint_timer_query")]
    QueryResultEXT                                                 = 0x8866,

    [GLExtension("GL_ARB_occlusion_query")]
    QueryResultAvailableARB                                        = 0x8867,

    [GLExtension("GL_EXT_disjoint_timer_query")]
    QueryResultAvailableEXT                                        = 0x8867,

    [GLExtension("GL_ARB_vertex_program")]
    MaxVertexAttribsARB                                            = 0x8869,

    [GLExtension("GL_ARB_vertex_program")]
    VertexAttribArrayNormalizedARB                                 = 0x886A,
    MaxTessControlInputComponents                                  = 0x886C,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessControlInputComponentsEXT                               = 0x886C,
    MaxTessEvaluationInputComponents                               = 0x886D,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessEvaluationInputComponentsEXT                            = 0x886D,
    MaxTextureCoords                                               = 0x8871,

    [GLExtension("GL_ARB_fragment_program")]
    MaxTextureCoordsARB                                            = 0x8871,

    [GLExtension("GL_ARB_fragment_program")]
    MaxTextureImageUnitsARB                                        = 0x8872,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramErrorStringARB                                          = 0x8874,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramFormatARB                                               = 0x8876,
    GeometryShaderInvocations                                      = 0x887F,

    [GLExtension("GL_EXT_geometry_shader")]
    GeometryShaderInvocationsEXT                                   = 0x887F,

    [GLExtension("GL_EXT_depth_bounds_test")]
    DepthBoundsTestEXT                                             = 0x8890,

    [GLExtension("GL_EXT_depth_bounds_test")]
    DepthBoundsEXT                                                 = 0x8891,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    ArrayBufferARB                                                 = 0x8892,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    ElementArrayBufferARB                                          = 0x8893,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    ArrayBufferBindingARB                                          = 0x8894,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    ElementArrayBufferBindingARB                                   = 0x8895,
    VertexArrayBufferBinding                                       = 0x8896,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    VertexArrayBufferBindingARB                                    = 0x8896,
    NormalArrayBufferBinding                                       = 0x8897,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    NormalArrayBufferBindingARB                                    = 0x8897,
    ColorArrayBufferBinding                                        = 0x8898,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    ColorArrayBufferBindingARB                                     = 0x8898,
    IndexArrayBufferBinding                                        = 0x8899,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    IndexArrayBufferBindingARB                                     = 0x8899,
    TextureCoordArrayBufferBinding                                 = 0x889A,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    TextureCoordArrayBufferBindingARB                              = 0x889A,
    EdgeFlagArrayBufferBinding                                     = 0x889B,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    EdgeFlagArrayBufferBindingARB                                  = 0x889B,
    SecondaryColorArrayBufferBinding                               = 0x889C,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    SecondaryColorArrayBufferBindingARB                            = 0x889C,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    FogCoordinateArrayBufferBindingARB                             = 0x889D,
    FogCoordinateArrayBufferBinding                                = 0x889D,
    FogCoordArrayBufferBinding                                     = 0x889D,
    WeightArrayBufferBinding                                       = 0x889E,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    WeightArrayBufferBindingARB                                    = 0x889E,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    VertexAttribArrayBufferBindingARB                              = 0x889F,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramInstructionsARB                                         = 0x88A0,

    [GLExtension("GL_ARB_fragment_program")]
    MaxProgramInstructionsARB                                      = 0x88A1,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramNativeInstructionsARB                                   = 0x88A2,

    [GLExtension("GL_ARB_fragment_program")]
    MaxProgramNativeInstructionsARB                                = 0x88A3,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramTemporariesARB                                          = 0x88A4,

    [GLExtension("GL_ARB_fragment_program")]
    MaxProgramTemporariesARB                                       = 0x88A5,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramNativeTemporariesARB                                    = 0x88A6,

    [GLExtension("GL_ARB_fragment_program")]
    MaxProgramNativeTemporariesARB                                 = 0x88A7,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramParametersARB                                           = 0x88A8,

    [GLExtension("GL_ARB_fragment_program")]
    MaxProgramParametersARB                                        = 0x88A9,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramNativeParametersARB                                     = 0x88AA,

    [GLExtension("GL_ARB_fragment_program")]
    MaxProgramNativeParametersARB                                  = 0x88AB,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramAttribsARB                                              = 0x88AC,

    [GLExtension("GL_ARB_fragment_program")]
    MaxProgramAttribsARB                                           = 0x88AD,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramNativeAttribsARB                                        = 0x88AE,

    [GLExtension("GL_ARB_fragment_program")]
    MaxProgramNativeAttribsARB                                     = 0x88AF,

    [GLExtension("GL_ARB_vertex_program")]
    ProgramAddressRegistersARB                                     = 0x88B0,

    [GLExtension("GL_ARB_vertex_program")]
    MaxProgramAddressRegistersARB                                  = 0x88B1,

    [GLExtension("GL_ARB_vertex_program")]
    ProgramNativeAddressRegistersARB                               = 0x88B2,

    [GLExtension("GL_ARB_vertex_program")]
    MaxProgramNativeAddressRegistersARB                            = 0x88B3,

    [GLExtension("GL_ARB_fragment_program")]
    MaxProgramLocalParametersARB                                   = 0x88B4,

    [GLExtension("GL_ARB_fragment_program")]
    MaxProgramEnvParametersARB                                     = 0x88B5,

    [GLExtension("GL_ARB_fragment_program")]
    ProgramUnderNativeLimitsARB                                    = 0x88B6,

    [GLExtension("GL_ARB_fragment_program")]
    TransposeCurrentMatrixARB                                      = 0x88B7,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    ReadOnlyARB                                                    = 0x88B8,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    WriteOnlyARB                                                   = 0x88B9,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    ReadWriteARB                                                   = 0x88BA,

    [GLExtension("GL_EXT_disjoint_timer_query")]
    TimeElapsedEXT                                                 = 0x88BF,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix0ARB                                                     = 0x88C0,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix1ARB                                                     = 0x88C1,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix2ARB                                                     = 0x88C2,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix3ARB                                                     = 0x88C3,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix4ARB                                                     = 0x88C4,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix5ARB                                                     = 0x88C5,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix6ARB                                                     = 0x88C6,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix7ARB                                                     = 0x88C7,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix8ARB                                                     = 0x88C8,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix9ARB                                                     = 0x88C9,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix10ARB                                                    = 0x88CA,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix11ARB                                                    = 0x88CB,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix12ARB                                                    = 0x88CC,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix13ARB                                                    = 0x88CD,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix14ARB                                                    = 0x88CE,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix15ARB                                                    = 0x88CF,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix16ARB                                                    = 0x88D0,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix17ARB                                                    = 0x88D1,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix18ARB                                                    = 0x88D2,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix19ARB                                                    = 0x88D3,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix20ARB                                                    = 0x88D4,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix21ARB                                                    = 0x88D5,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix22ARB                                                    = 0x88D6,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix23ARB                                                    = 0x88D7,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix24ARB                                                    = 0x88D8,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix25ARB                                                    = 0x88D9,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix26ARB                                                    = 0x88DA,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix27ARB                                                    = 0x88DB,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix28ARB                                                    = 0x88DC,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix29ARB                                                    = 0x88DD,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix30ARB                                                    = 0x88DE,

    [GLExtension("GL_ARB_fragment_program")]
    Matrix31ARB                                                    = 0x88DF,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    StreamDrawARB                                                  = 0x88E0,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    StreamReadARB                                                  = 0x88E1,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    StreamCopyARB                                                  = 0x88E2,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    StaticDrawARB                                                  = 0x88E4,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    StaticReadARB                                                  = 0x88E5,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    StaticCopyARB                                                  = 0x88E6,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    DynamicDrawARB                                                 = 0x88E8,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    DynamicReadARB                                                 = 0x88E9,

    [GLExtension("GL_ARB_vertex_buffer_object")]
    DynamicCopyARB                                                 = 0x88EA,

    [GLExtension("GL_ARB_pixel_buffer_object")]
    PixelPackBufferARB                                             = 0x88EB,

    [GLExtension("GL_EXT_pixel_buffer_object")]
    PixelPackBufferEXT                                             = 0x88EB,

    [GLExtension("GL_ARB_pixel_buffer_object")]
    PixelUnpackBufferARB                                           = 0x88EC,

    [GLExtension("GL_EXT_pixel_buffer_object")]
    PixelUnpackBufferEXT                                           = 0x88EC,

    [GLExtension("GL_ARB_pixel_buffer_object")]
    PixelPackBufferBindingARB                                      = 0x88ED,

    [GLExtension("GL_EXT_pixel_buffer_object")]
    PixelPackBufferBindingEXT                                      = 0x88ED,

    [GLExtension("GL_ARB_pixel_buffer_object")]
    PixelUnpackBufferBindingARB                                    = 0x88EF,

    [GLExtension("GL_EXT_pixel_buffer_object")]
    PixelUnpackBufferBindingEXT                                    = 0x88EF,
    TextureStencilSize                                             = 0x88F1,

    [GLExtension("GL_EXT_packed_depth_stencil")]
    TextureStencilSizeEXT                                          = 0x88F1,

    [GLExtension("GL_EXT_stencil_clear_tag")]
    StencilTagBitsEXT                                              = 0x88F2,

    [GLExtension("GL_EXT_stencil_clear_tag")]
    StencilClearTagValueEXT                                        = 0x88F3,

    [GLExtension("GL_EXT_blend_func_extended")]
    Src1ColorEXT                                                   = 0x88F9,

    [GLExtension("GL_EXT_blend_func_extended")]
    OneMinusSrc1ColorEXT                                           = 0x88FA,

    [GLExtension("GL_EXT_blend_func_extended")]
    OneMinusSrc1AlphaEXT                                           = 0x88FB,

    [GLExtension("GL_EXT_blend_func_extended")]
    MaxDualSourceDrawBuffersEXT                                    = 0x88FC,

    [GLExtension("GL_ARB_instanced_arrays")]
    VertexAttribArrayDivisorARB                                    = 0x88FE,

    [GLExtension("GL_EXT_instanced_arrays")]
    VertexAttribArrayDivisorEXT                                    = 0x88FE,

    [GLExtension("GL_EXT_texture_array")]
    MaxArrayTextureLayersEXT                                       = 0x88FF,

    [GLExtension("GL_EXT_gpu_shader4")]
    MinProgramTexelOffsetEXT                                       = 0x8904,

    [GLExtension("GL_EXT_gpu_shader4")]
    MaxProgramTexelOffsetEXT                                       = 0x8905,

    [GLExtension("GL_EXT_stencil_two_side")]
    StencilTestTwoSideEXT                                          = 0x8910,

    [GLExtension("GL_EXT_stencil_two_side")]
    ActiveStencilFaceEXT                                           = 0x8911,

    [GLExtension("GL_EXT_texture_mirror_clamp")]
    MirrorClampToBorderEXT                                         = 0x8912,

    [GLExtension("GL_ARB_occlusion_query")]
    SamplesPassedARB                                               = 0x8914,

    [GLExtension("GL_EXT_geometry_shader")]
    GeometryLinkedVerticesOutEXT                                   = 0x8916,

    [GLExtension("GL_EXT_geometry_shader")]
    GeometryLinkedInputTypeEXT                                     = 0x8917,

    [GLExtension("GL_EXT_geometry_shader")]
    GeometryLinkedOutputTypeEXT                                    = 0x8918,
    ClampVertexColor                                               = 0x891A,
    ClampFragmentColor                                             = 0x891B,

    [GLExtension("GL_EXT_geometry_shader")]
    MaxGeometryUniformBlocksEXT                                    = 0x8A2C,

    [GLExtension("GL_EXT_geometry_shader")]
    MaxCombinedGeometryUniformComponentsEXT                        = 0x8A32,

    [GLExtension("GL_EXT_texture_sRGB_decode")]
    TextureSrgbDecodeEXT                                           = 0x8A48,

    [GLExtension("GL_EXT_texture_sRGB_decode")]
    DecodeEXT                                                      = 0x8A49,

    [GLExtension("GL_EXT_texture_sRGB_decode")]
    SkipDecodeEXT                                                  = 0x8A4A,

    [GLExtension("GL_EXT_debug_label")]
    ProgramPipelineObjectEXT                                       = 0x8A4F,

    [GLExtension("GL_EXT_shader_framebuffer_fetch")]
    FragmentShaderDiscardsSamplesEXT                               = 0x8A52,

    [GLExtension("GL_EXT_pvrtc_sRGB")]
    CompressedSrgbPvrtc2bppv1EXT                                   = 0x8A54,

    [GLExtension("GL_EXT_pvrtc_sRGB")]
    CompressedSrgbPvrtc4bppv1EXT                                   = 0x8A55,

    [GLExtension("GL_EXT_pvrtc_sRGB")]
    CompressedSrgbAlphaPvrtc2bppv1EXT                              = 0x8A56,

    [GLExtension("GL_EXT_pvrtc_sRGB")]
    CompressedSrgbAlphaPvrtc4bppv1EXT                              = 0x8A57,

    [GLExtension("GL_ARB_shader_objects")]
    ShaderObjectARB                                                = 0x8B48,

    [GLExtension("GL_EXT_debug_label")]
    ShaderObjectEXT                                                = 0x8B48,

    [GLExtension("GL_ARB_fragment_shader")]
    MaxFragmentUniformComponentsARB                                = 0x8B49,

    [GLExtension("GL_ARB_vertex_shader")]
    MaxVertexUniformComponentsARB                                  = 0x8B4A,

    [GLExtension("GL_EXT_geometry_shader4")]
    MaxVaryingComponentsEXT                                        = 0x8B4B,

    [GLExtension("GL_ARB_vertex_shader")]
    MaxVaryingFloatsARB                                            = 0x8B4B,

    [GLExtension("GL_ARB_vertex_shader")]
    MaxVertexTextureImageUnitsARB                                  = 0x8B4C,

    [GLExtension("GL_ARB_vertex_shader")]
    MaxCombinedTextureImageUnitsARB                                = 0x8B4D,

    [GLExtension("GL_ARB_shader_objects")]
    ObjectTypeARB                                                  = 0x8B4E,

    [GLExtension("GL_ARB_shader_objects")]
    ObjectSubtypeARB                                               = 0x8B4F,

    [GLExtension("GL_ARB_shader_objects")]
    ObjectDeleteStatusARB                                          = 0x8B80,

    [GLExtension("GL_ARB_shader_objects")]
    ObjectCompileStatusARB                                         = 0x8B81,

    [GLExtension("GL_ARB_shader_objects")]
    ObjectLinkStatusARB                                            = 0x8B82,

    [GLExtension("GL_ARB_shader_objects")]
    ObjectValidateStatusARB                                        = 0x8B83,

    [GLExtension("GL_ARB_shader_objects")]
    ObjectInfoLogLengthARB                                         = 0x8B84,

    [GLExtension("GL_ARB_shader_objects")]
    ObjectAttachedObjectsARB                                       = 0x8B85,

    [GLExtension("GL_ARB_shader_objects")]
    ObjectActiveUniformsARB                                        = 0x8B86,

    [GLExtension("GL_ARB_shader_objects")]
    ObjectActiveUniformMaxLengthARB                                = 0x8B87,

    [GLExtension("GL_ARB_shader_objects")]
    ObjectShaderSourceLengthARB                                    = 0x8B88,

    [GLExtension("GL_ARB_vertex_shader")]
    ObjectActiveAttributesARB                                      = 0x8B89,

    [GLExtension("GL_ARB_vertex_shader")]
    ObjectActiveAttributeMaxLengthARB                              = 0x8B8A,

    [GLExtension("GL_ARB_shading_language_100")]
    ShadingLanguageVersionARB                                      = 0x8B8C,
    FragmentProgramPositionMESA                                    = 0x8BB0,
    FragmentProgramCallbackMESA                                    = 0x8BB1,
    FragmentProgramCallbackFuncMESA                                = 0x8BB2,
    FragmentProgramCallbackDataMESA                                = 0x8BB3,
    VertexProgramPositionMESA                                      = 0x8BB4,
    VertexProgramCallbackMESA                                      = 0x8BB5,
    VertexProgramCallbackFuncMESA                                  = 0x8BB6,
    VertexProgramCallbackDataMESA                                  = 0x8BB7,

    [GLExtension("GL_EXT_YUV_target")]
    SamplerExternal2DY2yEXT                                        = 0x8BE7,

    [GLExtension("GL_EXT_protected_textures")]
    TextureProtectedEXT                                            = 0x8BFA,
    TextureRedType                                                 = 0x8C10,

    [GLExtension("GL_ARB_texture_float")]
    TextureRedTypeARB                                              = 0x8C10,
    TextureGreenType                                               = 0x8C11,

    [GLExtension("GL_ARB_texture_float")]
    TextureGreenTypeARB                                            = 0x8C11,
    TextureBlueType                                                = 0x8C12,

    [GLExtension("GL_ARB_texture_float")]
    TextureBlueTypeARB                                             = 0x8C12,
    TextureAlphaType                                               = 0x8C13,

    [GLExtension("GL_ARB_texture_float")]
    TextureAlphaTypeARB                                            = 0x8C13,
    TextureLuminanceType                                           = 0x8C14,

    [GLExtension("GL_ARB_texture_float")]
    TextureLuminanceTypeARB                                        = 0x8C14,
    TextureIntensityType                                           = 0x8C15,

    [GLExtension("GL_ARB_texture_float")]
    TextureIntensityTypeARB                                        = 0x8C15,
    TextureDepthType                                               = 0x8C16,

    [GLExtension("GL_ARB_texture_float")]
    TextureDepthTypeARB                                            = 0x8C16,
    UnsignedNormalized                                             = 0x8C17,

    [GLExtension("GL_ARB_texture_float")]
    UnsignedNormalizedARB                                          = 0x8C17,

    [GLExtension("GL_EXT_color_buffer_half_float")]
    UnsignedNormalizedEXT                                          = 0x8C17,

    [GLExtension("GL_EXT_texture_array")]
    Texture1DArrayEXT                                              = 0x8C18,

    [GLExtension("GL_EXT_texture_array")]
    Texture2DArrayEXT                                              = 0x8C1A,

    [GLExtension("GL_EXT_texture_array")]
    TextureBinding1DArrayEXT                                       = 0x8C1C,

    [GLExtension("GL_EXT_texture_array")]
    TextureBinding2DArrayEXT                                       = 0x8C1D,

    [GLExtension("GL_ARB_geometry_shader4")]
    MaxGeometryTextureImageUnitsARB                                = 0x8C29,

    [GLExtension("GL_EXT_geometry_shader")]
    MaxGeometryTextureImageUnitsEXT                                = 0x8C29,

    [GLExtension("GL_ARB_texture_buffer_object")]
    TextureBufferARB                                               = 0x8C2A,

    [GLExtension("GL_EXT_texture_buffer")]
    TextureBufferEXT                                               = 0x8C2A,
    TextureBufferBinding                                           = 0x8C2A,

    [GLExtension("GL_EXT_texture_buffer")]
    TextureBufferBindingEXT                                        = 0x8C2A,

    [GLExtension("GL_ARB_texture_buffer_object")]
    MaxTextureBufferSizeARB                                        = 0x8C2B,

    [GLExtension("GL_EXT_texture_buffer")]
    MaxTextureBufferSizeEXT                                        = 0x8C2B,

    [GLExtension("GL_ARB_texture_buffer_object")]
    TextureBindingBufferARB                                        = 0x8C2C,

    [GLExtension("GL_EXT_texture_buffer")]
    TextureBindingBufferEXT                                        = 0x8C2C,
    TextureBufferDataStoreBinding                                  = 0x8C2D,

    [GLExtension("GL_ARB_texture_buffer_object")]
    TextureBufferDataStoreBindingARB                               = 0x8C2D,

    [GLExtension("GL_EXT_texture_buffer")]
    TextureBufferDataStoreBindingEXT                               = 0x8C2D,

    [GLExtension("GL_ARB_texture_buffer_object")]
    TextureBufferFormatARB                                         = 0x8C2E,

    [GLExtension("GL_EXT_texture_buffer_object")]
    TextureBufferFormatEXT                                         = 0x8C2E,

    [GLExtension("GL_EXT_occlusion_query_boolean")]
    AnySamplesPassedEXT                                            = 0x8C2F,

    [GLExtension("GL_ARB_sample_shading")]
    SampleShadingARB                                               = 0x8C36,
    MinSampleShadingValue                                          = 0x8C37,

    [GLExtension("GL_ARB_sample_shading")]
    MinSampleShadingValueARB                                       = 0x8C37,

    [GLExtension("GL_EXT_packed_float")]
    RgbaSignedComponentsEXT                                        = 0x8C3C,
    TextureSharedSize                                              = 0x8C3F,

    [GLExtension("GL_EXT_texture_shared_exponent")]
    TextureSharedSizeEXT                                           = 0x8C3F,
    SluminanceAlpha                                                = 0x8C44,

    [GLExtension("GL_EXT_texture_sRGB")]
    SluminanceAlphaEXT                                             = 0x8C44,
    Sluminance8Alpha8                                              = 0x8C45,

    [GLExtension("GL_EXT_texture_sRGB")]
    Sluminance8Alpha8EXT                                           = 0x8C45,
    Sluminance                                                     = 0x8C46,

    [GLExtension("GL_EXT_texture_sRGB")]
    SluminanceEXT                                                  = 0x8C46,
    Sluminance8                                                    = 0x8C47,

    [GLExtension("GL_EXT_texture_sRGB")]
    Sluminance8EXT                                                 = 0x8C47,

    [GLExtension("GL_EXT_texture_sRGB")]
    CompressedSrgbEXT                                              = 0x8C48,

    [GLExtension("GL_EXT_texture_sRGB")]
    CompressedSrgbAlphaEXT                                         = 0x8C49,
    CompressedSluminance                                           = 0x8C4A,

    [GLExtension("GL_EXT_texture_sRGB")]
    CompressedSluminanceEXT                                        = 0x8C4A,
    CompressedSluminanceAlpha                                      = 0x8C4B,

    [GLExtension("GL_EXT_texture_sRGB")]
    CompressedSluminanceAlphaEXT                                   = 0x8C4B,

    [GLExtension("GL_EXT_texture_compression_latc")]
    CompressedLuminanceLatc1EXT                                    = 0x8C70,

    [GLExtension("GL_EXT_texture_compression_latc")]
    CompressedSignedLuminanceLatc1EXT                              = 0x8C71,

    [GLExtension("GL_EXT_texture_compression_latc")]
    CompressedLuminanceAlphaLatc2EXT                               = 0x8C72,

    [GLExtension("GL_EXT_texture_compression_latc")]
    CompressedSignedLuminanceAlphaLatc2EXT                         = 0x8C73,

    [GLExtension("GL_EXT_transform_feedback")]
    TransformFeedbackVaryingMaxLengthEXT                           = 0x8C76,

    [GLExtension("GL_EXT_transform_feedback")]
    TransformFeedbackBufferModeEXT                                 = 0x8C7F,
    MaxTransformFeedbackSeparateComponents                         = 0x8C80,

    [GLExtension("GL_EXT_transform_feedback")]
    MaxTransformFeedbackSeparateComponentsEXT                      = 0x8C80,

    [GLExtension("GL_EXT_transform_feedback")]
    TransformFeedbackVaryingsEXT                                   = 0x8C83,

    [GLExtension("GL_EXT_transform_feedback")]
    TransformFeedbackBufferStartEXT                                = 0x8C84,

    [GLExtension("GL_EXT_transform_feedback")]
    TransformFeedbackBufferSizeEXT                                 = 0x8C85,

    [GLExtension("GL_EXT_geometry_shader")]
    PrimitivesGeneratedEXT                                         = 0x8C87,

    [GLExtension("GL_EXT_transform_feedback")]
    TransformFeedbackPrimitivesWrittenEXT                          = 0x8C88,

    [GLExtension("GL_EXT_transform_feedback")]
    RasterizerDiscardEXT                                           = 0x8C89,
    MaxTransformFeedbackInterleavedComponents                      = 0x8C8A,

    [GLExtension("GL_EXT_transform_feedback")]
    MaxTransformFeedbackInterleavedComponentsEXT                   = 0x8C8A,
    MaxTransformFeedbackSeparateAttribs                            = 0x8C8B,

    [GLExtension("GL_EXT_transform_feedback")]
    MaxTransformFeedbackSeparateAttribsEXT                         = 0x8C8B,

    [GLExtension("GL_EXT_transform_feedback")]
    InterleavedAttribsEXT                                          = 0x8C8C,

    [GLExtension("GL_EXT_transform_feedback")]
    SeparateAttribsEXT                                             = 0x8C8D,

    [GLExtension("GL_EXT_transform_feedback")]
    TransformFeedbackBufferEXT                                     = 0x8C8E,

    [GLExtension("GL_EXT_transform_feedback")]
    TransformFeedbackBufferBindingEXT                              = 0x8C8F,
    PointSpriteCoordOrigin                                         = 0x8CA0,

    [GLExtension("GL_EXT_clip_control")]
    LowerLeftEXT                                                   = 0x8CA1,

    [GLExtension("GL_EXT_clip_control")]
    UpperLeftEXT                                                   = 0x8CA2,

    [GLExtension("GL_EXT_framebuffer_blit")]
    DrawFramebufferBindingEXT                                      = 0x8CA6,
    FramebufferBinding                                             = 0x8CA6,
    FramebufferBindingAngle                                        = 0x8CA6,

    [GLExtension("GL_EXT_framebuffer_object")]
    FramebufferBindingEXT                                          = 0x8CA6,
    RenderbufferBindingAngle                                       = 0x8CA7,

    [GLExtension("GL_EXT_framebuffer_object")]
    RenderbufferBindingEXT                                         = 0x8CA7,

    [GLExtension("GL_EXT_framebuffer_blit")]
    ReadFramebufferEXT                                             = 0x8CA8,

    [GLExtension("GL_EXT_framebuffer_blit")]
    DrawFramebufferEXT                                             = 0x8CA9,

    [GLExtension("GL_EXT_framebuffer_blit")]
    ReadFramebufferBindingEXT                                      = 0x8CAA,

    [GLExtension("GL_EXT_framebuffer_object")]
    FramebufferCompleteEXT                                         = 0x8CD5,

    [GLExtension("GL_EXT_framebuffer_object")]
    FramebufferIncompleteAttachmentEXT                             = 0x8CD6,

    [GLExtension("GL_EXT_framebuffer_object")]
    FramebufferIncompleteMissingAttachmentEXT                      = 0x8CD7,
    FramebufferIncompleteDimensions                                = 0x8CD9,

    [GLExtension("GL_EXT_framebuffer_object")]
    FramebufferIncompleteDimensionsEXT                             = 0x8CD9,

    [GLExtension("GL_EXT_framebuffer_object")]
    FramebufferIncompleteFormatsEXT                                = 0x8CDA,

    [GLExtension("GL_EXT_framebuffer_object")]
    FramebufferIncompleteDrawBufferEXT                             = 0x8CDB,
    FramebufferIncompleteDrawBufferOES                             = 0x8CDB,

    [GLExtension("GL_EXT_framebuffer_object")]
    FramebufferIncompleteReadBufferEXT                             = 0x8CDC,
    FramebufferIncompleteReadBufferOES                             = 0x8CDC,

    [GLExtension("GL_EXT_framebuffer_object")]
    FramebufferUnsupportedEXT                                      = 0x8CDD,

    [GLExtension("GL_EXT_framebuffer_object")]
    FramebufferEXT                                                 = 0x8D40,

    [GLExtension("GL_EXT_framebuffer_object")]
    RenderbufferEXT                                                = 0x8D41,

    [GLExtension("GL_EXT_framebuffer_multisample")]
    FramebufferIncompleteMultisampleEXT                            = 0x8D56,
    MaxSamples                                                     = 0x8D57,

    [GLExtension("GL_EXT_framebuffer_multisample")]
    MaxSamplesEXT                                                  = 0x8D57,

    [GLExtension("GL_EXT_YUV_target")]
    TextureExternalOES                                             = 0x8D65,

    [GLExtension("GL_EXT_YUV_target")]
    TextureBindingExternalOES                                      = 0x8D67,

    [GLExtension("GL_EXT_YUV_target")]
    RequiredTextureImageUnitsOES                                   = 0x8D68,

    [GLExtension("GL_EXT_occlusion_query_boolean")]
    AnySamplesPassedConservativeEXT                                = 0x8D6A,

    [GLExtension("GL_EXT_texture_integer")]
    RedIntegerEXT                                                  = 0x8D94,

    [GLExtension("GL_EXT_texture_integer")]
    GreenIntegerEXT                                                = 0x8D95,

    [GLExtension("GL_EXT_texture_integer")]
    BlueIntegerEXT                                                 = 0x8D96,
    AlphaInteger                                                   = 0x8D97,

    [GLExtension("GL_EXT_texture_integer")]
    AlphaIntegerEXT                                                = 0x8D97,

    [GLExtension("GL_EXT_texture_integer")]
    RgbIntegerEXT                                                  = 0x8D98,

    [GLExtension("GL_EXT_texture_integer")]
    RgbaIntegerEXT                                                 = 0x8D99,

    [GLExtension("GL_EXT_texture_integer")]
    BgrIntegerEXT                                                  = 0x8D9A,

    [GLExtension("GL_EXT_texture_integer")]
    BgraIntegerEXT                                                 = 0x8D9B,

    [GLExtension("GL_EXT_texture_integer")]
    LuminanceIntegerEXT                                            = 0x8D9C,

    [GLExtension("GL_EXT_texture_integer")]
    LuminanceAlphaIntegerEXT                                       = 0x8D9D,

    [GLExtension("GL_EXT_texture_integer")]
    RgbaIntegerModeEXT                                             = 0x8D9E,

    [GLExtension("GL_ARB_geometry_shader4")]
    FramebufferIncompleteLayerTargetsARB                           = 0x8DA8,

    [GLExtension("GL_EXT_geometry_shader")]
    FramebufferIncompleteLayerTargetsEXT                           = 0x8DA8,

    [GLExtension("GL_ARB_geometry_shader4")]
    FramebufferIncompleteLayerCountARB                             = 0x8DA9,

    [GLExtension("GL_EXT_geometry_shader4")]
    FramebufferIncompleteLayerCountEXT                             = 0x8DA9,

    [GLExtension("GL_ARB_shading_language_include")]
    ShaderIncludeARB                                               = 0x8DAE,

    [GLExtension("GL_EXT_framebuffer_sRGB")]
    FramebufferSrgbEXT                                             = 0x8DB9,

    [GLExtension("GL_EXT_framebuffer_sRGB")]
    FramebufferSrgbCapableEXT                                      = 0x8DBA,

    [GLExtension("GL_EXT_gpu_shader4")]
    Sampler1DArrayEXT                                              = 0x8DC0,

    [GLExtension("GL_EXT_gpu_shader4")]
    Sampler2DArrayEXT                                              = 0x8DC1,

    [GLExtension("GL_EXT_gpu_shader4")]
    SamplerBufferEXT                                               = 0x8DC2,

    [GLExtension("GL_EXT_gpu_shader4")]
    Sampler1DArrayShadowEXT                                        = 0x8DC3,

    [GLExtension("GL_EXT_gpu_shader4")]
    Sampler2DArrayShadowEXT                                        = 0x8DC4,

    [GLExtension("GL_EXT_gpu_shader4")]
    SamplerCubeShadowEXT                                           = 0x8DC5,

    [GLExtension("GL_EXT_gpu_shader4")]
    UnsignedIntVec2EXT                                             = 0x8DC6,

    [GLExtension("GL_EXT_gpu_shader4")]
    UnsignedIntVec3EXT                                             = 0x8DC7,

    [GLExtension("GL_EXT_gpu_shader4")]
    UnsignedIntVec4EXT                                             = 0x8DC8,

    [GLExtension("GL_EXT_gpu_shader4")]
    IntSampler1DEXT                                                = 0x8DC9,

    [GLExtension("GL_EXT_gpu_shader4")]
    IntSampler2DEXT                                                = 0x8DCA,

    [GLExtension("GL_EXT_gpu_shader4")]
    IntSampler3DEXT                                                = 0x8DCB,

    [GLExtension("GL_EXT_gpu_shader4")]
    IntSamplerCubeEXT                                              = 0x8DCC,

    [GLExtension("GL_EXT_gpu_shader4")]
    IntSampler2DRectEXT                                            = 0x8DCD,

    [GLExtension("GL_EXT_gpu_shader4")]
    IntSampler1DArrayEXT                                           = 0x8DCE,

    [GLExtension("GL_EXT_gpu_shader4")]
    IntSampler2DArrayEXT                                           = 0x8DCF,

    [GLExtension("GL_EXT_gpu_shader4")]
    IntSamplerBufferEXT                                            = 0x8DD0,

    [GLExtension("GL_EXT_gpu_shader4")]
    UnsignedIntSampler1DEXT                                        = 0x8DD1,

    [GLExtension("GL_EXT_gpu_shader4")]
    UnsignedIntSampler2DEXT                                        = 0x8DD2,

    [GLExtension("GL_EXT_gpu_shader4")]
    UnsignedIntSampler3DEXT                                        = 0x8DD3,

    [GLExtension("GL_EXT_gpu_shader4")]
    UnsignedIntSamplerCubeEXT                                      = 0x8DD4,

    [GLExtension("GL_EXT_gpu_shader4")]
    UnsignedIntSampler2DRectEXT                                    = 0x8DD5,

    [GLExtension("GL_EXT_gpu_shader4")]
    UnsignedIntSampler1DArrayEXT                                   = 0x8DD6,

    [GLExtension("GL_EXT_gpu_shader4")]
    UnsignedIntSampler2DArrayEXT                                   = 0x8DD7,

    [GLExtension("GL_EXT_gpu_shader4")]
    UnsignedIntSamplerBufferEXT                                    = 0x8DD8,

    [GLExtension("GL_ARB_geometry_shader4")]
    GeometryShaderARB                                              = 0x8DD9,

    [GLExtension("GL_EXT_geometry_shader")]
    GeometryShaderEXT                                              = 0x8DD9,

    [GLExtension("GL_ARB_geometry_shader4")]
    GeometryVerticesOutARB                                         = 0x8DDA,

    [GLExtension("GL_EXT_geometry_shader4")]
    GeometryVerticesOutEXT                                         = 0x8DDA,

    [GLExtension("GL_ARB_geometry_shader4")]
    GeometryInputTypeARB                                           = 0x8DDB,

    [GLExtension("GL_EXT_geometry_shader4")]
    GeometryInputTypeEXT                                           = 0x8DDB,

    [GLExtension("GL_ARB_geometry_shader4")]
    GeometryOutputTypeARB                                          = 0x8DDC,

    [GLExtension("GL_EXT_geometry_shader4")]
    GeometryOutputTypeEXT                                          = 0x8DDC,

    [GLExtension("GL_ARB_geometry_shader4")]
    MaxGeometryVaryingComponentsARB                                = 0x8DDD,

    [GLExtension("GL_EXT_geometry_shader4")]
    MaxGeometryVaryingComponentsEXT                                = 0x8DDD,

    [GLExtension("GL_ARB_geometry_shader4")]
    MaxVertexVaryingComponentsARB                                  = 0x8DDE,

    [GLExtension("GL_EXT_geometry_shader4")]
    MaxVertexVaryingComponentsEXT                                  = 0x8DDE,

    [GLExtension("GL_ARB_geometry_shader4")]
    MaxGeometryUniformComponentsARB                                = 0x8DDF,

    [GLExtension("GL_EXT_geometry_shader")]
    MaxGeometryUniformComponentsEXT                                = 0x8DDF,
    MaxGeometryOutputVertices                                      = 0x8DE0,

    [GLExtension("GL_ARB_geometry_shader4")]
    MaxGeometryOutputVerticesARB                                   = 0x8DE0,

    [GLExtension("GL_EXT_geometry_shader")]
    MaxGeometryOutputVerticesEXT                                   = 0x8DE0,
    MaxGeometryTotalOutputComponents                               = 0x8DE1,

    [GLExtension("GL_ARB_geometry_shader4")]
    MaxGeometryTotalOutputComponentsARB                            = 0x8DE1,

    [GLExtension("GL_EXT_geometry_shader")]
    MaxGeometryTotalOutputComponentsEXT                            = 0x8DE1,

    [GLExtension("GL_EXT_bindable_uniform")]
    MaxVertexBindableUniformsEXT                                   = 0x8DE2,

    [GLExtension("GL_EXT_bindable_uniform")]
    MaxFragmentBindableUniformsEXT                                 = 0x8DE3,

    [GLExtension("GL_EXT_bindable_uniform")]
    MaxGeometryBindableUniformsEXT                                 = 0x8DE4,
    MaxSubroutines                                                 = 0x8DE7,
    MaxSubroutineUniformLocations                                  = 0x8DE8,

    [GLExtension("GL_ARB_shading_language_include")]
    NamedStringLengthARB                                           = 0x8DE9,

    [GLExtension("GL_ARB_shading_language_include")]
    NamedStringTypeARB                                             = 0x8DEA,

    [GLExtension("GL_EXT_bindable_uniform")]
    MaxBindableUniformSizeEXT                                      = 0x8DED,

    [GLExtension("GL_EXT_bindable_uniform")]
    UniformBufferEXT                                               = 0x8DEE,

    [GLExtension("GL_EXT_bindable_uniform")]
    UniformBufferBindingEXT                                        = 0x8DEF,
    PolygonOffsetClamp                                             = 0x8E1B,

    [GLExtension("GL_EXT_polygon_offset_clamp")]
    PolygonOffsetClampEXT                                          = 0x8E1B,
    MaxCombinedTessControlUniformComponents                        = 0x8E1E,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxCombinedTessControlUniformComponentsEXT                     = 0x8E1E,
    MaxCombinedTessEvaluationUniformComponents                     = 0x8E1F,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxCombinedTessEvaluationUniformComponentsEXT                  = 0x8E1F,
    TransformFeedbackBufferPaused                                  = 0x8E23,
    TransformFeedbackBufferActive                                  = 0x8E24,
    TransformFeedbackBinding                                       = 0x8E25,

    [GLExtension("GL_EXT_direct_state_access")]
    ProgramMatrixEXT                                               = 0x8E2D,

    [GLExtension("GL_EXT_direct_state_access")]
    TransposeProgramMatrixEXT                                      = 0x8E2E,

    [GLExtension("GL_EXT_direct_state_access")]
    ProgramMatrixStackDepthEXT                                     = 0x8E2F,

    [GLExtension("GL_EXT_texture_swizzle")]
    TextureSwizzleREXT                                             = 0x8E42,

    [GLExtension("GL_EXT_texture_swizzle")]
    TextureSwizzleGEXT                                             = 0x8E43,

    [GLExtension("GL_EXT_texture_swizzle")]
    TextureSwizzleBEXT                                             = 0x8E44,

    [GLExtension("GL_EXT_texture_swizzle")]
    TextureSwizzleAEXT                                             = 0x8E45,

    [GLExtension("GL_EXT_texture_swizzle")]
    TextureSwizzleRgbaEXT                                          = 0x8E46,
    QuadsFollowProvokingVertexConvention                           = 0x8E4C,

    [GLExtension("GL_EXT_provoking_vertex")]
    QuadsFollowProvokingVertexConventionEXT                        = 0x8E4C,

    [GLExtension("GL_EXT_geometry_shader")]
    FirstVertexConventionEXT                                       = 0x8E4D,

    [GLExtension("GL_EXT_geometry_shader")]
    LastVertexConventionEXT                                        = 0x8E4E,

    [GLExtension("GL_EXT_provoking_vertex")]
    ProvokingVertexEXT                                             = 0x8E4F,
    SampleMaskValue                                                = 0x8E52,
    MaxGeometryShaderInvocations                                   = 0x8E5A,

    [GLExtension("GL_EXT_geometry_shader")]
    MaxGeometryShaderInvocationsEXT                                = 0x8E5A,
    MinFragmentInterpolationOffset                                 = 0x8E5B,
    MaxFragmentInterpolationOffset                                 = 0x8E5C,
    FragmentInterpolationOffsetBits                                = 0x8E5D,
    MinProgramTextureGatherOffset                                  = 0x8E5E,

    [GLExtension("GL_ARB_texture_gather")]
    MinProgramTextureGatherOffsetARB                               = 0x8E5E,
    MaxProgramTextureGatherOffset                                  = 0x8E5F,

    [GLExtension("GL_ARB_texture_gather")]
    MaxProgramTextureGatherOffsetARB                               = 0x8E5F,
    MaxTransformFeedbackBuffers                                    = 0x8E70,
    MaxVertexStreams                                               = 0x8E71,

    [GLExtension("GL_EXT_tessellation_shader")]
    PatchVerticesEXT                                               = 0x8E72,
    PatchDefaultInnerLevelEXT                                      = 0x8E73,
    PatchDefaultOuterLevelEXT                                      = 0x8E74,
    TessControlOutputVertices                                      = 0x8E75,

    [GLExtension("GL_EXT_tessellation_shader")]
    TessControlOutputVerticesEXT                                   = 0x8E75,
    TessGenMode                                                    = 0x8E76,

    [GLExtension("GL_EXT_tessellation_shader")]
    TessGenModeEXT                                                 = 0x8E76,
    TessGenSpacing                                                 = 0x8E77,

    [GLExtension("GL_EXT_tessellation_shader")]
    TessGenSpacingEXT                                              = 0x8E77,
    TessGenVertexOrder                                             = 0x8E78,

    [GLExtension("GL_EXT_tessellation_shader")]
    TessGenVertexOrderEXT                                          = 0x8E78,
    TessGenPointMode                                               = 0x8E79,

    [GLExtension("GL_EXT_tessellation_shader")]
    TessGenPointModeEXT                                            = 0x8E79,
    Isolines                                                       = 0x8E7A,

    [GLExtension("GL_EXT_tessellation_shader")]
    IsolinesEXT                                                    = 0x8E7A,
    FractionalOdd                                                  = 0x8E7B,

    [GLExtension("GL_EXT_tessellation_shader")]
    FractionalOddEXT                                               = 0x8E7B,
    FractionalEven                                                 = 0x8E7C,

    [GLExtension("GL_EXT_tessellation_shader")]
    FractionalEvenEXT                                              = 0x8E7C,
    MaxPatchVertices                                               = 0x8E7D,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxPatchVerticesEXT                                            = 0x8E7D,
    MaxTessGenLevel                                                = 0x8E7E,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessGenLevelEXT                                             = 0x8E7E,
    MaxTessControlUniformComponents                                = 0x8E7F,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessControlUniformComponentsEXT                             = 0x8E7F,
    MaxTessEvaluationUniformComponents                             = 0x8E80,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessEvaluationUniformComponentsEXT                          = 0x8E80,
    MaxTessControlTextureImageUnits                                = 0x8E81,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessControlTextureImageUnitsEXT                             = 0x8E81,
    MaxTessEvaluationTextureImageUnits                             = 0x8E82,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessEvaluationTextureImageUnitsEXT                          = 0x8E82,
    MaxTessControlOutputComponents                                 = 0x8E83,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessControlOutputComponentsEXT                              = 0x8E83,
    MaxTessPatchComponents                                         = 0x8E84,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessPatchComponentsEXT                                      = 0x8E84,
    MaxTessControlTotalOutputComponents                            = 0x8E85,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessControlTotalOutputComponentsEXT                         = 0x8E85,
    MaxTessEvaluationOutputComponents                              = 0x8E86,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessEvaluationOutputComponentsEXT                           = 0x8E86,

    [GLExtension("GL_EXT_tessellation_shader")]
    TessEvaluationShaderEXT                                        = 0x8E87,

    [GLExtension("GL_EXT_tessellation_shader")]
    TessControlShaderEXT                                           = 0x8E88,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessControlUniformBlocksEXT                                 = 0x8E89,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessEvaluationUniformBlocksEXT                              = 0x8E8A,

    [GLExtension("GL_EXT_window_rectangles")]
    InclusiveEXT                                                   = 0x8F10,

    [GLExtension("GL_EXT_window_rectangles")]
    ExclusiveEXT                                                   = 0x8F11,

    [GLExtension("GL_EXT_window_rectangles")]
    WindowRectangleEXT                                             = 0x8F12,

    [GLExtension("GL_EXT_window_rectangles")]
    WindowRectangleModeEXT                                         = 0x8F13,

    [GLExtension("GL_EXT_window_rectangles")]
    MaxWindowRectanglesEXT                                         = 0x8F14,

    [GLExtension("GL_EXT_window_rectangles")]
    NumWindowRectanglesEXT                                         = 0x8F15,
    CopyReadBufferBinding                                          = 0x8F36,
    CopyWriteBufferBinding                                         = 0x8F37,
    MaxImageUnits                                                  = 0x8F38,

    [GLExtension("GL_EXT_shader_image_load_store")]
    MaxImageUnitsEXT                                               = 0x8F38,
    MaxCombinedImageUnitsAndFragmentOutputs                        = 0x8F39,

    [GLExtension("GL_EXT_shader_image_load_store")]
    MaxCombinedImageUnitsAndFragmentOutputsEXT                     = 0x8F39,
    MaxCombinedShaderOutputResources                               = 0x8F39,
    ImageBindingName                                               = 0x8F3A,

    [GLExtension("GL_EXT_shader_image_load_store")]
    ImageBindingNameEXT                                            = 0x8F3A,
    ImageBindingLevel                                              = 0x8F3B,

    [GLExtension("GL_EXT_shader_image_load_store")]
    ImageBindingLevelEXT                                           = 0x8F3B,
    ImageBindingLayered                                            = 0x8F3C,

    [GLExtension("GL_EXT_shader_image_load_store")]
    ImageBindingLayeredEXT                                         = 0x8F3C,
    ImageBindingLayer                                              = 0x8F3D,

    [GLExtension("GL_EXT_shader_image_load_store")]
    ImageBindingLayerEXT                                           = 0x8F3D,
    ImageBindingAccess                                             = 0x8F3E,

    [GLExtension("GL_EXT_shader_image_load_store")]
    ImageBindingAccessEXT                                          = 0x8F3E,
    DrawIndirectBufferBinding                                      = 0x8F43,

    [GLExtension("GL_EXT_vertex_attrib_64bit")]
    DoubleMat2EXT                                                  = 0x8F46,

    [GLExtension("GL_EXT_vertex_attrib_64bit")]
    DoubleMat3EXT                                                  = 0x8F47,

    [GLExtension("GL_EXT_vertex_attrib_64bit")]
    DoubleMat4EXT                                                  = 0x8F48,

    [GLExtension("GL_EXT_vertex_attrib_64bit")]
    DoubleMat2x3EXT                                                = 0x8F49,

    [GLExtension("GL_EXT_vertex_attrib_64bit")]
    DoubleMat2x4EXT                                                = 0x8F4A,

    [GLExtension("GL_EXT_vertex_attrib_64bit")]
    DoubleMat3x2EXT                                                = 0x8F4B,

    [GLExtension("GL_EXT_vertex_attrib_64bit")]
    DoubleMat3x4EXT                                                = 0x8F4C,

    [GLExtension("GL_EXT_vertex_attrib_64bit")]
    DoubleMat4x2EXT                                                = 0x8F4D,

    [GLExtension("GL_EXT_vertex_attrib_64bit")]
    DoubleMat4x3EXT                                                = 0x8F4E,
    VertexBindingBuffer                                            = 0x8F4F,

    [GLExtension("GL_EXT_shader_pixel_local_storage")]
    MaxShaderPixelLocalStorageFastSizeEXT                          = 0x8F63,

    [GLExtension("GL_EXT_shader_pixel_local_storage")]
    ShaderPixelLocalStorageEXT                                     = 0x8F64,

    [GLExtension("GL_EXT_shader_pixel_local_storage")]
    MaxShaderPixelLocalStorageSizeEXT                              = 0x8F67,

    [GLExtension("GL_EXT_texture_compression_astc_decode_mode")]
    TextureAstcDecodePrecisionEXT                                  = 0x8F69,

    [GLExtension("GL_EXT_texture_snorm")]
    RedSnorm                                                       = 0x8F90,

    [GLExtension("GL_EXT_texture_snorm")]
    RgSnorm                                                        = 0x8F91,

    [GLExtension("GL_EXT_texture_snorm")]
    RgbSnorm                                                       = 0x8F92,

    [GLExtension("GL_EXT_texture_snorm")]
    RgbaSnorm                                                      = 0x8F93,
    SignedNormalized                                               = 0x8F9C,

    [GLExtension("GL_ARB_texture_gather")]
    MaxProgramTextureGatherComponentsARB                           = 0x8F9F,

    [GLExtension("GL_EXT_disjoint_timer_query")]
    GpuDisjointEXT                                                 = 0x8FBB,

    [GLExtension("GL_EXT_texture_format_sRGB_override")]
    TextureFormatSrgbOverrideEXT                                   = 0x8FBF,

    [GLExtension("GL_EXT_vertex_attrib_64bit")]
    DoubleVec2EXT                                                  = 0x8FFC,

    [GLExtension("GL_EXT_vertex_attrib_64bit")]
    DoubleVec3EXT                                                  = 0x8FFD,

    [GLExtension("GL_EXT_vertex_attrib_64bit")]
    DoubleVec4EXT                                                  = 0x8FFE,
    TextureBindingCubeMapArray                                     = 0x900A,

    [GLExtension("GL_ARB_texture_cube_map_array")]
    TextureBindingCubeMapArrayARB                                  = 0x900A,

    [GLExtension("GL_EXT_texture_cube_map_array")]
    TextureBindingCubeMapArrayEXT                                  = 0x900A,

    [GLExtension("GL_ARB_texture_cube_map_array")]
    SamplerCubeMapArrayARB                                         = 0x900C,

    [GLExtension("GL_EXT_texture_cube_map_array")]
    SamplerCubeMapArrayEXT                                         = 0x900C,

    [GLExtension("GL_ARB_texture_cube_map_array")]
    SamplerCubeMapArrayShadowARB                                   = 0x900D,

    [GLExtension("GL_EXT_texture_cube_map_array")]
    SamplerCubeMapArrayShadowEXT                                   = 0x900D,

    [GLExtension("GL_ARB_texture_cube_map_array")]
    IntSamplerCubeMapArrayARB                                      = 0x900E,

    [GLExtension("GL_EXT_texture_cube_map_array")]
    IntSamplerCubeMapArrayEXT                                      = 0x900E,

    [GLExtension("GL_ARB_texture_cube_map_array")]
    UnsignedIntSamplerCubeMapArrayARB                              = 0x900F,

    [GLExtension("GL_EXT_texture_cube_map_array")]
    UnsignedIntSamplerCubeMapArrayEXT                              = 0x900F,

    [GLExtension("GL_EXT_texture_snorm")]
    AlphaSnorm                                                     = 0x9010,

    [GLExtension("GL_EXT_texture_snorm")]
    LuminanceSnorm                                                 = 0x9011,

    [GLExtension("GL_EXT_texture_snorm")]
    LuminanceAlphaSnorm                                            = 0x9012,

    [GLExtension("GL_EXT_texture_snorm")]
    IntensitySnorm                                                 = 0x9013,

    [GLExtension("GL_EXT_texture_snorm")]
    Alpha8Snorm                                                    = 0x9014,

    [GLExtension("GL_EXT_texture_snorm")]
    Luminance8Snorm                                                = 0x9015,

    [GLExtension("GL_EXT_texture_snorm")]
    Luminance8Alpha8Snorm                                          = 0x9016,

    [GLExtension("GL_EXT_texture_snorm")]
    Intensity8Snorm                                                = 0x9017,

    [GLExtension("GL_EXT_texture_snorm")]
    Alpha16Snorm                                                   = 0x9018,

    [GLExtension("GL_EXT_texture_snorm")]
    Luminance16Snorm                                               = 0x9019,

    [GLExtension("GL_EXT_texture_snorm")]
    Luminance16Alpha16Snorm                                        = 0x901A,

    [GLExtension("GL_EXT_texture_snorm")]
    Intensity16Snorm                                               = 0x901B,

    [GLExtension("GL_EXT_shader_image_load_store")]
    Image1DEXT                                                     = 0x904C,

    [GLExtension("GL_EXT_shader_image_load_store")]
    Image2DEXT                                                     = 0x904D,

    [GLExtension("GL_EXT_shader_image_load_store")]
    Image3DEXT                                                     = 0x904E,

    [GLExtension("GL_EXT_shader_image_load_store")]
    Image2DRectEXT                                                 = 0x904F,

    [GLExtension("GL_EXT_shader_image_load_store")]
    ImageCubeEXT                                                   = 0x9050,

    [GLExtension("GL_EXT_shader_image_load_store")]
    ImageBufferEXT                                                 = 0x9051,

    [GLExtension("GL_EXT_shader_image_load_store")]
    Image1DArrayEXT                                                = 0x9052,

    [GLExtension("GL_EXT_shader_image_load_store")]
    Image2DArrayEXT                                                = 0x9053,

    [GLExtension("GL_EXT_shader_image_load_store")]
    ImageCubeMapArrayEXT                                           = 0x9054,

    [GLExtension("GL_EXT_shader_image_load_store")]
    Image2DMultisampleEXT                                          = 0x9055,

    [GLExtension("GL_EXT_shader_image_load_store")]
    Image2DMultisampleArrayEXT                                     = 0x9056,

    [GLExtension("GL_EXT_shader_image_load_store")]
    IntImage1DEXT                                                  = 0x9057,

    [GLExtension("GL_EXT_shader_image_load_store")]
    IntImage2DEXT                                                  = 0x9058,

    [GLExtension("GL_EXT_shader_image_load_store")]
    IntImage3DEXT                                                  = 0x9059,

    [GLExtension("GL_EXT_shader_image_load_store")]
    IntImage2DRectEXT                                              = 0x905A,

    [GLExtension("GL_EXT_shader_image_load_store")]
    IntImageCubeEXT                                                = 0x905B,

    [GLExtension("GL_EXT_shader_image_load_store")]
    IntImageBufferEXT                                              = 0x905C,

    [GLExtension("GL_EXT_shader_image_load_store")]
    IntImage1DArrayEXT                                             = 0x905D,

    [GLExtension("GL_EXT_shader_image_load_store")]
    IntImage2DArrayEXT                                             = 0x905E,

    [GLExtension("GL_EXT_shader_image_load_store")]
    IntImageCubeMapArrayEXT                                        = 0x905F,

    [GLExtension("GL_EXT_shader_image_load_store")]
    IntImage2DMultisampleEXT                                       = 0x9060,

    [GLExtension("GL_EXT_shader_image_load_store")]
    IntImage2DMultisampleArrayEXT                                  = 0x9061,

    [GLExtension("GL_EXT_shader_image_load_store")]
    UnsignedIntImage1DEXT                                          = 0x9062,

    [GLExtension("GL_EXT_shader_image_load_store")]
    UnsignedIntImage2DEXT                                          = 0x9063,

    [GLExtension("GL_EXT_shader_image_load_store")]
    UnsignedIntImage3DEXT                                          = 0x9064,

    [GLExtension("GL_EXT_shader_image_load_store")]
    UnsignedIntImage2DRectEXT                                      = 0x9065,

    [GLExtension("GL_EXT_shader_image_load_store")]
    UnsignedIntImageCubeEXT                                        = 0x9066,

    [GLExtension("GL_EXT_shader_image_load_store")]
    UnsignedIntImageBufferEXT                                      = 0x9067,

    [GLExtension("GL_EXT_shader_image_load_store")]
    UnsignedIntImage1DArrayEXT                                     = 0x9068,

    [GLExtension("GL_EXT_shader_image_load_store")]
    UnsignedIntImage2DArrayEXT                                     = 0x9069,

    [GLExtension("GL_EXT_shader_image_load_store")]
    UnsignedIntImageCubeMapArrayEXT                                = 0x906A,

    [GLExtension("GL_EXT_shader_image_load_store")]
    UnsignedIntImage2DMultisampleEXT                               = 0x906B,

    [GLExtension("GL_EXT_shader_image_load_store")]
    UnsignedIntImage2DMultisampleArrayEXT                          = 0x906C,
    MaxImageSamples                                                = 0x906D,

    [GLExtension("GL_EXT_shader_image_load_store")]
    MaxImageSamplesEXT                                             = 0x906D,
    ImageBindingFormat                                             = 0x906E,

    [GLExtension("GL_EXT_shader_image_load_store")]
    ImageBindingFormatEXT                                          = 0x906E,

    [GLExtension("GL_EXT_framebuffer_multisample_blit_scaled")]
    ScaledResolveFastestEXT                                        = 0x90BA,

    [GLExtension("GL_EXT_framebuffer_multisample_blit_scaled")]
    ScaledResolveNicestEXT                                         = 0x90BB,
    ImageFormatCompatibilityBySize                                 = 0x90C8,
    ImageFormatCompatibilityByClass                                = 0x90C9,
    MaxVertexImageUniforms                                         = 0x90CA,
    MaxTessControlImageUniforms                                    = 0x90CB,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessControlImageUniformsEXT                                 = 0x90CB,
    MaxTessEvaluationImageUniforms                                 = 0x90CC,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessEvaluationImageUniformsEXT                              = 0x90CC,
    MaxGeometryImageUniforms                                       = 0x90CD,

    [GLExtension("GL_EXT_geometry_shader")]
    MaxGeometryImageUniformsEXT                                    = 0x90CD,
    MaxFragmentImageUniforms                                       = 0x90CE,
    MaxCombinedImageUniforms                                       = 0x90CF,

    [GLExtension("GL_EXT_geometry_shader")]
    MaxGeometryShaderStorageBlocksEXT                              = 0x90D7,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessControlShaderStorageBlocksEXT                           = 0x90D8,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessEvaluationShaderStorageBlocksEXT                        = 0x90D9,
    MaxShaderStorageBlockSize                                      = 0x90DE,

    [GLExtension("GL_EXT_x11_sync_object")]
    SyncX11FenceEXT                                                = 0x90E1,

    [GLExtension("GL_ARB_compute_variable_group_size")]
    MaxComputeFixedGroupInvocationsARB                             = 0x90EB,

    [GLExtension("GL_EXT_multiview_draw_buffers")]
    ColorAttachmentEXT                                             = 0x90F0,

    [GLExtension("GL_EXT_multiview_draw_buffers")]
    MultiviewEXT                                                   = 0x90F1,

    [GLExtension("GL_EXT_multiview_draw_buffers")]
    MaxMultiviewBuffersEXT                                         = 0x90F2,
    ContextRobustAccess                                            = 0x90F3,

    [GLExtension("GL_EXT_robustness")]
    ContextRobustAccessEXT                                         = 0x90F3,

    [GLExtension("GL_KHR_robustness")]
    ContextRobustAccessKhr                                         = 0x90F3,
    TextureSamples                                                 = 0x9106,
    TextureFixedSampleLocations                                    = 0x9107,
    SyncFence                                                      = 0x9116,
    Unsignaled                                                     = 0x9118,
    Signaled                                                       = 0x9119,

    [GLExtension("GL_EXT_geometry_shader")]
    MaxGeometryInputComponentsEXT                                  = 0x9123,

    [GLExtension("GL_EXT_geometry_shader")]
    MaxGeometryOutputComponentsEXT                                 = 0x9124,
    UnpackCompressedBlockWidth                                     = 0x9127,
    UnpackCompressedBlockHeight                                    = 0x9128,
    UnpackCompressedBlockDepth                                     = 0x9129,
    UnpackCompressedBlockSize                                      = 0x912A,
    PackCompressedBlockWidth                                       = 0x912B,
    PackCompressedBlockHeight                                      = 0x912C,
    PackCompressedBlockDepth                                       = 0x912D,
    PackCompressedBlockSize                                        = 0x912E,
    TextureImmutableFormat                                         = 0x912F,

    [GLExtension("GL_EXT_texture_storage")]
    TextureImmutableFormatEXT                                      = 0x912F,
    MaxDebugMessageLength                                          = 0x9143,

    [GLExtension("GL_ARB_debug_output")]
    MaxDebugMessageLengthARB                                       = 0x9143,

    [GLExtension("GL_KHR_debug")]
    MaxDebugMessageLengthKhr                                       = 0x9143,
    MaxDebugLoggedMessages                                         = 0x9144,

    [GLExtension("GL_ARB_debug_output")]
    MaxDebugLoggedMessagesARB                                      = 0x9144,

    [GLExtension("GL_KHR_debug")]
    MaxDebugLoggedMessagesKhr                                      = 0x9144,
    DebugLoggedMessages                                            = 0x9145,

    [GLExtension("GL_ARB_debug_output")]
    DebugLoggedMessagesARB                                         = 0x9145,

    [GLExtension("GL_KHR_debug")]
    DebugLoggedMessagesKhr                                         = 0x9145,

    [GLExtension("GL_ARB_debug_output")]
    DebugSeverityHighARB                                           = 0x9146,

    [GLExtension("GL_KHR_debug")]
    DebugSeverityHighKhr                                           = 0x9146,

    [GLExtension("GL_ARB_debug_output")]
    DebugSeverityMediumARB                                         = 0x9147,

    [GLExtension("GL_KHR_debug")]
    DebugSeverityMediumKhr                                         = 0x9147,

    [GLExtension("GL_ARB_debug_output")]
    DebugSeverityLowARB                                            = 0x9148,

    [GLExtension("GL_KHR_debug")]
    DebugSeverityLowKhr                                            = 0x9148,

    [GLExtension("GL_EXT_debug_label")]
    BufferObjectEXT                                                = 0x9151,

    [GLExtension("GL_EXT_debug_label")]
    QueryObjectEXT                                                 = 0x9153,

    [GLExtension("GL_EXT_debug_label")]
    VertexArrayObjectEXT                                           = 0x9154,
    QueryBufferBinding                                             = 0x9193,

    [GLExtension("GL_ARB_sparse_texture")]
    VirtualPageSizeXARB                                            = 0x9195,

    [GLExtension("GL_EXT_sparse_texture")]
    VirtualPageSizeXEXT                                            = 0x9195,

    [GLExtension("GL_ARB_sparse_texture")]
    VirtualPageSizeYARB                                            = 0x9196,

    [GLExtension("GL_EXT_sparse_texture")]
    VirtualPageSizeYEXT                                            = 0x9196,

    [GLExtension("GL_ARB_sparse_texture")]
    VirtualPageSizeZARB                                            = 0x9197,

    [GLExtension("GL_EXT_sparse_texture")]
    VirtualPageSizeZEXT                                            = 0x9197,

    [GLExtension("GL_ARB_sparse_texture")]
    MaxSparseTextureSizeARB                                        = 0x9198,

    [GLExtension("GL_EXT_sparse_texture")]
    MaxSparseTextureSizeEXT                                        = 0x9198,

    [GLExtension("GL_ARB_sparse_texture")]
    MaxSparse3DTextureSizeARB                                      = 0x9199,

    [GLExtension("GL_EXT_sparse_texture")]
    MaxSparse3DTextureSizeEXT                                      = 0x9199,

    [GLExtension("GL_ARB_sparse_texture")]
    MaxSparseArrayTextureLayersARB                                 = 0x919A,

    [GLExtension("GL_EXT_sparse_texture")]
    MaxSparseArrayTextureLayersEXT                                 = 0x919A,
    TextureBufferOffset                                            = 0x919D,

    [GLExtension("GL_EXT_texture_buffer")]
    TextureBufferOffsetEXT                                         = 0x919D,
    TextureBufferSize                                              = 0x919E,

    [GLExtension("GL_EXT_texture_buffer")]
    TextureBufferSizeEXT                                           = 0x919E,

    [GLExtension("GL_EXT_texture_buffer")]
    TextureBufferOffsetAlignmentEXT                                = 0x919F,

    [GLExtension("GL_ARB_sparse_texture")]
    TextureSparseARB                                               = 0x91A6,

    [GLExtension("GL_EXT_sparse_texture")]
    TextureSparseEXT                                               = 0x91A6,

    [GLExtension("GL_ARB_sparse_texture")]
    VirtualPageSizeIndexARB                                        = 0x91A7,

    [GLExtension("GL_EXT_sparse_texture")]
    VirtualPageSizeIndexEXT                                        = 0x91A7,

    [GLExtension("GL_ARB_sparse_texture")]
    NumVirtualPageSizesARB                                         = 0x91A8,

    [GLExtension("GL_EXT_sparse_texture")]
    NumVirtualPageSizesEXT                                         = 0x91A8,

    [GLExtension("GL_ARB_sparse_texture")]
    SparseTextureFullArrayCubeMipmapsARB                           = 0x91A9,

    [GLExtension("GL_EXT_sparse_texture")]
    SparseTextureFullArrayCubeMipmapsEXT                           = 0x91A9,

    [GLExtension("GL_ARB_sparse_texture")]
    NumSparseLevelsARB                                             = 0x91AA,

    [GLExtension("GL_EXT_sparse_texture")]
    NumSparseLevelsEXT                                             = 0x91AA,

    [GLExtension("GL_KHR_parallel_shader_compile")]
    MaxShaderCompilerThreadsKhr                                    = 0x91B0,

    [GLExtension("GL_ARB_parallel_shader_compile")]
    MaxShaderCompilerThreadsARB                                    = 0x91B0,

    [GLExtension("GL_KHR_parallel_shader_compile")]
    CompletionStatusKhr                                            = 0x91B1,

    [GLExtension("GL_ARB_parallel_shader_compile")]
    CompletionStatusARB                                            = 0x91B1,
    MaxComputeImageUniforms                                        = 0x91BD,

    [GLExtension("GL_ARB_compute_variable_group_size")]
    MaxComputeFixedGroupSizeARB                                    = 0x91BF,
    UnpackFlipYWebgl                                               = 0x9240,
    UnpackPremultiplyAlphaWebgl                                    = 0x9241,
    ContextLostWebgl                                               = 0x9242,
    UnpackColorspaceConversionWebgl                                = 0x9243,
    BrowserDefaultWebgl                                            = 0x9244,

    [GLExtension("GL_KHR_blend_equation_advanced_coherent")]
    BlendAdvancedCoherentKhr                                       = 0x9285,
    Multiply                                                       = 0x9294,

    [GLExtension("GL_KHR_blend_equation_advanced")]
    MultiplyKhr                                                    = 0x9294,
    Screen                                                         = 0x9295,

    [GLExtension("GL_KHR_blend_equation_advanced")]
    ScreenKhr                                                      = 0x9295,
    Overlay                                                        = 0x9296,

    [GLExtension("GL_KHR_blend_equation_advanced")]
    OverlayKhr                                                     = 0x9296,
    Darken                                                         = 0x9297,

    [GLExtension("GL_KHR_blend_equation_advanced")]
    DarkenKhr                                                      = 0x9297,
    Lighten                                                        = 0x9298,

    [GLExtension("GL_KHR_blend_equation_advanced")]
    LightenKhr                                                     = 0x9298,
    Colordodge                                                     = 0x9299,

    [GLExtension("GL_KHR_blend_equation_advanced")]
    ColordodgeKhr                                                  = 0x9299,
    Colorburn                                                      = 0x929A,

    [GLExtension("GL_KHR_blend_equation_advanced")]
    ColorburnKhr                                                   = 0x929A,
    Hardlight                                                      = 0x929B,

    [GLExtension("GL_KHR_blend_equation_advanced")]
    HardlightKhr                                                   = 0x929B,
    Softlight                                                      = 0x929C,

    [GLExtension("GL_KHR_blend_equation_advanced")]
    SoftlightKhr                                                   = 0x929C,
    Difference                                                     = 0x929E,

    [GLExtension("GL_KHR_blend_equation_advanced")]
    DifferenceKhr                                                  = 0x929E,
    Exclusion                                                      = 0x92A0,

    [GLExtension("GL_KHR_blend_equation_advanced")]
    ExclusionKhr                                                   = 0x92A0,
    HslHue                                                         = 0x92AD,

    [GLExtension("GL_KHR_blend_equation_advanced")]
    HslHueKhr                                                      = 0x92AD,
    HslSaturation                                                  = 0x92AE,

    [GLExtension("GL_KHR_blend_equation_advanced")]
    HslSaturationKhr                                               = 0x92AE,
    HslColor                                                       = 0x92AF,

    [GLExtension("GL_KHR_blend_equation_advanced")]
    HslColorKhr                                                    = 0x92AF,
    HslLuminosity                                                  = 0x92B0,

    [GLExtension("GL_KHR_blend_equation_advanced")]
    HslLuminosityKhr                                               = 0x92B0,

    [GLExtension("GL_ARB_ES3_2_compatibility")]
    PrimitiveBoundingBoxARB                                        = 0x92BE,
    PrimitiveBoundingBox                                           = 0x92BE,

    [GLExtension("GL_EXT_primitive_bounding_box")]
    PrimitiveBoundingBoxEXT                                        = 0x92BE,
    AtomicCounterBufferStart                                       = 0x92C2,
    AtomicCounterBufferSize                                        = 0x92C3,
    MaxVertexAtomicCounterBuffers                                  = 0x92CC,
    MaxTessControlAtomicCounterBuffers                             = 0x92CD,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessControlAtomicCounterBuffersEXT                          = 0x92CD,
    MaxTessEvaluationAtomicCounterBuffers                          = 0x92CE,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessEvaluationAtomicCounterBuffersEXT                       = 0x92CE,
    MaxGeometryAtomicCounterBuffers                                = 0x92CF,

    [GLExtension("GL_EXT_geometry_shader")]
    MaxGeometryAtomicCounterBuffersEXT                             = 0x92CF,
    MaxFragmentAtomicCounterBuffers                                = 0x92D0,
    MaxCombinedAtomicCounterBuffers                                = 0x92D1,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessControlAtomicCountersEXT                                = 0x92D3,

    [GLExtension("GL_EXT_tessellation_shader")]
    MaxTessEvaluationAtomicCountersEXT                             = 0x92D4,

    [GLExtension("GL_EXT_geometry_shader")]
    MaxGeometryAtomicCountersEXT                                   = 0x92D5,
    MaxAtomicCounterBufferSize                                     = 0x92D8,
    UnsignedIntAtomicCounter                                       = 0x92DB,
    MaxAtomicCounterBufferBindings                                 = 0x92DC,

    [GLExtension("GL_KHR_debug")]
    DebugOutputKhr                                                 = 0x92E0,

    [GLExtension("GL_EXT_tessellation_shader")]
    IsPerPatchEXT                                                  = 0x92E7,

    [GLExtension("GL_EXT_tessellation_shader")]
    ReferencedByTessControlShaderEXT                               = 0x9307,

    [GLExtension("GL_EXT_tessellation_shader")]
    ReferencedByTessEvaluationShaderEXT                            = 0x9308,

    [GLExtension("GL_EXT_geometry_shader")]
    ReferencedByGeometryShaderEXT                                  = 0x9309,

    [GLExtension("GL_EXT_blend_func_extended")]
    LocationIndexEXT                                               = 0x930F,

    [GLExtension("GL_EXT_geometry_shader")]
    FramebufferDefaultLayersEXT                                    = 0x9312,

    [GLExtension("GL_EXT_geometry_shader")]
    MaxFramebufferLayersEXT                                        = 0x9317,

    [GLExtension("GL_EXT_raster_multisample")]
    RasterMultisampleEXT                                           = 0x9327,

    [GLExtension("GL_EXT_raster_multisample")]
    RasterSamplesEXT                                               = 0x9328,

    [GLExtension("GL_EXT_raster_multisample")]
    MaxRasterSamplesEXT                                            = 0x9329,

    [GLExtension("GL_EXT_raster_multisample")]
    RasterFixedSampleLocationsEXT                                  = 0x932A,

    [GLExtension("GL_EXT_raster_multisample")]
    MultisampleRasterizationAllowedEXT                             = 0x932B,

    [GLExtension("GL_EXT_raster_multisample")]
    EffectiveRasterSamplesEXT                                      = 0x932C,

    [GLExtension("GL_ARB_sample_locations")]
    SampleLocationSubpixelBitsARB                                  = 0x933D,

    [GLExtension("GL_ARB_sample_locations")]
    SampleLocationPixelGridWidthARB                                = 0x933E,

    [GLExtension("GL_ARB_sample_locations")]
    SampleLocationPixelGridHeightARB                               = 0x933F,

    [GLExtension("GL_ARB_sample_locations")]
    ProgrammableSampleLocationTableSizeARB                         = 0x9340,

    [GLExtension("GL_ARB_sample_locations")]
    FramebufferProgrammableSampleLocationsARB                      = 0x9342,

    [GLExtension("GL_ARB_sample_locations")]
    FramebufferSampleLocationPixelGridARB                          = 0x9343,

    [GLExtension("GL_ARB_compute_variable_group_size")]
    MaxComputeVariableGroupInvocationsARB                          = 0x9344,

    [GLExtension("GL_ARB_compute_variable_group_size")]
    MaxComputeVariableGroupSizeARB                                 = 0x9345,
    ClipOrigin                                                     = 0x935C,

    [GLExtension("GL_EXT_clip_control")]
    ClipOriginEXT                                                  = 0x935C,
    ClipDepthMode                                                  = 0x935D,

    [GLExtension("GL_EXT_clip_control")]
    ClipDepthModeEXT                                               = 0x935D,

    [GLExtension("GL_EXT_clip_control")]
    NegativeOneToOneEXT                                            = 0x935E,

    [GLExtension("GL_EXT_clip_control")]
    ZeroToOneEXT                                                   = 0x935F,

    [GLExtension("GL_ARB_texture_filter_minmax")]
    TextureReductionModeARB                                        = 0x9366,

    [GLExtension("GL_EXT_texture_filter_minmax")]
    TextureReductionModeEXT                                        = 0x9366,

    [GLExtension("GL_ARB_texture_filter_minmax")]
    WeightedAverageARB                                             = 0x9367,

    [GLExtension("GL_EXT_texture_filter_minmax")]
    WeightedAverageEXT                                             = 0x9367,

    [GLExtension("GL_ARB_ES3_2_compatibility")]
    MultisampleLineWidthRangeARB                                   = 0x9381,
    MultisampleLineWidthRange                                      = 0x9381,

    [GLExtension("GL_ARB_ES3_2_compatibility")]
    MultisampleLineWidthGranularityARB                             = 0x9382,
    MultisampleLineWidthGranularity                                = 0x9382,

    [GLExtension("GL_ARB_internalformat_query2")]
    ViewClassEacR11                                                = 0x9383,

    [GLExtension("GL_ARB_internalformat_query2")]
    ViewClassEacRg11                                               = 0x9384,

    [GLExtension("GL_ARB_internalformat_query2")]
    ViewClassEtc2Rgb                                               = 0x9385,

    [GLExtension("GL_ARB_internalformat_query2")]
    ViewClassEtc2Rgba                                              = 0x9386,

    [GLExtension("GL_ARB_internalformat_query2")]
    ViewClassEtc2EacRgba                                           = 0x9387,

    [GLExtension("GL_ARB_internalformat_query2")]
    ViewClassAstc4x4Rgba                                           = 0x9388,

    [GLExtension("GL_ARB_internalformat_query2")]
    ViewClassAstc5x4Rgba                                           = 0x9389,

    [GLExtension("GL_ARB_internalformat_query2")]
    ViewClassAstc5x5Rgba                                           = 0x938A,

    [GLExtension("GL_ARB_internalformat_query2")]
    ViewClassAstc6x5Rgba                                           = 0x938B,

    [GLExtension("GL_ARB_internalformat_query2")]
    ViewClassAstc6x6Rgba                                           = 0x938C,

    [GLExtension("GL_ARB_internalformat_query2")]
    ViewClassAstc8x5Rgba                                           = 0x938D,

    [GLExtension("GL_ARB_internalformat_query2")]
    ViewClassAstc8x6Rgba                                           = 0x938E,

    [GLExtension("GL_ARB_internalformat_query2")]
    ViewClassAstc8x8Rgba                                           = 0x938F,

    [GLExtension("GL_ARB_internalformat_query2")]
    ViewClassAstc10x5Rgba                                          = 0x9390,

    [GLExtension("GL_ARB_internalformat_query2")]
    ViewClassAstc10x6Rgba                                          = 0x9391,

    [GLExtension("GL_ARB_internalformat_query2")]
    ViewClassAstc10x8Rgba                                          = 0x9392,

    [GLExtension("GL_ARB_internalformat_query2")]
    ViewClassAstc10x10Rgba                                         = 0x9393,

    [GLExtension("GL_ARB_internalformat_query2")]
    ViewClassAstc12x10Rgba                                         = 0x9394,

    [GLExtension("GL_ARB_internalformat_query2")]
    ViewClassAstc12x12Rgba                                         = 0x9395,

    [GLExtension("GL_EXT_pvrtc_sRGB")]
    CompressedSrgbAlphaPvrtc2bppv2Img                              = 0x93F0,

    [GLExtension("GL_EXT_pvrtc_sRGB")]
    CompressedSrgbAlphaPvrtc4bppv2Img                              = 0x93F1,

    [GLExtension("GL_KHR_shader_subgroup")]
    SubgroupSizeKhr                                                = 0x9532,

    [GLExtension("GL_KHR_shader_subgroup")]
    SubgroupSupportedStagesKhr                                     = 0x9533,

    [GLExtension("GL_KHR_shader_subgroup")]
    SubgroupSupportedFeaturesKhr                                   = 0x9534,

    [GLExtension("GL_KHR_shader_subgroup")]
    SubgroupQuadAllStagesKhr                                       = 0x9535,

    [GLExtension("GL_ARB_gl_spirv")]
    ShaderBinaryFormatSpirVARB                                     = 0x9551,
    SpirVBinary                                                    = 0x9552,

    [GLExtension("GL_ARB_gl_spirv")]
    SpirVBinaryARB                                                 = 0x9552,
    SpirVExtensions                                                = 0x9553,
    NumSpirVExtensions                                             = 0x9554,

    [GLExtension("GL_EXT_memory_object")]
    NumTilingTypesEXT                                              = 0x9582,

    [GLExtension("GL_EXT_memory_object")]
    TilingTypesEXT                                                 = 0x9583,

    [GLExtension("GL_EXT_memory_object")]
    OptimalTilingEXT                                               = 0x9584,

    [GLExtension("GL_EXT_memory_object")]
    LinearTilingEXT                                                = 0x9585,

    [GLExtension("GL_OVR_multiview")]
    MaxViewsOvr                                                    = 0x9631,

    [GLExtension("GL_OVR_multiview")]
    FramebufferIncompleteViewTargetsOvr                            = 0x9633,
    GsShaderBinaryMtk                                              = 0x9640,
    GsProgramBinaryMtk                                             = 0x9641,

    [GLExtension("GL_EXT_shader_pixel_local_storage2")]
    MaxShaderCombinedLocalStorageFastSizeEXT                       = 0x9650,

    [GLExtension("GL_EXT_shader_pixel_local_storage2")]
    MaxShaderCombinedLocalStorageSizeEXT                           = 0x9651,

    [GLExtension("GL_EXT_shader_pixel_local_storage2")]
    FramebufferIncompleteInsufficientShaderCombinedLocalStorageEXT = 0x9652,
    ValidateShaderBinaryQCOM                                       = 0x96A3,
}

