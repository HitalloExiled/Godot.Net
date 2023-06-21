#define MODULE_FREETYPE_ENABLED

namespace Godot.Net.Modules.TextServerAdv;

using Godot.Net.Core.Enums;
using Godot.Net.Core.Math;

public partial class TextServerAdvanced
{
    protected partial record ShapedTextDataAdvanced
    {
        public record EmbeddedObject
        {
			public double          Baseline    { get; set; }
			public InlineAlignment InlineAlign { get; set; } = InlineAlignment.INLINE_ALIGNMENT_CENTER;
			public int             Pos         { get; set; }
			public Rect2<RealT>    Rect        { get; set; }
		}
	}
}
