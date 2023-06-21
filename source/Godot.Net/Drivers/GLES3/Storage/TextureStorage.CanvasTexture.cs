namespace Godot.Net.Drivers.GLES3.Storage;

using Godot.Net.Core.Math;

public record CanvasTexture
{
    public Guid                       Diffuse       { get; set; }
    public Guid                       NormalMap     { get; set; }
    public float                      Shininess     { get; set; } = 1;
    public Guid                       Specular      { get; set; }
    public Color                      SpecularColor { get; set; } = new Color(1, 1, 1, 1);
    public RS.CanvasItemTextureFilter TextureFilter { get; set; } = RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_DEFAULT;
    public RS.CanvasItemTextureRepeat TextureRepeat { get; set; } = RS.CanvasItemTextureRepeat.CANVAS_ITEM_TEXTURE_REPEAT_DEFAULT;
}
