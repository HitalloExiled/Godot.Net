namespace Godot.Net.Servers.Rendering;

using System.Runtime.CompilerServices;
using Godot.Net.Core;
using Godot.Net.Core.Generics;
using Godot.Net.Core.IO;
using Godot.Net.Core.Math;
using Godot.Net.Core.Object;
using Godot.Net.Core.OS;
using Godot.Net.Servers.Rendering.Storage;

#pragma warning disable IDE0060, IDE0059, IDE0052, CS0414, IDE0044, CS0649, CS8618 // TODO - Remove

#pragma warning disable IDE0001
public class RenderingServerDefault : RenderingServer, IDisposable
#pragma warning restore IDE0001
{
    private static int changes;

    private readonly CommandQueueMT commandQueue = new();
    private readonly bool           createThread;
    private readonly SafeFlags      drawThreadUp = new();
    private readonly SafeFlags      exit         = new();

    private bool    disposed;
    private long    frameProfileFrame;
    private double  frameSetupTime;
    private Thread? serverThread;

    public override Color        DefaultClearColor   => RSG.Utilities.DefaultClearColor;
    public override bool         HasChanged          => changes > 0;
    public override bool         IsLowEnd            => RendererCompositor.IsLowEnd;
    public override Vector2<int> MaximumViewportSize => RSG.Utilities.MaximumViewportSize;
    public override string       VideoAdapterName    => RSG.Utilities.VideoAdapterName;

    public RenderingServerDefault(bool createThread) : base()
    {
        base.Init();

        this.createThread = createThread;

        if (!createThread)
        {
            this.serverThread = Thread.CurrentThread;
        }

        RSG.Threaded          = createThread;
        RSG.Canvas            = new RendererCanvasCull();
        RSG.Viewport          = new RendererViewport();
        var sr                = new RendererSceneCull();
        RSG.CameraAttributes  = new RendererCameraAttributes();
        RSG.Scene             = sr;
        RSG.Rasterizer        = RendererCompositor.Create();
        RSG.Utilities         = RSG.Rasterizer.Utilities;
        RSG.LightStorage      = RSG.Rasterizer.LightStorage;
        RSG.MaterialStorage   = RSG.Rasterizer.MaterialStorage;
        RSG.MeshStorage       = RSG.Rasterizer.MeshStorage;
        RSG.ParticlesStorage  = RSG.Rasterizer.ParticlesStorage;
        RSG.TextureStorage    = RSG.Rasterizer.TextureStorage;
        RSG.Gi                = RSG.Rasterizer.Gi;
        RSG.Fog               = RSG.Rasterizer.Fog;
        RSG.CanvasRender      = RSG.Rasterizer.Canvas;
        sr.SceneRender        = RSG.Rasterizer.Scene;

        this.frameProfileFrame = 0L;
    }

    ~RenderingServerDefault() =>
        this.Dispose(false);

    private static void SYNC_DEBUG([CallerMemberName] string caller = "") =>
        PrintLine($"sync on: {caller}");

    #region private static methods
    private static void InternalInit() =>
        RSG.Rasterizer.Initialize();

    private static void ThreadCallback(RenderingServerDefault instance)
    {
        var vsmt = instance;

        vsmt.ThreadLoop();
    }
    #endregion private static methods

    #region public static methods
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void RedrawRequest() =>
        changes++;

    public static void ThreadFlush() { /* NOOP */ }
    #endregion public static methods

    #region private methods
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private Guid CreateResource(Action<Guid> action, bool isSync = default)
    {
        var id = Guid.NewGuid();

        this.RunTask(() => action(id), isSync);

        return id;
    }

    private void InternalDraw(bool swapBuffers, double frameStep)
    {
        //needs to be done before changes is reset to 0, to not force the editor to redraw
        this.NotifyPreFrameDraw();

        changes = 0;

        RSG.Rasterizer.BeginFrame(frameStep);

        TIMESTAMP_BEGIN();

        var timeUsec = OS.Singleton.TicksUsec;

        RSG.Scene.Update(); //update scenes stuff before updating instances

        this.frameSetupTime = (OS.Singleton.TicksUsec - timeUsec) / 1000.0;

        // RSG::particles_storage->update_particles(); //need to be done after instances are updated (colliders and particle transforms), and colliders are rendered

        // RSG::scene->render_probes();

        RSG.Viewport.DrawViewports();
        RSG.CanvasRender.Update();

        if (OS.Singleton.CurrentRenderingDriverName != "opengl3")
        {
            // Already called for gl_compatibility renderer.
            RSG.Rasterizer.EndFrame(swapBuffers);
        }

        // XRServer *xr_server = XRServer::get_singleton();
        // if (xr_server != nullptr) {
        //     // let our XR server know we're done so we can get our frame timing
        //     xr_server->end_frame();
        // }

        RSG.Canvas.UpdateVisibilityNotifiers();
        RSG.Scene.UpdateVisibilityNotifiers();

        // while (frame_drawn_callbacks.front())
        // {
        //     Callable c = frame_drawn_callbacks.front()->get();
        //     Variant result;
        //     Callable::CallError ce;
        //     c.callp(nullptr, 0, result, ce);
        //     if (ce.error != Callable::CallError::CALL_OK) {
        //         String err = Variant::get_callable_error_text(c, nullptr, 0, ce);
        //         ERR_PRINT("Error calling frame drawn function: " + err);
        //     }

        //     frame_drawn_callbacks.pop_front();
        // }

        this.NotifyPostFrameDraw();

        // if (RSG::utilities->get_captured_timestamps_count()) {
        //     Vector<FrameProfileArea> new_profile;
        //     if (RSG::utilities->capturing_timestamps) {
        //         new_profile.resize(RSG::utilities->get_captured_timestamps_count());
        //     }

        //     uint64_t base_cpu = RSG::utilities->get_captured_timestamp_cpu_time(0);
        //     uint64_t base_gpu = RSG::utilities->get_captured_timestamp_gpu_time(0);
        //     for (uint32_t i = 0; i < RSG::utilities->get_captured_timestamps_count(); i++) {
        //         uint64_t time_cpu = RSG::utilities->get_captured_timestamp_cpu_time(i);
        //         uint64_t time_gpu = RSG::utilities->get_captured_timestamp_gpu_time(i);

        //         String name = RSG::utilities->get_captured_timestamp_name(i);

        //         if (name.begins_with("vp_")) {
        //             RSG::viewport->handle_timestamp(name, time_cpu, time_gpu);
        //         }

        //         if (RSG::utilities->capturing_timestamps) {
        //             new_profile.write[i].gpu_msec = double((time_gpu - base_gpu) / 1000) / 1000.0;
        //             new_profile.write[i].cpu_msec = double(time_cpu - base_cpu) / 1000.0;
        //             new_profile.write[i].name = RSG::utilities->get_captured_timestamp_name(i);
        //         }
        //     }

        //     frame_profile = new_profile;
        // }

        // frame_profile_frame = RSG::utilities->get_captured_timestamps_frame();

        // if (print_gpu_profile) {
        //     if (print_frame_profile_ticks_from == 0) {
        //         print_frame_profile_ticks_from = OS::get_singleton()->get_ticks_usec();
        //     }
        //     double total_time = 0.0;

        //     for (int i = 0; i < frame_profile.size() - 1; i++) {
        //         String name = frame_profile[i].name;
        //         if (name[0] == '<' || name[0] == '>') {
        //             continue;
        //         }

        //         double time = frame_profile[i + 1].gpu_msec - frame_profile[i].gpu_msec;

        //         if (name[0] != '<' && name[0] != '>') {
        //             if (print_gpu_profile_task_time.has(name)) {
        //                 print_gpu_profile_task_time[name] += time;
        //             } else {
        //                 print_gpu_profile_task_time[name] = time;
        //             }
        //         }
        //     }

        //     if (frame_profile.size()) {
        //         total_time = frame_profile[frame_profile.size() - 1].gpu_msec;
        //     }

        //     uint64_t ticks_elapsed = OS::get_singleton()->get_ticks_usec() - print_frame_profile_ticks_from;
        //     print_frame_profile_frame_count++;
        //     if (ticks_elapsed > 1000000) {
        //         print_line("GPU PROFILE (total " + rtos(total_time) + "ms): ");

        //         float print_threshold = 0.01;
        //         for (const KeyValue<String, float> &E : print_gpu_profile_task_time) {
        //             double time = E.value / double(print_frame_profile_frame_count);
        //             if (time > print_threshold) {
        //                 print_line("\t-" + E.key + ": " + rtos(time) + "ms");
        //             }
        //         }
        //         print_gpu_profile_task_time.clear();
        //         print_frame_profile_ticks_from = OS::get_singleton()->get_ticks_usec();
        //         print_frame_profile_frame_count = 0;
        //     }
        // }

        // RSG::utilities->update_memory_info();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void RunTask(Action task, bool isSync = default)
    {
        if (!isSync && this.serverThread != Thread.CurrentThread)
        {
            this.commandQueue.PushAndSync(task);
        }
        else
        {
            this.commandQueue.FlushIfPending();

            task();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private Ref<T> RunTask<T>(Func<T?> task, bool isSync = default)
    {
        if (!isSync && this.serverThread != Thread.CurrentThread)
        {
            this.commandQueue.PushAndRet(task, out var ret);

            SYNC_DEBUG();

            return ret!;
        }
        else
        {
            this.commandQueue.FlushIfPending();

            return new(task());
        }
	}

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void RunTaskWithRedraw(Action action)
    {
        RedrawRequest();
        this.RunTask(action);
    }

    private void ThreadLoop()
    {
        this.serverThread = Thread.CurrentThread;

        DisplayServer.Singleton.MakeRenderingThread();

        InternalInit();

        this.drawThreadUp.Value = true;

        while (this.exit.Value)
        {
            // flush commands one by one, until exit is requested
            this.commandQueue.WaitAndFlush();
        }

        this.commandQueue.FlushAll(); // flush all

        // this.Finish(); Not needed in dotnet
    }

    private void ThreadExit() => this.exit.Value = true;

    // private void Finish() => RSG.Rasterizer.Finalize(); Not needed in dotnet
    #endregion private methods

    #region protected virtual methods
    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                this.commandQueue.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            this.disposed = true;
        }
    }
    #endregion protected virtual methods

    #region public methods
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion public methods

    #region public override methods
    // FUNCRIDSPLIT(canvas)
    public override Guid CanvasCreate() =>
        this.CreateResource(RSG.Canvas.CanvasInitialize);

    public override void CanvasItemAddRect(Guid item, in Rect2<float> rect, in Color color) => throw new NotImplementedException();

    // FUNC9(canvas_item_add_triangle_array, RID, const Vector<int> &, const Vector<Point2> &, const Vector<Color> &, const Vector<Point2> &, const Vector<int> &, const Vector<float> &, RID, int)
    public override void CanvasItemAddTriangleArray(Guid item, IList<int> indices, IList<Vector2<RealT>> points, IList<Color> colors, IList<Vector2<RealT>>? uvs = default, IList<int>? bones = default, IList<float>? weights = default, Guid texture = default, int count = -1) =>
        this.RunTask(() => RSG.Canvas.CanvasItemAddTriangleArray(item, indices, points, colors, uvs ?? Array.Empty<Vector2<RealT>>(), bones ?? Array.Empty<int>(), weights ?? Array.Empty<float>(), texture, count));

    // FUNC1(canvas_item_clear, RID)
    public override void CanvasItemClear(Guid canvasItemId) =>
        this.RunTask(() => RSG.Canvas.CanvasItemClear(canvasItemId));

    // FUNCRIDSPLIT(canvas_item)
    public override Guid CanvasItemCreate() =>
        this.CreateResource(RSG.Canvas.CanvasItemInitialize);

    // FUNC2(canvas_item_set_clip, RID, bool)
    public override void CanvasItemSetClip(Guid item, bool clip) =>
        this.RunTaskWithRedraw(() => RSG.Canvas.CanvasItemSetClip(item, clip));

    // FUNC3(canvas_item_set_custom_rect, RID, bool, const Rect2 &)
    public override void CanvasItemSetCustomRect(Guid item, bool customRect, Rect2<float> rect = default) =>
        this.RunTaskWithRedraw(() => RSG.Canvas.CanvasItemSetCustomRect(item, customRect, rect));

    // FUNC2(canvas_item_set_default_texture_filter, RID, CanvasItemTextureFilter)
    public override void CanvasItemSetDefaultTextureFilter(Guid canvasItemId, CanvasItemTextureFilter textureFilter) =>
        this.RunTaskWithRedraw(() => RSG.Canvas.CanvasItemSetDefaultTextureFilter(canvasItemId, textureFilter));

    // FUNC2(canvas_item_set_default_texture_filter, RID, CanvasItemTextureRepeat)
    public override void CanvasItemSetDefaultTextureRepeat(Guid canvasItemId, CanvasItemTextureRepeat textureRepeat) =>
        this.RunTaskWithRedraw(() => RSG.Canvas.CanvasItemSetDefaultTextureRepeat(canvasItemId, textureRepeat));

    // FUNC2(canvas_item_set_draw_index, RID, int)
    public override void CanvasItemSetDrawIndex(Guid canvasItemId, int drawIndex) =>
        this.RunTaskWithRedraw(() => RSG.Canvas.CanvasItemSetDrawIndex(canvasItemId, drawIndex));

    // FUNC2(canvas_item_set_parent, RID, Guid)
    public override void CanvasItemSetParent(Guid canvasItemId, Guid canvasItemParentId) =>
        this.RunTaskWithRedraw(() => RSG.Canvas.CanvasItemSetParent(canvasItemId, canvasItemParentId));

    // FUNC2(canvas_item_set_transform, RID, Guid)
    public override void CanvasItemSetTransform(Guid item, Transform2D<RealT> transform) =>
        this.RunTaskWithRedraw(() => RSG.Canvas.CanvasItemSetTransform(item, transform));

    // FUNC2(canvas_item_set_visible, RID, bool)
    public override void CanvasItemSetVisible(Guid canvasItemId, bool visible) =>
        this.RunTaskWithRedraw(() => RSG.Canvas.CanvasItemSetVisible(canvasItemId, visible));

    // FUNC2(canvas_item_set_visibility_layer, RID, uint32_t)
    public override void CanvasItemSetVisibilityLayer(Guid canvasItemId, uint visibilityLayer) =>
        this.RunTaskWithRedraw(() => RSG.Canvas.CanvasItemSetVisibilityLayer(canvasItemId, visibilityLayer));

    public override void Draw(bool swapBuffers, double frameStep)
    {
        var action = () => this.InternalDraw(swapBuffers, frameStep);

        if (this.createThread)
        {
            this.RunTask(action);
        }
        else
        {
            action();
        }
    }

    public override void Finish()
    {
        if (this.createThread)
        {
            this.commandQueue.PushAndSync(this.ThreadExit);
            this.serverThread!.Start();
            this.serverThread.Join();
        }
    }

    public override void Init()
    {
        if (this.createThread)
        {
            PrintVerbose("RenderingServerWrapMT: Creating render thread");

            DisplayServer.Singleton.ReleaseRenderingThread();

            if (this.createThread)
            {
                this.serverThread = new(() => ThreadCallback(this));
                PrintVerbose("RenderingServerWrapMT: Starting render thread");
            }

            while (!this.drawThreadUp.Value)
            {
                Thread.Sleep(1000);
            }

            PrintVerbose("RenderingServerWrapMT: Finished render thread");
        }
        else
        {
            InternalInit();
        }
    }

    // FUNCRIDSPLIT(scenario)
    public override Guid ScenarioCreate() =>
        this.CreateResource(RSG.Scene.ScenarioInitialize);

    public override void ScenarioSetFallbackEnvironment(Guid scenarioId, Guid fallbackEnvironment) => throw new NotImplementedException();

    public override void SetDefaultClearColor(in Color color) =>
        RSG.Viewport.SetDefaultClearColor(color);

    public override void Sync()
    {
        if (this.createThread)
        {
            this.RunTask(ThreadFlush);
        }
        else
        {
            this.commandQueue.FlushAll(); //flush all pending from other threads
        }
    }

    // FUNCRIDTEX1(texture_2d, const Ref<Image> &)
    public override Guid Texture2DCreate(Image image) =>
        this.CreateResource((id) => RSG.TextureStorage.Texture2DInitialize(id, image), RSG.TextureStorage.CanCreateResourcesAsync());

    // FUNCRIDTEX1(texture_proxy, RID)
    public override Guid TextureProxyCreate(Ref<Guid> textureId) =>
        this.CreateResource((id) => RSG.TextureStorage.TextureProxyInitialize(id, textureId.Value), RSG.TextureStorage.CanCreateResourcesAsync());

    // FUNC2(texture_replace, RID, RID)
    public override void TextureReplace(Guid texture, Guid byTexture) =>
        this.RunTaskWithRedraw(() => RSG.TextureStorage.TextureReplace(texture, byTexture));

    // FUNC2(viewport_attach_canvas, RID, RID)
    public override void ViewportAttachCanvas(Guid viewportId, Guid currentCanvas) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportAttachCanvas(viewportId, currentCanvas));

    // FUNC3(viewport_attach_to_screen, RID, const Rect2 &, int)
    public override void ViewportAttachToScreen(Guid viewportId, Rect2<int> screenRect, int window) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportAttachToScreen(viewportId, screenRect, window));

    // FUNCRIDSPLIT(viewport)
    public override Guid ViewportCreate() =>
        this.CreateResource(RSG.Viewport.ViewportInitialize);

    // FUNC1RC(RID, viewport_get_texture, RID)
    public override Ref<Guid> ViewportGetTexture(Guid viewportId) =>
        this.RunTask(() => RSG.Viewport.ViewportGetTexture(viewportId));

    // FUNC2(viewport_set_active, RID, bool)
    public override void ViewportSetActive(Guid viewportId, bool active) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetActive(viewportId, active));

    // FUNC2(viewport_set_canvas_cull_mask, RID, uint32_t)
    public override void ViewportSetCanvasCullMask(Guid viewportId, uint canvasCullMask) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetCanvasCullMask(viewportId, canvasCullMask));

    // FUNC3(viewport_set_canvas_transform, RID, RID, const Transform2D &)
    public override void ViewportSetCanvasTransform(Guid viewportId, Guid canvasId, Transform2D<float> offset) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetCanvasTransform(viewportId, canvasId, offset));

    // FUNC2(viewport_set_fsr_sharpness, RID, float)
    public override void ViewportSetFsrSharpness(Guid viewportId, float fsrSharpness) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetFsrSharpness(viewportId, fsrSharpness));

    // FUNC2(viewport_set_global_canvas_transform, RID, const Transform2D &)
    public override void ViewportSetGlobalCanvasTransform(Guid viewportId, Transform2D<RealT> transform) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetGlobalCanvasTransform(viewportId, transform));

    // FUNC3(viewport_set_positional_shadow_atlas_quadrant_subdivision, RID, int, int)
    public override void ViewportSetPositionalShadowAtlasQuadrantSubdivision(Guid viewportId, int quadrant, int subdiv) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetPositionalShadowAtlasQuadrantSubdivision(viewportId, quadrant, subdiv));

    // FUNC3(viewport_set_positional_shadow_atlas_size, RID, int, bool)
    public override void ViewportSetPositionalShadowAtlasSize(Guid viewportId, int size, bool positionalShadowAtlasSize) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetPositionalShadowAtlasSize(viewportId, size, positionalShadowAtlasSize));

    // FUNC2(viewport_set_mesh_lod_threshold, RID, float)
    public override void ViewportSetMeshLodThreshold(Guid viewportId, float meshLodThreshold) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetMeshLodThreshold(viewportId, meshLodThreshold));

    // FUNC2(viewport_set_msaa_2d, RID, ViewportMSAA)
    public override void ViewportSetMsaa2D(Guid viewportId, ViewportMSAA msaa) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetMsaa2D(viewportId, msaa));

    // FUNC2(viewport_set_msaa_3d, RID, ViewportMSAA)
    public override void ViewportSetMsaa3D(Guid viewportId, ViewportMSAA msaa) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetMsaa3D(viewportId, msaa));

    // FUNC2(viewport_set_parent_viewport, RID, RID)
    public override void ViewportSetParentViewport(Guid viewportId, Guid parentViewportId) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetParentViewport(viewportId, parentViewportId));

    // FUNC2(viewport_set_scaling_3d_mode, RID, ViewportScaling3DMode)
    public override void ViewportSetScaling3DMode(Guid viewportId, ViewportScaling3DMode scaling3DMode) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetScaling3DMode(viewportId, scaling3DMode));

    // FUNC2(viewport_set_scaling_3d_scale, RID, float)
    public override void ViewportSetScaling3DScale(Guid viewportId, float scaling3DScale) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetScaling3DScale(viewportId, scaling3DScale));

    // FUNC2(viewport_set_scenario, RID, RID)
    public override void ViewportSetScenario(Guid viewportId, Guid scenarioId) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetScenario(viewportId, scenarioId));

    public override void ViewportSetScreenSpaceAA(Guid viewportId, ViewportScreenSpaceAA screenSpaceAA) => throw new NotImplementedException();

    // FUNC3(viewport_set_sdf_oversize_and_scale, RID, ViewportSDFOversize, ViewportSDFScale)
    public override void ViewportSetSdfOversizeAndScale(Guid viewportId, ViewportSDFOversize sdfOversize, ViewportSDFScale sdfScale) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetSdfOversizeAndScale(viewportId, sdfOversize, sdfScale));

    // FUNC3(viewport_set_size, RID, int, int)
    public override void ViewportSetSize(Guid viewportId, int width, int height) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetSize(viewportId, width, height));

    // FUNC2(viewport_set_snap_2d_transforms_to_pixel, RID, bool)
    public override void ViewportSetSnap2DTransformsToPixel(Guid viewportId, bool snap2DTransformsToPixel) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetSnap2DTransformsToPixel(viewportId, snap2DTransformsToPixel));

    // FUNC2(viewport_set_snap2_d_vertices_to_pixel, RID, bool)
    public override void ViewportSetSnap2DVerticesToPixel(Guid viewportId, bool snap2DVerticesToPixel) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetSnap2DVerticesToPixel(viewportId, snap2DVerticesToPixel));

    // FUNC2(viewport_set_texture_mipmap_bias, RID, float)
    public override void ViewportSetTextureMipmapBias(Guid viewportId, float textureMipmapBias) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetTextureMipmapBias(viewportId, textureMipmapBias));

    // FUNC2(viewport_set_transparent_background, RID, bool)
    public override void ViewportSetTransparentBackground(Guid viewportId, bool transparentBackground) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetTransparentBackground(viewportId, transparentBackground));

    // FUNC2(viewport_set_update_mode, RID, ViewportUpdateMode)
    public override void ViewportSetUpdateMode(Guid viewportId, ViewportUpdateMode updateMode) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetUpdateMode(viewportId, updateMode));

    // FUNC2(viewport_set_use_debanding, RID, bool)
    public override void ViewportSetUseDebanding(Guid viewportId, bool useDebanding) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetUseDebanding(viewportId, useDebanding));

    public override void ViewportSetUseOcclusionCulling(Guid viewportId, bool useOcclusionCulling) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetUseOcclusionCulling(viewportId, useOcclusionCulling));

    // FUNC2(viewport_set_use_taa, RID, bool)
    public override void ViewportSetUseTAA(Guid viewportId, bool useTAA) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetUseTAA(viewportId, useTAA));

    // FUNC2(viewport_set_vrs_mode, RID, bool)
    public override void ViewportSetVrsMode(Guid viewportId, ViewportVRSMode vrsMode) =>
        this.RunTaskWithRedraw(() => RSG.Viewport.ViewportSetVrsMode(viewportId, vrsMode));
    #endregion public override methods
}

