#define MODULE_RAYCAST_ENABLED
#define MODULE_SVG_ENABLED
#define MODULE_TEXT_SERVER_ADV_ENABLED

namespace Godot.Net.Modules;

using static Godot.Net.Modules.RayCast.RegisterTypes;
using static Godot.Net.Modules.SVG.RegisterTypes;
using static Godot.Net.Modules.TextServerAdv.RegisterTypes;

#pragma warning disable IDE0022 // TODO Remove

// TODO - Implement source generation
public static class RegisterModulesTypesModule
{
    public enum ModuleInitializationLevel
    {
        MODULE_INITIALIZATION_LEVEL_CORE, // = GDEXTENSION_INITIALIZATION_CORE,
        MODULE_INITIALIZATION_LEVEL_SERVERS, // = GDEXTENSION_INITIALIZATION_SERVERS,
        MODULE_INITIALIZATION_LEVEL_SCENE, // = GDEXTENSION_INITIALIZATION_SCENE,
        MODULE_INITIALIZATION_LEVEL_EDITOR, // = GDEXTENSION_INITIALIZATION_EDITOR
    };

    public static void InitializeModules(ModuleInitializationLevel level)
    {
        // #if MODULE_ASTCENC_ENABLED
        //     initialize_astcenc_module(p_level);
        // #endif
        // #if MODULE_BASIS_UNIVERSAL_ENABLED
        //     initialize_basis_universal_module(p_level);
        // #endif
        // #if MODULE_BMP_ENABLED
        //     initialize_bmp_module(p_level);
        // #endif
        // #if MODULE_CAMERA_ENABLED
        //     initialize_camera_module(p_level);
        // #endif
        // #if MODULE_CSG_ENABLED
        //     initialize_csg_module(p_level);
        // #endif
        // #if MODULE_CVTT_ENABLED
        //     initialize_cvtt_module(p_level);
        // #endif
        // #if MODULE_DDS_ENABLED
        //     initialize_dds_module(p_level);
        // #endif
        // #if MODULE_DENOISE_ENABLED
        //     initialize_denoise_module(p_level);
        // #endif
        // #if MODULE_ENET_ENABLED
        //     initialize_enet_module(p_level);
        // #endif
        // #if MODULE_ETCPAK_ENABLED
        //     initialize_etcpak_module(p_level);
        // #endif
        // #if MODULE_FREETYPE_ENABLED
        //     initialize_freetype_module(p_level);
        // #endif
        // #if MODULE_GDSCRIPT_ENABLED
        //     initialize_gdscript_module(p_level);
        // #endif
        // #if MODULE_GLSLANG_ENABLED
        //     initialize_glslang_module(p_level);
        // #endif
        // #if MODULE_GLTF_ENABLED
        //     initialize_gltf_module(p_level);
        // #endif
        // #if MODULE_GRIDMAP_ENABLED
        //     initialize_gridmap_module(p_level);
        // #endif
        // #if MODULE_HDR_ENABLED
        //     initialize_hdr_module(p_level);
        // #endif
        // #if MODULE_JPG_ENABLED
        //     initialize_jpg_module(p_level);
        // #endif
        // #if MODULE_JSONRPC_ENABLED
        //     initialize_jsonrpc_module(p_level);
        // #endif
        // #if MODULE_LIGHTMAPPER_RD_ENABLED
        //     initialize_lightmapper_rd_module(p_level);
        // #endif
        // #if MODULE_MBEDTLS_ENABLED
        //     initialize_mbedtls_module(p_level);
        // #endif
        // #if MODULE_MESHOPTIMIZER_ENABLED
        //     initialize_meshoptimizer_module(p_level);
        // #endif
        // #if MODULE_MINIMP3_ENABLED
        //     initialize_minimp3_module(p_level);
        // #endif
        // #if MODULE_MOBILE_VR_ENABLED
        //     initialize_mobile_vr_module(p_level);
        // #endif
        // #if MODULE_MONO_ENABLED
        //     initialize_mono_module(p_level);
        // #endif
        // #if MODULE_MSDFGEN_ENABLED
        //     initialize_msdfgen_module(p_level);
        // #endif
        // #if MODULE_MULTIPLAYER_ENABLED
        //     initialize_multiplayer_module(p_level);
        // #endif
        // #if MODULE_NAVIGATION_ENABLED
        //     initialize_navigation_module(p_level);
        // #endif
        // #if MODULE_NOISE_ENABLED
        //     initialize_noise_module(p_level);
        // #endif
        // #if MODULE_OGG_ENABLED
        //     initialize_ogg_module(p_level);
        // #endif
        // #if MODULE_OPENXR_ENABLED
        //     initialize_openxr_module(p_level);
        // #endif
        #if MODULE_RAYCAST_ENABLED
            InitializeRaycastModule(level);
        #endif
        // #if MODULE_REGEX_ENABLED
        //     initialize_regex_module(p_level);
        // #endif
        // #if MODULE_SQUISH_ENABLED
        //     initialize_squish_module(p_level);
        // #endif
        #if MODULE_SVG_ENABLED
            InitializeSvgModule(level);
        #endif
        #if MODULE_TEXT_SERVER_ADV_ENABLED
            InitializeTextServerAdvModule(level);
        #endif
        // #if MODULE_TEXT_SERVER_FB_ENABLED
        //     initialize_text_server_fb_module(p_level);
        // #endif
        // #if MODULE_TGA_ENABLED
        //     initialize_tga_module(p_level);
        // #endif
        // #if MODULE_THEORA_ENABLED
        //     initialize_theora_module(p_level);
        // #endif
        // #if MODULE_TINYEXR_ENABLED
        //     initialize_tinyexr_module(p_level);
        // #endif
        // #if MODULE_UPNP_ENABLED
        //     initialize_upnp_module(p_level);
        // #endif
        // #if MODULE_VHACD_ENABLED
        //     initialize_vhacd_module(p_level);
        // #endif
        // #if MODULE_VORBIS_ENABLED
        //     initialize_vorbis_module(p_level);
        // #endif
        // #if MODULE_WEBP_ENABLED
        //     initialize_webp_module(p_level);
        // #endif
        // #if MODULE_WEBRTC_ENABLED
        //     initialize_webrtc_module(p_level);
        // #endif
        // #if MODULE_WEBSOCKET_ENABLED
        //     initialize_websocket_module(p_level);
        // #endif
        // #if MODULE_WEBXR_ENABLED
        //     initialize_webxr_module(p_level);
        // #endif
        // #if MODULE_XATLAS_UNWRAP_ENABLED
        //     initialize_xatlas_unwrap_module(p_level);
        // #endif
        // #if MODULE_ZIP_ENABLED
        //     initialize_zip_module(p_level);
        // #endif
    }

