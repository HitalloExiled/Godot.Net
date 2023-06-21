namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;

public partial class RendererCanvasCull
{
    public partial record Item : RendererCanvasRender.Item
    {
        public List<Item> ChildItems { get; } = new();

        public IEnumerable<Item>?      E                    { get; set; }
        public bool                    ChildrenOrderDirty   { get; set; }
        public int                     Index                { get; set; }
        public Command?                LastCommand          { get; set; }
        public Color                   Modulate             { get; set; }
        public Guid                    Parent               { get; set; } // canvas it belongs to
        public Color                   SelfModulate         { get; set; }
        public bool                    SortY                { get; set; }
        public bool                    UseParentMaterial    { get; set; }
        public uint                    VisibilityLayer      { get; set; } = 0xffffffff;
        public VisibilityNotifierData? VisibilityNotifier   { get; set; }
        public int                     YsortChildrenCount   { get; set; }
        public int                     YsortIndex           { get; set; }
        public Color                   YsortModulate        { get; set; }
        public int                     YsortParentAbsZIndex { get; set; } // Absolute Z index of parent. Only populated and used when y-sorting.
        public Vector2<RealT>          YsortPos             { get; set; }
        public Transform2D<RealT>      YsortXform           { get; set; } = new();
        public int                     ZIndex               { get; set; }
        public bool                    ZRelative            { get; set; }

        public override Rect2<RealT> Rect
        {
            get
            {
                if (this.CustomRect || !this.RectDirty && !this.UpdateWhenVisible && this.Skeleton == default)
                {
                    return base.Rect;
                }

                //must update rect

                if (this.Commands == null)
                {
                    base.Rect      = default;
                    this.RectDirty = false;

                    return base.Rect;
                }

                var xf         = new Transform2D<RealT>();
                var foundXform = false;
                var first      = true;

                var c = this.Commands;

                while (c != null)
                {
                    var r = new Rect2<RealT>();

                    switch (c.Type)
                    {
                        case CommandType.TYPE_RECT:
                            r = ((CommandRect)c).Rect;
                            break;
                        case CommandType.TYPE_NINEPATCH:
                            r = ((CommandNinePatch)c).Rect;
                            break;
                        case CommandType.TYPE_POLYGON:
                            r = ((CommandPolygon)c).Polygon!.RectCache;
                            break;
                        case CommandType.TYPE_PRIMITIVE:
                            {
                                var primitive = (CommandPrimitive)c;

                                for (var j = 0; j < primitive.PointCount; j++)
                                {
                                    if (j == 0)
                                    {
                                        r.Position = primitive.Points[0];
                                    }
                                    else
                                    {
                                        r.ExpandTo(primitive.Points[j]);
                                    }
                                }
                            }
                            break;
                        #region TODO
                        // case CommandType.TYPE_MESH:
                        //     {
                        //         var aabb = RSG.MeshStorage.MeshGetAabb(((CommandMesh)c).Mesh, this.Skeleton);

                        //         r = new(aabb.position.x, aabb.position.y, aabb.size.x, aabb.size.y);
                        //     }
                        //     break;
                        // case CommandType.TYPE_MULTIMESH:
                        //     {
                        //         var aabb = RSG.MeshStorage.MultimeshGetAabb(((CommandMultiMesh)c).Multimesh);

                        //         r = new(aabb.position.x, aabb.position.y, aabb.size.x, aabb.size.y);

                        //     }
                        //     break;
                        // case CommandType.TYPE_PARTICLES:
                        //     {
                        //         var particlesCmd = ((CommandParticles)c);
                        //         if (particlesCmd.Particles != null)
                        //         {
                        //             var aabb = RSG.ParticlesStorage->ParticlesGetAabb(particlesCmd.particles);
                        //             r = new(aabb.position.x, aabb.position.y, aabb.size.x, aabb.size.y);
                        //         }
                        //     }
                        //     break;
                        // case CommandType.TYPE_TRANSFORM:
                        //     xf = ((CommandTransform)c).Xform;
                        //     foundXform = true;
                        #endregion TODO
                        default:
                            c = c.Next;
                            continue;
                    }

                    if (foundXform)
                    {
                        r = xf.Xform(r);
                    }

                    if (first)
                    {
                        base.Rect = r;
                        first = false;
                    }
                    else
                    {
                        base.Rect = base.Rect.Merge(r);
                    }
                    c = c.Next;
                }

                this.RectDirty = false;
                return base.Rect;
            }
        }

        public Item()
        {
            this.ChildrenOrderDirty   = true;
            this.E                    = null;
            this.ZIndex               = 0;
            this.Modulate             = new(1, 1, 1, 1);
            this.SelfModulate         = new(1, 1, 1, 1);
            this.SortY                = false;
            this.UseParentMaterial    = false;
            this.ZRelative            = true;
            this.Index                = 0;
            this.YsortChildrenCount   = -1;
            this.YsortXform           = new();
            this.YsortPos             = new();
            this.YsortIndex           = 0;
            this.YsortParentAbsZIndex = 0;
        }

        public T AllocCommand<T>() where T : Command, new()
        {
            T? command;
            if (this.Commands == null)
            {
                command = new()
                {
                    Next = null
                };

                this.Commands = command;
                this.LastCommand = command;
            }
            else
            {
                command = new();
                this.LastCommand!.Next = command;
                this.LastCommand = command;
            }

            this.RectDirty = true;

            return command;
        }

        public static bool operator <(Item left, Item right) =>
            left.Index < right.Index;

        public static bool operator >(Item left, Item right) =>
            left.Index > right.Index;
    }
}
