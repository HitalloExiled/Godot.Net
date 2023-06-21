namespace Godot.Net.Servers.Text;

using System;
using System.Collections.Generic;
using Godot.Net.Core.Math;

public class TextServerExtension : TextServer
{
    public override Feature Features
    {
        get
        {
            var ret = default(Feature);
            // GDVIRTUAL_CALL(Features, ret);
            return ret;
        }
    }

    public override string Name
    {
        get
        {
            var ret = "Unknown";
            // GDVIRTUAL_CALL(Name, ret);
            return ret;
        }
    }

    public override Guid CreateShapedText(Direction direction = Direction.DIRECTION_AUTO, Orientation orientation = Orientation.ORIENTATION_HORIZONTAL) =>
        // GDVIRTUAL_CALL
        default;

    public override void DrawHexCodeBox(Guid canvas, long size, in Vector2<float> pos, long index, in Color color) => throw new NotImplementedException();
    public override void FontDrawGlyph(Guid fontId, Guid canvas, long size, in Vector2<float> pos, long index) => throw new NotImplementedException();
    public override void FontDrawGlyph(Guid fontId, Guid canvas, long size, in Vector2<float> pos, long index, in Color color) => throw new NotImplementedException();
    public override void FontDrawGlyphOutline(Guid fontId, Guid canvas, long size, long outlineSize, in Vector2<float> pos, long index) => throw new NotImplementedException();
    public override void FontDrawGlyphOutline(Guid fontId, Guid canvas, long size, long outlineSize, in Vector2<float> pos, long index, in Color color) => throw new NotImplementedException();

    public override double FontGetAscent(Guid fontId, int size) =>
        // GDVIRTUAL_CALL
        default;

    public override double FontGetDescent(Guid fontId, int size) =>
        // GDVIRTUAL_CALL
        default;

    public override void FreeId(Guid id) => throw new NotImplementedException();

    public override Vector2<RealT> GetHexCodeBoxSize(int size, int index) =>
        // if GDVIRTUAL_CALL
        // else
	    base.GetHexCodeBoxSize(size, index);

    public override bool HasFeature(Feature feature) =>
        // GDVIRTUAL_CALL
        default;

    public override bool LoadSupportData(string filename) =>
        // GDVIRTUAL_CALL
        default;

    public override HarfBuzzSharp.Tag NameToTag(string name) =>
        // GDVIRTUAL_CALL
        default;

    public override long ShapedGetSpanCount(Guid shaped) =>
        // GDVIRTUAL_CALL
        default;

    public override void ShapedSetSpanUpdateFont(Guid shaped, long index, IList<Guid> fonts, long size, Dictionary<uint, Feature>? opentypeFeatures = null)
    {
        // GDVIRTUAL_CALL
    }

    public override bool ShapedTextAddString(Guid shaped, string text, IList<Guid> fonts, long size, Dictionary<uint, Feature>? opentypeFeatures = null, string language = "", object? meta = null) =>
        // GDVIRTUAL_CALL
        default;

    public override void ShapedTextClear(Guid shaped)
    {
        // GDVIRTUAL_CALL
    }

    public override double ShapedTextFitToWidth(Guid shaped, double width, JustificationFlag jstFlags = JustificationFlag.JUSTIFICATION_KASHIDA | JustificationFlag.JUSTIFICATION_WORD_BOUND) => throw new NotImplementedException();
    public override double ShapedTextGetAscent(Guid shaped) =>
        // GDVIRTUAL_CALL
        default;
    public override CaretInfo ShapedTextGetCarets(Guid shaped, long position) => throw new NotImplementedException();
    public override double ShapedTextGetDescent(Guid shaped) =>
        // GDVIRTUAL_CALL
        default;

    public override long ShapedTextGetEllipsisGlyphCount(Guid shaped) =>
        // GDVIRTUAL_CALL
        default;

    public override IList<Glyph> ShapedTextGetEllipsisGlyphs(Guid shaped) =>
        // GDVIRTUAL_CALL
        Array.Empty<Glyph>();

    public override long ShapedTextGetEllipsisPos(Guid shaped) =>
        // GDVIRTUAL_CALL
        default;

    public override long ShapedTextGetGlyphCount(Guid shaped) =>
        // GDVIRTUAL_CALL
        default;

    public override IList<Glyph> ShapedTextGetGlyphs(Guid shaped) =>
        // GDVIRTUAL_CALL
        Array.Empty<Glyph>();

    public override Direction ShapedTextGetInferredDirection(Guid shaped) =>
	    // GDVIRTUAL_CALL
        default;

    public override IList<int> ShapedTextGetLineBreaks(Guid shaped, double width, long start = 0, LineBreakFlag breakFlags = LineBreakFlag.BREAK_MANDATORY | LineBreakFlag.BREAK_WORD_BOUND) =>
        // if GDVIRTUAL_CALL
        // else
        base.ShapedTextGetLineBreaks(shaped, width, start, breakFlags);

    public override Vector2<int> ShapedTextGetRange(Guid shaped) =>
        // GDVIRTUAL_CALL
        default;

    public override Vector2<float> ShapedTextGetSize(Guid shaped) =>
        // GDVIRTUAL_CALL
        default;

    public override long ShapedTextGetTrimPos(Guid shaped) =>
        // GDVIRTUAL_CALL
        default;

    public override bool ShapedTextIsReady(Guid shaped) =>
        // GDVIRTUAL_CALL
        default;

    public override void ShapedTextOverrunTrimToWidth(Guid shaped, double width, TextOverrunFlag trimFlags)
    {
        // GDVIRTUAL_CALL
    }

    public override void ShapedTextSetBidiOverride(Guid shaped, IList<object> @override)
    {
        // GDVIRTUAL_CALL
    }

    public override void ShapedTextSetDirection(Guid shaped, Direction direction = Direction.DIRECTION_AUTO)
    {
        // GDVIRTUAL_CALL
    }

    public override void ShapedTextSetPreserveControl(Guid shaped, bool enabled)
    {
        // GDVIRTUAL_CALL
    }

    public override void ShapedTextSetSpacing(Guid shaped, SpacingType spacing, long value)
    {
        // GDVIRTUAL_CALL
    }

    public override IList<Glyph> ShapedTextSortLogical(Guid shaped) =>
        // GDVIRTUAL_CALL
        Array.Empty<Glyph>();


    public override Guid ShapedTextSubstr(Guid shaped, int start, int length) =>
        // GDVIRTUAL_CALL
        default;

    public override bool ShapedTextUpdateBreaks(Guid shaped) => throw new NotImplementedException();
    public override string StringToUpper(string @string, string language = "") => throw new NotImplementedException();
}
