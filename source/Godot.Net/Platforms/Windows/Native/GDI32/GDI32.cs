namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.InteropServices;

internal static partial class GDI32
{
    public const int PFD_MAIN_PLANE = 0;

    [LibraryImport(nameof(GDI32))]
    public static partial int ChoosePixelFormat(HDC hdc, ref PIXELFORMATDESCRIPTOR ppfd);

    [LibraryImport(nameof(GDI32))]
    public static partial HRGN CreateRectRgn(int x1, int y1, int x2, int y2);

    [LibraryImport(nameof(GDI32))]
    public static partial HBRUSH CreateSolidBrush(COLORREF color);

    [LibraryImport(nameof(GDI32))]
    public static partial BOOL DeleteObject(HGDIOBJ ho);

    [LibraryImport(nameof(GDI32))]
    public static partial int GetDeviceCaps(HDC hdc, DEVICE_CAP index);

    [LibraryImport(nameof(GDI32))]
    public static partial BOOL SetPixelFormat(HDC hdc, int format, ref PIXELFORMATDESCRIPTOR ppfd);

    [LibraryImport(nameof(GDI32))]
    public static partial BOOL SwapBuffers(HDC unnamedParam1);
}
