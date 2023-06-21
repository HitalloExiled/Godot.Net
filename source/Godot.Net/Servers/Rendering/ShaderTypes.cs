namespace Godot.Net.Servers.Rendering;

using System;
using System.Collections.Generic;

using Argument       = ShaderLanguage.StageFunctionInfo.Argument;
using ModeInfo       = ShaderLanguage.ModeInfo;
using ShaderDataType = ShaderLanguage.DataType;

#pragma warning disable IDE0044, IDE0052, CS0649 // TODO Remove

public partial class ShaderTypes
{
    private static ShaderTypes? singleton;

    private readonly Dictionary<RS.ShaderMode, Type> shaderModes;

    public static ShaderTypes Singleton => singleton ?? throw new NullReferenceException();

    public HashSet<string> Types     { get; }
    public List<string>    TypesList { get; }

    public ShaderTypes()
    {
        singleton = this;

        this.shaderModes = new()
        {
            {
                RS.ShaderMode.SHADER_SPATIAL,
                new()
                {
                    Functions = new()
                    {
                        {
                            "global",
                            new()
                            {
                                BuiltIns =
                                {
                                    { "TIME", new(ShaderDataType.TYPE_FLOAT, true) },
                                }
                            }
                        },
                        {
                            "constants",
                            new()
                            {
                                BuiltIns =
                                {
                                    { "PI",  new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "TAU", new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "E",   new(ShaderDataType.TYPE_FLOAT, true) },
                                }
                            }
                        },
                        {
                            "vertex",
                            new()
                            {
                                BuiltIns =
                                {
                                    { "VERTEX",                  new(ShaderDataType.TYPE_VEC3) },
                                    { "NORMAL",                  new(ShaderDataType.TYPE_VEC3) },
                                    { "TANGENT",                 new(ShaderDataType.TYPE_VEC3) },
                                    { "BINORMAL",                new(ShaderDataType.TYPE_VEC3) },
                                    { "POSITION",                new(ShaderDataType.TYPE_VEC4) },
                                    { "UV",                      new(ShaderDataType.TYPE_VEC2) },
                                    { "UV2",                     new(ShaderDataType.TYPE_VEC2) },
                                    { "COLOR",                   new(ShaderDataType.TYPE_VEC4) },
                                    { "POINT_SIZE",              new(ShaderDataType.TYPE_FLOAT) },
                                    { "INSTANCE_ID",             new(ShaderDataType.TYPE_INT, true) },
                                    { "INSTANCE_CUSTOM",         new(ShaderDataType.TYPE_VEC4, true) },
                                    { "VERTEX_ID",               new(ShaderDataType.TYPE_INT, true) },
                                    { "ROUGHNESS",               new(ShaderDataType.TYPE_FLOAT) },
                                    { "BONE_INDICES",            new(ShaderDataType.TYPE_UVEC4) },
                                    { "BONE_WEIGHTS",            new(ShaderDataType.TYPE_VEC4) },
                                    { "CUSTOM0",                 new(ShaderDataType.TYPE_VEC4) },
                                    { "CUSTOM1",                 new(ShaderDataType.TYPE_VEC4) },
                                    { "CUSTOM2",                 new(ShaderDataType.TYPE_VEC4) },
                                    { "CUSTOM3",                 new(ShaderDataType.TYPE_VEC4) },
                                    { "MODEL_MATRIX",            new(ShaderDataType.TYPE_MAT4, true) },
                                    { "MODEL_NORMAL_MATRIX",     new(ShaderDataType.TYPE_MAT3, true) },
                                    { "VIEW_MATRIX",             new(ShaderDataType.TYPE_MAT4, true) },
                                    { "INV_VIEW_MATRIX",         new(ShaderDataType.TYPE_MAT4, true) },
                                    { "PROJECTION_MATRIX",       new(ShaderDataType.TYPE_MAT4) },
                                    { "INV_PROJECTION_MATRIX",   new(ShaderDataType.TYPE_MAT4, true) },
                                    { "MODELVIEW_MATRIX",        new(ShaderDataType.TYPE_MAT4) },
                                    { "MODELVIEW_NORMAL_MATRIX", new(ShaderDataType.TYPE_MAT3) },
                                    { "VIEWPORT_SIZE",           new(ShaderDataType.TYPE_VEC2, true) },
                                    { "OUTPUT_IS_SRGB",          new(ShaderDataType.TYPE_BOOL, true) },
                                    { "NODE_POSITION_WORLD",     new(ShaderDataType.TYPE_VEC3) },
                                    { "CAMERA_POSITION_WORLD",   new(ShaderDataType.TYPE_VEC3) },
                                    { "CAMERA_DIRECTION_WORLD",  new(ShaderDataType.TYPE_VEC3) },
                                    { "CAMERA_VISIBLE_LAYERS",   new(ShaderDataType.TYPE_UINT) },
                                    { "NODE_POSITION_VIEW",      new(ShaderDataType.TYPE_VEC3) },
                                    { "VIEW_INDEX",              new(ShaderDataType.TYPE_INT, true) },
                                    { "VIEW_MONO_LEFT",          new(ShaderDataType.TYPE_INT, true) },
                                    { "VIEW_RIGHT",              new(ShaderDataType.TYPE_INT, true) },
                                    { "EYE_OFFSET",              new(ShaderDataType.TYPE_VEC3, true) },
                                },
                                CanDiscard   = false,
                                MainFunction = true,
                            }
                        },
                        {
                            "fragment",
                            new()
                            {
                                BuiltIns =
                                {
                                    { "VERTEX",                   new(ShaderDataType.TYPE_VEC3, true) },
                                    { "FRAGCOORD",                new(ShaderDataType.TYPE_VEC4, true) },
                                    { "FRONT_FACING",             new(ShaderDataType.TYPE_BOOL, true) },
                                    { "NORMAL",                   new(ShaderDataType.TYPE_VEC3) },
                                    { "TANGENT",                  new(ShaderDataType.TYPE_VEC3) },
                                    { "BINORMAL",                 new(ShaderDataType.TYPE_VEC3) },
                                    { "VIEW",                     new(ShaderDataType.TYPE_VEC3, true) },
                                    { "NORMAL_MAP",               new(ShaderDataType.TYPE_VEC3) },
                                    { "NORMAL_MAP_DEPTH",         new(ShaderDataType.TYPE_FLOAT) },
                                    { "UV",                       new(ShaderDataType.TYPE_VEC2, true) },
                                    { "UV2",                      new(ShaderDataType.TYPE_VEC2, true) },
                                    { "COLOR",                    new(ShaderDataType.TYPE_VEC4, true) },
                                    { "ALBEDO",                   new(ShaderDataType.TYPE_VEC3) },
                                    { "ALPHA",                    new(ShaderDataType.TYPE_FLOAT) },
                                    { "METALLIC",                 new(ShaderDataType.TYPE_FLOAT) },
                                    { "SPECULAR",                 new(ShaderDataType.TYPE_FLOAT) },
                                    { "ROUGHNESS",                new(ShaderDataType.TYPE_FLOAT) },
                                    { "RIM",                      new(ShaderDataType.TYPE_FLOAT) },
                                    { "RIM_TINT",                 new(ShaderDataType.TYPE_FLOAT) },
                                    { "CLEARCOAT",                new(ShaderDataType.TYPE_FLOAT) },
                                    { "CLEARCOAT_ROUGHNESS",      new(ShaderDataType.TYPE_FLOAT) },
                                    { "ANISOTROPY",               new(ShaderDataType.TYPE_FLOAT) },
                                    { "ANISOTROPY_FLOW",          new(ShaderDataType.TYPE_VEC2) },
                                    { "SSS_STRENGTH",             new(ShaderDataType.TYPE_FLOAT) },
                                    { "SSS_TRANSMITTANCE_COLOR",  new(ShaderDataType.TYPE_VEC4) },
                                    { "SSS_TRANSMITTANCE_DEPTH",  new(ShaderDataType.TYPE_FLOAT) },
                                    { "SSS_TRANSMITTANCE_BOOST",  new(ShaderDataType.TYPE_FLOAT) },
                                    { "BACKLIGHT",                new(ShaderDataType.TYPE_VEC3) },
                                    { "AO",                       new(ShaderDataType.TYPE_FLOAT) },
                                    { "AO_LIGHT_AFFECT",          new(ShaderDataType.TYPE_FLOAT) },
                                    { "EMISSION",                 new(ShaderDataType.TYPE_VEC3) },
                                    { "DEPTH",                    new(ShaderDataType.TYPE_FLOAT) },
                                    { "SCREEN_UV",                new(ShaderDataType.TYPE_VEC2, true) },
                                    { "POINT_COORD",              new(ShaderDataType.TYPE_VEC2, true) },
                                    { "NODE_POSITION_WORLD",      new(ShaderDataType.TYPE_VEC3) },
                                    { "CAMERA_POSITION_WORLD",    new(ShaderDataType.TYPE_VEC3) },
                                    { "CAMERA_DIRECTION_WORLD",   new(ShaderDataType.TYPE_VEC3) },
                                    { "CAMERA_VISIBLE_LAYERS",    new(ShaderDataType.TYPE_UINT) },
                                    { "NODE_POSITION_VIEW",       new(ShaderDataType.TYPE_VEC3) },
                                    { "VIEW_INDEX",               new(ShaderDataType.TYPE_INT, true) },
                                    { "VIEW_MONO_LEFT",           new(ShaderDataType.TYPE_INT, true) },
                                    { "VIEW_RIGHT",               new(ShaderDataType.TYPE_INT, true) },
                                    { "EYE_OFFSET",               new(ShaderDataType.TYPE_VEC3, true) },
                                    { "OUTPUT_IS_SRGB",           new(ShaderDataType.TYPE_BOOL, true) },
                                    { "MODEL_MATRIX",             new(ShaderDataType.TYPE_MAT4, true) },
                                    { "MODEL_NORMAL_MATRIX",      new(ShaderDataType.TYPE_MAT3, true) },
                                    { "VIEW_MATRIX",              new(ShaderDataType.TYPE_MAT4, true) },
                                    { "INV_VIEW_MATRIX",          new(ShaderDataType.TYPE_MAT4, true) },
                                    { "PROJECTION_MATRIX",        new(ShaderDataType.TYPE_MAT4, true) },
                                    { "INV_PROJECTION_MATRIX",    new(ShaderDataType.TYPE_MAT4, true) },
                                    { "VIEWPORT_SIZE",            new(ShaderDataType.TYPE_VEC2, true) },
                                    { "FOG",                      new(ShaderDataType.TYPE_VEC4) }, // TODO consider adding to light shader
                                    { "RADIANCE",                 new(ShaderDataType.TYPE_VEC4) },
                                    { "IRRADIANCE",               new(ShaderDataType.TYPE_VEC4) },
                                    { "ALPHA_SCISSOR_THRESHOLD",  new(ShaderDataType.TYPE_FLOAT) },
                                    { "ALPHA_HASH_SCALE",         new(ShaderDataType.TYPE_FLOAT) },
                                    { "ALPHA_ANTIALIASING_EDGE",  new(ShaderDataType.TYPE_FLOAT) },
                                    { "ALPHA_TEXTURE_COORDINATE", new(ShaderDataType.TYPE_VEC2) },
                                },
                                CanDiscard   = true,
                                MainFunction = true,
                            }
                        },
                        {
                            "light",
                            new()
                            {
                                BuiltIns =
                                {
                                    { "MODEL_MATRIX",          new(ShaderDataType.TYPE_MAT4, true) },
                                    { "VIEW_MATRIX",           new(ShaderDataType.TYPE_MAT4, true) },
                                    { "INV_VIEW_MATRIX",       new(ShaderDataType.TYPE_MAT4, true) },
                                    { "PROJECTION_MATRIX",     new(ShaderDataType.TYPE_MAT4, true) },
                                    { "INV_PROJECTION_MATRIX", new(ShaderDataType.TYPE_MAT4, true) },
                                    { "VIEWPORT_SIZE",         new(ShaderDataType.TYPE_VEC2, true) },
                                    { "FRAGCOORD",             new(ShaderDataType.TYPE_VEC4, true) },
                                    { "NORMAL",                new(ShaderDataType.TYPE_VEC3, true) },
                                    { "UV",                    new(ShaderDataType.TYPE_VEC2, true) },
                                    { "UV2",                   new(ShaderDataType.TYPE_VEC2, true) },
                                    { "VIEW",                  new(ShaderDataType.TYPE_VEC3, true) },
                                    { "LIGHT",                 new(ShaderDataType.TYPE_VEC3, true) },
                                    { "LIGHT_COLOR",           new(ShaderDataType.TYPE_VEC3, true) },
                                    { "ATTENUATION",           new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "ALBEDO",                new(ShaderDataType.TYPE_VEC3, true) },
                                    { "BACKLIGHT",             new(ShaderDataType.TYPE_VEC3, true) },
                                    { "METALLIC",              new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "ROUGHNESS",             new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "DIFFUSE_LIGHT",         new(ShaderDataType.TYPE_VEC3) },
                                    { "SPECULAR_LIGHT",        new(ShaderDataType.TYPE_VEC3) },
                                    { "OUTPUT_IS_SRGB",        new(ShaderDataType.TYPE_BOOL, true) },
                                    { "ALPHA",                 new(ShaderDataType.TYPE_FLOAT) },
                                },
                                CanDiscard   = true,
                                MainFunction = true,
                            }
                        },
                    },
                    Modes = new ModeInfo[]
                    {
                        new(PNAME("blend"), "mix", "add", "sub", "mul"),
                        new(PNAME("depth_draw"), "opaque", "always", "never"),
                        new(PNAME("depth_prepass_alpha")),
                        new(PNAME("depth_test_disabled")),
                        new(PNAME("sss_mode_skin")),
                        new(PNAME("cull"), "back", "front", "disabled"),
                        new(PNAME("unshaded")),
                        new(PNAME("wireframe")),
                        new(PNAME("diffuse"), "lambert", "lambert_wrap", "burley", "toon"),
                        new(PNAME("specular"), "schlick_ggx", "toon", "disabled"),
                        new(PNAME("skip_vertex_transform")),
                        new(PNAME("world_vertex_coords")),
                        new(PNAME("ensure_correct_normals")),
                        new(PNAME("shadows_disabled")),
                        new(PNAME("ambient_light_disabled")),
                        new(PNAME("shadow_to_opacity")),
                        new(PNAME("vertex_lighting")),
                        new(PNAME("particle_trails")),
                        new(PNAME("alpha_to_coverage")),
                        new(PNAME("alpha_to_coverage_and_one")),
                    }
                }
            },
            {
                RS.ShaderMode.SHADER_CANVAS_ITEM,
                new()
                {
                    Functions = new()
                    {
                        {
                            "global",
                            new()
                            {
                                BuiltIns =
                                {
                                    { "TIME", new(ShaderDataType.TYPE_FLOAT, true) },
                                }
                            }
                        },
                        {
                            "constants",
                            new()
                            {
                                BuiltIns =
                                {
                                    { "PI",  new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "TAU", new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "E",   new(ShaderDataType.TYPE_FLOAT, true) },
                                }
                            }
                        },
                        {
                            "vertex",
                            new()
                            {
                                BuiltIns =
                                {
                                    { "PI",                 new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "TAU",                new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "E",                  new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "VERTEX",             new(ShaderDataType.TYPE_VEC2) },
                                    { "UV",                 new(ShaderDataType.TYPE_VEC2) },
                                    { "COLOR",              new(ShaderDataType.TYPE_VEC4) },
                                    { "POINT_SIZE",         new(ShaderDataType.TYPE_FLOAT) },
                                    { "MODEL_MATRIX",       new(ShaderDataType.TYPE_MAT4, true) },
                                    { "CANVAS_MATRIX",      new(ShaderDataType.TYPE_MAT4, true) },
                                    { "SCREEN_MATRIX",      new(ShaderDataType.TYPE_MAT4, true) },
                                    { "INSTANCE_CUSTOM",    new(ShaderDataType.TYPE_VEC4, true) },
                                    { "INSTANCE_ID",        new(ShaderDataType.TYPE_INT, true) },
                                    { "VERTEX_ID",          new(ShaderDataType.TYPE_INT, true) },
                                    { "AT_LIGHT_PASS",      new(ShaderDataType.TYPE_BOOL, true) },
                                    { "TEXTURE_PIXEL_SIZE", new(ShaderDataType.TYPE_VEC2, true) },
                                },
                                CanDiscard = false,
                                MainFunction = true,
                            }
                        },
                        {
                            "fragment",
                            new()
                            {
                                BuiltIns =
                                {
                                    { "VERTEX",                     new(ShaderDataType.TYPE_VEC2) },
                                    { "SHADOW_VERTEX",              new(ShaderDataType.TYPE_VEC2) },
                                    { "LIGHT_VERTEX",               new(ShaderDataType.TYPE_VEC3) },
                                    { "FRAGCOORD",                  new(ShaderDataType.TYPE_VEC4, true) },
                                    { "NORMAL",                     new(ShaderDataType.TYPE_VEC3) },
                                    { "NORMAL_MAP",                 new(ShaderDataType.TYPE_VEC3) },
                                    { "NORMAL_MAP_DEPTH",           new(ShaderDataType.TYPE_FLOAT) },
                                    { "UV",                         new(ShaderDataType.TYPE_VEC2, true) },
                                    { "COLOR",                      new(ShaderDataType.TYPE_VEC4) },
                                    { "TEXTURE",                    new(ShaderDataType.TYPE_SAMPLER2D, true) },
                                    { "TEXTURE_PIXEL_SIZE",         new(ShaderDataType.TYPE_VEC2, true) },
                                    { "NORMAL_TEXTURE",             new(ShaderDataType.TYPE_SAMPLER2D, true) },
                                    { "SPECULAR_SHININESS_TEXTURE", new(ShaderDataType.TYPE_SAMPLER2D, true) },
                                    { "SPECULAR_SHININESS",         new(ShaderDataType.TYPE_VEC4, true) },
                                    { "SCREEN_UV",                  new(ShaderDataType.TYPE_VEC2, true) },
                                    { "SCREEN_PIXEL_SIZE",          new(ShaderDataType.TYPE_VEC2, true) },
                                    { "POINT_COORD",                new(ShaderDataType.TYPE_VEC2, true) },
                                    { "AT_LIGHT_PASS",              new(ShaderDataType.TYPE_BOOL, true) },
                                },
                                CanDiscard     = true,
                                MainFunction   = true,
                                StageFunctions =
                                {
                                    { "texture_sdf",        new(ShaderDataType.TYPE_FLOAT, new Argument("sdf_pos", ShaderDataType.TYPE_VEC2)) },
                                    { "sdf_to_screen_uv",   new(ShaderDataType.TYPE_VEC2,  new Argument("sdf_pos", ShaderDataType.TYPE_VEC2)) },
                                    { "texture_sdf_normal", new(ShaderDataType.TYPE_VEC2,  new Argument("sdf_pos", ShaderDataType.TYPE_VEC2)) },
                                    { "screen_uv_to_sdf",   new(ShaderDataType.TYPE_VEC2,  new Argument("uv", ShaderDataType.TYPE_VEC2)) },
                                }
                            }
                        },
                        {
                            "light",
                            new()
                            {
                                BuiltIns =
                                {
                                    { "FRAGCOORD",            new(ShaderDataType.TYPE_VEC4, true) },
                                    { "NORMAL",               new(ShaderDataType.TYPE_VEC3, true) },
                                    { "COLOR",                new(ShaderDataType.TYPE_VEC4, true) },
                                    { "UV",                   new(ShaderDataType.TYPE_VEC2, true) },
                                    { "SPECULAR_SHININESS",   new(ShaderDataType.TYPE_VEC4, true) },
                                    { "LIGHT_COLOR",          new(ShaderDataType.TYPE_VEC4, true) },
                                    { "LIGHT_POSITION",       new(ShaderDataType.TYPE_VEC3, true) },
                                    { "LIGHT_DIRECTION",      new(ShaderDataType.TYPE_VEC3, true) },
                                    { "LIGHT_ENERGY",         new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "LIGHT_IS_DIRECTIONAL", new(ShaderDataType.TYPE_BOOL, true) },
                                    { "LIGHT_VERTEX",         new(ShaderDataType.TYPE_VEC3, true) },
                                    { "LIGHT",                new(ShaderDataType.TYPE_VEC4) },
                                    { "SHADOW_MODULATE",      new(ShaderDataType.TYPE_VEC4) },
                                    { "SCREEN_UV",            new(ShaderDataType.TYPE_VEC2, true) },
                                    { "TEXTURE",              new(ShaderDataType.TYPE_SAMPLER2D, true) },
                                    { "TEXTURE_PIXEL_SIZE",   new(ShaderDataType.TYPE_VEC2, true) },
                                    { "POINT_COORD",          new(ShaderDataType.TYPE_VEC2, true) },
                                },
                                CanDiscard     = true,
                                MainFunction   = true,
                                StageFunctions =
                                {
                                    { "texture_sdf",        new(ShaderDataType.TYPE_FLOAT, new Argument("sdf_pos", ShaderDataType.TYPE_FLOAT)) },
                                    { "sdf_to_screen_uv",   new(ShaderDataType.TYPE_FLOAT, new Argument("sdf_pos", ShaderDataType.TYPE_VEC2)) },
                                    { "texture_sdf_normal", new(ShaderDataType.TYPE_FLOAT, new Argument("sdf_pos", ShaderDataType.TYPE_VEC2)) },
                                    { "screen_uv_to_sdf",   new(ShaderDataType.TYPE_FLOAT, new Argument("uv", ShaderDataType.TYPE_VEC2)) },
                                }
                            }
                        }
                    },
                    Modes = new ModeInfo[]
                    {
                        new(PNAME("skip_vertex_transform")),
                        new(PNAME("blend"), "mix", "add", "sub", "mul", "premul_alpha", "disabled"),
                        new(PNAME("unshaded")),
                        new(PNAME("light_only")),
                    },
                }
            },
            {
                RS.ShaderMode.SHADER_PARTICLES,
                new()
                {
                    Functions = new()
                    {
                        {
                            "global",
                            new()
                            {
                                BuiltIns = new()
                                {
                                    { "TIME", new(ShaderDataType.TYPE_FLOAT, true) },
                                }
                            }
                        },
                        {
                            "constants",
                            new()
                            {
                                BuiltIns = new()
                                {
                                    { "PI",  new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "TAU", new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "E",   new(ShaderDataType.TYPE_FLOAT, true) },
                                }
                            }
                        },
                        {
                            "start",
                            new()
                            {
                                BuiltIns = new()
                                {
                                    { "COLOR",               new(ShaderDataType.TYPE_VEC4) },
                                    { "VELOCITY",            new(ShaderDataType.TYPE_VEC3) },
                                    { "MASS",                new(ShaderDataType.TYPE_FLOAT) },
                                    { "ACTIVE",              new(ShaderDataType.TYPE_BOOL) },
                                    { "CUSTOM",              new(ShaderDataType.TYPE_VEC4) },
                                    { "USERDATA1",           new(ShaderDataType.TYPE_VEC4) },
                                    { "USERDATA2",           new(ShaderDataType.TYPE_VEC4) },
                                    { "USERDATA3",           new(ShaderDataType.TYPE_VEC4) },
                                    { "USERDATA4",           new(ShaderDataType.TYPE_VEC4) },
                                    { "USERDATA5",           new(ShaderDataType.TYPE_VEC4) },
                                    { "USERDATA6",           new(ShaderDataType.TYPE_VEC4) },
                                    { "TRANSFORM",           new(ShaderDataType.TYPE_MAT4) },
                                    { "LIFETIME",            new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "DELTA",               new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "NUMBER",              new(ShaderDataType.TYPE_UINT, true) },
                                    { "INDEX",               new(ShaderDataType.TYPE_UINT, true) },
                                    { "EMISSION_TRANSFORM",  new(ShaderDataType.TYPE_MAT4, true) },
                                    { "RANDOM_SEED",         new(ShaderDataType.TYPE_UINT, true) },
                                    { "FLAG_EMIT_POSITION",  new(ShaderDataType.TYPE_UINT, true) },
                                    { "FLAG_EMIT_ROT_SCALE", new(ShaderDataType.TYPE_UINT, true) },
                                    { "FLAG_EMIT_VELOCITY",  new(ShaderDataType.TYPE_UINT, true) },
                                    { "FLAG_EMIT_COLOR",     new(ShaderDataType.TYPE_UINT, true) },
                                    { "FLAG_EMIT_CUSTOM",    new(ShaderDataType.TYPE_UINT, true) },
                                    { "RESTART_POSITION",    new(ShaderDataType.TYPE_BOOL, true) },
                                    { "RESTART_ROT_SCALE",   new(ShaderDataType.TYPE_BOOL, true) },
                                    { "RESTART_VELOCITY",    new(ShaderDataType.TYPE_BOOL, true) },
                                    { "RESTART_COLOR",       new(ShaderDataType.TYPE_BOOL, true) },
                                    { "RESTART_CUSTOM",      new(ShaderDataType.TYPE_BOOL, true) },
                                },
                                MainFunction = true,
                                StageFunctions = new()
                                {
                                    {
                                        "emit_subparticle",
                                        new(
                                            ShaderDataType.TYPE_BOOL,
                                            new Argument("xform",    ShaderDataType.TYPE_MAT4),
                                            new Argument("velocity", ShaderDataType.TYPE_VEC3),
                                            new Argument("color",    ShaderDataType.TYPE_VEC4),
                                            new Argument("custom",   ShaderDataType.TYPE_VEC4),
                                            new Argument("flags",    ShaderDataType.TYPE_UINT)
                                        )
                                    }
                                }
                            }
                        },
                        {
                            "process",
                            new()
                            {
                                BuiltIns = new()
                                {
                                    { "COLOR",               new(ShaderDataType.TYPE_VEC4) },
                                    { "VELOCITY",            new(ShaderDataType.TYPE_VEC3) },
                                    { "MASS",                new(ShaderDataType.TYPE_FLOAT) },
                                    { "ACTIVE",              new(ShaderDataType.TYPE_BOOL) },
                                    { "RESTART",             new(ShaderDataType.TYPE_BOOL, true) },
                                    { "CUSTOM",              new(ShaderDataType.TYPE_VEC4) },
                                    { "USERDATA1",           new(ShaderDataType.TYPE_VEC4) },
                                    { "USERDATA2",           new(ShaderDataType.TYPE_VEC4) },
                                    { "USERDATA3",           new(ShaderDataType.TYPE_VEC4) },
                                    { "USERDATA4",           new(ShaderDataType.TYPE_VEC4) },
                                    { "USERDATA5",           new(ShaderDataType.TYPE_VEC4) },
                                    { "USERDATA6",           new(ShaderDataType.TYPE_VEC4) },
                                    { "TRANSFORM",           new(ShaderDataType.TYPE_MAT4) },
                                    { "LIFETIME",            new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "DELTA",               new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "NUMBER",              new(ShaderDataType.TYPE_UINT, true) },
                                    { "INDEX",               new(ShaderDataType.TYPE_UINT, true) },
                                    { "EMISSION_TRANSFORM",  new(ShaderDataType.TYPE_MAT4, true) },
                                    { "RANDOM_SEED",         new(ShaderDataType.TYPE_UINT, true) },
                                    { "FLAG_EMIT_POSITION",  new(ShaderDataType.TYPE_UINT, true) },
                                    { "FLAG_EMIT_ROT_SCALE", new(ShaderDataType.TYPE_UINT, true) },
                                    { "FLAG_EMIT_VELOCITY",  new(ShaderDataType.TYPE_UINT, true) },
                                    { "FLAG_EMIT_COLOR",     new(ShaderDataType.TYPE_UINT, true) },
                                    { "FLAG_EMIT_CUSTOM",    new(ShaderDataType.TYPE_UINT, true) },
                                    { "COLLIDED",            new(ShaderDataType.TYPE_BOOL, true) },
                                    { "COLLISION_NORMAL",    new(ShaderDataType.TYPE_VEC3, true) },
                                    { "COLLISION_DEPTH",     new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "ATTRACTOR_FORCE",     new(ShaderDataType.TYPE_VEC3, true) },
                                },
                                MainFunction = true,
                                StageFunctions = new()
                                {
                                    {
                                        "emit_subparticle",
                                        new(
                                            ShaderDataType.TYPE_BOOL,
                                            new Argument("xform",    ShaderDataType.TYPE_MAT4),
                                            new Argument("velocity", ShaderDataType.TYPE_VEC3),
                                            new Argument("color",    ShaderDataType.TYPE_VEC4),
                                            new Argument("custom",   ShaderDataType.TYPE_VEC4),
                                            new Argument("flags",    ShaderDataType.TYPE_UINT)
                                        )
                                    }
                                }
                            }
                        }
                    },
                    Modes = new ModeInfo[]
                    {
                        new(PNAME("collision_use_scale")),
                        new(PNAME("disable_force")),
                        new(PNAME("disable_velocity")),
                        new(PNAME("keep_data")),
                    }
                }
            },
            {
                RS.ShaderMode.SHADER_SKY,
                new()
                {
                    Functions = new()
                    {
                        {
                            "global",
                            new()
                            {
                                BuiltIns = new()
                                {
                                    { "TIME",                new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "POSITION",            new(ShaderDataType.TYPE_VEC3, true) },
                                    { "RADIANCE",            new(ShaderDataType.TYPE_SAMPLERCUBE, true) },
                                    { "AT_HALF_RES_PASS",    new(ShaderDataType.TYPE_BOOL, true) },
                                    { "AT_QUARTER_RES_PASS", new(ShaderDataType.TYPE_BOOL, true) },
                                    { "AT_CUBEMAP_PASS",     new(ShaderDataType.TYPE_BOOL, true) },
                                    { "LIGHT0_ENABLED",      new(ShaderDataType.TYPE_BOOL, true) },
                                    { "LIGHT0_DIRECTION",    new(ShaderDataType.TYPE_VEC3, true) },
                                    { "LIGHT0_ENERGY",       new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "LIGHT0_COLOR",        new(ShaderDataType.TYPE_VEC3, true) },
                                    { "LIGHT0_SIZE",         new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "LIGHT1_ENABLED",      new(ShaderDataType.TYPE_BOOL, true) },
                                    { "LIGHT1_DIRECTION",    new(ShaderDataType.TYPE_VEC3, true) },
                                    { "LIGHT1_ENERGY",       new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "LIGHT1_COLOR",        new(ShaderDataType.TYPE_VEC3, true) },
                                    { "LIGHT1_SIZE",         new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "LIGHT2_ENABLED",      new(ShaderDataType.TYPE_BOOL, true) },
                                    { "LIGHT2_DIRECTION",    new(ShaderDataType.TYPE_VEC3, true) },
                                    { "LIGHT2_ENERGY",       new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "LIGHT2_COLOR",        new(ShaderDataType.TYPE_VEC3, true) },
                                    { "LIGHT2_SIZE",         new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "LIGHT3_ENABLED",      new(ShaderDataType.TYPE_BOOL, true) },
                                    { "LIGHT3_DIRECTION",    new(ShaderDataType.TYPE_VEC3, true) },
                                    { "LIGHT3_ENERGY",       new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "LIGHT3_COLOR",        new(ShaderDataType.TYPE_VEC3, true) },
                                    { "LIGHT3_SIZE",         new(ShaderDataType.TYPE_FLOAT, true) },
                                }
                            }
                        },
                        {
                            "constants",
                            new()
                            {
                                BuiltIns = new()
                                {
                                    { "PI",  new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "TAU", new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "E",   new(ShaderDataType.TYPE_FLOAT, true) },
                                }
                            }
                        },
                        {
                            "sky",
                            new()
                            {
                                BuiltIns = new()
                                {
                                    { "COLOR",             new(ShaderDataType.TYPE_VEC3) },
                                    { "ALPHA",             new(ShaderDataType.TYPE_FLOAT) },
                                    { "EYEDIR",            new(ShaderDataType.TYPE_VEC3, true) },
                                    { "SCREEN_UV",         new(ShaderDataType.TYPE_VEC2, true) },
                                    { "FRAGCOORD",         new(ShaderDataType.TYPE_VEC4, true) },
                                    { "SKY_COORDS",        new(ShaderDataType.TYPE_VEC2, true) },
                                    { "HALF_RES_COLOR",    new(ShaderDataType.TYPE_VEC4, true) },
                                    { "QUARTER_RES_COLOR", new(ShaderDataType.TYPE_VEC4, true) },
                                    { "FOG",               new(ShaderDataType.TYPE_VEC4) },
                                },
                                MainFunction = true,
                            }
                        },
                    },
                    Modes = new ModeInfo[]
                    {
                        new(PNAME("use_half_res_pass")),
                        new(PNAME("use_quarter_res_pass")),
                        new(PNAME("disable_fog")),
                        new(PNAME("use_debanding")),
                    },
                }
            },
            {
                RS.ShaderMode.SHADER_FOG,
                new()
                {
                    Functions = new()
                    {
                        {
                            "gloabl",
                            new()
                            {
                                BuiltIns = new()
                                {
                                    { "TIME", new(ShaderDataType.TYPE_FLOAT, true) },
                                }
                            }
                        },
                        {
                            "constants",
                            new()
                            {
                                BuiltIns = new()
                                {
                                    { "PI",  new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "TAU", new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "E",   new(ShaderDataType.TYPE_FLOAT, true) },
                                }
                            }
                        },
                        {
                            "fog",
                            new()
                            {
                                BuiltIns = new()
                                {
                                    { "WORLD_POSITION",  new(ShaderDataType.TYPE_VEC3, true) },
                                    { "OBJECT_POSITION", new(ShaderDataType.TYPE_VEC3, true) },
                                    { "UVW",             new(ShaderDataType.TYPE_VEC3, true) },
                                    { "SIZE",            new(ShaderDataType.TYPE_VEC3, true) },
                                    { "SDF",             new(ShaderDataType.TYPE_FLOAT, true) },
                                    { "ALBEDO",          new(ShaderDataType.TYPE_VEC3) },
                                    { "DENSITY",         new(ShaderDataType.TYPE_FLOAT) },
                                    { "EMISSION",        new(ShaderDataType.TYPE_VEC3) },
                                },
                                MainFunction = true,
                            }
                        }
                    }
                }
            }
        };

        this.TypesList = new()
        {
            "spatial",
            "canvas_item",
            "particles",
            "sky",
            "fog",
        };

        this.Types = this.TypesList.ToHashSet();
    }

    public Dictionary<string, ShaderLanguage.FunctionInfo> GetFunctions(RS.ShaderMode mode) => this.shaderModes[mode].Functions;
    public ModeInfo[] GetModes(RS.ShaderMode mode) => this.shaderModes[mode].Modes;
}
