#define TOOLS_ENABLED

namespace Godot.Net.Scene.GUI;

using System.Runtime.CompilerServices;
using Godot.Net.Core;
using Godot.Net.Core.Config;
using Godot.Net.Core.Enums;
using Godot.Net.Core.Input;
using Godot.Net.Core.Math;
using Godot.Net.Core.String;
using Godot.Net.Scene.Main;
using Godot.Net.Scene.Resources;
using Godot.Net.Servers;

using ThemeOwner = Theme.ThemeOwner;

#pragma warning disable IDE0052, CS0067 // TODO Remove

public partial class Control : CanvasItem
{
    #region public delegates
    public delegate bool ForwardCanDrop(in Vector2<RealT> point, object? data, Control fromControl);
    public delegate void ForwardDrop(in Vector2<RealT> point, object? data, Control fromControl);
    public delegate object? ForwardDrag(in Vector2<RealT> point, Control fromControl);
    #endregion public delegates

    #region public events
    public event Action? Resized;
	public event Action? GuiInput;
	public event Action? MouseEntered;
	public event Action? MouseExited;
	public event Action? FocusEntered;
	public event Action? FocusExited;
	public event Action? SizeFlagsChanged;
	public event Action? MinimumSizeChanged;
	public event Action? ThemeChanged;
    #endregion public events

    #region private readonly fields
    private readonly Data       data       = new();
    private readonly ThemeOwner themeOwner = new();
    #endregion private readonly fields

    #region private fields
    #endregion private fields

    #region private readonly properties
    private Transform2D<RealT> InternalTransform
    {
        get
        {
            var rotScale = new Transform2D<RealT>();
            rotScale.SetRotationAndScale(this.data.Rotation, this.data.Scale);

            var offset = new Transform2D<RealT>
            {
                Origin = -this.data.PivotOffset
            };

            return offset.AffineInverse() * (rotScale * offset);
        }
    }
    #endregion private readonly properties

    #region protected readonly properties
    #endregion protected readonly properties

    #region public readonly properties
    public virtual Vector2<RealT> CombinedMinimumSize
    {
        get
        {
            if (!this.data.MinimumSizeValid)
            {
                this.UpdateMinimumSizeCache();
            }

            return this.data.MinimumSizeCache;
        }
    }

    public bool HasFocus => this.IsInsideTree && this.Viewport.GuiControlHasFocus(this);

    public bool HasThemeOwnerNode => this.data.ThemeOwner.OwnerNode != null;

    public virtual Vector2<RealT> MinimumSize
    {
        get
        {
            var ms = new Vector2<RealT>();
            // GDVIRTUAL_CALL(_get_minimum_size, ms);
            return ms;
        }
    }

    public Control?       ParentControl => this.data.ParentControl;
    public Window?        ParentWindow  => this.data.ParentWindow;
    public Vector2<float> Position      => this.data.PosCache;
    public Vector2<RealT> Size          => this.data.SizeCache;

    #endregion public readonly properties

    #region public properties
    public CursorShape DefaultCursorShape
    {
        get => throw new NotImplementedException();
        set
        {
            if (ERR_FAIL_INDEX(value, CursorShape.CURSOR_MAX))
            {
                return;
            }

            if (this.data.DefaultCursor == value)
            {
                return;
            }

            this.data.DefaultCursor = value;

            if (!this.IsInsideTree)
            {
                return;
            }

            if (!this.GetGlobalRect().HasPoint(this.GetGlobalMousePosition()))
            {
                return;
            }

            this.Viewport.GetBaseWindow().UpdateMouseCursorShape();
        }
    }

    public FocusModeKind FocusMode
    {
        get => this.data.FocusMode;
        set
        {
            if (ERR_FAIL_INDEX((int)value, 3))
            {
                return;
            }

            if (this.IsInsideTree && value == FocusModeKind.FOCUS_NONE && this.data.FocusMode != FocusModeKind.FOCUS_NONE && this.HasFocus)
            {
                this.ReleaseFocus();
            }

            this.data.FocusMode = value;
        }
    }

    public SizeFlags HSizeFlags
    {
        get => this.data.HSizeFlags;
        set
        {
            if ((int)this.data.HSizeFlags == (int)value)
            {
                return;
            }
            this.data.HSizeFlags = value;
            SizeFlagsChanged?.Invoke();
        }
    }

    public MouseFilterKind MouseFilter
    {
        get => throw new NotImplementedException();
        set
        {
            if (ERR_FAIL_INDEX((int)value, 3))
            {
                return;
            }

            this.data.MouseFilter = value;
            this.NotifyPropertyListChanged();
            this.UpdateConfigurationWarnings();
        }
    }

    public RealT Rotation
    {
        get => this.data.Rotation;
        set
        {
            if (this.data.Rotation == value)
            {
                return;
            }

            this.data.Rotation = value;

            this.QueueRedraw();
            this.NotifyTransform();
        }
    }

    public Vector2<RealT> Scale
    {
        get => this.data.Scale;
        set
        {
            if (this.data.Scale == value)
            {
                return;
            }

            this.data.Scale = value;
            // Avoid having 0 scale values, can lead to errors in physics and rendering.
            if (this.data.Scale.X == 0)
            {
                this.data.Scale = this.data.Scale with { X = MathX.CMP_EPSILON };
            }
            if (this.data.Scale.Y == 0)
            {
                this.data.Scale = this.data.Scale with { Y = MathX.CMP_EPSILON };
            }

            this.QueueRedraw();
            this.NotifyTransform();
        }
    }

    public RealT StretchRatio
    {
        get => this.data.Expand;
        set => throw new NotImplementedException();
    }

    public Theme? Theme
    {
        get => this.data.Theme;
        set
        {
            if (this.data.Theme == value)
            {
                return;
            }

            if (this.data.Theme != null)
            {
                this.data.Theme.Changed -= this.OnThemeChanged;
            }

            this.data.Theme = value;

            if (this.data.Theme != null)
            {
                ThemeOwner.PropagateThemeChanged(this, this, this.IsInsideTree, true);

                this.data.Theme.Changed += this.OnThemeChanged;

                return;
            }

            if (this.Parent is Control parentC && parentC.HasThemeOwnerNode)
            {
                ThemeOwner.PropagateThemeChanged(this, parentC.ThemeOwnerNode, this.IsInsideTree, true);
                return;
            }

            if (this.Parent is Window parentW && parentW.HasThemeOwnerNode)
            {
                ThemeOwner.PropagateThemeChanged(this, parentW.ThemeOwnerNode, this.IsInsideTree, true);
                return;
            }

            ThemeOwner.PropagateThemeChanged(this, null, this.IsInsideTree, true);
        }
    }

    public Node? ThemeOwnerNode
    {
        get => this.data.ThemeOwner.OwnerNode;
        set => this.data.ThemeOwner.OwnerNode = value;
    }

    public string? TooltipText
    {
        get => this.data.Tooltip;
        set
        {
            this.data.Tooltip = value;
            this.UpdateConfigurationWarnings();
        }
    }

    public SizeFlags VSizeFlags
    {
        get => this.data.VSizeFlags;
        set
        {
            if ((int)this.data.VSizeFlags == (int)value)
            {
                return;
            }

            this.data.VSizeFlags = value;

            SizeFlagsChanged?.Invoke();
        }
    }
    #endregion public properties

    public Control()
    {
        //
    }

    #region private methods
    private void CallGuiInput(InputEvent @event) => throw new NotImplementedException();

    private void ClearSizeWarning() =>
        this.data.SizeWarning = false;

    private void ComputeAnchors(in Rect2<RealT> rect, RealT[] offsets, RealT[] anchors)
    {
        var parentRectSize = this.GetParentAnchorableRect().Size;
        if (ERR_FAIL_COND(parentRectSize.X == 0.0))
        {
            return;
        }

        if (ERR_FAIL_COND(parentRectSize.Y == 0.0))
        {
            return;
        }

        var x = rect.Position.X;
        if (this.IsLayoutRtl())
        {
            x = parentRectSize.X - x - rect.Size.X;
        }

        anchors[0] = (x - offsets[0]) / parentRectSize.X;
        anchors[1] = (rect.Position.Y - offsets[1]) / parentRectSize.Y;
        anchors[2] = (x + rect.Size.X - offsets[2]) / parentRectSize.X;
        anchors[3] = (rect.Position.Y + rect.Size.Y - offsets[3]) / parentRectSize.Y;
    }

