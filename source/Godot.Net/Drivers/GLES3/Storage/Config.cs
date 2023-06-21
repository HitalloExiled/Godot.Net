#define GLES_OVER_GL
// #define ANDROID_ENABLED
// #define IOS_ENABLED

namespace Godot.Net.Drivers.GLES3.Storage;

using Godot.Net.Drivers.GLES3.Api.Enums;

#if ANDROID_ENABLED
using Godot.Net.Drivers.GLES3.Api.Extensions.OVR;
#endif

#pragma warning disable IDE0044,IDE0052,CS0414 // TODO Remove

public class Config
{

    private static Config? singleton;

    public static Config Singleton => singleton ?? throw new NullReferenceException();

    private readonly HashSet<string> extensions;

    public float AnisotropicLevel           { get; set; }
    public bool  AstcHdrSupported           { get; set; }
    public bool  AstcLayeredSupported       { get; set; }
    public bool  AstcSupported              { get; set; }
    public bool  BptcSupported              { get; set; }
    public bool  Etc2Supported              { get; set; }
    public bool  FloatTextureSupported      { get; set; }
    public bool  ForceVertexShading         { get; set; }
    public int   MaxLightsPerObject         { get; set; }
    public int   MaxRenderableElements      { get; set; }
    public int   MaxRenderableLights        { get; set; }
    public int   MaxTextureImageUnits       { get; set; }
    public int   MaxTextureSize             { get; set; }
    public int   MaxUniformBufferSize       { get; set; }
    public int   MaxVertexTextureImageUnits { get; set; }
    public int[] MaxViewportSize            { get; set; } = new int[2];
    public bool  MultiviewSupported         { get; set; }
    public bool  RgtcSupported              { get; set; }
    public bool  S3tcSupported              { get; set; }
    public bool  SupportAnisotropicFilter   { get; set; }
    public bool  UseDepthPrepass            { get; set; }
    public bool  UseNearestMifilter         { get; set; }
    public bool  UseNearestMipFilter        { get; set; }

    public Config()
    {
        singleton = this;

        var gl = GL.Singleton;

        this.extensions = gl.GetExtensions();

        this.BptcSupported        = this.extensions.Contains("GL_ARB_texture_compression_bptc") || this.extensions.Contains("EXT_texture_compression_bptc");
        this.AstcSupported        = this.extensions.Contains("GL_KHR_texture_compression_astc") || this.extensions.Contains("GL_OES_texture_compression_astc") || this.extensions.Contains("GL_KHR_texture_compression_astc_ldr") || this.extensions.Contains("GL_KHR_texture_compression_astc_hdr");
        this.AstcHdrSupported     = this.extensions.Contains("GL_KHR_texture_compression_astc_ldr");
        this.AstcLayeredSupported = this.extensions.Contains("GL_KHR_texture_compression_astc_sliced_3d");

        #if GLES_OVER_GL
        this.FloatTextureSupported = true;
        this.Etc2Supported         = false;
        this.S3tcSupported         = true;
        this.RgtcSupported         = true; //RGTC - core since OpenGL version 3.0
        #else
        this.floatTextureSupported = this.extensions.Contains("GL_EXT_color_buffer_float");
        this.etc2Supported         = true;
        #if ANDROID_ENABLED || IOS_ENABLED
        // Some Android devices report support for S3TC but we don't expect that and don't export the textures.
        // This could be fixed but so few devices support it that it doesn't seem useful (and makes bigger APKs).
        // For good measure we do the same hack for iOS, just in case.
        this.s3tcSupported = false;
        #else
        this.s3tcSupported = this.extensions.Contains("GL_EXT_texture_compression_dxt1") || this.extensions.Contains("GL_EXT_texture_compression_s3tc") || this.extensions.Contains("WEBGL_compressed_texture_s3tc");
        #endif
        this.rgtcSupported = this.extensions.Contains("GL_EXT_texture_compression_rgtc") || this.extensions.Contains("GL_ARB_texture_compression_rgtc") || this.extensions.Contains("EXT_texture_compression_rgtc");
        #endif

        var maxViewportSize = new int[2];

        gl.GetIntegerv(GetPName.MaxVertexTextureImageUnits, out int maxVertexTextureImageUnits);
        gl.GetIntegerv(GetPName.MaxTextureImageUnits,       out int maxTextureImageUnits);
        gl.GetIntegerv(GetPName.MaxTextureSize,             out int maxTextureSize);
        gl.GetIntegerv(GetPName.MaxUniformBlockSize,        out int maxUniformBufferSize);
        gl.GetIntegerv(GetPName.MaxViewportDims,            out     maxViewportSize);

        this.MaxVertexTextureImageUnits = maxVertexTextureImageUnits;
        this.MaxTextureImageUnits       = maxTextureImageUnits;
        this.MaxTextureSize             = maxTextureSize;
        this.MaxUniformBufferSize       = maxUniformBufferSize;
        this.MaxViewportSize            = maxViewportSize;

        this.SupportAnisotropicFilter = this.extensions.Contains("GL_EXT_texture_filter_anisotropic");
        if (this.SupportAnisotropicFilter)
        {
            gl.GetFloatv(GetPName.MaxTextureMaxAnisotropyEXT, out float anisotropicLevel);
            this.AnisotropicLevel = Math.Min(1 << GLOBAL_GET<int>("rendering/textures/default_filters/anisotropic_filtering_level"), anisotropicLevel);
        }

        this.MultiviewSupported = this.extensions.Contains("GL_OVR_multiview2") || this.extensions.Contains("GL_OVR_multiview");
        #if ANDROID_ENABLED
        this.multiviewSupported = this.multiviewSupported && gl.TryGetExtension<GlOvrMultiview>(out var _);
        #endif

        this.ForceVertexShading  = false; //GLOBAL_GET("rendering/quality/shading/force_vertex_shading");
        this.UseNearestMipFilter = GLOBAL_GET<bool>("rendering/textures/default_filters/use_nearest_mipmap_filter");

        this.UseDepthPrepass = GLOBAL_GET<bool>("rendering/driver/depth_prepass/enable");
        if (this.UseDepthPrepass)
        {
            var vendors = GLOBAL_GET<string>("rendering/driver/depth_prepass/disable_for_vendors");

            var vendorMatch = vendors?.Split(",");
            var renderer = gl.GetString(StringName.Renderer);

            for (var i = 0; i < vendorMatch!.Length; i++)
            {
                var v = vendorMatch[i].Trim();
                if (v == null)
                {
                    continue;
                }

                if (renderer!.IndexOf(v) != -1)
                {
                    this.UseDepthPrepass = false;
                }
            }
        }

        this.MaxRenderableElements = GLOBAL_GET<int>("rendering/limits/opengl/max_renderable_elements");
        this.MaxRenderableLights   = GLOBAL_GET<int>("rendering/limits/opengl/max_renderable_lights");
        this.MaxLightsPerObject    = GLOBAL_GET<int>("rendering/limits/opengl/max_lights_per_object");
    }
}
