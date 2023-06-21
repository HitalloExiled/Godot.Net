namespace Godot.Net.Servers;

using Godot.Net.Core.Math;
using Godot.Net.Extensions;

using HbTag = HarfBuzzSharp.Tag;

public abstract partial class TextServer
{
    public virtual string Name => "";

    public abstract Feature Features { get; }

    public static IList<Vector3<int>> ParseStructuredText(StructuredTextParser parserType, IList<object> args, string text)
    {
        var ret = new List<Vector3<int>>();

        switch (parserType)
        {
            case StructuredTextParser.STRUCTURED_TEXT_URI:
                {
                    var prev = 0;
                    for (var i = 0; i < text.Length; i++)
                    {
                        if (text[i] == '\\' || text[i] == '/' || text[i] == '.' || text[i] == ':' || text[i] == '&' || text[i] == '=' || text[i] == '@' || text[i] == '?' || text[i] == '#')
                        {
                            if (prev != i)
                            {
                                ret.Add(new(prev, i, (int)Direction.DIRECTION_AUTO));
                            }
                            ret.Add(new(i, i + 1, (int)Direction.DIRECTION_LTR));
                            prev = i + 1;
                        }
                    }
                    if (prev != text.Length)
                    {
                        ret.Add(new(prev, text.Length, (int)Direction.DIRECTION_AUTO));
                    }
                }
                break;
            case StructuredTextParser.STRUCTURED_TEXT_FILE:
                {
                    var prev = 0;
                    for (var i = 0; i < text.Length; i++)
                    {
                        if (text[i] == '\\' || text[i] == '/' || text[i] == ':')
                        {
                            if (prev != i)
                            {
                                ret.Add(new(prev, i, (int)Direction.DIRECTION_AUTO));
                            }
                            ret.Add(new(i, i + 1, (int)Direction.DIRECTION_LTR));
                            prev = i + 1;
                        }
                    }
                    if (prev != text.Length)
                    {
                        ret.Add(new(prev, text.Length, (int)Direction.DIRECTION_AUTO));
                    }
                }
                break;
            case StructuredTextParser.STRUCTURED_TEXT_EMAIL:
                {
                    var local = true;
                    var prev = 0;
                    for (var i = 0; i < text.Length; i++)
                    {
                        if (text[i] == '@' && local)
                        { // Add full "local" as single context.
                            local = false;
                            ret.Add(new(prev, i, (int)Direction.DIRECTION_AUTO));
                            ret.Add(new(i, i + 1, (int)Direction.DIRECTION_LTR));
                            prev = i + 1;
                        }
                        else if (!local && text[i] == '.')
                        { // Add each dot separated "domain" part as context.
                            if (prev != i)
                            {
                                ret.Add(new(prev, i, (int)Direction.DIRECTION_AUTO));
                            }
                            ret.Add(new(i, i + 1, (int)Direction.DIRECTION_LTR));
                            prev = i + 1;
                        }
                    }
                    if (prev != text.Length)
                    {
                        ret.Add(new(prev, text.Length, (int)Direction.DIRECTION_AUTO));
                    }
                }

                break;
            case StructuredTextParser.STRUCTURED_TEXT_LIST:
                if (args.Count == 1 && args[0] is string arg)
                {
                    var tags = text.Split(arg);
                    var prev = 0;
                    for (var i = 0; i < tags.Length; i++)
                    {
                        if (prev != i)
                        {
                            ret.Add(new(prev, prev + tags[i].Length, (int)Direction.DIRECTION_INHERITED));
                        }
                        ret.Add(new(prev + tags[i].Length, prev + tags[i].Length + 1, (int)Direction.DIRECTION_INHERITED));
                        prev = prev + tags[i].Length + 1;
                    }
                }

                break;
            case StructuredTextParser.STRUCTURED_TEXT_GDSCRIPT:
                {
                    var inStringLiteral = false;
                    var inStringLiteralSingle = false;
                    var inId = false;

                    var prev = 0;
                    for (var i = 0; i < text.Length; i++)
                    {
                        var c = text[i];
                        if (inStringLiteral)
                        {
                            if (c == '\\')
                            {
                                i++;
                                continue; // Skip escaped chars.
                            }
                            else if (c == '\"')
                            {
                                // String literal end, push string and ".
                                if (prev != i)
                                {
                                    ret.Add(new(prev, i, (int)Direction.DIRECTION_AUTO));
                                }
                                prev = i + 1;
                                ret.Add(new(i, i + 1, (int)Direction.DIRECTION_LTR));
                                inStringLiteral = false;
                            }
                        }
                        else if (inStringLiteralSingle)
                        {
                            if (c == '\\')
                            {
                                i++;
                                continue; // Skip escaped chars.
                            }
                            else if (c == '\'')
                            {
                                // String literal end, push string and '.
                                if (prev != i)
                                {
                                    ret.Add(new(prev, i, (int)Direction.DIRECTION_AUTO));
                                }
                                prev = i + 1;
                                ret.Add(new(i, i + 1, (int)Direction.DIRECTION_LTR));
                                inStringLiteralSingle = false;
                            }
                        }
                        else if (inId)
                        {
                            if (!c.IsUnicodeIdentifierContinue())
                            {
                                // End of id, push id.
                                if (prev != i)
                                {
                                    ret.Add(new(prev, i, (int)Direction.DIRECTION_AUTO));
                                }
                                prev = i;
                                inId = false;
                            }
                        }
                        else if (c.IsUnicodeIdentifierStart())
                        {
                            // Start of new id, push prev element.
                            if (prev != i)
                            {
                                ret.Add(new(prev, i, (int)Direction.DIRECTION_AUTO));
                            }
                            prev = i;
                            inId = true;
                        }
                        else if (c == '\"')
                        {
                            // String literal start, push prev element and ".
                            if (prev != i)
                            {
                                ret.Add(new(prev, i, (int)Direction.DIRECTION_AUTO));
                            }
                            prev = i + 1;
                            ret.Add(new(i, i + 1, (int)Direction.DIRECTION_LTR));
                            inStringLiteral = true;
                        }
                        else if (c == '\'')
                        {
                            // String literal start, push prev element and '.
                            if (prev != i)
                            {
                                ret.Add(new(prev, i, (int)Direction.DIRECTION_AUTO));
                            }
                            prev = i + 1;
                            ret.Add(new(i, i + 1, (int)Direction.DIRECTION_LTR));
                            inStringLiteralSingle = true;
                        }
                        else if (c == '#')
                        {
                            // Start of comment, push prev element and #, skip the rest of the text.
                            if (prev != i)
                            {
                                ret.Add(new(prev, i, (int)Direction.DIRECTION_AUTO));
                            }
                            prev = i + 1;
                            ret.Add(new(i, i + 1, (int)Direction.DIRECTION_LTR));
                            break;
                        }
                    }
                    if (prev < text.Length)
                    {
                        ret.Add(new(prev, text.Length, (int)Direction.DIRECTION_AUTO));
                    }
                }
                break;
            case StructuredTextParser.STRUCTURED_TEXT_CUSTOM:
            case StructuredTextParser.STRUCTURED_TEXT_DEFAULT:
            default:
                ret.Add(new(0, text.Length, (int)Direction.DIRECTION_INHERITED));

                break;
        }

        return ret;
    }

