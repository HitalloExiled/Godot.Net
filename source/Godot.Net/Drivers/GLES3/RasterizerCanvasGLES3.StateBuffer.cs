namespace Godot.Net.Drivers.GLES3;
public partial class RasterizerCanvasGLES3
{
    public unsafe struct StateBuffer
    {
        public fixed float CanvasModulate[4];
        public fixed float CanvasNormalTransform[16];
        public fixed float CanvasTransform[16];
        public uint        DirectionalLightCount;
        public uint        Pad1;
        public uint        Pad2;
        public fixed float ScreenPixelSize[2];
        public fixed float ScreenToSdf[2];
        public fixed float ScreenTransform[16];
        public fixed float SdfToScreen[2];
        public fixed float SdfToTex[4];
        public float       TexToSdf;
        public float       Time;
        public uint        UsePixelSnap;
    }
}
