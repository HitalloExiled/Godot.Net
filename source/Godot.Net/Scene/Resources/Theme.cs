namespace Godot.Net.Scene.Resources;

using Godot.Net.Core.IO;
using Godot.Net.Core.Math;
using Godot.Net.Core.Object;
using Godot.Net.Extensions;
using Godot.Net.Scene.Theme;

using ThemeColorMap    = Dictionary<string, Core.Math.Color>;
using ThemeConstantMap = Dictionary<string, int>;
using ThemeFontMap     = Dictionary<string, Font>;
using ThemeFontSizeMap = Dictionary<string, int>;
using ThemeIconMap     = Dictionary<string, Texture2D>;
using ThemeStyleMap    = Dictionary<string, StyleBox>;

#pragma warning disable CS0649, IDE0044 // TODO Remove

public partial class Theme : Resource
{
    private readonly Dictionary<string, ThemeColorMap>    colorMap         = new();
    private readonly Dictionary<string, ThemeConstantMap> constantMap      = new();
    private readonly Dictionary<string, ThemeFontMap>     fontMap          = new();
    private readonly Dictionary<string, ThemeFontSizeMap> fontSizeMap      = new();
    private readonly Dictionary<string, ThemeIconMap>     iconMap          = new();
    private readonly Dictionary<string, ThemeStyleMap>    styleMap         = new();
    private readonly Dictionary<string, List<string>>     variationBaseMap = new();
    private readonly Dictionary<string, string>           variationMap     = new();

    private float defaultBaseScale;
    private Font? defaultFont;
    private int   defaultFontSize;
    private bool  noChangePropagation;

    public float DefaultBaseScale
    {
        get => this.defaultBaseScale;
        set
        {
            if (this.defaultBaseScale == value)
            {
                return;
            }

            this.defaultBaseScale = value;

            this.EmitThemeChanged();
        }
    }

    public Font? DefaultFont
    {
        get => this.defaultFont;
        set
        {
            if (this.defaultFont == value)
            {
                return;
            }

            if (this.defaultFont != null)
            {
                this.defaultFont.Changed -= this.EmitThemeChangedWithoutNotifyListChanged;
            }

            this.defaultFont = value;

            if (this.defaultFont != null)
            {
                this.defaultFont.Changed += this.EmitThemeChangedWithoutNotifyListChanged;
            }

            this.EmitThemeChanged();
        }
    }

    public int DefaultFontSize
    {
        get => this.defaultFontSize;
        set
        {
            if (this.defaultFontSize == value)
            {
                return;
            }

            this.defaultFontSize = value;

            this.EmitThemeChanged();
        }
    }

    public bool HasDefaultBaseScale => this.DefaultBaseScale > 0;
    public bool HasDefaultFont      => this.defaultFont != null;
    public bool HasDefaultFontSize  => this.defaultFontSize > 0;

    private void EmitThemeChanged(bool notifyListChanged = false)
    {
        if (this.noChangePropagation)
        {
            return;
        }

        if (notifyListChanged)
        {
            this.NotifyPropertyListChanged();
        }

        this.EmitChanged();
    }

    private void EmitThemeChangedWithoutNotifyListChanged() => this.EmitThemeChanged(false);

    public static bool IsValidItemName(string? name) =>
        name != null && name.ToCharArray().All(x => x.IsAsciiIdentifierChar());

    public static bool IsValidTypeName(string name) =>
        name.ToCharArray().All(x => x.IsAsciiIdentifierChar());

    public Color GetColor(string name, string themeType) =>
        this.colorMap.TryGetValue(themeType, out var entry) && entry.TryGetValue(name, out var value) ? value : default;

    public int GetConstant(string name, string themeType) =>
        this.constantMap.TryGetValue(themeType, out var entry) && entry.TryGetValue(name, out var value) ? value : default;

    public Font? GetFont(string name, string themeType) =>
        this.fontMap.TryGetValue(themeType, out var entry) && entry.TryGetValue(name, out var value)
            ? value
            : this.defaultFont ?? ThemeDB.Singleton.FallbackFont;

    public int GetFontSize(string name, string themeType) =>
        this.fontSizeMap.TryGetValue(themeType, out var entry) && entry.TryGetValue(name, out var value) && value > 0
            ? value
            : this.HasDefaultFontSize
                ? this.defaultFontSize
                : ThemeDB.Singleton.FallbackFontSize;

    public Texture2D? GetIcon(string name, string themeType) =>
        this.iconMap.TryGetValue(themeType, out var entry) && entry.TryGetValue(name, out var value)
            ? value
            : ThemeDB.Singleton.FallbackIcon;

    public StyleBox? GetStyleBox(string name, string themeType) =>
        this.styleMap.TryGetValue(themeType, out var entry) && entry.TryGetValue(name, out var value)
            ? value
            : ThemeDB.Singleton.FallbackStyleBox;

