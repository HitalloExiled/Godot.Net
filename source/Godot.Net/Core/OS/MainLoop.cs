namespace Godot.Net.Core.OS;

using Godot.Net.Core.Object;
using Godot.Net.Scene.Main;

public partial class MainLoop : GodotObject
{
    // Finalize is Reserved and it is not a dispose
    public virtual void Complete() => throw new NotImplementedException();

    public virtual void Initialize()
    {
        #region TODO
        // if (initialize_script.is_valid()) {
        //     set_script(initialize_script);
        // }

        // GDVIRTUAL_CALL(_initialize);
        #endregion TODO
    }

    public virtual void Notification(NotificationKind notificationType)
    {
        // TODO
    }

    public virtual bool Process(double time)
    {
        var quit = false;
        // TODO GDVIRTUAL_CALL(_process, p_time, quit);
        return quit;
    }
}
