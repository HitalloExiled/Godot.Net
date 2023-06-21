namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core;
using Godot.Net.Core.Math;

#pragma warning disable IDE0044,CS0649 // TODO Remove

public partial class RendererCanvasCull
{
    private const int Z_RANGE = RS.CANVAS_ITEM_Z_MAX - RS.CANVAS_ITEM_Z_MIN + 1;

    private readonly GuidOwner<Item> canvasItemOwner = new();

    private bool                                       snapping2DTransformsToPixel;
    private RendererCanvasRender.Item[]                zLastList = Array.Empty<RendererCanvasRender.Item>();
    private RendererCanvasRender.Item[]                zList     = Array.Empty<RendererCanvasRender.Item>();
    private SelfList<Item.VisibilityNotifierData>.List visibilityNotifierList = new();

    public GuidOwner<Canvas> CanvasOwner  { get; } = new();
    public bool              DisableScale { get; }
    public bool              WasSdfUsed   { get; private set; }

    #region private static methods
    private static void CollectYsortChildren(
        Item               canvasItem,
        Transform2D<RealT> transform,
        Item               materialOwner,
        List<Item>?        items,
        int                index,
        int                z
    )
    {
        var childItems = canvasItem.ChildItems;

        foreach (var childItem in childItems)
        {
            var absZ = 0;
            if (childItem.Visible)
            {
                if (items != null)
                {
                    items[index] = childItem;
                    childItem.YsortXform           = transform;
                    childItem.YsortPos             = transform.Xform(childItem.Xform[2]);
                    childItem.MaterialOwner        = childItem.UseParentMaterial ? materialOwner : null;
                    childItem.YsortIndex           = index;
                    childItem.YsortParentAbsZIndex = z;

                    // Y sorted canvas items are flattened into r_items. Calculate their absolute z index to use when rendering r_items.
                    absZ = childItem.ZRelative ? Math.Clamp(z + childItem.ZIndex, RS.CANVAS_ITEM_Z_MIN, RS.CANVAS_ITEM_Z_MAX) : childItem.ZIndex;
                }

                index++;

                if (childItem.SortY)
                {
                    CollectYsortChildren(
                        childItem,
                        transform * childItem.Xform,
                        childItem.UseParentMaterial ? materialOwner : childItem,
                        items,
                        index,
                        absZ
                    );
                }
            }
        }
    }

    private static int Compare(Item left, Item right) =>
        left < right ? -1 : left > right ? 1 : 0;

    private static int Compare(Canvas.ChildItem left, Canvas.ChildItem right) =>
        left < right ? -1 : left > right ? 1 : 0;

    private static void MarkYsortDirty(Item ysortOwner, GuidOwner<Item> canvasItemOwner)
    {
        do
        {
            ysortOwner.YsortChildrenCount = -1;
            ysortOwner = canvasItemOwner.Owns(ysortOwner.Parent) ? canvasItemOwner.GetOrNull(ysortOwner.Parent)! : null!;
        } while (ysortOwner != null && ysortOwner.SortY);
    }
    #endregion private static methods

