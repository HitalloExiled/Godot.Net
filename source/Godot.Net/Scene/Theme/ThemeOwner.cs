namespace Godot.Net.Scene.Theme;

using Godot.Net.Scene.GUI;
using Godot.Net.Scene.Main;

using Theme         = Resources.Theme;
using ThemeDataType = Resources.Theme.DataType;

public class ThemeOwner
{
    private Control? ownerControl;
    private Window?  ownerWindow;

    public bool HasOwnerNode => this.ownerControl != null || this.ownerWindow != null;

    public Node? OwnerNode
    {
        get
        {
            if (this.ownerControl != null)
            {
                return this.ownerControl;
            }
            else if (this.ownerWindow != null)
            {
                return this.ownerWindow;
            }
            return null;
        }
        set
        {
            this.ownerControl = null;
            this.ownerWindow = null;

            if (value is Control c)
            {
                this.ownerControl = c;
                return;
            }

            if (value is Window w)
            {
                this.ownerWindow = w;
                return;
            }
        }
    }

    private Node GetNextOwnerNode(Node fromNode) => throw new NotImplementedException();

    private static Theme? GetOwnerNodeTheme(Node ownerNode) =>
        ownerNode is Control control
            ? control.Theme
            : ownerNode is Window window
                ? window.Theme
                : default;

    public static void AssignThemeOnParented(Node forNode)
    {
        // We check if there are any themes affecting the parent. If that's the case
        // its children also need to be affected.
        // We don't notify here because `NOTIFICATION_THEME_CHANGED` will be handled
        // a bit later by `NOTIFICATION_ENTER_TREE`.

        if (forNode.Parent is Control parentControl && parentControl.HasThemeOwnerNode)
        {
            PropagateThemeChanged(forNode, parentControl.ThemeOwnerNode, false, true);
        }
        else
        {
            if (forNode.Parent is Window parentWindow && parentWindow.HasThemeOwnerNode)
            {
                PropagateThemeChanged(forNode, parentWindow.ThemeOwnerNode, false, true);
            }
        }
    }

    public static void GetThemeTypeDependencies(Node forNode, string themeType, out List<string> list)
    {
        list = new();

        var forC = forNode as Control;
        var forW = forNode as Window;

        if (ERR_FAIL_COND_MSG(forC == null && forW == null, "Only Control and Window nodes and derivatives can be polled for theming."))
        {
            return;
        }

        var defaultTheme = ThemeDB.Singleton.DefaultTheme!;
        var projectTheme = ThemeDB.Singleton.ProjectTheme;

        var typeVariation = default(string);

        if (forC != null)
        {
            typeVariation = forC.GetThemeTypeVariation();
        }
        else if (forW != null)
        {
            typeVariation = forW.GetThemeTypeVariation();
        }

        var className = forNode.GetType().Name;

        if (themeType == "" || themeType == className || themeType == typeVariation)
        {
            if (projectTheme != null && projectTheme.GetTypeVariationBase(typeVariation ?? "") != default)
            {
                projectTheme.GetTypeDependencies(className, typeVariation, list);
            }
            else
            {
                defaultTheme.GetTypeDependencies(className, typeVariation, list);
            }
        }
        else
        {
            defaultTheme.GetTypeDependencies(themeType, default, list);
        }
    }

    public static void PropagateThemeChanged(Node toNode, Node? ownerNode, bool notify, bool assign)
    {
        var c = toNode as Control;
        var w = c == null ? toNode as Window : null;

        if (c == null && w == null)
        {
            // Theme inheritance chains are broken by nodes that aren't Control or Window.
            return;
        }

        if (c != null)
        {
            if (c != ownerNode && c.Theme != null)
            {
                // Has a theme, so we don't want to change the theme owner,
                // but we still want to propagate in case this child has theme items
                // it inherits from the theme this node uses.
                // See https://github.com/godotengine/godot/issues/62844.
                assign = false;
            }

            if (assign)
            {
                c.ThemeOwnerNode = ownerNode;
            }

            if (notify)
            {
                c.Notification(NotificationKind.CONTROL_NOTIFICATION_THEME_CHANGED);
            }
        }
        else if (w != null)
        {
            if (w != ownerNode && w.Theme != null)
            {
                // Same as above.
                assign = false;
            }

            if (assign)
            {
                w.ThemeOwnerNode = ownerNode;
            }

            if (notify)
            {
                w.Notification(NotificationKind.WINDOW_NOTIFICATION_THEME_CHANGED);
            }
        }

        for (var i = 0; i < toNode.GetChildCount(); i++)
        {
            PropagateThemeChanged(toNode.GetChild(i)!, ownerNode, notify, assign);
        }
    }

