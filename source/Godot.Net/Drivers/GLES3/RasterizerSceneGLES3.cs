#define GLES_OVER_GL

namespace Godot.Net.Drivers.GLES3;

using System.Runtime.InteropServices;
using Godot.Net.Core.Generics;
using Godot.Net.Drivers.GLES3.Api.Enums;
using Godot.Net.Drivers.GLES3.Shaders;
using Godot.Net.Drivers.GLES3.Storage;
using Godot.Net.Servers.Rendering;
using Godot.Net.Servers.Rendering.Storage;

#pragma warning disable IDE0052, CA1822, CS0414, IDE0044 // TODO REMOVE

public partial class RasterizerSceneGLES3 : RendererSceneRender
{
    private static RasterizerSceneGLES3? singleton;

    public static RasterizerSceneGLES3 Singleton => singleton ?? throw new NullReferenceException();

    private readonly Queue<Sky>   dirtySkyList = new();
    private readonly SceneGlobals sceneGlobals = new();
    private readonly SceneState   sceneState   = new();
    private readonly SkyGlobals   skyGlobals   = new();

    private bool usePhysicalLightUnits;


    public RasterizerSceneGLES3()
    {
        singleton = this;

        var materialStorage = MaterialStorage.Singleton;
        var config          = Config.Singleton;
        var gl              = GL.Singleton;

        // Quality settings.
        this.usePhysicalLightUnits = GLOBAL_GET<bool>("rendering/lights_and_shadows/use_physical_light_units");

        // Setup Lights

        config.MaxRenderableLights = Math.Min(config.MaxRenderableLights, config.MaxUniformBufferSize / Marshal.SizeOf<LightData>());
        config.MaxLightsPerObject  = Math.Min(config.MaxLightsPerObject,  config.MaxRenderableLights);

        var lightBufferSize = (uint)(config.MaxRenderableLights * Marshal.SizeOf<LightData>());

        this.sceneState.OmniLights    = Common.FillArray<LightData>(config.MaxRenderableLights);
        this.sceneState.OmniLightSort = Common.FillArray<InstanceSort<LightInstance>>(() => new(new(), 0f), config.MaxRenderableLights);

        gl.GenBuffers(out var omniLightBuffer);
        this.sceneState.OmniLightBuffer = omniLightBuffer;

        gl.BindBuffer(BufferTargetARB.UniformBuffer, this.sceneState.OmniLightBuffer);
        gl.BufferData(BufferTargetARB.UniformBuffer, lightBufferSize, default, BufferUsageARB.StreamDraw);

        this.sceneState.SpotLights    = Common.FillArray<LightData>(config.MaxRenderableLights);
        this.sceneState.SpotLightSort = Common.FillArray<InstanceSort<LightInstance>>(() => new(new(), 0f), config.MaxRenderableLights);

        gl.GenBuffers(out var spotLightBuffer);
        this.sceneState.SpotLightBuffer = spotLightBuffer;

        gl.BindBuffer(BufferTargetARB.UniformBuffer, this.sceneState.SpotLightBuffer);
        gl.BufferData(BufferTargetARB.UniformBuffer, lightBufferSize, default, BufferUsageARB.StreamDraw);

        var directionalLightBufferSize = (uint)(MAX_DIRECTIONAL_LIGHTS * Marshal.SizeOf<DirectionalLightData>());

        this.sceneState.DirectionalLights = Common.FillArray<DirectionalLightData>(MAX_DIRECTIONAL_LIGHTS);

        gl.GenBuffers(out var sceneStateDirectionalLightBuffer);
        this.sceneState.DirectionalLightBuffer = sceneStateDirectionalLightBuffer;

        gl.BindBuffer(BufferTargetARB.UniformBuffer, this.sceneState.DirectionalLightBuffer);
        gl.BufferData(BufferTargetARB.UniformBuffer, directionalLightBufferSize, default, BufferUsageARB.StreamDraw);
        gl.BindBuffer(BufferTargetARB.UniformBuffer, 0);

        this.skyGlobals.MaxDirectionalLights = 4;

        directionalLightBufferSize = (uint)(this.skyGlobals.MaxDirectionalLights * Marshal.SizeOf<DirectionalLightData>());

        this.skyGlobals.DirectionalLights              = Common.FillArray<DirectionalLightData>(this.skyGlobals.MaxDirectionalLights);
        this.skyGlobals.LastFrameDirectionalLights     = Common.FillArray<DirectionalLightData>(this.skyGlobals.MaxDirectionalLights);
        this.skyGlobals.LastFrameDirectionalLightCount = this.skyGlobals.MaxDirectionalLights + 1;

        gl.GenBuffers(out var skyGlobalsDirectionalLightBuffer);
        this.skyGlobals.DirectionalLightBuffer = skyGlobalsDirectionalLightBuffer;

        gl.BindBuffer(BufferTargetARB.UniformBuffer, this.skyGlobals.DirectionalLightBuffer);
        gl.BufferData(BufferTargetARB.UniformBuffer, directionalLightBufferSize, default, BufferUsageARB.StreamDraw);
        gl.BindBuffer(BufferTargetARB.UniformBuffer, 0);

        // TODO: MAX_GLOBAL_SHADER_UNIFORMS is arbitrary for now
        var globalDefines =
            $"""
            #define MAX_GLOBAL_SHADER_UNIFORMS         256
            #define MAX_LIGHT_DATA_STRUCTS             {config.MaxRenderableLights}
            #define MAX_DIRECTIONAL_LIGHT_DATA_STRUCTS {MAX_DIRECTIONAL_LIGHTS}
            #define MAX_FORWARD_LIGHTS                 uint({config.MaxLightsPerObject})
            """;

        materialStorage.Shaders.SceneShader.Initialize(globalDefines);

        this.sceneGlobals.ShaderDefaultVersion = materialStorage.Shaders.SceneShader.VersionCreate();

        materialStorage.Shaders.SceneShader.VersionBindShader(this.sceneGlobals.ShaderDefaultVersion, SceneShaderGLES3.ShaderVariant.MODE_COLOR);

        //default material and shader
        this.sceneGlobals.DefaultShader = Guid.NewGuid();
        materialStorage.ShaderInitialize(this.sceneGlobals.DefaultShader);
        materialStorage.ShaderSetCode(
            this.sceneGlobals.DefaultShader,
            """
            // Default 3D material shader.

            shader_type spatial;

            void vertex() {
                ROUGHNESS = 0.8;
            }

            void fragment() {
                ALBEDO = vec3(0.6);
                ROUGHNESS = 0.8;
                METALLIC = 0.2;
            }
            """
        );

        this.sceneGlobals.DefaultMaterial = Guid.NewGuid();

        materialStorage.MaterialInitialize(this.sceneGlobals.DefaultMaterial);
        materialStorage.MaterialSetShader(this.sceneGlobals.DefaultMaterial, this.sceneGlobals.DefaultShader);

        // Initialize Sky stuff
        this.skyGlobals.RoughnessLayers = GLOBAL_GET<uint>("rendering/reflections/sky_reflections/roughness_layers");
        this.skyGlobals.GgxSamples      = GLOBAL_GET<uint>("rendering/reflections/sky_reflections/ggx_samples");

        // TODO: MAX_GLOBAL_SHADER_UNIFORMS is arbitrary for now
        globalDefines =
            $"""
            #define MAX_GLOBAL_SHADER_UNIFORMS         256
            #define MAX_DIRECTIONAL_LIGHT_DATA_STRUCTS {this.skyGlobals.MaxDirectionalLights}
            """;

        materialStorage.Shaders.SkyShader.Initialize(globalDefines);
        this.skyGlobals.ShaderDefaultVersion = materialStorage.Shaders.SkyShader.VersionCreate();

        globalDefines = $"#define MAX_SAMPLE_COUNT {this.skyGlobals.GgxSamples}";
        materialStorage.Shaders.CubemapFilterShader.Initialize(globalDefines);
        this.sceneGlobals.CubemapFilterShaderVersion = materialStorage.Shaders.CubemapFilterShader.VersionCreate();

        this.skyGlobals.DefaultShader = Guid.NewGuid();

        materialStorage.ShaderInitialize(this.skyGlobals.DefaultShader);

        materialStorage.ShaderSetCode(
            this.skyGlobals.DefaultShader,
            """
            // Default sky shader.

            shader_type sky;

            void sky() {
                COLOR = vec3(0.0);
            }
            """
        );

        this.skyGlobals.DefaultMaterial = Guid.NewGuid();
        materialStorage.MaterialInitialize(this.skyGlobals.DefaultMaterial);

        materialStorage.MaterialSetShader(this.skyGlobals.DefaultMaterial, this.skyGlobals.DefaultShader);

        this.skyGlobals.FogShader = Guid.NewGuid();
        materialStorage.ShaderInitialize(this.skyGlobals.FogShader);

        materialStorage.ShaderSetCode(
            this.skyGlobals.FogShader,
            """
            // Default clear color sky shader.

            shader_type sky;

            uniform vec4 clear_color;

            void sky() {
                COLOR = clear_color.rgb;
            }
            """
        );

        this.skyGlobals.FogMaterial = Guid.NewGuid();

        materialStorage.MaterialInitialize(this.skyGlobals.FogMaterial);
        materialStorage.MaterialSetShader(this.skyGlobals.FogMaterial, this.skyGlobals.FogShader);

        gl.GenBuffers(out var screenTriangle);
        this.skyGlobals.ScreenTriangle = screenTriangle;

        gl.BindBuffer(BufferTargetARB.ArrayBuffer, this.skyGlobals.ScreenTriangle);

        var qv = new[]
        {
            -1.0f,
            -1.0f,
             3.0f,
            -1.0f,
            -1.0f,
             3.0f,
        };

        gl.BufferData(BufferTargetARB.ArrayBuffer, qv, BufferUsageARB.StaticDraw);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0); //unbind

