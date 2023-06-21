namespace Godot.Net.Scene.GUI;

using System.Collections.Generic;
using Godot.Net.Core.Enums;
using Godot.Net.Core.Math;
using Godot.Net.Scene.Main;
using Godot.Net.Scene.Resources;
using Godot.Net.Servers;
using static Godot.Net.Servers.TextServer;

#pragma warning disable IDE0044, IDE0052, IDE0051, CS0169, CS0414, CS0649 // TODO Remove

public partial class Label : Control
{
    private readonly List<Guid> linesId    = new();
    private readonly Guid       textId;
    private readonly ThemeCache themeCache = new();

    private AutowrapMode                         autowrapMode         = AutowrapMode.AUTOWRAP_OFF;
    private bool                                 clip;
    private bool                                 dirty                = true;
    private bool                                 fontDirty            = true;
    private HorizontalAlignment                  horizontalAlignment  = HorizontalAlignment.HORIZONTAL_ALIGNMENT_LEFT;
    private string                               language             = "";
    private bool                                 linesDirty           = true;
    private int                                  linesSkipped;
    private int                                  maxLinesVisible      = -1;
    private Vector2<RealT>                       minsize;
    private OverrunBehavior                      overrunBehavior      = OverrunBehavior.OVERRUN_NO_TRIMMING;
    private LabelSettings?                       settings;
    private object[]                             stArgs               = Array.Empty<object>();
    private StructuredTextParser                 stParser             = TextServer.StructuredTextParser.STRUCTURED_TEXT_DEFAULT;
    private string                               text                 = "";
    private TextDirection                        textDirection        = TextDirection.TEXT_DIRECTION_AUTO;
    private bool                                 uppercase;
    private VerticalAlignment                    verticalAlignment    = VerticalAlignment.VERTICAL_ALIGNMENT_TOP;
    private int                                  visibleChars         = -1;
    private VisibleCharactersBehavior            visibleCharsBehavior = VisibleCharactersBehavior.VC_CHARS_BEFORE_SHAPING;
    private float                                visibleRatio         = 1;
    private string                               xlText               = "";

    public int LineHeight => throw new NotImplementedException();

    public int TotalCharacterCount
    {
        get
        {
            if (this.dirty || this.fontDirty || this.linesDirty)
            {
                this.Shape();
            }

            return this.xlText.Length;
        }
    }

    public string Text
    {
        get => this.text;
        set
        {
            if (this.text == value)
            {
                return;
            }

            this.text   = value;
            this.xlText = this.Atr(value);
            this.dirty  = true;

            if (this.visibleRatio < 1)
            {
                this.visibleChars = (int)(this.TotalCharacterCount * this.visibleRatio);
            }

            this.QueueRedraw();
            this.UpdateMinimumSize();
            this.UpdateConfigurationWarnings();
        }
    }

    public Label(string text = "")
    {
        this.textId = TextServerManager.Singleton.PrimaryInterface.CreateShapedText();

        this.MouseFilter = MouseFilterKind.MOUSE_FILTER_IGNORE;
        this.Text = text;
        this.        VSizeFlags = SizeFlags.SIZE_SHRINK_CENTER;
    }

    private void DrawGlyph(Glyph gl, Guid canvasId, Color fontColor, Vector2<RealT> ofs) => throw new NotImplementedException();

    private void DrawGlyphOutline(
        Glyph             gl,
        Guid              canvasId,
        in Color          fontColor,
        in Color          fontShadowColor,
        in Color          fontOutlineColor,
        int               shadowOutlineSize,
        int               outlineSize,
        in Vector2<RealT> ofs,
        in Vector2<RealT> shadowOfs
    ) => throw new NotImplementedException();

