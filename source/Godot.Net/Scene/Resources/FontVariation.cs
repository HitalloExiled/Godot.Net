namespace Godot.Net.Scene.Resources;

using Godot.Net.Attributes;
using Godot.Net.Core.Math;
using Godot.Net.Servers;

#pragma warning disable CS0414, IDE0052 // TODO Remove

public partial class FontVariation : Font
{
    [Clonable]
    private readonly Variation variation    = new();
    private readonly int[]     extraSpacing = new int[(int)TextServer.SpacingType.SPACING_MAX];

    private Font? baseFont;

    public Font? BaseFont
    {
        get => this.baseFont;
        set
        {
            if (this.baseFont != value)
            {
                if (this.baseFont != null)
                {
                    this.baseFont.Changed -= this.InvalidateIds;
                }
                this.baseFont = value;

                if (this.baseFont != null)
                {
                    this.baseFont.Changed += this.InvalidateIds;
                }

                this.InvalidateIds();
                this.NotifyPropertyListChanged();
            }
        }
    }

    public float VariationEmbolden
    {
        get => this.variation.Embolden;
        set
        {
            if (this.variation.Embolden != value)
            {
                this.variation.Embolden = value;
                this.InvalidateIds();
            }
        }
    }

    public Transform2D<RealT> VariationTransform
    {
        get => this.variation.Transform;
        set
        {
            if (this.variation.Transform != value)
            {
                this.variation.Transform = value;
                this.InvalidateIds();
            }
        }
    }

    public virtual void SetSpacing(TextServer.SpacingType spacing, int value)
    {
        if (ERR_FAIL_INDEX(spacing, TextServer.SpacingType.SPACING_MAX))
        {
            return;
        }

        if (this.extraSpacing[(int)spacing] != value)
        {
            this.extraSpacing[(int)spacing] = value;
            this.InvalidateIds();
        }
    }

    public override int GetSpacing(TextServer.SpacingType spacing) =>
        ERR_FAIL_INDEX_V(spacing, TextServer.SpacingType.SPACING_MAX) ? 0 : this.extraSpacing[(int)spacing];
}
