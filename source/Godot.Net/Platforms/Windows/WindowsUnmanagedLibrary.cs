namespace Godot.Net.Platforms.Windows;

using System.Runtime.InteropServices;
using Godot.Net.Core.OS.Interfaces;
using Godot.Net.Platforms.Windows.Native;

public class WindowsUnmanagedLibrary : IUnmanagedLibrary
{
    private bool disposed;

    private readonly HMODULE handler;

    public bool IsLoaded => this.handler != default;

    public WindowsUnmanagedLibrary(string lib) =>
        this.handler = Kernel32.LoadLibraryExW(lib, default, Kernel32.LOAD_LIBRARY_FLAGS.LOAD_LIBRARY_SEARCH_DEFAULT_DIRS | Kernel32.LOAD_LIBRARY_FLAGS.LOAD_LIBRARY_SEARCH_DLL_LOAD_DIR);
        // this.handler = Kernel32.LoadLibraryW(lib);

    ~WindowsUnmanagedLibrary() =>
        this.Dispose(disposing: false);

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            Kernel32.FreeLibrary(this.handler);

            this.disposed = true;
        }
    }

    public nint GetProcAddress(string proc) => Kernel32.GetProcAddress(this.handler, proc);

    public T? GetProcAddress<T>(string proc) where T : Delegate
    {
        var pointer = Kernel32.GetProcAddress(this.handler, proc);

        return pointer != default ? Marshal.GetDelegateForFunctionPointer<T>(pointer) : null;
    }

    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
