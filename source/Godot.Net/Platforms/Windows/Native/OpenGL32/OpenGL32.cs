namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.InteropServices;

internal static partial class OpenGL32
{
    [LibraryImport(nameof(OpenGL32), EntryPoint = "wglCreateContext")]
    public static partial HGLRC WglCreateContext(HDC hdc);

    [LibraryImport(nameof(OpenGL32), EntryPoint = "wglDeleteContext")]
    public static partial BOOL WglDeleteContext(HGLRC hglrc);

    [LibraryImport(nameof(OpenGL32), EntryPoint = "wglGetCurrentContext")]
    public static partial HGLRC WglGetCurrentContext();

    [LibraryImport(nameof(OpenGL32), EntryPoint = "wglGetCurrentDC")]
    public static partial HDC WglGetCurrentDC();

    [LibraryImport(nameof(OpenGL32), EntryPoint = "wglGetProcAddress")]
    public static partial PROC WglGetProcAddress(LPCSTR lpcstr);

    public static PROC WglGetProcAddress(string str)
    {
        using var lpcstr = new LPCSTR(str);

        return WglGetProcAddress(lpcstr);
    }

    [LibraryImport(nameof(OpenGL32), EntryPoint = "wglMakeCurrent")]
    public static partial BOOL WglMakeCurrent(HDC hdc, HGLRC hglrc);
}
