namespace Godot.Net.Scene.GUI;

using Godot.Net.Core.Math;
using Godot.Net.Scene.Main;
using Godot.Net.Scene.Resources;

public class Panel : Control
{
    private record ThemeCache
    {
        public StyleBox? PanelStyle { get; set; }
    };

    private readonly ThemeCache themeCache = new();

    public Panel() => this.MouseFilter = MouseFilterKind.MOUSE_FILTER_STOP;

    protected override void UpdateThemeItemCache()
    {
        base.UpdateThemeItemCache();

        this.themeCache.PanelStyle = this.GetThemeStylebox("panel");
    }

    public override void Notification(NotificationKind notification, bool reversed = false)
    {
        if (!reversed)
        {
            base.Notification(notification, reversed);
        }

        switch (notification)
        {
            case NotificationKind.CANVAS_ITEM_NOTIFICATION_DRAW:
                this.themeCache.PanelStyle!.Draw(this.CanvasItemId, new Rect2<RealT>(new(), this.Size));
                break;
        }

        if (reversed)
        {
            base.Notification(notification, reversed);
        }
    }
}