    private void AttachCanvasItemForDraw(
        Item                        ci,
        Item?                       canvasClip,
        RendererCanvasRender.Item[] rZList,
        RendererCanvasRender.Item[] rZLastList,
        Transform2D<RealT>          xform,
        Rect2<RealT>                clipRect,
        Rect2<RealT>                globalRect,
        Color                       modulate,
        int                         z,
        Item                        materialOwner,
        bool                        useCanvasGroup,
        RendererCanvasRender.Item?  canvasGroupFrom,
        Transform2D<RealT>          xform2
    )
    {
        _ = canvasClip;
        _ = materialOwner;
        _ = xform2;

        if (ci.CopyBackBuffer != null)
        {
            ci.CopyBackBuffer.ScreenRect = xform.Xform(ci.CopyBackBuffer.Rect).Intersection(clipRect);
        }

        if (useCanvasGroup)
        {
            var zidx = z - RS.CANVAS_ITEM_Z_MIN;
            if (canvasGroupFrom == null)
            {
                // no list before processing this item, means must put stuff in group from the beginning of list.
                canvasGroupFrom = rZList[zidx];
            }
            else
            {
                // there was a list before processing, so begin group from this one.
                canvasGroupFrom = canvasGroupFrom.Next;
            }

            if (canvasGroupFrom != null)
            {
                // Has a place to begin the group from!

                //compute a global rect (in global coords) for children in the same z layer
                var rectAccum = new Rect2<RealT>();
                var c = canvasGroupFrom;

                while (c != null)
                {
                    rectAccum = c == canvasGroupFrom ? c.GlobalRectCache : rectAccum.Merge(c.GlobalRectCache);

                    c = c.Next;
                }

                // We have two choices now, if user has drawn something, we must assume users wants to draw the "mask", so compute the size based on this.
                // If nothing has been drawn, we just take it over and draw it ourselves.
                if (ci.CanvasGroup!.FitEmpty && (ci.Commands == null || ci.Commands.Next == null && ci.Commands.Type == RendererCanvasRender.Item.CommandType.TYPE_RECT && ((RendererCanvasRender.Item.CommandRect)ci.Commands).Flags.HasFlag(RendererCanvasRender.CanvasRectFlags.CANVAS_RECT_IS_GROUP)))
                {
                    // No commands, or sole command is the one used to draw, so we (re)create the draw command.
                    ci.Clear();

                    if (rectAccum == default)
                    {
                        rectAccum.Size = new(1, 1);
                    }

                    rectAccum = rectAccum.Grow(ci.CanvasGroup.FitMargin);

                    //draw it?
                    var crect = ci.AllocCommand<RendererCanvasRender.Item.CommandRect>();

                    crect.Flags = RendererCanvasRender.CanvasRectFlags.CANVAS_RECT_IS_GROUP; // so we can recognize it later
                    crect.Rect = xform.AffineInverse().Xform(rectAccum);
                    crect.Modulate = new(1, 1, 1, 1);

                    //the global rect is used to do the copying, so update it
                    globalRect = rectAccum.Grow(ci.CanvasGroup.ClearMargin); //grow again by clear margin
                    globalRect.Position += clipRect.Position;
                }
                else
                {
                    globalRect.Position -= clipRect.Position;

                    globalRect = globalRect.Merge(rectAccum); //must use both rects for this
                    globalRect = globalRect.Grow(ci.CanvasGroup.ClearMargin); //grow by clear margin

                    globalRect.Position += clipRect.Position;
                }

                // Very important that this is cleared after used in RendererCanvasRender to avoid
                // potential crashes.
                canvasGroupFrom.CanvasGroupOwner = ci;
            }
        }

        if ((ci.Commands != null || ci.VisibilityNotifier != null) && clipRect.Intersects(globalRect, true) || ci.VpRender != null || ci.CopyBackBuffer != null)
        {
            //something to draw?

            if (ci.UpdateWhenVisible)
            {
                RenderingServerDefault.RedrawRequest();
            }

            if (ci.Commands != null || ci.CopyBackBuffer != null)
            {
                ci.FinalTransform = xform;
                ci.FinalModulate = modulate * ci.SelfModulate;
                ci.GlobalRectCache = globalRect;
                ci.GlobalRectCache = ci.GlobalRectCache with { Position = ci.GlobalRectCache.Position - clipRect.Position };
                ci.LightMasked = false;

                var zidx = z - RS.CANVAS_ITEM_Z_MIN;

                if (rZLastList[zidx] != null)
                {
                    rZLastList[zidx].Next = ci;
                    rZLastList[zidx] = ci;

                }
                else
                {
                    rZList[zidx] = ci;
                    rZLastList[zidx] = ci;
                }

                ci.ZFinal = z;

                ci.Next = null;
            }

            if (ci.VisibilityNotifier != null)
            {
                if (!ci.VisibilityNotifier.VisibleElement.InList)
                {
                    this.visibilityNotifierList.Add(ci.VisibilityNotifier.VisibleElement);
                    ci.VisibilityNotifier.JustVisible = true;
                }

                ci.VisibilityNotifier.VisibleInFrame = RSG.Rasterizer.FrameNumber;
            }
        }
    }


