#define TOOLS_ENABLED

namespace Godot.Net.Scene.GUI;

using Godot.Net.Core.Config;
using Godot.Net.Core.Enums;
using Godot.Net.Core.Input;
using Godot.Net.Core.Math;
using Godot.Net.Core.OS;
using Godot.Net.Editor;
using Godot.Net.Scene.Main;
using Godot.Net.Scene.Resources;
using Godot.Net.Servers;

#pragma warning disable CS0067, CS0649, IDE0052, IDE0044, CS0414, IDE0051, CS0169 // TODO Remove

public partial class LineEdit : Control
{
    public event Action<string>? TextChanged;

    #region private readonly fields
    private readonly ClearButtonStatus   clearButtonStatus = new();
    private readonly Selection           selection         = new();
    private readonly Guid                textId;
    private readonly ThemeCache          themeCache        = new();
    private readonly List<TextOperation> undoStack         = new();
    #endregion private readonly fields

    #region private fields
    private HorizontalAlignment             alignment                  = HorizontalAlignment.HORIZONTAL_ALIGNMENT_LEFT;
    private bool                            caretBlinkEnabled;
    private float                           caretBlinkInterval         = 0.65f;
    private double                          caretBlinkTimer;
    private bool                            caretCanDraw;
    private int                             caretColumn;
    private bool                            caretForceDisplayed;
    private bool                            clearButtonEnabled;
    private bool                            deselectOnFocusLossEnabled = true;
    private bool                            dragAction;
    private bool                            dragCaretForceDisplayed;
    private bool                            drawCaret                  = true;
    private bool                            drawControlChars;
    private bool                            editable;
    private bool                            expandToTextLength;
    private bool                            flat;
    private float                           fullWidth;
    private Vector2<RealT>                  imeSelection;
    private string                          imeText                    = "";
    private string                          language                   = "";
    private PopupMenu?                      menu;
    private bool                            pass;
    private bool                            pendingSelectAllOnFocus;
    private string                          placeholder                = "";
    private string                          placeholderTranslated      = "";
    private Texture2D?                      rightIcon;
    private float                           scrollOffset;
    private char                            secretCharacter            = '•';
    private bool                            selectAllOnFocus;
    private object[]                        stArgs                     = Array.Empty<object>();
    private TextServer.StructuredTextParser stParser;
    private string                          text                       = "";
    private TextDirection                   textDirection;
    private string                          undoText                   = "";
    private bool                            virtualKeyboardEnabled     = true;
    private bool                            windowHasFocus             = true;
    #endregion private fields

    #region public properties
    public bool CaretBlinkEnabled
    {
        get => this.caretBlinkEnabled;
        set
        {
            if (this.caretBlinkEnabled == value)
            {
                return;
            }

            this.caretBlinkEnabled = value;
            this.ProcessInternal = value;

            this.drawCaret = !this.caretBlinkEnabled;
            if (this.caretBlinkEnabled)
            {
                this.caretBlinkTimer = 0.0;
            }
            this.QueueRedraw();

            this.NotifyPropertyListChanged();
        }
    }

    public float CaretBlinkInterval
    {
        get => this.caretBlinkInterval;
        set
        {
            if (ERR_FAIL_COND(value <= 0))
            {
                return;
            }

            this.caretBlinkInterval = value;
        }
    }

    public int CaretColumn
    {
        get => this.caretColumn;
        set => throw new NotImplementedException();
    }

    public bool Editable
    {
        get => this.editable;
        set
        {
            if (this.editable == value)
            {
                return;
            }

            this.editable = value;
            this.ValidateCaretCanDraw();

            this.UpdateMinimumSize();
            this.QueueRedraw();
        }
    }

    public string Placeholder
    {
        get => this.placeholder;
        set
        {
            if (this.placeholder == value)
            {
                return;
            }

            this.placeholder = value;
            this.placeholderTranslated = this.Atr(this.placeholder);
            this.Shape();
            this.QueueRedraw();
        }
    }
    #endregion public properties

    public LineEdit(string placeholder = "")
    {
        this.textId = TextServerManager.Singleton.PrimaryInterface.CreateShapedText();

        this.CreateUndoState();
        this.Deselect();

        this.FocusMode                = FocusModeKind.FOCUS_ALL;
        this.DefaultCursorShape       = CursorShape.CURSOR_IBEAM;
        this.MouseFilter              = MouseFilterKind.MOUSE_FILTER_STOP;
        this.ProcessUnhandledKeyInput = true;
        this.CaretBlinkEnabled        = false;
        this.Placeholder              = placeholder;
        this.Editable                 = true; // Initialize to opposite first, so we get past the early-out in set_editable.
    }

