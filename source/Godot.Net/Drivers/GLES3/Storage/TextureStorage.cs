#define GLES_OVER_GL

namespace Godot.Net.Drivers.GLES3.Storage;

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Godot.Net.Core;
using Godot.Net.Core.IO;
using Godot.Net.Core.Math;
using Godot.Net.Drivers.GLES3.Api.Enums;
using Godot.Net.Drivers.GLES3.Api.Extensions.OVR;

public partial class TextureStorage : Servers.Rendering.Storage.RendererTextureStorage
{
    #region private static readonly fields
    private static readonly TextureTarget[] cubeSideEnum =
    {
        TextureTarget.TextureCubeMapNegativeX,
        TextureTarget.TextureCubeMapPositiveX,
        TextureTarget.TextureCubeMapNegativeY,
        TextureTarget.TextureCubeMapPositiveY,
        TextureTarget.TextureCubeMapNegativeZ,
        TextureTarget.TextureCubeMapPositiveZ,
    };
    #endregion private static readonly fields

    #region private static fields
    private static TextureStorage? singleton;
    #endregion private static fields

    #region public properties
    public static TextureStorage Singleton => singleton ?? throw new NullReferenceException();
    public static uint           SystemFbo { get; private set; }
    #endregion public properties

    #region private readonly fields
    private readonly GuidOwner<CanvasTexture> canvasTextureOwner = new(true);
    private readonly Guid[]                   defaultGlTextures  = new Guid[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_MAX];
    private readonly GuidOwner<RenderTarget>  renderTargetOwner  = new();
    private readonly RenderTargetSDF          sdfShader          = new();
    private readonly TextureAtlas             textureAtlas       = new();
    private readonly GuidOwner<Texture>       textureOwner       = new();
    #endregion private readonly fields

    public TextureStorage()
    {
        singleton = this;

        var gl = GL.Singleton;

        SystemFbo = 0;

        //create default textures
        {
            // White Textures
            {
                var image = Image.CreateEmpty(4, 4, true, ImageFormat.FORMAT_RGBA8);
                image.Fill(new(1, 1, 1, 1));
                image.GenerateMipmaps();

                this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_WHITE] = Guid.NewGuid();
                this.Texture2DInitialize(this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_WHITE], image);

                var images = new List<Image>
                {
                    image
                };

                this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_2D_ARRAY_WHITE] = Guid.NewGuid();
                this.Texture2DLayeredInitialize(this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_2D_ARRAY_WHITE], images, RS.TextureLayeredType.TEXTURE_LAYERED_2D_ARRAY);

                for (var i = 0; i < 3; i++)
                {
                    images.Add(image);
                }

                this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_3D_WHITE] = Guid.NewGuid();
                this.Texture3DInitialize(this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_3D_WHITE], image.Format, 4, 4, 4, false, images);

                for (var i = 0; i < 2; i++)
                {
                    images.Add(image);
                }

                this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_CUBEMAP_WHITE] = Guid.NewGuid();
                this.Texture2DLayeredInitialize(this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_CUBEMAP_WHITE], images, RS.TextureLayeredType.TEXTURE_LAYERED_CUBEMAP);
            }

            // black
            {
                var image = Image.CreateEmpty(4, 4, true, ImageFormat.FORMAT_RGBA8);
                image.Fill(new(0, 0, 0, 1));
                image.GenerateMipmaps();

                this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_BLACK] = Guid.NewGuid();
                this.Texture2DInitialize(this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_BLACK], image);

                var images = new List<Image>();

                for (var i = 0; i < 4; i++)
                {
                    images.Add(image);
                }

                this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_3D_BLACK] = Guid.NewGuid();
                this.Texture3DInitialize(this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_3D_BLACK], image.Format, 4, 4, 4, false, images);

                for (var i = 0; i < 2; i++)
                {
                    images.Add(image);
                }

                this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_CUBEMAP_BLACK] = Guid.NewGuid();
                this.Texture2DLayeredInitialize(this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_CUBEMAP_BLACK], images, RS.TextureLayeredType.TEXTURE_LAYERED_CUBEMAP);
            }

            // transparent black
            {
                var image = Image.CreateEmpty(4, 4, true, ImageFormat.FORMAT_RGBA8);
                image.Fill(new(0, 0, 0, 0));
                image.GenerateMipmaps();

                this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_TRANSPARENT] = Guid.NewGuid();
                this.Texture2DInitialize(this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_TRANSPARENT], image);
            }

            {
                var image = Image.CreateEmpty(4, 4, true, ImageFormat.FORMAT_RGBA8);
                image.Fill(new(0.5f, 0.5f, 1, 1));
                image.GenerateMipmaps();

                this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_NORMAL] = Guid.NewGuid();
                this.Texture2DInitialize(this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_NORMAL], image);
            }

            {
                var image = Image.CreateEmpty(4, 4, true, ImageFormat.FORMAT_RGBA8);
                image.Fill(new(1.0f, 0.5f, 1, 1));
                image.GenerateMipmaps();

                this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_ANISO] = Guid.NewGuid();
                this.Texture2DInitialize(this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_ANISO], image);
            }

            {
                var pixelData = new byte[4 * 4 * 4];

                // for (var i = 0; i < 16; i++)
                // {
                //     pixelData[i * 4 + 0] = 0;
                //     pixelData[i * 4 + 1] = 0;
                //     pixelData[i * 4 + 2] = 0;
                //     pixelData[i * 4 + 3] = 0;
                // }

                this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_2D_UINT] = Guid.NewGuid();
                var texture = new Texture
                {
                    Width  = 4,
                    Height = 4,
                    Format = ImageFormat.FORMAT_RGBA8,
                    Type   = TextureType.T2D,
                    Target = TextureTarget.Texture2D,
                    Active = true
                };

                gl.GenTextures(out var texId);

                texture.TexId = texId;

                this.textureOwner.Initialize(this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_2D_UINT], texture);

                gl.BindTexture(TextureTarget.Texture2D, texture.TexId);
                gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8ui, 4, 4, 0, PixelFormat.RgbaInteger, PixelType.UnsignedByte, pixelData);
                texture.GlSetFilter(RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_NEAREST);
            }

            {
                var pixelData = new byte[4 * 4];

                for (var i = 0; i < 16; i++)
                {
                    pixelData[i] = (byte)Half.One;
                }

                this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_DEPTH] = Guid.NewGuid();

                var texture = new Texture
                {
                    Width  = 4,
                    Height = 4,
                    Format = ImageFormat.FORMAT_RGBA8,
                    Type   = TextureType.T2D,
                    Target = TextureTarget.Texture2D,
                    Active = true
                };

                gl.GenTextures(out var texId);
                texture.TexId = texId;

                this.textureOwner.Initialize(this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_DEPTH], texture);

                gl.BindTexture(TextureTarget.Texture2D, texture.TexId);
                gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.DepthComponent16, 4, 4, 0, PixelFormat.DepthComponent, PixelType.UnsignedShort, pixelData);
                texture.GlSetFilter(RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_NEAREST);
            }
        }

        gl.BindTexture(TextureTarget.Texture2D, 0);

        // Atlas Texture initialize.
        {
            var pixelData = new byte[4 * 4 * 4];

            for (var i = 0; i < 16; i++)
            {
                pixelData[i * 4 + 0] = 0;
                pixelData[i * 4 + 1] = 0;
                pixelData[i * 4 + 2] = 0;
                pixelData[i * 4 + 3] = 255;
            }

            gl.GenTextures(out var texture);
            this.textureAtlas.Texture = texture;

            gl.BindTexture(TextureTarget.Texture2D, this.textureAtlas.Texture);
            gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, 4, 4, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixelData);
        }

        gl.BindTexture(TextureTarget.Texture2D, 0);

        {
            this.sdfShader.Shader.Initialize();
            this.sdfShader.ShaderVersion = this.sdfShader.Shader.VersionCreate();
        }