    private void CullCanvasItem(
        Item                        canvasItem,
        Transform2D<RealT>          transform,
        Rect2<RealT>                clipRect,
        Color                       modulateColor,
        int                         z,
        RendererCanvasRender.Item[] zList,
        RendererCanvasRender.Item[] zLastList,
        Item?                       canvasClip,
        Item?                       materialOwner,
        bool                        allowYSort,
        uint                        canvasCullMask
    )
    {
        var ci = canvasItem;

        if (!ci.Visible)
        {
            return;
        }

        if ((ci.VisibilityLayer & canvasCullMask) == 0)
        {
            return;
        }

        if (ci.ChildrenOrderDirty)
        {
            ci.ChildItems.Sort(Compare);
            ci.ChildrenOrderDirty = false;
        }

        var rect = ci.Rect;

        if (ci.VisibilityNotifier != null)
        {
            if (ci.VisibilityNotifier.Area.Size != default)
            {
                rect = rect.Merge(ci.VisibilityNotifier.Area);
            }
        }

        var xform = ci.Xform;

        if (this.snapping2DTransformsToPixel)
        {
            xform[2] = xform[2].Floor();
        }

        xform = transform * xform;

        var globalRect = xform.Xform(rect);
        globalRect.Position += clipRect.Position;

        if (ci.UseParentMaterial && materialOwner != null)
        {
            ci.MaterialOwner = materialOwner;
        }
        else
        {
            materialOwner = ci;
            ci.MaterialOwner = null;
        }

        var modulate = new Color(
            ci.Modulate.R * modulateColor.R,
            ci.Modulate.G * modulateColor.G,
            ci.Modulate.B * modulateColor.B,
            ci.Modulate.A * modulateColor.A
        );

        if (modulate.A < 0.007)
        {
            return;
        }

        var childItems = ci.ChildItems;

        if (ci.Clip)
        {
            ci.FinalClipRect = canvasClip != null ? canvasClip.FinalClipRect.Intersection(globalRect) : clipRect.Intersection(globalRect);
            if (ci.FinalClipRect.Size.X < 0.5 || ci.FinalClipRect.Size.Y < 0.5)
            {
                // The clip rect area is 0, so don't draw the item.
                return;
            }
            ci.FinalClipRect = new(ci.FinalClipRect.Position.Round(), ci.FinalClipRect.Size.Round());
            ci.FinalClipOwner = ci;

        }
        else
        {
            ci.FinalClipOwner = canvasClip;
        }

        var parentZ = z;
        z = ci.ZRelative ? Math.Clamp(z + ci.ZIndex, RS.CANVAS_ITEM_Z_MIN, RS.CANVAS_ITEM_Z_MAX) : ci.ZIndex;

        if (ci.SortY)
        {
            if (allowYSort)
            {
                if (ci.YsortChildrenCount == -1)
                {
                    ci.YsortChildrenCount = 0;
                    CollectYsortChildren(ci, new Transform2D<RealT>(), materialOwner, null, ci.YsortChildrenCount, z);
                }

                childItems = new(ci.YsortChildrenCount + 1);
                ci.YsortParentAbsZIndex = parentZ;
                childItems[0] = ci;

                var i = 1;

                CollectYsortChildren(ci, new Transform2D<RealT>(), materialOwner, childItems, i, z);

                ci.YsortXform = ci.Xform.AffineInverse();

                childItems.Sort();

                foreach (var childItem in childItems)
                {
                    this.CullCanvasItem(
                        childItem,
                        xform * childItem.YsortXform,
                        clipRect,
                        modulate,
                        childItem.YsortParentAbsZIndex,
                        zList,
                        zLastList,
                        ci.FinalClipOwner as Item,
                        childItem.MaterialOwner as Item,
                        false,
                        canvasCullMask
                    );
                }
            }
            else
            {
                var canvasGroupFrom = default(RendererCanvasRender.Item);
                var useCanvasGroup  = ci.CanvasGroup != null && (ci.CanvasGroup.FitEmpty || ci.Commands != null);
                if (useCanvasGroup)
                {
                    var zidx = z - RS.CANVAS_ITEM_Z_MIN;
                    canvasGroupFrom = zLastList![zidx];
                }

                this.AttachCanvasItemForDraw(
                    ci,
                    canvasClip,
                    zList,
                    zLastList,
                    xform,
                    clipRect,
                    globalRect,
                    modulate,
                    z,
                    materialOwner,
                    useCanvasGroup,
                    canvasGroupFrom,
                    xform
                );
            }
        }
        else
        {
            var canvasGroupFrom = default(RendererCanvasRender.Item);
            var useCanvasGroup = ci.CanvasGroup != null && (ci.CanvasGroup.FitEmpty || ci.Commands != null);
            if (useCanvasGroup)
            {
                var zidx = z - RS.CANVAS_ITEM_Z_MIN;
                canvasGroupFrom = zLastList[zidx];
            }

            foreach (var childItem in childItems)
            {
                if (!childItem.Behind && !useCanvasGroup)
                {
                    continue;
                }

                this.CullCanvasItem(
                    childItem,
                    xform,
                    clipRect,
                    modulate,
                    z,
                    zList,
                    zLastList,
                    ci.FinalClipOwner as Item,
                    materialOwner,
                    true,
                    canvasCullMask
                );
            }

            this.AttachCanvasItemForDraw(
                ci,
                canvasClip,
                zList,
                zLastList,
                xform,
                clipRect,
                globalRect,
                modulate,
                z,
                materialOwner,
                useCanvasGroup,
                canvasGroupFrom,
                xform
            );

            // for (var i = 0; i < child_item_count; i++)
            foreach (var childItem in childItems)
            {
                if (childItem.Behind || useCanvasGroup)
                {
                    continue;
                }

                this.CullCanvasItem(
                    childItem,
                    xform,
                    clipRect,
                    modulate,
                    z,
                    zList,
                    zLastList,
                    ci.FinalClipOwner as Item,
                    materialOwner,
                    true,
                    canvasCullMask
                );
            }
        }
    }



