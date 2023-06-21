namespace Godot.Net.Drivers.GLES3;

using Godot.Net.Drivers.GLES3.Api;

public class GL
{
    private static OpenGL? singleton;
    public static OpenGL Singleton => singleton ?? throw new NullReferenceException();

    public static void Initialize() => singleton = new(new WindowsLoader());
    public static void Finish()
    {
        singleton?.Dispose();
        singleton = null;
    }
}
