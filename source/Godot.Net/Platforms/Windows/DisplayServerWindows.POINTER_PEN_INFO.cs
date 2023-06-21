namespace Godot.Net.Platforms.Windows;

public partial class DisplayServerWindows
{
    public struct POINTER_PEN_INFO
    {
        public POINTER_INFO pointerInfo;
        public uint         penFlags;
        public uint         penMask;
        public uint         pressure;
        public uint         rotation;
        public int          tiltX;
        public int          tiltY;
    };

}

