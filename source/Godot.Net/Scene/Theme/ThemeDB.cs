namespace Godot.Net.Scene.Theme;

using Godot.Net.Core.Config;
using Godot.Net.Core.IO;
using Godot.Net.Scene.Resources;
using Godot.Net.Servers;
using ResourcesDefaultTheme = Resources.DefaultTheme.DefaultTheme;
using Theme = Resources.Theme;

#pragma warning disable IDE0044, IDE0052, CS0414 // TODO Remove

public class ThemeDB
{
    public event Action? FallbackChanged;

    private static ThemeDB? singleton;

    private float      fallbackBaseScale;
    private Font?      fallbackFont;
    private int        fallbackFontSize;
    private Texture2D? fallbackIcon;
    private StyleBox?  fallbackStylebox;

    public static ThemeDB Singleton => singleton ?? throw new NullReferenceException();

    public Theme? DefaultTheme { get; set; }

    public float FallbackBaseScale
    {
        get => this.fallbackBaseScale;
        set
        {
            if (this.fallbackBaseScale == value)
            {
                return;
            }

            this.fallbackBaseScale = value;
            FallbackChanged?.Invoke();
        }
    }

    public Font? FallbackFont
    {
        get => this.fallbackFont;
        set
        {
            if (this.fallbackFont == value)
            {
                return;
            }

            this.fallbackFont = value;
            FallbackChanged?.Invoke();
        }
    }

    public int FallbackFontSize
    {
        get => this.fallbackFontSize;
        set
        {
            if (this.fallbackFontSize == value)
            {
                return;
            }

            this.fallbackFontSize = value;
            FallbackChanged?.Invoke();
        }
    }

    public Texture2D? FallbackIcon
    {
        get => this.fallbackIcon;
        set
        {
            if (this.fallbackIcon == value)
            {
                return;
            }

            this.fallbackIcon = value;
            FallbackChanged?.Invoke();
        }
    }

    public StyleBox? FallbackStyleBox
    {
        get => this.fallbackStylebox;
        set
        {
            if (this.fallbackStylebox == value)
            {
                return;
            }

            this.fallbackStylebox = value;
            FallbackChanged?.Invoke();
        }
    }

    public Theme? ProjectTheme { get; set; }

    public ThemeDB()
    {
        singleton = this;

        // Universal default values, final fallback for every theme.
        this.FallbackBaseScale = 1.0f;
        this.FallbackFontSize  = 16;
    }

    public void InitializeTheme()
    {
        // Allow creating the default theme at a different scale to suit higher/lower base resolutions.
        var defaultThemeScale = GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "gui/theme/default_theme_scale", PropertyHint.PROPERTY_HINT_RANGE, "0.5,8,0.01", PropertyUsageFlags.PROPERTY_USAGE_DEFAULT | PropertyUsageFlags.PROPERTY_USAGE_RESTART_IF_CHANGED), 1.0f);

        var themePath = GLOBAL_DEF_RST(new PropertyInfo(VariantType.STRING, "gui/theme/custom", PropertyHint.PROPERTY_HINT_FILE, "*.tres,*.res,*.theme", PropertyUsageFlags.PROPERTY_USAGE_DEFAULT | PropertyUsageFlags.PROPERTY_USAGE_RESTART_IF_CHANGED), "");

        var fontPath = GLOBAL_DEF_RST(new PropertyInfo(VariantType.STRING, "gui/theme/custom_font", PropertyHint.PROPERTY_HINT_FILE, "*.tres,*.res,*.otf,*.ttf,*.woff,*.woff2,*.fnt,*.font,*.pfb,*.pfm", PropertyUsageFlags.PROPERTY_USAGE_DEFAULT | PropertyUsageFlags.PROPERTY_USAGE_RESTART_IF_CHANGED), "");

        var fontAntialiasing = (TextServer.FontAntialiasing)GLOBAL_DEF_RST(new PropertyInfo(VariantType.INT, "guiThemeDefaultFontAntialiasing", PropertyHint.PROPERTY_HINT_ENUM, "None,Grayscale,LCD Subpixel", PropertyUsageFlags.PROPERTY_USAGE_DEFAULT | PropertyUsageFlags.PROPERTY_USAGE_RESTART_IF_CHANGED), 1);

        var fontHinting = GLOBAL_DEF_RST(new PropertyInfo(VariantType.INT, "guiThemeDefaultFontHinting", PropertyHint.PROPERTY_HINT_ENUM, "None,Light,Normal", PropertyUsageFlags.PROPERTY_USAGE_DEFAULT | PropertyUsageFlags.PROPERTY_USAGE_RESTART_IF_CHANGED), TextServer.Hinting.HINTING_LIGHT);

        var fontSubpixelPositioning = GLOBAL_DEF_RST(new PropertyInfo(VariantType.INT, "guiThemeDefaultFontSubpixelPositioning", PropertyHint.PROPERTY_HINT_ENUM, "Disabled,Auto,One Half of a Pixel,One Quarter of a Pixel", PropertyUsageFlags.PROPERTY_USAGE_DEFAULT | PropertyUsageFlags.PROPERTY_USAGE_RESTART_IF_CHANGED), TextServer.SubpixelPositioning.SUBPIXEL_POSITIONING_AUTO);

        var fontMsdf = GLOBAL_DEF_RST("guiThemeDefaultFontMultichannelSignedDistanceField", false);
        var fontGenerateMipmaps = GLOBAL_DEF_RST("guiThemeDefaultFontGenerateMipmaps", false);

        GLOBAL_DEF_RST(new PropertyInfo(VariantType.INT, "gui/theme/lcd_subpixel_layout", PropertyHint.PROPERTY_HINT_ENUM, "Disabled,Horizontal RGB,Horizontal BGR,Vertical RGB,Vertical BGR"), 1);
        ProjectSettings.Singleton.SetRestartIfChanged("gui/theme/lcd_subpixel_layout", false);

        Font? font = null;
        if (!string.IsNullOrEmpty(fontPath))
        {
            font = ResourceLoader.Load(fontPath!) as Font;
            if (font != null)
            {
                this.FallbackFont = font;
            }
            else
            {
                ERR_PRINT("Error loading custom font '" + fontPath + "'");
            }
        }

        // Always make the default theme to avoid invalid default font/icon/style in the given theme.
        if (RS.Singleton != null)
        {
            ResourcesDefaultTheme.MakeDefaultTheme(
                defaultThemeScale,
                font,
                fontSubpixelPositioning,
                fontHinting,
                fontAntialiasing,
                fontMsdf,
                fontGenerateMipmaps
            );
        }

        if (!string.IsNullOrEmpty(themePath))
        {
            if (ResourceLoader.Load(themePath) is Theme theme)
            {
                this.ProjectTheme = theme;
            }
            else
            {
                ERR_PRINT("Error loading custom theme '" + themePath + "'");
            }
        }
    }
}
