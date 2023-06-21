#define GLES_OVER_GL

namespace Godot.Net.Drivers.GLES3;

using Godot.Net.Core.Math;
using Godot.Net.Drivers.GLES3.Storage;

public partial class RasterizerSceneGLES3
{
    public record Sky
    {
        // Screen Buffers
        public uint         HalfResFramebuffer    { get; set; }
        public uint         HalfResPass           { get; set; }
        public uint         QuarterResFramebuffer { get; set; }
        public uint         QuarterResPass        { get; set; }
        public Vector2<int> ScreenSize            { get; set; }

        // Radiance Cubemap
        public Guid       Material            { get; set; }
        public int        MipmapCount         { get; set; } = 1;
        public RS.SkyMode Mode                { get; set; } = RS.SkyMode.SKY_MODE_AUTOMATIC;
        public uint       Radiance            { get; set; }
        public uint       RadianceFramebuffer { get; set; }
        public int        RadianceSize        { get; set; } = 256;
        public uint       RawRadiance         { get; set; }
        public uint       UniformBuffer       { get; set; }

        //ReflectionData reflection;
        public float  BakedExposure   { get; set; } = 1.0f;
        public bool   Dirty           { get; set; }
        public Sky[]? DirtyList       { get; set; }
        public int    ProcessingLayer { get; set; }
        public bool   ReflectionDirty { get; set; }

        //State to track when radiance cubemap needs updating
        public SkyMaterialData[]? PrevMaterial { get; set; }
        public Vector3<RealT>     PrevPosition { get; set; }
        public float              PrevTime     { get; set; }
    };

}