    private void RenderCanvasItemTree(
        Guid                        toRenderTarget,
        List<Canvas.ChildItem>?     childItems,
        Item?                       canvasItem,
        in Transform2D<float>       transform,
        in Rect2<float>             clipRect,
        in Color                    modulate,
        RendererCanvasRender.Light? lights,
        RendererCanvasRender.Light? directionalLights,
        RS.CanvasItemTextureFilter  defaultFilter,
        RS.CanvasItemTextureRepeat  defaultRepeat,
        bool                        snap2DVerticesToPixel,
        uint                        canvasCullMask
    )
    {
        RENDER_TIMESTAMP("Cull CanvasItem Tree");

        this.zList     = new RendererCanvasRender.Item[Z_RANGE];
        this.zLastList = new RendererCanvasRender.Item[Z_RANGE];

        if (childItems != null)
        {
            foreach (var childItem in childItems)
            {
                this.CullCanvasItem(
                    childItem.Item!,
                    transform,
                    clipRect,
                    new(1, 1, 1, 1),
                    0,
                    this.zList,
                    this.zLastList,
                    null,
                    null,
                    true,
                    canvasCullMask
                );
            }
        }

        if (canvasItem != null)
        {
            this.CullCanvasItem(
                canvasItem,
                transform,
                clipRect,
                new(1, 1, 1, 1),
                0,
                this.zList,
                this.zLastList,
                null,
                null,
                true,
                canvasCullMask
            );
        }

        var list    = default(RendererCanvasRender.Item);
        var listEnd = default(RendererCanvasRender.Item);

        for (var i = 0; i < Z_RANGE; i++)
        {
            if (this.zList[i] == null)
            {
                continue;
            }

            if (list == null)
            {
                list    = this.zList[i];
                listEnd = this.zLastList[i];
            }
            else
            {
                listEnd!.Next = this.zList[i];
                listEnd       = this.zLastList[i];
            }
        }

        RENDER_TIMESTAMP("Render CanvasItems");

        RSG.CanvasRender.CanvasRenderItems(
            toRenderTarget,
            list,
            modulate,
            lights,
            directionalLights,
            transform,
            defaultFilter,
            defaultRepeat,
            snap2DVerticesToPixel,
            out var sdfFlag
        );

        if (sdfFlag)
        {
            this.WasSdfUsed = true;
        }
    }

