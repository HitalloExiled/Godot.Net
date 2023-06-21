namespace Godot.Net.Drivers.GLES3.Storage;

using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Godot.Net.Core;
using Godot.Net.Core.Config;
using Godot.Net.Core.Generics;
using Godot.Net.Drivers.GLES3.Api.Enums;
using Godot.Net.Servers.Rendering;
using Godot.Net.Servers.Rendering.Storage;

public partial class MaterialStorage : RendererMaterialStorage
{
    public const ushort PARTICLES_MAX_USERDATAS = 6;

    public delegate MaterialData MaterialDataRequestFunction(ShaderData shaderData);
    public delegate ShaderData ShaderDataRequestFunction();

    private static MaterialStorage? singleton;

    public static MaterialStorage Singleton => singleton ?? throw new NullReferenceException();

    private readonly GlobalShaderUniforms           globalShaderUniforms    = new();
    private readonly MaterialDataRequestFunction?[] materialDataRequestFunc = new MaterialDataRequestFunction[(int)RS.ShaderMode.SHADER_MAX];
    private readonly GuidOwner<Material>            materialOwner           = new(true);
    private readonly SelfList<Material>.List        materialUpdateList      = new();
    private readonly ShaderDataRequestFunction?[]   shaderDataRequestFunc   = new ShaderDataRequestFunction[(int)RS.ShaderMode.SHADER_MAX];
    private readonly GuidOwner<Shader>              shaderOwner             = new(true);

    public ShadersRecord Shaders { get; } = new();

