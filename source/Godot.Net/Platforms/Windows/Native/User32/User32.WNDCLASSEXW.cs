namespace Godot.Net.Platforms.Windows.Native;

internal static partial class User32
{
    /// <summary>
    /// Contains window class information. It is used with the <see cref="RegisterClassExW(WNDCLASSEXW)"/> and <see cref="GetClassInfoEx"/> functions.
    /// The <see cref="WNDCLASSEXW"/> structure is similar to the <see cref="WNDCLASS"/> structure. There are two differences. <see cref="WNDCLASSEXW"/> includes the <see cref="cbSize"/> member, which specifies the size of the structure, and the <see cref="hIconSm"/> member, which contains a handle to a small icon associated with the window class.
    /// </summary>
    public unsafe struct WNDCLASSEXW
    {
        public UINT         cbSize;
        public CLASS_STYLES style;
        public WNDPROC      lpfnWndProc;
        public int          cbClsExtra;
        public int          cbWndExtra;
        public HINSTANCE    hInstance;
        public HICON        hIcon;
        public HCURSOR      hCursor;
        public HBRUSH       hbrBackground;
        public LPCWSTR      lpszMenuName;
        public LPCWSTR      lpszClassName;
        public HICON        hIconSm;
    }
}