    private void ComputeOffsets(in Rect2<RealT> rect, RealT[] anchors, RealT[] offsets)
    {
        var parentRectSize = this.GetParentAnchorableRect().Size;

        var x = rect.Position.X;

        if (this.IsLayoutRtl())
        {
            x = parentRectSize.X - x - rect.Size.X;
        }

        offsets[0] = x - anchors[0] * parentRectSize.X;
        offsets[1] = rect.Position.Y - anchors[1] * parentRectSize.Y;
        offsets[2] = x + rect.Size.X - anchors[2] * parentRectSize.X;
        offsets[3] = rect.Position.Y + rect.Size.Y - anchors[3] * parentRectSize.Y;
    }

    private int GetAnchorsLayoutPreset()
    {
        // If this is a layout mode that doesn't rely on anchors, avoid excessive checks.
        if (this.data.StoredLayoutMode != LayoutMode.LAYOUT_MODE_UNCONTROLLED && this.data.StoredLayoutMode != LayoutMode.LAYOUT_MODE_ANCHORS)
        {
            return (int)LayoutPreset.PRESET_TOP_LEFT;
        }

        // If the custom preset was selected by user, use it.
        if (this.data.StoredUseCustomAnchors)
        {
            return -1;
        }

        // Check anchors to determine if the current state matches a preset, or not.

        var left   = this.GetAnchor(Side.SIDE_LEFT);
        var right  = this.GetAnchor(Side.SIDE_RIGHT);
        var top    = this.GetAnchor(Side.SIDE_TOP);
        var bottom = this.GetAnchor(Side.SIDE_BOTTOM);

        if (left == (int)AnchorKind.ANCHOR_BEGIN && right == (int)AnchorKind.ANCHOR_BEGIN && top == (int)AnchorKind.ANCHOR_BEGIN && bottom == (int)AnchorKind.ANCHOR_BEGIN)
        {
            return (int)LayoutPreset.PRESET_TOP_LEFT;
        }
        if (left == (int)AnchorKind.ANCHOR_END && right == (int)AnchorKind.ANCHOR_END && top == (int)AnchorKind.ANCHOR_BEGIN && bottom == (int)AnchorKind.ANCHOR_BEGIN)
        {
            return (int)LayoutPreset.PRESET_TOP_RIGHT;
        }
        if (left == (int)AnchorKind.ANCHOR_BEGIN && right == (int)AnchorKind.ANCHOR_BEGIN && top == (int)AnchorKind.ANCHOR_END && bottom == (int)AnchorKind.ANCHOR_END)
        {
            return (int)LayoutPreset.PRESET_BOTTOM_LEFT;
        }
        if (left == (int)AnchorKind.ANCHOR_END && right == (int)AnchorKind.ANCHOR_END && top == (int)AnchorKind.ANCHOR_END && bottom == (int)AnchorKind.ANCHOR_END)
        {
            return (int)LayoutPreset.PRESET_BOTTOM_RIGHT;
        }

        if (left == (int)AnchorKind.ANCHOR_BEGIN && right == (int)AnchorKind.ANCHOR_BEGIN && top == 0.5 && bottom == 0.5)
        {
            return (int)LayoutPreset.PRESET_CENTER_LEFT;
        }
        if (left == (int)AnchorKind.ANCHOR_END && right == (int)AnchorKind.ANCHOR_END && top == 0.5 && bottom == 0.5)
        {
            return (int)LayoutPreset.PRESET_CENTER_RIGHT;
        }
        if (left == 0.5 && right == 0.5 && top == (int)AnchorKind.ANCHOR_BEGIN && bottom == (int)AnchorKind.ANCHOR_BEGIN)
        {
            return (int)LayoutPreset.PRESET_CENTER_TOP;
        }
        if (left == 0.5 && right == 0.5 && top == (int)AnchorKind.ANCHOR_END && bottom == (int)AnchorKind.ANCHOR_END)
        {
            return (int)LayoutPreset.PRESET_CENTER_BOTTOM;
        }
        if (left == 0.5 && right == 0.5 && top == 0.5 && bottom == 0.5)
        {
            return (int)LayoutPreset.PRESET_CENTER;
        }

        if (left == (int)AnchorKind.ANCHOR_BEGIN && right == (int)AnchorKind.ANCHOR_BEGIN && top == (int)AnchorKind.ANCHOR_BEGIN && bottom == (int)AnchorKind.ANCHOR_END)
        {
            return (int)LayoutPreset.PRESET_LEFT_WIDE;
        }
        if (left == (int)AnchorKind.ANCHOR_END && right == (int)AnchorKind.ANCHOR_END && top == (int)AnchorKind.ANCHOR_BEGIN && bottom == (int)AnchorKind.ANCHOR_END)
        {
            return (int)LayoutPreset.PRESET_RIGHT_WIDE;
        }
        if (left == (int)AnchorKind.ANCHOR_BEGIN && right == (int)AnchorKind.ANCHOR_END && top == (int)AnchorKind.ANCHOR_BEGIN && bottom == (int)AnchorKind.ANCHOR_BEGIN)
        {
            return (int)LayoutPreset.PRESET_TOP_WIDE;
        }
        if (left == (int)AnchorKind.ANCHOR_BEGIN && right == (int)AnchorKind.ANCHOR_END && top == (int)AnchorKind.ANCHOR_END && bottom == (int)AnchorKind.ANCHOR_END)
        {
            return (int)LayoutPreset.PRESET_BOTTOM_WIDE;
        }

        if (left == 0.5 && right == 0.5 && top == (int)AnchorKind.ANCHOR_BEGIN && bottom == (int)AnchorKind.ANCHOR_END)
        {
            return (int)LayoutPreset.PRESET_VCENTER_WIDE;
        }
        if (left == (int)AnchorKind.ANCHOR_BEGIN && right == (int)AnchorKind.ANCHOR_END && top == 0.5 && bottom == 0.5)
        {
            return (int)LayoutPreset.PRESET_HCENTER_WIDE;
        }

        if (left == (int)AnchorKind.ANCHOR_BEGIN && right == (int)AnchorKind.ANCHOR_END && top == (int)AnchorKind.ANCHOR_BEGIN && bottom == (int)AnchorKind.ANCHOR_END)
        {
            return (int)LayoutPreset.PRESET_FULL_RECT;
        }

        // Does not match any preset, return "Custom".
        return -1;
    }

    private LayoutMode GetDefaultLayoutMode() => throw new NotImplementedException();
    private Control GetFocusNeighbor(Side side, int count = 0) => throw new NotImplementedException();


    private LayoutMode GetLayoutMode()
    {
        var parentNode = this.ParentControl;
        // In these modes the property is read-only.
        if (parentNode == null)
        {
            return LayoutMode.LAYOUT_MODE_UNCONTROLLED;
        }
        else if (parentNode is Container)
        {
            return LayoutMode.LAYOUT_MODE_CONTAINER;
        }

        // If anchors are not in the top-left position, this is definitely in anchors mode.
        if (this.GetAnchorsLayoutPreset() != (int)LayoutPreset.PRESET_TOP_LEFT)
        {
            return LayoutMode.LAYOUT_MODE_ANCHORS;
        }

        // Otherwise fallback on what's stored.
        return this.data.StoredLayoutMode;
    }

    private void InvalidateThemeCache()
    {
        this.data.ThemeIconCache.Clear();
        this.data.ThemeStyleCache.Clear();
        this.data.ThemeFontCache.Clear();
        this.data.ThemeFontSizeCache.Clear();
        this.data.ThemeColorCache.Clear();
        this.data.ThemeConstantCache.Clear();
    }