    public MaterialStorage()
    {
        singleton = this;

        var gl = GL.Singleton;

        this.shaderDataRequestFunc[(int)RS.ShaderMode.SHADER_SPATIAL]     = this.CreateSceneShaderFunc;
        this.shaderDataRequestFunc[(int)RS.ShaderMode.SHADER_CANVAS_ITEM] = this.CreateCanvasShaderFunc;
        this.shaderDataRequestFunc[(int)RS.ShaderMode.SHADER_PARTICLES]   = this.CreateParticlesShaderFunc;
        this.shaderDataRequestFunc[(int)RS.ShaderMode.SHADER_SKY]         = this.CreateSkyShaderFunc;
        this.shaderDataRequestFunc[(int)RS.ShaderMode.SHADER_FOG]         = null;

        this.materialDataRequestFunc[(int)RS.ShaderMode.SHADER_SPATIAL]     = this.CreateSceneMaterialFunc;
        this.materialDataRequestFunc[(int)RS.ShaderMode.SHADER_CANVAS_ITEM] = this.CreateCanvasMaterialFunc;
        this.materialDataRequestFunc[(int)RS.ShaderMode.SHADER_PARTICLES]   = this.CreateParticlesMaterialFunc;
        this.materialDataRequestFunc[(int)RS.ShaderMode.SHADER_SKY]         = this.CreateSkyMaterialFunc;
        this.materialDataRequestFunc[(int)RS.ShaderMode.SHADER_FOG]         = null;

        Contract.Assert(Marshal.SizeOf<GlobalShaderUniforms.Value>() == 16);

        this.globalShaderUniforms.BufferSize = (uint)Math.Max(4096, GLOBAL_GET<int>("rendering/limits/global_shader_variables/buffer_size"));

        if (this.globalShaderUniforms.BufferSize > (uint)Config.Singleton.MaxUniformBufferSize)
        {
            this.globalShaderUniforms.BufferSize = (uint)Config.Singleton.MaxUniformBufferSize;
            WARN_PRINT("Project setting: rendering/limits/global_shader_variables/buffer_size exceeds maximum uniform buffer size of: " + Config.Singleton.MaxUniformBufferSize);
        }

        this.globalShaderUniforms.BufferValues       = Common.FillArray<GlobalShaderUniforms.Value>(this.globalShaderUniforms.BufferSize);
        this.globalShaderUniforms.BufferUsage        = Common.FillArray<GlobalShaderUniforms.ValueUsage>(this.globalShaderUniforms.BufferSize);
        this.globalShaderUniforms.BufferDirtyRegions = new bool[this.globalShaderUniforms.BufferSize / GlobalShaderUniforms.BUFFER_DIRTY_REGION_SIZE];

        gl.GenBuffers(out var buffer);
        this.globalShaderUniforms.Buffer = buffer;

        gl.BindBuffer(BufferTargetARB.UniformBuffer, this.globalShaderUniforms.Buffer);
        gl.BufferData(BufferTargetARB.UniformBuffer, (uint)(Marshal.SizeOf<GlobalShaderUniforms.Value>() * this.globalShaderUniforms.BufferSize), default, BufferUsageARB.DynamicDraw);
        gl.BindBuffer(BufferTargetARB.UniformBuffer, 0);

        // Setup CanvasItem compiler
        var canvasItemActions = new ShaderCompiler.DefaultIdentifierActions();

        canvasItemActions.Renames.Add("VERTEX",        "vertex");
        canvasItemActions.Renames.Add("LIGHT_VERTEX",  "light_vertex");
        canvasItemActions.Renames.Add("SHADOW_VERTEX", "shadow_vertex");
        canvasItemActions.Renames.Add("UV",            "uv");
        canvasItemActions.Renames.Add("POINT_SIZE",    "gl_PointSize");

        canvasItemActions.Renames.Add("MODEL_MATRIX",    "model_matrix");
        canvasItemActions.Renames.Add("CANVAS_MATRIX",   "canvas_transform");
        canvasItemActions.Renames.Add("SCREEN_MATRIX",   "screen_transform");
        canvasItemActions.Renames.Add("TIME",            "time");
        canvasItemActions.Renames.Add("PI",              Math.PI.ToString());
        canvasItemActions.Renames.Add("TAU",             Math.Tau.ToString());
        canvasItemActions.Renames.Add("E",               Math.E.ToString());
        canvasItemActions.Renames.Add("AT_LIGHT_PASS",   "false");
        canvasItemActions.Renames.Add("INSTANCE_CUSTOM", "instance_custom");

        canvasItemActions.Renames.Add("COLOR",                      "color");
        canvasItemActions.Renames.Add("NORMAL",                     "normal");
        canvasItemActions.Renames.Add("NORMAL_MAP",                 "normal_map");
        canvasItemActions.Renames.Add("NORMAL_MAP_DEPTH",           "normal_map_depth");
        canvasItemActions.Renames.Add("TEXTURE",                    "color_texture");
        canvasItemActions.Renames.Add("TEXTURE_PIXEL_SIZE",         "color_texture_pixel_size");
        canvasItemActions.Renames.Add("NORMAL_TEXTURE",             "normal_texture");
        canvasItemActions.Renames.Add("SPECULAR_SHININESS_TEXTURE", "specular_texture");
        canvasItemActions.Renames.Add("SPECULAR_SHININESS",         "specular_shininess");
        canvasItemActions.Renames.Add("SCREEN_UV",                  "screen_uv");
        canvasItemActions.Renames.Add("SCREEN_PIXEL_SIZE",          "screen_pixel_size");
        canvasItemActions.Renames.Add("FRAGCOORD",                  "gl_FragCoord");
        canvasItemActions.Renames.Add("POINT_COORD",                "gl_PointCoord");
        canvasItemActions.Renames.Add("INSTANCE_ID",                "gl_InstanceIndex");
        canvasItemActions.Renames.Add("VERTEX_ID",                  "gl_VertexIndex");

        canvasItemActions.Renames.Add("LIGHT_POSITION",       "light_position");
        canvasItemActions.Renames.Add("LIGHT_DIRECTION",      "light_direction");
        canvasItemActions.Renames.Add("LIGHT_IS_DIRECTIONAL", "is_directional");
        canvasItemActions.Renames.Add("LIGHT_COLOR",          "light_color");
        canvasItemActions.Renames.Add("LIGHT_ENERGY",         "light_energy");
        canvasItemActions.Renames.Add("LIGHT",                "light");
        canvasItemActions.Renames.Add("SHADOW_MODULATE",      "shadow_modulate");

        canvasItemActions.Renames.Add("texture_sdf",        "texture_sdf");
        canvasItemActions.Renames.Add("texture_sdf_normal", "texture_sdf_normal");
        canvasItemActions.Renames.Add("sdf_to_screen_uv",   "sdf_to_screen_uv");
        canvasItemActions.Renames.Add("screen_uv_to_sdf",   "screen_uv_to_sdf");

        canvasItemActions.UsageDefines.Add("COLOR",              "#define COLOR_USED\n");
        canvasItemActions.UsageDefines.Add("SCREEN_UV",          "#define SCREEN_UV_USED\n");
        canvasItemActions.UsageDefines.Add("SCREEN_PIXEL_SIZE",  "@SCREEN_UV");
        canvasItemActions.UsageDefines.Add("NORMAL",             "#define NORMAL_USED\n");
        canvasItemActions.UsageDefines.Add("NORMAL_MAP",         "#define NORMAL_MAP_USED\n");
        canvasItemActions.UsageDefines.Add("LIGHT",              "#define LIGHT_SHADER_CODE_USED\n");
        canvasItemActions.UsageDefines.Add("SPECULAR_SHININESS", "#define SPECULAR_SHININESS_USED\n");

        canvasItemActions.RenderModeDefines.Add("skip_vertex_transform", "#define SKIP_TRANSFORM_USED\n");
        canvasItemActions.RenderModeDefines.Add("unshaded",              "#define MODE_UNSHADED\n");
        canvasItemActions.RenderModeDefines.Add("light_only",            "#define MODE_LIGHT_ONLY\n");

        canvasItemActions.GlobalBufferArrayVariable = "global_shader_uniforms";

        this.Shaders.CompilerCanvas.Initialize(canvasItemActions);

        // Setup Scene compiler

        //shader compiler
        var sceneActions = new ShaderCompiler.DefaultIdentifierActions();

        sceneActions.Renames.Add("MODEL_MATRIX",            "model_matrix");
        sceneActions.Renames.Add("MODEL_NORMAL_MATRIX",     "model_normal_matrix");
        sceneActions.Renames.Add("VIEW_MATRIX",             "scene_data.view_matrix");
        sceneActions.Renames.Add("INV_VIEW_MATRIX",         "scene_data.inv_view_matrix");
        sceneActions.Renames.Add("PROJECTION_MATRIX",       "projection_matrix");
        sceneActions.Renames.Add("INV_PROJECTION_MATRIX",   "inv_projection_matrix");
        sceneActions.Renames.Add("MODELVIEW_MATRIX",        "modelview");
        sceneActions.Renames.Add("MODELVIEW_NORMAL_MATRIX", "modelview_normal");

        sceneActions.Renames.Add("VERTEX",      "vertex");
        sceneActions.Renames.Add("NORMAL",      "normal");
        sceneActions.Renames.Add("TANGENT",     "tangent");
        sceneActions.Renames.Add("BINORMAL",    "binormal");
        sceneActions.Renames.Add("POSITION",    "position");
        sceneActions.Renames.Add("UV",          "uv_interp");
        sceneActions.Renames.Add("UV2",         "uv2_interp");
        sceneActions.Renames.Add("COLOR",       "color_interp");
        sceneActions.Renames.Add("POINT_SIZE",  "gl_PointSize");
        sceneActions.Renames.Add("INSTANCE_ID", "gl_InstanceID");
        sceneActions.Renames.Add("VERTEX_ID",   "gl_VertexID");

        sceneActions.Renames.Add("ALPHA_SCISSOR_THRESHOLD",  "alpha_scissor_threshold");
        sceneActions.Renames.Add("ALPHA_HASH_SCALE",         "alpha_hash_scale");
        sceneActions.Renames.Add("ALPHA_ANTIALIASING_EDGE",  "alpha_antialiasing_edge");
        sceneActions.Renames.Add("ALPHA_TEXTURE_COORDINATE", "alpha_texture_coordinate");

        //builtins

        sceneActions.Renames.Add("TIME",          "scene_data.time");
        sceneActions.Renames.Add("PI",            Math.PI.ToString());
        sceneActions.Renames.Add("TAU",           Math.Tau.ToString());
        sceneActions.Renames.Add("E",             Math.E.ToString());
        sceneActions.Renames.Add("VIEWPORT_SIZE", "scene_data.viewport_size");

        sceneActions.Renames.Add("FRAGCOORD",               "gl_FragCoord");
        sceneActions.Renames.Add("FRONT_FACING",            "gl_FrontFacing");
        sceneActions.Renames.Add("NORMAL_MAP",              "normal_map");
        sceneActions.Renames.Add("NORMAL_MAP_DEPTH",        "normal_map_depth");
        sceneActions.Renames.Add("ALBEDO",                  "albedo");
        sceneActions.Renames.Add("ALPHA",                   "alpha");
        sceneActions.Renames.Add("METALLIC",                "metallic");
        sceneActions.Renames.Add("SPECULAR",                "specular");
        sceneActions.Renames.Add("ROUGHNESS",               "roughness");
        sceneActions.Renames.Add("RIM",                     "rim");
        sceneActions.Renames.Add("RIM_TINT",                "rim_tint");
        sceneActions.Renames.Add("CLEARCOAT",               "clearcoat");
        sceneActions.Renames.Add("CLEARCOAT_ROUGHNESS",     "clearcoat_roughness");
        sceneActions.Renames.Add("ANISOTROPY",              "anisotropy");
        sceneActions.Renames.Add("ANISOTROPY_FLOW",         "anisotropy_flow");
        sceneActions.Renames.Add("SSS_STRENGTH",            "sss_strength");
        sceneActions.Renames.Add("SSS_TRANSMITTANCE_COLOR", "transmittance_color");
        sceneActions.Renames.Add("SSS_TRANSMITTANCE_DEPTH", "transmittance_depth");
        sceneActions.Renames.Add("SSS_TRANSMITTANCE_BOOST", "transmittance_boost");
        sceneActions.Renames.Add("BACKLIGHT",               "backlight");
        sceneActions.Renames.Add("AO",                      "ao");
        sceneActions.Renames.Add("AO_LIGHT_AFFECT",         "ao_light_affect");
        sceneActions.Renames.Add("EMISSION",                "emission");
        sceneActions.Renames.Add("POINT_COORD",             "gl_PointCoord");
        sceneActions.Renames.Add("INSTANCE_CUSTOM",         "instance_custom");
        sceneActions.Renames.Add("SCREEN_UV",               "screen_uv");
        sceneActions.Renames.Add("DEPTH",                   "gl_FragDepth");
        // sceneActions.Renames.Add("OUTPUT_IS_SRGB", "true"); // Overrided
        sceneActions.Renames.Add("FOG",            "fog");
        sceneActions.Renames.Add("RADIANCE",       "custom_radiance");
        sceneActions.Renames.Add("IRRADIANCE",     "custom_irradiance");
        sceneActions.Renames.Add("BONE_INDICES",   "bone_attrib");
        sceneActions.Renames.Add("BONE_WEIGHTS",   "weight_attrib");
        sceneActions.Renames.Add("CUSTOM0",        "custom0_attrib");
        sceneActions.Renames.Add("CUSTOM1",        "custom1_attrib");
        sceneActions.Renames.Add("CUSTOM2",        "custom2_attrib");
        sceneActions.Renames.Add("CUSTOM3",        "custom3_attrib");
        sceneActions.Renames.Add("OUTPUT_IS_SRGB", "SHADER_IS_SRGB");

        sceneActions.Renames.Add("NODE_POSITION_WORLD",    "model_matrix[3].xyz");
        sceneActions.Renames.Add("CAMERA_POSITION_WORLD",  "scene_data.inv_view_matrix[3].xyz");
        sceneActions.Renames.Add("CAMERA_DIRECTION_WORLD", "scene_data.view_matrix[3].xyz");
        sceneActions.Renames.Add("CAMERA_VISIBLE_LAYERS",  "scene_data.camera_visible_layers");
        sceneActions.Renames.Add("NODE_POSITION_VIEW",     "(model_matrix * scene_data.view_matrix)[3].xyz");

        sceneActions.Renames.Add("VIEW_INDEX",     "ViewIndex");
        sceneActions.Renames.Add("VIEW_MONO_LEFT", "uint(0)");
        sceneActions.Renames.Add("VIEW_RIGHT",     "uint(1)");
        sceneActions.Renames.Add("EYE_OFFSET",     "eye_offset");

        //for light
        sceneActions.Renames.Add("VIEW",           "view");
        sceneActions.Renames.Add("LIGHT_COLOR",    "light_color");
        sceneActions.Renames.Add("LIGHT",          "light");
        sceneActions.Renames.Add("ATTENUATION",    "attenuation");
        sceneActions.Renames.Add("DIFFUSE_LIGHT",  "diffuse_light");
        sceneActions.Renames.Add("SPECULAR_LIGHT", "specular_light");

        sceneActions.UsageDefines.Add("NORMAL",              "#define NORMAL_USED\n");
        sceneActions.UsageDefines.Add("TANGENT",             "#define TANGENT_USED\n");
        sceneActions.UsageDefines.Add("BINORMAL",            "@TANGENT");
        sceneActions.UsageDefines.Add("RIM",                 "#define LIGHT_RIM_USED\n");
        sceneActions.UsageDefines.Add("RIM_TINT",            "@RIM");
        sceneActions.UsageDefines.Add("CLEARCOAT",           "#define LIGHT_CLEARCOAT_USED\n");
        sceneActions.UsageDefines.Add("CLEARCOAT_ROUGHNESS", "@CLEARCOAT");
        sceneActions.UsageDefines.Add("ANISOTROPY",          "#define LIGHT_ANISOTROPY_USED\n");
        sceneActions.UsageDefines.Add("ANISOTROPY_FLOW",     "@ANISOTROPY");
        sceneActions.UsageDefines.Add("AO",                  "#define AO_USED\n");
        sceneActions.UsageDefines.Add("AO_LIGHT_AFFECT",     "#define AO_USED\n");
        sceneActions.UsageDefines.Add("UV",                  "#define UV_USED\n");
        sceneActions.UsageDefines.Add("UV2",                 "#define UV2_USED\n");
        sceneActions.UsageDefines.Add("BONE_INDICES",        "#define BONES_USED\n");
        sceneActions.UsageDefines.Add("BONE_WEIGHTS",        "#define WEIGHTS_USED\n");
        sceneActions.UsageDefines.Add("CUSTOM0",             "#define CUSTOM0_USED\n");
        sceneActions.UsageDefines.Add("CUSTOM1",             "#define CUSTOM1_USED\n");
        sceneActions.UsageDefines.Add("CUSTOM2",             "#define CUSTOM2_USED\n");
        sceneActions.UsageDefines.Add("CUSTOM3",             "#define CUSTOM3_USED\n");
        sceneActions.UsageDefines.Add("NORMAL_MAP",          "#define NORMAL_MAP_USED\n");
        sceneActions.UsageDefines.Add("NORMAL_MAP_DEPTH",    "@NORMAL_MAP");
        sceneActions.UsageDefines.Add("COLOR",               "#define COLOR_USED\n");
        sceneActions.UsageDefines.Add("INSTANCE_CUSTOM",     "#define ENABLE_INSTANCE_CUSTOM\n");
        sceneActions.UsageDefines.Add("POSITION",            "#define OVERGuidE_POSITION\n");

        sceneActions.UsageDefines.Add("ALPHA_SCISSOR_THRESHOLD",  "#define ALPHA_SCISSOR_USED\n");
        sceneActions.UsageDefines.Add("ALPHA_HASH_SCALE",         "#define ALPHA_HASH_USED\n");
        sceneActions.UsageDefines.Add("ALPHA_ANTIALIASING_EDGE",  "#define ALPHA_ANTIALIASING_EDGE_USED\n");
        sceneActions.UsageDefines.Add("ALPHA_TEXTURE_COORDINATE", "@ALPHA_ANTIALIASING_EDGE");

        sceneActions.UsageDefines.Add("SSS_STRENGTH",            "#define ENABLE_SSS\n");
        sceneActions.UsageDefines.Add("SSS_TRANSMITTANCE_DEPTH", "#define ENABLE_TRANSMITTANCE\n");
        sceneActions.UsageDefines.Add("BACKLIGHT",               "#define LIGHT_BACKLIGHT_USED\n");
        sceneActions.UsageDefines.Add("SCREEN_UV",               "#define SCREEN_UV_USED\n");

        sceneActions.UsageDefines.Add("DIFFUSE_LIGHT",  "#define USE_LIGHT_SHADER_CODE\n");
        sceneActions.UsageDefines.Add("SPECULAR_LIGHT", "#define USE_LIGHT_SHADER_CODE\n");

        sceneActions.UsageDefines.Add("FOG",        "#define CUSTOM_FOG_USED\n");
        sceneActions.UsageDefines.Add("RADIANCE",   "#define CUSTOM_RADIANCE_USED\n");
        sceneActions.UsageDefines.Add("IRRADIANCE", "#define CUSTOM_IRRADIANCE_USED\n");

        sceneActions.RenderModeDefines.Add("skip_vertex_transform",  "#define SKIP_TRANSFORM_USED\n");
        sceneActions.RenderModeDefines.Add("world_vertex_coords",    "#define VERTEX_WORLD_COORDS_USED\n");
        sceneActions.RenderModeDefines.Add("ensure_correct_normals", "#define ENSURE_CORRECT_NORMALS\n");
        sceneActions.RenderModeDefines.Add("cull_front",             "#define DO_SIDE_CHECK\n");
        sceneActions.RenderModeDefines.Add("cull_disabled",          "#define DO_SIDE_CHECK\n");
        sceneActions.RenderModeDefines.Add("particle_trails",        "#define USE_PARTICLE_TRAILS\n");
        sceneActions.RenderModeDefines.Add("depth_draw_opaque",      "#define USE_OPAQUE_PREPASS\n");

        var forceLambert = GLOBAL_GET<bool>("rendering/shading/overrides/force_lambert_over_burley");

        if (!forceLambert)
        {
            sceneActions.RenderModeDefines.Add("diffuse_burley", "#define DIFFUSE_BURLEY\n");
        }

        sceneActions.RenderModeDefines.Add("diffuse_lambert_wrap", "#define DIFFUSE_LAMBERT_WRAP\n");
        sceneActions.RenderModeDefines.Add("diffuse_toon",         "#define DIFFUSE_TOON\n");

        sceneActions.RenderModeDefines.Add("sss_mode_skin", "#define SSS_MODE_SKIN\n");

        sceneActions.RenderModeDefines.Add("specular_schlick_ggx",   "#define SPECULAR_SCHLICK_GGX\n");
        sceneActions.RenderModeDefines.Add("specular_toon",          "#define SPECULAR_TOON\n");
        sceneActions.RenderModeDefines.Add("specular_disabled",      "#define SPECULAR_DISABLED\n");
        sceneActions.RenderModeDefines.Add("shadows_disabled",       "#define SHADOWS_DISABLED\n");
        sceneActions.RenderModeDefines.Add("ambient_light_disabled", "#define AMBIENT_LIGHT_DISABLED\n");
        sceneActions.RenderModeDefines.Add("shadow_to_opacity",      "#define USE_SHADOW_TO_OPACITY\n");
        sceneActions.RenderModeDefines.Add("unshaded",               "#define MODE_UNSHADED\n");

        sceneActions.DefaultFilter = ShaderLanguage.TextureFilter.FILTER_LINEAR_MIPMAP;
        sceneActions.DefaultRepeat = ShaderLanguage.TextureRepeat.REPEAT_ENABLE;

        sceneActions.CheckMultiviewSamplers = RendererCompositor.Singleton.IsXrEnabled;
        sceneActions.GlobalBufferArrayVariable = "global_shader_uniforms";

        this.Shaders.CompilerScene.Initialize(sceneActions);

        // Setup Particles compiler

        var particlesActions = new ShaderCompiler.DefaultIdentifierActions();

        particlesActions.Renames.Add("COLOR",    "out_color");
        particlesActions.Renames.Add("VELOCITY", "out_velocity_flags.xyz");
        //actions.Renames.Add("MASS", "mass"); ?
        particlesActions.Renames.Add("ACTIVE",  "particle_active");
        particlesActions.Renames.Add("RESTART", "restart");
        particlesActions.Renames.Add("CUSTOM",  "out_custom");
        for (var i = 0; i < PARTICLES_MAX_USERDATAS; i++)
        {
            var udname = "USERDATA" + i + 1;
            particlesActions.Renames.Add(udname, "out_userdata" + i + 1);
            particlesActions.UsageDefines.Add(udname, "#define USERDATA" + i + 1 + "_USED\n");
        }
        particlesActions.Renames.Add("TRANSFORM", "xform");
        particlesActions.Renames.Add("TIME",      "time");
        particlesActions.Renames.Add("PI",        Math.PI.ToString());
        particlesActions.Renames.Add("TAU",       Math.Tau.ToString());
        particlesActions.Renames.Add("E",         Math.E.ToString());
        particlesActions.Renames.Add("LIFETIME",  "lifetime");
        particlesActions.Renames.Add("DELTA",     "local_delta");
        particlesActions.Renames.Add("NUMBER",    "particle_number");
        particlesActions.Renames.Add("INDEX",     "index");
        //actions.Renames.Add("GRAVITY", "current_gravity");
        particlesActions.Renames.Add("EMISSION_TRANSFORM",  "emission_transform");
        particlesActions.Renames.Add("RANDOM_SEED",         "random_seed");
        particlesActions.Renames.Add("FLAG_EMIT_POSITION",  "EMISSION_FLAG_HAS_POSITION");
        particlesActions.Renames.Add("FLAG_EMIT_ROT_SCALE", "EMISSION_FLAG_HAS_ROTATION_SCALE");
        particlesActions.Renames.Add("FLAG_EMIT_VELOCITY",  "EMISSION_FLAG_HAS_VELOCITY");
        particlesActions.Renames.Add("FLAG_EMIT_COLOR",     "EMISSION_FLAG_HAS_COLOR");
        particlesActions.Renames.Add("FLAG_EMIT_CUSTOM",    "EMISSION_FLAG_HAS_CUSTOM");
        particlesActions.Renames.Add("RESTART_POSITION",    "restart_position");
        particlesActions.Renames.Add("RESTART_ROT_SCALE",   "restart_rotation_scale");
        particlesActions.Renames.Add("RESTART_VELOCITY",    "restart_velocity");
        particlesActions.Renames.Add("RESTART_COLOR",       "restart_color");
        particlesActions.Renames.Add("RESTART_CUSTOM",      "restart_custom");
        particlesActions.Renames.Add("emit_subparticle",    "emit_subparticle");
        particlesActions.Renames.Add("COLLIDED",            "collided");
        particlesActions.Renames.Add("COLLISION_NORMAL",    "collision_normal");
        particlesActions.Renames.Add("COLLISION_DEPTH",     "collision_depth");
        particlesActions.Renames.Add("ATTRACTOR_FORCE",     "attractor_force");

        particlesActions.RenderModeDefines.Add("disable_force",       "#define DISABLE_FORCE\n");
        particlesActions.RenderModeDefines.Add("disable_velocity",    "#define DISABLE_VELOCITY\n");
        particlesActions.RenderModeDefines.Add("keep_data",           "#define ENABLE_KEEP_DATA\n");
        particlesActions.RenderModeDefines.Add("collision_use_scale", "#define USE_COLLISION_SCALE\n");

        particlesActions.DefaultFilter = ShaderLanguage.TextureFilter.FILTER_LINEAR_MIPMAP;
        particlesActions.DefaultRepeat = ShaderLanguage.TextureRepeat.REPEAT_ENABLE;

        particlesActions.GlobalBufferArrayVariable = "global_shader_uniforms";

        this.Shaders.CompilerParticles.Initialize(particlesActions);

        // Setup Sky compiler
        var skyActions = new ShaderCompiler.DefaultIdentifierActions();

        skyActions.Renames.Add("COLOR",               "color");
        skyActions.Renames.Add("ALPHA",               "alpha");
        skyActions.Renames.Add("EYEDIR",              "cube_normal");
        skyActions.Renames.Add("POSITION",            "position");
        skyActions.Renames.Add("SKY_COORDS",          "panorama_coords");
        skyActions.Renames.Add("SCREEN_UV",           "uv");
        skyActions.Renames.Add("TIME",                "time");
        skyActions.Renames.Add("FRAGCOORD",           "gl_FragCoord");
        skyActions.Renames.Add("PI",                  Math.PI.ToString());
        skyActions.Renames.Add("TAU",                 Math.Tau.ToString());
        skyActions.Renames.Add("E",                   Math.E.ToString());
        skyActions.Renames.Add("HALF_RES_COLOR",      "half_res_color");
        skyActions.Renames.Add("QUARTER_RES_COLOR",   "quarter_res_color");
        skyActions.Renames.Add("RADIANCE",            "radiance");
        skyActions.Renames.Add("FOG",                 "custom_fog");
        skyActions.Renames.Add("LIGHT0_ENABLED",      "directional_lights.data[0].enabled");
        skyActions.Renames.Add("LIGHT0_DIRECTION",    "directional_lights.data[0].direction_energy.xyz");
        skyActions.Renames.Add("LIGHT0_ENERGY",       "directional_lights.data[0].direction_energy.w");
        skyActions.Renames.Add("LIGHT0_COLOR",        "directional_lights.data[0].color_size.xyz");
        skyActions.Renames.Add("LIGHT0_SIZE",         "directional_lights.data[0].color_size.w");
        skyActions.Renames.Add("LIGHT1_ENABLED",      "directional_lights.data[1].enabled");
        skyActions.Renames.Add("LIGHT1_DIRECTION",    "directional_lights.data[1].direction_energy.xyz");
        skyActions.Renames.Add("LIGHT1_ENERGY",       "directional_lights.data[1].direction_energy.w");
        skyActions.Renames.Add("LIGHT1_COLOR",        "directional_lights.data[1].color_size.xyz");
        skyActions.Renames.Add("LIGHT1_SIZE",         "directional_lights.data[1].color_size.w");
        skyActions.Renames.Add("LIGHT2_ENABLED",      "directional_lights.data[2].enabled");
        skyActions.Renames.Add("LIGHT2_DIRECTION",    "directional_lights.data[2].direction_energy.xyz");
        skyActions.Renames.Add("LIGHT2_ENERGY",       "directional_lights.data[2].direction_energy.w");
        skyActions.Renames.Add("LIGHT2_COLOR",        "directional_lights.data[2].color_size.xyz");
        skyActions.Renames.Add("LIGHT2_SIZE",         "directional_lights.data[2].color_size.w");
        skyActions.Renames.Add("LIGHT3_ENABLED",      "directional_lights.data[3].enabled");
        skyActions.Renames.Add("LIGHT3_DIRECTION",    "directional_lights.data[3].direction_energy.xyz");
        skyActions.Renames.Add("LIGHT3_ENERGY",       "directional_lights.data[3].direction_energy.w");
        skyActions.Renames.Add("LIGHT3_COLOR",        "directional_lights.data[3].color_size.xyz");
        skyActions.Renames.Add("LIGHT3_SIZE",         "directional_lights.data[3].color_size.w");
        skyActions.Renames.Add("AT_CUBEMAP_PASS",     "AT_CUBEMAP_PASS");
        skyActions.Renames.Add("AT_HALF_RES_PASS",    "AT_HALF_RES_PASS");
        skyActions.Renames.Add("AT_QUARTER_RES_PASS", "AT_QUARTER_RES_PASS");
        skyActions.UsageDefines.Add("HALF_RES_COLOR",    "\n#define USES_HALF_RES_COLOR\n");
        skyActions.UsageDefines.Add("QUARTER_RES_COLOR", "\n#define USES_QUARTER_RES_COLOR\n");
        skyActions.RenderModeDefines.Add("disable_fog",   "#define DISABLE_FOG\n");
        skyActions.RenderModeDefines.Add("use_debanding", "#define USE_DEBANDING\n");

        skyActions.DefaultFilter = ShaderLanguage.TextureFilter.FILTER_LINEAR_MIPMAP;
        skyActions.DefaultRepeat = ShaderLanguage.TextureRepeat.REPEAT_ENABLE;

        skyActions.GlobalBufferArrayVariable = "global_shader_uniforms";

        this.Shaders.CompilerSky.Initialize(skyActions);
    }