    #region public virtual methods

    public virtual Vector2<RealT> GetHexCodeBoxSize(int size, int index)
    {
        var w  = (index <= 0xFF) ? 1 : ((index <= 0xFFFF) ? 2 : 3);
        var sp = Math.Max(0, w - 1);
        var sz = (int)Math.Max(1, Math.Round(size / 15f));

        return new Vector2<RealT>(4 + 3 * w + sp + 1, 15f) * sz;
    }

    public virtual HbTag NameToTag(string name) => 0;

    public virtual IList<int> ShapedTextGetLineBreaks(Guid shaped, double width, long start = 0, LineBreakFlag breakFlags = LineBreakFlag.BREAK_MANDATORY | LineBreakFlag.BREAK_WORD_BOUND)
    {
        var lines = new List<int>();

        this.ShapedTextUpdateBreaks(shaped);

        var range = this.ShapedTextGetRange(shaped);

        var currentWidth  = 0.0;
        var lineStart     = (int)Math.Max(start, range.X);
        var prevSafeBreak = 0;
        var lastSafeBreak = -1;
        var wordCount     = 0;
        var trimNext      = false;

        var lSize = (int)this.ShapedTextGetGlyphCount(shaped);
        var lGl   = this.ShapedTextSortLogical(shaped);

        for (var i = 0; i < lSize; i++)
        {
            if (lGl[i].Start < start)
            {
                prevSafeBreak = i + 1;
                continue;
            }
            if (lGl[i].Count > 0)
            {
                if (width > 0 && currentWidth + lGl[i].Advance * lGl[i].Repeat > width && lastSafeBreak >= 0)
                {
                    if (breakFlags.HasFlag(LineBreakFlag.BREAK_TRIM_EDGE_SPACES))
                    {
                        var startPos = prevSafeBreak;
                        var endPos = lastSafeBreak;
                        while (trimNext && startPos < endPos && (lGl[startPos].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_SPACE) || lGl[startPos].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_BREAK_HARD) || lGl[startPos].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_BREAK_SOFT)))
                        {
                            startPos += lGl[startPos].Count;
                        }
                        while (startPos < endPos && (lGl[endPos].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_SPACE) || lGl[endPos].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_BREAK_HARD) || lGl[endPos].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_BREAK_SOFT)))
                        {
                            endPos -= lGl[endPos].Count;
                        }
                        lines.Add(lGl[startPos].Start);
                        lines.Add(lGl[endPos].End);
                        trimNext = true;
                    }
                    else
                    {
                        lines.Add(lineStart);
                        lines.Add(lGl[lastSafeBreak].End);
                    }

                    lineStart     = lGl[lastSafeBreak].End;
                    prevSafeBreak = lastSafeBreak + 1;
                    i             = lastSafeBreak;
                    lastSafeBreak = -1;
                    currentWidth         = 0;
                    wordCount     = 0;

                    continue;
                }
                if (breakFlags.HasFlag(LineBreakFlag.BREAK_MANDATORY))
                {
                    if (lGl[i].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_BREAK_HARD))
                    {
                        if (breakFlags.HasFlag(LineBreakFlag.BREAK_TRIM_EDGE_SPACES))
                        {
                            var startPos = prevSafeBreak;
                            var endPos = i;

                            while (trimNext && startPos < endPos && lGl[startPos].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_SPACE) || lGl[startPos].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_BREAK_HARD) || lGl[startPos].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_BREAK_SOFT))
                            {
                                startPos += lGl[startPos].Count;
                            }

                            while (startPos < endPos && lGl[endPos].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_SPACE) || lGl[endPos].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_BREAK_HARD) || lGl[endPos].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_BREAK_SOFT))
                            {
                                endPos -= lGl[endPos].Count;
                            }

                            trimNext = false;

                            lines.Add(lGl[startPos].Start);
                            lines.Add(lGl[endPos].End);
                        }
                        else
                        {
                            lines.Add(lineStart);
                            lines.Add(lGl[i].End);
                        }
                        lineStart     = lGl[i].End;
                        prevSafeBreak = i + 1;
                        lastSafeBreak = -1;
                        currentWidth         = 0;

                        continue;
                    }
                }
                if (breakFlags.HasFlag(LineBreakFlag.BREAK_WORD_BOUND))
                {
                    if (lGl[i].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_BREAK_SOFT))
                    {
                        lastSafeBreak = i;
                        wordCount++;
                    }
                    if (breakFlags.HasFlag(LineBreakFlag.BREAK_ADAPTIVE) && wordCount == 0)
                    {
                        lastSafeBreak = i;
                    }
                }
                if (breakFlags.HasFlag(LineBreakFlag.BREAK_GRAPHEME_BOUND))
                {
                    lastSafeBreak = i;
                }
            }

            currentWidth += lGl[i].Advance * lGl[i].Repeat;
        }

        if (lSize > 0)
        {
            if (lines.Count == 0 || lines[^1] < range.Y && prevSafeBreak < lSize)
            {
                if (breakFlags.HasFlag(LineBreakFlag.BREAK_TRIM_EDGE_SPACES))
                {
                    var startPos = (prevSafeBreak < lSize) ? prevSafeBreak : lSize - 1;
                    var endPos   = lSize - 1;

                    while (trimNext && startPos < endPos && lGl[startPos].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_SPACE) || lGl[startPos].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_BREAK_HARD) || lGl[startPos].Flags.HasFlag(GraphemeFlag.GRAPHEME_IS_BREAK_SOFT))
                    {
                        startPos += lGl[startPos].Count;
                    }

                    lines.Add(lGl[startPos].Start);
                }
                else
                {
                    lines.Add(lineStart);
                }

                lines.Add(range.Y);
            }
        }
        else
        {
            lines.Add(0);
            lines.Add(0);
        }

        return lines;
    }

    public virtual IList<Vector2<RealT>> ShapedTextGetSelection(Guid shaped, long start, long end) => throw new NotImplementedException();
    #endregion public virtual methods

    #region public abstract methods
    public abstract Guid CreateShapedText(Direction direction = Direction.DIRECTION_AUTO, Orientation orientation = Orientation.ORIENTATION_HORIZONTAL);
    public abstract void DrawHexCodeBox(Guid canvas, long size, in Vector2<RealT> pos, long index, in Color color);
    public abstract void FreeId(Guid id);
    public abstract void FontDrawGlyph(Guid fontId, Guid canvas, long size, in Vector2<RealT> pos, long index);
    public abstract void FontDrawGlyph(Guid fontId, Guid canvas, long size, in Vector2<RealT> pos, long index, in Color color);
    public abstract void FontDrawGlyphOutline(Guid fontId, Guid canvas, long size, long outlineSize, in Vector2<RealT> pos, long index);
    public abstract void FontDrawGlyphOutline(Guid fontId, Guid canvas, long size, long outlineSize, in Vector2<RealT> pos, long index, in Color color);
    public abstract double FontGetAscent(Guid fontId, int size);
    public abstract double FontGetDescent(Guid fontId, int size);
    public abstract bool HasFeature(Feature feature);
    public abstract bool LoadSupportData(string filename);
    public abstract long ShapedGetSpanCount(Guid shaped);
    public abstract void ShapedSetSpanUpdateFont(Guid shaped, long index, IList<Guid> fonts, long size, Dictionary<uint, Feature>? opentypeFeatures = default);
    public abstract bool ShapedTextAddString(Guid shaped, string text, IList<Guid> fonts, long size, Dictionary<uint, Feature>? opentypeFeatures = null, string language = "", object? meta = null);
    public abstract void ShapedTextClear(Guid shaped);
    public abstract double ShapedTextFitToWidth(Guid shaped, double width, JustificationFlag jstFlags = JustificationFlag.JUSTIFICATION_WORD_BOUND | JustificationFlag.JUSTIFICATION_KASHIDA);
    public abstract double ShapedTextGetAscent(Guid shaped);
    public abstract CaretInfo ShapedTextGetCarets(Guid shaped, long position);
    public abstract double ShapedTextGetDescent(Guid shaped);
    public abstract IList<Glyph> ShapedTextGetEllipsisGlyphs(Guid shaped);
    public abstract long ShapedTextGetEllipsisGlyphCount(Guid shaped);
    public abstract long ShapedTextGetEllipsisPos(Guid shaped);
    public abstract IList<Glyph> ShapedTextGetGlyphs(Guid shaped);
    public abstract long ShapedTextGetGlyphCount(Guid shaped);
    public abstract Direction ShapedTextGetInferredDirection(Guid shaped);
    public abstract Vector2<RealT> ShapedTextGetSize(Guid shaped);
    public abstract Guid ShapedTextSubstr(Guid shaped, int start, int length); // Copy shaped substring (e.g. line break) without reshaping, but correctly reordered, preservers range.
    public abstract Vector2<int> ShapedTextGetRange(Guid shaped);
    public abstract long ShapedTextGetTrimPos(Guid shaped);
    public abstract bool ShapedTextIsReady(Guid shaped);
    public abstract void ShapedTextOverrunTrimToWidth(Guid shaped, double width, TextOverrunFlag trimFlags);
    public abstract void ShapedTextSetBidiOverride(Guid shaped, IList<object> @override);
    public abstract void ShapedTextSetDirection(Guid shaped, Direction direction = Direction.DIRECTION_AUTO);
    public abstract void ShapedTextSetPreserveControl(Guid shaped, bool enabled);
    public abstract void ShapedTextSetSpacing(Guid shaped, SpacingType spacing, long value);
    public abstract IList<Glyph> ShapedTextSortLogical(Guid shaped);
    public abstract bool ShapedTextUpdateBreaks(Guid shaped);
    public abstract string StringToUpper(string @string, string language = "");
    #endregion public abstract methods
}
