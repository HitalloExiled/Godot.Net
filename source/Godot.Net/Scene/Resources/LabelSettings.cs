namespace Godot.Net.Scene.Resources;

using Godot.Net.Core.IO;
using Godot.Net.Core.Math;

#pragma warning disable IDE0044, IDE0051, CS0649 // TODO Remove;

public class LabelSettings : Resource
{
    private Font?          font;
    private Color          fontColor    = new(1, 1, 1);
    private int            fontSize     = Font.DEFAULTFONTSIZE;
    private RealT          lineSpacing  = 3;
    private Color          outlineColor = new(1, 1, 1);
    private int            outlineSize;
    private Color          shadowColor  = new(0, 0, 0, 0);
    private Vector2<RealT> shadowOffset = new(1, 1);
    private int            shadowSize   = 1;

    public Font? Font
    {
        get => this.font;
        set => throw new NotImplementedException();
    }
    public Color FontColor
    {
        get => this.fontColor;
        set => throw new NotImplementedException();
    }
    public int FontSize
    {
        get => this.fontSize;
        set => throw new NotImplementedException();
    }
    public RealT LineSpacing
    {
        get => this.lineSpacing;
        set => throw new NotImplementedException();
    }
    public Color OutlineColor
    {
        get => this.outlineColor;
        set => throw new NotImplementedException();
    }
    public int OutlineSize
    {
        get => this.outlineSize;
        set => throw new NotImplementedException();
    }
    public Color ShadowColor
    {
        get => this.shadowColor;
        set => throw new NotImplementedException();
    }
    public Vector2<RealT> ShadowOffset
    {
        get => this.shadowOffset;
        set => throw new NotImplementedException();
    }
    public int ShadowSize
    {
        get => this.shadowSize;
        set => throw new NotImplementedException();
    }

    protected static void BindMethods() => throw new NotImplementedException();

    private void FontChanged() => throw new NotImplementedException();
}
