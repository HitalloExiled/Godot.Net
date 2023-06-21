namespace Godot.Net.Core;

public class SafeFlags
{
    private readonly object locker = new();

    private bool value;

    public bool Value
    {
        get => this.value;
        set
        {
            lock(this.locker)
            {
                this.value = value;
            }
        }
    }
}
