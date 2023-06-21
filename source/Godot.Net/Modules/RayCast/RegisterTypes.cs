#define TOOLS_ENABLED

namespace Godot.Net.Modules.RayCast;

public static class RegisterTypes
{
    #pragma warning disable IDE0052 // Used through the singleton
    private static RaycastOcclusionCull? raycastOcclusionCull;

    public static void InitializeRaycastModule(RegisterModulesTypesModule.ModuleInitializationLevel level)
    {
        if (level != RegisterModulesTypesModule.ModuleInitializationLevel.MODULE_INITIALIZATION_LEVEL_SCENE)
        {
            return;
        }

        #region TODO
        // #ifdef TOOLS_ENABLED
        // LightmapRaycasterEmbree::make_default_raycaster();
        // StaticRaycasterEmbree::make_default_raycaster();
        // #endif
        #endregion TODO

        raycastOcclusionCull = new RaycastOcclusionCull();
    }

    public static void UninitializeRaycastModule(RegisterModulesTypesModule.ModuleInitializationLevel level)
    {
        if (level != RegisterModulesTypesModule.ModuleInitializationLevel.MODULE_INITIALIZATION_LEVEL_SCENE)
        {
            return;
        }

        raycastOcclusionCull = null;

        #region TODO
        // #ifdef TOOLS_ENABLED
        // StaticRaycasterEmbree::free();
        // #endif
        #endregion TODO
    }
}
