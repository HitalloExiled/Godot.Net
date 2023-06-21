#define GLES_OVER_GL
namespace Godot.Net.Drivers.GLES3.Storage;

using Godot.Net.Core.Config;
using Godot.Net.Core.Math;
using Godot.Net.Core.OS;
using Godot.Net.Drivers.GLES3.Api.Enums;
using Godot.Net.Servers.Rendering.Storage;

#pragma warning disable CS0414, IDE0052 // TODO Remove

public partial class Utilities : RendererUtilities
{
    private const ushort MAX_QUERIES = 256;
    private const ushort FRAME_COUNT = 3;

    private static Utilities? singleton;

    public static Utilities Singleton => singleton ?? throw new NotImplementedException();

    private readonly Frame[] frames                    = new Frame[FRAME_COUNT];
    private readonly int     maxTimestampQueryElements = MAX_QUERIES;

    private int frame;

    public override Vector2<int> MaximumViewportSize =>
        Config.Singleton != null
            ? new(Config.Singleton.MaxViewportSize[0], Config.Singleton.MaxViewportSize[1])
            : default;

    public override unsafe string VideoAdapterName => GL.Singleton.GetString(StringName.Renderer)!;

    public Utilities()
    {
        singleton = this;
        this.frame = 0;
        for (var i = 0; i < FRAME_COUNT; i++)
        {
            var frame = new Frame();

            GL.Singleton.GenQueries(frame.Queries);

            this.frames[i] = frame;
        }
    }

    public void CaptureTimestampsBegin()
    {
        // frame is incremented at the end of the frame so this gives us the queries for frame - 2. By then they should be ready.
        if (this.frames[this.frame].TimestampCount > 0)
        {
            #if GLES_OVER_GL
            for (var i = 0; i < this.frames[this.frame].TimestampCount; i++)
            {
                GL.Singleton.GetQueryObjectui64v(this.frames[this.frame].Queries[i], QueryObjectParameterName.QueryResult, out var temp);
                this.frames[this.frame].TimestampResultValues[i] = (long)temp;
            }
            #endif

            (this.frames[this.frame].TimestampNames,     this.frames[this.frame].TimestampResultNames)     = (this.frames[this.frame].TimestampResultNames,     this.frames[this.frame].TimestampNames);
            (this.frames[this.frame].TimestampCpuValues, this.frames[this.frame].TimestampCpuResultValues) = (this.frames[this.frame].TimestampCpuResultValues, this.frames[this.frame].TimestampCpuValues);
        }

        this.frames[this.frame].TimestampResultCount = this.frames[this.frame].TimestampCount;
        this.frames[this.frame].TimestampCount       = 0;
        this.frames[this.frame].Index                = Engine.Singleton.FramesDrawn;
        this.CaptureTimestamp("Internal Begin");
    }

    public override void CaptureTimestamp(string name)
    {
        if (ERR_FAIL_COND(this.frames[this.frame].TimestampCount >= this.maxTimestampQueryElements))
        {
            return;
        }

        #if GLES_OVER_GL
        GL.Singleton.QueryCounter(this.frames[this.frame].Queries[this.frames[this.frame].TimestampCount], QueryCounterTarget.Timestamp);
        #endif

        var frame = this.frames[this.frame];

        frame.TimestampNames[this.frames[this.frame].TimestampCount]     = name;
        frame.TimestampCpuValues[this.frames[this.frame].TimestampCount] = OS.Singleton.TicksUsec;
        frame.TimestampCount++;
    }

    public override void CaptureTimestampsBegin(string? name) => throw new NotImplementedException();

    public void CaptureTimestampsEnd()
    {
        this.CaptureTimestamp("Internal End");
        this.frame = (this.frame + 1) % FRAME_COUNT;
    }

    public override void UpdateDirtyResources()
    {
        MaterialStorage.Singleton.UpdateGlobalShaderUniforms();
        MaterialStorage.Singleton.UpdateQueuedMaterials();
        MeshStorage.Singleton.UpdateDirtyMultimeshes();
        TextureStorage.Singleton.UpdateTextureAtlas();
    }

    public override void VisibilityNotifierCall(Guid notifier, bool enter, bool deferred) { /* NOOP */ }
}
