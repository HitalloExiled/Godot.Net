namespace Godot.Net.Core.Math;

using System.Runtime.CompilerServices;

#pragma warning disable IDE0044, CS0649 // TODO - Remove

public record BVHVolume
{
    public Vector3<RealT> Min { get; set; }
    public Vector3<RealT> Max { get; set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public bool Contains(BVHVolume volume) =>
        this.Min.X <= volume.Min.X &&
        this.Min.Y <= volume.Min.Y &&
        this.Min.Z <= volume.Min.Z &&
        this.Max.X >= volume.Max.X &&
        this.Max.Y >= volume.Max.Y &&
        this.Max.Z >= volume.Max.Z;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public BVHVolume Merge(BVHVolume b)
    {
        var r = new BVHVolume();

        for (var i = 0; i < 3; ++i)
        {
            var min = r.Min;
            var max = r.Max;

            min[i] = this.Min[i] < b.Min[i] ? this.Min[i] : b.Min[i];

            max[i] = this.Max[i] > b.Max[i] ? this.Max[i] : b.Max[i];

            r.Min = min;
            r.Max = max;
        }
        return r;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public RealT GetProximityTo(BVHVolume volume)
    {
        var d = this.Min + this.Max - (volume.Min + volume.Max);

        return System.Math.Abs(d.X) + System.Math.Abs(d.Y) + System.Math.Abs(d.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public int SelectByProximity(BVHVolume a, BVHVolume b) =>
        this.GetProximityTo(a) < this.GetProximityTo(b) ? 0 : 1;
}

public class BVHNode
{
    public BVHNode[] Childs        { get; } = new BVHNode[2];
    public int       IndexInParent => this.Parent != null ? Array.IndexOf(this.Parent.Childs, this) : -1;
    public bool      IsInternal    => !this.IsLeaf;
    public bool      IsLeaf        => this.Childs.Length == 0;
    public BVHNode?  Parent        { get; set; }
    public BVHVolume Volume        { get; set; } = new();
}

public class DynamicBVH
{
    private BVHNode? bvhRoot;
    private uint     index;
    private uint     opath;
    private int      totalLeaves;

    public uint Index { get => this.index; set => this.SetIndex(value); }

    private void SetIndex(uint value)
    {
        if (ERR_FAIL_COND(this.bvhRoot != null))
        {
            return;
        }

        this.index = value;
    }

    private void InsertLeaf(BVHNode root, BVHNode leaf)
    {
        if (this.bvhRoot != null)
        {
            this.bvhRoot = leaf;
            leaf.Parent = null;
        }
        else
        {
            if (!root.IsLeaf)
            {
                do
                {
                    root = root.Childs[leaf.Volume.SelectByProximity(root.Childs[0].Volume, root.Childs[1].Volume)];
                } while (!root.IsLeaf);
            }

            var prev = root.Parent!;
            var node = new BVHNode
            {
                Parent = prev,
                Volume = leaf.Volume.Merge(root.Volume)
            };

            if (prev != null)
            {
                prev.Childs[root.IndexInParent] = node;
                node.Childs[0]                  = root;
                root.Parent                     = node;
                node.Childs[1]                  = leaf;
                leaf.Parent                     = node;
                do
                {
                    if (!prev.Volume.Contains(node.Volume))
                    {
                        prev.Volume = prev.Childs[0].Volume.Merge(prev.Childs[1].Volume);
                    }
                    else
                    {
                        break;
                    }

                    node = prev;
                }
                while (null != (prev = node.Parent));
            }
            else
            {
                node.Childs[0] = root;
                root.Parent    = node;
                node.Childs[1] = leaf;
                leaf.Parent    = node;

                this.bvhRoot   = node;
            }
        }
    }

    private BVHNode NodeSort(BVHNode n)
    {
        var p = n.Parent;

        if (p != null)
        {
            var i = n.IndexInParent;
            var j = 1 - i;
            var s = p.Childs[j];
            var q = p.Parent;

            if (q != null)
            {
                q.Childs[p.IndexInParent] = n;
            }
            else
            {
                this.bvhRoot = n;
            }

            s.Parent = n;
            p.Parent = n;
            n.Parent = q;
            p.Childs[0] = n.Childs[0];
            p.Childs[1] = n.Childs[1];
            n.Childs[0].Parent = p;
            n.Childs[1].Parent = s;

            (n.Volume, p.Volume) = (p.Volume, n.Volume);

            return p;
        }

        return n;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]

    private BVHNode? RemoveLeaf(BVHNode leaf)
    {
        if (leaf == this.bvhRoot)
        {
            this.bvhRoot = null;

            return null;
        }
        else
        {
            var parent  = leaf.Parent!;
            var prev    = parent.Parent;
            var sibling = parent.Childs[1 - leaf.IndexInParent];

            if (prev != null)
            {
                prev.Childs[parent.IndexInParent] = sibling;
                sibling.Parent = prev;

                while (prev != null)
                {
                    var pb = prev.Volume;

                    prev.Volume = prev.Childs[0].Volume.Merge(prev.Childs[1].Volume);

                    if (pb != prev.Volume)
                    {
                        prev = prev.Parent;
                    }
                    else
                    {
                        break;
                    }
                }

                return prev ?? this.bvhRoot;
            }
            else
            {
                this.bvhRoot = sibling;

                sibling.Parent = null;

                return this.bvhRoot;
            }
        }

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]

    private void Update(BVHNode leaf, int lookahead = 0)
    {
        var root = this.RemoveLeaf(leaf);

        if (root != null)
        {
            if (lookahead >= 0)
            {
                for (var i = 0; i < lookahead && root.Parent != null; ++i)
                {
                    root = root.Parent;
                }
            }
            else
            {
                root = this.bvhRoot;
            }
        }

        this.InsertLeaf(root!, leaf);
    }

    public void OptimizeIncremental(int passes)
    {
        if (passes < 0)
        {
            passes = this.totalLeaves;
        }

        if (passes > 0)
        {
            do
            {
                if (this.bvhRoot == null)
                {
                    break;
                }

                var node = this.bvhRoot;

                var bit = 0u;

                while (node.IsInternal)
                {
                    node = this.NodeSort(node).Childs[((int)this.opath >> (int)bit) & 1];
                    bit = (bit + 1) & (sizeof(uint) * 8 - 1);
                }

                this.Update(node);
                this.opath++;

            } while (--passes > 0);
        }
    }
}
