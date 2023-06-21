namespace Godot.Net.Scene.Main;

using Godot.Net.Core.OS;
using Godot.Net.Scene.GUI;

#pragma warning disable CA1069 // FIXME

public enum NotificationKind
{
    NOTIFICATION_POSTINITIALIZE     = 0,
    // you can make your own, but don't use the same numbers as other notifications in other nodes
    NOTIFICATION_ENTER_TREE         = 10,
    NOTIFICATION_EXIT_TREE          = 11,
    NOTIFICATION_MOVED_IN_PARENT    = 12,
    NOTIFICATION_READY              = 13,
    NOTIFICATION_PAUSED             = 14,
    NOTIFICATION_UNPAUSED           = 15,
    NOTIFICATION_PHYSICS_PROCESS    = 16,
    NOTIFICATION_PROCESS            = 17,
    NOTIFICATION_PARENTED           = 18,
    NOTIFICATION_UNPARENTED         = 19,
    NOTIFICATION_SCENE_INSTANTIATED = 20,
    NOTIFICATION_DRAG_BEGIN         = 21,
    NOTIFICATION_DRAG_END           = 22,
    NOTIFICATION_PATH_RENAMED       = 23,
    //NOTIFICATION_TRANSLATION_CHANGED = 24, moved below
    NOTIFICATION_INTERNAL_PROCESS         = 25,
    NOTIFICATION_INTERNAL_PHYSICS_PROCESS = 26,
    NOTIFICATION_POST_ENTER_TREE          = 27,
    NOTIFICATION_DISABLED                 = 28,
    NOTIFICATION_ENABLED                  = 29,
    NOTIFICATION_NODE_RECACHE_REQUESTED   = 30,
    //keep these linked to node

    NOTIFICATION_WM_MOUSE_ENTER      = 1002,
    NOTIFICATION_WM_MOUSE_EXIT       = 1003,
    NOTIFICATION_WM_WINDOW_FOCUS_IN  = 1004,
    NOTIFICATION_WM_WINDOW_FOCUS_OUT = 1005,
    NOTIFICATION_WM_CLOSE_REQUEST    = 1006,
    NOTIFICATION_WM_GO_BACK_REQUEST  = 1007,
    NOTIFICATION_WM_SIZE_CHANGED     = 1008,
    NOTIFICATION_WM_DPI_CHANGE       = 1009,
    NOTIFICATION_VP_MOUSE_ENTER      = 1010,
    NOTIFICATION_VP_MOUSE_EXIT       = 1011,

    MAIN_LOOP_NOTIFICATION_OS_MEMORY_WARNING     = MainLoopNotificationType.NOTIFICATION_OS_MEMORY_WARNING,
    MAIN_LOOP_NOTIFICATION_TRANSLATION_CHANGED   = MainLoopNotificationType.NOTIFICATION_TRANSLATION_CHANGED,
    MAIN_LOOP_NOTIFICATION_WM_ABOUT              = MainLoopNotificationType.NOTIFICATION_WM_ABOUT,
    MAIN_LOOP_NOTIFICATION_CRASH                 = MainLoopNotificationType.NOTIFICATION_CRASH,
    MAIN_LOOP_NOTIFICATION_OS_IME_UPDATE         = MainLoopNotificationType.NOTIFICATION_OS_IME_UPDATE,
    MAIN_LOOP_NOTIFICATION_APPLICATION_RESUMED   = MainLoopNotificationType.NOTIFICATION_APPLICATION_RESUMED,
    MAIN_LOOP_NOTIFICATION_APPLICATION_PAUSED    = MainLoopNotificationType.NOTIFICATION_APPLICATION_PAUSED,
    MAIN_LOOP_NOTIFICATION_APPLICATION_FOCUS_IN  = MainLoopNotificationType.NOTIFICATION_APPLICATION_FOCUS_IN,
    MAIN_LOOP_NOTIFICATION_APPLICATION_FOCUS_OUT = MainLoopNotificationType.NOTIFICATION_APPLICATION_FOCUS_OUT,
    MAIN_LOOP_NOTIFICATION_TEXT_SERVER_CHANGED   = MainLoopNotificationType.NOTIFICATION_TEXT_SERVER_CHANGED,

