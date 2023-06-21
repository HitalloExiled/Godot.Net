namespace Godot.Net.Scene.Main;

using Godot.Net.Core.Input;
using Godot.Net.Core.Math;
using Godot.Net.Scene.GUI;

using ObjectID = Int32;

public partial class Viewport
{
    public record GUI
    {
        // info used when this is a window

        public List<Control>             Roots      { get; } = new();
        public List<SubWindow>           SubWindows { get; } = new(); // Don't obtain references or pointers to the elements, as their location can change.
        public Dictionary<int, ObjectID> TouchFocus { get; } = new();

        public bool                      ForcedMouseFocus         { get; set; } //used for menu buttons
        public bool                      MouseInViewport          { get; set; } = true;
        public bool                      KeyEventAccepted         { get; set; }
        public Control?                  MouseFocus               { get; set; }
        public Control?                  LastMouseFocus           { get; set; }
        public Control?                  MouseClickGrabber        { get; set; }
        public MouseButtonMask           MouseFocusMask           { get; set; }
        public Control?                  KeyFocus                 { get; set; }
        public Control?                  MouseOver                { get; set; }
        public Control?                  DragMouseOver            { get; set; }
        public Vector2<RealT>            DragMouseOverPos         { get; set; }
        public Control?                  TooltipControl           { get; set; }
        public Window?                   TooltipPopup             { get; set; }
        public Label?                    TooltipLabel             { get; set; }
        public Vector2<RealT>            TooltipPos               { get; set; }
        public Vector2<RealT>            LastMousePos             { get; set; }
        public Vector2<RealT>            DragAccum                { get; set; }
        public bool                      DragAttempted            { get; set; }
        public object?                   DragData                 { get; set; }
        public ObjectID                  DragPreviewId            { get; set; }
        public SceneTreeTimer?           TooltipTimer             { get; set; }
        public double                    TooltipDelay             { get; set; }
        public bool                      RootsOrderDirty          { get; set; }
        public int                       CanvasSortIndex          { get; set; } //for sorting items with canvas as root
        public bool                      Dragging                 { get; set; }
        public bool                      DragSuccessful           { get; set; }
        public bool                      EmbedSubwindowsHint      { get; set; }
        public Window?                   SubwindowFocused         { get; set; }
        public SubWindowDrag             SubwindowDrag            { get; set; } = SubWindowDrag.SUB_WINDOW_DRAG_DISABLED;
        public Vector2<RealT>            SubwindowDragFrom        { get; set; } = new();
        public Vector2<RealT>            SubwindowDragPos         { get; set; } = new();
        public Rect2<int>                SubwindowDragCloseRect   { get; set; }
        public bool                      SubwindowDragCloseInside { get; set; }
        public SubWindowResize           SubwindowResizeMode      { get; set; }
        public Rect2<int>                SubwindowResizeFromRect  { get; set; }
    }
}
