namespace Godot.Net.Drivers.GLES3;

using Godot.Net.Core.IO;
using Godot.Net.Drivers.GLES3.Api.Enums;
using Godot.Net.Drivers.GLES3.Storage;

public partial record Texture
{
    private RS.CanvasItemTextureFilter stateFilter = RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_LINEAR;
    private RS.CanvasItemTextureRepeat stateRepeat = RS.CanvasItemTextureRepeat.CANVAS_ITEM_TEXTURE_REPEAT_DISABLED;

    #region public properties
    public bool                               Active                    { get; set; }
    public int                                AllocHeight               { get; set; }
    public int                                AllocWidth                { get; set; }
    public CanvasTexture?                     CanvasTexture             { get; set; }
    public bool                               Compressed                { get; set; }
    public RS.TextureDetectCallback?          Detect3DCallback          { get; set; }
    public object?                            Detect3DCallbackUd        { get; set; }
    public RS.TextureDetectCallback?          DetectNormalCallback      { get; set; }
    public object?                            DetectNormalCallbackUd    { get; set; }
    public RS.TextureDetectRoughnessCallback? DetectRoughnessCallback   { get; set; }
    public object?                            DetectRoughnessCallbackUd { get; set; }
    public ImageFormat                        Format                    { get; set; } = ImageFormat.FORMAT_R8;
    public PixelFormat                        FormatCache               { get; set; }
    public PixelFormat                        GlFormatCache             { get; set; }
    public InternalFormat                     GlInternalFormatCache     { get; set; }
    public PixelType                          GlTypeCache               { get; set; }
    public int                                Height                    { get; set; }
    public Guid                               Id                        { get; set; }
    public InternalFormat                     InternalFormatCache       { get; set; }
    public bool                               IsExternal                { get; set; }
    public bool                               IsProxy                   { get; set; }
    public bool                               IsRenderTarget            { get; set; }
    public RS.TextureLayeredType              LayeredType               { get; set; } = RS.TextureLayeredType.TEXTURE_LAYERED_2D_ARRAY;
    public uint                               Layers                    { get; set; } = 1;
    public int                                Mipmaps                   { get; set; } = 1;
    public string?                            Path                      { get; set; }
    public List<Guid>                         Proxies                   { get; } = new();
    public Guid                               ProxyTo                   { get; set; }
    public ImageFormat                        RealFormat                { get; set; } = ImageFormat.FORMAT_R8;
    public bool                               RedrawIfVisible           { get; set; }
    public RenderTarget                       RenderTarget              { get; set; } = null!;
    public bool                               ResizeToPo2               { get; set; }
    public ulong                              StoredCubeSides           { get; set; }
    public TextureTarget                      Target                    { get; set; }
    public uint                               TexId                     { get; set; }
    public int                                TotalDataSize             { get; set; }
    public TextureType                        Type                      { get; set; }
    public PixelType                          TypeCache                 { get; set; }
    public int                                Width                     { get; set; }
    #endregion public properties

    public void CopyFrom(Texture o)
    {
        this.ProxyTo                   = o.ProxyTo;
        this.IsProxy                   = o.IsProxy;
        this.IsExternal                = o.IsExternal;
        this.Width                     = o.Width;
        this.Height                    = o.Height;
        this.AllocWidth                = o.AllocWidth;
        this.AllocHeight               = o.AllocHeight;
        this.Format                    = o.Format;
        this.Type                      = o.Type;
        this.LayeredType               = o.LayeredType;
        this.Target                    = o.Target;
        this.TotalDataSize             = o.TotalDataSize;
        this.Compressed                = o.Compressed;
        this.Mipmaps                   = o.Mipmaps;
        this.ResizeToPo2               = o.ResizeToPo2;
        this.Active                    = o.Active;
        this.TexId                     = o.TexId;
        this.StoredCubeSides           = o.StoredCubeSides;
        this.RenderTarget              = o.RenderTarget;
        this.IsRenderTarget            = o.IsRenderTarget;
        this.RedrawIfVisible           = o.RedrawIfVisible;
        this.Detect3DCallback          = o.Detect3DCallback;
        this.Detect3DCallbackUd        = o.Detect3DCallbackUd;
        this.DetectNormalCallback      = o.DetectNormalCallback;
        this.DetectNormalCallbackUd    = o.DetectNormalCallbackUd;
        this.DetectRoughnessCallback   = o.DetectRoughnessCallback;
        this.DetectRoughnessCallbackUd = o.DetectRoughnessCallbackUd;
    }

