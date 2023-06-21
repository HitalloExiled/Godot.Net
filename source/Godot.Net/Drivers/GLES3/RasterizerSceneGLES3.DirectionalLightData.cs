namespace Godot.Net.Drivers.GLES3;

public partial class RasterizerSceneGLES3
{
    public unsafe struct DirectionalLightData
    {
        public fixed float Color[3];
        public fixed float Direction[3];
        public uint        Enabled; // For use by SkyShaders
        public float       Energy;
        public fixed float Pad[2];
        public float       Size;
        public float       Specular;
    }
}