    public object? GetThemeItem(DataType dataType, string name, string themeType)
    {
        switch (dataType)
        {
            case DataType.DATA_TYPE_COLOR:
                return this.GetColor(name, themeType);
            case DataType.DATA_TYPE_CONSTANT:
                return this.GetConstant(name, themeType);
            case DataType.DATA_TYPE_FONT:
                return this.GetFont(name, themeType);
            case DataType.DATA_TYPE_FONT_SIZE:
                return this.GetFontSize(name, themeType);
            case DataType.DATA_TYPE_ICON:
                return this.GetIcon(name, themeType);
            case DataType.DATA_TYPE_STYLEBOX:
                return this.GetStyleBox(name, themeType);
            case DataType.DATA_TYPE_MAX:
                break; // Can't happen, but silences warning.
        }

        return null;
    }

    public void GetTypeDependencies(string baseType, string? typeVariation, List<string>? list)
    {
        if (ERR_FAIL_NULL(list))
        {
            return;
        }

        // Build the dependency chain for type variations.
        if (typeVariation != null)
        {
            var variationName = typeVariation;
            while (variationName != null)
            {
                list!.Add(variationName);
                variationName = this.GetTypeVariationBase(variationName);

                // If we have reached the base type dependency, it's safe to stop (assuming no funny business was done to the Theme).
                if (variationName == baseType)
                {
                    break;
                }
            }
        }

        // Continue building the chain using native class hierarchy.
        var className = baseType;
        while (className != null)
        {
            list!.Add(className);
            className = ClassDB.GetType(className)?.BaseType?.Name;
        }
    }

    public string? GetTypeVariationBase(string themeType) =>
        this.variationMap.TryGetValue(themeType, out var value) ? value : default;

    public bool HasColor(string name, string themeType) =>
        this.colorMap.TryGetValue(themeType, out var entry) && entry.ContainsKey(name);

    public bool HasColorNocheck(string name, string themeType) =>
        this.colorMap.TryGetValue(themeType, out var entry) && entry.ContainsKey(name);

    public bool HasConstant(string name, string themeType) =>
        this.constantMap.TryGetValue(themeType, out var entry) && entry.ContainsKey(name);

    public bool HasConstantNocheck(string name, string themeType) =>
        this.constantMap.TryGetValue(themeType, out var entry) && entry.ContainsKey(name);

    public bool HasFont(string name, string themeType) =>
	    this.fontMap.TryGetValue(themeType, out var entry) && entry.ContainsKey(name) || this.HasDefaultFont;

    public bool HasFontSize(string name, string themeType) =>
	    this.fontSizeMap.TryGetValue(themeType, out var entry) && entry.ContainsKey(name) || this.HasDefaultFontSize;

    public bool HasFontSizeNocheck(string name, string themeType) =>
        this.fontSizeMap.TryGetValue(themeType, out var entry) && entry.ContainsKey(name);

    public bool HasIcon(string name, string themeType) =>
        this.iconMap.TryGetValue(themeType, out var entry) && entry.ContainsKey(name);

    public bool HasStylebox(string name, string themeType) =>
        this.styleMap.TryGetValue(themeType, out var entry) && entry.ContainsKey(name);

    public bool HasThemeItem(DataType dataType, string name, string themeType)
    {
        switch (dataType)
        {
            case DataType.DATA_TYPE_COLOR:
                return this.HasColor(name, themeType);
            case DataType.DATA_TYPE_CONSTANT:
                return this.HasConstant(name, themeType);
            case DataType.DATA_TYPE_FONT:
                return this.HasFont(name, themeType);
            case DataType.DATA_TYPE_FONT_SIZE:
                return this.HasFontSize(name, themeType);
            case DataType.DATA_TYPE_ICON:
                return this.HasIcon(name, themeType);
            case DataType.DATA_TYPE_STYLEBOX:
                return this.HasStylebox(name, themeType);
            case DataType.DATA_TYPE_MAX:
                break; // Can't happen, but silences warning.
        }

        return false;
    }

    public void MergeWith(Theme other) => throw new NotImplementedException();

    public void SetColor(string name, string themeType, in Color color)
    {
        if (ERR_FAIL_COND_MSG(!IsValidItemName(name), $"Invalid item name: '{name}'"))
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(!IsValidTypeName(themeType), $"Invalid type name: '{themeType}'"))
        {
            return;
        }

        var existing = this.HasColorNocheck(name, themeType);

        if (!this.colorMap.TryGetValue(themeType, out var themeColorMap))
        {
            this.colorMap[themeType] = themeColorMap = new();
        }

        themeColorMap[name] = color;