    private void NotifyThemeOverrideChanged()
    {
        if (!this.data.BulkThemeOverride && this.IsInsideTree)
        {
            this.Notification(NotificationKind.WINDOW_NOTIFICATION_THEME_CHANGED);
        }
    }

    private void SetAnchor(Side side, RealT anchor) => throw new NotImplementedException();
    private void SetAnchorsLayoutPreset(int preset) => throw new NotImplementedException();
    private void SetGlobalPosition(Vector2<RealT> point) => throw new NotImplementedException();
    private void SetLayoutMode(LayoutMode mode) => throw new NotImplementedException();

    private void SizeChanged()
    {
        var parentRect = this.GetParentAnchorableRect();

        var edgePos = new RealT[4];

        for (var i = 0; i < 4; i++)
        {
            var area = parentRect.Size[i & 1];
            edgePos[i] = this.data.Offset[i] + this.data.Anchor[i] * area;
        }

        var newPosCache  = new Vector2<RealT>(edgePos[0], edgePos[1]);
        var newSizeCache = new Vector2<RealT>(edgePos[2], edgePos[3]) - newPosCache;

        var minimumSize = this.CombinedMinimumSize;

        if (minimumSize.X > newSizeCache.X)
        {
            if (this.data.HGrow == GrowDirection.GROW_DIRECTION_BEGIN)
            {
                newPosCache.X += newSizeCache.X - minimumSize.X;
            }
            else if (this.data.HGrow == GrowDirection.GROW_DIRECTION_BOTH)
            {
                newPosCache.X += 0.5f * (newSizeCache.X - minimumSize.X);
            }

            newSizeCache.X = minimumSize.X;
        }

        if (this.IsLayoutRtl())
        {
            newPosCache.X = parentRect.Size.X - newPosCache.X - newSizeCache.X;
        }

        if (minimumSize.Y > newSizeCache.Y)
        {
            if (this.data.VGrow == GrowDirection.GROW_DIRECTION_BEGIN)
            {
                newPosCache.Y += newSizeCache.Y - minimumSize.Y;
            }
            else if (this.data.VGrow == GrowDirection.GROW_DIRECTION_BOTH)
            {
                newPosCache.Y += 0.5f * (newSizeCache.Y - minimumSize.Y);
            }

            newSizeCache.Y = minimumSize.Y;
        }

        var posChanged  = newPosCache != this.data.PosCache;
        var sizeChanged = newSizeCache != this.data.SizeCache;

        this.data.PosCache  = newPosCache;
        this.data.SizeCache = newSizeCache;

        if (this.IsInsideTree)
        {
            if (sizeChanged)
            {
                this.Notification(NotificationKind.CONTROL_NOTIFICATION_RESIZED);
            }
            if (posChanged || sizeChanged)
            {
                this.NotifyItemRectChanged(sizeChanged);
                this.NotifyTransform();
            }

            if (posChanged && !sizeChanged)
            {
                this.UpdateCanvasItemTransform(); //move because it won't be updated
            }
        }
    }

    private void OnThemeChanged() => throw new NotImplementedException();
    private void TopLevelChanged() => throw new NotImplementedException(); // Controls don't need to do anything, only other CanvasItems.
    private void TopLevelChangedOnParent() => throw new NotImplementedException();

    private void UpdateCanvasItemTransform()
    {
        var xform = this.InternalTransform;
        xform[2] += this.Position;

        // We use a little workaround to avoid flickering when moving the pivot with _edit_set_pivot()
        if (this.IsInsideTree && Math.Abs(Math.Sin(this.data.Rotation * 4.0f)) < 0.00001f && this.Viewport.SnapControlsToPixelsEnabled)
        {
            xform[2] = xform[2].Round();
        }

        RS.Singleton.CanvasItemSetTransform(this.CanvasItemId, xform);
    }

    private void UpdateLayoutMode()
    {
        var computedLayout = this.GetLayoutMode();

        if (this.data.StoredLayoutMode != computedLayout)
        {
            this.data.StoredLayoutMode = computedLayout;
            this.NotifyPropertyListChanged();
        }
    }

    private void UpdateMinimumSizeInternal() => throw new NotImplementedException();

    private void UpdateMinimumSizeCache()
    {
        var minsize = this.MinimumSize;

        minsize.X = Math.Max(minsize.X, this.data.CustomMinimumSize.X);
        minsize.Y = Math.Max(minsize.Y, this.data.CustomMinimumSize.Y);

        var sizeChanged = false;
        if (this.data.MinimumSizeCache != minsize)
        {
            sizeChanged = true;
        }

        this.data.MinimumSizeCache = minsize;
        this.data.MinimumSizeValid = true;

        if (sizeChanged)
        {
            this.UpdateMinimumSize();
        }
    }

    private void WindowFindFocusNeighbor(Vector2<RealT> dir, Node at, Vector2<RealT> points, RealT min, out RealT closestDist, out Control[] closest) => throw new NotImplementedException();
    #endregion private methods

    #region protected virtual methods
    protected virtual IList<Vector3<int>> StructuredTextParser(TextServer.StructuredTextParser parserType, object[] args, string text)
    {
        if (parserType == TextServer.StructuredTextParser.STRUCTURED_TEXT_CUSTOM)
        {
            // GDVIRTUAL_CALL
            return Array.Empty<Vector3<int>>();
        }
        else
        {
            return TextServer.ParseStructuredText(parserType, args, text);
        }
    }
    protected virtual void UpdateThemeItemCache() { }
    #endregion protected virtual methods

    #region protected methods
    protected bool Get(string name, out object? ret) => throw new NotImplementedException();
    protected void GetPropertyList(List<PropertyInfo> list) => throw new NotImplementedException();
    protected bool PropertyCanRevert(string name) => throw new NotImplementedException();
    protected bool PropertyGetRevert(string name, out object? property) => throw new NotImplementedException();
    protected bool Set(string name, object? value) => throw new NotImplementedException();
    protected void ValidateProperty(PropertyInfo property) => throw new NotImplementedException();
    #endregion protected virtual methods

    #region public virtual methods
    public virtual bool CanDropData(Vector2<RealT> point, object? data) => throw new NotImplementedException();
    public virtual void DropData(Vector2<RealT> point, object? data) => throw new NotImplementedException();


    public virtual CursorShape GetCursorShape(in Vector2<RealT> pos = default) => throw new NotImplementedException();
    public virtual VariantType GetDragData(in Vector2<RealT> point) => throw new NotImplementedException();

    public virtual string GetTooltip(in Vector2<RealT> pos) => throw new NotImplementedException();
    public virtual void OnGuiInput(InputEvent @event) => throw new NotImplementedException();
    public virtual bool HasPoint(Vector2<RealT> point) => throw new NotImplementedException();

    public virtual bool IsLayoutRtl()
    {
        if (this.data.IsRtlDirty)
        {
            this.data.IsRtlDirty = false;
            if (this.data.LayoutDir == LayoutDirection.LAYOUT_DIRECTION_INHERITED)
            {
                var parentWindow = this.ParentWindow;
                var parentControl = this.ParentControl;
                if (parentControl != null)
                {
                    this.data.IsRtl = parentControl.IsLayoutRtl();
                }
                else if (parentWindow != null)
                {
                    this.data.IsRtl = parentWindow.IsLayoutRtl();
                }
                else
                {
                    if (GLOBAL_GET<bool>("internationalization/rendering/force_right_to_left_layout_direction"))
                    {
                        this.data.IsRtl = true;
                    }
                    else
                    {
                        var locale = TranslationServer.Singleton.GetToolLocale();
                        this.data.IsRtl = TextServerManager.Singleton.IsLocaleRightToLeft(locale);
                    }
                }
            }
            else if (this.data.LayoutDir == LayoutDirection.LAYOUT_DIRECTION_LOCALE)
            {
                if (GLOBAL_GET<bool>("internationalization/rendering/force_right_to_left_layout_direction"))
                {
                    this.data.IsRtl = true;
                }
                else
                {
                    var locale = TranslationServer.Singleton.GetToolLocale();
                    this.data.IsRtl = TextServerManager.Singleton.IsLocaleRightToLeft(locale);
                }
            }
            else
            {
                this.data.IsRtl = this.data.LayoutDir == LayoutDirection.LAYOUT_DIRECTION_RTL;
            }
        }

        return this.data.IsRtl;
    }