    public static void UninitializeModules(ModuleInitializationLevel level)
    {
        // #if MODULE_ASTCENC_ENABLED
        //     uninitialize_astcenc_module(p_level);
        // #endif
        // #if MODULE_BASIS_UNIVERSAL_ENABLED
        //     uninitialize_basis_universal_module(p_level);
        // #endif
        // #if MODULE_BMP_ENABLED
        //     uninitialize_bmp_module(p_level);
        // #endif
        // #if MODULE_CAMERA_ENABLED
        //     uninitialize_camera_module(p_level);
        // #endif
        // #if MODULE_CSG_ENABLED
        //     uninitialize_csg_module(p_level);
        // #endif
        // #if MODULE_CVTT_ENABLED
        //     uninitialize_cvtt_module(p_level);
        // #endif
        // #if MODULE_DDS_ENABLED
        //     uninitialize_dds_module(p_level);
        // #endif
        // #if MODULE_DENOISE_ENABLED
        //     uninitialize_denoise_module(p_level);
        // #endif
        // #if MODULE_ENET_ENABLED
        //     uninitialize_enet_module(p_level);
        // #endif
        // #if MODULE_ETCPAK_ENABLED
        //     uninitialize_etcpak_module(p_level);
        // #endif
        // #if MODULE_FREETYPE_ENABLED
        //     uninitialize_freetype_module(p_level);
        // #endif
        // #if MODULE_GDSCRIPT_ENABLED
        //     uninitialize_gdscript_module(p_level);
        // #endif
        // #if MODULE_GLSLANG_ENABLED
        //     uninitialize_glslang_module(p_level);
        // #endif
        // #if MODULE_GLTF_ENABLED
        //     uninitialize_gltf_module(p_level);
        // #endif
        // #if MODULE_GRIDMAP_ENABLED
        //     uninitialize_gridmap_module(p_level);
        // #endif
        // #if MODULE_HDR_ENABLED
        //     uninitialize_hdr_module(p_level);
        // #endif
        // #if MODULE_JPG_ENABLED
        //     uninitialize_jpg_module(p_level);
        // #endif
        // #if MODULE_JSONRPC_ENABLED
        //     uninitialize_jsonrpc_module(p_level);
        // #endif
        // #if MODULE_LIGHTMAPPER_RD_ENABLED
        //     uninitialize_lightmapper_rd_module(p_level);
        // #endif
        // #if MODULE_MBEDTLS_ENABLED
        //     uninitialize_mbedtls_module(p_level);
        // #endif
        // #if MODULE_MESHOPTIMIZER_ENABLED
        //     uninitialize_meshoptimizer_module(p_level);
        // #endif
        // #if MODULE_MINIMP3_ENABLED
        //     uninitialize_minimp3_module(p_level);
        // #endif
        // #if MODULE_MOBILE_VR_ENABLED
        //     uninitialize_mobile_vr_module(p_level);
        // #endif
        // #if MODULE_MONO_ENABLED
        //     uninitialize_mono_module(p_level);
        // #endif
        // #if MODULE_MSDFGEN_ENABLED
        //     uninitialize_msdfgen_module(p_level);
        // #endif
        // #if MODULE_MULTIPLAYER_ENABLED
        //     uninitialize_multiplayer_module(p_level);
        // #endif
        // #if MODULE_NAVIGATION_ENABLED
        //     uninitialize_navigation_module(p_level);
        // #endif
        // #if MODULE_NOISE_ENABLED
        //     uninitialize_noise_module(p_level);
        // #endif
        // #if MODULE_OGG_ENABLED
        //     uninitialize_ogg_module(p_level);
        // #endif
        // #if MODULE_OPENXR_ENABLED
        //     uninitialize_openxr_module(p_level);
        // #endif
        #if MODULE_RAYCAST_ENABLED
            UninitializeRaycastModule(level);
        #endif
        // #if MODULE_REGEX_ENABLED
        //     uninitialize_regex_module(p_level);
        // #endif
        // #if MODULE_SQUISH_ENABLED
        //     uninitialize_squish_module(p_level);
        // #endif
        #if MODULE_SVG_ENABLED
            UninitializeSvgModule(level);
        #endif
        #if MODULE_TEXT_SERVER_ADV_ENABLED
            UninitializeTextServerAdvModule(level);
        #endif
        // #if MODULE_TEXT_SERVER_FB_ENABLED
        //     uninitialize_text_server_fb_module(p_level);
        // #endif
        // #if MODULE_TGA_ENABLED
        //     uninitialize_tga_module(p_level);
        // #endif
        // #if MODULE_THEORA_ENABLED
        //     uninitialize_theora_module(p_level);
        // #endif
        // #if MODULE_TINYEXR_ENABLED
        //     uninitialize_tinyexr_module(p_level);
        // #endif
        // #if MODULE_UPNP_ENABLED
        //     uninitialize_upnp_module(p_level);
        // #endif
        // #if MODULE_VHACD_ENABLED
        //     uninitialize_vhacd_module(p_level);
        // #endif
        // #if MODULE_VORBIS_ENABLED
        //     uninitialize_vorbis_module(p_level);
        // #endif
        // #if MODULE_WEBP_ENABLED
        //     uninitialize_webp_module(p_level);
        // #endif
        // #if MODULE_WEBRTC_ENABLED
        //     uninitialize_webrtc_module(p_level);
        // #endif
        // #if MODULE_WEBSOCKET_ENABLED
        //     uninitialize_websocket_module(p_level);
        // #endif
        // #if MODULE_WEBXR_ENABLED
        //     uninitialize_webxr_module(p_level);
        // #endif
        // #if MODULE_XATLAS_UNWRAP_ENABLED
        //     uninitialize_xatlas_unwrap_module(p_level);
        // #endif
        // #if MODULE_ZIP_ENABLED
        //     uninitialize_zip_module(p_level);
        // #endif
    }
}
