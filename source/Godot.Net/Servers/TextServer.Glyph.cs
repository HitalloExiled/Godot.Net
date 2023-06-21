namespace Godot.Net.Servers;

public abstract partial class TextServer
{
    public record Glyph : IComparable<Glyph>
    {
        public float        Advance  { get; set; } // Advance to the next glyph along baseline(x for horizontal layout, y for vertical).
        public byte         Count    { get; set; } // Number of glyphs in the grapheme, set in the first glyph only.
        public int          End      { get; set; } = -1; // End offset in the source string.
        public GraphemeFlag Flags    { get; set; } // Grapheme flags (valid, rtl, virtual), set in the first glyph only.
        public Guid         FontId   { get; set; } // Font resource.
        public int          FontSize { get; set; } // Font size;
        public int          Index    { get; set; } // Glyph index (font specific) or UTF-32 codepoint (for the invalid glyphs).
        public byte         Repeat   { get; set; } = 1; // Draw multiple times in the row.
        public int          Start    { get; set; } = -1; // Start offset in the source string.
        public float        XOff     { get; set; } // Offset from the origin of the glyph on baseline.
        public float        YOff     { get; set; }

        public int CompareTo(Glyph? other) =>
            other == null
                ? -1
                : this < other
                    ? -1
                    : this > other ? 1 : 0;

        public static bool operator <(Glyph left, Glyph right) =>
            right.Start == left.Start
                ? right.Count == left.Count
                    ? right.Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_VIRTUAL)
                    : right.Count > left.Count
                : right.Start < left.Start;

        public static bool operator >(Glyph left, Glyph right) =>
            right.Start == left.Start
                ? right.Count == left.Count
                    ? right.Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_VIRTUAL)
                    : right.Count < left.Count
                : right.Start > left.Start;
    }
}
