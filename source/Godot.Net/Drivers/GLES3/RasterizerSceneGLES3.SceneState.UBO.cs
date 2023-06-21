#define GLES_OVER_GL

namespace Godot.Net.Drivers.GLES3;
#pragma warning disable IDE0052, CA1822, CS0414, IDE0044 // TODO REMOVE

public partial class RasterizerSceneGLES3
{
    public partial record SceneState
    {
        public unsafe struct UBO
        {
            public float       AmbientColorSkyMix;
            public fixed float AmbientLightColorEnergy[4];
            public uint        CameraVisibleLayers;
            public uint        DirectionalLightCount;
            public float       EmissiveExposureNormalization;
            public float       FogAerialPerspective;
            public float       FogDensity;
            public uint        FogEnabled;
            public float       FogHeight;
            public float       FogHeightDensity;
            public fixed float FogLightColor[3];
            public float       FogSunScatter;
            public float       IblExposureNormalization;
            public fixed float InvProjectionMatrix[16];
            public fixed float InvViewMatrix[16];
            public uint        MaterialUv2Mode;
            public uint        Pad1;
            public uint        Pad2;
            public uint        Pad3;
            public fixed float ProjectionMatrix[16];
            public fixed float RadianceInverseXform[12];
            public fixed float ScreenPixelSize[2];
            public float       Time;
            public uint        UseAmbientCubemap = 0;
            public uint        UseAmbientLight = 0;
            public uint        UseReflectionCubemap = 0;
            public fixed float ViewMatrix[16];
            public fixed float ViewportSize[2];
            public float       ZFar;
            public float       ZNear;

            public UBO()
            { }
        }
    }

}
