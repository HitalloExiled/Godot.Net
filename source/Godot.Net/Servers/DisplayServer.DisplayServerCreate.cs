namespace Godot.Net.Servers;
public abstract partial class DisplayServer
{
    public record DisplayServerCreate(
        string             Name,
        CreateFuncDelegate CreateFunction,
        Func<List<string>> GetRenderingDriversFunction
    );
}
