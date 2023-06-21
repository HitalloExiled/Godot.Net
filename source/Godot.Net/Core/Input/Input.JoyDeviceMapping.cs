namespace Godot.Net.Core.Input;
#pragma warning disable IDE0021, CA1822, IDE0060, CS0649 // TODO - REMOVE

public partial class Input
{
    private record JoyDeviceMapping
    {
        public List<JoyBinding> Bindings { get; } = new();

        public required string Name { get; set; }
        public required string Uid  { get; set; }
    };
}
