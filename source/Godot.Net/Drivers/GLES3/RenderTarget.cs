namespace Godot.Net.Drivers.GLES3;

using Godot.Net.Core.IO;
using Godot.Net.Core.Math;
using Godot.Net.Drivers.GLES3.Api.Enums;

public record RenderTarget
{
    public uint                   Backbuffer          { get; set; }
    public uint                   BackbufferFbo       { get; set; }
    public Color                  ClearColor          { get; set; }
    public bool                   ClearRequest        { get; set; }
    public bool                   ClearRequested      { get; set; }
    public uint                   Color               { get; set; }
    public PixelFormat            ColorFormat         { get; set; }
    public InternalFormat         ColorInternalFormat { get; set; }
    public PixelType              ColorType           { get; set; }
    public uint                   Depth               { get; set; }
    public bool                   DirectToScreen      { get; set; }
    public uint                   Fbo                 { get; set; }
    public ImageFormat            ImageFormat         { get; set; }
    public int                    InternalColorFormat { get; set; }
    public bool                   IsTransparent       { get; set; }
    public RS.ViewportMSAA        Msaa                { get; set; }
    public RTOverridden           Overridden          { get; set; } = new();
    public bool                   SdfEnabled          { get; set; }
    public RS.ViewportSDFOversize SdfOversize         { get; set; }
    public RS.ViewportSDFScale    SdfScale            { get; set; }
    public uint[]                 SdfTextureProcess   { get; } = new uint[2];
    public uint                   SdfTextureRead      { get; set; }
    public uint                   SdfTextureWrite     { get; set; }
    public uint                   SdfTextureWriteFb   { get; set; }
    public Vector2<int>           Size                { get; set; }
    public Guid                   Texture             { get; set; }
    public bool                   UsedInFrame         { get; set; }
    public uint                   ViewCount           { get; set; }
}
