#define CAN_DEBUG
#define GLES_OVER_GL

namespace Godot.Net.Drivers.GLES3;

using System.Collections.Generic;
using System.Diagnostics;
using Godot.Net.Core.Math;
using Godot.Net.Core.OS;
using Godot.Net.Drivers.GLES3.Api.Enums;
#if GLES_OVER_GL
using Godot.Net.Drivers.GLES3.Api.Extensions.ARB;
#else
using Godot.Net.Drivers.GLES3.Api.Extensions.KHR;
#endif
using Godot.Net.Drivers.GLES3.Effects;
using Godot.Net.Drivers.GLES3.Environment;
using Godot.Net.Drivers.GLES3.Storage;
using Godot.Net.Servers;
using Godot.Net.Servers.Rendering;
using Godot.Net.Servers.Rendering.Environment;
using Godot.Net.Servers.Rendering.Storage;

using GLES3Utilities = Storage.Utilities;

#pragma warning disable CS0414,IDE0052,CS0649,IDE0044 // TODO Remove

public class RasterizerGLES3 : RendererCompositor, IDisposable
{
    #region private readonly fields
    private readonly RendererCanvasRender?     canvas;
    private readonly Config?                   config;
    private readonly CopyEffects?              copyEffects;
    private readonly RendererFog?              fog;
    private readonly RendererGI?               gi;
    private readonly RendererLightStorage?     lightStorage;
    private readonly RendererMaterialStorage?  materialStorage;
    private readonly RendererMeshStorage?      meshStorage;
    private readonly RendererParticlesStorage? particlesStorage;
    private readonly RendererSceneRender?      scene;
    private readonly RendererTextureStorage?   textureStorage;
    private readonly RendererUtilities?        utilities;
    #endregion private readonly fields

    #region private readonly fields
    private bool   disposed;
    private int    frame;
    private double delta;
    private double timeTotal;
    private ulong  frameNumber;
    #endregion private readonly fields

    #region public readonly properties
    public Config      Config      => this.config      ?? throw new NullReferenceException();
    public CopyEffects CopyEffects => this.copyEffects ?? throw new NullReferenceException();
    #endregion public readonly properties

    #region public overrideed readonly properties
    public override RendererCanvasRender     Canvas           => this.canvas           ?? throw new NullReferenceException();
    public override RendererFog              Fog              => this.fog              ?? throw new NullReferenceException();
    public override ulong                    FrameNumber      => this.frameNumber;
    public override RendererGI               Gi               => this.gi               ?? throw new NullReferenceException();
    public override RendererLightStorage     LightStorage     => this.lightStorage     ?? throw new NullReferenceException();
    public override RendererMaterialStorage  MaterialStorage  => this.materialStorage  ?? throw new NullReferenceException();
    public override RendererMeshStorage      MeshStorage      => this.meshStorage      ?? throw new NullReferenceException();
    public override RendererParticlesStorage ParticlesStorage => this.particlesStorage ?? throw new NullReferenceException();
    public override RendererSceneRender      Scene            => this.scene            ?? throw new NullReferenceException();
    public override RendererTextureStorage   TextureStorage   => this.textureStorage   ?? throw new NullReferenceException();
    public override double                   TotalTime        => this.timeTotal;
    public override RendererUtilities        Utilities        => this.utilities        ?? throw new NullReferenceException();
    #endregion public overrideed readonly properties

