namespace Godot.Net.Servers.Rendering;

public abstract partial class RenderingDevice
{
    public enum FinalAction
    {
        FINAL_ACTION_READ, //will no longer render to it, allows attached textures to be read again, but depth buffer contents will be dropped (Can't be read from)
        FINAL_ACTION_DISCARD, // discard contents after rendering
        FINAL_ACTION_CONTINUE, //will continue rendering later, attached textures can't be read until re-bound with "finish"
        FINAL_ACTION_MAX
    }
}
