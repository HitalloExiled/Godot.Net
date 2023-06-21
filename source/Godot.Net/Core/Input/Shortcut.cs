namespace Godot.Net.Core.Input;

using Godot.Net.Core.IO;

public class Shortcut : Resource
{
    private InputEvent[] events = Array.Empty<InputEvent>();
    public InputEvent[] Events
    {
        get => this.events;
        set
        {
            for (var i = 0; i < value.Length; i++)
            {
                var ies = value[i] as InputEventShortcut;

                if (ERR_FAIL_COND_MSG(ies == null, "Cannot set a shortcut event to an instance of InputEventShortcut."))
                {
                    return;
                }
            }

            this.events = value;
            this.EmitChanged();
        }
    }

    public static bool IsEventArrayEqual(IEnumerable<InputEvent> left, IEnumerable<InputEvent> right) => throw new NotImplementedException();

	public object GetMeta(string name, object? @default = null) => throw new NotImplementedException();
	public void GetMetaList(out List<string> list) => throw new NotImplementedException();
    public bool HasMeta(string name) => throw new NotImplementedException();
    public void RemoveMeta(string name) => throw new NotImplementedException();
	public void SetMeta(string name, object value) => throw new NotImplementedException();
}
