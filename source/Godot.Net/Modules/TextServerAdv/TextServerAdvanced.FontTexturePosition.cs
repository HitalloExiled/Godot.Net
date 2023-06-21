namespace Godot.Net.Modules.TextServerAdv;

public partial class TextServerAdvanced
{
    private record FontTexturePosition
    {
		public int Index { get; set; } = -1;
		public int X     { get; set; }
		public int Y     { get; set; }

		public FontTexturePosition() { }

		public FontTexturePosition(int id, int x, int y)
        {
            this.Index = id;
            this.X     = x;
            this.Y     = y;
        }
	};
}
