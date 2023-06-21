namespace Godot.Net.Servers;

using System;
using Godot.Net.Core.Config;
using Godot.Net.Core.IO;
using Godot.Net.Core.Math;
using Godot.Net.Core.Object;

public abstract partial class RenderingServer
{
    public const int ARRAY_WEIGHTS_SIZE            = 4;
    public const int CANVAS_ITEM_Z_MAX             = 4096;
    public const int CANVAS_ITEM_Z_MIN             = -4096;
    public const int MAX_2D_DIRECTIONAL_LIGHTS     = 8;
    public const int MAX_CURSORS                   = 8;
    public const int MAX_GLOW_LEVELS               = 7;
    public const int MAX_MESH_SURFACES             = 25;
    public const int NO_INDEX_ARRAY                = -1;
    public const int VIEWPORT_RENDER_INFO_MAX      = 3;
    public const int VIEWPORT_RENDER_INFO_TYPE_MAX = 2;

    public delegate void TextureDetectCallback(object? v1);
    public delegate void TextureDetectRoughnessCallback(object? v1, string? v2, TextureDetectRoughnessChannel v3);

    public event Action? PreFrameDraw;
    public event Action? PostFrameDraw;

    private static RS? singleton;
    public static RS Singleton => singleton ?? throw new NullReferenceException();

    public abstract Color        DefaultClearColor   { get; }
    public abstract bool         HasChanged          { get; }
    public abstract bool         IsLowEnd            { get; }
    public abstract Vector2<int> MaximumViewportSize { get; }
    public abstract string       VideoAdapterName    { get; }

    public bool PrintGpuProfile   { get; set; }
    public bool RenderLoopEnabled { get; set; }

    public RenderingServer() => singleton = this;

    protected void NotifyPreFrameDraw() => this.PreFrameDraw?.Invoke();
    protected void NotifyPostFrameDraw() => this.PostFrameDraw?.Invoke();

