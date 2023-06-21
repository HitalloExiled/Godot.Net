#define MODULE_FREETYPE_ENABLED

namespace Godot.Net.Modules.TextServerAdv;

using System.Collections.Generic;

public partial class TextServerAdvanced
{
    private record NumSystemData
    {
        public required HashSet<string> Lang        { get; init; }
        public required string          Digits      { get; init; }
        public required string          PercentSign { get; init; }
        public required string          Exp         { get; init; }
    }
}
