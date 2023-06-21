namespace Godot.Net.Servers.Rendering.Storage;

// Filename and type are different
public class RendererCameraAttributes
{
    private static RendererCameraAttributes? singleton;

    public static RendererCameraAttributes Singleton => singleton ?? throw new NotImplementedException();

    public RendererCameraAttributes() => singleton = this;
}