    #region public methods
    public void CanvasInitialize(Guid id) =>
        this.CanvasOwner.Initialize(id);

    public void CanvasItemAddTriangleArray(
        Guid                  item,
        IList<int>            indices,
        IList<Vector2<RealT>> points,
        IList<Color>          colors,
        IList<Vector2<RealT>> uvs,
        IList<int>            bones,
        IList<float>          weights,
        Guid                  texture,
        int                   _
    )
    {
        var canvasItem = this.canvasItemOwner.GetOrNull(item);

        if (ERR_FAIL_COND(canvasItem == null))
        {
            return;
        }

        var vertexCount = points.Count;

        if (ERR_FAIL_COND(vertexCount == 0))
        {
            return;
        }

        if (ERR_FAIL_COND(colors.Count != 0 && colors.Count != vertexCount && colors.Count != 1))
        {
            return;
        }

        if (ERR_FAIL_COND(uvs.Count != 0 && uvs.Count != vertexCount))
        {
            return;
        }

        if (ERR_FAIL_COND(bones.Count != 0 && bones.Count != vertexCount * 4))
        {
            return;
        }

        if (ERR_FAIL_COND(weights.Count != 0 && weights.Count != vertexCount * 4))
        {
            return;
        }

        var polygon = canvasItem!.AllocCommand<RendererCanvasRender.Item.CommandPolygon>();
        if (ERR_FAIL_COND(polygon == null))
        {
            return;
        }

        polygon!.Texture = texture;

        polygon.Polygon.Create(indices, points, colors, uvs, bones, weights);

        polygon.Primitive = RS.PrimitiveType.PRIMITIVE_TRIANGLES;
    }

    public void CanvasItemClear(Guid p_item)
    {
        var canvasItem = this.canvasItemOwner.GetOrNull(p_item);

        if (ERR_FAIL_COND(canvasItem == null))
        {
            return;
        }

        canvasItem!.Clear();
    }

    public void CanvasItemInitialize(Guid canvasItemId) =>
        this.canvasItemOwner.Initialize(canvasItemId);

    public void CanvasItemSetClip(Guid item, bool clip)
    {
        var canvasItem = this.canvasItemOwner.GetOrNull(item);

        if (ERR_FAIL_COND(canvasItem == null))
        {
            return;
        }

        canvasItem!.Clip = clip;
    }

    public void CanvasItemSetCustomRect(Guid item, bool customRect, in Rect2<RealT> rect)
    {
        var canvasItem = this.canvasItemOwner.GetOrNull(item);

        if (ERR_FAIL_COND(canvasItem == null))
        {
            return;
        }

        canvasItem!.CustomRect = customRect;
        canvasItem!.Rect       = rect;
    }

    public void CanvasItemSetDefaultTextureFilter(Guid canvasItemId, RS.CanvasItemTextureFilter textureFilter)
    {
        var canvasItem = this.canvasItemOwner.GetOrNull(canvasItemId);

        if (ERR_FAIL_COND(canvasItem == null))
        {
            return;
        }

        canvasItem!.TextureFilter = textureFilter;
    }

    public void CanvasItemSetDefaultTextureRepeat(Guid canvasItemId, RS.CanvasItemTextureRepeat textureRepeat)
    {
        var canvasItem = this.canvasItemOwner.GetOrNull(canvasItemId);

        if (ERR_FAIL_COND(canvasItem == null))
        {
            return;
        }

        canvasItem!.TextureRepeat = textureRepeat;
    }

