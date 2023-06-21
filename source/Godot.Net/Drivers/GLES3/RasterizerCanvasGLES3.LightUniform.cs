namespace Godot.Net.Drivers.GLES3;
public partial class RasterizerCanvasGLES3
{
    public unsafe struct LightUniform
    {
        public fixed float AtlasRect[4];
        public fixed float Color[4];
        public uint        Flags; //index to light texture
        public float       Height;
        public fixed float Matrix[8]; //light to texture coordinate matrix
        public fixed float Position[2];
        public fixed byte  ShadowColor[4];
        public fixed float ShadowMatrix[8]; //light to shadow coordinate matrix
        public float       ShadowPixelSize;
        public float       ShadowYOfs;
        public float       ShadowZFarInv;
    }
}
