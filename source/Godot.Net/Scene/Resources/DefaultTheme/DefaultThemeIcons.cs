namespace Godot.Net.Scene.Resources.DefaultTheme;

public class DefaultThemeIcons
{
    public static Dictionary<string, string> Sources { get; } = new();

    static DefaultThemeIcons()
    {
        var iconsPath = Path.Join(AppContext.BaseDirectory, "Assets", "DefaultTheme", "Icons");

        var files = Directory.GetFiles(iconsPath);

        foreach (var file in files)
        {
            var name = Path.GetFileNameWithoutExtension(file);

            Sources[name] = File.ReadAllText(file);
        }
    }
}
