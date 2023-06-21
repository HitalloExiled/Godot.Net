namespace Godot.Net.Core.Input;
#pragma warning disable IDE0021, CA1822, IDE0060, CS0649 // TODO - REMOVE

public partial class Input
{
    public record JoyBinding
    {
        public record InputBinding
        {
            public JoyAxis      Axis    { get; set; }
            public JoyButton    Button  { get; set; }
            public HatDir       Hat     { get; set; }
            public HatMask      HatMask { get; set; }
            public bool         Invert  { get; set; }
            public JoyAxisRange Range   { get; set; }
            public JoyType      Type    { get; set; }
        }

        public record OutputBinding
        {
            public JoyAxis      Axis   { get; set; }
            public JoyButton    Button { get; set; }
            public JoyAxisRange Range  { get; set; }
            public JoyType      Type   { get; set; }
        }

        public InputBinding  Input  { get; } = new();
        public OutputBinding Output { get; } = new();
    }
}
