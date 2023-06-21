namespace Godot.Net.Drivers.GLES3.Api.Extensions.ARB;

using Godot.Net.Drivers.GLES3.Api.Interfaces;


public class ArbUniformBufferObject : IExtension
{
    public static string Name => "GL_ARB_uniform_buffer_object";

    public static IExtension Create(ILoader loader) => throw new NotImplementedException();
}
