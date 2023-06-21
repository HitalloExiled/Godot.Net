namespace Godot.Net.Modules.TextServerAdv;

using System.Collections.Generic;

public partial class TextServerAdvanced
{
    private record SystemFontCache
    {
		public List<SystemFontCacheRec> Var { get; } = new();

		public int MaxVar { get; set; }
	}
}
