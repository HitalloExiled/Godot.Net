namespace Godot.Net.Core.Config;

public enum PropertyHint
{
    PROPERTY_HINT_NONE, ///< no hint provided.
	PROPERTY_HINT_RANGE, ///< hint_text = "min,max[,step][,or_greater][,or_less][,hide_slider][,radians][,degrees][,exp][,suffix:<keyword>] range.
	PROPERTY_HINT_ENUM, ///< hint_text= "val1,val2,val3,etc"
	PROPERTY_HINT_ENUM_SUGGESTION, ///< hint_text= "val1,val2,val3,etc"
	PROPERTY_HINT_EXP_EASING, /// exponential easing function (Math::ease) use "attenuation" hint string to revert (flip h), "positive_only" to exclude in-out and out-in. (ie: "attenuation,positive_only")
	PROPERTY_HINT_LINK,
	PROPERTY_HINT_FLAGS, ///< hint_text= "flag1,flag2,etc" (as bit flags)
	PROPERTY_HINT_LAYERS_2D_RENDER,
	PROPERTY_HINT_LAYERS_2D_PHYSICS,
	PROPERTY_HINT_LAYERS_2D_NAVIGATION,
	PROPERTY_HINT_LAYERS_3D_RENDER,
	PROPERTY_HINT_LAYERS_3D_PHYSICS,
	PROPERTY_HINT_LAYERS_3D_NAVIGATION,
	PROPERTY_HINT_FILE, ///< a file path must be passed, hint_text (optionally) is a filter "*.png,*.wav,*.doc,"
	PROPERTY_HINT_DIR, ///< a directory path must be passed
	PROPERTY_HINT_GLOBAL_FILE, ///< a file path must be passed, hint_text (optionally) is a filter "*.png,*.wav,*.doc,"
	PROPERTY_HINT_GLOBAL_DIR, ///< a directory path must be passed
	PROPERTY_HINT_RESOURCE_TYPE, ///< a resource object type
	PROPERTY_HINT_MULTILINE_TEXT, ///< used for string properties that can contain multiple lines
	PROPERTY_HINT_EXPRESSION, ///< used for string properties that can contain multiple lines
	PROPERTY_HINT_PLACEHOLDER_TEXT, ///< used to set a placeholder text for string properties
	PROPERTY_HINT_COLOR_NO_ALPHA, ///< used for ignoring alpha component when editing a color
	PROPERTY_HINT_OBJECT_ID,
	PROPERTY_HINT_TYPE_STRING, ///< a type string, the hint is the base type to choose
	PROPERTY_HINT_NODE_PATH_TO_EDITED_NODE, ///< so something else can provide this (used in scripts)
	PROPERTY_HINT_OBJECT_TOO_BIG, ///< object is too big to send
	PROPERTY_HINT_NODE_PATH_VALID_TYPES,
	PROPERTY_HINT_SAVE_FILE, ///< a file path must be passed, hint_text (optionally) is a filter "*.png,*.wav,*.doc,". This opens a save dialog
	PROPERTY_HINT_GLOBAL_SAVE_FILE, ///< a file path must be passed, hint_text (optionally) is a filter "*.png,*.wav,*.doc,". This opens a save dialog
	PROPERTY_HINT_INT_IS_OBJECTID,
	PROPERTY_HINT_INT_IS_POINTER,
	PROPERTY_HINT_ARRAY_TYPE,
	PROPERTY_HINT_LOCALE_ID,
	PROPERTY_HINT_LOCALIZABLE_STRING,
	PROPERTY_HINT_NODE_TYPE, ///< a node object type
	PROPERTY_HINT_HIDE_QUATERNION_EDIT, /// Only Node3D::transform should hide the quaternion editor.
	PROPERTY_HINT_PASSWORD,
	PROPERTY_HINT_MAX,
}
