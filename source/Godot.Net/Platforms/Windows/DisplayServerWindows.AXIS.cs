namespace Godot.Net.Platforms.Windows;

public partial class DisplayServerWindows
{
    public struct AXIS
    {
        public long axMin;
        public long axMax;
        public uint axUnits;
        public nint axResolution;
    }
}
