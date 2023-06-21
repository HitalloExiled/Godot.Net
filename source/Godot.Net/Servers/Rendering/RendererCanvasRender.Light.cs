namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Math;

public abstract partial class RendererCanvasRender
{
    public record Light
    {
        public RS.CanvasLightBlendMode    BlendMode           { get; set; }
        public Guid                       Canvas              { get; set; }
        public Color                      Color               { get; set; }
        public float                      DirectionalDistance { get; set; }
        public bool                       Enabled             { get; set; }
        public float                      Energy              { get; set; }
        public float                      Height              { get; set; }
        public int                        ItemMask            { get; set; }
        public int                        ItemShadowMask      { get; set; }
        public int                        LayerMax            { get; set; }
        public int                        LayerMin            { get; set; }
        public RS.CanvasLightMode         Mode                { get; set; }
        public float                      Scale               { get; set; }
        public int                        ShadowBufferSize    { get; set; }
        public Color                      ShadowColor         { get; set; }
        public RS.CanvasLightShadowFilter ShadowFilter        { get; set; }
        public float                      ShadowSmooth        { get; set; }
        public Guid                       Texture             { get; set; }
        public Vector2<RealT>             TextureOffset       { get; set; }
        public bool                       UseShadow           { get; set; }
        public Transform2D<RealT>         Xform               { get; set; } = new();
        public int                        ZMax                { get; set; }
        public int                        ZMin                { get; set; }

        //void *texture_cache; // implementation dependent
        public float              RadiusCache { get; set; } //used for shadow far plane
        public Rect2<RealT>       RectCache   { get; set; }
        public Transform2D<RealT> XformCache  { get; set; } = new();
        //Projection shadow_matrix_cache;

        public Transform2D<RealT> LightShaderXform { get; set; } = new();
        //Vector2 light_shader_pos;

        public Light? DirectionalNextPtr { get; set; }
        public Light? FilterNextPtr      { get; set; }
        public Guid   LightInternal      { get; set; }
        public Light? Next               { get; set; }
        public int    RenderIndexCache   { get; set; }
        public Light? ShadowsNextPtr     { get; set; }
        public long   Version            { get; set; }

        public Light()
        {
            this.Version        = 0;
            this.Enabled        = true;
            this.Color          = new(1, 1, 1);
            this.ShadowColor    = new(0, 0, 0, 0);
            this.Height         = 0;
            this.ZMin           = -1024;
            this.ZMax           = 1024;
            this.LayerMin       = 0;
            this.LayerMax       = 0;
            this.ItemMask       = 1;
            this.Scale          = 1.0f;
            this.Energy         = 1.0f;
            this.ItemShadowMask = 1;
            this.Mode           = RS.CanvasLightMode.CANVAS_LIGHT_MODE_POINT;
            this.BlendMode      = RS.CanvasLightBlendMode.CANVAS_LIGHT_BLEND_MODE_ADD;
            // texture_cache = nullptr;
            this.Next             = null;
            this.DirectionalNextPtr  = null;
            this.FilterNextPtr       = null;
            this.UseShadow           = false;
            this.ShadowBufferSize    = 2048;
            this.ShadowFilter        = RS.CanvasLightShadowFilter.CANVAS_LIGHT_FILTER_NONE;
            this.ShadowSmooth        = 0.0f;
            this.RenderIndexCache    = -1;
            this.DirectionalDistance = 10000.0f;
        }
    }
}
