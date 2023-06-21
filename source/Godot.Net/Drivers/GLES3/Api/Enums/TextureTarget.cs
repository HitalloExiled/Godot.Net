namespace Godot.Net.Drivers.GLES3.Api.Enums;

public enum TextureTarget
{
    Texture1D                      = 0x0DE0,
    Texture2D                      = 0x0DE1,
    ProxyTexture1D                 = 0x8063,
    ProxyTexture2D                 = 0x8064,
    Texture3D                      = 0x806F,
    ProxyTexture3D                 = 0x8070,
    TextureRectangle               = 0x84F5,
    ProxyTextureRectangle          = 0x84F7,
    TextureCubeMap                 = 0x8513,
    TextureCubeMapPositiveX        = 0x8515,
    TextureCubeMapNegativeX        = 0x8516,
    TextureCubeMapPositiveY        = 0x8517,
    TextureCubeMapNegativeY        = 0x8518,
    TextureCubeMapPositiveZ        = 0x8519,
    TextureCubeMapNegativeZ        = 0x851A,
    ProxyTextureCubeMap            = 0x851B,
    Texture1DArray                 = 0x8C18,
    ProxyTexture1DArray            = 0x8C19,
    Texture2DArray                 = 0x8C1A,
    ProxyTexture2DArray            = 0x8C1B,
    TextureBuffer                  = 0x8C2A,
    Renderbuffer                   = 0x8D41,
    TextureCubeMapArray            = 0x9009,
    ProxyTextureCubeMapArray       = 0x900B,
    Texture2DMultisample           = 0x9100,
    ProxyTexture2DMultisample      = 0x9101,
    Texture2DMultisampleArray      = 0x9102,
    ProxyTexture2DMultisampleArray = 0x9103,
}
