namespace Godot.Net.Platforms.Windows.Native;

#pragma warning disable CS0649

/// <content>
/// Contains the <see cref="WNDCLASSEXW"/> nested type.
/// </content>
internal static partial class User32
{
    /// <summary>
    /// Contains window class information. It is used with the <see cref="RegisterClassExW"/> and <see cref="GetClassInfoExW"/> functions.
    /// The <see cref="WNDCLASSEXW"/> structure is similar to the <see cref="WndClass"/> structure. There are two differences. <see cref="WNDCLASSEXW"/> includes the <see cref="cbSize"/> member, which specifies the size of the structure, and the <see cref="hIconSm"/> member, which contains a handle to a small icon associated with the window class.
    /// </summary>
    public unsafe struct MOUSEHOOKSTRUCT
    {
        public POINT     pt;
        public HWND      hwnd;
        public UINT      wHitTestCode;
        public ULONG_PTR dwExtraInfo;
    }
}
