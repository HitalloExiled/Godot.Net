namespace Godot.Net.Scene.Main;

public partial class Node
{
    public enum ProcessModeKind
    {
        PROCESS_MODE_INHERIT, // same as parent node
        PROCESS_MODE_PAUSABLE, // process only if not paused
        PROCESS_MODE_WHEN_PAUSED, // process only if paused
        PROCESS_MODE_ALWAYS, // process always
        PROCESS_MODE_DISABLED, // never process
    };
}
