namespace Godot.Net.Drivers.GLES3;
public partial class RasterizerCanvasGLES3
{
    public unsafe struct InstanceData
    {
        public fixed float ColorTexturePixelSize[2];
        public int         Flags;
        public fixed uint  Lights[4];
        public uint        SpecularShininess;
        public fixed float World[6];
        #region rect
        public fixed float DstRect[4];
        public fixed float Modulation[4];
        public fixed float Msdf[4];
        public fixed float NinepatchMargins[4];
        public fixed float Pad[2];
        public fixed float SrcRect[4];
        #endregion rect
        #region primitive
        public fixed uint  Colors[6]; // colors encoded as half
        public fixed float Points[6]; // vec2 points[3]
        public fixed float Uvs[6]; // vec2 points[3]
        #endregion primitive
    };
}