    private void Shape()
    {
        var style = this.themeCache.NormalStyle;
        var width = (int)(this.Size.X - style.MinimumSize.X);

        if (this.dirty || this.fontDirty)
        {
            if (this.dirty)
            {
                TextServerManager.Singleton.PrimaryInterface.ShapedTextClear(this.textId);
            }
            if (this.textDirection == TextDirection.TEXT_DIRECTION_INHERITED)
            {
                TextServerManager.Singleton.PrimaryInterface.ShapedTextSetDirection(this.textId, this.IsLayoutRtl() ? Direction.DIRECTION_RTL : Direction.DIRECTION_LTR);
            }
            else
            {
                TextServerManager.Singleton.PrimaryInterface.ShapedTextSetDirection(this.textId, (Direction)this.textDirection);
            }
            var font = this.settings?.Font ?? this.themeCache.Font;
            var fontSize = this.settings?.FontSize ?? this.themeCache.FontSize;

            if (ERR_FAIL_COND(font == null))
            {
                return;
            }

            var txt = this.uppercase ? TextServerManager.Singleton.PrimaryInterface.StringToUpper(this.xlText, this.language) : this.xlText;
            if (this.visibleChars >= 0 && this.visibleCharsBehavior == VisibleCharactersBehavior.VC_CHARS_BEFORE_SHAPING)
            {
                txt = txt[..this.visibleChars];
            }
            if (this.dirty)
            {
                TextServerManager.Singleton.PrimaryInterface.ShapedTextAddString(this.textId, txt, font!.Ids, fontSize, font.OpentypeFeatures, this.language);
            }
            else
            {
                var spans = (int)TextServerManager.Singleton.PrimaryInterface.ShapedGetSpanCount(this.textId);
                for (var i = 0; i < spans; i++)
                {
                    TextServerManager.Singleton.PrimaryInterface.ShapedSetSpanUpdateFont(this.textId, i, font!.Ids, fontSize, font.OpentypeFeatures);
                }
            }
            for (var i = 0; i < (int)SpacingType.SPACING_MAX; i++)
            {
                TextServerManager.Singleton.PrimaryInterface.ShapedTextSetSpacing(this.textId, (SpacingType)i, font!.GetSpacing((SpacingType)i));
            }
            TextServerManager.Singleton.PrimaryInterface.ShapedTextSetBidiOverride(this.textId, this.StructuredTextParser(this.stParser, this.stArgs, txt).Cast<object>().ToArray());
            this.dirty = false;
            this.fontDirty = false;
            this.linesDirty = true;
        }

        if (this.linesDirty)
        {
            for (var i = 0; i < this.linesId.Count; i++)
            {
                TextServerManager.Singleton.PrimaryInterface.FreeId(this.linesId[i]);
            }
            this.linesId.Clear();

            var autowrapFlags = LineBreakFlag.BREAK_MANDATORY;

            switch (this.autowrapMode)
            {
                case AutowrapMode.AUTOWRAP_WORD_SMART:
                    autowrapFlags = LineBreakFlag.BREAK_WORD_BOUND | LineBreakFlag.BREAK_ADAPTIVE | LineBreakFlag.BREAK_MANDATORY;
                    break;
                case AutowrapMode.AUTOWRAP_WORD:
                    autowrapFlags = LineBreakFlag.BREAK_WORD_BOUND | LineBreakFlag.BREAK_MANDATORY;
                    break;
                case AutowrapMode.AUTOWRAP_ARBITRARY:
                    autowrapFlags = LineBreakFlag.BREAK_GRAPHEME_BOUND | LineBreakFlag.BREAK_MANDATORY;
                    break;
                case AutowrapMode.AUTOWRAP_OFF:
                    break;
            }

            autowrapFlags |= LineBreakFlag.BREAK_TRIM_EDGE_SPACES;

            var lineBreaks = TextServerManager.Singleton.PrimaryInterface.ShapedTextGetLineBreaks(this.textId, width, 0, autowrapFlags);
            for (var i = 0; i < lineBreaks.Count; i += 2)
            {
                var line = TextServerManager.Singleton.PrimaryInterface.ShapedTextSubstr(this.textId, lineBreaks[i], lineBreaks[i + 1] - lineBreaks[i]);
                this.linesId.Add(line);
            }
        }

        if (this.xlText.Length == 0)
        {
            this.minsize = new(1, this.LineHeight);
            return;
        }

        if (this.autowrapMode == AutowrapMode.AUTOWRAP_OFF)
        {
            this.minsize.X = 0.0f;
            for (var i = 0; i < this.linesId.Count; i++)
            {
                if (this.minsize.X < TextServerManager.Singleton.PrimaryInterface.ShapedTextGetSize(this.linesId[i]).X)
                {
                    this.minsize.X = TextServerManager.Singleton.PrimaryInterface.ShapedTextGetSize(this.linesId[i]).X;
                }
            }
        }

        if (this.linesDirty)
        {
            var overrunFlags = TextOverrunFlag.OVERRUN_NO_TRIM;
            switch (this.overrunBehavior)
            {
                case OverrunBehavior.OVERRUN_TRIM_WORD_ELLIPSIS:
                    overrunFlags |= TextOverrunFlag.OVERRUN_TRIM;
                    overrunFlags |= TextOverrunFlag.OVERRUN_TRIM_WORD_ONLY;
                    overrunFlags |= TextOverrunFlag.OVERRUN_ADD_ELLIPSIS;
                    break;
                case OverrunBehavior.OVERRUN_TRIM_ELLIPSIS:
                    overrunFlags |= TextOverrunFlag.OVERRUN_TRIM;
                    overrunFlags |= TextOverrunFlag.OVERRUN_ADD_ELLIPSIS;
                    break;
                case OverrunBehavior.OVERRUN_TRIM_WORD:
                    overrunFlags |= TextOverrunFlag.OVERRUN_TRIM;
                    overrunFlags |= TextOverrunFlag.OVERRUN_TRIM_WORD_ONLY;
                    break;
                case OverrunBehavior.OVERRUN_TRIM_CHAR:
                    overrunFlags |= TextOverrunFlag.OVERRUN_TRIM;
                    break;
                case OverrunBehavior.OVERRUN_NO_TRIMMING:
                    break;
            }

            // Fill after min_size calculation.

            if (this.autowrapMode != AutowrapMode.AUTOWRAP_OFF)
            {
                var visibleLines = this.GetVisibleLineCount();
                var linesHidden = visibleLines > 0 && visibleLines < this.linesId.Count;
                if (linesHidden)
                {
                    overrunFlags |= TextOverrunFlag.OVERRUN_ENFORCE_ELLIPSIS;
                }
                if (this.horizontalAlignment == HorizontalAlignment.HORIZONTAL_ALIGNMENT_FILL)
                {
                    for (var i = 0; i < this.linesId.Count; i++)
                    {
                        if (i < visibleLines - 1 || this.linesId.Count == 1)
                        {
                            TextServerManager.Singleton.PrimaryInterface.ShapedTextFitToWidth(this.linesId[i], width);
                        }
                        else if (i == visibleLines - 1)
                        {
                            TextServerManager.Singleton.PrimaryInterface.ShapedTextOverrunTrimToWidth(this.linesId[visibleLines - 1], width, overrunFlags);
                        }
                    }
                }
                else if (linesHidden)
                {
                    TextServerManager.Singleton.PrimaryInterface.ShapedTextOverrunTrimToWidth(this.linesId[visibleLines - 1], width, overrunFlags);
                }
            }
            else
            {
                // Autowrap disabled.
                for (var i = 0; i < this.linesId.Count; i++)
                {
                    if (this.horizontalAlignment == HorizontalAlignment.HORIZONTAL_ALIGNMENT_FILL)
                    {
                        TextServerManager.Singleton.PrimaryInterface.ShapedTextFitToWidth(this.linesId[i], width);
                        overrunFlags |= TextOverrunFlag.OVERRUN_JUSTIFICATION_AWARE;
                        TextServerManager.Singleton.PrimaryInterface.ShapedTextOverrunTrimToWidth(this.linesId[i], width, overrunFlags);
                        TextServerManager.Singleton.PrimaryInterface.ShapedTextFitToWidth(this.linesId[i], width, JustificationFlag.JUSTIFICATION_WORD_BOUND | JustificationFlag.JUSTIFICATION_KASHIDA | JustificationFlag.JUSTIFICATION_CONSTRAIN_ELLIPSIS);
                    }
                    else
                    {
                        TextServerManager.Singleton.PrimaryInterface.ShapedTextOverrunTrimToWidth(this.linesId[i], width, overrunFlags);
                    }
                }
            }
            this.linesDirty = false;
        }

        this.UpdateVisible();

        if (this.autowrapMode == AutowrapMode.AUTOWRAP_OFF || !this.clip || this.overrunBehavior == OverrunBehavior.OVERRUN_NO_TRIMMING)
        {
            this.UpdateMinimumSize();
        }
    }