    private CanvasMaterialData CreateCanvasMaterialFunc(ShaderData shaderData)
    {
        var materialData = new CanvasMaterialData
        {
            ShaderData = (CanvasShaderData)shaderData
        };

        //update will happen later anyway so do nothing.
        return materialData;
    }

    // GLES3::ShaderData *GLES3::_create_canvas_shader_func()
    private CanvasShaderData CreateCanvasShaderFunc() => new();

    private MaterialData CreateParticlesMaterialFunc(ShaderData shaderData) => throw new NotImplementedException();
    private ShaderData   CreateParticlesShaderFunc() => throw new NotImplementedException();

    private SceneMaterialData CreateSceneMaterialFunc(ShaderData shaderData)
    {
        var materialData = new SceneMaterialData
        {
            ShaderData = (SceneShaderData)shaderData
        };

        //update will happen later anyway so do nothing.
        return materialData;
    }

    private SceneShaderData CreateSceneShaderFunc() => new();

    private SkyMaterialData CreateSkyMaterialFunc(ShaderData shaderData)
    {
        var materialData = new SkyMaterialData
        {
            ShaderData = (SkyShaderData)shaderData
        };

        //update will happen later anyway so do nothing.
        return materialData;
    }

    private SkyShaderData CreateSkyShaderFunc() => new();