#if GLES_OVER_GL
        gl.Enable(EnableCap.ProgramPointSize);
#endif
    }

    #region private static methods
    private static void WARN_PRINT(string message) => throw new NotImplementedException();

    private static void ClearRenderTargetOverriddenFboCache(RenderTarget renderTarget)
    {
        var gl = GL.Singleton;
        foreach (var entry in renderTarget.Overridden.FboCache)
        {
            // OpenGL.Singleton.DeleteTextures((uint)entry.Value.AllocatedTextures.Count, entry.Value.AllocatedTextures.ToArray());
            gl.DeleteFramebuffers(new[] { entry.Value.Fbo });
        }
    }

    private static Image? GetGlImageAndFormat(Image? image, ImageFormat format, out ImageFormat realFormat, out PixelFormat glFormat, out InternalFormat glInternalFormat, out PixelType glType, out bool compressed, bool forceDecompress)
    {
        var config = Config.Singleton;

        glFormat         = 0;
        compressed       = false;
        realFormat       = format;
        glInternalFormat = default;
        glFormat         = default;
        glType           = default;

        var needDecompress   = false;
        var decompressRaToRg = false;

        switch (format)
        {
            case ImageFormat.FORMAT_L8:
                #if GLES_OVER_GL
                glInternalFormat = InternalFormat.R8;
                glFormat         = PixelFormat.Red;
                glType           = PixelType.UnsignedByte;
                #else
                glInternalFormat = (InternalFormat)PixelFormat.Luminance;
                glFormat         = PixelFormat.Luminance;
                glType           = PixelType.UnsignedByte;
                #endif

                break;
            case ImageFormat.FORMAT_LA8:
                #if GLES_OVER_GL
                glInternalFormat = InternalFormat.Rg8;
                glFormat         = PixelFormat.Rg;
                glType           = PixelType.UnsignedByte;
                #else
                glInternalFormat = (InternalFormat)PixelFormat.LuminanceAlpha;
                glFormat         = PixelFormat.LuminanceAlpha;
                glType           = PixelType.UnsignedByte;
                #endif

                break;
            case ImageFormat.FORMAT_R8:
                glInternalFormat = InternalFormat.R8;
                glFormat         = PixelFormat.Red;
                glType           = PixelType.UnsignedByte;

                break;
            case ImageFormat.FORMAT_RG8:
                glInternalFormat = InternalFormat.Rg8;
                glFormat         = PixelFormat.Rg;
                glType           = PixelType.UnsignedByte;

                break;
            case ImageFormat.FORMAT_RGB8:
                glInternalFormat = InternalFormat.Rgb8;
                glFormat         = PixelFormat.Rgb;
                glType           = PixelType.UnsignedByte;

                break;
            case ImageFormat.FORMAT_RGBA8:
                glInternalFormat = InternalFormat.Rgba8;
                glFormat         = PixelFormat.Rgba;
                glType           = PixelType.UnsignedByte;

                break;
            case ImageFormat.FORMAT_RGBA4444:
                glInternalFormat = InternalFormat.Rgba4;
                glFormat         = PixelFormat.Rgba;
                glType           = PixelType.UnsignedShort4444;

                break;
            case ImageFormat.FORMAT_RF:
                glInternalFormat = InternalFormat.R32f;
                glFormat         = PixelFormat.Red;
                glType           = PixelType.Float;

                break;
            case ImageFormat.FORMAT_RGF:
                glInternalFormat = InternalFormat.Rg32f;
                glFormat         = PixelFormat.Rg;
                glType           = PixelType.Float;

                break;
            case ImageFormat.FORMAT_RGBF:
                glInternalFormat = InternalFormat.Rgb32f;
                glFormat         = PixelFormat.Rgb;
                glType           = PixelType.Float;

                break;
            case ImageFormat.FORMAT_RGBAF:
                glInternalFormat = InternalFormat.Rgba32f;
                glFormat         = PixelFormat.Rgba;
                glType           = PixelType.Float;

                break;
            case ImageFormat.FORMAT_RH:
                glInternalFormat = InternalFormat.R16f;
                glFormat         = PixelFormat.Red;
                glType           = PixelType.HalfFloat;

                break;
            case ImageFormat.FORMAT_RGH:
                glInternalFormat = InternalFormat.Rg16f;
                glFormat         = PixelFormat.Rg;
                glType           = PixelType.HalfFloat;

                break;
            case ImageFormat.FORMAT_RGBH:
                glInternalFormat = InternalFormat.Rgb16f;
                glFormat         = PixelFormat.Rgb;
                glType           = PixelType.HalfFloat;

                break;
            case ImageFormat.FORMAT_RGBAH:
                glInternalFormat = InternalFormat.Rgba16f;
                glFormat         = PixelFormat.Rgba;
                glType           = PixelType.HalfFloat;

                break;
            case ImageFormat.FORMAT_RGBE9995:
                glInternalFormat = InternalFormat.Rgb9E5;
                glFormat         = PixelFormat.Rgb;
                glType           = PixelType.UnsignedInt5999Rev;

                break;
            case ImageFormat.FORMAT_DXT1:
                if (config.S3tcSupported)
                {
                    glInternalFormat = InternalFormat.CompressedRgbaS3tcDxt1EXT;
                    glFormat         = PixelFormat.Rgba;
                    glType           = PixelType.UnsignedByte;
                    compressed       = true;
                }
                else
                {
                    needDecompress = true;
                }

                break;
            case ImageFormat.FORMAT_DXT3:
                if (config.S3tcSupported)
                {
                    glInternalFormat = InternalFormat.CompressedRgbaS3tcDxt3EXT;
                    glFormat         = PixelFormat.Rgba;
                    glType           = PixelType.UnsignedByte;
                    compressed       = true;
                }
                else
                {
                    needDecompress = true;
                }

                break;
            case ImageFormat.FORMAT_DXT5:
                if (config.S3tcSupported)
                {
                    glInternalFormat = InternalFormat.CompressedRgbaS3tcDxt5EXT;
                    glFormat         = PixelFormat.Rgba;
                    glType           = PixelType.UnsignedByte;
                    compressed       = true;
                }
                else
                {
                    needDecompress = true;
                }

                break;
            case ImageFormat.FORMAT_RGTC_R:
                if (config.RgtcSupported)
                {
                    glInternalFormat = InternalFormat.CompressedRedRgtc1EXT;
                    glFormat         = PixelFormat.Rgba;
                    glType           = PixelType.UnsignedByte;
                    compressed       = true;
                }
                else
                {
                    needDecompress = true;
                }

                break;
            case ImageFormat.FORMAT_RGTC_RG:
                if (config.RgtcSupported)
                {
                    glInternalFormat = InternalFormat.CompressedRedGreenRgtc2EXT;
                    glFormat         = PixelFormat.Rgba;
                    glType           = PixelType.UnsignedByte;
                    compressed       = true;
                }
                else
                {
                    needDecompress = true;
                }

                break;
            case ImageFormat.FORMAT_BPTC_RGBA:
                if (config.BptcSupported)
                {
                    glInternalFormat = InternalFormat.CompressedRgbaBptcUnorm;
                    glFormat         = PixelFormat.Rgba;
                    glType           = PixelType.UnsignedByte;
                    compressed       = true;
                }
                else
                {
                    needDecompress = true;
                }

                break;
            case ImageFormat.FORMAT_BPTC_RGBF:
                if (config.BptcSupported)
                {
                    glInternalFormat = InternalFormat.CompressedRgbBptcSignedFloat;
                    glFormat         = PixelFormat.Rgb;
                    glType           = PixelType.Float;
                    compressed       = true;
                }
                else
                {
                    needDecompress = true;
                }

                break;
            case ImageFormat.FORMAT_BPTC_RGBFU:
                if (config.BptcSupported)
                {
                    glInternalFormat = InternalFormat.CompressedRgbBptcUnsignedFloat;
                    glFormat         = PixelFormat.Rgb;
                    glType           = PixelType.Float;
                    compressed       = true;
                }
                else
                {
                    needDecompress = true;
                }

                break;
            case ImageFormat.FORMAT_ETC2_R11:
                if (config.Etc2Supported)
                {
                    glInternalFormat = InternalFormat.CompressedR11Eac;
                    glFormat         = PixelFormat.Red;
                    glType           = PixelType.UnsignedByte;
                    compressed       = true;
                }
                else
                {
                    needDecompress = true;
                }

                break;
            case ImageFormat.FORMAT_ETC2_R11S:
                if (config.Etc2Supported)
                {
                    glInternalFormat = InternalFormat.CompressedSignedR11Eac;
                    glFormat         = PixelFormat.Red;
                    glType           = PixelType.UnsignedByte;
                    compressed       = true;
                }
                else
                {
                    needDecompress = true;
                }

                break;
            case ImageFormat.FORMAT_ETC2_RG11:
                if (config.Etc2Supported)
                {
                    glInternalFormat = InternalFormat.CompressedRg11Eac;
                    glFormat         = PixelFormat.Rg;
                    glType           = PixelType.UnsignedByte;
                    compressed       = true;
                }
                else
                {
                    needDecompress = true;
                }

                break;
            case ImageFormat.FORMAT_ETC2_RG11S:
                if (config.Etc2Supported)
                {
                    glInternalFormat = InternalFormat.CompressedSignedRg11Eac;
                    glFormat         = PixelFormat.Rg;
                    glType           = PixelType.UnsignedByte;
                    compressed       = true;
                }
                else
                {
                    needDecompress = true;
                }

                break;
            case ImageFormat.FORMAT_ETC:
            case ImageFormat.FORMAT_ETC2_RGB8:
                if (config.Etc2Supported)
                {
                    glInternalFormat = InternalFormat.CompressedRgb8Etc2;
                    glFormat         = PixelFormat.Rgb;
                    glType           = PixelType.UnsignedByte;
                    compressed       = true;
                }
                else
                {
                    needDecompress = true;
                }

                break;
            case ImageFormat.FORMAT_ETC2_RGBA8:
                if (config.Etc2Supported)
                {
                    glInternalFormat = InternalFormat.CompressedRgba8Etc2Eac;
                    glFormat         = PixelFormat.Rgba;
                    glType           = PixelType.UnsignedByte;
                    compressed       = true;
                }
                else
                {
                    needDecompress = true;
                }

                break;
            case ImageFormat.FORMAT_ETC2_RGB8A1:
                if (config.Etc2Supported)
                {
                    glInternalFormat = InternalFormat.CompressedRgb8PunchthroughAlpha1Etc2;
                    glFormat         = PixelFormat.Rgba;
                    glType           = PixelType.UnsignedByte;
                    compressed       = true;
                }
                else
                {
                    needDecompress = true;
                }

                break;
            case ImageFormat.FORMAT_ETC2_RA_AS_RG:
                #if !WEB_ENABLED
                if (config.Etc2Supported)
                {
                    glInternalFormat = InternalFormat.CompressedRgba8Etc2Eac;
                    glFormat         = PixelFormat.Rgba;
                    glType           = PixelType.UnsignedByte;
                    compressed       = true;
                }
                else
                #endif
                {
                    needDecompress = true;
                }
                decompressRaToRg = true;

                break;
            case ImageFormat.FORMAT_DXT5_RA_AS_RG:
                #if !WEB_ENABLED
                if (config.S3tcSupported)
                {
                    glInternalFormat = InternalFormat.CompressedRgbaS3tcDxt5EXT;
                    glFormat         = PixelFormat.Rgba;
                    glType           = PixelType.UnsignedByte;
                    compressed       = true;
                }
                else
                #endif
                {
                    needDecompress = true;
                }
                decompressRaToRg = true;

                break;
            case ImageFormat.FORMAT_ASTC_4x4:
                if (config.AstcSupported)
                {
                    glInternalFormat = InternalFormat.CompressedRgbaAstc4x4Khr;
                    glFormat         = PixelFormat.Rgba;
                    glType           = PixelType.UnsignedByte;
                    compressed       = true;
                }
                else
                {
                    needDecompress = true;
                }

                break;
            case ImageFormat.FORMAT_ASTC_4x4_HDR:
                if (config.AstcHdrSupported)
                {
                    glInternalFormat = InternalFormat.CompressedRgbaAstc4x4Khr;
                    glFormat         = PixelFormat.Rgba;
                    glType           = PixelType.UnsignedByte;
                    compressed       = true;
                }
                else
                {
                    needDecompress = true;
                }

                break;
            case ImageFormat.FORMAT_ASTC_8x8:
                if (config.AstcSupported)
                {
                    glInternalFormat = InternalFormat.CompressedRgbaAstc8x8Khr;
                    glFormat         = PixelFormat.Rgba;
                    glType           = PixelType.UnsignedByte;
                    compressed       = true;
                }
                else
                {
                    needDecompress = true;
                }

                break;
            case ImageFormat.FORMAT_ASTC_8x8_HDR:
                if (config.AstcHdrSupported)
                {
                    glInternalFormat = InternalFormat.CompressedRgbaAstc8x8Khr;
                    glFormat         = PixelFormat.Rgba;
                    glType           = PixelType.UnsignedByte;
                    compressed       = true;
                }
                else
                {
                    needDecompress = true;
                }

                break;
            default:
                return ERR_FAIL_V_MSG(default(Image), $"Image Format: {format} is not supported by the OpenGL3 Renderer");
        }

        if (needDecompress || forceDecompress)
        {
            if (image != null)
            {
                image = (Image)image.Duplicate();

                image.Decompress();

                if (ERR_FAIL_COND_V(image.IsCompressed))
                {
                    return image;
                }

                if (decompressRaToRg)
                {
                    image.ConvertRaRgba8ToRg();
                    image.Convert(ImageFormat.FORMAT_RG8);
                }
                switch (image.Format)
                {
                    case ImageFormat.FORMAT_RG8:
                        glFormat         = PixelFormat.Rg;
                        glInternalFormat = InternalFormat.Rg8;
                        glType           = PixelType.UnsignedByte;
                        realFormat       = ImageFormat.FORMAT_RG8;
                        compressed       = false;

                        break;
                    case ImageFormat.FORMAT_RGB8:
                        glFormat         = PixelFormat.Rgb;
                        glInternalFormat = InternalFormat.Rgb;
                        glType           = PixelType.UnsignedByte;
                        realFormat       = ImageFormat.FORMAT_RGB8;
                        compressed       = false;

                        break;
                    case ImageFormat.FORMAT_RGBA8:
                        glFormat         = PixelFormat.Rgba;
                        glInternalFormat = InternalFormat.Rgba;
                        glType           = PixelType.UnsignedByte;
                        realFormat       = ImageFormat.FORMAT_RGBA8;
                        compressed       = false;

                        break;
                    default:
                        image.Convert(ImageFormat.FORMAT_RGBA8);
                        glFormat         = PixelFormat.Rgba;
                        glInternalFormat = InternalFormat.Rgba;
                        glType           = PixelType.UnsignedByte;
                        realFormat       = ImageFormat.FORMAT_RGBA8;
                        compressed       = false;

                        break;
                }
            }

            return image;
        }

        return image;
    }

    private static void RenderTargetClearSdf(RenderTarget renderTarget)
    {
        var gl = GL.Singleton;
        if (renderTarget.SdfTextureWriteFb != 0)
        {
            gl.DeleteTextures(new[] { renderTarget.SdfTextureRead });
            gl.DeleteTextures(new[] { renderTarget.SdfTextureWrite });
            gl.DeleteTextures(renderTarget.SdfTextureProcess);
            gl.DeleteFramebuffers(new[] { renderTarget.SdfTextureWriteFb });

            renderTarget.SdfTextureRead       = 0;
            renderTarget.SdfTextureWrite      = 0;
            renderTarget.SdfTextureProcess[0] = 0;
            renderTarget.SdfTextureProcess[1] = 0;
            renderTarget.SdfTextureWriteFb    = 0;
        }
    }

    private static Rect2<int> RenderTargetGetSdfRect(RenderTarget rt)
    {
        var scale = rt.SdfOversize switch
        {
            RS.ViewportSDFOversize.VIEWPORT_SDF_OVERSIZE_100_PERCENT => 100,
            RS.ViewportSDFOversize.VIEWPORT_SDF_OVERSIZE_120_PERCENT => 120,
            RS.ViewportSDFOversize.VIEWPORT_SDF_OVERSIZE_150_PERCENT => 150,
            RS.ViewportSDFOversize.VIEWPORT_SDF_OVERSIZE_200_PERCENT => 200,
            _ => 0,
        };

        var margin = rt.Size * scale / 100 - rt.Size;

        var r = new Rect2<int>(default, rt.Size);

        r.Position -= margin;
        r.Size += margin * 2;

        return r;
    }
    #endregion private static methods

    #region public static methods
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void StoreCamera(in Projection<RealT> mtx, float[] array) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void StoreTransform(in Transform3D<RealT> mtx, float[] array) => throw new NotImplementedException();

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void StoreTransform3x3(in Basis<RealT> mtx, float[] array) => throw new NotImplementedException();
    #endregion public static methods

    #region private methods
    private void ClearRenderTarget(RenderTarget renderTarget)
    {
        var gl = GL.Singleton;

        if (renderTarget.DirectToScreen)
        {
            return;
        }

        if (renderTarget.Fbo > 0)
        {
            gl.DeleteFramebuffers(new[] { renderTarget.Fbo });
            renderTarget.Fbo = 0;
        }

        if (renderTarget.Overridden.Color == default)
        {
            gl.DeleteTextures(new[] { renderTarget.Color });
        }

        if (renderTarget.Overridden.Depth == default)
        {
            gl.DeleteTextures(new[] { renderTarget.Depth });
        }

        if (renderTarget.Texture != default)
        {
            var texture = this.GetTexture(renderTarget.Texture)!;

            texture.AllocHeight = 0;
            texture.AllocWidth  = 0;
            texture.Width       = 0;
            texture.Height      = 0;
            texture.Active      = false;
        }

        if (renderTarget.Overridden.Color != default)
        {
            var texture = this.GetTexture(renderTarget.Overridden.Color)!;

            texture.IsRenderTarget = false;
        }

        if (renderTarget.BackbufferFbo != 0)
        {
            gl.DeleteFramebuffers(new[] { renderTarget.BackbufferFbo });
            gl.DeleteTextures(new[] { renderTarget.Backbuffer });

            renderTarget.BackbufferFbo = 0;
            renderTarget.Backbuffer    = 0;
        }

        RenderTargetClearSdf(renderTarget);
    }
    private void UpdateRenderTarget(RenderTarget rt)
    {
        var gl = GL.Singleton;

        if (rt.Size.X <= 0 || rt.Size.Y <= 0)
        {
            return;
        }

        if (rt.DirectToScreen)
        {
            rt.Fbo = SystemFbo;
            return;
        }

        rt.ColorInternalFormat = rt.IsTransparent ? InternalFormat.Rgba8 : InternalFormat.Rgb10A2;
        rt.ColorFormat         = PixelFormat.Rgba;
        rt.ColorType           = rt.IsTransparent ? PixelType.UnsignedByte : PixelType.UnsignedInt2101010Rev;
        rt.ImageFormat         = ImageFormat.FORMAT_RGBA8;

        gl.Disable(EnableCap.ScissorTest);
        gl.ColorMask(true, true, true, true);
        gl.DepthMask(false);

        var useMultiView = false; // renderTarget.ViewCount > 1 && config.multiview_supported;
        var textureTarget = useMultiView ? TextureTarget.Texture2DArray : TextureTarget.Texture2D;

        gl.GenFramebuffers(out var fbo);
        rt.Fbo = fbo;

        gl.BindFramebuffer(FramebufferTarget.Framebuffer, rt.Fbo);

        Texture texture;

        if (rt.Overridden.Color != default)
        {
            texture = this.GetTexture(rt.Overridden.Color)!;

            rt.Color = texture.TexId;
            rt.Size  = new Vector2<int>(texture.Width, texture.Height);
        }
        else
        {
            texture = this.GetTexture(rt.Texture)!;

            gl.GenTextures(out var color);
            rt.Color = color;

            gl.BindTexture(textureTarget, rt.Color);

            unsafe
            {
                if (useMultiView)
                {
                    gl.TexImage3D(
                        textureTarget,
                        0,
                        rt.ColorInternalFormat,
                        rt.Size.X,
                        rt.Size.Y,
                        (int)rt.ViewCount,
                        0,
                        rt.ColorFormat,
                        rt.ColorType,
                        default(nint)
                    );
                }
                else
                {
                    gl.TexImage2D(
                        textureTarget,
                        0,
                        rt.ColorInternalFormat,
                        rt.Size.X,
                        rt.Size.Y,
                        0,
                        rt.ColorFormat,
                        rt.ColorType,
                        default(nint)
                    );
                }
            }

            gl.TexParameterI(textureTarget, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            gl.TexParameterI(textureTarget, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            gl.TexParameterI(textureTarget, TextureParameterName.TextureWrapS,     (int)TextureWrapMode.ClampToEdge);
            gl.TexParameterI(textureTarget, TextureParameterName.TextureWrapT,     (int)TextureWrapMode.ClampToEdge);
        }

        var ovr = default(GlOvrMultiview);

        if (useMultiView && gl.TryGetExtension(out ovr))
        {
            ovr!.FramebufferTextureMultiview(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                rt.Color,
                0,
                0,
                (int)rt.ViewCount
            );
        }
        else
        {
            gl.FramebufferTexture2D(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0,
                TextureTarget.Texture2D,
                rt.Color,
                0
            );
        }

        if (rt.Overridden.Depth != default)
        {
            texture = this.GetTexture(rt.Overridden.Depth)!;

            rt.Depth = texture.TexId;
        }
        else
        {
            gl.GenTextures(out var depth);
            rt.Depth = depth;

            gl.BindTexture(textureTarget, rt.Depth);

            unsafe
            {
                if (useMultiView)
                {
                    gl.TexImage3D(
                        textureTarget,
                        0,
                        InternalFormat.DepthComponent24,
                        rt.Size.X,
                        rt.Size.Y,
                        (int)rt.ViewCount,
                        0,
                        PixelFormat.DepthComponent,
                        PixelType.UnsignedInt,
                        default(nint)
                    );
                }
                else
                {
                    gl.TexImage2D(
                        textureTarget,
                        0,
                        InternalFormat.DepthComponent24,
                        rt.Size.X,
                        rt.Size.Y,
                        0,
                        PixelFormat.DepthComponent,
                        PixelType.UnsignedInt,
                        default(nint)
                    );
                }
            }

            gl.TexParameterI(textureTarget, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            gl.TexParameterI(textureTarget, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            gl.TexParameterI(textureTarget, TextureParameterName.TextureWrapS,     (int)TextureWrapMode.ClampToEdge);
            gl.TexParameterI(textureTarget, TextureParameterName.TextureWrapT,     (int)TextureWrapMode.ClampToEdge);
        }

        if (useMultiView && ovr != null)
        {
            ovr.FramebufferTextureMultiview(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthAttachment,
                rt.Depth,
                0,
                0,
                (int)rt.ViewCount
            );
        }
        else
        {
            gl.FramebufferTexture2D(
                FramebufferTarget.Framebuffer,
                FramebufferAttachment.DepthAttachment,
                TextureTarget.Texture2D,
                rt.Depth,
                0
            );
        }

        var status = gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer);

        if (status != FramebufferStatus.FramebufferComplete)
        {
            gl.DeleteBuffers(rt.Fbo);
            gl.DeleteTextures(rt.Color);

            rt.Fbo   = 0;
            rt.Size  = new Vector2<int>();
            rt.Color = 0;
            rt.Depth = 0;

            if (rt.Overridden.Color != default)
            {
                texture.TexId  = 0;
                texture.Active = false;
            }

            WARN_PRINT($"Could not create render target, status: {status}");
        }

        if (rt.Overridden.Color != default)
        {
            texture.IsRenderTarget = true;
        }
        else
        {
            texture.Format     = rt.ImageFormat;
            texture.RealFormat = rt.ImageFormat;
            texture.Target     = textureTarget;

            if (rt.ViewCount > 1 /*  && config.MultiviewSupported */)
            {
                texture.Type   = TextureType.LAYERED;
                texture.Layers = rt.ViewCount;
            }
            else
            {
                texture.Type   = TextureType.T2D;
                texture.Layers = 1;
            }

            texture.FormatCache         = rt.ColorFormat;
            texture.TypeCache           = PixelType.UnsignedByte;
            texture.InternalFormatCache = rt.ColorInternalFormat;
            texture.TexId               = rt.Color;
            texture.Width               = rt.Size.X;
            texture.Height              = rt.Size.Y;
            texture.AllocHeight         = rt.Size.Y;
            texture.Active              = true;
        }

        gl.ClearColor(0, 0, 0, 0);
        gl.Clear(ClearBufferMask.ColorBufferBit);
        gl.BindFramebuffer(FramebufferTarget.Framebuffer, SystemFbo);
    }
    #endregion private methods

    #region public methods
    public CanvasTexture? GetCanvasTexture(Guid id) =>
        this.canvasTextureOwner.GetOrNull(id);

    public RenderTarget? GetRenderTarget(Guid renderTargetId) =>
        this.renderTargetOwner.GetOrNull(renderTargetId);

    public Texture? GetTexture(Guid id)
    {
        var texture = this.textureOwner.GetOrNull(id);

        return texture?.IsProxy ?? false ? this.textureOwner[texture.ProxyTo] : texture;
    }

    public void RenderTargetClearBackBuffer(Guid p_to_render_target, Rect2<int> rect2, Color color) => throw new NotImplementedException();

    public void RenderTargetClearUsed(Guid renderTargetId)
    {
        var renderTarget = this.renderTargetOwner[renderTargetId];

        renderTarget.UsedInFrame = false;
    }

    public void RenderTargetCopyToBackBuffer(Guid p_to_render_target, Rect2<RealT> groupRect, bool v) => throw new NotImplementedException();
    public void RenderTargetGenBackBufferMipmaps(Guid p_to_render_target, Rect2<float> globalRectCache) => throw new NotImplementedException();

    public uint RenderTargetGetSdfTexture(Guid renderTargetId)
    {
        var rt = this.renderTargetOwner.GetOrNull(renderTargetId);

        if (ERR_FAIL_COND_V(rt == null))
        {
            return default;
        }

        if (rt!.SdfTextureRead == 0)
        {
            var texture = this.textureOwner.GetOrNull(this.defaultGlTextures[(int)DefaultGLTexture.DEFAULT_GL_TEXTURE_BLACK]);

            return texture!.TexId;
        }

        return rt.SdfTextureRead;
    }

    public uint TextureAtlasGetTexture() => throw new NotImplementedException();
    public Rect2<RealT> TextureAtlasGetTextureRect(Guid textureId) => throw new NotImplementedException();

    public Guid TextureGlGetDefault(DefaultGLTexture texture) =>
        this.defaultGlTextures[(int)texture];

    public void TextureSetData(Guid textureId, Image image, int layer = 0)
    {
        var gl = GL.Singleton;

        var texture = this.textureOwner.GetOrNull(textureId);

        if (ERR_FAIL_COND(texture == null))
        {
            return;
        }

        if (texture!.Target == TextureTarget.Texture3D)
        {
            // Target is set to a 3D texture or array texture, exit early to avoid spamming errors
            return;
        }

        if (ERR_FAIL_COND(!texture.Active))
        {
            return;
        }

        if (ERR_FAIL_COND(texture.IsRenderTarget))
        {
            return;
        }

        if (ERR_FAIL_COND(image == null))
        {
            return;
        }

        if (ERR_FAIL_COND(texture.Format != image!.Format))
        {
            return;
        }

        if (ERR_FAIL_COND(image.Width == 0))
        {
            return;
        }

        if (ERR_FAIL_COND(image.Height == 0))
        {
            return;
        }

        var img = GetGlImageAndFormat(
            image,
            image.Format,
            out var realFormat,
            out var format,
            out var internalFormat,
            out var type,
            out var compressed,
            texture.ResizeToPo2
        );

        if (ERR_FAIL_COND(img == null))
        {
            return;
        }

        if (texture.ResizeToPo2)
        {
            if (image.IsCompressed)
            {
                ERR_PRINT($"Texture '{texture.Path}' is required to be a power of 2 because it uses either mipmaps or repeat, so it was decompressed. This will hurt performance and memory usage.");
            }

            if (img == image)
            {
                img = (Image)img.Duplicate();
            }

            img!.ResizeToPo2(false);
        }

        var blitTarget = (texture.Target == TextureTarget.TextureCubeMap) ? cubeSideEnum[layer] : texture.Target;

        var read = img!.Data.AsSpan();

        gl.ActiveTexture(TextureUnit.Texture0);
        gl.BindTexture(texture.Target, texture.TexId);

        // set filtering and repeat state to default
        texture.GlSetFilter(RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_NEAREST);
        texture.GlSetRepeat(RS.CanvasItemTextureRepeat.CANVAS_ITEM_TEXTURE_REPEAT_ENABLED);

        #if !WEB_ENABLED
        switch (texture.Format)
        {
            #if GLES_OVER_GL
            case ImageFormat.FORMAT_L8:
                gl.TexParameterI(texture.Target, TextureParameterName.TextureSwizzleR, (int)PixelFormat.Red);
                gl.TexParameterI(texture.Target, TextureParameterName.TextureSwizzleG, (int)PixelFormat.Red);
                gl.TexParameterI(texture.Target, TextureParameterName.TextureSwizzleB, (int)PixelFormat.Red);
                gl.TexParameterI(texture.Target, TextureParameterName.TextureSwizzleA, 1);

                break;
            case ImageFormat.FORMAT_LA8:
                gl.TexParameterI(texture.Target, TextureParameterName.TextureSwizzleR, (int)PixelFormat.Red);
                gl.TexParameterI(texture.Target, TextureParameterName.TextureSwizzleG, (int)PixelFormat.Red);
                gl.TexParameterI(texture.Target, TextureParameterName.TextureSwizzleB, (int)PixelFormat.Red);
                gl.TexParameterI(texture.Target, TextureParameterName.TextureSwizzleA, (int)PixelFormat.Green);

                break;
            #endif // GLES3_OVER_GL

            case ImageFormat.FORMAT_ETC2_RA_AS_RG:
            case ImageFormat.FORMAT_DXT5_RA_AS_RG:
                gl.TexParameterI(texture.Target, TextureParameterName.TextureSwizzleR, (int)PixelFormat.Red);

                if (texture.Format == realFormat)
                {
                    // Swizzle RA from compressed texture into RG
                    gl.TexParameterI(texture.Target, TextureParameterName.TextureSwizzleG, (int)PixelFormat.Alpha);
                }
                else
                {
                    // Converted textures are already in RG, leave as-is
                    gl.TexParameterI(texture.Target, TextureParameterName.TextureSwizzleG, (int)PixelFormat.Green);
                }

                gl.TexParameterI(texture.Target, TextureParameterName.TextureSwizzleB, 0);
                gl.TexParameterI(texture.Target, TextureParameterName.TextureSwizzleA, 1);

                break;
            default:
                gl.TexParameterI(texture.Target, TextureParameterName.TextureSwizzleR, (int)PixelFormat.Red);
                gl.TexParameterI(texture.Target, TextureParameterName.TextureSwizzleG, (int)PixelFormat.Green);
                gl.TexParameterI(texture.Target, TextureParameterName.TextureSwizzleB, (int)PixelFormat.Blue);
                gl.TexParameterI(texture.Target, TextureParameterName.TextureSwizzleA, (int)PixelFormat.Alpha);

                break;
        }
        #endif // WEB_ENABLED

        var mipmaps = img.HasMipmaps ? img.MipmapCount + 1 : 1;

        var w = img.Width;
        var h = img.Height;

        var tsize = 0;

        for (var i = 0; i < mipmaps; i++)
        {
            img.GetMipmapOffsetAndSize(i, out var ofs, out var size, out var _);

            if (compressed)
            {
                gl.PixelStorei(PixelStoreParameter.UnpackAlignment, 4);

                var bw = w;
                var bh = h;

                gl.CompressedTexImage2D(blitTarget, i, internalFormat, bw, bh, 0, size, read[ofs..]);
            }
            else
            {
                gl.PixelStorei(PixelStoreParameter.UnpackAlignment, 1);

                if (texture.Target == TextureTarget.Texture2DArray)
                {
                    gl.TexSubImage3D(TextureTarget.Texture2DArray, i, 0, 0, layer, w, h, 0, format, type, read[ofs..]);
                }
                else
                {
                    gl.TexImage2D(blitTarget, i, internalFormat, w, h, 0, format, type, read[ofs..]);
                }
            }

            tsize += size;

            w = Math.Max(1, w >> 1);
            h = Math.Max(1, h >> 1);
        }

        // info.texture_mem -= texture.TotalDataSize; // TODO make this work again!!
        texture.TotalDataSize = tsize;
        // info.texture_mem += texture.TotalDataSize; // TODO make this work again!!


        texture.StoredCubeSides = (ulong)((int)texture.StoredCubeSides | (1 << layer));

        texture.Mipmaps = mipmaps;
    }

    public void UpdateTextureAtlas() => throw new NotImplementedException();
    #endregion public methods

    #region public override methods
    public override bool CanCreateResourcesAsync() => false;
    public override Guid CanvasTextureAllocate() => throw new NotImplementedException();
    public override void CanvasTextureFree(Guid id) => throw new NotImplementedException();
    public override void CanvasTextureInitialize(Guid id) => this.canvasTextureOwner.Initialize(id);
    public override void CanvasTextureSetChannel(Guid canvasTexture, RS.CanvasTextureChannel channel, Guid textureId) => throw new NotImplementedException();
    public override void CanvasTextureSetShadingParameters(Guid canvasTexture, in Color baseColor, float shininess) => throw new NotImplementedException();
    public override void CanvasTextureSetTextureFilter(Guid item, RS.CanvasItemTextureFilter filter) => throw new NotImplementedException();
    public override void CanvasTextureSetTextureRepeat(Guid item, RS.CanvasItemTextureRepeat repeat) => throw new NotImplementedException();
    public override Guid DecalAllocate() => throw new NotImplementedException();
    public override void DecalFree(Guid id) => throw new NotImplementedException();
    public override AABB DecalGetAabb(Guid decal) => throw new NotImplementedException();
    public override uint DecalGetCullMask(Guid decal) => throw new NotImplementedException();
    public override void DecalInitialize(Guid id) => throw new NotImplementedException();
    public override Guid DecalInstanceCreate(Guid decal) => throw new NotImplementedException();
    public override void DecalInstanceFree(Guid decalInstance) => throw new NotImplementedException();
    public override void DecalInstanceSetSortingOffset(Guid decalInstance, float sortingOffset) => throw new NotImplementedException();
    public override void DecalInstanceSetTransform(Guid decalInstance, in Transform3D<float> transform) => throw new NotImplementedException();
    public override void DecalSetAlbedoMix(Guid decal, float mix) => throw new NotImplementedException();
    public override void DecalSetCullMask(Guid decal, uint layers) => throw new NotImplementedException();
    public override void DecalSetDistanceFade(Guid decal, bool enabled, float begin, float length) => throw new NotImplementedException();
    public override void DecalSetEmissionEnergy(Guid decal, float energy) => throw new NotImplementedException();
    public override void DecalSetFade(Guid decal, float above, float below) => throw new NotImplementedException();
    public override void DecalSetModulate(Guid decal, in Color modulate) => throw new NotImplementedException();
    public override void DecalSetNormalFade(Guid decal, float fade) => throw new NotImplementedException();
    public override void DecalSetSize(Guid decal, in Vector3<float> size) => throw new NotImplementedException();
    public override void DecalSetTexture(Guid decal, RS.DecalTexture type, Guid textureId) => throw new NotImplementedException();

    public override Guid RenderTargetCreate()
    {
        var renderTarget = new RenderTarget()
        {
            UsedInFrame    = false,
            ClearRequested = false
        };

        var t = new Texture
        {
            Active         = true,
            RenderTarget   = renderTarget,
            IsRenderTarget = true
        };

        renderTarget.Texture = this.textureOwner.Add(t);

        this.UpdateRenderTarget(renderTarget);

        return this.renderTargetOwner.Add(renderTarget);
    }

    public override void RenderTargetDoClearRequest(Guid renderTargetId)
    {
        var rt = this.renderTargetOwner.GetOrNull(renderTargetId);

        if (ERR_FAIL_COND(rt == null))
        {
            return;
        }

        if (!rt!.ClearRequested)
        {
            return;
        }

        var gl = GL.Singleton;

        gl.BindFramebuffer(FramebufferTarget.Framebuffer, rt.Fbo);

        var components = new[]
        {
            rt.ClearColor.R,
            rt.ClearColor.G,
            rt.ClearColor.B,
            rt.ClearColor.A,
        };

        gl.ClearBufferfv(Buffer.Color, 0, components);
        rt.ClearRequested = false;
        gl.BindFramebuffer(FramebufferTarget.Framebuffer, SystemFbo);
    }

    public override void RenderTargetDisableClearRequest(Guid renderTargetId) => throw new NotImplementedException();
    public override void RenderTargetFree(Guid renderTargetId) => throw new NotImplementedException();
    public override Color RenderTargetGetClearRequestColor(Guid renderTargetId) => throw new NotImplementedException();
    public override bool RenderTargetGetDirectToScreen(Guid renderTargetId) => throw new NotImplementedException();
    public override RS.ViewportMSAA RenderTargetGetMsaa(Guid renderTargetId) => throw new NotImplementedException();

    public override Guid RenderTargetGetOverrideColor(Guid renderTargetId)
    {
        var rt = this.renderTargetOwner.GetOrNull(renderTargetId);

        return ERR_FAIL_COND_V(rt == null) ? default : rt!.Overridden.Color;
    }

    public override Guid RenderTargetGetOverrideDepth(Guid renderTargetId) => throw new NotImplementedException();
    public override Guid RenderTargetGetOverrideVelocity(Guid renderTargetId) => throw new NotImplementedException();
    public override Vector2<int> RenderTargetGetPosition(Guid renderTargetId) => throw new NotImplementedException();

    public override Rect2<int> RenderTargetGetSdfRect(Guid renderTargetId)
    {
        var rt = this.renderTargetOwner.GetOrNull(renderTargetId);

        return ERR_FAIL_COND_V(rt == null) ? default : RenderTargetGetSdfRect(rt!);
    }

    public override Vector2<int> RenderTargetGetSize(Guid renderTargetId)
    {
        var rt = this.renderTargetOwner.GetOrNull(renderTargetId);

        return ERR_FAIL_COND_V(rt == null) ? default : rt!.Size;
    }

    public override Guid RenderTargetGetTexture(Guid renderTargetId)
    {
        var rt = this.renderTargetOwner.GetOrNull(renderTargetId);

        return ERR_FAIL_COND_V(rt == null) ? default : rt!.Overridden.Color != default ? rt.Overridden.Color : rt.Texture;
    }

    public override bool RenderTargetGetTransparent(Guid renderTargetId) => throw new NotImplementedException();
    public override RS.ViewportVRSMode RenderTargetGetVrsMode(Guid renderTargetId) => throw new NotImplementedException();
    public override Guid RenderTargetGetVrsTexture(Guid renderTargetId) => throw new NotImplementedException();

    public override bool RenderTargetIsSdfEnabled(Guid renderTargetId) => throw new NotImplementedException();

    public override bool RenderTargetIsClearRequested(Guid renderTargetId)
    {
        var rt = this.renderTargetOwner.GetOrNull(renderTargetId);

        return !ERR_FAIL_COND_V(rt == null) && rt!.ClearRequested;
    }

    public override void RenderTargetMarkSdfEnabled(Guid renderTargetId, bool enabled)
    {
        var rt = this.renderTargetOwner.GetOrNull(renderTargetId);

        if (ERR_FAIL_COND_V(rt == null))
        {
            return;
        }

        rt!.SdfEnabled = enabled;
    }

    public override void RenderTargetRequestClear(Guid renderTargetId, in Color color = default)
    {
        var rt = this.renderTargetOwner.GetOrNull(renderTargetId);

        if (ERR_FAIL_COND_V(rt == null))
        {
            return;
        }

        rt!.ClearRequested = true;
        rt.ClearColor = color;
    }

    public override void RenderTargetSetAsUnused(Guid renderTargetId) => this.RenderTargetClearUsed(renderTargetId);
    public override void RenderTargetSetDirectToScreen(Guid renderTargetId, bool directToScreen) => throw new NotImplementedException();

    public override void RenderTargetSetMsaa(Guid renderTargetId, RS.ViewportMSAA msaa)
    {
        var rt = this.renderTargetOwner.GetOrNull(renderTargetId);

        if (ERR_FAIL_COND_V(rt == null))
        {
            return;
        }

        if (msaa == rt!.Msaa)
        {
            return;
        }

        WARN_PRINT("2D MSAA is not yet supported for GLES3.");

        this.ClearRenderTarget(rt);
        rt.Msaa = msaa;
        this.UpdateRenderTarget(rt);
    }

    public override void RenderTargetSetOverride(Guid renderTargetId, Guid colorTexture, Guid depthTexture, Guid velocityTexture)
    {
        var renderTarget = this.renderTargetOwner[renderTargetId];

        renderTarget.Overridden.Velocity = velocityTexture;

        if (renderTarget.Overridden.Color == colorTexture && renderTarget.Overridden.Depth == depthTexture)
        {
            return;
        }

        if (colorTexture == default && depthTexture == default)
        {
            this.ClearRenderTarget(renderTarget);

            renderTarget.Overridden.IsOverridden = false;

            renderTarget.Color = default;
            renderTarget.Depth = default;
            renderTarget.Size = default;

            ClearRenderTargetOverriddenFboCache(renderTarget);

            return;
        }

        if (!renderTarget.Overridden.IsOverridden)
        {
            this.ClearRenderTarget(renderTarget);
        }

        renderTarget.Overridden.Color = colorTexture;
        renderTarget.Overridden.Depth = depthTexture;
        renderTarget.Overridden.IsOverridden = true;

        var hash = (uint)colorTexture.GetHashCode() ^ (uint)depthTexture.GetHashCode();

        if (renderTarget.Overridden.FboCache.TryGetValue(hash, out var cache))
        {
            renderTarget.Fbo = cache.Fbo;
            renderTarget.Size = cache.Size;
            renderTarget.Texture = colorTexture;

            return;
        }

        this.UpdateRenderTarget(renderTarget);
    }

    public override void RenderTargetSetPosition(Guid renderTargetId, int x, int y) => throw new NotImplementedException();

    public override void RenderTargetSetSize(Guid renderTargetId, int width, int height, uint viewCount)
    {
        var rt = this.renderTargetOwner.GetOrNull(renderTargetId);
        if (ERR_FAIL_COND(rt == null))
        {
            return;
        }

        if (width == rt!.Size.X && height == rt.Size.Y && viewCount == rt.ViewCount)
        {
            return;
        }
        if (rt.Overridden.Color != default)
        {
            return;
        }

        this.ClearRenderTarget(rt);

        rt.Size = new(width, height);
        rt.ViewCount = viewCount;

        this.UpdateRenderTarget(rt);
    }

    public override void RenderTargetSetSdfSizeAndScale(Guid renderTargetId, RS.ViewportSDFOversize sdfOversize, RS.ViewportSDFScale sdfScale)
    {
        var rt = this.renderTargetOwner.GetOrNull(renderTargetId);

        if (ERR_FAIL_COND(rt == null))
        {
            return;
        }

        if (rt!.SdfOversize == sdfOversize && rt.SdfScale == sdfScale)
        {
            return;
        }

        rt.SdfOversize = sdfOversize;
        rt.SdfScale = sdfScale;

        RenderTargetClearSdf(rt);
    }

    public override void RenderTargetSetTransparent(Guid renderTargetId, bool isTransparent)
    {
        var rt = this.renderTargetOwner.GetOrNull(renderTargetId);

        if (ERR_FAIL_COND(rt == null))
        {
            return;
        }

        rt!.IsTransparent = isTransparent;

        if (rt.Overridden.Color == default)
        {
            this.ClearRenderTarget(rt);
            this.UpdateRenderTarget(rt);
        }
    }

    public override void RenderTargetSetVrsMode(Guid renderTargetId, RS.ViewportVRSMode mode)
    {
        // Empty
    }

    public override void RenderTargetSetVrsTexture(Guid renderTargetId, Guid textureId) => throw new NotImplementedException();

    public override bool RenderTargetWasUsed(Guid renderTargetId)
    {
        var rt = this.renderTargetOwner.GetOrNull(renderTargetId);

        return !ERR_FAIL_COND_V(rt == null) && rt!.UsedInFrame;
    }

    public override Image Texture2DGet(Guid textureId) => throw new NotImplementedException();
    public override void Texture2DInitialize(Guid textureId, Image image)
    {
        if (ERR_FAIL_COND(image == null))
        {
            return;
        }

        var gl = GL.Singleton;

        var texture = new Texture
        {
            Width       = image!.Width,
            Height      = image.Height,
            AllocWidth  = image.Width,
            AllocHeight = image.Height,
            Mipmaps     = image.MipmapCount + 1,
            Format      = image.Format,
            Type        = TextureType.T2D,
            Target      = TextureTarget.Texture2D,
        };

        GetGlImageAndFormat(
            null,
            texture.Format,
            out var realFormat,
            out var glFormatCache,
            out var glInternalFormatCache,
            out var glTypeCache,
            out var compressed,
            false
        );

        texture.RealFormat            = realFormat;
        texture.GlFormatCache         = glFormatCache;
        texture.GlInternalFormatCache = glInternalFormatCache;
        texture.GlTypeCache           = glTypeCache;
        texture.Compressed            = compressed;
        //texture.total_data_size = image.get_image_data_size(); // verify that this returns size in bytes
        texture.Active = true;
        gl.GenTextures(out var texId);
        texture.TexId = texId;

        this.textureOwner.Initialize(textureId, texture);
        this.TextureSetData(textureId, image);
    }

    public override void Texture2DLayeredInitialize(Guid textureId, IList<Image> layers, RS.TextureLayeredType layeredType) =>
        this.textureOwner.Initialize(textureId);

    public override void Texture2DLayeredPlaceholderInitialize(Guid textureId, RS.TextureLayeredType layeredType) => throw new NotImplementedException();
    public override Image Texture2DLayerGet(Guid textureId, int layer) => throw new NotImplementedException();
    public override void Texture2DPlaceholderInitialize(Guid textureId) => throw new NotImplementedException();
    public override void Texture2DUpdate(Guid textureId, Image image, int layer = 0) => throw new NotImplementedException();
    public override Image[] Texture3DGet(Guid textureId) => throw new NotImplementedException();

    public override void Texture3DInitialize(Guid textureId, ImageFormat format, int width, int height, int depth, bool mipmaps, IList<Image> data) =>
        this.textureOwner.Initialize(textureId);

    public override void Texture3DPlaceholderInitialize(Guid textureId) => throw new NotImplementedException();
    public override void Texture3DUpdate(Guid textureId, Image[] data) => throw new NotImplementedException();
    public override void TextureAddToDecalAtlas(Guid textureId, bool panoramaToDp = false) => throw new NotImplementedException();
    public override Guid TextureAllocate() => throw new NotImplementedException();
    public override void TextureDebugUsage(out List<RS.TextureInfo> info) => throw new NotImplementedException();
    public override void TextureFree(Guid textureId) => throw new NotImplementedException();
    public override string TextureGetPath(Guid textureId) => throw new NotImplementedException();
    public override Guid TextureGetRdTextureGuid(Guid textureId, bool srgb = false) => throw new NotImplementedException();

    public override void TextureProxyInitialize(Guid textureId, Guid baseId)
    {
        var texture = this.textureOwner.GetOrNull(baseId);

        if (ERR_FAIL_COND(texture == null))
        {
            return;
        }

        var proxyTex = new Texture();

        proxyTex.CopyFrom(texture!);
        proxyTex.ProxyTo = baseId;
        proxyTex.IsRenderTarget = false;
        proxyTex.IsProxy = true;
        proxyTex.Proxies.Clear();

        texture!.Proxies.Add(textureId);

        this.textureOwner.Initialize(textureId, proxyTex);
    }

    public override void TextureProxyUpdate(Guid proxy, Guid @base) => throw new NotImplementedException();
    public override void TextureRemoveFromDecalAtlas(Guid textureId, bool panoramaToDp = false) => throw new NotImplementedException();
    public override void TextureReplace(Guid textureId, Guid byTexture) => throw new NotImplementedException();
    public override void TextureSetDetect3DCallback(Guid textureId, RS.TextureDetectCallback callback, object? userdata) => throw new NotImplementedException();
    public override void TextureSetDetectNormalCallback(Guid textureId, RS.TextureDetectCallback callback, object? userdata) => throw new NotImplementedException();
    public override void TextureSetDetectRoughnessCallback(Guid textureId, RS.TextureDetectRoughnessCallback callback, object? userdata) => throw new NotImplementedException();
    public override void TextureSetForceRedrawIfVisible(Guid textureId, bool enable) => throw new NotImplementedException();
    public override void TextureSetPath(Guid textureId, string path) => throw new NotImplementedException();
    public override Vector2<RealT> TextureSizeWithProxy(Guid textureId) => throw new NotImplementedException();
    public override void TextureSetSizeOverride(Guid textureId, int width, int height) => throw new NotImplementedException();

    #endregion public override methods
}