    // Editor specific node notifications
    NOTIFICATION_EDITOR_PRE_SAVE = 9001,
    NOTIFICATION_EDITOR_POST_SAVE = 9002,

    // MainLoop
    WINDOW_NOTIFICATION_TRANSFORM_CHANGED  = MainLoopNotificationType.NOTIFICATION_TRANSFORM_CHANGED,
    WINDOW_NOTIFICATION_POST_POPUP         = Window.WindowNotificationType.NOTIFICATION_POST_POPUP,
    WINDOW_NOTIFICATION_THEME_CHANGED      = Window.WindowNotificationType.NOTIFICATION_THEME_CHANGED,
    WINDOW_NOTIFICATION_VISIBILITY_CHANGED = Window.WindowNotificationType.NOTIFICATION_VISIBILITY_CHANGED,

    // CanvasItem

    CANVAS_ITEM_NOTIFICATION_TRANSFORM_CHANGED       = CanvasItemNotificationType.NOTIFICATION_TRANSFORM_CHANGED,
    CANVAS_ITEM_NOTIFICATION_DRAW                    = CanvasItemNotificationType.NOTIFICATION_DRAW,
    CANVAS_ITEM_NOTIFICATION_VISIBILITY_CHANGED      = CanvasItemNotificationType.NOTIFICATION_VISIBILITY_CHANGED,
    CANVAS_ITEM_NOTIFICATION_ENTER_CANVAS            = CanvasItemNotificationType.NOTIFICATION_ENTER_CANVAS,
    CANVAS_ITEM_NOTIFICATION_EXIT_CANVAS             = CanvasItemNotificationType.NOTIFICATION_EXIT_CANVAS,
    CANVAS_ITEM_NOTIFICATION_LOCAL_TRANSFORM_CHANGED = CanvasItemNotificationType.NOTIFICATION_LOCAL_TRANSFORM_CHANGED,
    CANVAS_ITEM_NOTIFICATION_WORLD_2D_CHANGED        = CanvasItemNotificationType.NOTIFICATION_WORLD_2D_CHANGED,

    CONTROL_NOTIFICATION_RESIZED                  = ControlNotificationType.NOTIFICATION_RESIZED,
    CONTROL_NOTIFICATION_MOUSE_ENTER              = ControlNotificationType.NOTIFICATION_MOUSE_ENTER,
    CONTROL_NOTIFICATION_MOUSE_EXIT               = ControlNotificationType.NOTIFICATION_MOUSE_EXIT,
    CONTROL_NOTIFICATION_FOCUS_ENTER              = ControlNotificationType.NOTIFICATION_FOCUS_ENTER,
    CONTROL_NOTIFICATION_FOCUS_EXIT               = ControlNotificationType.NOTIFICATION_FOCUS_EXIT,
    CONTROL_NOTIFICATION_THEME_CHANGED            = ControlNotificationType.NOTIFICATION_THEME_CHANGED,
    CONTROL_NOTIFICATION_SCROLL_BEGIN             = ControlNotificationType.NOTIFICATION_SCROLL_BEGIN,
    CONTROL_NOTIFICATION_SCROLL_END               = ControlNotificationType.NOTIFICATION_SCROLL_END,
    CONTROL_NOTIFICATION_LAYOUT_DIRECTION_CHANGED = ControlNotificationType.NOTIFICATION_LAYOUT_DIRECTION_CHANGED,

    CONTAINER_NOTIFICATION_PRE_SORT_CHILDREN      = ContainerNotificationKind.NOTIFICATION_PRE_SORT_CHILDREN,
    CONTAINER_NOTIFICATION_SORT_CHILDREN          = ContainerNotificationKind.NOTIFICATION_SORT_CHILDREN,
}