    #region private methods
    private void CreateUndoState()
    {
        var op = new TextOperation
        {
            Text         = this.text,
            CaretColumn  = this.caretColumn,
            ScrollOffset = this.scrollOffset
        };

        this.undoStack.Add(op);
    }

    private void EditorSettingsChanged() => throw new NotImplementedException();

    private void FitToWidth()
    {
        if (this.alignment == HorizontalAlignment.HORIZONTAL_ALIGNMENT_FILL)
        {
            var style = this.themeCache.Normal;

            ASSERT_NOT_NULL(style);

            var tWidth           = (int)(this.Size.X - style.GetMargin(Side.SIDE_RIGHT) - style.GetMargin(Side.SIDE_LEFT));
            var usingPlaceholder = string.IsNullOrEmpty(this.text) && string.IsNullOrEmpty(this.imeText);
            var displayClearIcon = !usingPlaceholder && this.Editable && this.clearButtonEnabled;

            if (this.rightIcon != null || displayClearIcon)
            {
                var rIcon = displayClearIcon ? this.themeCache.ClearIcon : this.rightIcon;

                ASSERT_NOT_NULL(rIcon);

                tWidth -= rIcon.Width;
            }

            TextServerManager.Singleton.PrimaryInterface.ShapedTextFitToWidth(this.textId, Math.Max(tWidth, this.fullWidth));
        }
    }

    private Vector2<RealT> GetCaretPixelPos() => throw new NotImplementedException();

    private void Shape()
    {
        var font     = this.themeCache.Font;
        var fontSize = this.themeCache.FontSize;

        if (font == null)
        {
            return;
        }

        var oldSize = TextServerManager.Singleton.PrimaryInterface.ShapedTextGetSize(this.textId);
        TextServerManager.Singleton.PrimaryInterface.ShapedTextClear(this.textId);

        var t = this.text.Length == 0 && this.imeText.Length == 0
            ? this.placeholderTranslated
            : this.pass
                ? new string(this.secretCharacter, this.text.Length + this.imeText.Length)
                : this.imeText.Length > 0
                    ? string.Concat(this.text.AsSpan(0, this.caretColumn), this.imeText, this.text.AsSpan(this.caretColumn, this.text.Length))
                    : this.text;

        if (this.textDirection == TextDirection.TEXT_DIRECTION_INHERITED)
        {
            TextServerManager.Singleton.PrimaryInterface.ShapedTextSetDirection(this.textId, this.IsLayoutRtl() ? TextServer.Direction.DIRECTION_RTL : TextServer.Direction.DIRECTION_LTR);
        }
        else
        {
            TextServerManager.Singleton.PrimaryInterface.ShapedTextSetDirection(this.textId, (TextServer.Direction)this.textDirection);
        }

        TextServerManager.Singleton.PrimaryInterface.ShapedTextSetPreserveControl(this.textId, this.drawControlChars);
        TextServerManager.Singleton.PrimaryInterface.ShapedTextAddString(this.textId, t, font.Ids, fontSize, font.OpentypeFeatures, this.language);

        for (var i = 0; i < (int)TextServer.SpacingType.SPACING_MAX; i++)
        {
            TextServerManager.Singleton.PrimaryInterface.ShapedTextSetSpacing(this.textId, (TextServer.SpacingType)i, font.GetSpacing((TextServer.SpacingType)i));
        }

        TextServerManager.Singleton.PrimaryInterface.ShapedTextSetBidiOverride(this.textId, this.StructuredTextParser(this.stParser, this.stArgs, t).Cast<object>().ToArray());

        this.fullWidth = TextServerManager.Singleton.PrimaryInterface.ShapedTextGetSize(this.textId).X;
        this.FitToWidth();

        var size = TextServerManager.Singleton.PrimaryInterface.ShapedTextGetSize(this.textId);

        if (this.expandToTextLength && oldSize.X != size.X || oldSize.Y != size.Y)
        {
            this.UpdateMinimumSize();
        }
    }

    private void ToggleDrawCaret() => throw new NotImplementedException();