    public virtual bool IsTextField() => throw new NotImplementedException();
    public virtual Control MakeCustomTooltip(string text) => throw new NotImplementedException();

    public virtual void SetDragForwarding(ForwardDrag drag, ForwardCanDrop canDrop, ForwardDrop drop)
    {
        this.data.ForwardDrag    = drag;
        this.data.ForwardCanDrop = canDrop;
        this.data.ForwardDrop    = drop;
    }
    #endregion public virtual methods

    #region public methods
    public void AcceptEvent() => throw new NotImplementedException();
    public void AddThemeColorOverride(string name, in Color color) => throw new NotImplementedException();
    public void AddThemeConstantOverride(string name, int constant) => throw new NotImplementedException();

    public void AddThemeFontOverride(string name, Font font)
    {
        if (ERR_FAIL_COND(font == null))
        {
            return;
        }

        if (this.data.ThemeFontOverride.TryGetValue(name, out var value))
        {
            value.Changed -= this.NotifyThemeOverrideChanged;
        }

        this.data.ThemeFontOverride[name] = font!;
        font!.Changed += this.NotifyThemeOverrideChanged;
        this.NotifyThemeOverrideChanged();
    }

    public void AddThemeFontSizeOverride(string name, int fontSize) => throw new NotImplementedException();
    public void AddThemeIconOverride(string name, Texture2D icon) => throw new NotImplementedException();

    public void AddThemeStyleOverride(string name, StyleBox style)
    {
        if (ERR_FAIL_COND(style == null))
        {
            return;
        }

        if (this.data.ThemeStyleOverride.TryGetValue(name, out var value))
        {
            value.Changed -= this.NotifyThemeOverrideChanged;
        }

        this.data.ThemeStyleOverride[name] = style!;
        this.data.ThemeStyleOverride[name].Changed += this.NotifyThemeOverrideChanged;
        this.NotifyThemeOverrideChanged();
    }

    #pragma warning disable CA1822
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public string Atr(string @string) =>
        // TODO return is_auto_translating() ? tr(p_string) : p_string;
        @string;
    #pragma warning disable CA1822

    public void BeginBulkThemeOverride() => throw new NotImplementedException();
    public void EndBulkThemeOverride() => throw new NotImplementedException();
    public Control FindNextValidFocus() => throw new NotImplementedException();
    public Control FindPrevValidFocus() => throw new NotImplementedException();
    public void ForceDrag(object? data, Control control) => throw new NotImplementedException();

    public RealT GetAnchor(Side side) =>
        ERR_FAIL_INDEX_V((int)side, 4) ? 0 : this.data.Anchor[(int)side];

    public Vector2<RealT> GetBegin() => throw new NotImplementedException();
    public Vector2<RealT> GetCustomMinimumSize() => throw new NotImplementedException();
    public Vector2<RealT> GetEnd() => throw new NotImplementedException();
    public NodePath GetFocusNeighbor(Side side) => throw new NotImplementedException();
    public NodePath GetFocusNext() => throw new NotImplementedException();
    public NodePath GetFocusPrevious() => throw new NotImplementedException();
    public Vector2<RealT> GetGlobalPosition() => throw new NotImplementedException();
    public Rect2<RealT> GetGlobalRect() => throw new NotImplementedException();
    public GrowDirection GetHGrowDirection() => throw new NotImplementedException();

    public LayoutDirection GetLayoutDirection() => throw new NotImplementedException();

    public RealT GetOffset(Side side) => throw new NotImplementedException();

    public Rect2<RealT> GetParentAnchorableRect()
    {
        if (!this.IsInsideTree)
        {
            return new();
        }

        var parentRect = new Rect2<RealT>();
        if (this.data.ParentCanvasItem != null)
        {
            parentRect = this.data.ParentCanvasItem.GetAnchorableRect();
        }
        else
        {
            #if TOOLS_ENABLED
            var editedSceneRoot = this.Tree.EditedSceneRoot;
            var sceneRootParent = editedSceneRoot?.Parent;

            if (sceneRootParent != null && this.Viewport == sceneRootParent.Viewport)
            {
                parentRect.Size = new(GLOBAL_GET<RealT>("display/window/size/viewport_width"), GLOBAL_GET<RealT>("display/window/size/viewport_height"));
            }
            else
            {
                parentRect = this.Viewport.GetVisibleRect();
            }

            #else
            parentRect = Viewport.VisibleRect;
            #endif
        }

        return parentRect;
    }

    public Vector2<RealT> GetParentAreaSize() => throw new NotImplementedException();
    public Vector2<RealT> GetPivotOffset() => throw new NotImplementedException();
    public Rect2<RealT> GetRect() => throw new NotImplementedException();
    public Control GetRootParentControl() => throw new NotImplementedException();
    public RealT GetRotationDegrees() => throw new NotImplementedException();
    public Vector2<RealT> GetScreenPosition() => throw new NotImplementedException();
    public Rect2<RealT> GetScreenRect() => throw new NotImplementedException();
    public Node GetShortcutContext() => throw new NotImplementedException();

    public Color GetThemeColor(string name, string themeType = "")
    {
        if ((themeType == "" || themeType == this.GetType().Name || themeType == this.data.ThemeTypeVariation) && this.data.ThemeColorOverride.TryGetValue(name, out var colorOverride))
        {
            return colorOverride;
        }

        if (this.data.ThemeColorCache.TryGetValue(themeType, out var cache) && cache.TryGetValue(name, out var value))
        {
            return value;
        }

        ThemeOwner.GetThemeTypeDependencies(this, themeType, out var themeTypes);

        var color = (Color)this.data.ThemeOwner.GetThemeItemInTypes(Theme.DataType.DATA_TYPE_COLOR, name, themeTypes)!;

        if (cache == null)
        {
            this.data.ThemeColorCache[themeType] = cache = new();
        }

        cache[name] = color;

        return color;
    }

    public int GetThemeConstant(string name, string themeType = "")
    {
        if ((themeType == "" || themeType == this.GetType().Name || themeType == this.data.ThemeTypeVariation) && this.data.ThemeConstantOverride.TryGetValue(name, out var constantOverride))
        {
            return constantOverride;
        }

        if (this.data.ThemeConstantCache.TryGetValue(themeType, out var cache) && cache.TryGetValue(name, out var value))
        {
            return value;
        }

        ThemeOwner.GetThemeTypeDependencies(this, themeType, out var themeTypes);

        var constant = (int)this.data.ThemeOwner.GetThemeItemInTypes(Theme.DataType.DATA_TYPE_CONSTANT, name, themeTypes)!;

        if (cache == null)
        {
            this.data.ThemeConstantCache[themeType] = cache = new();
        }

        cache[name] = constant;

        return constant;
    }

    public float GetThemeDefaultBaseScale() =>
        this.data.ThemeOwner.GetThemeDefaultBaseScale();

    public Font GetThemeDefaultFont() => throw new NotImplementedException();
    public int GetThemeDefaultFontSize() => throw new NotImplementedException();

    public Font GetThemeFont(string name, string themeType = "")
    {
        if ((themeType == "" || themeType == this.GetType().Name || themeType == this.data.ThemeTypeVariation) && this.data.ThemeFontOverride.TryGetValue(name, out var fontOverride))
        {
            return fontOverride;
        }

        if (this.data.ThemeFontCache.TryGetValue(themeType, out var cache) && cache.TryGetValue(name, out var value))
        {
            return value;
        }

        ThemeOwner.GetThemeTypeDependencies(this, themeType, out var themeTypes);

        var font = (Font)this.data.ThemeOwner.GetThemeItemInTypes(Theme.DataType.DATA_TYPE_FONT, name, themeTypes)!;

        if (cache == null)
        {
            this.data.ThemeFontCache[themeType] = cache = new();
        }

        cache[name] = font;

        return font;
    }