    private void UpdateVisible()
    {
        var lineSpacing  = (int)(this.settings?.LineSpacing ?? this.themeCache.LineSpacing);
        var style        = this.themeCache.NormalStyle;
        var linesVisible = this.linesId.Count;

        if (this.maxLinesVisible >= 0 && linesVisible > this.maxLinesVisible)
        {
            linesVisible = this.maxLinesVisible;
        }

        this.minsize.Y = 0;
        var lastLine   = Math.Min(this.linesId.Count, linesVisible + this.linesSkipped);

        for (var i = this.linesSkipped; i < lastLine; i++)
        {
            this.minsize.Y += TextServerManager.Singleton.PrimaryInterface.ShapedTextGetSize(this.linesId[i]).Y + lineSpacing;

            if (this.minsize.Y > this.Size.Y - style.MinimumSize.Y + lineSpacing)
            {
                break;
            }
        }
    }

    protected override void UpdateThemeItemCache()
    {
        base.UpdateThemeItemCache();

        this.themeCache.NormalStyle           = this.GetThemeStylebox("normal");
        this.themeCache.Font                  = this.GetThemeFont("font");
        this.themeCache.FontSize              = this.GetThemeFontSize("font_size");
        this.themeCache.LineSpacing           = this.GetThemeConstant("line_spacing");
        this.themeCache.FontColor             = this.GetThemeColor("font_color");
        this.themeCache.FontShadowColor       = this.GetThemeColor("font_shadow_color");
        this.themeCache.FontShadowOffset      = new(this.GetThemeConstant("shadow_offset_x"), this.GetThemeConstant("shadow_offset_y"));
        this.themeCache.FontOutlineColor      = this.GetThemeColor("font_outline_color");
        this.themeCache.FontOutlineSize       = this.GetThemeConstant("outline_size");
        this.themeCache.FontShadowOutlineSize = this.GetThemeConstant("shadow_outline_size");
    }

