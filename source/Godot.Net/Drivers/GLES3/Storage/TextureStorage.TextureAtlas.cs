namespace Godot.Net.Drivers.GLES3.Storage;

using System.Collections.Generic;
using Godot.Net.Core.Math;

#pragma warning disable IDE0044 // TODO Remove

public partial class TextureStorage
{
    private record TextureAtlas
    {
        public record TextureRecord
        {
            public int          Users { get; set; }
            public Rect2<RealT> UvRect { get; set; }
        }

        public record SortItem
        {
            public Vector2<int> PixelSize { get; set; }
            public Vector2<int> Pos       { get; set; }
            public Vector2<int> Size      { get; set; }
            public Guid         Texture   { get; set; }

            public static bool operator <(SortItem left, SortItem right) =>
                //sort larger to smaller
                left.Size.Y == right.Size.Y ?
                    left.Size.X < right.Size.X :
                    left.Size.Y < right.Size.Y;

            public static bool operator >(SortItem left, SortItem right) =>
                //sort larger to smaller
                left.Size.Y == right.Size.Y ?
                    left.Size.X > right.Size.X :
                    left.Size.Y > right.Size.Y;
        }

        public bool                            Dirty       { get; set; } = true;
        public uint                            Framebuffer { get; set; }
        public Vector2<int>                    Size        { get; set; }
        public uint                            Texture     { get; set; }
        public Dictionary<Guid, TextureRecord> Textures    { get; set; } = new();
    }
}
