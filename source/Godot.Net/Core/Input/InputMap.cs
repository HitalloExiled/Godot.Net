namespace Godot.Net.Core.Input;

#pragma warning disable CS0649 // TODO Remove;

public class InputMap
{
    private static readonly InputMap? singleton;

    public static InputMap Singleton => singleton ?? throw new NullReferenceException();

    public InputMap() => throw new NotImplementedException();

	public void ActionAddEvent(string action, InputEvent @event) => throw new NotImplementedException();
	public void ActionEraseEvent(string action, InputEvent @event) => throw new NotImplementedException();
	public void ActionEraseEvents(string action) => throw new NotImplementedException();
	public float ActionGetDeadzone(string action) => throw new NotImplementedException();
	public List<InputEvent> ActionGetEvents(string action) => throw new NotImplementedException();
	public bool ActionHasEvent(string action, InputEvent @event) => throw new NotImplementedException();
	public void ActionSetDeadzone(string action, float deadzone) => throw new NotImplementedException();
	public void AddAction(string action, float deadzone = 0.5f) => throw new NotImplementedException();
	public void EraseAction(string action) => throw new NotImplementedException();
	public bool EventGetActionStatus(InputEvent @event, string action, bool exactMatch = false) => throw new NotImplementedException();
	public bool EventGetActionStatus(InputEvent @event, string action, bool exactMatch, out bool pressed) => throw new NotImplementedException();
	public bool EventGetActionStatus(InputEvent @event, string action, bool exactMatch, out bool pressed, out float strength) => throw new NotImplementedException();
	public bool EventGetActionStatus(InputEvent @event, string action, bool exactMatch, out bool pressed, out float strength, out float rawStrength) => throw new NotImplementedException();
	public bool EventIsAction(InputEvent @event, string action, bool exactMatch = false) => throw new NotImplementedException();
	public Dictionary<string, Action> GetActionMap() => throw new NotImplementedException();
	public List<string> GetActions() => throw new NotImplementedException();
	public string GetBuiltinDisplayName(string name) => throw new NotImplementedException();
	public Dictionary<string, List<InputEvent>> GetBuiltins() => throw new NotImplementedException();
	public Dictionary<string, List<InputEvent>> GetBuiltinsWithFeatureOverridesApplied() => throw new NotImplementedException();
	public bool HasAction(string action) => throw new NotImplementedException();
	public void LoadDefault() => throw new NotImplementedException();
	public void LoadFromProjectSettings() => throw new NotImplementedException();
	public string SuggestActions(string action) => throw new NotImplementedException();
}