    public RasterizerGLES3()
    {
        GL.Initialize();

        var gl = GL.Singleton;

        #if GLES_OVER_GL
        var arbDebugOutput = default(ArbDebugOutput);
        try
        {
            GL.Singleton.TryGetExtension(out arbDebugOutput); // TODO - WHYYYYY???
            if (OS.Singleton.IsStdoutVerbose)
            {
                if (arbDebugOutput != null)
                {
                    gl.Enable(EnableCap.DebugOutputSynchronous);
                    arbDebugOutput.DebugMessageCallback(GlDebugPrint);
                    gl.Enable(EnableCap.DebugOutput);
                }
                else
                {
                    PrintLine("OpenGL debugging not supported!");
                }
            }
        }
        catch (Exception)
        {
            PrintLine($"Can't get {ArbDebugOutput.Name} extension!");
        }
        #endif

        // For debugging
        if (Debugger.IsAttached)
        {
            #if GLES_OVER_GL
            if (OS.Singleton.IsStdoutVerbose && arbDebugOutput != null)
            {
                // arbDebugOutput.DebugMessageControl(ARB.DebugSourceApiArb, ARB.DebugTypeErrorArb,              ARB.DebugSeverityHighArb, 0, 0, true);
                // arbDebugOutput.DebugMessageControl(ARB.DebugSourceApiArb, ARB.DebugTypeDeprecatedBehaviorArb, ARB.DebugSeverityHighArb, 0, 0, true);
                // arbDebugOutput.DebugMessageControl(ARB.DebugSourceApiArb, ARB.DebugTypeUndefinedBehaviorArb,  ARB.DebugSeverityHighArb, 0, 0, true);
                // arbDebugOutput.DebugMessageControl(ARB.DebugSourceApiArb, ARB.DebugTypePortabilityArb,        ARB.DebugSeverityHighArb, 0, 0, true);
                // arbDebugOutput.DebugMessageControl(ARB.DebugSourceApiArb, ARB.DebugTypePerformanceArb,        ARB.DebugSeverityHighArb, 0, 0, true);
                // arbDebugOutput.DebugMessageControl(ARB.DebugSourceApiArb, ARB.DebugTypeOtherArb,              ARB.DebugSeverityHighArb, 0, 0, true);
                //     glDebugMessageInsertARB(
                //          GL_DEBUG_SOURCE_API_ARB,
                //          GL_DEBUG_TYPE_OTHER_ARB, 1,
                //          GL_DEBUG_SEVERITY_HIGH_ARB, 5, "hello");
            }
            #else
            if (OS.Singleton.IsStdoutVerbose)
            {
                if (gl.TryGetExtension<KhrDebug>(out var khrDebug))
                {
                    PrintLine("godot: ENABLING GL DEBUG");
                    gl.Enable(EnableCap.DebugOutputSynchronous);
                    khrDebug!.DebugMessageCallback(this.GlDebugPrint, default(nint));
                    gl.Enable(EnableCap.DebugOutput);
                }
            }
            #endif
        }

        // OpenGL needs to be initialized before initializing the Rasterizers
        this.config           = new Config();
        this.utilities        = new GLES3Utilities();
        this.textureStorage   = new TextureStorage();
        this.materialStorage  = new MaterialStorage();
        this.meshStorage      = new MeshStorage();
        this.particlesStorage = new ParticlesStorage();
        this.lightStorage     = new LightStorage();
        this.copyEffects      = new CopyEffects();
        this.gi               = new GI();
        this.fog              = new RendererFog();
        this.canvas           = new RasterizerCanvasGLES3();
        this.scene            = new RasterizerSceneGLES3();
    }

    ~RasterizerGLES3() => this.Dispose();

    #region private static methods

