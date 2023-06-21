#define TOOLS_ENABLED

namespace Godot.Net.Scene.Main;
public partial class SceneTree
{
    public record UGCall
    {
        public string Call  { get; set; } = "";
        public string Group { get; set; } = "";

        public static uint Hash(UGCall val) => (uint)(val.Group.GetHashCode() ^ val.Call.GetHashCode());

        public static bool operator <(UGCall left, UGCall right) =>
            (left.Group == right.Group ? left.Call.CompareTo(right.Call) : left.Group.CompareTo(right.Group)) == -1;

        public static bool operator >(UGCall left, UGCall right) =>
            (left.Group == right.Group ? left.Call.CompareTo(right.Call) : left.Group.CompareTo(right.Group)) == 1;
    }

}
