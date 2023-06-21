namespace Godot.Net.Scene.Resources.DefaultTheme;

public static class DefaultFont
{
    public static byte[] FontOpenSansSemiBold { get; }

    static DefaultFont()
    {
        var fontName = Path.Join(AppContext.BaseDirectory, "Assets", "Fonts", "OpenSans_SemiBold.woff2");

        FontOpenSansSemiBold = File.ReadAllBytes(fontName);
    }
}
