namespace Godot.Net.Drivers.GLES3;

public partial class RasterizerSceneGLES3
{
    public unsafe struct LightData
    {
        public float       Attenuation;
        public fixed float Color[3];
        public float       CosSpotAngle;
        public fixed float Direction[3]; // Only used by SpotLight
        public float       InvRadius;
        public float       InvSpotAttenuation;
        public fixed float Position[3];
        public float       ShadowOpacity;
        public float       Size;
        public float       SpecularAmount;
    }
}
