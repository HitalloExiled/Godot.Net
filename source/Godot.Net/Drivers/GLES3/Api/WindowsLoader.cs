namespace Godot.Net.Drivers.GLES3.Api;

using System.Runtime.InteropServices;
using Godot.Net.Drivers.GLES3.Api.Interfaces;
using Godot.Net.Platforms.Windows;
using Godot.Net.Platforms.Windows.Native;

public class WindowsLoader : ILoader
{
    private delegate nint WglGetExtensionsStringARB(nint hdc);
    private delegate nint WglGetExtensionsStringEXT();
    private delegate nint WglGetProcAddress(nint proc);

    private readonly WglGetProcAddress wglGetProcAddress;

    private readonly WindowsUnmanagedLibrary library;

    private bool disposed;

    public WindowsLoader(string lib = "OpenGL32")
    {
        this.library = new WindowsUnmanagedLibrary(lib);

        this.wglGetProcAddress = this.library.GetProcAddress<WglGetProcAddress>("wglGetProcAddress")!;
    }

    private T? GetProcAddress<T>(string proc) where T : Delegate
    {
        using var lpproc = new StringA(proc);

        var pointer = this.wglGetProcAddress.Invoke(lpproc.Value);

        return pointer != default ? Marshal.GetDelegateForFunctionPointer<T>(pointer) : null;
    }

    protected void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.library.Dispose();
            }

            this.disposed = true;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    public HashSet<string> GetExtensions()
    {
        var hashSet = new HashSet<string>();

        void add(nint? buffer)
        {
            if (!buffer.HasValue)
            {
                return;
            }

            var content = Marshal.PtrToStringAnsi(buffer.Value) ?? "";

            foreach (var entry in content.Split(" "))
            {
                hashSet.Add(entry);
            }
        }

        if (this.TryLoad<WglGetExtensionsStringEXT>("wglGetExtensionsStringEXT", out var wglGetExtensionsStringEXT))
        {
            add(wglGetExtensionsStringEXT!.Invoke());
        }

        if (this.TryLoad<WglGetExtensionsStringARB>("wglGetExtensionsStringARB", out var wglGetExtensionsStringARB))
        {
            add(wglGetExtensionsStringARB!.Invoke(OpenGL32.WglGetCurrentDC()));
        }

        return hashSet;
    }

    public T Load<T>(string name) where T : Delegate =>
        this.TryLoad<T>(name, out var function) ? function! : throw new LoadSymbolException(name);

    public bool TryLoad<T>(string name, out T? result) where T : Delegate
    {
        result = this.GetProcAddress<T>(name) ?? this.library.GetProcAddress<T>(name);
        return result != null;
    }
}
