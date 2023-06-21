#define GLES_OVER_GL

namespace Godot.Net.Drivers.GLES3;
using Godot.Net.Core.Math;

#pragma warning disable IDE0052, CA1822, CS0414, IDE0044 // TODO REMOVE

public partial class RasterizerSceneGLES3
{
    public record SkyGlobals
    {
        public Guid                   DefaultMaterial                { get; set; }
        public Guid                   DefaultShader                  { get; set; }
        public uint                   DirectionalLightBuffer         { get; set; }
        public uint                   DirectionalLightCount          { get; set; }
        public DirectionalLightData[] DirectionalLights              { get; set; } = Array.Empty<DirectionalLightData>();
        public float                  FogAerialPerspective           { get; set; }
        public float                  FogDensity                     { get; set; }
        public bool                   FogEnabled                     { get; set; }
        public Color                  FogLightColor                  { get; set; }
        public Guid                   FogMaterial                    { get; set; }
        public Guid                   FogShader                      { get; set; }
        public float                  FogSunScatter                  { get; set; }
        public uint                   GgxSamples                     { get; set; } = 128;
        public uint                   LastFrameDirectionalLightCount { get; set; }
        public DirectionalLightData[] LastFrameDirectionalLights     { get; set; } = Array.Empty<DirectionalLightData>();
        public uint                   MaxDirectionalLights           { get; set; } = 4;
        public uint                   RadicalInverseVdcCacheTex      { get; set; }
        public uint                   RoughnessLayers                { get; set; } = 8;
        public uint                   ScreenTriangle                 { get; set; }
        public uint                   ScreenTriangleArray            { get; set; }
        public Guid                   ShaderDefaultVersion           { get; set; }
        public float                  ZFar                           { get; set; }
    }

}