    public void CanvasItemSetDrawIndex(Guid canvasItemId, int index)
    {
        var canvasItem = this.canvasItemOwner.GetOrNull(canvasItemId);

        if (ERR_FAIL_COND(canvasItem == null))
        {
            return;
        }

        canvasItem!.Index = index;

        if (this.canvasItemOwner.Owns(canvasItem.Parent))
        {
            var canvasItemParent = this.canvasItemOwner.GetOrNull(canvasItem.Parent)!;

            canvasItemParent.ChildrenOrderDirty = true;

            return;
        }

        var canvas = this.CanvasOwner.GetOrNull(canvasItem.Parent);

        if (canvas != null)
        {
            canvas.ChildrenOrderDirty = true;
            return;
        }
    }

    public void CanvasItemSetParent(Guid canvasItemId, Guid canvasItemParentId)
    {
        var canvasItem = this.canvasItemOwner.GetOrNull(canvasItemId);

        if (ERR_FAIL_COND(canvasItem == null))
        {
            return;
        }

        if (canvasItem!.Parent != default)
        {
            if (this.CanvasOwner.Owns(canvasItem.Parent))
            {
                var canvas = this.CanvasOwner.GetOrNull(canvasItem.Parent);
                canvas!.EraseItem(canvasItem);
            }
            else if (this.canvasItemOwner.Owns(canvasItem.Parent))
            {
                var itemOwner = this.canvasItemOwner.GetOrNull(canvasItem.Parent);
                itemOwner!.ChildItems.Remove(canvasItem);

                if (itemOwner.SortY)
                {
                    MarkYsortDirty(itemOwner, this.canvasItemOwner);
                }
            }

            canvasItem.Parent = default;
        }

        if (canvasItemParentId != default)
        {
            if (this.CanvasOwner.Owns(canvasItemParentId))
            {
                var canvas = this.CanvasOwner.GetOrNull(canvasItemParentId)!;
                var ci = new Canvas.ChildItem
                {
                    Item = canvasItem
                };
                canvas.ChildItems.Add(ci);
                canvas.ChildrenOrderDirty = true;
            }
            else if (this.canvasItemOwner.Owns(canvasItemParentId))
            {
                var itemOwner = this.canvasItemOwner.GetOrNull(canvasItemParentId)!;
                itemOwner.ChildItems.Add(canvasItem);
                itemOwner.ChildrenOrderDirty = true;

                if (itemOwner.SortY)
                {
                    MarkYsortDirty(itemOwner, this.canvasItemOwner);
                }

            }
            else
            {
                if (ERR_FAIL_MSG("Invalid parent."))
                {
                    return;
                }
            }
        }

        canvasItem.Parent = canvasItemParentId;
    }

    public void CanvasItemSetTransform(Guid item, in Transform2D<RealT> transform)
    {
        var canvasItem = this.canvasItemOwner.GetOrNull(item);

        if (ERR_FAIL_COND(canvasItem == null))
        {
            return;
        }

        canvasItem!.Xform = transform;
    }

    public void CanvasItemSetVisible(Guid item, bool visible)
    {
        var canvasItem = this.canvasItemOwner.GetOrNull(item);

        if (ERR_FAIL_COND(canvasItem == null))
        {
            return;
        }

        canvasItem!.Visible = visible;

        MarkYsortDirty(canvasItem, this.canvasItemOwner);
    }

    public void CanvasItemSetVisibilityLayer(Guid item, uint visibilityLayer)
    {
        var canvasItem = this.canvasItemOwner.GetOrNull(item);

        if (ERR_FAIL_COND(canvasItem == null))
        {
            return;
        }

        canvasItem!.VisibilityLayer = visibilityLayer;
    }