    private static void BlitRenderTargetToScreen(Guid renderTargetId, int screen, Rect2<RealT> screenRect, uint layer, bool first)
    {
        _ = screen;

        var rt = Storage.TextureStorage.Singleton.GetRenderTarget(renderTargetId);

        if (ERR_FAIL_COND(rt == null))
        {
            return;
        }

        // We normally render to the render target upside down, so flip Y when blitting to the screen.
        var flipY = true;
        if (rt!.Overridden.Color != default)
        {
            // If we've overridden the render target's color texture, that means we
            // didn't render upside down, so we don't need to flip it.
            // We're probably rendering directly to an XR device.
            flipY = false;
        }

        var gl = GL.Singleton;

        var readFbo = 0U;

        if (rt.ViewCount > 1)
        {
            gl.GenFramebuffers(out readFbo);
            gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, readFbo);
            gl.FramebufferTextureLayer(FramebufferTarget.ReadFramebuffer, FramebufferAttachment.ColorAttachment0, rt.Color, 0, (int)layer);
        }
        else
        {
            gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, rt.Fbo);
        }

        // gl.ReadBuffer(GL_COLOR_ATTACHMENT0);
        gl.BindFramebuffer(FramebufferTarget.DrawFramebuffer, Storage.TextureStorage.SystemFbo);

        if (first)
        {
            var winSize = DisplayServer.Singleton.WindowGetSize(0);
            if (screenRect.Position != default || screenRect.Size.As<int>() != rt.Size)
            {
                // Viewport doesn't cover entire window so clear window to black before blitting.
                gl.Viewport(0, 0, winSize.X, winSize.Y);
                gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
                gl.Clear(ClearBufferMask.ColorBufferBit);
            }
        }

        var screenRectEnd = screenRect.End;

        gl.BlitFramebuffer(
            0,
            0,
            rt.Size.X,
            rt.Size.Y,
            (int)screenRect.Position.X,
            (int)(flipY ? screenRectEnd.Y : screenRect.Position.Y),
            (int)screenRectEnd.X,
            (int)(flipY ? screenRect.Position.Y : screenRectEnd.Y),
            ClearBufferMask.ColorBufferBit,
            BlitFramebufferFilter.Nearest
        );

        if (readFbo != 0)
        {
            gl.BindFramebuffer(FramebufferTarget.ReadFramebuffer, 0);
            gl.DeleteFramebuffers(readFbo);
        }
    }

    private static void GlDebugPrint(GLEnum source, GLEnum type, uint id, GLEnum severity, string message)
    {
        if (type == GLEnum.DebugTypeOtherARB)
        {
            return;
        }

        if (type == GLEnum.DebugTypePerformanceARB)
        {
            return; //these are ultimately annoying, so removing for now
        }

        var debSource = "";
        var debType   = "";
        var debSev    = "";

        if (source == GLEnum.DebugSourceApiARB)
        {
            debSource = "OpenGL";
        }
        else if (source == GLEnum.DebugSourceWindowSystemARB)
        {
            debSource = "Windows";
        }
        else if (source == GLEnum.DebugSourceShaderCompilerARB)
        {
            debSource = "Shader Compiler";
        }
        else if (source == GLEnum.DebugSourceThirdPartyARB)
        {
            debSource = "Third Party";
        }
        else if (source == GLEnum.DebugSourceApplicationARB)
        {
            debSource = "Application";
        }
        else if (source == GLEnum.DebugSourceOtherARB)
        {
            debSource = "Other";
        }

        if (type == GLEnum.DebugTypeErrorARB)
        {
            debType = "Error";
        }
        else if (type == GLEnum.DebugTypeDeprecatedBehaviorARB)
        {
            debType = "Deprecated behavior";
        }
        else if (type == GLEnum.DebugTypeUndefinedBehaviorARB)
        {
            debType = "Undefined behavior";
        }
        else if (type == GLEnum.DebugTypePortabilityARB)
        {
            debType = "Portability";
        }
        else if (type == GLEnum.DebugTypePerformanceARB)
        {
            debType = "Performance";
        }
        else if (type == GLEnum.DebugTypeOtherARB)
        {
            debType = "Other";
        }

        if (severity == GLEnum.DebugSeverityHighARB)
        {
            debSev = "High";
        }
        else if (severity == GLEnum.DebugSeverityMediumARB)
        {
            debSev = "Medium";
        }
        else if (severity == GLEnum.DebugSeverityLowARB)
        {
            debSev = "Low";
        }

        var output = "GL ERROR: Source: " + debSource + "\tType: " + debType + "\tID: " + id + "\tSeverity: " + debSev + "\tMessage: " + message;

        ERR_PRINT(output);
    }

    private static RasterizerGLES3 CreateCurrent() => new();
    #endregion private static methods

    #region public static methods
    public static void MakeCurrent()
    {
        CreateFunc = CreateCurrent;
        IsLowEnd   = true;
    }
    #endregion public static methods

    #region public methods

    public void Dispose()
    {
        if (!this.disposed)
        {
            this.disposed = true;

            GC.SuppressFinalize(this);

            GL.Finish();
        }
    }
    #endregion public methods


    #region public overrided methods
    public override void BeginFrame(double frameStep)
    {
        this.frame++;
        this.delta = frameStep;

        this.timeTotal += frameStep;

        var timeRollOver = GLOBAL_GET<double>("rendering/limits/time/time_rollover_secs");
        this.timeTotal %= timeRollOver;

        this.Canvas.Time    = this.timeTotal;
        this.Scene.Time     = this.timeTotal;
        this.Scene.TimeStep = frameStep;

        ((GLES3Utilities)this.Utilities).CaptureTimestampsBegin();
    }

    public override void BlitRenderTargetsToScreen(int screen, IList<BlitToScreen> renderTargets, int ammount)
    {
        for (var i = 0; i < ammount; i++)
        {
            var blit = renderTargets[i];

            var idRt = blit.RenderTarget;

            var dstRect = blit.DstRect.As<RealT>();

            BlitRenderTargetToScreen(idRt, screen, dstRect, blit.MultiView.UseLayer ? blit.MultiView.Layer : 0, i == 0);
        }
    }

    public override void EndFrame(double frameStep) => throw new NotImplementedException();

    public override void EndFrame(bool swapBuffers)
    {
        if (swapBuffers)
        {
            DisplayServer.Singleton.SwapBuffers();
        }
        else
        {
            GL.Singleton.Finish();
        }
    }

    public override void Initialize() => PrintLine("OpenGL Renderer: " + RS.Singleton.VideoAdapterName);
    public override void PrepareForBlittingRenderTargets()
    {
        // This is a hack, but this function is called one time after all viewports have been updated.
        // So it marks the end of the frame for all viewports
        // In the OpenGL renderer we have to call end_frame for each viewport so we can swap the
        // buffers for each window before proceeding to the next.
        // This allows us to only increment the frame after all viewports are done.
        var utils = GLES3Utilities.Singleton;
        utils.CaptureTimestampsEnd();
    }
    #endregion public overrided methods
}
