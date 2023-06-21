namespace Godot.Net.Scene.GUI;

using Godot.Net.Core.Math;
using Godot.Net.Scene.Main;

#pragma warning disable CS0649 // TODO Remove;

public partial class BoxContainer : Container
{
    private readonly ThemeCache themeCache = new();

    private AlignmentMode alignment;
    private bool          vertical;

    protected bool IsFixed { get; set; }

    public bool Vertical
    {
        get => this.vertical;
        set
        {
            if (ERR_FAIL_COND_MSG(this.IsFixed, $"Can't change orientation of {this.GetType().Name}."))
            {
                return;
            }

            this.vertical = value;

            this.UpdateMinimumSize();
            this.Resort();
        }
    }

    public BoxContainer(bool vertical = false) =>
        this.vertical = vertical;

    private void Resort()
    {
        /** First pass, determine minimum size AND amount of stretchable elements */

        var newSize = this.Size;

        var rtl = this.IsLayoutRtl();
        var childrenCount     = 0;
        var stretchMin        = 0;
        var stretchAvail      = 0;
        var stretchRatioTotal = 0f;
        var minSizeCache      = new Dictionary<Control, MinSizeCache>();

        for (var i = 0; i < this.GetChildCount(); i++)
        {
            if (this.GetChild(i) is not Control c || !c.IsVisibleInTree)
            {
                continue;
            }
            if (c.IsSetAsTopLevel)
            {
                continue;
            }

            var size = c.CombinedMinimumSize;
            MinSizeCache msc;

            if (this.vertical)
            {
                /* VERTICAL */
                stretchMin += (int)size.Y;
                msc = new()
                {
                    MinSize     = (int)size.Y,
                    WillStretch = c.VSizeFlags.HasFlag(SizeFlags.SIZE_EXPAND)
                };

            }
            else
            {
                /* HORIZONTAL */
                stretchMin += (int)size.X;
                msc = new()
                {
                    MinSize     = (int)size.X,
                    WillStretch = c.HSizeFlags.HasFlag(SizeFlags.SIZE_EXPAND)
                };
            }

            if (msc.WillStretch)
            {
                stretchAvail      += msc.MinSize;
                stretchRatioTotal += c.StretchRatio;
            }

            msc.FinalSize = msc.MinSize;
            minSizeCache[c] = msc;
            childrenCount++;
        }

        if (childrenCount == 0)
        {
            return;
        }

        var stretchMax  = (int)((this.vertical ? newSize.Y : newSize.X) - (childrenCount - 1) * this.themeCache.Separation);
        var stretchDiff = stretchMax - stretchMin;
        if (stretchDiff < 0)
        {
            //avoid negative stretch space
            stretchDiff = 0;
        }

        stretchAvail += stretchDiff; //available stretch space.
        /** Second, pass successively to discard elements that can't be stretched, this will run while stretchable
            elements exist */

        var hasStretched = false;
        while (stretchRatioTotal > 0)
        { // first of all, don't even be here if no stretchable objects exist

            hasStretched = true;

            var refitSuccessful = true; //assume refit-test will go well
            var error           = 0f; // Keep track of accumulated error in pixels

            for (var i = 0; i < this.GetChildCount(); i++)
            {
                if (this.GetChild(i) is not Control c || !c.IsVisibleInTree)
                {
                    continue;
                }
                if (c.IsSetAsTopLevel)
                {
                    continue;
                }

                if (ERR_FAIL_COND(!minSizeCache.ContainsKey(c)))
                {
                    return;
                }

                var msc = minSizeCache[c];

                if (msc.WillStretch)
                { //wants to stretch
                  //let's see if it can really stretch
                    var finalPixelSize = stretchAvail * c.StretchRatio / stretchRatioTotal;
                    // Add leftover fractional pixels to error accumulator
                    error += finalPixelSize - (int)finalPixelSize;
                    if (finalPixelSize < msc.MinSize)
                    {
                        //if available stretching area is too small for widget,
                        //then remove it from stretching area
                        msc.WillStretch = false;

                        stretchRatioTotal -= c.StretchRatio;
                        refitSuccessful   = false;
                        stretchAvail      -= msc.MinSize;

                        msc.FinalSize = msc.MinSize;
                        break;
                    }
                    else
                    {
                        msc.FinalSize = (int)finalPixelSize;
                        // Dump accumulated error if one pixel or more
                        if (error >= 1)
                        {
                            msc.FinalSize += 1;
                            error -= 1;
                        }
                    }
                }
            }

            if (refitSuccessful)
            { //uf refit went well, break
                break;
            }
        }

        /** Final pass, draw and stretch elements **/

        var ofs = 0;
        if (!hasStretched)
        {
            if (!this.vertical)
            {
                switch (this.alignment)
                {
                    case AlignmentMode.ALIGNMENT_BEGIN:
                        if (rtl)
                        {
                            ofs = stretchDiff;
                        }
                        break;
                    case AlignmentMode.ALIGNMENT_CENTER:
                        ofs = stretchDiff / 2;
                        break;
                    case AlignmentMode.ALIGNMENT_END:
                        if (!rtl)
                        {
                            ofs = stretchDiff;
                        }
                        break;
                }
            }
            else
            {
                switch (this.alignment)
                {
                    case AlignmentMode.ALIGNMENT_BEGIN:
                        break;
                    case AlignmentMode.ALIGNMENT_CENTER:
                        ofs = stretchDiff / 2;
                        break;
                    case AlignmentMode.ALIGNMENT_END:
                        ofs = stretchDiff;
                        break;
                }
            }
        }

        var first = true;
        var idx = 0;

        int start;
        int end;
        int delta;
        if (!rtl || this.vertical)
        {
            start = 0;
            end  = this.GetChildCount();
            delta = +1;
        }
        else
        {
            start = this.GetChildCount() - 1;
            end   = -1;
            delta = -1;
        }

        for (var i = start; i != end; i += delta)
        {
            if (this.GetChild(i) is not Control c || !c.IsVisibleInTree)
            {
                continue;
            }
            if (c.IsSetAsTopLevel)
            {
                continue;
            }

            var msc = minSizeCache[c];

            if (first)
            {
                first = false;
            }
            else
            {
                ofs += this.themeCache.Separation;
            }

            var from = ofs;
            var to   = ofs + msc.FinalSize;

            if (msc.WillStretch && idx == childrenCount - 1)
            {
                //adjust so the last one always fits perfect
                //compensating for numerical imprecision

                to = (int)(this.vertical ? newSize.Y : newSize.X);
            }

            var size = to - from;

            var rect = this.vertical
                ? new Rect2<RealT>(0, from, newSize.X, size)
                : new Rect2<RealT>(from, 0, size, newSize.Y);

            this.FitChildInRect(c, rect);

            ofs = to;
            idx++;
        }
    }

    public override void Notification(NotificationKind notification, bool reversed = false)
    {
        if (!reversed)
        {
            base.Notification(notification, reversed);
        }

        switch (notification)
        {
            case NotificationKind.CONTAINER_NOTIFICATION_SORT_CHILDREN:
                this.Resort();

                break;

            case NotificationKind.CONTROL_NOTIFICATION_THEME_CHANGED:
                this.UpdateMinimumSize();

                break;

            case NotificationKind.MAIN_LOOP_NOTIFICATION_TRANSLATION_CHANGED:
            case NotificationKind.CONTROL_NOTIFICATION_LAYOUT_DIRECTION_CHANGED:
                this.QueueSort();

                break;
        }

        if (reversed)
        {
            base.Notification(notification, reversed);
        }
    }
}
