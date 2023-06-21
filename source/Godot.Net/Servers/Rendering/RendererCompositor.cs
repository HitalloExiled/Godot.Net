namespace Godot.Net.Servers.Rendering;

using System.Collections.Generic;
using Godot.Net.Servers.Rendering.Environment;
using Godot.Net.Servers.Rendering.Storage;

#pragma warning disable IDE0044, CS0649 // TODO Remove

public abstract partial class RendererCompositor
{
    #region private static fields
    private static Func<RendererCompositor>? createFunc;
    private static RendererCompositor?       singleton;
    #endregion private static fields

    #region public static readonly properties
    public static Func<RendererCompositor> CreateFunc { get => createFunc ?? throw new NullReferenceException(); set => createFunc = value; }
    public static RendererCompositor       Singleton  =>  singleton ?? throw new NullReferenceException();
    public static bool                     IsLowEnd   { get; protected set; }
    #endregion public static readonly properties

    #region public abstract readonly properties
    public abstract RendererCanvasRender     Canvas           { get; }
    public abstract RendererFog              Fog              { get; }
    public abstract ulong                    FrameNumber      { get; }
    public abstract RendererGI               Gi               { get; }
    public abstract RendererLightStorage     LightStorage     { get; }
    public abstract RendererMaterialStorage  MaterialStorage  { get; }
    public abstract RendererMeshStorage      MeshStorage      { get; }
    public abstract RendererParticlesStorage ParticlesStorage { get; }
    public abstract RendererSceneRender      Scene            { get; }
    public abstract RendererTextureStorage   TextureStorage   { get; }
    public abstract RendererUtilities        Utilities        { get; }
    public abstract double                   TotalTime        { get; }
    #endregion public abstract readonly properties

    #region public readonly properties
    public bool IsXrEnabled { get; }
    #endregion public readonly properties

    public RendererCompositor()
    {
        singleton = this;

        this.IsXrEnabled = XRServer.XrMode == XRServer.XRMode.XRMODE_DEFAULT
            ? GLOBAL_GET<bool>("xr/shaders/enabled")
            : XRServer.XrMode == XRServer.XRMode.XRMODE_ON;
    }

    public static RendererCompositor Create() => CreateFunc();

    #region public abstract methods
    public abstract void BeginFrame(double frameStep);
    public abstract void BlitRenderTargetsToScreen(int screen, IList<BlitToScreen> renderTargets, int ammount);
    public abstract void EndFrame(bool swapBuffers);
    public abstract void EndFrame(double frameStep);
    public abstract void Initialize();
    public abstract void PrepareForBlittingRenderTargets();
    #endregion public abstract methods
}
