namespace Godot.Net.Editor;

public static class EditorIcons
{
    public static HashSet<string>            BgThumbs { get; } = new();
    public static HashSet<string>            MdThumbs { get; } = new();
    public static Dictionary<string, string> Sources  { get; } = new();

    static EditorIcons()
    {
        var iconsPath = Path.Join(AppContext.BaseDirectory, "Assets", "Editor", "Icons");

        var files = Directory.GetFiles(iconsPath);

        foreach (var file in files)
        {
            var name = Path.GetFileNameWithoutExtension(file);

            if (name.EndsWith("MediumThumb"))
            {
                MdThumbs.Add(name);
            }

            if (name.EndsWith("BigThumb") || name.EndsWith("GodotFile"))
            {
                BgThumbs.Add(name);
            }

            Sources[name] = File.ReadAllText(file);
        }
    }
}
