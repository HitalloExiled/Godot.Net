namespace Godot.Net.Platforms.Windows.Native;

#pragma warning disable CS0649

internal static partial class User32
{
    public struct CREATESTRUCTW
    {
        public LPVOID    lpCreateParams;
        public HINSTANCE hInstance;
        public HMENU     hMenu;
        public HWND      hwndParent;
        public int       cy;
        public int       cx;
        public int       y;
        public int       x;
        public LONG      style;
        public LPCWSTR   lpszName;
        public LPCWSTR   lpszClass;
        public DWORD     dwExStyle;
    }
}
