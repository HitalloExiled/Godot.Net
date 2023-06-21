namespace Godot.Net.Scene.GUI;

using Godot.Net.Core;
using Godot.Net.Core.Enums;
using Godot.Net.Core.Math;
using Godot.Net.Scene.Main;

#pragma warning disable CS0067, IDE0044, IDE0052, CS0649, CS0414 // TODO Remove

public partial class TabContainer : Container
{
    public event Action<int>? TabChanged;

    private readonly TabBar     tabBar;
    private readonly ThemeCache themeCache = new();

    private bool   menuHovered;
    private Popup? popup;
    private bool   tabsVisible = true;
    private bool   themeChanging;

    #region public readonly properties
    public bool                 ClipTabs     => this.tabBar.ClipTabs;
    public TabBar.AlignmentMode TabAlignment => this.tabBar.TabAlignment;
    public int                  TabCount     => this.tabBar.TabCount;

    public int TopMargin
    {
        get
        {
            var height = 0;
            if (this.tabsVisible && this.TabCount > 0)
            {
                height = (int)this.tabBar.MinimumSize.Y;
            }

            return height;
        }
    }
    #endregion public readonly properties

    #region public properties
    public Popup? Popup
    {
        get => this.popup;
        set
        {
            if (this.popup != value)
            {
                this.QueueRedraw();
                this.UpdateMargins();

                if (!this.ClipTabs)
                {
                    this.UpdateMinimumSize();
                }
            }

            this.popup = value;
        }
    }
    #endregion public properties

    public TabContainer()
    {
        this.tabBar = new TabBar() { PostInitialize = true };

        this.tabBar.SetDragForwarding(this.GetDragDataFw, this.CanDropDataFw, this.DropDataFw);

        this.AddChild(this.tabBar, false, InternalMode.INTERNAL_MODE_FRONT);

        this.tabBar.SetAnchorsAndOffsetsPreset(LayoutPreset.PRESET_TOP_WIDE);

        this.tabBar.TabChanged       += this.OnTabChanged;
        this.tabBar.TabSelected      += this.OnTabSelected;
        this.tabBar.TabButtonPressed += this.OnTabButtonPressed;

        MouseExited += this.OnMouseExited;
    }

    private bool CanDropDataFw(in Vector2<RealT> point, object? data, Control fromControl) => throw new NotImplementedException();
    private void DropDataFw(in Vector2<RealT> point, object? data, Control fromControl) => throw new NotImplementedException();
    private object? GetDragDataFw(in Vector2<RealT> point, Control fromControl) => throw new NotImplementedException();
    private void OnMouseExited() => throw new NotImplementedException();
    private void OnTabButtonPressed(int tab) => throw new NotImplementedException();
    private void OnTabChanged(int tab) => throw new NotImplementedException();
    private void OnTabSelected(int tab) => throw new NotImplementedException();
    private void OnThemeChanged() => throw new NotImplementedException();
    private void RefreshTabNames() => throw new NotImplementedException();

    private void UpdateMargins()
    {
        ASSERT_NOT_NULL(this.themeCache.MenuIcon);

        var menuWidth = this.themeCache.MenuIcon.Width;

        // Directly check for validity, to avoid errors when quitting.
        var hasPopup = this.popup != default;

        if (this.TabCount == 0)
        {
            this.tabBar.SetOffset(Side.SIDE_LEFT, 0);
            this.tabBar.SetOffset(Side.SIDE_RIGHT, hasPopup ? menuWidth : 0);

            return;
        }

        switch (this.TabAlignment)
        {
            case TabBar.AlignmentMode.ALIGNMENT_LEFT:
                this.tabBar.SetOffset(Side.SIDE_LEFT, this.themeCache.SideMargin);
                this.tabBar.SetOffset(Side.SIDE_RIGHT, hasPopup ? menuWidth : 0);

                break;

            case TabBar.AlignmentMode.ALIGNMENT_CENTER:
                this.tabBar.SetOffset(Side.SIDE_LEFT, 0);
                this.tabBar.SetOffset(Side.SIDE_RIGHT, hasPopup ? menuWidth : 0);

                break;

            case TabBar.AlignmentMode.ALIGNMENT_RIGHT:
                {
                    this.tabBar.SetOffset(Side.SIDE_LEFT, 0);

                    if (hasPopup)
                    {
                        this.tabBar.SetOffset(Side.SIDE_RIGHT, menuWidth);
                        return;
                    }

                    var firstTabPos    = this.tabBar.GetTabRect(0).Position.X;
                    var lastTabRect    = this.tabBar.GetTabRect(this.TabCount - 1);
                    var totalTabsWidth = lastTabRect.Position.X - firstTabPos + lastTabRect.Size.X;

                    // Calculate if all the tabs would still fit if the margin was present.
                    if (this.ClipTabs && (this.tabBar.OffsetButtonsVisible || this.TabCount > 1 && totalTabsWidth + this.themeCache.SideMargin > this.Size.X))
                    {
                        this.tabBar.SetOffset(Side.SIDE_RIGHT, hasPopup ? menuWidth : 0);
                    }
                    else
                    {
                        this.tabBar.SetOffset(Side.SIDE_RIGHT, this.themeCache.SideMargin);
                    }
                }

                break;

            case TabBar.AlignmentMode.ALIGNMENT_MAX:
                break; // Can't happen, but silences warning.
        }
    }

