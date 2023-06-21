namespace Godot.Net.Scene.Main;

using System.Runtime.CompilerServices;
using Godot.Net.Core;
using Godot.Net.Core.Math;
using Godot.Net.Scene.Resources;

#pragma warning disable IDE0052, IDE0044, CS0414, CS0649, IDE0051, CS0169 // TODO Remover

public partial class CanvasItem : Node
{
    #region public events
    public event Action? Draw;
    public event Action? ItemRectChanged;
    public event Action? VisibilityChanged;
    #endregion public events

    private static CanvasItem? currentItemDrawn;

    private readonly SelfList<Node> xformChange;

    #region private fields
    private bool                       behind;
    private bool                       blockTransformNotify;
    private string?                    canvasGroup;
    private CanvasLayer?               canvasLayer;
    private ClipChildrenMode           clipChildrenMode   = ClipChildrenMode.CLIP_CHILDREN_DISABLED;
    private bool                       drawing;
    private bool                       globalInvalid      = true;
    private Transform2D<RealT>         globalTransform    = new();
    private bool                       hideClipChildren;
    private int                        lightMask          = 1;
    private Material?                  material;
    private Color                      modulate           = new(1, 1, 1, 1);
    private bool                       notifyLocalTransform;
    private bool                       notifyTransform;
    private bool                       parentVisibleInTree;
    private bool                       pendingUpdate;
    private Color                      selfModulate       = new(1, 1, 1, 1);
    private TextureFilter              textureFilter      = TextureFilter.TEXTURE_FILTER_PARENT_NODE;
    private RS.CanvasItemTextureFilter textureFilterCache = RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_LINEAR;
    private TextureRepeat              textureRepeat      = TextureRepeat.TEXTURE_REPEAT_PARENT_NODE;
    private RS.CanvasItemTextureRepeat textureRepeatCache = RS.CanvasItemTextureRepeat.CANVAS_ITEM_TEXTURE_REPEAT_DISABLED;
    private bool                       useParentMaterial;
    private uint                       visibilityLayer    = 1;
    private bool                       visible            = true;
    private Window?                    window;
    private bool                       ySortEnabled;
    private int                        zIndex;
    private bool                       zRelative          = true;
    #endregion private fields

    #region public readonly properties
    public List<CanvasItem> ChildrenItems   { get; } = new();
    public Guid             CanvasItemId    { get; }
    public bool             IsSetAsTopLevel { get; }
    public bool             IsVisibleInTree => this.visible && this.parentVisibleInTree;
    public CanvasItem?      ParentItem      => this.IsSetAsTopLevel ? null : this.Parent as CanvasItem;
    #endregion public readonly properties

    #region public properties
    public bool Visible
    {
        get => this.visible;
        set
        {
            if (this.visible == value)
            {
                return;
            }

            this.visible = value;

            if (!this.parentVisibleInTree)
            {
                this.Notification(NotificationKind.CANVAS_ITEM_NOTIFICATION_VISIBILITY_CHANGED);

                return;
            }

            this.HandleVisibilityChange(value);
        }
    }
    #endregion public properties

    public CanvasItem()
    {
        this.xformChange  = new(this);
        this.CanvasItemId = RS.Singleton.CanvasItemCreate();
    }

    #region private methods
    private void EnterCanvas()
    {
        // Resolves to nullptr if the node is top_level.
        if (this.ParentItem != null)
        {
            this.canvasLayer = this.ParentItem.canvasLayer;
            RS.Singleton.CanvasItemSetParent(this.CanvasItemId, this.ParentItem.CanvasItemId);
            RS.Singleton.CanvasItemSetDrawIndex(this.CanvasItemId, this.GetIndex());
            RS.Singleton.CanvasItemSetVisibilityLayer(this.CanvasItemId, this.visibilityLayer);
        }
        else
        {
            Node? n = this;

            this.canvasLayer = null;

            while (n != null)
            {
                this.canvasLayer = n as CanvasLayer;

                if (this.canvasLayer != null)
                {
                    break;
                }

                if (n is Viewport)
                {
                    break;
                }

                n = n!.Parent;
            }

            var canvas = this.canvasLayer != null ? this.canvasLayer.Canvas : this.Viewport.FindWorld2D().Canvas;

            RS.Singleton.CanvasItemSetParent(this.CanvasItemId, canvas);
            RS.Singleton.CanvasItemSetVisibilityLayer(this.CanvasItemId, this.visibilityLayer);

            this.canvasGroup = "root_canvas" + canvas;

            this.AddToGroup(this.canvasGroup);

            if (this.canvasLayer != null)
            {
                this.canvasLayer.ResetSortIndex();
            }
            else
            {
                this.Viewport.GuiResetCanvasSortIndex();
            }

            this.Tree.CallGroupFlags(SceneTree.GroupCallFlags.GROUP_CALL_UNIQUE | SceneTree.GroupCallFlags.GROUP_CALL_DEFERRED, this.canvasGroup, "_top_level_raise_self");
        }

        this.pendingUpdate = false;
        this.QueueRedraw();

        base.Notification(NotificationKind.CANVAS_ITEM_NOTIFICATION_ENTER_CANVAS);
    }
    private void ExitCanvas() => throw new NotImplementedException();
    private void HandleVisibilityChange(bool value) => throw new NotImplementedException();