        gl.GenVertexArrays(out var screenTriangleArray);
        this.skyGlobals.ScreenTriangleArray = screenTriangleArray;

        gl.BindVertexArray(this.skyGlobals.ScreenTriangleArray);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, this.skyGlobals.ScreenTriangle);
        gl.VertexAttribPointer((uint)RS.ArrayType.ARRAY_VERTEX, 2, VertexAttribPointerType.Float, false, sizeof(float) * 2, default);
        gl.EnableVertexAttribArray((uint)RS.ArrayType.ARRAY_VERTEX);
        gl.BindVertexArray(0);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0); //unbind

        #if GLES_OVER_GL
        gl.Enable(EnableCap.TextureCubeMapSeamless);
        #endif

        // MultiMesh may read from color when color is disabled, so make sure that the color defaults to white instead of black;
        gl.VertexAttrib4f((uint)RS.ArrayType.ARRAY_COLOR, 1.0f, 1.0f, 1.0f, 1.0f);
    }

    private void UpdateDirtySkys()
    {
        #region TODO
        // while (this.dirtySkyList.Count > 0)
        // {
        // var sky = this.dirtySkyList.Dequeue();
        //     if (sky.Radiance == 0) {
        //         sky.MipmapCount = Image::get_image_required_mipmaps(sky.RadianceSize, sky.RadianceSize, Image::FORMAT_RGBA8) - 1;
        //         // Left uninitialized, will attach a texture at render time
        //         glGenFramebuffers(1, &sky.RadianceFramebuffer);

        //         GLenum internal_format = GL_RGB10_A2;

        //         glGenTextures(1, &sky.Radiance);
        //         glBindTexture(GL_TEXTURE_CUBE_MAP, sky.Radiance);

        // #if GLES_OVER_GL
        //         GLenum format = GL_RGBA;
        //         GLenum type = GL_UNSIGNED_INT_2_10_10_10_REV;
        //         //TODO, on low-end compare this to allocating each face of each mip individually
        //         // see: https://www.khronos.org/registry/OpenGL-Refpages/es3.0/html/glTexStorage2D.xhtml
        //         for (int i = 0; i < 6; i++) {
        //             glTexImage2D(GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, 0, internal_format, sky.RadianceSize, sky.RadianceSize, 0, format, type, nullptr);
        //         }

        //         glGenerateMipmap(GL_TEXTURE_CUBE_MAP);
        // #else
        //         glTexStorage2D(GL_TEXTURE_CUBE_MAP, sky.MipmapCount, internal_format, sky.RadianceSize, sky.RadianceSize);
        // #endif
        //         glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
        //         glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
        //         glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
        //         glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
        //         glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_BASE_LEVEL, 0);
        //         glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MAX_LEVEL, sky.MipmapCount - 1);

        //         glGenTextures(1, &sky.RawRadiance);
        //         glBindTexture(GL_TEXTURE_CUBE_MAP, sky.RawRadiance);

        // #if GLES_OVER_GL
        //         //TODO, on low-end compare this to allocating each face of each mip individually
        //         // see: https://www.khronos.org/registry/OpenGL-Refpages/es3.0/html/glTexStorage2D.xhtml
        //         for (int i = 0; i < 6; i++) {
        //             glTexImage2D(GL_TEXTURE_CUBE_MAP_POSITIVE_X + i, 0, internal_format, sky.RadianceSize, sky.RadianceSize, 0, format, type, nullptr);
        //         }

        //         glGenerateMipmap(GL_TEXTURE_CUBE_MAP);
        // #else
        //         glTexStorage2D(GL_TEXTURE_CUBE_MAP, sky.MipmapCount, internal_format, sky.RadianceSize, sky.RadianceSize);
        // #endif
        //         glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
        //         glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
        //         glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_S, GL_CLAMP_TO_EDGE);
        //         glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_WRAP_T, GL_CLAMP_TO_EDGE);
        //         glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_BASE_LEVEL, 0);
        //         glTexParameteri(GL_TEXTURE_CUBE_MAP, GL_TEXTURE_MAX_LEVEL, sky.MipmapCount - 1);

        //         glBindTexture(GL_TEXTURE_CUBE_MAP, 0);
        //     }

        //     sky.ReflectionDirty = true;
        //     sky.ProcessingLayer = 0;

        //     Sky *next = sky.DirtyList;
        //     sky.DirtyList = nullptr;
        //     sky.Dirty = false;
        //     sky = next;
        // }
        #endregion TODO
    }

    #pragma warning disable CA1822
    public RenderSceneBuffers RenderBuffersCreate() =>
        new RenderSceneBuffersGLES3();
    #pragma warning restore CA1822

    public override void Update() => this.UpdateDirtySkys();

}