    private void MaterialQueueUpdate(Material material, bool uniform, bool texture)
    {
        material.UniformDirty = material.UniformDirty || uniform;
        material.TextureDirty = material.TextureDirty || texture;

        if (material.UpdateElement.InList)
        {
            return;
        }

        this.materialUpdateList.Add(material.UpdateElement);
    }

    #region public methods
    public Material? GetMaterial(Guid materialId) =>
        this.materialOwner.GetOrNull(materialId);

    private Shader? GetShader(Guid shaderId) =>
        this.shaderOwner.GetOrNull(shaderId);

    public uint GlobalShaderParametersGetUniformBuffer() =>
        this.globalShaderUniforms.Buffer;

    public MaterialData MaterialGetData(Guid material, RS.ShaderMode sHADER_CANVAS_ITEM) => throw new NotImplementedException();


    // void MaterialStorage::_update_global_shader_uniforms()
    public void UpdateGlobalShaderUniforms()
    {
        var materialStorage = Singleton;
        var gl              = GL.Singleton;

        if (this.globalShaderUniforms.BufferDirtyRegionCount > 0)
        {
            var totalRegions = this.globalShaderUniforms.BufferSize / GlobalShaderUniforms.BUFFER_DIRTY_REGION_SIZE;
            if (totalRegions / this.globalShaderUniforms.BufferDirtyRegionCount <= 4)
            {
                this.globalShaderUniforms.BufferValues = new GlobalShaderUniforms.Value[this.globalShaderUniforms.BufferSize];

                // 25% of regions dirty, just update all buffer
                gl.BindBuffer(BufferTargetARB.UniformBuffer, this.globalShaderUniforms.Buffer);
                gl.BufferData(BufferTargetARB.UniformBuffer, this.globalShaderUniforms.BufferValues, BufferUsageARB.DynamicDraw);
                gl.BindBuffer(BufferTargetARB.UniformBuffer, 0);
                this.globalShaderUniforms.BufferDirtyRegions = new bool[totalRegions];
            }
            else
            {
                var regionByteSize = Marshal.SizeOf<GlobalShaderUniforms.Value>() * GlobalShaderUniforms.BUFFER_DIRTY_REGION_SIZE;
                gl.BindBuffer(BufferTargetARB.UniformBuffer, this.globalShaderUniforms.Buffer);
                for (var i = 0u; i < totalRegions; i++)
                {
                    if (this.globalShaderUniforms.BufferDirtyRegions![i])
                    {
                        gl.BufferSubData(BufferTargetARB.UniformBuffer, (int)(i * regionByteSize), regionByteSize, this.globalShaderUniforms.BufferValues![i * GlobalShaderUniforms.BUFFER_DIRTY_REGION_SIZE]);
                        this.globalShaderUniforms.BufferDirtyRegions[i] = false;
                    }
                }
                gl.BindBuffer(BufferTargetARB.UniformBuffer, 0);
            }

            this.globalShaderUniforms.BufferDirtyRegionCount = 0;
        }

        if (this.globalShaderUniforms.MustUpdateBufferMaterials)
        {
            // only happens in the case of a buffer variable added or removed,
            // so not often.
            foreach (var materialId in this.globalShaderUniforms.MaterialsUsingBuffer)
            {
                var material = materialStorage.GetMaterial(materialId);

                if (ERR_CONTINUE(material == null)) //wtf
                {
                    continue;
                }

                materialStorage.MaterialQueueUpdate(material!, true, false);
            }

            this.globalShaderUniforms.MustUpdateBufferMaterials = false;
        }

        if (this.globalShaderUniforms.MustUpdateTextureMaterials)
        {
            // only happens in the case of a buffer variable added or removed,
            // so not often.
            foreach (var materialId in this.globalShaderUniforms.MaterialsUsingTexture)
            {
                var material = materialStorage.GetMaterial(materialId);

                if (ERR_CONTINUE(material == null)) //wtf
                {
                    continue;
                }

                materialStorage.MaterialQueueUpdate(material!, false, true);
            }

            this.globalShaderUniforms.MustUpdateTextureMaterials = false;
        }
    }