    public int GetVisibleLineCount() => throw new NotImplementedException();

    public override void Notification(NotificationKind notification, bool reversed = false)
    {
        if (!reversed)
        {
            base.Notification(notification, reversed);
        }

        switch (notification)
        {
            case NotificationKind.MAIN_LOOP_NOTIFICATION_TRANSLATION_CHANGED:
                {
                    var newText = this.Atr(this.text);
                    if (newText == this.xlText)
                    {
                        return; // Nothing new.
                    }
                    this.xlText = newText;
                    if (this.visibleRatio < 1)
                    {
                        this.visibleChars = (int)(this.TotalCharacterCount * this.visibleRatio);
                    }
                    this.dirty = true;

                    this.QueueRedraw();
                    this.UpdateConfigurationWarnings();
                }
                break;

            case NotificationKind.CONTROL_NOTIFICATION_LAYOUT_DIRECTION_CHANGED:
                this.QueueRedraw();
                break;

            case NotificationKind.CANVAS_ITEM_NOTIFICATION_DRAW:
                {
                    if (this.clip)
                    {
                        RS.Singleton.CanvasItemSetClip(this.CanvasItemId, true);
                    }

                    // When a shaped text is invalidated by an external source, we want to reshape it.
                    if (!TextServerManager.Singleton.PrimaryInterface.ShapedTextIsReady(this.textId))
                    {
                        this.dirty = true;
                    }

                    foreach (var lineId in this.linesId)
                    {
                        if (!TextServerManager.Singleton.PrimaryInterface.ShapedTextIsReady(lineId))
                        {
                            this.linesDirty = true;
                            break;
                        }
                    }

                    if (this.dirty || this.fontDirty || this.linesDirty)
                    {
                        this.Shape();
                    }

                    var ci = this.CanvasItemId;

                    var size              = this.Size;
                    var style             = this.themeCache.NormalStyle;
                    var fontColor         = this.settings?.FontColor    ?? this.themeCache.FontColor;
                    var fontShadowColor   = this.settings?.ShadowColor  ?? this.themeCache.FontShadowColor;
                    var shadowOfs         = this.settings?.ShadowOffset ?? this.themeCache.FontShadowOffset;
                    var lineSpacing       = this.settings?.LineSpacing  ?? this.themeCache.LineSpacing;
                    var fontOutlineColor  = this.settings?.OutlineColor ?? this.themeCache.FontOutlineColor;
                    var outlineSize       = this.settings?.OutlineSize  ?? this.themeCache.FontOutlineSize;
                    var shadowOutlineSize = this.settings?.ShadowSize   ?? this.themeCache.FontShadowOutlineSize;
                    var rtl               = TextServerManager.Singleton.PrimaryInterface.ShapedTextGetInferredDirection(this.textId) == Direction.DIRECTION_RTL;
                    var rtlLayout         = this.IsLayoutRtl();

                    style.Draw(ci, new(new(0, 0), this.Size));

                    var totalH       = 0f;
                    var linesVisible = 0;

                    // Get number of lines to fit to the height.
                    for (var i = this.linesSkipped; i < this.linesId.Count; i++)
                    {
                        totalH += TextServerManager.Singleton.PrimaryInterface.ShapedTextGetSize(this.linesId[i]).Y + lineSpacing;
                        if (totalH > this.Size.Y - style.MinimumSize.Y + lineSpacing)
                        {
                            break;
                        }
                        linesVisible++;
                    }

                    if (this.maxLinesVisible >= 0 && linesVisible > this.maxLinesVisible)
                    {
                        linesVisible = this.maxLinesVisible;
                    }

                    var lastLine      = Math.Min(this.linesId.Count, linesVisible + this.linesSkipped);
                    var trimChars     = this.visibleChars >= 0 && this.visibleCharsBehavior == VisibleCharactersBehavior.VC_CHARS_AFTER_SHAPING;
                    var trimGlyphsLtr = this.visibleChars >= 0 && (this.visibleCharsBehavior == VisibleCharactersBehavior.VC_GLYPHS_LTR || this.visibleCharsBehavior == VisibleCharactersBehavior.VC_GLYPHS_AUTO && !rtlLayout);
                    var trimGlyphsRtl = this.visibleChars >= 0 && (this.visibleCharsBehavior == VisibleCharactersBehavior.VC_GLYPHS_RTL || this.visibleCharsBehavior == VisibleCharactersBehavior.VC_GLYPHS_AUTO && rtlLayout);

                    // Get real total height.
                    var totalGlyphs = 0L;
                    totalH = 0;
                    for (var i = this.linesSkipped; i < lastLine; i++)
                    {
                        totalH += TextServerManager.Singleton.PrimaryInterface.ShapedTextGetSize(this.linesId[i]).Y + lineSpacing;
                        totalGlyphs += TextServerManager.Singleton.PrimaryInterface.ShapedTextGetGlyphCount(this.linesId[i]) + TextServerManager.Singleton.PrimaryInterface.ShapedTextGetEllipsisGlyphCount(this.linesId[i]);
                    }
                    var visibleGlyphs   = (int)(totalGlyphs * this.visibleRatio);
                    var processedGlyphs = 0;

                    totalH += style.GetMargin(Side.SIDE_TOP) + style.GetMargin(Side.SIDE_BOTTOM);

                    int vbegin = 0, vsep = 0;
                    if (linesVisible > 0)
                    {
                        switch (this.verticalAlignment)
                        {
                            case VerticalAlignment.VERTICAL_ALIGNMENT_TOP:
                                // Nothing.
                                break;
                            case VerticalAlignment.VERTICAL_ALIGNMENT_CENTER:
                                vbegin = (int)((size.Y - (totalH - lineSpacing)) / 2);
                                vsep   = 0;

                                break;
                            case VerticalAlignment.VERTICAL_ALIGNMENT_BOTTOM:
                                vbegin = (int)(size.Y - (totalH - lineSpacing));
                                vsep   = 0;

                                break;
                            case VerticalAlignment.VERTICAL_ALIGNMENT_FILL:
                                vbegin = 0;
                                vsep   = linesVisible > 1 ? (int)((size.Y - (totalH - lineSpacing)) / (linesVisible - 1)) : 0;

                                break;
                        }
                    }

                    var ofs = new Vector2<RealT>
                    {
                        Y = style.Offset.Y + vbegin
                    };

                    for (var i = this.linesSkipped; i < lastLine; i++)
                    {
                        var lineSize = TextServerManager.Singleton.PrimaryInterface.ShapedTextGetSize(this.linesId[i]);
                        ofs.X = 0;
                        ofs.Y += (RealT)TextServerManager.Singleton.PrimaryInterface.ShapedTextGetAscent(this.linesId[i]);
                        switch (this.horizontalAlignment)
                        {
                            case HorizontalAlignment.HORIZONTAL_ALIGNMENT_FILL:
                                ofs.X = rtl && this.autowrapMode != AutowrapMode.AUTOWRAP_OFF
                                    ? (int)(size.X - style.GetMargin(Side.SIDE_RIGHT) - lineSize.X)
                                    : style.Offset.X;
                                break;
                            case HorizontalAlignment.HORIZONTAL_ALIGNMENT_LEFT:
                                ofs.X = rtlLayout ? (int)(size.X - style.GetMargin(Side.SIDE_RIGHT) - lineSize.X) : style.Offset.X;
                                break;
                            case HorizontalAlignment.HORIZONTAL_ALIGNMENT_CENTER:
                                ofs.X = (int)(size.X - lineSize.X) / 2;
                                break;
                            case HorizontalAlignment.HORIZONTAL_ALIGNMENT_RIGHT:
                                    ofs.X = rtlLayout ? style.Offset.X : (int)(size.X - style.GetMargin(Side.SIDE_RIGHT) - lineSize.X);
                                break;
                        }

                        var glyphs = TextServerManager.Singleton.PrimaryInterface.ShapedTextGetGlyphs(this.linesId[i]);
                        var glSize = (int)TextServerManager.Singleton.PrimaryInterface.ShapedTextGetGlyphCount(this.linesId[i]);

                        var ellipsisPos = (int)TextServerManager.Singleton.PrimaryInterface.ShapedTextGetEllipsisPos(this.linesId[i]);
                        var trimPos     = (int)TextServerManager.Singleton.PrimaryInterface.ShapedTextGetTrimPos(this.linesId[i]);

                        var ellipsisGlyphs = TextServerManager.Singleton.PrimaryInterface.ShapedTextGetEllipsisGlyphs(this.linesId[i]);
                        var ellipsisGlSize = (int)TextServerManager.Singleton.PrimaryInterface.ShapedTextGetEllipsisGlyphCount(this.linesId[i]);

                        // Draw outline. Note: Do not merge this into the single loop with the main text, to prevent overlaps.
                        var processedGlyphsOl = processedGlyphs;
                        if (outlineSize > 0 && fontOutlineColor.A != 0 || fontShadowColor.A != 0)
                        {
                            var offset = ofs;
                            // Draw RTL ellipsis string when necessary.
                            if (rtl && ellipsisPos >= 0)
                            {
                                for (var glIdx = ellipsisGlSize - 1; glIdx >= 0; glIdx--)
                                {
                                    for (var j = 0; j < ellipsisGlyphs[glIdx].Repeat; j++)
                                    {
                                        var skip = trimChars && ellipsisGlyphs[glIdx].End > this.visibleChars
                                            || trimGlyphsLtr && processedGlyphsOl >= visibleGlyphs
                                            || trimGlyphsRtl && processedGlyphsOl < totalGlyphs - visibleGlyphs;
                                        //Draw glyph outlines and shadow.
                                        if (!skip)
                                        {
                                            this.DrawGlyphOutline(
                                                ellipsisGlyphs[glIdx],
                                                ci,
                                                fontColor,
                                                fontShadowColor,
                                                fontOutlineColor,
                                                shadowOutlineSize,
                                                outlineSize,
                                                offset,
                                                shadowOfs
                                            );
                                        }
                                        processedGlyphsOl++;
                                        offset.X += ellipsisGlyphs[glIdx].Advance;
                                    }
                                }
                            }

                            // Draw main text.
                            for (var j = 0; j < glSize; j++)
                            {
                                // Trim when necessary.
                                if (trimPos >= 0)
                                {
                                    if (rtl)
                                    {
                                        if (j < trimPos)
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        if (j >= trimPos)
                                        {
                                            break;
                                        }
                                    }
                                }
                                for (var k = 0; k < glyphs[j].Repeat; k++)
                                {
                                    var skip = trimChars && glyphs[j].End > this.visibleChars
                                        || trimGlyphsLtr && processedGlyphsOl >= visibleGlyphs
                                        || trimGlyphsRtl && processedGlyphsOl < totalGlyphs - visibleGlyphs;

                                    // Draw glyph outlines and shadow.
                                    if (!skip)
                                    {
                                        this.DrawGlyphOutline(
                                            glyphs[j],
                                            ci,
                                            fontColor,
                                            fontShadowColor,
                                            fontOutlineColor,
                                            shadowOutlineSize,
                                            outlineSize,
                                            offset,
                                            shadowOfs
                                        );
                                    }

                                    processedGlyphsOl++;
                                    offset.X += glyphs[j].Advance;
                                }
                            }
                            // Draw LTR ellipsis string when necessary.
                            if (!rtl && ellipsisPos >= 0)
                            {
                                for (var glIdx = 0; glIdx < ellipsisGlSize; glIdx++)
                                {
                                    for (var j = 0; j < ellipsisGlyphs[glIdx].Repeat; j++)
                                    {
                                        var skip = trimChars && ellipsisGlyphs[glIdx].End > this.visibleChars
                                            || trimGlyphsLtr && processedGlyphsOl >= visibleGlyphs
                                            || trimGlyphsRtl && processedGlyphsOl < totalGlyphs - visibleGlyphs;
                                        //Draw glyph outlines and shadow.
                                        if (!skip)
                                        {
                                            this.DrawGlyphOutline(
                                                ellipsisGlyphs[glIdx],
                                                ci,
                                                fontColor,
                                                fontShadowColor,
                                                fontOutlineColor,
                                                shadowOutlineSize,
                                                outlineSize,
                                                offset,
                                                shadowOfs
                                            );
                                        }
                                        processedGlyphsOl++;
                                        offset.X += ellipsisGlyphs[glIdx].Advance;
                                    }
                                }
                            }
                        }

                        // Draw main text. Note: Do not merge this into the single loop with the outline, to prevent overlaps.

                        // Draw RTL ellipsis string when necessary.
                        if (rtl && ellipsisPos >= 0)
                        {
                            for (var glIdx = ellipsisGlSize - 1; glIdx >= 0; glIdx--)
                            {
                                for (var j = 0; j < ellipsisGlyphs[glIdx].Repeat; j++)
                                {
                                    var skip = trimChars && ellipsisGlyphs[glIdx].End > this.visibleChars
                                        || trimGlyphsLtr && processedGlyphs >= visibleGlyphs
                                        || trimGlyphsRtl && processedGlyphs < totalGlyphs - visibleGlyphs;
                                    //Draw glyph outlines and shadow.
                                    if (!skip)
                                    {
                                        this.DrawGlyph(ellipsisGlyphs[glIdx], ci, fontColor, ofs);
                                    }
                                    processedGlyphs++;
                                    ofs.X += ellipsisGlyphs[glIdx].Advance;
                                }
                            }
                        }

                        // Draw main text.
                        for (var j = 0; j < glSize; j++)
                        {
                            // Trim when necessary.
                            if (trimPos >= 0)
                            {
                                if (rtl)
                                {
                                    if (j < trimPos)
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (j >= trimPos)
                                    {
                                        break;
                                    }
                                }
                            }
                            for (var k = 0; k < glyphs[j].Repeat; k++)
                            {
                                var skip = trimChars && glyphs[j].End > this.visibleChars
                                    || trimGlyphsLtr && processedGlyphs >= visibleGlyphs
                                    || trimGlyphsRtl && processedGlyphs < totalGlyphs - visibleGlyphs;

                                // Draw glyph outlines and shadow.
                                if (!skip)
                                {
                                    this.DrawGlyph(glyphs[j], ci, fontColor, ofs);
                                }
                                processedGlyphs++;
                                ofs.X += glyphs[j].Advance;
                            }
                        }
                        // Draw LTR ellipsis string when necessary.
                        if (!rtl && ellipsisPos >= 0)
                        {
                            for (var glIdx = 0; glIdx < ellipsisGlSize; glIdx++)
                            {
                                for (var j = 0; j < ellipsisGlyphs[glIdx].Repeat; j++)
                                {
                                    var skip = trimChars && ellipsisGlyphs[glIdx].End > this.visibleChars
                                        || trimGlyphsLtr && processedGlyphs >= visibleGlyphs
                                        || trimGlyphsRtl && processedGlyphs < totalGlyphs - visibleGlyphs;
                                    //Draw glyph outlines and shadow.
                                    if (!skip)
                                    {
                                        this.DrawGlyph(ellipsisGlyphs[glIdx], ci, fontColor, ofs);
                                    }
                                    processedGlyphs++;
                                    ofs.X += ellipsisGlyphs[glIdx].Advance;
                                }
                            }
                        }
                        ofs.Y += (RealT)TextServerManager.Singleton.PrimaryInterface.ShapedTextGetDescent(this.linesId[i]) + vsep + lineSpacing;
                    }
                }

                break;

            case NotificationKind.CONTROL_NOTIFICATION_THEME_CHANGED:
                this.fontDirty = true;
                this.QueueRedraw();

                break;

            case NotificationKind.CONTROL_NOTIFICATION_RESIZED:
                this.linesDirty = true;

                break;
        }

        if (reversed)
        {
            base.Notification(notification, reversed);
        }
    }
}
