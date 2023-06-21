namespace Godot.Net.Servers;

using Godot.Net.Servers.XR;

#pragma warning disable IDE0044, IDE0051, CS0414 // TODO Remove;

public partial class XRServer
{
    private static XRServer? singleton;

    public static XRMode XrMode { get; set; }

    private float worldScale = 1;

    public static bool     IsInitialized => singleton != null;
    public static XRServer Singleton     => singleton ?? throw new NullReferenceException();

    public XRInterface? PrimaryInterface
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public XRServer() =>
        singleton = this;

    public void PreRender() => throw new NotImplementedException();
}
