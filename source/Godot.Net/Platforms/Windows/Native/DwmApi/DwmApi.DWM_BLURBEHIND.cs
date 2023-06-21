namespace Godot.Net.Platforms.Windows.Native;

internal static partial class DwmApi
{
    public struct DWM_BLURBEHIND
    {
        public DWORD dwFlags;
        public BOOL  fEnable;
        public HRGN  hRgnBlur;
        public BOOL  fTransitionOnMaximized;
    }
}
