namespace Godot.Net.Platforms.Windows.Native;

internal static partial class GDI32
{
    public struct PIXELFORMATDESCRIPTOR
    {
        public WORD                          nSize;
        public WORD                          nVersion;
        public PIXEL_FORMAT_DESCRIPTOR_FLAGS dwFlags;
        public PIXEL_TYPE                    iPixelType;
        public BYTE                          cColorBits;
        public BYTE                          cRedBits;
        public BYTE                          cRedShift;
        public BYTE                          cGreenBits;
        public BYTE                          cGreenShift;
        public BYTE                          cBlueBits;
        public BYTE                          cBlueShift;
        public BYTE                          cAlphaBits;
        public BYTE                          cAlphaShift;
        public BYTE                          cAccumBits;
        public BYTE                          cAccumRedBits;
        public BYTE                          cAccumGreenBits;
        public BYTE                          cAccumBlueBits;
        public BYTE                          cAccumAlphaBits;
        public BYTE                          cDepthBits;
        public BYTE                          cStencilBits;
        public BYTE                          cAuxBuffers;
        public BYTE                          iLayerType;
        public BYTE                          bReserved;
        public DWORD                         dwLayerMask;
        public DWORD                         dwVisibleMask;
        public DWORD                         dwDamageMask;
    }
}
