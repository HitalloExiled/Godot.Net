#define MODULE_FREETYPE_ENABLED

namespace Godot.Net.Modules.TextServerAdv;

using System;
using Godot.Net.Core.Math;
using Godot.Net.ThirdParty.Icu;

public partial class TextServerAdvanced
{
    protected partial record ShapedTextDataAdvanced
    {
        public List<BiDi>                         BidiIter        { get; } = new();
        public List<Vector3<int>>                 BidiOverride    { get; } = new();
        public Dictionary<int, bool>              Breaks          { get; } = new();
        public int[]                              ExtraSpacing    { get; } = new[] { 0, 0, 0, 0 };
        public Dictionary<int, bool>              Jstops          { get; } = new();
        public Mutex                              Mutex           { get; } = new();
        public Dictionary<object, EmbeddedObject> Objects         { get; } = new();
        public List<Span>                         Spans           { get; } = new();

        public double                             Ascent                 { get; set; } // Ascent for horizontal layout, 1/2 of width for vertical.
        public int                                BreakInserts           { get; set; }
        public bool                               BreakOpsValid          { get; set; }
        public string                             CustomPunct            { get; set; } = "";
        public double                             Descent                { get; set; } // Descent for horizontal layout, 1/2 of width for vertical.
        public Direction                          Direction              { get; set; } = Direction.DIRECTION_LTR; // Desired text direction.
        public int                                End                    { get; set; } // Substring end offset in the parent string.
        public bool                               FitWidthMinimumReached { get; set; }
        public List<Glyph>                        Glyphs                 { get; set; } = new();
        public List<Glyph>                        GlyphsLogical          { get; set; } = new();
        public HarfBuzzSharp.Buffer?              HbBuffer               { get; set; }
        public bool                               JsOpsValid             { get; set; }
        public bool                               JustificationOpsValid  { get; set; } // Virtual elongation glyphs are added to the string.
        public bool                               LineBreaksValid        { get; set; } // Line and word break flags are populated (and virtual zero width spaces inserted).
        public Orientation                        Orientation            { get; set; } = Orientation.ORIENTATION_HORIZONTAL;
        public TrimData                           OverrunTrimData        { get; set; } = new();
        public Direction                          ParaDirection          { get; set; } = Direction.DIRECTION_LTR; // Detected text direction.
        public Guid                               Parent                 { get; set; } // Substring parent ShapedTextData.
        public bool                               PreserveControl        { get; set; } // Draw control characters.
        public bool                               PreserveInvalid        { get; set; } = true; // Draw hex code box instead of missing characters.
        public ScriptIterator?                    ScriptIter             { get; set; }
        public bool                               SortValid              { get; set; }
        public int                                Start                  { get; set; } // Substring start offset in the parent string.
        public string                             Text                   { get; set; } = "";
        public bool                               TextTrimmed            { get; set; }
        public double                             Upos                   { get; set; }
        public string                             Utf16                  { get; set; } = "";
        public double                             Uthk                   { get; set; }
        public bool                               Valid                  { get; set; } // String is shaped.
        public double                             Width                  { get; set; } // Width for horizontal layout, height for vertical.
        public double                             WidthTrimmed           { get; set; }
    }
}