    private void NotifyTransform(CanvasItem node)
    {
        /* This check exists to avoid re-propagating the transform
        * notification down the tree on dirty nodes. It provides
        * optimization by avoiding redundancy (nodes are dirty, will get the
        * notification anyway).
        */

        if (/*node.xformChange.InList &&*/ node.globalInvalid)
        {
            return; //nothing to do
        }

        node.globalInvalid = true;

        if (node.notifyTransform && !node.xformChange.InList)
        {
            if (!node.blockTransformNotify)
            {
                if (node.IsInsideTree)
                {
                    this.Tree.XformChangeList.Add(node.xformChange);
                }
            }
        }

        foreach (var ci in node.ChildrenItems)
        {
            if (ci.IsSetAsTopLevel)
            {
                continue;
            }

            this.NotifyTransform(ci);
        }
    }

    private void RedrawCallback()
    {
        if (!this.IsInsideTree)
        {
            this.pendingUpdate = false;
            return;
        }

        RS.Singleton.CanvasItemClear(this.CanvasItemId);
        //todo updating = true - only allow drawing here
        if (this.IsVisibleInTree)
        {
            this.drawing = true;
            currentItemDrawn = this;
            this.Notification(NotificationKind.CANVAS_ITEM_NOTIFICATION_DRAW);
            this.Draw?.Invoke();
            // GDVIRTUAL_CALL(_draw);
            currentItemDrawn = null;
            this.drawing = false;
        }
        //todo updating = false
        this.pendingUpdate = false; // don't change to false until finished drawing (avoid recursive update)
    }

    private void RefreshTextureFilterCache()
    {
        if (!this.IsInsideTree)
        {
            return;
        }

        this.textureFilterCache = this.textureFilter == TextureFilter.TEXTURE_FILTER_PARENT_NODE
            ? this.ParentItem != null
                ? this.ParentItem.textureFilterCache
                : RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_DEFAULT
            : (RS.CanvasItemTextureFilter)this.textureFilter;
    }


    private void UpdateTextureFilterChanged(bool propagate)
    {
        if (!this.IsInsideTree)
        {
            return;
        }

        this.RefreshTextureFilterCache();

        RS.Singleton.CanvasItemSetDefaultTextureFilter(this.CanvasItemId, this.textureFilterCache);
        this.QueueRedraw();

        if (propagate)
        {
            foreach (var e in this.ChildrenItems)
            {
                if (!e.IsSetAsTopLevel && e.textureFilter == TextureFilter.TEXTURE_FILTER_PARENT_NODE)
                {
                    e.UpdateTextureFilterChanged(true);
                }
            }
        }
    }

    private void UpdateTextureRepeatChanged(bool propagate)
    {
        if (!this.IsInsideTree)
        {
            return;
        }

        this.RefreshTextureRepeatCache();

        RS.Singleton.CanvasItemSetDefaultTextureRepeat(this.CanvasItemId, this.textureRepeatCache);

        this.QueueRedraw();

        if (propagate)
        {
            foreach (var item in this.ChildrenItems)
            {
                if (!item.IsSetAsTopLevel && item.textureRepeat == TextureRepeat.TEXTURE_REPEAT_PARENT_NODE)
                {
                    item.UpdateTextureRepeatChanged(true);
                }
            }
        }
    }

    private void RefreshTextureRepeatCache()
    {
        if (!this.IsInsideTree)
        {
            return;
        }

        this.textureRepeatCache = this.textureRepeat == TextureRepeat.TEXTURE_REPEAT_PARENT_NODE
            ? this.ParentItem != null
                ? this.ParentItem.textureRepeatCache
                : RS.CanvasItemTextureRepeat.CANVAS_ITEM_TEXTURE_REPEAT_DEFAULT
            : (RS.CanvasItemTextureRepeat)this.textureRepeat;
    }

    private void WindowVisibilityChanged() => throw new NotImplementedException();
    #endregion private methods

