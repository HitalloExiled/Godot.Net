namespace Godot.Net.Servers;

#pragma warning disable IDE0044, CS0649 // TODO - REMOVE

public class CameraServer
{
    private static Func<CameraServer>? createFunc;
    public static CameraServer Create() => createFunc?.Invoke() ?? new();
}
