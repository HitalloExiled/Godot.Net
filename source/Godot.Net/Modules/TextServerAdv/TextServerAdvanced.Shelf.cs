namespace Godot.Net.Modules.TextServerAdv;

public partial class TextServerAdvanced
{
    private record Shelf
    {
		public int X { get; set; }
		public int Y { get; set; }
		public int W { get; set; }
		public int H { get; set; }

		public Shelf() { }

		public Shelf(int x, int y, int w, int h)
        {
            this.X = x;
            this.Y = y;
            this.W = w;
            this.H = h;
        }

        public FontTexturePosition AllocShelf(int id, int w, int h)
        {
            if (w > this.W || h > this.H)
            {
                return new FontTexturePosition(-1, 0, 0);
            }

            var xx = this.X;

            this.X += w;
            this.W -= w;

            return new FontTexturePosition(id, xx, this.Y);
        }
	}
}