    public int GetThemeFontSize(string name, string themeType = "")
    {
        if ((themeType == "" || themeType == this.GetType().Name || themeType == this.data.ThemeTypeVariation) && this.data.ThemeFontSizeOverride.TryGetValue(name, out var fontSizeOverride))
        {
            return fontSizeOverride;
        }

        if (this.data.ThemeFontSizeCache.TryGetValue(themeType, out var cache) && cache.TryGetValue(name, out var value))
        {
            return value;
        }

        ThemeOwner.GetThemeTypeDependencies(this, themeType, out var themeTypes);

        var fontSize = (int)this.data.ThemeOwner.GetThemeItemInTypes(Theme.DataType.DATA_TYPE_FONT_SIZE, name, themeTypes)!;

        if (cache == null)
        {
            this.data.ThemeFontSizeCache[themeType] = cache = new();
        }

        cache[name] = fontSize;

        return fontSize;
    }

    public Texture2D GetThemeIcon(string name, string themeType = "")
    {
        if ((themeType == "" || themeType == this.GetType().Name || themeType == this.data.ThemeTypeVariation) && this.data.ThemeIconOverride.TryGetValue(name, out var iconOverride))
        {
            return iconOverride;
        }

        if (this.data.ThemeIconCache.TryGetValue(themeType, out var cache) && cache.TryGetValue(name, out var value))
        {
            return value;
        }

        ThemeOwner.GetThemeTypeDependencies(this, themeType, out var themeTypes);

        var icon = (Texture2D)this.data.ThemeOwner.GetThemeItemInTypes(Theme.DataType.DATA_TYPE_ICON, name, themeTypes)!;

        if (cache == null)
        {
            this.data.ThemeIconCache[themeType] = cache = new();
        }

        cache[name] = icon;

        return icon;
    }

    public StyleBox GetThemeStylebox(string name, string themeType = "")
    {
        if ((themeType == "" || themeType == this.GetType().Name || themeType == this.data.ThemeTypeVariation) && this.data.ThemeStyleOverride.TryGetValue(name, out var styleOverride))
        {
            return styleOverride;
        }

        if (this.data.ThemeStyleCache.TryGetValue(themeType, out var cache) && cache.TryGetValue(name, out var value))
        {
            return value;
        }

        ThemeOwner.GetThemeTypeDependencies(this, themeType, out var themeTypes);

        var style = (StyleBox)this.data.ThemeOwner.GetThemeItemInTypes(Theme.DataType.DATA_TYPE_STYLEBOX, name, themeTypes)!;

        if (cache == null)
        {
            this.data.ThemeStyleCache[themeType] = cache = new();
        }

        cache[name] = style;

        return style!;
    }

    public string? GetThemeTypeVariation() => this.data.ThemeTypeVariation;

    public GrowDirection GetVGrowDirection() => throw new NotImplementedException();

    public void GrabClickFocus() => throw new NotImplementedException();
    public void GrabFocus() => throw new NotImplementedException();
    public bool HasThemeColor(string name, string themeType = "") => throw new NotImplementedException();
    public bool HasThemeColorOverride(string name) => throw new NotImplementedException();
    public bool HasThemeConstant(string name, string themeType = "") => throw new NotImplementedException();
    public bool HasThemeConstantOverride(string name) => throw new NotImplementedException();
    public bool HasThemeFont(string name, string themeType = "") => throw new NotImplementedException();
    public bool HasThemeFontOverride(string name) => throw new NotImplementedException();
    public bool HasThemeFontSize(string name, string themeType = "") => throw new NotImplementedException();
    public bool HasThemeFontSizeOverride(string name) => throw new NotImplementedException();
    public bool HasThemeIcon(string name, string themeType = "") => throw new NotImplementedException();
    public bool HasThemeIconOverride(string name) => throw new NotImplementedException();

    public bool HasThemeStylebox(string name, string themeType = "") => throw new NotImplementedException();
    public bool HasThemeStyleboxOverride(string name) => throw new NotImplementedException();
    public bool IsAutoTranslating() => throw new NotImplementedException();
    public bool IsClippingContents() => throw new NotImplementedException();
    public bool IsDragSuccessful() => throw new NotImplementedException();
    public bool IsFocusOwnerInShortcutContext() => throw new NotImplementedException();
    public bool IsForcePassScrollEvents() => throw new NotImplementedException();
    public bool IsLocalizingNumeralSystem() => throw new NotImplementedException();
    public bool IsTopLevelControl() => throw new NotImplementedException();
    public bool IsVisibilityClipDisabled() => throw new NotImplementedException();
    public void ReleaseFocus() => throw new NotImplementedException();
    public void RemoveThemeColorOverride(string name) => throw new NotImplementedException();
    public void RemoveThemeConstantOverride(string name) => throw new NotImplementedException();
    public void RemoveThemeFontOverride(string name) => throw new NotImplementedException();
    public void RemoveThemeFontSizeOverride(string name) => throw new NotImplementedException();
    public void RemoveThemeIconOverride(string name) => throw new NotImplementedException();
    public void RemoveThemeStyleOverride(string name) => throw new NotImplementedException();
    public void ResetSize() => throw new NotImplementedException();
    public void SetAnchor(Side side, RealT anchor, bool keepOffset = true, bool pushOppositeAnchor = true)
    {
        var sideIndex = (int)side;

        if (ERR_FAIL_INDEX(sideIndex, 4))
        {
            return;
        }

        var parentRect          = this.GetParentAnchorableRect();
        var parentRange         = (side is Side.SIDE_LEFT or Side.SIDE_RIGHT) ? parentRect.Size.X : parentRect.Size.Y;
        var previousPos         = this.data.Offset[sideIndex] + this.data.Anchor[sideIndex] * parentRange;
        var previousOppositePos = this.data.Offset[(sideIndex + 2) % 4] + this.data.Anchor[(sideIndex + 2) % 4] * parentRange;

        this.data.Anchor[sideIndex] = anchor;

        if ((side == Side.SIDE_LEFT || side == Side.SIDE_TOP) && this.data.Anchor[sideIndex] > this.data.Anchor[(sideIndex + 2) % 4] ||
                (side == Side.SIDE_RIGHT || side == Side.SIDE_BOTTOM) && this.data.Anchor[sideIndex] < this.data.Anchor[(sideIndex + 2) % 4])
        {
            if (pushOppositeAnchor)
            {
                this.data.Anchor[(sideIndex + 2) % 4] = this.data.Anchor[sideIndex];
            }
            else
            {
                this.data.Anchor[sideIndex] = this.data.Anchor[(sideIndex + 2) % 4];
            }
        }

        if (!keepOffset)
        {
            this.data.Offset[sideIndex] = previousPos - this.data.Anchor[sideIndex] * parentRange;
            if (pushOppositeAnchor)
            {
                this.data.Offset[(sideIndex + 2) % 4] = previousOppositePos - this.data.Anchor[(sideIndex + 2) % 4] * parentRange;
            }
        }
        if (this.IsInsideTree)
        {
            this.SizeChanged();
        }

        this.QueueRedraw();
    }

    public void SetAnchorAndOffset(Side side, RealT anchor, RealT pos, bool pushOppositeAnchor = true) => throw new NotImplementedException();

    public void SetAnchorsAndOffsetsPreset(LayoutPreset preset, LayoutPresetMode resizeMode = LayoutPresetMode.PRESET_MODE_MINSIZE, int margin = 0)
    {
        this.SetAnchorsPreset(preset);
        this.SetOffsetsPreset(preset, resizeMode, margin);
    }

