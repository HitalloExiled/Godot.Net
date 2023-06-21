namespace Godot.Net.Modules.TextServerAdv;

using System;

public partial class TextServerAdvanced
{
    private record SystemFontCacheRec
    {
		public Guid Id    { get; set; }
		public int  Index { get; set; }
	}
}
