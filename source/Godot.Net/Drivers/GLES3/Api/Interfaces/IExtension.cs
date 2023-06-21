namespace Godot.Net.Drivers.GLES3.Api.Interfaces;

public interface IExtension
{
    static abstract string Name { get; }
    static abstract IExtension Create(ILoader loader);
}