    public void SetAnchorsPreset(LayoutPreset preset, bool keepOffsets = true)
    {
        if (ERR_FAIL_INDEX((int)preset, 16))
        {
            return;
        }

        //Left
        switch (preset)
        {
            case LayoutPreset.PRESET_TOP_LEFT:
            case LayoutPreset.PRESET_BOTTOM_LEFT:
            case LayoutPreset.PRESET_CENTER_LEFT:
            case LayoutPreset.PRESET_TOP_WIDE:
            case LayoutPreset.PRESET_BOTTOM_WIDE:
            case LayoutPreset.PRESET_LEFT_WIDE:
            case LayoutPreset.PRESET_HCENTER_WIDE:
            case LayoutPreset.PRESET_FULL_RECT:
                this.SetAnchor(Side.SIDE_LEFT, (float)AnchorKind.ANCHOR_BEGIN, keepOffsets);
                break;

            case LayoutPreset.PRESET_CENTER_TOP:
            case LayoutPreset.PRESET_CENTER_BOTTOM:
            case LayoutPreset.PRESET_CENTER:
            case LayoutPreset.PRESET_VCENTER_WIDE:
                this.SetAnchor(Side.SIDE_LEFT, 0.5f, keepOffsets);
                break;

            case LayoutPreset.PRESET_TOP_RIGHT:
            case LayoutPreset.PRESET_BOTTOM_RIGHT:
            case LayoutPreset.PRESET_CENTER_RIGHT:
            case LayoutPreset.PRESET_RIGHT_WIDE:
                this.SetAnchor(Side.SIDE_LEFT, (float)AnchorKind.ANCHOR_END, keepOffsets);
                break;
        }

        // Top
        switch (preset)
        {
            case LayoutPreset.PRESET_TOP_LEFT:
            case LayoutPreset.PRESET_TOP_RIGHT:
            case LayoutPreset.PRESET_CENTER_TOP:
            case LayoutPreset.PRESET_LEFT_WIDE:
            case LayoutPreset.PRESET_RIGHT_WIDE:
            case LayoutPreset.PRESET_TOP_WIDE:
            case LayoutPreset.PRESET_VCENTER_WIDE:
            case LayoutPreset.PRESET_FULL_RECT:
                this.SetAnchor(Side.SIDE_TOP, (float)AnchorKind.ANCHOR_BEGIN, keepOffsets);
                break;

            case LayoutPreset.PRESET_CENTER_LEFT:
            case LayoutPreset.PRESET_CENTER_RIGHT:
            case LayoutPreset.PRESET_CENTER:
            case LayoutPreset.PRESET_HCENTER_WIDE:
                this.SetAnchor(Side.SIDE_TOP, 0.5f, keepOffsets);
                break;

            case LayoutPreset.PRESET_BOTTOM_LEFT:
            case LayoutPreset.PRESET_BOTTOM_RIGHT:
            case LayoutPreset.PRESET_CENTER_BOTTOM:
            case LayoutPreset.PRESET_BOTTOM_WIDE:
                this.SetAnchor(Side.SIDE_TOP, (float)AnchorKind.ANCHOR_END, keepOffsets);
                break;
        }

        // Right
        switch (preset)
        {
            case LayoutPreset.PRESET_TOP_LEFT:
            case LayoutPreset.PRESET_BOTTOM_LEFT:
            case LayoutPreset.PRESET_CENTER_LEFT:
            case LayoutPreset.PRESET_LEFT_WIDE:
                this.SetAnchor(Side.SIDE_RIGHT, (float)AnchorKind.ANCHOR_BEGIN, keepOffsets);
                break;

            case LayoutPreset.PRESET_CENTER_TOP:
            case LayoutPreset.PRESET_CENTER_BOTTOM:
            case LayoutPreset.PRESET_CENTER:
            case LayoutPreset.PRESET_VCENTER_WIDE:
                this.SetAnchor(Side.SIDE_RIGHT, 0.5f, keepOffsets);
                break;

            case LayoutPreset.PRESET_TOP_RIGHT:
            case LayoutPreset.PRESET_BOTTOM_RIGHT:
            case LayoutPreset.PRESET_CENTER_RIGHT:
            case LayoutPreset.PRESET_TOP_WIDE:
            case LayoutPreset.PRESET_RIGHT_WIDE:
            case LayoutPreset.PRESET_BOTTOM_WIDE:
            case LayoutPreset.PRESET_HCENTER_WIDE:
            case LayoutPreset.PRESET_FULL_RECT:
                this.SetAnchor(Side.SIDE_RIGHT, (float)AnchorKind.ANCHOR_END, keepOffsets);
                break;
        }

        // Bottom
        switch (preset)
        {
            case LayoutPreset.PRESET_TOP_LEFT:
            case LayoutPreset.PRESET_TOP_RIGHT:
            case LayoutPreset.PRESET_CENTER_TOP:
            case LayoutPreset.PRESET_TOP_WIDE:
                this.SetAnchor(Side.SIDE_BOTTOM, (float)AnchorKind.ANCHOR_BEGIN, keepOffsets);
                break;

            case LayoutPreset.PRESET_CENTER_LEFT:
            case LayoutPreset.PRESET_CENTER_RIGHT:
            case LayoutPreset.PRESET_CENTER:
            case LayoutPreset.PRESET_HCENTER_WIDE:
                this.SetAnchor(Side.SIDE_BOTTOM, 0.5f, keepOffsets);
                break;

            case LayoutPreset.PRESET_BOTTOM_LEFT:
            case LayoutPreset.PRESET_BOTTOM_RIGHT:
            case LayoutPreset.PRESET_CENTER_BOTTOM:
            case LayoutPreset.PRESET_LEFT_WIDE:
            case LayoutPreset.PRESET_RIGHT_WIDE:
            case LayoutPreset.PRESET_BOTTOM_WIDE:
            case LayoutPreset.PRESET_VCENTER_WIDE:
            case LayoutPreset.PRESET_FULL_RECT:
                this.SetAnchor(Side.SIDE_BOTTOM, (float)AnchorKind.ANCHOR_END, keepOffsets);
                break;
        }
    }

    public void SetAutoTranslate(bool enable) => throw new NotImplementedException();
    public void SetBegin(in Vector2<RealT> point) => throw new NotImplementedException();
    public void SetBlockMinimumSizeAdjust(bool block) => throw new NotImplementedException();
    public void SetClipContents(bool clip) => throw new NotImplementedException();
    public void SetCustomMinimumSize(in Vector2<RealT> custom) => throw new NotImplementedException();
    public void SetDisableVisibilityClip(bool ignore) => throw new NotImplementedException();
    public void SetDragPreview(Control control) => throw new NotImplementedException();
    public void SetEnd(in Vector2<RealT> point) => throw new NotImplementedException();
    public void SetFocusNeighbor(Side side, NodePath neighbor) => throw new NotImplementedException();
    public void SetFocusNext(NodePath next) => throw new NotImplementedException();
    public void SetFocusPrevious(NodePath prev) => throw new NotImplementedException();
    public void SetForcePassScrollEvents(bool forcePassScrollEvents) => throw new NotImplementedException();
    public void SetGlobalPosition(in Vector2<RealT> point, bool keepOffsets = false) => throw new NotImplementedException();
    public void SetGrowDirectionPreset(LayoutPreset preset) => throw new NotImplementedException();
    public void SetHGrowDirection(GrowDirection direction) => throw new NotImplementedException();

    public void SetLayoutDirection(LayoutDirection direction) => throw new NotImplementedException();
    public void SetLocalizeNumeralSystem(bool enable) => throw new NotImplementedException();

    public void SetOffset(Side side, RealT value)
    {
        if (ERR_FAIL_INDEX((int)side, 4))
        {
            return;
        }

        if (this.data.Offset[(int)side] == value)
        {
            return;
        }

        this.data.Offset[(int)side] = value;
        this.SizeChanged();
    }

