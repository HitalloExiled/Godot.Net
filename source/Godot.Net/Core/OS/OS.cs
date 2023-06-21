#define TOOLS_ENABLED

namespace Godot.Net.Core.OS;

using System;
using System.Globalization;
using Godot.Net.Core.Config;
using Godot.Net.Core.IO;

#pragma warning disable IDE0051, CS0169, IDE0044, CS0649, IDE0052 // TODO - Remove

public abstract partial class OS
{
    public delegate bool HasServerFeature(string feature);

    #region private static fields
    private static OS?   singleton;
    private static ulong targetTicks;
    #endregion private static fields

    #region public static readonly properties
    public static string GodotDirName  => GodotVersion.VERSION_SHORT_NAME;
    public static string ConfigPath    => Environment.GetEnvironmentVariable("APPDATA") ?? ".";
    public static string DataPath      => ConfigPath;
    public static bool   IsInitialized => singleton != null;
    public static OS     Singleton     => singleton ?? throw new NullReferenceException();
    #endregion public static readonly properties

    #region private readonly fields
    #endregion private readonly fields

    #region private fields
    private bool         stdoutEnabled;
    private List<string> userArgs = new();
    private bool writingMovie;
    #endregion private fields

    #region protected abstract readonly properties
    protected abstract CompositeLogger? Logger { get; }
    #endregion protected abstract readonly properties

    #region protected properties
    protected HasServerFeature? HasServerFeatureCallback { get; set; }
    #endregion protected properties

    #region public abstract readonly properties
    public abstract long   TicksUsec                 { get; }
    public abstract string UserDataDir               { get; }
    #endregion public abstract readonly properties

    #region public readonly properties
    public List<string> CmdlineArgs { get; private set; } = new();
    public string?      Execpath    { get; private set; }

    #endregion public readonly properties

    #region public properties
    public bool                 AllowHidpi                     { get; set; }
    public bool                 AllowLayered                   { get; set; }
    public string?              CurrentRenderingDriverName     { get; set; }
    public string?              CurrentRenderingMethod         { get; set; }
    public int                  DisplayDriverId                { get; set; }
    public int                  ExitCode                       { get; set; }
    public bool                 InLowProcessorUsageMode        { get; set; }
    public bool                 IsLayeredAllowed               { get; set; }
    public bool                 IsStdoutVerbose                { get; set; }
    public bool                 LowProcessorUsageMode          { get; set; }
    public int                  LowProcessorUsageModeSleepUsec { get; set; }
    public MainLoop?            MainLoop                       { get; set; }
    public RenderThreadModeType RenderThreadMode               { get; set; }
    public bool                 VerboseStdout                  { get; set; }
    #endregion public properties

    #region public virtual properties
    public virtual string BundleResourceDir => ".";
    public virtual string CachePath         => ".";
    public virtual string Locale            => CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
    public virtual string ResourceDir       => ProjectSettings.Singleton.ResourcePath;
    #endregion public virtual properties

    public OS() =>
        singleton = this;

    #region protected static methods
    protected static string? GetSafeDirName(in string dirName, bool allowDirSeparator = false)
    {
        var invalidChars = ": * ? \" < > |".Split(" ").ToList();

        if (allowDirSeparator)
        {
            // Dir separators are allowed, but disallow ".." to avoid going up the filesystem
            invalidChars.Add("..");
        }
        else
        {
            invalidChars.Add("/");
        }

        var safeDirName = dirName.Replace("\\", "/").Trim();

        for (var i = 0; i < invalidChars.Count; i++)
        {
            safeDirName = safeDirName.Replace(invalidChars[i], "-");
        }

        return safeDirName;
    }
    #endregion protected static methods

    #region public abstract methods
    public abstract void Alert(string alert, string title = "ALERT!");
    public abstract bool CheckInternalFeatureSupport(string feature);
    public abstract void DelayUsec(uint usec);
    public abstract void FinalizeCore();
    public abstract void Initialize();
    public abstract void Run();
    #endregion public abstract methods

