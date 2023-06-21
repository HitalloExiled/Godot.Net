namespace Godot.Net.Platforms.Windows;

using Godot.Net.Platforms.Windows.Native;

public partial class DisplayServerWindows
{
    public struct POINTER_INFO
    {
        public POINTER_INPUT_TYPE         pointerType;
        public uint                       pointerId;
        public uint                       frameId;
        public uint                       pointerFlags;
        public nint                       sourceDevice;
        public nint                       hwndTarget;
        public POINT                      ptPixelLocation;
        public POINT                      ptHimetricLocation;
        public POINT                      ptPixelLocationRaw;
        public POINT                      ptHimetricLocationRaw;
        public nint                       dwTime;
        public uint                       historyCount;
        public int                        InputData;
        public nint                       dwKeyStates;
        public ulong                      PerformanceCount;
        public POINTER_BUTTON_CHANGE_TYPE ButtonChangeType;
    }
}

