#define GLES_OVER_GL

namespace Godot.Net.Drivers.GLES3;

using System.Diagnostics;
using System.Runtime.InteropServices;
using Godot.Net.Drivers.GLES3.Storage;

#pragma warning disable IDE0052, CA1822, CS0414, IDE0044 // TODO REMOVE

public partial class RasterizerSceneGLES3
{
    public partial record SceneState
    {
        public SceneShaderData.Cull          CullMode               { get; set; } = SceneShaderData.Cull.CULL_BACK;
        public SceneShaderData.BlendModeKind     CurrentBlendMode       { get; set; } = SceneShaderData.BlendModeKind.BLEND_MODE_MIX;
        public SceneShaderData.DepthDrawKind     CurrentDepthDraw       { get; set; } = SceneShaderData.DepthDrawKind.DEPTH_DRAW_OPAQUE;
        public SceneShaderData.DepthTestKind     CurrentDepthTest       { get; set; } = SceneShaderData.DepthTestKind.DEPTH_TEST_DISABLED;
        public uint                          DirectionalLightBuffer { get; set; }
        public DirectionalLightData[]        DirectionalLights      { get; set; } = Array.Empty<DirectionalLightData>();
        public uint                          MultiviewBuffer        { get; set; }
        public MultiviewUBO                  MultiviewUbo           { get; set; }
        public uint                          OmniLightBuffer        { get; set; }
        public uint                          OmniLightCount         { get; set; }
        public InstanceSort<LightInstance>[] OmniLightSort          { get; set; } = Array.Empty<InstanceSort<LightInstance>>();
        public LightData[]                   OmniLights             { get; set; } = Array.Empty<LightData>();
        public uint                          SpotLightBuffer        { get; set; }
        public uint                          SpotLightCount         { get; set; }
        public InstanceSort<LightInstance>[] SpotLightSort          { get; set; } = Array.Empty<InstanceSort<LightInstance>>();
        public LightData[]                   SpotLights             { get; set; } = Array.Empty<LightData>();
        public bool                          TexscreenCopied        { get; set; }
        public uint                          TonemapBuffer          { get; set; }
        public UBO                           Ubo                    { get; set; }
        public uint                          UboBuffer              { get; set; }
        public bool                          UsedDepthPrepass       { get; set; }
        public bool                          UsedDepthTexture       { get; set; }
        public bool                          UsedNormalTexture      { get; set; }
        public bool                          UsedScreenTexture      { get; set; }

        #if DEBUG
        static SceneState()
        {
            Debug.Assert(Marshal.SizeOf<UBO>() % 16 == 0, "Scene UBO size must be a multiple of 16 bytes");
            Debug.Assert(Marshal.SizeOf<MultiviewUBO>() % 16 == 0, "Multiview UBO size must be a multiple of 16 bytes");
            Debug.Assert(Marshal.SizeOf<TonemapUBO>() % 16 == 0, "Tonemap UBO size must be a multiple of 16 bytes");
        }
        #endif
    }
}