    public void SetOffsetsPreset(LayoutPreset preset, LayoutPresetMode resizeMode = LayoutPresetMode.PRESET_MODE_MINSIZE, int margin = 0)
    {
        if (ERR_FAIL_INDEX((int)preset, 16))
        {
            return;
        }

        if (ERR_FAIL_INDEX((int)resizeMode, 4))
        {
            return;
        }

        // Calculate the size if the node is not resized
        var minSize = this.MinimumSize;
        var newSize = this.Size;
        if (resizeMode is LayoutPresetMode.PRESET_MODE_MINSIZE or LayoutPresetMode.PRESET_MODE_KEEP_HEIGHT)
        {
            newSize.X = minSize.X;
        }

        if (resizeMode is LayoutPresetMode.PRESET_MODE_MINSIZE or LayoutPresetMode.PRESET_MODE_KEEP_WIDTH)
        {
            newSize.Y = minSize.Y;
        }

        var parentRect = this.GetParentAnchorableRect();

        var x = parentRect.Size.X;
        if (this.IsLayoutRtl())
        {
            x = parentRect.Size.X - x - newSize.X;
        }
        //Left
        switch (preset)
        {
            case LayoutPreset.PRESET_TOP_LEFT:
            case LayoutPreset.PRESET_BOTTOM_LEFT:
            case LayoutPreset.PRESET_CENTER_LEFT:
            case LayoutPreset.PRESET_TOP_WIDE:
            case LayoutPreset.PRESET_BOTTOM_WIDE:
            case LayoutPreset.PRESET_LEFT_WIDE:
            case LayoutPreset.PRESET_HCENTER_WIDE:
            case LayoutPreset.PRESET_FULL_RECT:
                this.data.Offset[0] = x * ((RealT)0.0 - this.data.Anchor[0]) + margin + parentRect.Position.X;
                break;

            case LayoutPreset.PRESET_CENTER_TOP:
            case LayoutPreset.PRESET_CENTER_BOTTOM:
            case LayoutPreset.PRESET_CENTER:
            case LayoutPreset.PRESET_VCENTER_WIDE:
                this.data.Offset[0] = x * ((RealT)0.5 - this.data.Anchor[0]) - newSize.X / 2 + parentRect.Position.X;
                break;

            case LayoutPreset.PRESET_TOP_RIGHT:
            case LayoutPreset.PRESET_BOTTOM_RIGHT:
            case LayoutPreset.PRESET_CENTER_RIGHT:
            case LayoutPreset.PRESET_RIGHT_WIDE:
                this.data.Offset[0] = x * ((RealT)1.0 - this.data.Anchor[0]) - newSize.X - margin + parentRect.Position.X;
                break;
        }

        // Top
        switch (preset)
        {
            case LayoutPreset.PRESET_TOP_LEFT:
            case LayoutPreset.PRESET_TOP_RIGHT:
            case LayoutPreset.PRESET_CENTER_TOP:
            case LayoutPreset.PRESET_LEFT_WIDE:
            case LayoutPreset.PRESET_RIGHT_WIDE:
            case LayoutPreset.PRESET_TOP_WIDE:
            case LayoutPreset.PRESET_VCENTER_WIDE:
            case LayoutPreset.PRESET_FULL_RECT:
                this.data.Offset[1] = parentRect.Size.Y * ((RealT)0.0 - this.data.Anchor[1]) + margin + parentRect.Position.Y;
                break;

            case LayoutPreset.PRESET_CENTER_LEFT:
            case LayoutPreset.PRESET_CENTER_RIGHT:
            case LayoutPreset.PRESET_CENTER:
            case LayoutPreset.PRESET_HCENTER_WIDE:
                this.data.Offset[1] = parentRect.Size.Y * ((RealT)0.5 - this.data.Anchor[1]) - newSize.Y / 2 + parentRect.Position.Y;
                break;

            case LayoutPreset.PRESET_BOTTOM_LEFT:
            case LayoutPreset.PRESET_BOTTOM_RIGHT:
            case LayoutPreset.PRESET_CENTER_BOTTOM:
            case LayoutPreset.PRESET_BOTTOM_WIDE:
                this.data.Offset[1] = parentRect.Size.Y * ((RealT)1.0 - this.data.Anchor[1]) - newSize.Y - margin + parentRect.Position.Y;
                break;
        }

        // Right
        switch (preset)
        {
            case LayoutPreset.PRESET_TOP_LEFT:
            case LayoutPreset.PRESET_BOTTOM_LEFT:
            case LayoutPreset.PRESET_CENTER_LEFT:
            case LayoutPreset.PRESET_LEFT_WIDE:
                this.data.Offset[2] = x * ((RealT)0.0 - this.data.Anchor[2]) + newSize.X + margin + parentRect.Position.X;
                break;

            case LayoutPreset.PRESET_CENTER_TOP:
            case LayoutPreset.PRESET_CENTER_BOTTOM:
            case LayoutPreset.PRESET_CENTER:
            case LayoutPreset.PRESET_VCENTER_WIDE:
                this.data.Offset[2] = x * ((RealT)0.5 - this.data.Anchor[2]) + newSize.X / 2 + parentRect.Position.X;
                break;

            case LayoutPreset.PRESET_TOP_RIGHT:
            case LayoutPreset.PRESET_BOTTOM_RIGHT:
            case LayoutPreset.PRESET_CENTER_RIGHT:
            case LayoutPreset.PRESET_TOP_WIDE:
            case LayoutPreset.PRESET_RIGHT_WIDE:
            case LayoutPreset.PRESET_BOTTOM_WIDE:
            case LayoutPreset.PRESET_HCENTER_WIDE:
            case LayoutPreset.PRESET_FULL_RECT:
                this.data.Offset[2] = x * ((RealT)1.0 - this.data.Anchor[2]) - margin + parentRect.Position.X;
                break;
        }

        // Bottom
        switch (preset)
        {
            case LayoutPreset.PRESET_TOP_LEFT:
            case LayoutPreset.PRESET_TOP_RIGHT:
            case LayoutPreset.PRESET_CENTER_TOP:
            case LayoutPreset.PRESET_TOP_WIDE:
                this.data.Offset[3] = parentRect.Size.Y * ((RealT)0.0 - this.data.Anchor[3]) + newSize.Y + margin + parentRect.Position.Y;
                break;

            case LayoutPreset.PRESET_CENTER_LEFT:
            case LayoutPreset.PRESET_CENTER_RIGHT:
            case LayoutPreset.PRESET_CENTER:
            case LayoutPreset.PRESET_HCENTER_WIDE:
                this.data.Offset[3] = parentRect.Size.Y * ((RealT)0.5 - this.data.Anchor[3]) + newSize.Y / 2 + parentRect.Position.Y;
                break;

            case LayoutPreset.PRESET_BOTTOM_LEFT:
            case LayoutPreset.PRESET_BOTTOM_RIGHT:
            case LayoutPreset.PRESET_CENTER_BOTTOM:
            case LayoutPreset.PRESET_LEFT_WIDE:
            case LayoutPreset.PRESET_RIGHT_WIDE:
            case LayoutPreset.PRESET_BOTTOM_WIDE:
            case LayoutPreset.PRESET_VCENTER_WIDE:
            case LayoutPreset.PRESET_FULL_RECT:
                this.data.Offset[3] = parentRect.Size.Y * ((RealT)1.0 - this.data.Anchor[3]) - margin + parentRect.Position.Y;
                break;
        }

        this.SizeChanged();
    }

    public void SetPivotOffset(in Vector2<RealT> pivot) => throw new NotImplementedException();

    public void SetPosition(in Vector2<RealT> point, bool keepOffsets = false)
    {
        if (keepOffsets)
        {
            this.ComputeAnchors(new(point, this.data.SizeCache), this.data.Offset, this.data.Anchor);
        }
        else
        {
            this.ComputeOffsets(new(point, this.data.SizeCache), this.data.Anchor, this.data.Offset);
        }

        this.SizeChanged();
    }

    public void SetRect(Rect2<RealT> rect)
    {
        for (var i = 0; i < 4; i++)
        {
            this.data.Anchor[i] = (float)AnchorKind.ANCHOR_BEGIN;
        }

        this.ComputeOffsets(rect, this.data.Anchor, this.data.Offset);

        if (this.IsInsideTree)
        {
            this.SizeChanged();
        }
    }

    public void SetRotationDegrees(RealT degrees) => throw new NotImplementedException();

    public void SetShortcutContext(Node node) => throw new NotImplementedException();

