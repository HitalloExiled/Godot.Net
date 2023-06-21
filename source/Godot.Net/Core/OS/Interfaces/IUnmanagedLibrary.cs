namespace Godot.Net.Core.OS.Interfaces;

public interface IUnmanagedLibrary : IDisposable
{
    bool IsLoaded { get; }
    nint GetProcAddress(string proc);
    T? GetProcAddress<T>(string proc) where T : Delegate;
}
