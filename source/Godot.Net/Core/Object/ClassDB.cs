namespace Godot.Net.Core.Object;

public static class ClassDB
{
    private static readonly Dictionary<string, string> classMap = new();

    public static Type? GetType(string name) =>
        classMap.TryGetValue(name, out var type) ? Type.GetType(type) : null;

    public static void Initialize()
    {
        #pragma warning disable IL2026
        foreach (var type in typeof(Program).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(GodotObject))))
        {
            if (!classMap.TryAdd(type.Name, $"{type.Namespace}.{type.Name}"))
            {
                throw new InvalidCastException($"alias '{type.Name}' already registered");
            }
        }
        #pragma warning restore IL2026
    }
}
