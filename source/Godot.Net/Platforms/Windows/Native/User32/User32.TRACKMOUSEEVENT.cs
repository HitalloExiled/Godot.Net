namespace Godot.Net.Platforms.Windows.Native;

internal static partial class User32
{
    public struct TRACKMOUSEEVENT
    {
        public DWORD                 cbSize;
        public TRACKMOUSEEVENT_FLAGS dwFlags;
        public HWND                  hwndTrack;
        public DWORD                 dwHoverTime;
    }
}
