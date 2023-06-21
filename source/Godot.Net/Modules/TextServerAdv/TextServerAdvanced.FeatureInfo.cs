#define MODULE_FREETYPE_ENABLED

namespace Godot.Net.Modules.TextServerAdv;
using Godot.Net.Core.Config;

public partial class TextServerAdvanced
{
    private record FeatureInfo(string Name, VariantType Type, bool Hidden);
}
