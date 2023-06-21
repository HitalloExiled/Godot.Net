namespace Godot.Net.Drivers.GLES3.Api.Interfaces;

public interface ILoader : IDisposable
{
    HashSet<string> GetExtensions();
    T Load<T>(string name) where T : Delegate;
    bool TryLoad<T>(string name, out T? result) where T : Delegate;
}
