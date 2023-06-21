#define TOOLS_ENABLED

namespace Godot.Net.Scene.Main;

public partial class SceneTree
{
    [Flags]
    public enum GroupCallFlags
    {
        GROUP_CALL_DEFAULT = 0,
        GROUP_CALL_REVERSE = 1,
        GROUP_CALL_DEFERRED = 2,
        GROUP_CALL_UNIQUE = 4,
    }
}