    // void MaterialStorage::_update_queued_materials()
    public void UpdateQueuedMaterials()
    {
        while (this.materialUpdateList.First != null)
        {
            var material = this.materialUpdateList.First.Self;

            if (material!.Data != null)
            {
                material.Data.UpdateParameters(material.Params, material.UniformDirty, material.TextureDirty);
            }
            material.TextureDirty = false;
            material.UniformDirty = false;

            this.materialUpdateList.Remove(material.UpdateElement);
        }
    }
    #endregion public methods

    #region public override methods
    public override void GetShaderParameterList(Guid shaderId, List<PropertyInfo> paramList) => throw new NotImplementedException();
    public override void GlobalShaderParameterAdd(string name, RS.GlobalShaderParameterType type, object value) => throw new NotImplementedException();
    public override object GlobalShaderParameterGet(string name) => throw new NotImplementedException();
    public override List<string> GlobalShaderParameterGetList() => throw new NotImplementedException();
    public override RS.GlobalShaderParameterType GlobalShaderParameterGetType(string name) => throw new NotImplementedException();
    public override void GlobalShaderParameterRemove(string name) => throw new NotImplementedException();
    public override void GlobalShaderParametersClear() => throw new NotImplementedException();
    public override void GlobalShaderParameterSet(string name, object value) => throw new NotImplementedException();
    public override void GlobalShaderParameterSetOverGuide(string name, object value) => throw new NotImplementedException();
    public override int GlobalShaderParametersInstanceAllocate(Guid instance) => throw new NotImplementedException();
    public override void GlobalShaderParametersInstanceFree(Guid instance) => throw new NotImplementedException();
    public override void GlobalShaderParametersInstanceUpdate(Guid instance, int index, object value, int flagsCount = 0) => throw new NotImplementedException();
    public override void GlobalShaderParametersLoadSettings(bool loadTextures = true) => throw new NotImplementedException();
    public override bool MaterialCastsShadows(Guid material) => throw new NotImplementedException();
    public override void MaterialFree(Guid id) => throw new NotImplementedException();
    public override void MaterialGetInstanceShaderParameters(Guid material, List<InstanceShaderParam> parameters) => throw new NotImplementedException();
    public override object? MaterialGetParam(Guid material, string param) => throw new NotImplementedException();