    #region public methods
    public virtual void Init()
    {
        GLOBAL_DEF_RST("rendering/textures/vram_compression/import_bptc", false);
        GLOBAL_DEF_RST("rendering/textures/vram_compression/import_s3tc", true);
        GLOBAL_DEF_RST("rendering/textures/vram_compression/import_etc", false);
        GLOBAL_DEF_RST("rendering/textures/vram_compression/import_etc2", true);

        GLOBAL_DEF("rendering/textures/lossless_compression/force_png", false);

        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/textures/webp_compression/compression_method", PropertyHint.PROPERTY_HINT_RANGE, "0,6,1"), 2);
        GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "rendering/textures/webp_compression/lossless_compression_factor", PropertyHint.PROPERTY_HINT_RANGE, "0,100,1"), 25);

        GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "rendering/limits/time/time_rollover_secs", PropertyHint.PROPERTY_HINT_RANGE, "0,10000,1,or_greater"), 3600);

        GLOBAL_DEF_RST("rendering/lights_and_shadows/use_physical_light_units", false);

        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/lights_and_shadows/directional_shadow/size", PropertyHint.PROPERTY_HINT_RANGE, "256,16384"), 4096);
        GLOBAL_DEF("rendering/lights_and_shadows/directional_shadow/size.mobile", 2048);
        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/lights_and_shadows/directional_shadow/soft_shadow_filter_quality", PropertyHint.PROPERTY_HINT_ENUM, "Hard (Fastest),Soft Very Low (Faster),Soft Low (Fast),Soft Medium (Average),Soft High (Slow),Soft Ultra (Slowest)"), 2);
        GLOBAL_DEF("rendering/lights_and_shadows/directional_shadow/soft_shadow_filter_quality.mobile", 0);
        GLOBAL_DEF("rendering/lights_and_shadows/directional_shadow/16_bits", true);

        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/lights_and_shadows/positional_shadow/soft_shadow_filter_quality", PropertyHint.PROPERTY_HINT_ENUM, "Hard (Fastest),Soft Very Low (Faster),Soft Low (Fast),Soft Medium (Average),Soft High (Slow),Soft Ultra (Slowest)"), 2);
        GLOBAL_DEF("rendering/lights_and_shadows/positional_shadow/soft_shadow_filter_quality.mobile", 0);

        GLOBAL_DEF("rendering/2d/shadow_atlas/size", 2048);

        // Number of commands that can be drawn per frame.
        GLOBAL_DEF_RST(new PropertyInfo(VariantType.INT, "rendering/gl_compatibility/item_buffer_size", PropertyHint.PROPERTY_HINT_RANGE, "1024,1048576,1"), 16384);

        GLOBAL_DEF("rendering/shader_compiler/shader_cache/enabled", true);
        GLOBAL_DEF("rendering/shader_compiler/shader_cache/compress", true);
        GLOBAL_DEF("rendering/shader_compiler/shader_cache/use_zstd_compression", true);
        GLOBAL_DEF("rendering/shader_compiler/shader_cache/strip_debug", false);
        GLOBAL_DEF("rendering/shader_compiler/shader_cache/strip_debug.release", true);

        GLOBAL_DEF_RST("rendering/reflections/sky_reflections/roughness_layers", 8); // Assumes a 256x256 cubemap
        GLOBAL_DEF_RST("rendering/reflections/sky_reflections/texture_array_reflections", true);
        GLOBAL_DEF("rendering/reflections/sky_reflections/texture_array_reflections.mobile", false);
        GLOBAL_DEF_RST("rendering/reflections/sky_reflections/ggx_samples", 32);
        GLOBAL_DEF("rendering/reflections/sky_reflections/ggx_samples.mobile", 16);
        GLOBAL_DEF("rendering/reflections/sky_reflections/fast_filter_high_quality", false);
        GLOBAL_DEF("rendering/reflections/reflection_atlas/reflection_size", 256);
        GLOBAL_DEF("rendering/reflections/reflection_atlas/reflection_size.mobile", 128);
        GLOBAL_DEF("rendering/reflections/reflection_atlas/reflection_count", 64);

        GLOBAL_DEF("rendering/global_illumination/gi/use_half_resolution", false);

        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/global_illumination/voxel_gi/quality", PropertyHint.PROPERTY_HINT_ENUM, "Low (4 Cones - Fast),High (6 Cones - Slow)"), 0);

        GLOBAL_DEF("rendering/shading/overrides/force_vertex_shading", false);
        GLOBAL_DEF("rendering/shading/overrides/force_vertex_shading.mobile", true);
        GLOBAL_DEF("rendering/shading/overrides/force_lambert_over_burley", false);
        GLOBAL_DEF("rendering/shading/overrides/force_lambert_over_burley.mobile", true);

        GLOBAL_DEF("rendering/driver/depth_prepass/enable", true);
        GLOBAL_DEF("rendering/driver/depth_prepass/disable_for_vendors", "PowerVR,Mali,Adreno,Apple");

        GLOBAL_DEF_RST("rendering/textures/default_filters/use_nearest_mipmap_filter", false);
        GLOBAL_DEF_RST(new PropertyInfo(VariantType.INT, "rendering/textures/default_filters/anisotropic_filtering_level", PropertyHint.PROPERTY_HINT_ENUM, "Disabled (Fastest),2× (Faster),4× (Fast),8× (Average),16× (Slow)"), 2);

        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/camera/depth_of_field/depth_of_field_bokeh_shape", PropertyHint.PROPERTY_HINT_ENUM, "Box (Fast),Hexagon (Average),Circle (Slowest)"), 1);
        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/camera/depth_of_field/depth_of_field_bokeh_quality", PropertyHint.PROPERTY_HINT_ENUM, "Very Low (Fastest),Low (Fast),Medium (Average),High (Slow)"), 1);
        GLOBAL_DEF("rendering/camera/depth_of_field/depth_of_field_use_jitter", false);

        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/environment/ssao/quality", PropertyHint.PROPERTY_HINT_ENUM, "Very Low (Fast),Low (Fast),Medium (Average),High (Slow),Ultra (Custom)"), 2);
        GLOBAL_DEF("rendering/environment/ssao/half_size", true);
        GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "rendering/environment/ssao/adaptive_target", PropertyHint.PROPERTY_HINT_RANGE, "0.0,1.0,0.01"), 0.5);
        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/environment/ssao/blur_passes", PropertyHint.PROPERTY_HINT_RANGE, "0,6"), 2);
        GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "rendering/environment/ssao/fadeout_from", PropertyHint.PROPERTY_HINT_RANGE, "0.0,512,0.1,or_greater"), 50.0);
        GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "rendering/environment/ssao/fadeout_to", PropertyHint.PROPERTY_HINT_RANGE, "64,65536,0.1,or_greater"), 300.0);

        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/environment/ssil/quality", PropertyHint.PROPERTY_HINT_ENUM, "Very Low (Fast),Low (Fast),Medium (Average),High (Slow),Ultra (Custom)"), 2);
        GLOBAL_DEF("rendering/environment/ssil/half_size", true);
        GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "rendering/environment/ssil/adaptive_target", PropertyHint.PROPERTY_HINT_RANGE, "0.0,1.0,0.01"), 0.5);
        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/environment/ssil/blur_passes", PropertyHint.PROPERTY_HINT_RANGE, "0,6"), 4);
        GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "rendering/environment/ssil/fadeout_from", PropertyHint.PROPERTY_HINT_RANGE, "0.0,512,0.1,or_greater"), 50.0);
        GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "rendering/environment/ssil/fadeout_to", PropertyHint.PROPERTY_HINT_RANGE, "64,65536,0.1,or_greater"), 300.0);

        GLOBAL_DEF("rendering/anti_aliasing/screen_space_roughness_limiter/enabled", true);
        GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "rendering/anti_aliasing/screen_space_roughness_limiter/amount", PropertyHint.PROPERTY_HINT_RANGE, "0.01,4.0,0.01"), 0.25);
        GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "rendering/anti_aliasing/screen_space_roughness_limiter/limit", PropertyHint.PROPERTY_HINT_RANGE, "0.01,1.0,0.01"), 0.18);

        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/scaling_3d/mode", PropertyHint.PROPERTY_HINT_ENUM, "Bilinear (Fastest),FSR 1.0 (Fast)"), 0);
        GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "rendering/scaling_3d/scale", PropertyHint.PROPERTY_HINT_RANGE, "0.25,2.0,0.01"), 1.0);
        GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "rendering/scaling_3d/fsr_sharpness", PropertyHint.PROPERTY_HINT_RANGE, "0,2,0.1"), 0.2f);
        GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "rendering/textures/default_filters/texture_mipmap_bias", PropertyHint.PROPERTY_HINT_RANGE, "-2,2,0.001"), 0.0f);

        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/textures/decals/filter", PropertyHint.PROPERTY_HINT_ENUM, "Nearest (Fast),Linear (Fast),Nearest Mipmap (Fast),Linear Mipmap (Fast),Nearest Mipmap Anisotropic (Average),Linear Mipmap Anisotropic (Average)"), DecalFilter.DECAL_FILTER_LINEAR_MIPMAPS);
        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/textures/light_projectors/filter", PropertyHint.PROPERTY_HINT_ENUM, "Nearest (Fast),Linear (Fast),Nearest Mipmap (Fast),Linear Mipmap (Fast),Nearest Mipmap Anisotropic (Average),Linear Mipmap Anisotropic (Average)"), LightProjectorFilter.LIGHT_PROJECTOR_FILTER_LINEAR_MIPMAPS);

        GLOBAL_DEF_RST("rendering/occlusion_culling/occlusion_rays_per_thread", 512);

        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/environment/glow/upscale_mode", PropertyHint.PROPERTY_HINT_ENUM, "Linear (Fast),Bicubic (Slow)"), 1);
        GLOBAL_DEF("rendering/environment/glow/upscale_mode.mobile", 0);

        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/environment/screen_space_reflection/roughness_quality", PropertyHint.PROPERTY_HINT_ENUM, "Disabled (Fastest),Low (Fast),Medium (Average),High (Slow)"), 1);

        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/environment/subsurface_scattering/subsurface_scattering_quality", PropertyHint.PROPERTY_HINT_ENUM, "Disabled (Fastest),Low (Fast),Medium (Average),High (Slow)"), 1);
        GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "rendering/environment/subsurface_scattering/subsurface_scattering_scale", PropertyHint.PROPERTY_HINT_RANGE, "0.001,1,0.001"), 0.05);
        GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "rendering/environment/subsurface_scattering/subsurface_scattering_depth_scale", PropertyHint.PROPERTY_HINT_RANGE, "0.001,1,0.001"), 0.01);

        GLOBAL_DEF("rendering/limits/global_shader_variables/buffer_size", 65536);

        GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "rendering/lightmapping/probe_capture/update_speed", PropertyHint.PROPERTY_HINT_RANGE, "0.001,256,0.001"), 15);

        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/global_illumination/sdfgi/probe_ray_count", PropertyHint.PROPERTY_HINT_ENUM, "8 (Fastest),16,32,64,96,128 (Slowest)"), 1);
        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/global_illumination/sdfgi/frames_to_converge", PropertyHint.PROPERTY_HINT_ENUM, "5 (Less Latency but Lower Quality),10,15,20,25,30 (More Latency but Higher Quality)"), 5);
        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/global_illumination/sdfgi/frames_to_update_lights", PropertyHint.PROPERTY_HINT_ENUM, "1 (Slower),2,4,8,16 (Faster)"), 2);

        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/environment/volumetric_fog/volume_size", PropertyHint.PROPERTY_HINT_RANGE, "16,512,1"), 64);
        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/environment/volumetric_fog/volume_depth", PropertyHint.PROPERTY_HINT_RANGE, "16,512,1"), 64);
        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/environment/volumetric_fog/use_filter", PropertyHint.PROPERTY_HINT_ENUM, "No (Faster),Yes (Higher Quality)"), 1);

        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/limits/spatial_indexer/update_iterations_per_frame", PropertyHint.PROPERTY_HINT_RANGE, "0,1024,1"), 10);
        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/limits/spatial_indexer/threaded_cull_minimum_instances", PropertyHint.PROPERTY_HINT_RANGE, "32,65536,1"), 1000);
        GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/limits/forward_renderer/threaded_render_minimum_instances", PropertyHint.PROPERTY_HINT_RANGE, "32,65536,1"), 500);

        GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "rendering/limits/cluster_builder/max_clustered_elements", PropertyHint.PROPERTY_HINT_RANGE, "32,8192,1"), 512);

        // OpenGL limits
        GLOBAL_DEF_RST(new PropertyInfo(VariantType.INT, "rendering/limits/opengl/max_renderable_elements", PropertyHint.PROPERTY_HINT_RANGE, "1024,65536,1"), 65536);
        GLOBAL_DEF_RST(new PropertyInfo(VariantType.INT, "rendering/limits/opengl/max_renderable_lights", PropertyHint.PROPERTY_HINT_RANGE, "2,256,1"), 32);
        GLOBAL_DEF_RST(new PropertyInfo(VariantType.INT, "rendering/limits/opengl/max_lights_per_object", PropertyHint.PROPERTY_HINT_RANGE, "2,1024,1"), 8);

        GLOBAL_DEF_RST_BASIC("xr/shaders/enabled", false);
    }
    #endregion public methods

    #region public abstract methods
    public abstract Guid CanvasCreate();
    public abstract void CanvasItemAddRect(Guid item, in Rect2<RealT> rect, in Color color);
    public abstract void CanvasItemAddTriangleArray(Guid item, IList<int> indices, IList<Vector2<RealT>> points, IList<Color> colors, IList<Vector2<RealT>>? uvs = default, IList<int>? bones = default, IList<float>? weights = default, Guid texture = default, int count = -1);
    public abstract void CanvasItemClear(Guid canvasItemId);
    public abstract Guid CanvasItemCreate();
    public abstract void CanvasItemSetClip(Guid item, bool clip);
    public abstract void CanvasItemSetCustomRect(Guid item, bool customRect, Rect2<RealT> rect = default);
    public abstract void CanvasItemSetDefaultTextureFilter(Guid canvasItemId, CanvasItemTextureFilter textureFilter);
    public abstract void CanvasItemSetDefaultTextureRepeat(Guid canvasItemId, CanvasItemTextureRepeat textureRepeat);
    public abstract void CanvasItemSetDrawIndex(Guid canvasItemId, int drawIndex);
    public abstract void CanvasItemSetParent(Guid canvasItemId, Guid canvasItemParentId);
    public abstract void CanvasItemSetTransform(Guid item, Transform2D<RealT> transform);
    public abstract void CanvasItemSetVisibilityLayer(Guid canvasItemId, uint visibilityLayer);
    public abstract void CanvasItemSetVisible(Guid canvasItemId, bool visible);
    public abstract void Draw(bool swapBuffers, double frameStep);
    public abstract void Finish();
    public abstract Guid ScenarioCreate();
    public abstract void ScenarioSetFallbackEnvironment(Guid scenarioId, Guid fallbackEnvironment);
    public abstract void SetDefaultClearColor(in Color color);
    public abstract void Sync();
    public abstract Guid Texture2DCreate(Image image);
    public abstract Guid TextureProxyCreate(Ref<Guid> textureId);
    public abstract void TextureReplace(Guid texture, Guid byTexture);
    public abstract void ViewportAttachCanvas(Guid viewportId, Guid currentCanvas);
    public abstract void ViewportAttachToScreen(Guid viewportId, Rect2<int> screenRect, int window);
    public abstract Guid ViewportCreate();
    public abstract Ref<Guid> ViewportGetTexture(Guid viewportId);
    public abstract void ViewportSetActive(Guid viewportId, bool active);
    public abstract void ViewportSetCanvasCullMask(Guid viewportId, uint canvasCullMask);
    public abstract void ViewportSetCanvasTransform(Guid viewportId, Guid canvasId, Transform2D<RealT> offset);
    public abstract void ViewportSetFsrSharpness(Guid viewportId, float fsrSharpness);
    public abstract void ViewportSetGlobalCanvasTransform(Guid viewportId, Transform2D<RealT> transform);
    public abstract void ViewportSetMeshLodThreshold(Guid viewportId, float meshLodThreshold);
    public abstract void ViewportSetMsaa2D(Guid viewportId, ViewportMSAA msaa);
    public abstract void ViewportSetMsaa3D(Guid viewportId, ViewportMSAA msaa);
    public abstract void ViewportSetParentViewport(Guid viewportId, Guid parentViewportId);
    public abstract void ViewportSetPositionalShadowAtlasQuadrantSubdivision(Guid viewportId, int quadrant, int subdiv);
    public abstract void ViewportSetPositionalShadowAtlasSize(Guid viewportId, int size, bool positionalShadowAtlasSize);
    public abstract void ViewportSetScaling3DMode(Guid viewportId, ViewportScaling3DMode scaling3DMode);
    public abstract void ViewportSetScaling3DScale(Guid viewportId, float scaling3DScale);
    public abstract void ViewportSetScenario(Guid viewportId, Guid scenarioId);
    public abstract void ViewportSetScreenSpaceAA(Guid viewportId, ViewportScreenSpaceAA screenSpaceAA);
    public abstract void ViewportSetSdfOversizeAndScale(Guid viewportId, ViewportSDFOversize sdfOversize, ViewportSDFScale sdfScale);
    public abstract void ViewportSetSize(Guid viewportId, int width, int height);
    public abstract void ViewportSetSnap2DTransformsToPixel(Guid viewportId, bool snap2DTransformsToPixel);
    public abstract void ViewportSetSnap2DVerticesToPixel(Guid viewportId, bool snap2DVerticesToPixel);
    public abstract void ViewportSetTextureMipmapBias(Guid viewportId, float textureMipmapBias);
    public abstract void ViewportSetTransparentBackground(Guid viewportId, bool transparentBackground);
    public abstract void ViewportSetUpdateMode(Guid viewportId, ViewportUpdateMode updateMode);
    public abstract void ViewportSetUseDebanding(Guid viewportId, bool useDebanding);
    public abstract void ViewportSetUseOcclusionCulling(Guid viewportId, bool useOcclusionCulling);
    public abstract void ViewportSetUseTAA(Guid viewportId, bool useTAA);
    public abstract void ViewportSetVrsMode(Guid viewportId, ViewportVRSMode vrsMode);
    #endregion public abstract methods
}
