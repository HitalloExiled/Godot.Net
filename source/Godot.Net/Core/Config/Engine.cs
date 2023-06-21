namespace Godot.Net.Core.Config;

using System.Runtime.CompilerServices;
using Godot.Net.Core.OS;

#pragma warning disable IDE0052 // TODO - Remove

public class Engine
{
    private static Engine? singleton;
    public static Engine Singleton => singleton ?? throw new NullReferenceException();

    private readonly Dictionary<Type, object>   singletons           = new();
    private readonly Dictionary<string, double> startupBenchmarkJson = new();

    private long    startupBenchmarkFrom;
    private string? startupBenchmarkSection;
    private long    startupBenchmarkTotalFrom;

    public bool   EditorHint                   { get; set; }
    public uint   Fps                          { get; set; } = 1;
    public int    FrameDelay                   { get; set; }
    public long   FramesDrawn                  { get; set; }
    public uint   FrameTicks                   { get; set; }
    public bool   IsEditorHint                 { get; set; }
    public bool   IsValidationLayersEnabled    { get; set; }
    public int    MaxFps                       { get; set; }
    public int    MaxPhysicsStepsPerFrame      { get; set; } = 8;
    public double PhysicsInterpolationFraction { get; set; }
    public double PhysicsJitterFix             { get; set; } = 0.5;
    public int    PhysicsTicksPerSecond        { get; set; }
    public long   ProcessFrames                { get; set; }
    public double ProcessStep                  { get; set; }
    public bool   ProjectManagerHint           { get; set; }
    public string ShaderCachePath              { get; set; } = "";
    public double TimeScale                    { get; set; } = 1;

    public Engine() => singleton = this;

    public void StartupBegin() => this.startupBenchmarkTotalFrom = OS.Singleton.TicksUsec;

    public void StartupBenchmarkBeginMeasure(string what)
    {
        this.startupBenchmarkSection = what;
        this.startupBenchmarkFrom    = OS.Singleton.TicksUsec;
    }

    public void StartupBenchmarkEndMeasure()
    {
        var total = OS.Singleton.TicksUsec - this.startupBenchmarkFrom / 1000000D;

        this.startupBenchmarkJson[this.startupBenchmarkSection!] = total;
    }

    public void AddSingleton<T>(T instance, [CallerArgumentExpression(nameof(instance))] string expression = null!) where T : notnull
    {
        var type = typeof(T);

        if (ERR_FAIL_COND_MSG(this.singletons.ContainsKey(type), "Can't register singleton that already exists: " + expression))
        {
            return;
        }

        this.singletons.Add(type, instance);
    }
}