    // void MaterialStorage::material_initialize(RID p_rid)
    public override void MaterialInitialize(Guid id)
    {
        var material = this.materialOwner.Initialize(id);
        material.Self = id;
    }

    public override bool MaterialIsAnimated(Guid material) => throw new NotImplementedException();
    public override void MaterialSetNextPass(Guid material, Guid nextMaterial) => throw new NotImplementedException();
    public override void MaterialSetParam(Guid material, string param, object value) => throw new NotImplementedException();
    public override void MaterialSetRenderPriority(Guid material, int priority) => throw new NotImplementedException();

    // void MaterialStorage::material_set_shader(RID p_material, RID p_shader)
    public override void MaterialSetShader(Guid materialId, Guid shaderId)
    {
        var material = this.materialOwner.GetOrNull(materialId);

        if (ERR_FAIL_COND(material == null))
        {
            return;
        }

        material!.Data = null;

        if (material.Shader != null)
        {
            material.Shader.Owners.Remove(material);
            material.Shader = null;
            material.ShaderMode = RS.ShaderMode.SHADER_MAX;
        }

        if (shaderId == default)
        {
            material.Dependency.ChangedNotify(Dependency.DependencyChangedNotification.DEPENDENCY_CHANGED_MATERIAL);
            material.ShaderId = default;
            return;
        }

        var shader = this.GetShader(shaderId);

        if (ERR_FAIL_COND(shader == null))
        {
            return;
        }

        material.Shader     = shader;
        material.ShaderMode = shader!.Mode;
        material.ShaderId   = shaderId;

        shader.Owners.Add(material);

        if (shader.Mode == RS.ShaderMode.SHADER_MAX)
        {
            return;
        }

        if (ERR_FAIL_COND(shader.Data == null))
        {
            return;
        }

        material.Data      = this.materialDataRequestFunc[(int)shader.Mode]!(shader!.Data!);
        material.Data.Self = materialId;

        material.Data.NextPass       = material.NextPass;
        material.Data.RenderPriority = material.Priority;
        //updating happens later
        material.Dependency.ChangedNotify(Dependency.DependencyChangedNotification.DEPENDENCY_CHANGED_MATERIAL);
        this.MaterialQueueUpdate(material, true, true);
    }

