namespace Godot.Net.Editor;

using System.Runtime.InteropServices;
using Godot.Net.Core.Config;
using Godot.Net.Core.OS;
using Godot.Net.Main;

#pragma warning disable IDE0044, IDE0052 // TODO Remove

public class EditorPaths
{
    private static EditorPaths? singleton;

    private readonly string exportTemplatesFolder  = "export_templates";
    private readonly string featureProfilesFolder  = "feature_profiles";
    private readonly string scriptTemplatesFolder  = "script_templates";
    private readonly string textEditorThemesFolder = "text_editor_themes";

    private string cacheDir       = "";
    private string dataDir        = "";
    private string projectDataDir = "";

    public static bool        IsInitialized => singleton != null;
    public static EditorPaths Singleton     => singleton ?? throw new NullReferenceException();

    public bool   ArePathsValid       { get; }
    public string ConfigDir           { get; } = "";
    public bool   IsSelfContained     { get; }
    public string SelfContainedFile   { get; } = "";
    public string TextEditorThemesDir => Path.Join(this.ConfigDir, this.textEditorThemesFolder);

    private EditorPaths()
    {
        singleton = this;

        this.projectDataDir = ProjectSettings.Singleton.ProjectDataPath;

        // Self-contained mode if a `._sc_` or `_sc_` file is present in executable dir.
        var exePath = AppContext.BaseDirectory;

        // On macOS, look outside .app bundle, since .app bundle is read-only.
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && exePath.EndsWith("MacOS") && Path.Join(exePath, "..").EndsWith("Contents"))
        {
            exePath = Path.Join(exePath, "../../..");
        }
        {
            if (File.Exists(exePath + "/._sc_"))
            {
                this.IsSelfContained = true;
                this.SelfContainedFile = exePath + "/._sc_";
            }
            else if (File.Exists(exePath + "/_sc_"))
            {
                this.IsSelfContained = true;
                this.SelfContainedFile = exePath + "/_sc_";
            }
        }

        string dataPath;
        string configPath;
        string cachePath;

        if (this.IsSelfContained)
        {
            // editor is self contained, all in same folder
            dataPath = exePath;
            this.dataDir = Path.Join(dataPath, "editor_data");
            configPath = exePath;
            this.ConfigDir = this.dataDir;
            cachePath = exePath;
            this.cacheDir = Path.Join(this.dataDir, "cache");
        }
        else
        {
            // Typically XDG_DATA_HOME or %APPDATA%.
            dataPath = Path.GetTempPath();
            this.dataDir = Path.Join(dataPath, OS.GodotDirName);

            // Can be different from dataPath e.g. on Linux or macOS.
            configPath = OS.ConfigPath;
            this.ConfigDir = Path.Join(configPath, OS.GodotDirName);

            // Can be different from above paths, otherwise a subfolder of data_dir.
            cachePath = OS.Singleton.CachePath;
            this.cacheDir = cachePath == dataPath ? Path.Join(this.dataDir, "cache") : Path.Join(cachePath, OS.GodotDirName);
        }

        this.ArePathsValid = !string.IsNullOrEmpty(dataPath) && !string.IsNullOrEmpty(configPath) && !string.IsNullOrEmpty(cachePath);
        if (ERR_FAIL_COND_MSG(!this.ArePathsValid, "Editor data, config, or cache paths are invalid."))
        {
            return;
        }

        // Validate or create each dir and its relevant subdirectories.

        // Data dir.
        try
        {
            Directory.CreateDirectory(this.dataDir);
        }
        catch (Exception)
        {

            ERR_PRINT($"Could not create editor data directory: {this.dataDir}");
            this.ArePathsValid = false;
        }

        var dataDirExportTemplatesFolder = Path.Join(this.dataDir, this.exportTemplatesFolder);

        if (!Directory.Exists(dataDirExportTemplatesFolder))
        {
            Directory.CreateDirectory(dataDirExportTemplatesFolder);
        }

        // Config dir.
        try
        {
            Directory.CreateDirectory(this.ConfigDir);
        }
        catch (Exception)
        {
            ERR_PRINT($"Could not create editor config directory: {this.ConfigDir}");
            this.ArePathsValid = false;
        }

        var configDirTextEditorThemesFolder = Path.Join(this.ConfigDir, this.textEditorThemesFolder);
        var configDirScriptTemplatesFolder  = Path.Join(this.ConfigDir, this.scriptTemplatesFolder);
        var configDirFeatureProfilesFolder  = Path.Join(this.ConfigDir, this.featureProfilesFolder);

        if (!Directory.Exists(configDirTextEditorThemesFolder))
        {
            Directory.CreateDirectory(configDirTextEditorThemesFolder);
        }
        if (!Directory.Exists(configDirScriptTemplatesFolder))
        {
            Directory.CreateDirectory(configDirScriptTemplatesFolder);
        }
        if (!Directory.Exists(configDirFeatureProfilesFolder))
        {
            Directory.CreateDirectory(configDirFeatureProfilesFolder);
        }

        // Cache dir.
        try
        {
            Directory.CreateDirectory(this.cacheDir);
        }
        catch (Exception)
        {
            ERR_PRINT($"Could not create editor cache directory: {this.cacheDir}");
            this.ArePathsValid = false;
        }

        // Validate or create project-specific editor data dir,
        // including shader cache subdir.
        if (Engine.Singleton.ProjectManagerHint || Main.IsCmdlineTool && !ProjectSettings.Singleton.IsProjectLoaded)
        {
            // Nothing to create, use shared editor data dir for shader cache.
            Engine.Singleton.ShaderCachePath = this.dataDir;
        }
        else
        {
            try
            {
                Directory.CreateDirectory(this.projectDataDir);
            }
            catch (Exception)
            {

                ERR_PRINT($"Could not create project data directory ({this.projectDataDir}) in: {Path.GetPathRoot(this.projectDataDir)}");
                this.ArePathsValid = false;
            }

            // Check that the project data directory '.godotignore' file exists
            var projectDataGdignoreFilePath = Path.Join(this.projectDataDir, ".godotignore");
            if (!File.Exists(projectDataGdignoreFilePath))
            {
                // Add an empty .gdignore file to avoid scan.

                try
                {
                    using var f = File.AppendText(projectDataGdignoreFilePath);
                    f.WriteLine("");
                }
                catch (Exception)
                {
                    ERR_PRINT("Failed to create file " + projectDataGdignoreFilePath);
                }
            }

            Engine.Singleton.ShaderCachePath = this.projectDataDir;

            var projectDataDirEditor = Path.Join(this.projectDataDir, "editor");

            // Editor metadata dir.
            if (!Directory.Exists(projectDataDirEditor))
            {
                Directory.CreateDirectory(projectDataDirEditor);
            }
            // Imported assets dir.
            var importedFilesPath = ProjectSettings.Singleton.ImportedFilesPath;

            var projectDataDirImportedFilesPath = Path.Join(this.projectDataDir, importedFilesPath);

            if (!Directory.Exists(projectDataDirImportedFilesPath))
            {
                Directory.CreateDirectory(projectDataDirImportedFilesPath);
            }
        }
    }

    public static void Create()
    {
        if (ERR_FAIL_COND(singleton != null))
        {
            return;
        }

        _ = new EditorPaths();
    }
}
