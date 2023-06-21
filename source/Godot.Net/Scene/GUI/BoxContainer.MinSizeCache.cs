namespace Godot.Net.Scene.GUI;
public partial class BoxContainer
{
    private record MinSizeCache
    {
        public int  FinalSize   { get; set; }
        public int  MinSize     { get; set; }
        public bool WillStretch { get; set; }
    }
}
