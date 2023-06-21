namespace Godot.Net.Servers;

using Godot.Net.Core.Math;

public abstract partial class TextServer
{
    public record CaretInfo
    {
        public Rect2<RealT> LCaret { get; set; }
        public Direction    LDir   { get; set; }
        public Rect2<RealT> TCaret { get; set; }
        public Direction    TDir   { get; set; }
    }
}
