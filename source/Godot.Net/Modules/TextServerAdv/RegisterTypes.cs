namespace Godot.Net.Modules.TextServerAdv;

using Godot.Net.Servers;
using Godot.Net.ThirdParty.Icu;
using static Godot.Net.Modules.RegisterModulesTypesModule;

public static class RegisterTypes
{
    public static void InitializeTextServerAdvModule(ModuleInitializationLevel level)
    {
        if (level != ModuleInitializationLevel.MODULE_INITIALIZATION_LEVEL_SERVERS)
        {
            return;
        }

        // GDREGISTER_CLASS(TextServerAdvanced);
        var tsman = TextServerManager.Singleton;

        tsman?.AddInterface(new TextServerAdvanced());
        IcuNative.Initialize();
    }

    public static void UninitializeTextServerAdvModule(ModuleInitializationLevel level)
    {
        if (level != ModuleInitializationLevel.MODULE_INITIALIZATION_LEVEL_SERVERS)
        {
            return;
        }

        IcuNative.Close();
    }
}
