namespace Godot.Net.Modules.TextServerAdv;

using System;

public partial class TextServerAdvanced
{
    protected partial record ShapedTextDataAdvanced
    {
        public record Span
        {
			public object?                   EmbeddedKey { get; set; }
			public int                       End         { get; set; } = -1;
			public Dictionary<uint, Feature> Features    { get; set; } = new();
			public IList<Guid>               Fonts       { get; set; } = Array.Empty<Guid>();
			public int                       FontSize    { get; set; }
			public string?                   Language    { get; set; }
			public object?                   Meta        { get; set; }
			public int                       Start       { get; set; } = -1;
		}
	}
}
