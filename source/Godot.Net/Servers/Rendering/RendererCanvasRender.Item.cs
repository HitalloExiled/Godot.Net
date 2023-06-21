namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;

public abstract partial class RendererCanvasRender
{
    public partial record Item
    {
        public virtual Rect2<RealT> Rect { get; set; }

        public bool                       Behind            { get; set; }
        public CanvasGroupRecord?         CanvasGroup       { get; set; }
        public Item?                      CanvasGroupOwner  { get; set; }
        public bool                       Clip              { get; set; }
        public Command?                   Commands          { get; set; }
        public CopyBackBufferRecord?      CopyBackBuffer    { get; set; }
        public bool                       CustomRect        { get; set; }
        public bool                       DistanceField     { get; set; }
        public Item?                      FinalClipOwner    { get; set; }
        public Rect2<RealT>               FinalClipRect     { get; set; }
        public Color                      FinalModulate     { get; set; }
        public Transform2D<RealT>         FinalTransform    { get; set; } = new();
        public Rect2<RealT>               GlobalRectCache   { get; set; }
        public int                        LightMask         { get; set; }
        public bool                       LightMasked       { get; set; }
        public Guid                       Material          { get; set; }
        public Item?                      MaterialOwner     { get; set; }
        public Item?                      Next              { get; set; }
        public bool                       RectDirty         { get; set; }
        public Guid                       Skeleton          { get; set; }
        public RS.CanvasItemTextureFilter TextureFilter     { get; set; }
        public RS.CanvasItemTextureRepeat TextureRepeat     { get; set; }
        public bool                       UpdateWhenVisible { get; set; }
        public bool                       Visible           { get; set; }
        public ViewportRenderRecord?      VpRender          { get; set; }
        public Transform2D<RealT>         Xform             { get; set; } = new();
        public int                        ZFinal            { get; set; }

        public Item()
        {
            this.Commands          = null;
            this.LightMask         = 1;
            this.VpRender          = null;
            this.Next              = null;
            this.FinalClipOwner    = null;
            this.CanvasGroupOwner  = null;
            this.Clip              = false;
            this.FinalModulate     = new(1, 1, 1, 1);
            this.Visible           = true;
            this.RectDirty         = true;
            this.CustomRect        = false;
            this.Behind            = false;
            this.MaterialOwner     = null;
            this.CopyBackBuffer    = null;
            this.DistanceField     = false;
            this.LightMasked       = false;
            this.UpdateWhenVisible = false;
            this.ZFinal            = 0;
            this.TextureFilter     = RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_DEFAULT;
            this.TextureRepeat     = RS.CanvasItemTextureRepeat.CANVAS_ITEM_TEXTURE_REPEAT_DEFAULT;
        }

        public void Clear()
        {
            this.Commands       = null;
            this.Clip           = false;
            this.RectDirty      = true;
            this.FinalClipOwner = null;
            this.MaterialOwner  = null;
            this.LightMasked    = false;
        }
    }
}