    private void ValidateCaretCanDraw()
    {
        if (this.caretBlinkEnabled)
        {
            this.drawCaret       = true;
            this.caretBlinkTimer = 0;
        }

        this.caretCanDraw = this.editable && (this.windowHasFocus || (this.menu?.HasFocus ?? false)) && (this.HasFocus || this.caretForceDisplayed);
    }
    #endregion private methods

    #region protected methods
    protected override void UpdateThemeItemCache()
    {
        base.UpdateThemeItemCache();

        this.themeCache.Normal                  = this.GetThemeStylebox("normal");
        this.themeCache.ReadOnly                = this.GetThemeStylebox("read_only");
        this.themeCache.Focus                   = this.GetThemeStylebox("focus");
        this.themeCache.Font                    = this.GetThemeFont("font");
        this.themeCache.FontSize                = this.GetThemeFontSize("font_size");
        this.themeCache.FontColor               = this.GetThemeColor("font_color");
        this.themeCache.FontUneditableColor     = this.GetThemeColor("font_uneditable_color");
        this.themeCache.FontSelectedColor       = this.GetThemeColor("font_selected_color");
        this.themeCache.FontOutlineSize         = this.GetThemeConstant("outline_size");
        this.themeCache.FontOutlineColor        = this.GetThemeColor("font_outline_color");
        this.themeCache.FontPlaceholderColor    = this.GetThemeColor("font_placeholder_color");
        this.themeCache.CaretWidth              = this.GetThemeConstant("caret_width");
        this.themeCache.CaretColor              = this.GetThemeColor("caret_color");
        this.themeCache.MinimumCharacterWidth   = this.GetThemeConstant("minimum_character_width");
        this.themeCache.SelectionColor          = this.GetThemeColor("selection_color");
        this.themeCache.ClearIcon               = this.GetThemeIcon("clear");
        this.themeCache.ClearButtonColor        = this.GetThemeColor("clear_button_color");
        this.themeCache.ClearButtonColorPressed = this.GetThemeColor("clear_button_color_pressed");
        this.themeCache.BaseScale               = this.GetThemeDefaultBaseScale();
    }
    #endregion protected methods

    #region public methods
    public void Deselect()
    {
        this.selection.Begin       = 0;
        this.selection.End         = 0;
        this.selection.StartColumn = 0;
        this.selection.Enabled     = false;
        this.selection.Creating    = false;
        this.selection.DoubleClick = false;

        this.QueueRedraw();
    }

    public void SelectAll() => throw new NotImplementedException();
    public void ShowVirtualKeyboard() => throw new NotImplementedException();
    public void SelectionDelete() => throw new NotImplementedException();
    #endregion public methods