        this.EmitThemeChanged(!existing);
    }

    public void SetConstant(string name, string themeType, int constant)
    {
        if (ERR_FAIL_COND_MSG(!IsValidItemName(name), $"Invalid item name: '{name}'"))
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(!IsValidTypeName(themeType), $"Invalid type name: '{themeType}'"))
        {
            return;
        }

        var existing = this.HasConstantNocheck(name, themeType);

        if (!this.constantMap.TryGetValue(themeType, out var themeFontSizeMap))
        {
            this.constantMap[themeType] = themeFontSizeMap = new();
        }

        themeFontSizeMap[name] = constant;

        this.EmitThemeChanged(!existing);
    }

    public void SetFont(string name, string themeType, Font? font)
    {
        if (ERR_FAIL_COND_MSG(!IsValidItemName(name), $"Invalid item name: '{name}'"))
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(!IsValidTypeName(themeType), $"Invalid type name: '{themeType}'"))
        {
            return;
        }

        if (!this.fontMap.TryGetValue(themeType, out var themeFontMap))
        {
            this.fontMap[themeType] = themeFontMap = new();
        }

        var existing = false;
        if (themeFontMap.TryGetValue(name, out var value))
        {
            existing = true;
            value.Changed -= this.EmitThemeChangedWithoutNotifyListChanged;
        }


        if (font != null)
        {
            themeFontMap[name] = font;
            font.Changed += this.EmitThemeChangedWithoutNotifyListChanged;
        }
        else
        {
            themeFontMap.Remove(name);
        }

        this.EmitThemeChanged(!existing);
    }

    public void SetFontSize(string name, string themeType, int fontSize)
    {
        if (ERR_FAIL_COND_MSG(!IsValidItemName(name), $"Invalid item name: '{name}'"))
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(!IsValidTypeName(themeType), $"Invalid type name: '{themeType}'"))
        {
            return;
        }

        var existing = this.HasFontSizeNocheck(name, themeType);

        if (!this.fontSizeMap.TryGetValue(themeType, out var themeFontSizeMap))
        {
            this.fontSizeMap[themeType] = themeFontSizeMap = new();
        }

        themeFontSizeMap[name] = fontSize;

        this.EmitThemeChanged(!existing);
    }

    public void SetIcon(string name, string themeType, Texture2D? icon)
    {
        if (ERR_FAIL_COND_MSG(!IsValidItemName(name), $"Invalid item name: '{name}'"))
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(!IsValidTypeName(themeType), $"Invalid type name: '{themeType}'"))
        {
            return;
        }

        var existing = false;

        if (!this.iconMap.TryGetValue(themeType, out var themeIconMap))
        {
            this.iconMap[themeType] = themeIconMap = new();
        }

        if (themeIconMap.TryGetValue(name, out var value))
        {
            existing = true;
            value.Changed -= this.EmitThemeChangedWithoutNotifyListChanged;
        }

        if (icon != null)
        {
            themeIconMap[name] = icon;
            icon.Changed += this.EmitThemeChangedWithoutNotifyListChanged;
        }

        this.EmitThemeChanged(!existing);
    }

    public void SetStylebox(string name, string themeType, StyleBox? style)
    {
        if (ERR_FAIL_COND_MSG(!IsValidItemName(name), $"Invalid item name: '{name}'"))
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(!IsValidTypeName(themeType), $"Invalid type name: '{themeType}'"))
        {
            return;
        }

        var existing = false;

        if (!this.styleMap.TryGetValue(themeType, out var themeStyleMap))
        {
            this.styleMap[themeType] = themeStyleMap = new();
        }

        if (themeStyleMap.TryGetValue(name, out var value))
        {
            existing = true;
            value.Changed -= this.EmitThemeChangedWithoutNotifyListChanged;
        }

        if (style != null)
        {
            themeStyleMap[name] = style;
            themeStyleMap[name].Changed += this.EmitThemeChangedWithoutNotifyListChanged;
        }
        else
        {
            themeStyleMap.Remove(name);
        }

        this.EmitThemeChanged(!existing);
    }

    public void SetTypeVariation(string themeType, string baseType)
    {
        if (ERR_FAIL_COND_MSG(!IsValidTypeName(themeType), $"Invalid type name: '{themeType}'"))
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(!IsValidTypeName(baseType), $"Invalid type name: '{baseType}'"))
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(themeType == null, "An empty theme type cannot be marked as a variation of another type."))
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(ClassDB.GetType(themeType!) != null, "A type associated with a built-in class cannot be marked as a variation of another type."))
        {
            return;
        }

        if (ERR_FAIL_COND_MSG(baseType == null, $"An empty theme type cannot be the base type of a variation. Use clear_type_variation() instead if you want to unmark '{themeType}' as a variation."))
        {
            return;
        }

        if (this.variationMap.TryGetValue(themeType!, out var value))
        {
            var oldBase = value;
            this.variationBaseMap[oldBase].Remove(themeType!);
        }

        this.variationMap[themeType!] = baseType!;

        if (!this.variationBaseMap.TryGetValue(baseType!, out var list))
        {
            this.variationBaseMap[baseType!] = list = new();
        }

        list.Add(themeType!);

        this.EmitThemeChanged(true);
    }
}