    public void SetSize(in Vector2<RealT> size, bool keepOffsets = false)
    {
        if (ERR_FAIL_COND(!RealT.IsFinite(size.X) || !RealT.IsFinite(size.Y)))
        {
            return;
        }

        var newSize = size;
        var min     = this.CombinedMinimumSize;

        if (newSize.X < min.X)
        {
            newSize.X = min.X;
        }
        if (newSize.Y < min.Y)
        {
            newSize.Y = min.Y;
        }

        if (keepOffsets)
        {
            this.ComputeAnchors(new(this.data.PosCache, newSize), this.data.Offset, this.data.Anchor);
        }
        else
        {
            this.ComputeOffsets(new(this.data.PosCache, newSize), this.data.Anchor, this.data.Offset);
        }

        this.SizeChanged();
    }

    public void SetThemeTypeVariation(string themeType) => throw new NotImplementedException();
    public void SetVGrowDirection(GrowDirection direction) => throw new NotImplementedException();

    public void UpdateMinimumSize()
    {
        if (!this.IsInsideTree || this.data.BlockMinimumSizeAdjust)
        {
            return;
        }

        var invalidate = this;

        // Invalidate cache upwards.
        while (invalidate != null && invalidate.data.MinimumSizeValid)
        {
            invalidate.data.MinimumSizeValid = false;
            if (invalidate.IsSetAsTopLevel)
            {
                break; // Do not go further up.
            }

            var parentWindow = invalidate.ParentWindow;
            if (parentWindow != null && parentWindow.IsWrappingControls())
            {
                parentWindow.ChildControlsChanged();
                break; // Stop on a window as well.
            }

            invalidate = invalidate.ParentControl;
        }

        if (!this.IsVisibleInTree)
        {
            return;
        }

        if (this.data.UpdatingLastMinimumSize)
        {
            return;
        }

        this.data.UpdatingLastMinimumSize = true;

        MessageQueue.Singleton.PushCallable(this.UpdateMinimumSize);
    }

    public void WarpMouse(in Vector2<RealT> position) => throw new NotImplementedException();
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
            case NotificationKind.NOTIFICATION_POSTINITIALIZE:
                this.InvalidateThemeCache();
                this.UpdateThemeItemCache();
                break;

            case NotificationKind.NOTIFICATION_PARENTED:
                {
                    var parentNode = this.Parent;

                    this.data.ParentControl = parentNode as Control;
                    this.data.ParentWindow  = parentNode as Window;

                    ThemeOwner.AssignThemeOnParented(this);

                    this.UpdateLayoutMode();
                }
                break;

            case NotificationKind.NOTIFICATION_UNPARENTED:
                this.data.ParentControl = null;
                this.data.ParentWindow  = null;

                this.data.ThemeOwner.ClearThemeOnUnparented(this);
                break;

            case NotificationKind.NOTIFICATION_ENTER_TREE:
                base.Notification(NotificationKind.WINDOW_NOTIFICATION_THEME_CHANGED);
                break;

            case NotificationKind.NOTIFICATION_POST_ENTER_TREE:
                this.data.MinimumSizeValid = false;
                this.data.IsRtlDirty       = true;

                this.SizeChanged();

                break;

            case NotificationKind.NOTIFICATION_EXIT_TREE:
                this.ReleaseFocus();
                this.Viewport.GuiRemoveControl(this);

                break;

            case NotificationKind.NOTIFICATION_READY:
                #if DEBUG
                Ready += this.ClearSizeWarning;
                #endif

                break;

            case NotificationKind.CANVAS_ITEM_NOTIFICATION_ENTER_CANVAS:
                {
                    this.data.IsRtlDirty = true;

                    var node = (CanvasItem)this;
                    var hasParentControl = false;

                    while (!node.IsSetAsTopLevel)
                    {
                        if (node.Parent is not CanvasItem parent)
                        {
                            break;
                        }

                        if (parent is Control parentControl)
                        {
                            hasParentControl = true;
                            break;
                        }

                        node = parent;
                    }

                    if (hasParentControl)
                    {
                        // Do nothing, has a parent control.
                    }
                    else
                    {
                        // Is a regular root control or top_level.
                        var viewport = this.Viewport;

                        if (ERR_FAIL_COND(viewport == null))
                        {
                            return;
                        }

                        this.data.RI = viewport!.GuiAddRootControl(this);
                    }

                    this.data.ParentCanvasItem = this.ParentItem;

                    if (this.data.ParentCanvasItem != null)
                    {
                        this.data.ParentCanvasItem.ItemRectChanged += this.SizeChanged;
                    }
                    else
                    {
                        // Connect viewport.
                        var viewport = this.Viewport;


                        if (ERR_FAIL_COND(viewport == null))
                        {
                            return;
                        }

                        viewport!.SizeChanged += this.SizeChanged;
                    }
                }
                break;

            case NotificationKind.CANVAS_ITEM_NOTIFICATION_EXIT_CANVAS:
                if (this.data.ParentCanvasItem != null)
                {
                    this.data.ParentCanvasItem.ItemRectChanged -= this.SizeChanged;
                    this.data.ParentCanvasItem = null;
                }
                else
                {
                    // Disconnect viewport.
                    var viewport = this.Viewport;
                    if (ERR_FAIL_COND(viewport == null))
                    {
                        return;
                    }

                    viewport!.SizeChanged -= this.SizeChanged;
                }

                if (this.data.RI != null)
                {
                    this.Viewport.GuiRemoveRootControl(this.data.RI);
                    this.data.RI = null;
                }

                this.data.ParentCanvasItem = null;
                this.data.IsRtlDirty       = true;

                break;

            case NotificationKind.NOTIFICATION_MOVED_IN_PARENT:
                {
                    // Some parents need to know the order of the children to draw (like TabContainer),
                    // so we update them just in case.
                    this.ParentControl?.QueueRedraw();

                    this.QueueRedraw();

                    if (this.data.RI != null)
                    {
                        this.Viewport.GuiSetRootOrderDirty();
                    }
                }
                break;

            case NotificationKind.CONTROL_NOTIFICATION_RESIZED:
                Resized?.Invoke();

                break;

            case NotificationKind.CANVAS_ITEM_NOTIFICATION_DRAW:
                this.UpdateCanvasItemTransform();
                RS.Singleton.CanvasItemSetCustomRect(this.CanvasItemId, !this.data.DisableVisibilityClip, new(default, this.Size));
                RS.Singleton.CanvasItemSetClip(this.CanvasItemId, this.data.ClipContents);

                break;

            case NotificationKind.CONTROL_NOTIFICATION_MOUSE_ENTER:
                MouseEntered?.Invoke();

                break;

            case NotificationKind.CONTROL_NOTIFICATION_MOUSE_EXIT:
                MouseExited?.Invoke();

                break;

            case NotificationKind.CONTROL_NOTIFICATION_FOCUS_ENTER:
                FocusEntered?.Invoke();
                this.QueueRedraw();

                break;

            case NotificationKind.CONTROL_NOTIFICATION_FOCUS_EXIT:
                FocusExited?.Invoke();
                this.QueueRedraw();

                break;

            case NotificationKind.CONTROL_NOTIFICATION_THEME_CHANGED:
                ThemeChanged?.Invoke();
                this.InvalidateThemeCache();
                this.UpdateThemeItemCache();
                this.UpdateMinimumSize();
                this.QueueRedraw();

                break;

            case NotificationKind.CANVAS_ITEM_NOTIFICATION_VISIBILITY_CHANGED:
                if (!this.IsVisibleInTree)
                {
                    if (this.HasViewport)
                    {
                        this.Viewport.GuiHideControl(this);
                    }
                }
                else
                {
                    this.data.MinimumSizeValid = false;
                    this.UpdateMinimumSize();
                    this.SizeChanged();
                }

                break;

            case NotificationKind.MAIN_LOOP_NOTIFICATION_TRANSLATION_CHANGED:
            case NotificationKind.CONTROL_NOTIFICATION_LAYOUT_DIRECTION_CHANGED:
                if (this.IsInsideTree)
                {
                    this.data.IsRtlDirty = true;
                    this.InvalidateThemeCache();
                    this.UpdateThemeItemCache();
                    this.SizeChanged();
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
