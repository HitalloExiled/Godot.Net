namespace Godot.Net.Drivers.GLES3.Api.Extensions.ARB;

using Godot.Net.Drivers.GLES3.Api.Interfaces;
using Godot.Net.Platforms.Windows.Native;

public class WglArbCreateContext : IExtension
{
    public static string Name => "WGL_ARB_create_context";

    private unsafe delegate HGLRC WglCreateContextAttribsARBext(nint hdc, nint hglrc, int* attribList);

    private readonly WglCreateContextAttribsARBext wglCreateContextAttribsARB;

    public WglArbCreateContext(ILoader loader) =>
        this.wglCreateContextAttribsARB = loader.Load<WglCreateContextAttribsARBext>("wglCreateContextAttribsARB");

    public static IExtension Create(ILoader loader) => new WglArbCreateContext(loader);

    public unsafe void CreateContextAttribsARB(HDC hdc, HGLRC hglrc, int* attribList) =>
        this.wglCreateContextAttribsARB.Invoke(hdc, hglrc, attribList);

    public unsafe void CreateContextAttribsARB(HDC hdc, HGLRC hglrc, int[] value)
    {
        fixed (int* pAttribList = value)
        {
            this.wglCreateContextAttribsARB.Invoke(hdc, hglrc, pAttribList);
        }
    }
}