    #region public overrided methods
    public override void Notification(NotificationKind notification, bool reversed = false)
    {
        if (!reversed)
        {
            base.Notification(notification, reversed);
        }

        switch (notification)
        {
            #if TOOLS_ENABLED
            case NotificationKind.NOTIFICATION_ENTER_TREE:
                {
                    if (Engine.Singleton.IsEditorHint && !this.Tree.IsNodeBeingEdited(this))
                    {
                        this.CaretBlinkEnabled  = EDITOR_GET<bool>("text_editor/appearance/caret/caret_blink");
                        this.CaretBlinkInterval = EDITOR_GET<float>("text_editor/appearance/caret/caret_blink_interval");

                        EditorSettings.Singleton.SettingsChanged -= this.EditorSettingsChanged;
                        EditorSettings.Singleton.SettingsChanged += this.EditorSettingsChanged;
                    }
                }
                break;
            #endif

            case NotificationKind.CONTROL_NOTIFICATION_RESIZED:
                this.FitToWidth();
                this.scrollOffset = 0;
                this.CaretColumn  = this.caretColumn;

                break;

            case NotificationKind.CONTROL_NOTIFICATION_LAYOUT_DIRECTION_CHANGED:
            case NotificationKind.CONTROL_NOTIFICATION_THEME_CHANGED:
                this.Shape();
                this.QueueRedraw();

                break;

            case NotificationKind.MAIN_LOOP_NOTIFICATION_TRANSLATION_CHANGED:
                this.placeholderTranslated = this.Atr(this.placeholder);
                this.Shape();
                this.QueueRedraw();

                break;

            case NotificationKind.NOTIFICATION_WM_WINDOW_FOCUS_IN:
                this.windowHasFocus = true;
                this.ValidateCaretCanDraw();
                this.QueueRedraw();

                break;

            case NotificationKind.NOTIFICATION_WM_WINDOW_FOCUS_OUT:
                this.windowHasFocus = false;
                this.ValidateCaretCanDraw();
                this.QueueRedraw();

                break;

            case NotificationKind.NOTIFICATION_INTERNAL_PROCESS:
                if (this.caretBlinkEnabled && this.caretCanDraw)
                {
                    this.caretBlinkTimer += this.ProcessDeltaTime;

                    if (this.caretBlinkTimer >= this.caretBlinkInterval)
                    {
                        this.caretBlinkTimer = 0.0;
                        this.ToggleDrawCaret();
                    }
                }

                break;

            case NotificationKind.CANVAS_ITEM_NOTIFICATION_DRAW:
                {
                    var rtl = this.IsLayoutRtl();

                    var size = this.Size;
                    var width = size.X;
                    var height = size.Y;

                    var ci = this.CanvasItemId;

                    var style = this.themeCache.Normal;

                    if (!this.Editable)
                    {
                        style = this.themeCache.ReadOnly;
                    }
                    var font = this.themeCache.Font;

                    ASSERT_NOT_NULL(style);

                    if (!this.flat)
                    {
                        style.Draw(ci, new(new(), size));
                    }

                    if (this.HasFocus)
                    {
                        ASSERT_NOT_NULL(this.themeCache.Focus);

                        this.themeCache.Focus.Draw(ci, new(new(), size));
                    }

                    var xOfs = 0;
                    var usingPlaceholder = string.IsNullOrEmpty(this.text) && string.IsNullOrEmpty(this.imeText);
                    var textWidth  = TextServerManager.Singleton.PrimaryInterface.ShapedTextGetSize(this.textId).X;
                    var textHeight = TextServerManager.Singleton.PrimaryInterface.ShapedTextGetSize(this.textId).Y;

                    switch (this.alignment)
                    {
                        case HorizontalAlignment.HORIZONTAL_ALIGNMENT_FILL:
                        case HorizontalAlignment.HORIZONTAL_ALIGNMENT_LEFT:
                            {
                                xOfs = rtl
                                    ? (int)Math.Max(style.GetMargin(Side.SIDE_LEFT), (int)(size.X - Math.Ceiling(style.GetMargin(Side.SIDE_RIGHT) + textWidth)))
                                    : (int)style.Offset.X;
                            }
                            break;
                        case HorizontalAlignment.HORIZONTAL_ALIGNMENT_CENTER:
                            {
                                xOfs = !MathX.IsZeroApprox(this.scrollOffset)
                                    ? (int)style.Offset.X
                                    : (int)Math.Max(style.GetMargin(Side.SIDE_LEFT), (int)(size.X - textWidth) / 2);
                            }
                            break;
                        case HorizontalAlignment.HORIZONTAL_ALIGNMENT_RIGHT:
                            {
                                xOfs = rtl
                                    ? (int)style.Offset.X
                                    : (int)Math.Max(style.GetMargin(Side.SIDE_LEFT), (int)(size.X - Math.Ceiling(style.GetMargin(Side.SIDE_RIGHT) + textWidth)));
                            }
                            break;
                    }

                    var ofsMax = (int)(width - style.GetMargin(Side.SIDE_RIGHT));
                    var yArea  = (int)(height - style.MinimumSize.Y);
                    var yOfs   = (int)(style.Offset.Y + (yArea - textHeight) / 2);

                    var selectionColor = this.themeCache.SelectionColor;
                    var fontColor         = this.Editable ? this.themeCache.FontColor : this.themeCache.FontUneditableColor;
                    var fontSelectedColor = this.themeCache.FontSelectedColor;
                    var caretColor        = this.themeCache.CaretColor;

                    // Draw placeholder color.
                    if (usingPlaceholder)
                    {
                        fontColor = this.themeCache.FontPlaceholderColor;
                    }

                    var displayClearIcon = !usingPlaceholder && this.Editable && this.clearButtonEnabled;
                    if (this.rightIcon != null || displayClearIcon)
                    {
                        var rIcon = displayClearIcon ? this.themeCache.ClearIcon : this.rightIcon;
                        var colorIcon = new Color(1, 1, 1, !this.Editable ? 0.5f * 0.9f : 0.9f);
                        if (displayClearIcon)
                        {
                            colorIcon = this.clearButtonStatus.PressAttempt && this.clearButtonStatus.PressingInside
                                ? this.themeCache.ClearButtonColorPressed
                                : this.themeCache.ClearButtonColor;
                        }

                        ASSERT_NOT_NULL(rIcon);

                        rIcon.Draw(ci, new(width - rIcon.Width - style.GetMargin(Side.SIDE_RIGHT), height / 2 - rIcon.Height / 2), colorIcon);

                        if (this.alignment == HorizontalAlignment.HORIZONTAL_ALIGNMENT_CENTER)
                        {
                            if (MathX.IsZeroApprox(this.scrollOffset))
                            {
                                xOfs = (int)Math.Max(style.GetMargin(Side.SIDE_LEFT), (int)(size.X - textWidth - rIcon.Width - style.GetMargin(Side.SIDE_RIGHT) * 2) / 2);
                            }
                        }
                        else
                        {
                            xOfs = (int)Math.Max(style.GetMargin(Side.SIDE_LEFT), xOfs - rIcon.Width - style.GetMargin(Side.SIDE_RIGHT));
                        }

                        ofsMax -= rIcon.Width;
                    }

                    // Draw selections rects.
                    var ofs = new Vector2<RealT>(xOfs + this.scrollOffset, yOfs);
                    if (this.selection.Enabled)
                    {
                        var sel = TextServerManager.Singleton.PrimaryInterface.ShapedTextGetSelection(this.textId, this.selection.Begin, this.selection.End);
                        for (var i = 0; i < sel.Count; i++)
                        {
                            var rect = new Rect2<RealT>(sel[i].X + ofs.X, ofs.Y, sel[i].Y - sel[i].X, textHeight);
                            if (rect.Position.X + rect.Size.X <= xOfs || rect.Position.X > ofsMax)
                            {
                                continue;
                            }
                            if (rect.Position.X < xOfs)
                            {
                                rect.Size = rect.Size with { X = rect.Size.X - (xOfs - rect.Position.X) };

                                rect.Position = rect.Position with { X = xOfs };
                            }
                            else if (rect.Position.X + rect.Size.X > ofsMax)
                            {
                                rect.Size = rect.Size with { X = ofsMax - rect.Position.X };
                            }

                            RS.Singleton.CanvasItemAddRect(ci, rect, selectionColor);
                        }
                    }

                    var glyphs = TextServerManager.Singleton.PrimaryInterface.ShapedTextGetGlyphs(this.textId);
                    var glSize = (int)TextServerManager.Singleton.PrimaryInterface.ShapedTextGetGlyphCount(this.textId);

                    // Draw text.
                    ofs.Y += (int)TextServerManager.Singleton.PrimaryInterface.ShapedTextGetAscent(this.textId);
                    var fontOutlineColor = this.themeCache.FontOutlineColor;
                    var outlineSize      = this.themeCache.FontOutlineSize;

                    if (outlineSize > 0 && fontOutlineColor.A > 0)
                    {
                        var oofs = ofs;
                        for (var i = 0; i < glSize; i++)
                        {
                            for (var j = 0; j < glyphs[i].Repeat; j++)
                            {
                                if (Math.Ceiling(oofs.X) >= xOfs && oofs.X + glyphs[i].Advance <= ofsMax)
                                {
                                    if (glyphs[i].FontId != default)
                                    {
                                        TextServerManager.Singleton.PrimaryInterface.FontDrawGlyphOutline(
                                            glyphs[i].FontId,
                                            ci,
                                            glyphs[i].FontSize,
                                            outlineSize,
                                            oofs + new Vector2<RealT>(glyphs[i].XOff, glyphs[i].YOff),
                                            glyphs[i].Index,
                                            fontOutlineColor
                                        );
                                    }
                                }
                                oofs.X += glyphs[i].Advance;
                            }
                            if (oofs.X >= ofsMax)
                            {
                                break;
                            }
                        }
                    }
                    for (var i = 0; i < glSize; i++)
                    {
                        var selected = this.selection.Enabled && glyphs[i].Start >= this.selection.Begin && glyphs[i].End <= this.selection.End;
                        for (var j = 0; j < glyphs[i].Repeat; j++)
                        {
                            if (Math.Ceiling(ofs.X) >= xOfs && ofs.X + glyphs[i].Advance <= ofsMax)
                            {
                                if (glyphs[i].FontId != default)
                                {
                                    TextServerManager.Singleton.PrimaryInterface.FontDrawGlyph(
                                        glyphs[i].FontId,
                                        ci,
                                        glyphs[i].FontSize,
                                        ofs + new Vector2<RealT>(glyphs[i].XOff, glyphs[i].YOff),
                                        glyphs[i].Index,
                                        selected ? fontSelectedColor : fontColor
                                    );
                                }
                                else if (!glyphs[i].Flags.HasFlag(TextServer.GraphemeFlag.GRAPHEME_IS_VIRTUAL))
                                {
                                    TextServerManager.Singleton.PrimaryInterface.DrawHexCodeBox(
                                        ci,
                                        glyphs[i].FontSize,
                                        ofs + new Vector2<RealT>(glyphs[i].XOff, glyphs[i].YOff),
                                        glyphs[i].Index,
                                        selected ? fontSelectedColor : fontColor
                                    );
                                }
                            }
                            ofs.X += glyphs[i].Advance;
                        }
                        if (ofs.X >= ofsMax)
                        {
                            break;
                        }
                    }

                    // Draw carets.
                    ofs.X = xOfs + this.scrollOffset;
                    if (this.caretCanDraw && this.drawCaret || this.dragCaretForceDisplayed)
                    {
                        // Prevent carets from disappearing at theme scales below 1.0 (if the caret width is 1).
                        var caretWidth = (int)(this.themeCache.CaretWidth * Math.Max(1, this.themeCache.BaseScale));

                        if (string.IsNullOrEmpty(this.imeText) || this.imeSelection.Y == 0)
                        {
                            // Normal caret.
                            var caret = TextServerManager.Singleton.PrimaryInterface.ShapedTextGetCarets(this.textId, (long)(string.IsNullOrEmpty(this.imeText) ? this.caretColumn : this.caretColumn + this.imeSelection.X));
                            if (usingPlaceholder || caret.LCaret == default && caret.TCaret == default)
                            {
                                // No carets, add one at the start.
                                ASSERT_NOT_NULL(this.themeCache.Font);
                                var h = (int)this.themeCache.Font.GetHeight(this.themeCache.FontSize);
                                var y = (int)(style.Offset.Y + (yArea - h) / 2);
                                caret.LDir = rtl ? TextServer.Direction.DIRECTION_RTL : TextServer.Direction.DIRECTION_LTR;
                                switch (this.alignment)
                                {
                                    case HorizontalAlignment.HORIZONTAL_ALIGNMENT_FILL:
                                    case HorizontalAlignment.HORIZONTAL_ALIGNMENT_LEFT:
                                        {
                                            caret.LCaret = rtl ? new(new(ofsMax, y), new(caretWidth, h)) : new(new(style.Offset.X, y), new(caretWidth, h));
                                        }
                                        break;
                                    case HorizontalAlignment.HORIZONTAL_ALIGNMENT_CENTER:
                                        {
                                            caret.LCaret = new(new(size.X / 2, y), new(caretWidth, h));
                                        }
                                        break;
                                    case HorizontalAlignment.HORIZONTAL_ALIGNMENT_RIGHT:
                                        {
                                            caret.LCaret = rtl ? new(new(style.Offset.X, y), new(caretWidth, h)) : new(new(ofsMax, y), new(caretWidth, h));
                                        }
                                        break;
                                }

                                RS.Singleton.CanvasItemAddRect(ci, caret.LCaret, caretColor);
                            }
                            else
                            {
                                if (caret.LCaret != default && caret.LDir == TextServer.Direction.DIRECTION_AUTO)
                                {
                                    // Draw extra marker on top of mid caret.
                                    var trect = new Rect2<RealT>((RealT)(caret.LCaret.Position.X - 2.5 * caretWidth), caret.LCaret.Position.Y, 6 * caretWidth, caretWidth);
                                    trect.Position += ofs;
                                    RS.Singleton.CanvasItemAddRect(ci, trect, caretColor);
                                }
                                else if (caret.LCaret != default && caret.TCaret != default && caret.LDir != caret.TDir)
                                {
                                    // Draw extra direction marker on top of split caret.
                                    var d = (caret.LDir == TextServer.Direction.DIRECTION_LTR) ? 0.5f : -3f;
                                    var trect = new Rect2<RealT>(caret.LCaret.Position.X + d * caretWidth, caret.LCaret.Position.Y + caret.LCaret.Size.Y - caretWidth, 3 * caretWidth, caretWidth);
                                    trect.Position += ofs;
                                    RS.Singleton.CanvasItemAddRect(ci, trect, caretColor);

                                    d = (caret.TDir == TextServer.Direction.DIRECTION_LTR) ? 0.5f : -3f;
                                    trect = new Rect2<RealT>(caret.TCaret.Position.X + d * caretWidth, caret.TCaret.Position.Y, 3 * caretWidth, caretWidth);
                                    trect.Position += ofs;
                                    RS.Singleton.CanvasItemAddRect(ci, trect, caretColor);
                                }

                                caret.LCaret = caret.LCaret with
                                {
                                    Position = caret.LCaret.Position + ofs,
                                    Size     = caret.LCaret.Size with { X = caretWidth }
                                };

                                RS.Singleton.CanvasItemAddRect(ci, caret.LCaret, caretColor);

                                caret.TCaret = caret.TCaret with
                                {
                                    Position = caret.TCaret.Position + ofs,
                                    Size     = caret.TCaret.Size with { X = caretWidth }
                                };

                                RS.Singleton.CanvasItemAddRect(ci, caret.TCaret, caretColor);
                            }
                        }
                        if (!string.IsNullOrEmpty(this.imeText))
                        {
                            {
                                // IME intermediate text range.
                                var sel = TextServerManager.Singleton.PrimaryInterface.ShapedTextGetSelection(
                                    this.textId,
                                    this.caretColumn,
                                    this.caretColumn + this.imeText.Length
                                );

                                for (var i = 0; i < sel.Count; i++)
                                {
                                    var rect = new Rect2<RealT>(sel[i].X + ofs.X, ofs.Y, sel[i].Y - sel[i].X, textHeight);
                                    if (rect.Position.X + rect.Size.X <= xOfs || rect.Position.X > ofsMax)
                                    {
                                        continue;
                                    }
                                    if (rect.Position.X < xOfs)
                                    {
                                        rect = rect with
                                        {
                                            Position = rect.Position with { X = xOfs },
                                            Size     = rect.Position with { X = rect.Size.X - (xOfs - rect.Position.X) },
                                        };
                                    }
                                    else if (rect.Position.X + rect.Size.X > ofsMax)
                                    {
                                        rect = rect with
                                        {
                                            Size = rect.Position with { X = ofsMax - rect.Position.X },
                                        };
                                    }

                                    rect = rect with
                                    {
                                        Size = rect.Position with { Y = caretWidth },
                                    };
                                    RS.Singleton.CanvasItemAddRect(ci, rect, caretColor);
                                }
                            }
                            {
                                // IME caret.
                                if (this.imeSelection.Y > 0)
                                {
                                    var sel = TextServerManager.Singleton.PrimaryInterface.ShapedTextGetSelection(
                                        this.textId,
                                        (long)(this.caretColumn + this.imeSelection.X),
                                        (long)(this.caretColumn + this.imeSelection.X + this.imeSelection.Y)
                                    );

                                    for (var i = 0; i < sel.Count; i++)
                                    {
                                        var rect = new Rect2<RealT>(sel[i].X + ofs.X, ofs.Y, sel[i].Y - sel[i].X, textHeight);
                                        if (rect.Position.X + rect.Size.X <= xOfs || rect.Position.X > ofsMax)
                                        {
                                            continue;
                                        }
                                        if (rect.Position.X < xOfs)
                                        {
                                            rect = rect with
                                            {
                                                Position = rect.Position with { X = xOfs },
                                                Size     = rect.Size     with { X = rect.Size.X - (xOfs - rect.Position.X) }
                                            };
                                        }
                                        else if (rect.Position.X + rect.Size.X > ofsMax)
                                        {
                                            rect = rect with
                                            {
                                                Size = rect.Size with { X = ofsMax - rect.Position.X }
                                            };
                                        }

                                        rect = rect with
                                        {
                                            Size = rect.Size with { Y = caretWidth * 3 }
                                        };

                                        RS.Singleton.CanvasItemAddRect(ci, rect, caretColor);
                                    }
                                }
                            }
                        }
                    }

                    if (this.HasFocus)
                    {
                        if (this.Viewport.WindowId != DisplayServer.INVALID_WINDOW_ID && DisplayServer.Singleton.HasFeature(DisplayServer.Feature.FEATURE_IME))
                        {
                            ASSERT_NOT_NULL(this.themeCache.Font);

                            DisplayServer.Singleton.WindowSetImeActive(true, this.Viewport.WindowId);
                            var pos = new Vector2<RealT>(this.GetCaretPixelPos().X, (this.Size.Y + this.themeCache.Font.GetHeight(this.themeCache.FontSize)) / 2);
                            DisplayServer.Singleton.WindowSetImePosition(this.GetGlobalPosition() + pos, this.Viewport.WindowId);
                        }
                    }
                }
                break;

            case NotificationKind.CONTROL_NOTIFICATION_FOCUS_ENTER:
                {
                    this.ValidateCaretCanDraw();

                    if (this.selectAllOnFocus)
                    {
                        if (Input.Singleton.IsMouseButtonPressed(MouseButton.LEFT))
                        {
                            // Select all when the mouse button is up.
                            this.pendingSelectAllOnFocus = true;
                        }
                        else
                        {
                            this.SelectAll();
                        }
                    }

                    if (this.Viewport.WindowId != DisplayServer.INVALID_WINDOW_ID && DisplayServer.Singleton.HasFeature(DisplayServer.Feature.FEATURE_IME))
                    {
                        ASSERT_NOT_NULL(this.themeCache.Font);
                        DisplayServer.Singleton.WindowSetImeActive(true, this.Viewport.WindowId);
                        var pos = new Vector2<RealT>(this.GetCaretPixelPos().X, (this.Size.Y + this.themeCache.Font.GetHeight(this.themeCache.FontSize)) / 2);
                        DisplayServer.Singleton.WindowSetImePosition(this.GetGlobalPosition() + pos, this.Viewport.WindowId);
                    }

                    this.ShowVirtualKeyboard();
                }
                break;

            case NotificationKind.CONTROL_NOTIFICATION_FOCUS_EXIT:
                {
                    this.ValidateCaretCanDraw();

                    if (this.Viewport.WindowId != DisplayServer.INVALID_WINDOW_ID && DisplayServer.Singleton.HasFeature(DisplayServer.Feature.FEATURE_IME))
                    {
                        DisplayServer.Singleton.WindowSetImePosition(new Vector2<RealT>(), this.Viewport.WindowId);
                        DisplayServer.Singleton.WindowSetImeActive(false, this.Viewport.WindowId);
                    }
                    this.imeText = "";
                    this.imeSelection = new Vector2<RealT>();
                    this.Shape();
                    this.CaretColumn = this.caretColumn; // Update scroll_offset

                    if (DisplayServer.Singleton.HasFeature(DisplayServer.Feature.FEATURE_VIRTUAL_KEYBOARD) && this.virtualKeyboardEnabled)
                    {
                        DisplayServer.Singleton.VirtualKeyboardHide();
                    }

                    if (this.deselectOnFocusLossEnabled && !this.selection.DragAttempt)
                    {
                        this.Deselect();
                    }
                }
                break;

            case NotificationKind.MAIN_LOOP_NOTIFICATION_OS_IME_UPDATE:
                {
                    if (this.HasFocus)
                    {
                        this.imeText      = DisplayServer.Singleton.ImeGetText();
                        this.imeSelection = DisplayServer.Singleton.ImeGetSelection();

                        if (!string.IsNullOrEmpty(this.imeText))
                        {
                            this.SelectionDelete();
                        }

                        this.Shape();

                        this.CaretColumn = this.caretColumn; // Update scroll_offset

                        this.QueueRedraw();
                    }
                }
                break;

            case NotificationKind.NOTIFICATION_DRAG_BEGIN:
                this.dragAction = true;
                break;

            case NotificationKind.NOTIFICATION_DRAG_END:
                {
                    if (this.IsDragSuccessful())
                    {
                        if (this.selection.DragAttempt)
                        {
                            this.selection.DragAttempt = false;
                            if (this.Editable && !Input.Singleton.IsKeyPressed(Key.CTRL))
                            {
                                this.SelectionDelete();
                            }
                            else if (this.deselectOnFocusLossEnabled)
                            {
                                this.Deselect();
                            }
                        }
                    }
                    else
                    {
                        this.selection.DragAttempt = false;
                    }
                    this.dragAction              = false;
                    this.dragCaretForceDisplayed = false;
                }
                break;
        }

        if (reversed)
        {
            base.Notification(notification, reversed);
        }
    }
    #endregion public overrided methods
}
