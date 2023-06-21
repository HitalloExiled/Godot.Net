namespace Godot.Net.Servers;

using Godot.Net.Servers.Rendering;

#pragma warning disable IDE0052, IDE0022  // TODO Remove

public static class RegisterServerTypesModule
{
    private static ShaderTypes? shaderTypes;

    public static void RegisterServerSingletons()
    {
        // TODO servers\register_server_types.cpp[299:307]
    }

    public static void RegisterServerTypes()
    {
        shaderTypes = new();
        // TODO servers\register_server_types.cpp[118:189]
    }

    public static void UnregisterServerTypes()
    {
        shaderTypes = null;
        // TODO servers\register_server_types.cpp[292:295]
    }
}