    public void RenderCanvas(
        Guid                        renderTarget,
        Canvas                      canvas,
        in Transform2D<RealT>       transform,
        RendererCanvasRender.Light? lights,
        RendererCanvasRender.Light? directionalLights,
        in Rect2<RealT>             clipRect,
        RS.CanvasItemTextureFilter  defaultFilter,
        RS.CanvasItemTextureRepeat  defaultRepeat,
        bool                        snap2DTransformsToPixel,
        bool                        snap2DVerticesToPixel,
        uint                        canvasCullMask
    )
    {
        RENDER_TIMESTAMP("> Render Canvas");

        this.WasSdfUsed = false;
        this.snapping2DTransformsToPixel = snap2DTransformsToPixel;

        if (canvas.ChildrenOrderDirty)
        {
            canvas.ChildItems.Sort(Compare);
            canvas.ChildrenOrderDirty = false;
        }

        var l  = canvas.ChildItems.Count;
        var ci = canvas.ChildItems;

        var hasMirror = false;
        for (var i = 0; i < l; i++)
        {
            if (ci[i].Mirror.X != 0 || ci[i].Mirror.Y != 0)
            {
                hasMirror = true;
                break;
            }
        }

        if (!hasMirror)
        {
            this.RenderCanvasItemTree(
                renderTarget,
                ci,
                null,
                transform,
                clipRect,
                canvas.Modulate,
                lights,
                directionalLights,
                defaultFilter,
                defaultRepeat,
                snap2DVerticesToPixel,
                canvasCullMask
            );

        }
        else
        {
            //used for parallaxlayer mirroring
            for (var i = 0; i < l; i++)
            {
                var ci2 = canvas.ChildItems[i];
                this.RenderCanvasItemTree(
                    renderTarget,
                    null,
                    ci2.Item,
                    transform,
                    clipRect,
                    canvas.Modulate,
                    lights,
                    directionalLights,
                    defaultFilter,
                    defaultRepeat,
                    snap2DVerticesToPixel,
                    canvasCullMask
                );

                //mirroring (useful for scrolling backgrounds)
                if (ci2.Mirror.X != 0)
                {
                    var xform2 = transform * new Transform2D<RealT>(0, new(ci2.Mirror.X, 0));
                    this.RenderCanvasItemTree(
                        renderTarget,
                        null,
                        ci2.Item,
                        xform2,
                        clipRect,
                        canvas.Modulate,
                        lights,
                        directionalLights,
                        defaultFilter,
                        defaultRepeat,
                        snap2DVerticesToPixel,
                        canvasCullMask
                    );
                }
                if (ci2.Mirror.Y != 0)
                {
                    var xform2 = transform * new Transform2D<RealT>(0, new(0, ci2.Mirror.Y));

                    this.RenderCanvasItemTree(
                        renderTarget,
                        null,
                        ci2.Item,
                        xform2,
                        clipRect,
                        canvas.Modulate,
                        lights,
                        directionalLights,
                        defaultFilter,
                        defaultRepeat,
                        snap2DVerticesToPixel,
                        canvasCullMask
                    );
                }
                if (ci2.Mirror.Y != 0 && ci2.Mirror.X != 0)
                {
                    var xform2 = transform * new Transform2D<RealT>(0, ci2.Mirror);

                    this.RenderCanvasItemTree(
                        renderTarget,
                        null,
                        ci2.Item,
                        xform2,
                        clipRect,
                        canvas.Modulate,
                        lights,
                        directionalLights,
                        defaultFilter,
                        defaultRepeat,
                        snap2DVerticesToPixel,
                        canvasCullMask
                    );
                }
            }
        }

        RENDER_TIMESTAMP("< Render Canvas");
    }

    public void UpdateVisibilityNotifiers()
    {
        var e = this.visibilityNotifierList.First;

        while (e != null)
        {
            var n = e.Next;

            var visibilityNotifier = e.Self;
            if (visibilityNotifier!.JustVisible)
            {
                visibilityNotifier.JustVisible = false;

                if (visibilityNotifier.EnterCallable != null)
                {
                    if (RSG.Threaded)
                    {
                        MessageQueue.Singleton.PushCallable(visibilityNotifier.EnterCallable);
                    }
                    else
                    {
                        visibilityNotifier.EnterCallable();
                    }
                }
            }
            else
            {
                if (visibilityNotifier.VisibleInFrame != RSG.Rasterizer.FrameNumber)
                {
                    this.visibilityNotifierList.Remove(e);

                    if (visibilityNotifier.ExitCallable != null)
                    {
                        if (RSG.Threaded)
                        {
                            MessageQueue.Singleton.PushCallable(visibilityNotifier.ExitCallable);
                        }
                        else
                        {
                            visibilityNotifier.ExitCallable.Invoke();
                        }
                    }
                }
            }

            e = n;
        }
    }
    #endregion public methods
}