    public void ClearThemeOnUnparented(Node forNode) => throw new NotImplementedException();

    public float GetThemeDefaultBaseScale()
    {
        // First, look through each control or window node in the branch, until no valid parent can be found.
        // Only nodes with a theme resource attached are considered.
        // For each theme resource see if their assigned theme has the default value defined and valid.
        var ownerNode = this.OwnerNode;

        while (ownerNode != null)
        {
            var ownerTheme = GetOwnerNodeTheme(ownerNode);

            if (ownerTheme?.HasDefaultBaseScale ?? false)
            {
                return ownerTheme.DefaultBaseScale;
            }

            ownerNode = this.GetNextOwnerNode(ownerNode);
        }

        // Secondly, check the project-defined Theme resource.
        if (ThemeDB.Singleton.ProjectTheme != null)
        {
            if (ThemeDB.Singleton.ProjectTheme.HasDefaultBaseScale)
            {
                return ThemeDB.Singleton.ProjectTheme.DefaultBaseScale;
            }
        }

        // Lastly, fall back on the default Theme.
        return ThemeDB.Singleton.DefaultTheme?.HasDefaultBaseScale ?? false
            ? ThemeDB.Singleton.DefaultTheme.DefaultBaseScale
            : ThemeDB.Singleton.FallbackBaseScale;
    }

    public object? GetThemeItemInTypes(ThemeDataType dataType, string name, List<string> themeTypes)
    {
        if (ERR_FAIL_COND_V_MSG(themeTypes.Count == 0, "At least one theme type must be specified."))
        {
            return null;
        }

        // First, look through each control or window node in the branch, until no valid parent can be found.
        // Only nodes with a theme resource attached are considered.
        var ownerNode = this.OwnerNode;

        while (ownerNode != null)
        {
            // For each theme resource check the theme types provided and see if p_name exists with any of them.
            foreach (var themeType in themeTypes)
            {
                var ownerTheme = GetOwnerNodeTheme(ownerNode);

                if (ownerTheme != null && ownerTheme.HasThemeItem(dataType, name, themeType))
                {
                    return ownerTheme.GetThemeItem(dataType, name, themeType);
                }
            }

            ownerNode = this.GetNextOwnerNode(ownerNode);
        }

        // Secondly, check the project-defined Theme resource.
        if (ThemeDB.Singleton.ProjectTheme != null)
        {
            foreach (var themeType in themeTypes)
            {
                if (ThemeDB.Singleton.ProjectTheme.HasThemeItem(dataType, name, themeType))
                {
                    return ThemeDB.Singleton.ProjectTheme.GetThemeItem(dataType, name, themeType);
                }
            }
        }

        ASSERT_NOT_NULL(ThemeDB.Singleton.DefaultTheme);

        // Lastly, fall back on the items defined in the default Theme, if they exist.
        foreach (var themeType in themeTypes)
        {
            if (ThemeDB.Singleton.DefaultTheme.HasThemeItem(dataType, name, themeType))
            {
                return ThemeDB.Singleton.DefaultTheme.GetThemeItem(dataType, name, themeType);
            }
        }

        // If they don't exist, use any type to return the default/empty value.
        return ThemeDB.Singleton.DefaultTheme.GetThemeItem(dataType, name, themeTypes[0]);
    }



}