    #region public virtual methods
    public virtual int GetEmbeddedPckOffset() => 0;
    public virtual string? GetSystemDir(SystemDir dir, bool sharedStorage = true) => ".";
    public virtual IList<string> GetSystemFontPathForText(
        string  fontName,
        string  text,
        string? locale  = default,
        string  script  = "",
        int     weight  = 400,
        int     stretch = 100,
        bool    italic  = false
    ) => Array.Empty<string>();
    #endregion public virtual methods

    #region public methods
    public void AddFrameDelay(bool canDraw)
    {
        var frameDelay = Engine.Singleton.FrameDelay;

        if (frameDelay > 0)
        {
            // Add fixed frame delay to decrease CPU/GPU usage. This doesn't take
            // the actual frame time into account.
            // Due to the high fluctuation of the actual sleep duration, it's not recommended
            // to use this as a FPS limiter.
            this.DelayUsec((uint)(frameDelay * 1000));
        }

        // Add a dynamic frame delay to decrease CPU/GPU usage. This takes the
        // previous frame time into account for a smoother result.
        var dynamicDelay = 0UL;
        if (this.LowProcessorUsageMode || !canDraw)
        {
            dynamicDelay = (ulong)this.LowProcessorUsageModeSleepUsec;
        }

        var maxFps = Engine.Singleton.MaxFps;

        if (maxFps > 0 && !Engine.Singleton.IsEditorHint)
        {
            // Override the low processor usage mode sleep delay if the target FPS is lower.
            dynamicDelay = Math.Max(dynamicDelay, (uint)(1000000 / maxFps));
        }

        if (dynamicDelay > 0)
        {
            targetTicks += dynamicDelay;
            var currentTicks = (ulong)Singleton.TicksUsec;

            if (currentTicks < targetTicks)
            {
                this.DelayUsec((uint)(targetTicks - currentTicks));
            }

            currentTicks = (ulong)Singleton.TicksUsec;
            targetTicks = Math.Min(Math.Max(targetTicks, currentTicks - dynamicDelay), currentTicks + dynamicDelay);
        }
    }

    public void EnsureUserDataDir()
    {
        var dd = this.UserDataDir;

        if (Directory.Exists(dd))
        {
            return;
        }

        try
        {
            Directory.CreateDirectory(dd);
        }
        catch (Exception)
        {
            if (ERR_FAIL_COND_MSG(false, $"Error attempting to create data dir: {dd}."))
            {
                return;
            }
        }
    }

    public bool HasFeature(string feature)
    {
        if (feature == "movie")
        {
            return this.writingMovie;
        }

        #if DEBUG
        if (feature == "debug")
        {
            return true;
        }
        #endif // DEBUG

        #if TOOLS_ENABLED
        if (feature == "editor")
        {
            return true;
        }
        #else
        if (feature == "template")
        {
            return true;
        }
        #if DEBUG
        if (feature == "template_debug")
        {
            return true;
        }
        #else
        if (feature == "template_release" || feature == "release")
        {
            return true;
        }
        #endif // DEBUG
        #endif // TOOLS_ENABLED

        #if REAL_T_IS_DOUBLE
        if (feature == "double")
        {
            return true;
        }
        #else
        if (feature == "single")
        {
            return true;
        }
        #endif // REAL_T_IS_DOUBLE

        return this.CheckInternalFeatureSupport(feature)
            || this.HasServerFeatureCallback != null && this.HasServerFeatureCallback.Invoke(feature)
            || ProjectSettings.Singleton.HasCustomFeature(feature);
    }

    public void Print(string message)
    {
        if (!this.stdoutEnabled)
        {
            return;
        }

        this.Logger?.Logv(message, false);
    }

    public void PrintError(string function, string file, int line, string code, string rationale, bool editorNotify, Logger.ErrorType type) =>
        this.Logger!.LogError(function, file, line, code, rationale, editorNotify, type);

    public void SetCmdline(string execpath, List<string> cmdline, List<string> userArgs)
    {
        this.Execpath    = execpath;
        this.CmdlineArgs = cmdline;
        this.userArgs    = userArgs;
    }
    #endregion public methods
}
