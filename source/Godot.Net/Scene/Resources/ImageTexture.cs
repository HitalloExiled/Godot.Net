namespace Godot.Net.Scene.Resources;

using Godot.Net.Core.IO;

#pragma warning disable CS0414, IDE0052 // TODO Remove

public class ImageTexture : Texture2D
{
    private ImageFormat format;
    private int         h;
    private bool        imageStored;
    private bool        mipmaps;
    private Guid        texture;
    private int         w;

    public override int Height => this.h;
    public override int Width  => this.w;

    public ImageTexture()
    { }

    public static ImageTexture CreateFromImage(Image image)
    {
        if (ERR_FAIL_COND_V_MSG(image == null, "Invalid image"))
        {
            return new();
        }

        var imageTexture = new ImageTexture();
        imageTexture.SetImage(image!);
        return imageTexture;
    }

    public void SetImage(Image image)
    {
        if (ERR_FAIL_COND_MSG(image == null, "Invalid image"))
        {
            return;
        }

        this.w       = image!.Width;
        this.h       = image.Height;
        this.format  = image.Format;
        this.mipmaps = image.HasMipmaps;

        if (this.texture == default)
        {
            this.texture = RS.Singleton.Texture2DCreate(image);
        }
        else
        {
            var newTexture = RS.Singleton.Texture2DCreate(image);
            RS.Singleton.TextureReplace(this.texture, newTexture);
        }
        this.NotifyPropertyListChanged();
        this.EmitChanged();

        this.imageStored = true;
    }
}
