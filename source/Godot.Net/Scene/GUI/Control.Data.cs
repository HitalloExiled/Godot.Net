namespace Godot.Net.Scene.GUI;

using Godot.Net.Core.Math;
using Godot.Net.Core.String;
using Godot.Net.Scene.Main;
using Godot.Net.Scene.Resources;
using Godot.Net.Scene.Theme;

using ThemeColorMap    = Dictionary<string, Core.Math.Color>;
using ThemeConstantMap = Dictionary<string, int>;
using ThemeFontMap     = Dictionary<string, Resources.Font>;
using ThemeFontSizeMap = Dictionary<string, int>;
using ThemeStyleMap    = Dictionary<string, Resources.StyleBox>;

#pragma warning disable IDE0052 // TODO Remove

public partial class Control
{
    private record Data
    {
        // Global relations.

        public Dictionary<string, ThemeColorMap>                 ThemeColorCache         { get; } = new();
        public ThemeColorMap                                     ThemeColorOverride      { get; } = new();
        public Dictionary<string, ThemeConstantMap>              ThemeConstantCache      { get; } = new();
        public ThemeConstantMap                                  ThemeConstantOverride   { get; } = new();
        public Dictionary<string, ThemeFontMap>                  ThemeFontCache          { get; } = new();
        public ThemeFontMap                                      ThemeFontOverride       { get; } = new();
        public Dictionary<string, ThemeFontSizeMap>              ThemeFontSizeCache      { get; } = new();
        public ThemeFontSizeMap                                  ThemeFontSizeOverride   { get; } = new();
        public Dictionary<string, Dictionary<string, Texture2D>> ThemeIconCache          { get; } = new();
        public Dictionary<string, Texture2D>                     ThemeIconOverride       { get; } = new();
        public ThemeOwner                                        ThemeOwner              { get; } = new();
        public Dictionary<string, ThemeStyleMap>                 ThemeStyleCache         { get; } = new();
        public ThemeStyleMap                                     ThemeStyleOverride      { get; } = new();

        public RealT[]                                           Anchor                  { get; set; } = { (RealT)AnchorKind.ANCHOR_BEGIN, (RealT)AnchorKind.ANCHOR_BEGIN, (RealT)AnchorKind.ANCHOR_BEGIN, (RealT)AnchorKind.ANCHOR_BEGIN };
        public bool                                              AutoTranslate           { get; set; } = true;
        public bool                                              BlockMinimumSizeAdjust  { get; set; }
        public bool                                              BulkThemeOverride       { get; set; }
        public bool                                              ClipContents            { get; set; }
        public Vector2<RealT>                                    CustomMinimumSize       { get; set; }
        public CursorShape                                       DefaultCursor           { get; set; } = CursorShape.CURSOR_ARROW;
        public bool                                              DisableVisibilityClip   { get; set; }
        public RealT                                             Expand                  { get; set; } = (RealT)1.0;
        public FocusModeKind                                     FocusMode               { get; set; } = FocusModeKind.FOCUS_NONE;
        public NodePath[]                                        FocusNeighbor           { get; set; } = new NodePath[4];
        public NodePath?                                         FocusNext               { get; set; }
        public NodePath?                                         FocusPrev               { get; set; }
        public bool                                              ForcePassScrollEvents   { get; set; } = true;
        public ForwardCanDrop?                                   ForwardCanDrop          { get; set; }
        public ForwardDrag?                                      ForwardDrag             { get; set; }
        public ForwardDrop?                                      ForwardDrop             { get; set; }
        public GrowDirection                                     HGrow                   { get; set; } = GrowDirection.GROW_DIRECTION_END;
        public SizeFlags                                         HSizeFlags              { get; set; } = SizeFlags.SIZE_FILL;
        public bool                                              IsRtl                   { get; set; }
        public bool                                              IsRtlDirty              { get; set; } = true;
        public Vector2<RealT>                                    LastMinimumSize         { get; set; }
        public LayoutDirection                                   LayoutDir               { get; set; } = LayoutDirection.LAYOUT_DIRECTION_INHERITED;
        public bool                                              LocalizeNumeralSystem   { get; set; } = true;
        public Vector2<RealT>                                    MinimumSizeCache        { get; set; }
        public bool                                              MinimumSizeValid        { get; set; }
        public MouseFilterKind                                   MouseFilter             { get; set; } = MouseFilterKind.MOUSE_FILTER_STOP;
        public RealT[]                                           Offset                  { get; set; } = { (RealT)0.0, (RealT)0.0, (RealT)0.0, (RealT)0.0 };
        public CanvasItem?                                       ParentCanvasItem        { get; set; }
        public Control?                                          ParentControl           { get; set; }
        public Window?                                           ParentWindow            { get; set; }
        public Vector2<RealT>                                    PivotOffset             { get; set; }
        public Vector2<RealT>                                    PosCache                { get; set; }
        public Control?                                          RI                      { get; set; }
        public RealT                                             Rotation                { get; set; }
        public Vector2<RealT>                                    Scale                   { get; set; } = new(1, 1);
        public object?                                           ShortcutContext         { get; set; }
        public Vector2<RealT>                                    SizeCache               { get; set; }
        public bool                                              SizeWarning             { get; set; } = true;
        public LayoutMode                                        StoredLayoutMode        { get; set; } = LayoutMode.LAYOUT_MODE_POSITION;
        public bool                                              StoredUseCustomAnchors  { get; set; }
        public Theme?                                            Theme                   { get; set; }
        public string?                                           ThemeTypeVariation      { get; set; }
        public string?                                           Tooltip                 { get; set; }
        public bool                                              UpdatingLastMinimumSize { get; set; }
        public GrowDirection                                     VGrow                   { get; set; } = GrowDirection.GROW_DIRECTION_END;
        public SizeFlags                                         VSizeFlags              { get; set; } = SizeFlags.SIZE_FILL;
    }

}