    protected override void UpdateThemeItemCache()
    {
        base.UpdateThemeItemCache();

        this.themeCache.SideMargin  = this.GetThemeConstant("side_margin");
        this.themeCache.PanelStyle  = this.GetThemeStylebox("panel");
        this.themeCache.TabbarStyle = this.GetThemeStylebox("tabbar_background");
        this.themeCache.MenuIcon    = this.GetThemeIcon("menu");
        this.themeCache.MenuHlIcon  = this.GetThemeIcon("menu_highlight");

        // TabBar overrides.
        this.themeCache.IconSeparation      = this.GetThemeConstant("icon_separation");
        this.themeCache.OutlineSize         = this.GetThemeConstant("outline_size");
        this.themeCache.TabUnselectedStyle  = this.GetThemeStylebox("tab_unselected");
        this.themeCache.TabSelectedStyle    = this.GetThemeStylebox("tab_selected");
        this.themeCache.TabDisabledStyle    = this.GetThemeStylebox("tab_disabled");
        this.themeCache.IncrementIcon       = this.GetThemeIcon("increment");
        this.themeCache.IncrementHlIcon     = this.GetThemeIcon("increment_highlight");
        this.themeCache.DecrementIcon       = this.GetThemeIcon("decrement");
        this.themeCache.DecrementHlIcon     = this.GetThemeIcon("decrement_highlight");
        this.themeCache.DropMarkIcon        = this.GetThemeIcon("drop_mark");
        this.themeCache.DropMarkColor       = this.GetThemeColor("drop_mark_color");
        this.themeCache.FontSelectedColor   = this.GetThemeColor("font_selected_color");
        this.themeCache.FontUnselectedColor = this.GetThemeColor("font_unselected_color");
        this.themeCache.FontDisabledColor   = this.GetThemeColor("font_disabled_color");
        this.themeCache.FontOutlineColor    = this.GetThemeColor("font_outline_color");
        this.themeCache.TabFont             = this.GetThemeFont("font");
        this.themeCache.TabFontSize         = this.GetThemeFontSize("font_size");
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
                // If some nodes happen to be renamed outside the tree, the tab names need to be updated manually.
                if (this.TabCount > 0)
                {
                    this.RefreshTabNames();
                }
                break;

            case NotificationKind.NOTIFICATION_READY:
            case NotificationKind.CONTROL_NOTIFICATION_RESIZED:
                this.UpdateMargins();
                break;

            case NotificationKind.CANVAS_ITEM_NOTIFICATION_DRAW:
                {
                    var canvas = this.CanvasItemId;
                    var size = this.Size;

                    // Draw only the tab area if the header is hidden.
                    if (!this.tabsVisible)
                    {
                        this.themeCache.PanelStyle!.Draw(canvas, new(0, 0, size.X, size.Y));
                        return;
                    }

                    var headerHeight = this.TopMargin;

                    // Draw background for the tabbar.
                    this.themeCache.TabbarStyle!.Draw(canvas, new(0, 0, size.X, headerHeight));
                    // Draw the background for the tab's content.
                    this.themeCache.PanelStyle!.Draw(canvas, new(0, headerHeight, size.X, size.Y - headerHeight));

                    // Draw the popup menu.
                    if (this.Popup != null)
                    {
                        var x = (int)(this.IsLayoutRtl() ? 0 : this.Size.X - this.themeCache.MenuIcon!.Width);

                        if (this.menuHovered)
                        {
                            this.themeCache.MenuHlIcon!.Draw(this.CanvasItemId, new(x, (headerHeight - this.themeCache.MenuHlIcon!.Height) / 2));
                        }
                        else
                        {
                            this.themeCache.MenuIcon!.Draw(this.CanvasItemId, new(x, (headerHeight - this.themeCache.MenuIcon!.Height) / 2));
                        }
                    }
                }
                break;

            case NotificationKind.MAIN_LOOP_NOTIFICATION_TRANSLATION_CHANGED:
            case NotificationKind.CONTROL_NOTIFICATION_LAYOUT_DIRECTION_CHANGED:
            case NotificationKind.CONTROL_NOTIFICATION_THEME_CHANGED:
                this.themeChanging = true;
                MessageQueue.Singleton.PushCallable(this.OnThemeChanged); // Wait until all changed theme.
                break;
        }

        if (reversed)
        {
            base.Notification(notification, reversed);
        }
    }
}
