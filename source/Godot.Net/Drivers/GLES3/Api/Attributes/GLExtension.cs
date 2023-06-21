namespace Godot.Net.Drivers.GLES3.Api.Attributes;

[AttributeUsage(AttributeTargets.All)]
public class GLExtensionAttribute : Attribute
{
    public string Name { get; }

    public GLExtensionAttribute(string name) => this.Name = name;
}
