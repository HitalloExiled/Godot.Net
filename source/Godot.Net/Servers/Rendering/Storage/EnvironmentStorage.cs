namespace Godot.Net.Servers.Rendering.Storage;

// Filename and type are different
public class RendererEnvironmentStorage
{
    private readonly Dictionary<Guid, object> environments = new();
    public bool IsEnvironment(Guid environment) =>
        this.environments.ContainsKey(environment);
}
