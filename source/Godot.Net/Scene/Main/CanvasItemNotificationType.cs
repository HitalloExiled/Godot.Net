namespace Godot.Net.Scene.Main;

public enum CanvasItemNotificationType
{
    NOTIFICATION_TRANSFORM_CHANGED       = NotificationKind.WINDOW_NOTIFICATION_TRANSFORM_CHANGED, //unique
    NOTIFICATION_DRAW                    = 30,
    NOTIFICATION_VISIBILITY_CHANGED      = 31,
    NOTIFICATION_ENTER_CANVAS            = 32,
    NOTIFICATION_EXIT_CANVAS             = 33,
    NOTIFICATION_LOCAL_TRANSFORM_CHANGED = 35,
    NOTIFICATION_WORLD_2D_CHANGED        = 36,
}
