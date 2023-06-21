namespace Godot.Net.Platforms.Windows;

public partial class DisplayServerWindows
{
    public struct PACKET
    {
        public int         pkStatus;
        public int         pkNormalPressure;
        public int         pkTangentPressure;
        public ORIENTATION pkOrientation;
    }
}
