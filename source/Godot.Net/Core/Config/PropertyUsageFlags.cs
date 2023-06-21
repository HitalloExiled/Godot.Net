namespace Godot.Net.Core.Config;

[Flags]
public enum PropertyUsageFlags
{
    PROPERTY_USAGE_NONE                      = 0,
    PROPERTY_USAGE_STORAGE                   = 1 << 1,
    PROPERTY_USAGE_EDITOR                    = 1 << 2,
    PROPERTY_USAGE_INTERNAL                  = 1 << 3,
    PROPERTY_USAGE_CHECKABLE                 = 1 << 4, // Used for editing global variables.
    PROPERTY_USAGE_CHECKED                   = 1 << 5, // Used for editing global variables.
    PROPERTY_USAGE_GROUP                     = 1 << 6, // Used for grouping props in the editor.
    PROPERTY_USAGE_CATEGORY                  = 1 << 7,
    PROPERTY_USAGE_SUBGROUP                  = 1 << 8,
    PROPERTY_USAGE_CLASS_IS_BITFIELD         = 1 << 9,
    PROPERTY_USAGE_NO_INSTANCE_STATE         = 1 << 10,
    PROPERTY_USAGE_RESTART_IF_CHANGED        = 1 << 11,
    PROPERTY_USAGE_SCRIPT_VARIABLE           = 1 << 12,
    PROPERTY_USAGE_STORE_IF_NULL             = 1 << 13,
    PROPERTY_USAGE_UPDATE_ALL_IF_MODIFIED    = 1 << 14,
    PROPERTY_USAGE_SCRIPT_DEFAULT_VALUE      = 1 << 15,
    PROPERTY_USAGE_CLASS_IS_ENUM             = 1 << 16,
    PROPERTY_USAGE_NIL_IS_VARIANT            = 1 << 17,
    PROPERTY_USAGE_ARRAY                     = 1 << 18, // Used in the inspector to group properties as elements of an array.
    PROPERTY_USAGE_ALWAYS_DUPLICATE          = 1 << 19, // When duplicating a resource, always duplicate, even with subresource duplication disabled.
    PROPERTY_USAGE_NEVER_DUPLICATE           = 1 << 20, // When duplicating a resource, never duplicate, even with subresource duplication enabled.
    PROPERTY_USAGE_HIGH_END_GFX              = 1 << 21,
    PROPERTY_USAGE_NODE_PATH_FROM_SCENE_ROOT = 1 << 22,
    PROPERTY_USAGE_RESOURCE_NOT_PERSISTENT   = 1 << 23,
    PROPERTY_USAGE_KEYING_INCREMENTS         = 1 << 24, // Used in inspector to increment property when keyed in animation player.
    PROPERTY_USAGE_DEFERRED_SET_RESOURCE     = 1 << 25, // when loading, the resource for this property can be set at the end of loading.
    PROPERTY_USAGE_EDITOR_INSTANTIATE_OBJECT = 1 << 26, // For Object properties, instantiate them when creating in editor.
    PROPERTY_USAGE_EDITOR_BASIC_SETTING      = 1 << 27, //for project or editor settings, show when basic settings are selected.
    PROPERTY_USAGE_READ_ONLY                 = 1 << 28, // Mark a property as read-only in the inspector.
    PROPERTY_USAGE_DEFAULT                   = PROPERTY_USAGE_STORAGE | PROPERTY_USAGE_EDITOR,
    PROPERTY_USAGE_NO_EDITOR                 = PROPERTY_USAGE_STORAGE,
}
