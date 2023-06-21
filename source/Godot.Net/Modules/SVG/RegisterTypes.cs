#define TOOLS_ENABLED

namespace Godot.Net.Modules.SVG;

using Godot.Net.Core.IO;

public static class RegisterTypes
{
    private static ImageLoaderSVG? imageLoaderSvg;

    public static void InitializeSvgModule(RegisterModulesTypesModule.ModuleInitializationLevel level)
    {
        if (level != RegisterModulesTypesModule.ModuleInitializationLevel.MODULE_INITIALIZATION_LEVEL_SCENE)
        {
            return;
        }

        // tvg::CanvasEngine tvgEngine = tvg::CanvasEngine::Sw;
        // if (tvg::Initializer::init(tvgEngine, 1) != tvg::Result::Success)
        // {
        //     return;
        // }

        imageLoaderSvg = new();
        ImageLoader.AddImageFormatLoader(imageLoaderSvg);
    }

    public static void UninitializeSvgModule(RegisterModulesTypesModule.ModuleInitializationLevel level)
    {
        if (level != RegisterModulesTypesModule.ModuleInitializationLevel.MODULE_INITIALIZATION_LEVEL_SCENE)
        {
            return;
        }

        if (imageLoaderSvg == null)
        {
            // It failed to initialize so it was not added.
            return;
        }

        ImageLoader.RemoveImageFormatLoader(imageLoaderSvg);
        // tvg::Initializer::term(tvg::CanvasEngine::Sw);
    }
}