    #region protected methods
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected void NotifyTransform()
    {
        if (!this.IsInsideTree)
        {
            return;
        }

        this.NotifyTransform(this);

        if (!this.blockTransformNotify && this.notifyLocalTransform)
        {
            this.Notification(NotificationKind.CANVAS_ITEM_NOTIFICATION_LOCAL_TRANSFORM_CHANGED);
        }
	}

    protected void NotifyItemRectChanged(bool sizeChanged = true)
    {
        if (sizeChanged)
        {
            this.QueueRedraw();
        }

        ItemRectChanged?.Invoke();
    }
    #endregion protected virtual methods

    #region public virtual methods
    public virtual Rect2<RealT> GetAnchorableRect() => new(0f, 0f, 0f, 0f);
    #endregion public virtual methods

    #region public methods
    public Vector2<RealT> GetGlobalMousePosition() => throw new NotImplementedException();

    public void Hide() =>
        this.Visible = false;

    public void QueueRedraw()
    {
        if (!this.IsInsideTree)
        {
            return;
        }

        if (this.pendingUpdate)
        {
            return;
        }

        this.pendingUpdate = true;

        MessageQueue.Singleton.PushCallable(this.RedrawCallback);
    }
    #endregion public methods

    #region public overrided methods
    public override void Notification(NotificationKind notification, bool reversed = false)
    {
        if (!reversed)
        {
            base.Notification(notification, reversed);
        }

        switch (notification)
        {
            case NotificationKind.NOTIFICATION_ENTER_TREE:
            {
                if (ERR_FAIL_COND(!this.IsInsideTree))
                {
                    return;
                }

                var parent = this.Parent;

                if (parent != null)
                {
                    if (parent is CanvasItem ci)
                    {
                        this.parentVisibleInTree = ci.IsVisibleInTree;
                        ci.ChildrenItems.Add(this);
                    }
                    else
                    {
                        if (parent is CanvasLayer cl)
                        {
                            this.parentVisibleInTree = cl.IsVisible;
                        }
                        else
                        {
                            // Look for a window.
                            var viewport = default(Viewport);

                            while (parent != null)
                            {
                                viewport = parent as Viewport;
                                if (viewport != null)
                                {
                                    break;
                                }
                                parent = parent.Parent;
                            }

                            if (ERR_FAIL_COND(viewport == null))
                            {
                                return;
                            }

                            this.window = viewport as Window;
                            if (this.window != null)
                            {
                                this.window.VisibilityChanged += this.WindowVisibilityChanged;
                                this.parentVisibleInTree = this.window.IsVisible;
                            }
                            else
                            {
                                this.parentVisibleInTree = true;
                            }
                        }
                    }
                }

                RS.Singleton.CanvasItemSetVisible(this.CanvasItemId, this.IsVisibleInTree); // The visibility of the parent may change.

                if (this.IsVisibleInTree)
                {
                    base.Notification(NotificationKind.WINDOW_NOTIFICATION_VISIBILITY_CHANGED); // Considered invisible until entered.
                }

                this.EnterCanvas();

                this.UpdateTextureFilterChanged(false);
                this.UpdateTextureRepeatChanged(false);

                if (!this.blockTransformNotify && !this.xformChange.InList)
                {
                    this.Tree.XformChangeList.Add(this.xformChange);
                }
            } break;

            case NotificationKind.NOTIFICATION_MOVED_IN_PARENT:
                if (!this.IsInsideTree)
                {
                    break;
                }

                if (this.canvasGroup != default)
                {
                    this.Tree.CallGroupFlags(SceneTree.GroupCallFlags.GROUP_CALL_UNIQUE | SceneTree.GroupCallFlags.GROUP_CALL_DEFERRED, this.canvasGroup, "_top_level_raise_self");
                }
                else
                {
                    if (ERR_FAIL_COND_MSG(this.ParentItem == null, "Moved child is in incorrect state (no canvas group, no canvas item parent)."))
                    {
                        return;
                    }

                    RS.Singleton.CanvasItemSetDrawIndex(this.CanvasItemId, this.GetIndex());
                }
                break;

            case NotificationKind.NOTIFICATION_EXIT_TREE:
                if (this.xformChange.InList)
                {
                    this.Tree.XformChangeList.Remove(this.xformChange);
                }

                this.ExitCanvas();

                ((CanvasItem)this.Parent!).ChildrenItems.Remove(this);

                if (this.window != null)
                {
                    this.window.VisibilityChanged -= this.WindowVisibilityChanged;
                    this.window = null;
                }
                this.globalInvalid       = true;
                this.parentVisibleInTree = false;
                break;

            case NotificationKind.WINDOW_NOTIFICATION_VISIBILITY_CHANGED:
                VisibilityChanged?.Invoke();
                break;
        }

        if (reversed)
        {
            base.Notification(notification, reversed);
        }
    }
    #endregion public overrided methods
}
