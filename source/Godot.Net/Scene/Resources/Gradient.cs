namespace Godot.Net.Scene.Resources;

using Godot.Net.Core.IO;
using Godot.Net.Core.Math;

#pragma warning disable IDE0052, CS0414 // TODO Remove

public partial class Gradient : Resource
{
    private readonly List<Point> points = new();

    private bool isSorted;

    public Color[] Colors
    {
        get => this.points.Select(x => x.Color).ToArray();
        set
        {
            if (this.points.Count < value.Length)
            {
                this.isSorted = false;
            }

            this.points.Capacity = value.Length;

            for (var i = 0; i < value.Length; i++)
            {
                if (i >= this.points.Count)
                {
                    this.points.Add(new() { Color = value[i] });
                }
                else
                {
                    this.points[i].Color = value[i];
                }
            }

            this.EmitChanged();
        }
    }

    public float[] Offsets
    {
        get => this.points.Select(x => x.Offset).ToArray();
        set
        {
            this.points.Capacity = value.Length;

            for (var i = 0; i < value.Length; i++)
            {
                if (i >= this.points.Count)
                {
                    this.points.Add(new() { Offset = value[i] });
                }
                else
                {
                    this.points[i].Offset = value[i];
                }
            }

            this.isSorted = false;
            this.EmitChanged();
        }
    }
}
