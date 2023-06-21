namespace Godot.Net.Core.IO;

using Godot.Net.Core.Error;

public static class ImageLoader
{
    private static readonly HashSet<ImageFormatLoader> loaders = new();

    public static void AddImageFormatLoader(ImageFormatLoader loader) => loaders.Add(loader);
    public static Error LoadImage(string vrsTexturePath, Image vrsImage) => throw new NotImplementedException();
    public static void RemoveImageFormatLoader(ImageFormatLoader loader) => loaders.Remove(loader);
}