    public void GlSetFilter(RS.CanvasItemTextureFilter filter)
    {
        var gl = GL.Singleton;

        if (filter == this.stateFilter)
        {
            return;
        }

        var config = Config.Singleton;

        this.stateFilter = filter;

        var pmin          = TextureMinFilter.Nearest; // param min
        var pmag          = TextureMagFilter.Nearest; // param mag
        var maxLod        = 1000;
        var useAnisotropy = false;

        switch (this.stateFilter)
        {
            case RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_NEAREST:
                pmin   = TextureMinFilter.Nearest;
                pmag   = TextureMagFilter.Nearest;
                maxLod = 0;

                break;
            case RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_LINEAR:
                pmin   = TextureMinFilter.Linear;
                pmag   = TextureMagFilter.Linear;
                maxLod = 0;

                break;
            case RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_NEAREST_WITH_MIPMAPS_ANISOTROPIC:
                useAnisotropy = true;

                goto case RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_NEAREST_WITH_MIPMAPS;
            case RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_NEAREST_WITH_MIPMAPS:
                pmag = TextureMagFilter.Nearest;

                if (this.Mipmaps <= 1)
                {
                    pmin   = TextureMinFilter.Nearest;
                    maxLod = 0;
                }
                else
                {
                    pmin = config.UseNearestMifilter ? TextureMinFilter.NearestMipmapNearest : TextureMinFilter.NearestMipmapLinear;
                }

                break;
            case RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_LINEAR_WITH_MIPMAPS_ANISOTROPIC:
                useAnisotropy = true;

                goto case RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_LINEAR_WITH_MIPMAPS;
            case RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_LINEAR_WITH_MIPMAPS:
                pmag = TextureMagFilter.Linear;

                if (this.Mipmaps <= 1)
                {
                    pmin   = TextureMinFilter.Linear;
                    maxLod = 0;
                }
                else
                {
                    pmin = config.UseNearestMifilter ? TextureMinFilter.LinearMipmapNearest : TextureMinFilter.LinearMipmapLinear;
                }

                break;
            default:
                break;
        }

        gl.TexParameterI(this.Target, TextureParameterName.TextureMinFilter, (int)pmin);
        gl.TexParameterI(this.Target, TextureParameterName.TextureMagFilter, (int)pmag);
        gl.TexParameterI(this.Target, TextureParameterName.TextureBaseLevel, 0);
        gl.TexParameterI(this.Target, TextureParameterName.TextureMaxLevel, maxLod);

        if (config.SupportAnisotropicFilter && useAnisotropy)
        {
            gl.TexParameterI(this.Target, TextureParameterName.TextureMaxAnisotropy, (int)config.AnisotropicLevel);
        }
    }

    public void GlSetRepeat(RS.CanvasItemTextureRepeat repeat)
    {
        if (repeat == this.stateRepeat)
        {
            return;
        }

        this.stateRepeat = repeat;

        var prep = TextureWrapMode.ClampToEdge; // parameter repeat

        switch (this.stateRepeat)
        {
            case RS.CanvasItemTextureRepeat.CANVAS_ITEM_TEXTURE_REPEAT_ENABLED:
                prep = TextureWrapMode.Repeat;

                break;
            case RS.CanvasItemTextureRepeat.CANVAS_ITEM_TEXTURE_REPEAT_MIRROR:
                prep = TextureWrapMode.MirroredRepeat;

                break;
            default:
                break;
        }

        var gl = GL.Singleton;

        gl.TexParameterI(this.Target, TextureParameterName.TextureWrapT, (int)prep);
        gl.TexParameterI(this.Target, TextureParameterName.TextureWrapR, (int)prep);
        gl.TexParameterI(this.Target, TextureParameterName.TextureWrapS, (int)prep);
    }
}
