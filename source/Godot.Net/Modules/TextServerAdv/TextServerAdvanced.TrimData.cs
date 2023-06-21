namespace Godot.Net.Modules.TextServerAdv;

public partial class TextServerAdvanced
{
    protected record TrimData
    {
        public List<Glyph> EllipsisGlyphBuf = new();
        public int         EllipsisPos      = -1;
        public int         TrimPos          = -1;
    }
}
