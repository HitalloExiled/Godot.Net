namespace Godot.Net.Scene.GUI;

using Godot.Net.Core;
using Godot.Net.Core.Math;
using Godot.Net.Scene.Main;

#pragma warning disable IDE0052, CS0414 // TODO Remove;

public class Container : Control
{
    public event Action? PreSortChildren;
    public event Action? SortChildren;

    private bool pendingSort;

    public Container() =>
        this.MouseFilter = MouseFilterKind.MOUSE_FILTER_PASS;

    private void NotifySortChildren()
    {
        if (!this.IsInsideTree)
        {
            return;
        }

        this.Notification(NotificationKind.CONTAINER_NOTIFICATION_PRE_SORT_CHILDREN);
        PreSortChildren?.Invoke();

        this.Notification(NotificationKind.CONTAINER_NOTIFICATION_SORT_CHILDREN);
        SortChildren?.Invoke();

        this.pendingSort = false;
    }

    protected void QueueSort()
    {
        if (!this.IsInsideTree)
        {
            return;
        }

        if (this.pendingSort)
        {
            return;
        }

        MessageQueue.Singleton.PushCallable(this.NotifySortChildren);

        this.pendingSort = true;
    }

    public void FitChildInRect(Control child, Rect2<RealT> rect)
    {
        if (ERR_FAIL_COND(child.Parent != this))
        {
            return;
        }

        var rtl     = this.IsLayoutRtl();
        var minsize = child.CombinedMinimumSize;
        var r       = rect;

        if (!child.HSizeFlags.HasFlag(SizeFlags.SIZE_FILL))
        {
            r.Size = r.Size with { X = minsize.X };

            r.Position = child.HSizeFlags.HasFlag(SizeFlags.SIZE_SHRINK_END)
                ? r.Position with
                {
                    X = r.Position.X + (rtl ? 0 : (rect.Size.X - minsize.X))
                }
                : child.HSizeFlags.HasFlag(SizeFlags.SIZE_SHRINK_CENTER)
                    ? r.Position with
                    {
                        X = r.Position.X + (RealT)Math.Floor((rect.Size.X - minsize.X) / 2)
                    }
                    : r.Position with
                    {
                        X = r.Position.X + (rtl ? (rect.Size.X - minsize.X) : 0)
                    };
        }

        if (!child.VSizeFlags.HasFlag(SizeFlags.SIZE_FILL))
        {
            r.Size = r.Size with { Y = minsize.Y };

            r.Position = child.VSizeFlags.HasFlag(SizeFlags.SIZE_SHRINK_END)
                ? r.Position with
                {
                    Y = r.Position.Y + rect.Size.Y - minsize.Y
                }
                : child.VSizeFlags.HasFlag(SizeFlags.SIZE_SHRINK_CENTER)
                    ? r.Position with
                    {
                        Y = r.Position.Y + (RealT)Math.Floor((rect.Size.Y - minsize.Y) / 2)
                    }
                    : r.Position with
                    {
                        Y = r.Position.Y + 0 // Why?
                    };
        }

        child.SetRect(r);
        child.        Rotation = 0;
        child.        Scale = new(1, 1);
    }

    public override void Notification(NotificationKind notification, bool reversed = false)
    {
        if (!reversed)
        {
            base.Notification(notification, reversed);
        }

        switch (notification)
        {
            case NotificationKind.NOTIFICATION_ENTER_TREE:
                this.pendingSort = false;
                this.QueueSort();

                break;

            case NotificationKind.CONTROL_NOTIFICATION_RESIZED:
            case NotificationKind.CONTROL_NOTIFICATION_THEME_CHANGED:
                this.QueueSort();

                break;

            case NotificationKind.CANVAS_ITEM_NOTIFICATION_VISIBILITY_CHANGED:
                if (this.IsVisibleInTree)
                {
                    this.QueueSort();
                }

                break;
        }

        if (reversed)
        {
            base.Notification(notification, reversed);
        }
    }
}