    public override void MaterialUpdateDependency(Guid material, DependencyTracker instance) => throw new NotImplementedException();
    public override void ShaderFree(Guid id) => throw new NotImplementedException();
    public override string ShaderGetCode(Guid shaderId) => throw new NotImplementedException();
    public override Guid ShaderGetDefaultTextureParameter(Guid shaderId, string name, int index) => throw new NotImplementedException();
    public override RS.ShaderNativeSourceCode ShaderGetNativeSourceCode(Guid shaderId) => throw new NotImplementedException();
    public override object ShaderGetParameterDefault(Guid material, string param) => throw new NotImplementedException();

    // void MaterialStorage::shader_initialize(RID p_rid)
    public override void ShaderInitialize(Guid id)
    {
        var shader = new Shader
        {
            Mode = RS.ShaderMode.SHADER_MAX,
        };

        this.shaderOwner.Initialize(id, shader);
    }

    // void MaterialStorage::shader_set_code(RID p_shader, const String &p_code)
    public override void ShaderSetCode(Guid shaderId, string code)
    {
        var shader = this.shaderOwner.GetOrNull(shaderId);

        if (ERR_FAIL_COND(shader == null))
        {
            return;
        }

        shader!.Code = code;

        var modeString = ShaderLanguage.GetShaderType(code);

        RS.ShaderMode newMode;
        if (modeString == "canvas_item")
        {
            newMode = RS.ShaderMode.SHADER_CANVAS_ITEM;
        }
        else if (modeString == "particles")
        {
            newMode = RS.ShaderMode.SHADER_PARTICLES;
        }
        else if (modeString == "spatial")
        {
            newMode = RS.ShaderMode.SHADER_SPATIAL;
        }
        else if (modeString == "sky")
        {
            newMode = RS.ShaderMode.SHADER_SKY;
            //} else if (mode_string == "fog") {
            //  new_mode = RS::SHADER_FOG;
        }
        else
        {
            newMode = RS.ShaderMode.SHADER_MAX;
            ERR_PRINT("shader type " + modeString + " not supported in OpenGL renderer");
        }

        if (newMode != shader.Mode)
        {
            shader.Data = null;

            foreach (var material in shader.Owners)
            {
                material.ShaderMode = newMode;
                material.Data       = null;
            }

            shader.Mode = newMode;

            if (newMode < RS.ShaderMode.SHADER_MAX && this.shaderDataRequestFunc[(int)newMode] != null)
            {
                shader.Data = this.shaderDataRequestFunc[(int)newMode]!();
            }
            else
            {
                shader.Mode = RS.ShaderMode.SHADER_MAX; //invalid
            }

            foreach (var material in shader.Owners)
            {
                if (shader.Data != null)
                {
                    material.Data                = this.materialDataRequestFunc[(int)newMode]!(shader.Data);
                    material.Data.Self           = material.Self;
                    material.Data.NextPass       = material.NextPass;
                    material.Data.RenderPriority = material.Priority;
                }
                material.ShaderMode = newMode;
            }

            if (shader.Data != null)
            {
                foreach (var e in shader.DefaultTextureParameter)
                {
                    foreach (var e2 in e.Value)
                    {
                        shader.Data.SetDefaultTextureParameter(e.Key, e2.Value, e2.Key);
                    }
                }
            }
        }

        shader.Data?.SetCode(code);

        foreach (var material in shader.Owners)
        {
            material.Dependency.ChangedNotify(Dependency.DependencyChangedNotification.DEPENDENCY_CHANGED_MATERIAL);
            this.MaterialQueueUpdate(material, true, true);
        }
    }


    public override void ShaderSetDefaultTextureParameter(Guid shaderId, string name, Guid texture, int index) => throw new NotImplementedException();
    public override void ShaderSetPathHint(Guid shaderId, string path) => throw new NotImplementedException();
    #endregion public override methods
}
