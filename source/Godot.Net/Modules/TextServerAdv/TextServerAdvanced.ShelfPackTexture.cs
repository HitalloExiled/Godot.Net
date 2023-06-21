namespace Godot.Net.Modules.TextServerAdv;

using System;
using System.Collections.Generic;
using Godot.Net.Core.IO;
using Godot.Net.Scene.Resources;

public partial class TextServerAdvanced
{
    private record ShelfPackTexture
    {
		public List<Shelf> Shelves { get; } = new();

		public bool          Dirty    { get; set; } = true;
		public ImageFormat   Format   { get; set; }
		public byte[]        Imgdata  { get; set; } = Array.Empty<byte>();
		public ImageTexture? Texture  { get; set; }
		public int           TextureH { get; set; } = 1024;
		public int           TextureW { get; set; } = 1024;

		public ShelfPackTexture() { }
		public ShelfPackTexture(int w, int h)
        {
            this.TextureW = w;
            this.TextureH = h;
        }

        public FontTexturePosition PackRect(int id, int h, int w)
        {
            var y = 0;
            Shelf? bestShelf = null;
            var bestWaste = int.MaxValue;

            foreach (var shelf in this.Shelves)
            {
                y += shelf.H;

                if (w > shelf.W)
                {
                    continue;
                }

                if (h == shelf.H)
                {
                    return shelf.AllocShelf(id, w, h);
                }

                if (h > shelf.H)
                {
                    continue;
                }

                if (h < shelf.H)
                {
                    var waste = (shelf.H - h) * w;

                    if (waste < bestWaste)
                    {
                        bestWaste = waste;
                        bestShelf = shelf;
                    }
                }
            }

            if (bestShelf != null)
            {
                return bestShelf.AllocShelf(id, w, h);
            }

            if (h <= this.TextureH - y && w <= this.TextureW)
            {
                var shelf = new Shelf(0, y, this.TextureW, h);
                this.Shelves.Add(shelf);
                return shelf.AllocShelf(id, w, h);
            }

            return new FontTexturePosition(-1, 0, 0);
        }
	}
}
