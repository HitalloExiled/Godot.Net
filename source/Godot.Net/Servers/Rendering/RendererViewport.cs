namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core;
using Godot.Net.Core.Config;
using Godot.Net.Core.Math;
using Godot.Net.Core.OS;
using Godot.Net.Servers.XR;

#pragma warning disable CA1822, IDE0059, IDE0044, CS0649, IDE0052, IDE0051 // TODO - Remove

public partial class RendererViewport
{
    private readonly Dictionary<int, List<BlitToScreen>> blitToScreenList = new();
    private readonly Dictionary<string, Guid>            timestampVpMap   = new();
    private readonly GuidOwner<Viewport>                 viewportOwner     = new();

    private List<Viewport> activeViewports        = new();
    private long           drawViewportsPass;
    private int            occlusionRaysPerThread = 512;
    private List<Viewport> sortedActiveViewports  = new();
    private bool           sortedActiveViewportsDirty;
    private int            totalDrawCallsUsed;
    private int            totalObjectsDrawn;
    private int            totalVerticesDrawn;

    public RendererViewport() =>
        this.occlusionRaysPerThread = GLOBAL_GET<int>("rendering/occlusion_culling/occlusion_rays_per_thread");

    private static void WARN_PRINT_ONCE(string message) => throw new NotImplementedException();

    #region private virtual methods
    private void Configure3DRenderBuffers(Viewport viewport)
    {
        if (viewport.RenderBuffers != null)
        {
            if (viewport.Size.X == 0 || viewport.Size.Y == 0)
            {
                viewport.RenderBuffers = null;
            }
            else
            {
                var scaling3dScale = viewport.Scaling3DScale;
                var scaling3dMode  = viewport.Scaling3DMode;
                var scalingEnabled = true;

                if (scaling3dMode == RS.ViewportScaling3DMode.VIEWPORT_SCALING_3D_MODE_FSR && scaling3dScale > 1.0)
                {
                    scaling3dMode = RS.ViewportScaling3DMode.VIEWPORT_SCALING_3D_MODE_BILINEAR;
                }

                if (scaling3dMode == RS.ViewportScaling3DMode.VIEWPORT_SCALING_3D_MODE_FSR && !viewport.FsrEnabled)
                {
                    WARN_PRINT_ONCE("FSR 1.0 3D resolution scaling is not available. Falling back to bilinear 3D resolution scaling.");
                    scaling3dMode = RS.ViewportScaling3DMode.VIEWPORT_SCALING_3D_MODE_BILINEAR;
                }

                if (scaling3dScale == 1.0)
                {
                    scalingEnabled = false;
                }

                int width;
                int height;
                int renderWidth;
                int renderHeight;

                if (scalingEnabled)
                {
                    switch (scaling3dMode)
                    {
                        case RS.ViewportScaling3DMode.VIEWPORT_SCALING_3D_MODE_BILINEAR:
                            width        = (int)Math.Clamp(viewport.Size.X * scaling3dScale, 1, 16384);
                            height       = (int)Math.Clamp(viewport.Size.Y * scaling3dScale, 1, 16384);
                            renderWidth  = width;
                            renderHeight = height;

                            break;
                        case RS.ViewportScaling3DMode.VIEWPORT_SCALING_3D_MODE_FSR:
                            width        = viewport.Size.X;
                            height       = viewport.Size.Y;
                            renderWidth  = (int)Math.Max(width * scaling3dScale, 1.0);
                            renderHeight = (int)Math.Max(height * scaling3dScale, 1.0);

                            break;
                        default:
                            WARN_PRINT_ONCE($"Unknown scaling mode: {scaling3dMode}. Disabling 3D resolution scaling.");

                            width        = viewport.Size.X;
                            height       = viewport.Size.Y;
                            renderWidth  = width;
                            renderHeight = height;

                            break;
                    }
                }
                else
                {
                    width        = viewport.Size.X;
                    height       = viewport.Size.Y;
                    renderWidth  = width;
                    renderHeight = height;
                }

                viewport.InternalSize = new Vector2<int>(renderWidth, renderHeight);

                var textureMipmapBias = Math.Log2(Math.Min(scaling3dScale, 1.0)) + viewport.TextureMipmapBias;

                viewport.RenderBuffers.Configure(
                    viewport.RenderTarget,
                    viewport.InternalSize,
                    new(width, height),
                    viewport.FsrSharpness,
                    textureMipmapBias,
                    viewport.Msaa3D,
                    viewport.ScreenSpaceAA,
                    viewport.UseTAA,
                    viewport.UseDebanding,
                    viewport.ViewCount
                );
            }
        }
    }

    private void DrawViewport(Viewport viewport)
    {
        #region TODO
        // if (viewport->measure_render_time) {
        //     String rt_id = "vp_begin_" + itos(viewport->self.get_id());
        //     RSG::utilities->capture_timestamp(rt_id);
        //     timestamp_vp_map[rt_id] = viewport->self;
        // }
        #endregion TODO

        if (OS.Singleton.CurrentRenderingMethod == "gl_compatibility")
        {
            // This is currently needed for GLES to keep the current window being rendered to up to date
            DisplayServer.Singleton.GlWindowMakeCurrent(viewport.ViewportToScreen);
        }

        /* Camera should always be BEFORE any other 3D */

        var scenarioDrawCanvasBg   = false; //draw canvas, or some layer of it, as BG for 3D instead of in front
        var scenarioCanvasMaxLayer = 0;
        var forceClearRenderTarget = false;

        for (var i = 0; i < RS.VIEWPORT_RENDER_INFO_TYPE_MAX; i++)
        {
            for (var j = 0; j < RS.VIEWPORT_RENDER_INFO_MAX; j++)
            {
                viewport.RenderInfo.Info[i, j] = 0;
            }
        }

        if (RSG.Scene.IsScenario(viewport.Scenario))
        {
            var environment = RSG.Scene.ScenarioGetEnvironment(viewport.Scenario);

            if (RSG.Scene.IsEnvironment(environment))
            {
                if (!viewport.Disable2D && !this.ViewportIsEnvironmentDisabled(viewport))
                {
                    scenarioDrawCanvasBg   = RSG.Scene.EnvironmentGetBackground(environment) == RS.EnvironmentBG.ENV_BG_CANVAS;
                    scenarioCanvasMaxLayer = RSG.Scene.EnvironmentGetCanvasMaxLayer(environment);
                }
                else if (RSG.Scene.EnvironmentGetBackground(environment) == RS.EnvironmentBG.ENV_BG_CANVAS)
                {
                    // The scene renderer will still copy over the last frame, so we need to clear the render target.
                    forceClearRenderTarget = true;
                }
            }
        }

        var canDraw3D = RSG.Scene.IsCamera(viewport.Camera) && !viewport.Disable3D;

        if ((scenarioDrawCanvasBg || canDraw3D) && viewport.RenderBuffers == null)
        {
            viewport.RenderBuffers = RSG.Scene.RenderBuffersCreate();

            this.Configure3DRenderBuffers(viewport);
        }

        var color = viewport.TransparentBg ? default : RSG.TextureStorage.DefaultClearColor;

        if (viewport.ClearMode != RS.ViewportClearMode.VIEWPORT_CLEAR_NEVER)
        {
            RSG.TextureStorage.RenderTargetRequestClear(viewport.RenderTarget, color);

            if (viewport.ClearMode == RS.ViewportClearMode.VIEWPORT_CLEAR_ONLY_NEXT_FRAME)
            {
                viewport.ClearMode = RS.ViewportClearMode.VIEWPORT_CLEAR_NEVER;
            }
        }

        if (!scenarioDrawCanvasBg && canDraw3D)
        {
            if (forceClearRenderTarget)
            {
                RSG.TextureStorage.RenderTargetDoClearRequest(viewport.RenderTarget);
            }

            this.Draw3D(viewport);
        }

        if (!viewport.Disable2D)
        {
            var canvasMap = new Dictionary<Viewport.CanvasKey, Viewport.CanvasData>();

            var clipRect = new Rect2<RealT>(0f, 0f, viewport.Size.X, viewport.Size.Y);

            var lights                      = default(RendererCanvasRender.Light);
            var lightsWithShadow            = default(RendererCanvasRender.Light);
            var directionalLights           = default(RendererCanvasRender.Light);
            var directionalLightsWithShadow = default(RendererCanvasRender.Light);

            if (viewport.SdfActive)
            {
                // Process SDF.

                var sdfRect = RSG.TextureStorage.RenderTargetGetSdfRect(viewport.RenderTarget);

                var occluders = default(RendererCanvasRender.LightOccluderInstance);

                // Make list of occluders.
                foreach (var canvasData in viewport.CanvasMap)
                {
                    var canvas = (RendererCanvasCull.Canvas)canvasData.Value.Canvas!;

                    var xf = this.CanvasGetTransform(viewport, canvas, canvasData.Value, clipRect.Size);

                    foreach (var occluder in canvas.Occluders)
                    {
                        if (!occluder.Enabled)
                        {
                            continue;
                        }

                        occluder.XformCache = xf * occluder.Xform;

                        if (sdfRect.As<RealT>().IntersectsTransformed(occluder.XformCache, occluder.AabbCache))
                        {
                            occluder.Next = occluders;
                            occluders     = occluder;
                        }
                    }
                }

                RSG.CanvasRender.RenderSdf(viewport.RenderTarget, occluders!);
                RSG.TextureStorage.RenderTargetMarkSdfEnabled(viewport.RenderTarget, true);

                viewport.SdfActive = false; // If used, gets set active again.
            }
            else
            {
                RSG.TextureStorage.RenderTargetMarkSdfEnabled(viewport.RenderTarget, false);
            }

            var shadowRect = new Rect2<RealT>();

            #pragma warning disable CS0219
            var shadowCount           = 0;
            #pragma warning restore CS0219
            var directionalLightCount = 0;

            RENDER_TIMESTAMP("Cull 2D Lights");
            foreach (var e in viewport.CanvasMap)
            {
                var canvas = (RendererCanvasCull.Canvas)e.Value.Canvas!;

                var xf = this.CanvasGetTransform(viewport, canvas, e.Value, clipRect.Size);

                // Find lights in canvas.

                foreach (var cl in canvas.Lights)
                {
                    if (cl.Enabled && cl.Texture != default)
                    {
                        //not super efficient..
                        var tsize = RSG.TextureStorage.TextureSizeWithProxy(cl.Texture);
                        tsize *= cl.Scale;

                        var offset = tsize / 2.0f;

                        cl.RectCache = new(-offset + cl.TextureOffset, tsize);
                        cl.XformCache = xf * cl.Xform;

                        if (clipRect.IntersectsTransformed(cl.XformCache, cl.RectCache))
                        {
                            cl.FilterNextPtr = lights;
                            lights = cl;

                            var scale = new Transform2D<RealT>();
                            scale.Scale(cl.RectCache.Size);
                            scale[2] = cl.RectCache.Position;

                            cl.LightShaderXform = xf * cl.Xform * scale;

                            if (cl.UseShadow)
                            {
                                cl.ShadowsNextPtr = lightsWithShadow;

                                shadowRect = lightsWithShadow == null ? cl.XformCache.Xform(cl.RectCache) : shadowRect.Merge(cl.XformCache.Xform(cl.RectCache));

                                lightsWithShadow = cl;
                                cl.RadiusCache = (RealT)cl.RectCache.Size.Length;
                            }
                        }
                    }
                }

                foreach (var cl in canvas.DirectionalLights)
                {
                    if (cl.Enabled)
                    {
                        cl.FilterNextPtr  = directionalLights;
                        directionalLights = cl;
                        var xformCache    = xf * cl.Xform;
                        xformCache[2]     = default;
                        cl.XformCache     = xformCache; //translation is pointless

                        if (cl.UseShadow)
                        {
                            cl.ShadowsNextPtr           = directionalLightsWithShadow;
                            directionalLightsWithShadow = cl;
                        }

                        directionalLightCount++;

                        if (directionalLightCount == RS.MAX_2D_DIRECTIONAL_LIGHTS)
                        {
                            break;
                        }
                    }
                }

                canvasMap.Add(new Viewport.CanvasKey(e.Key, e.Value.Layer, e.Value.Sublayer), e.Value);
            }

            // if (lights_with_shadow) {
            //     //update shadows if any

            //     RendererCanvasRender::LightOccluderInstance *occluders = nullptr;

            //     RENDER_TIMESTAMP("> Render PointLight2D Shadows");
            //     RENDER_TIMESTAMP("Cull LightOccluder2Ds");

            //     //make list of occluders
            //     for (KeyValue<RID, Viewport::CanvasData> &E : viewport->canvas_map) {
            //         RendererCanvasCull::Canvas *canvas = static_cast<RendererCanvasCull::Canvas *>(E.value.canvas);
            //         Transform2D xf = _canvas_get_transform(viewport, canvas, &E.value, clip_rect.size);

            //         for (RendererCanvasRender::LightOccluderInstance *F : canvas->occluders) {
            //             if (!F->enabled) {
            //                 continue;
            //             }
            //             F->xform_cache = xf * F->xform;
            //             if (shadow_rect.intersects_transformed(F->xform_cache, F->aabb_cache)) {
            //                 F->next = occluders;
            //                 occluders = F;
            //             }
            //         }
            //     }
            //     //update the light shadowmaps with them

            //     RendererCanvasRender::Light *light = lights_with_shadow;
            //     while (light) {
            //         RENDER_TIMESTAMP("Render PointLight2D Shadow");

            //         RSG::canvas_render->light_update_shadow(light->light_internal, shadow_count++, light->xform_cache.affine_inverse(), light->item_shadow_mask, light->radius_cache / 1000.0, light->radius_cache * 1.1, occluders);
            //         light = light->shadows_next_ptr;
            //     }

            //     RENDER_TIMESTAMP("< Render PointLight2D Shadows");
            // }

            // if (directional_lights_with_shadow) {
            //     //update shadows if any
            //     RendererCanvasRender::Light *light = directional_lights_with_shadow;
            //     while (light) {
            //         Vector2 light_dir = -light->xform_cache.columns[1].normalized(); // Y is light direction
            //         float cull_distance = light->directional_distance;

            //         Vector2 light_dir_sign;
            //         light_dir_sign.x = (ABS(light_dir.x) < CMP_EPSILON) ? 0.0 : ((light_dir.x > 0.0) ? 1.0 : -1.0);
            //         light_dir_sign.y = (ABS(light_dir.y) < CMP_EPSILON) ? 0.0 : ((light_dir.y > 0.0) ? 1.0 : -1.0);

            //         Vector2 points[6];
            //         int point_count = 0;

            //         for (int j = 0; j < 4; j++) {
            //             static const Vector2 signs[4] = { Vector2(1, 1), Vector2(1, 0), Vector2(0, 0), Vector2(0, 1) };
            //             Vector2 sign_cmp = signs[j] * 2.0 - Vector2(1.0, 1.0);
            //             Vector2 point = clip_rect.position + clip_rect.size * signs[j];

            //             if (sign_cmp == light_dir_sign) {
            //                 //both point in same direction, plot offsetted
            //                 points[point_count++] = point + light_dir * cull_distance;
            //             } else if (sign_cmp.x == light_dir_sign.x || sign_cmp.y == light_dir_sign.y) {
            //                 int next_j = (j + 1) % 4;
            //                 Vector2 next_sign_cmp = signs[next_j] * 2.0 - Vector2(1.0, 1.0);

            //                 //one point in the same direction, plot segment

            //                 if (next_sign_cmp.x == light_dir_sign.x || next_sign_cmp.y == light_dir_sign.y) {
            //                     if (light_dir_sign.x != 0.0 || light_dir_sign.y != 0.0) {
            //                         points[point_count++] = point;
            //                     }
            //                     points[point_count++] = point + light_dir * cull_distance;
            //                 } else {
            //                     points[point_count++] = point + light_dir * cull_distance;
            //                     if (light_dir_sign.x != 0.0 || light_dir_sign.y != 0.0) {
            //                         points[point_count++] = point;
            //                     }
            //                 }
            //             } else {
            //                 //plot normally
            //                 points[point_count++] = point;
            //             }
            //         }

            //         Vector2 xf_points[6];

            //         RendererCanvasRender::LightOccluderInstance *occluders = nullptr;

            //         RENDER_TIMESTAMP("> Render DirectionalLight2D Shadows");

            //         // Make list of occluders.
            //         for (KeyValue<RID, Viewport::CanvasData> &E : viewport->canvas_map) {
            //             RendererCanvasCull::Canvas *canvas = static_cast<RendererCanvasCull::Canvas *>(E.value.canvas);
            //             Transform2D xf = _canvas_get_transform(viewport, canvas, &E.value, clip_rect.size);

            //             for (RendererCanvasRender::LightOccluderInstance *F : canvas->occluders) {
            //                 if (!F->enabled) {
            //                     continue;
            //                 }
            //                 F->xform_cache = xf * F->xform;
            //                 Transform2D localizer = F->xform_cache.affine_inverse();

            //                 for (int j = 0; j < point_count; j++) {
            //                     xf_points[j] = localizer.xform(points[j]);
            //                 }
            //                 if (F->aabb_cache.intersects_filled_polygon(xf_points, point_count)) {
            //                     F->next = occluders;
            //                     occluders = F;
            //                 }
            //             }
            //         }

            //         RSG::canvas_render->light_update_directional_shadow(light->light_internal, shadow_count++, light->xform_cache, light->item_shadow_mask, cull_distance, clip_rect, occluders);

            //         light = light->shadows_next_ptr;
            //     }

            //     RENDER_TIMESTAMP("< Render DirectionalLight2D Shadows");
            // }

            if (scenarioDrawCanvasBg && canvasMap.Count > 0 && canvasMap.First().Key.Layer > scenarioCanvasMaxLayer)
            {
                // There may be an outstanding clear request if a clear was requested, but no 2D elements were drawn.
                // Clear now otherwise we copy over garbage from the render target.
                RSG.TextureStorage.RenderTargetDoClearRequest(viewport.RenderTarget);
                if (!canDraw3D)
                {
                    RSG.Scene.RenderEmptyScene(viewport.RenderBuffers, viewport.Scenario, viewport.ShadowAtlas);
                }
                else
                {
                    this.Draw3D(viewport);
                }
                scenarioDrawCanvasBg = false;
            }

            foreach (var e in canvasMap)
            {
                var canvas = (RendererCanvasCull.Canvas)e.Value.Canvas!;

                var xform = this.CanvasGetTransform(viewport, canvas, e.Value, clipRect.Size);

                var canvasLights            = default(RendererCanvasRender.Light);
                var canvasDirectionalLights = default(RendererCanvasRender.Light);

                var ptr = lights;

                while (ptr != null)
                {
                    if (e.Value.Layer >= ptr.LayerMin && e.Value.Layer <= ptr.LayerMax)
                    {
                        ptr.Next = canvasLights;
                        canvasLights = ptr;
                    }
                    ptr = ptr.FilterNextPtr;
                }

                ptr = directionalLights;

                while (ptr != null)
                {
                    if (e.Value.Layer >= ptr.LayerMin && e.Value.Layer <= ptr.LayerMax)
                    {
                        ptr.Next = canvasDirectionalLights;
                        canvasDirectionalLights = ptr;
                    }
                    ptr = ptr.FilterNextPtr;
                }

                RSG.Canvas.RenderCanvas(
                    viewport.RenderTarget,
                    canvas,
                    xform,
                    canvasLights,
                    canvasDirectionalLights,
                    clipRect,
                    viewport.TextureFilter,
                    viewport.TextureRepeat,
                    viewport.Snap2DTransformsToPixel,
                    viewport.Snap2DVerticesToPixel,
                    viewport.CanvasCullMask
                );

                if (RSG.Canvas.WasSdfUsed)
                {
                    viewport.SdfActive = true;
                }

                if (scenarioDrawCanvasBg && e.Key.Layer >= scenarioCanvasMaxLayer)
                {
                    // There may be an outstanding clear request if a clear was requested, but no 2D elements were drawn.
                    // Clear now otherwise we copy over garbage from the render target.
                    RSG.TextureStorage.RenderTargetDoClearRequest(viewport.RenderTarget);

                    if (!canDraw3D)
                    {
                        RSG.Scene.RenderEmptyScene(viewport.RenderBuffers, viewport.Scenario, viewport.ShadowAtlas);
                    }
                    else
                    {
                        this.Draw3D(viewport);
                    }

                    scenarioDrawCanvasBg = false;
                }
            }

            if (scenarioDrawCanvasBg)
            {
                // There may be an outstanding clear request if a clear was requested, but no 2D elements were drawn.
                // Clear now otherwise we copy over garbage from the render target.
                RSG.TextureStorage.RenderTargetDoClearRequest(viewport.RenderTarget);
                if (!canDraw3D)
                {
                    RSG.Scene.RenderEmptyScene(viewport.RenderBuffers, viewport.Scenario, viewport.ShadowAtlas);
                }
                else
                {
                    this.Draw3D(viewport);
                }
            }
        }

        if (RSG.TextureStorage.RenderTargetIsClearRequested(viewport.RenderTarget))
        {
            //was never cleared in the end, force clear it
            RSG.TextureStorage.RenderTargetDoClearRequest(viewport.RenderTarget);
        }

        #region TODO
        // if (viewport->measure_render_time) {
        //     String rt_id = "vp_end_" + itos(viewport->self.get_id());
        //     RSG::utilities->capture_timestamp(rt_id);
        //     timestamp_vp_map[rt_id] = viewport->self;
        // }
        #endregion TODO
    }

    private Transform2D<RealT> CanvasGetTransform(Viewport viewport, RendererCanvasCull.Canvas canvas, Viewport.CanvasData canvasData, Vector2<float> vpSize)
    {
        var xf = viewport.GlobalTransform;

        var scale = 1.0f;
        if (viewport.CanvasMap.TryGetValue(canvas.Parent, out var value))
        {
            var transform = value.Transform;
            if (viewport.Snap2DTransformsToPixel)
            {
                transform[2] = transform[2].Floor();
            }
            xf *= transform;
            scale = canvas.ParentScale;
        }

        var cXform = canvasData.Transform;

        if (viewport.Snap2DTransformsToPixel)
        {
            cXform[2] = cXform[2].Floor();
        }

        xf *= cXform;

        if (scale != 1.0 && !RSG.Canvas.DisableScale)
        {
            var pivot = vpSize * 0.5f;
            var xfpivot = new Transform2D<RealT>
            {
                Origin = pivot
            };
            var xfscale = new Transform2D<RealT>();

            xfscale.Scale(new(scale, scale));

            xf = xfpivot.AffineInverse() * xf;
            xf = xfscale * xf;
            xf = xfpivot * xf;
        }

        return xf;
    }

    private bool ViewportIsEnvironmentDisabled(Viewport viewport) => throw new NotImplementedException();

    private void ViewportSetSize(Viewport viewport, int width, int height, uint viewCount)
    {
        var newSize = new Vector2<int>(width, height);

        if (viewport.Size != newSize || viewport.ViewCount != viewCount)
        {
            viewport.Size      = newSize;
            viewport.ViewCount = viewCount;

            RSG.TextureStorage.RenderTargetSetSize(viewport.RenderTarget, width, height, viewCount);

            this.Configure3DRenderBuffers(viewport);

            viewport.OcclusionBufferDirty = true;
        }
    }

    private void Draw3D(Viewport viewport)
    {
        // TODO - ~\servers\rendering\renderer_viewport.cpp[190:193]

        if (viewport.UseOcclusionCulling && viewport.OcclusionBufferDirty)
        {
            var aspect  = viewport.Size.X / viewport.Size.Y;
            var maxSize = this.occlusionRaysPerThread * ThreadPool.ThreadCount;

            var viewportSize = viewport.Size.X * viewport.Size.Y;
            maxSize = Math.Clamp(maxSize, viewportSize / (32 * 32), viewportSize / (2 * 2));

            var height = Math.Sqrt(maxSize / aspect);
            var newSize = new Vector2<int>((int)height * aspect, (int)height);

            RendererSceneOcclusionCull.Singleton.BufferSetSize(viewport.Self, newSize);

            viewport.OcclusionBufferDirty = false;
        }

        var screenMeshLodThreshold = viewport.MeshLodThreshold / viewport.Size.X;
        RSG.Scene.RenderCamera(
            viewport.RenderBuffers!,
            viewport.Camera,
            viewport.Scenario,
            viewport.Self,
            viewport.InternalSize.As<RealT>(),
            viewport.UseTAA,
            screenMeshLodThreshold,
            viewport.ShadowAtlas,
            null, //TODO - xr_interface,
            viewport.RenderInfo!
        );
    }

    private List<Viewport> SortActiveViewports()
    {
        // We need to sort the viewports in a "topological order", children first and
        // parents last. We also need to keep sibling viewports in the original order
        // from top to bottom.

        var result = new List<Viewport>();
        var nodes  = new Queue<Viewport>();

        for (var i = this.activeViewports.Count - 1; i >= 0; --i)
        {
            var viewport = this.activeViewports[i];
            if (viewport.Parent != default)
            {
                continue;
            }

            nodes.Enqueue(viewport);
            result.Insert(0, viewport);
        }

        while (nodes.Count > 0)
        {
            var node = nodes.Dequeue();

            for (var i = this.activeViewports.Count - 1; i >= 0; --i)
            {
                var child = this.activeViewports[i];

                if (child.Parent != node.Self)
                {
                    continue;
                }

                if (!nodes.Contains(child))
                {
                    nodes.Enqueue(child);
                    result.Insert(0, child);
                }
            }
        }

        return result;
    }
    #endregion private virtual methods

    #region public methods
    public void DrawViewports()
    {
        this.timestampVpMap.Clear();

        // get our xr interface in case we need it
        XRInterface? xrInterface = null;
        if (XRServer.IsInitialized)
        {
            var xrServer = XRServer.Singleton;
            // let our XR server know we're about to render our frames so we can get our frame timing
            xrServer.PreRender();

            // retrieve the interface responsible for rendering
            xrInterface = xrServer.PrimaryInterface;
        }

        if (Engine.Singleton.IsEditorHint)
        {
            this.SetDefaultClearColor(GLOBAL_GET<Color>("rendering/environment/defaults/default_clear_color"));
        }

        if (this.sortedActiveViewportsDirty)
        {
            this.sortedActiveViewports      = this.SortActiveViewports();
            this.sortedActiveViewportsDirty = false;
        }

        var blitToScreenList = new Dictionary<int, List<BlitToScreen>>();
        //draw viewports
        RENDER_TIMESTAMP("> Render Viewports");

        //determine what is visible
        this.drawViewportsPass++;

        for (var i = this.sortedActiveViewports.Count - 1; i >= 0; i--)
        { //to compute parent dependency, must go in reverse draw order

            var vp = this.sortedActiveViewports[i];

            if (vp.UpdateMode == RS.ViewportUpdateMode.VIEWPORT_UPDATE_DISABLED)
            {
                continue;
            }

            if (vp.RenderTarget == default)
            {
                continue;
            }
            //ERR_CONTINUE(!vp->render_target.is_valid());

            var visible = vp.ViewportToScreenRect != default;

            if (vp.UseXr)
            {
                if (xrInterface != null)
                {
                    // Ignore update mode we have to commit frames to our XR interface
                    visible = true;

                    // Override our size, make sure it matches our required size and is created as a stereo target
                    var xrSize = xrInterface.RenderTargetSize;
                    this.ViewportSetSize(vp, (int)xrSize.X, (int)xrSize.Y, xrInterface.ViewCount);
                }
                else
                {
                    // don't render anything
                    visible = false;
                    vp.Size = new();
                }
            }
            else
            {
                if (vp.UpdateMode == RS.ViewportUpdateMode.VIEWPORT_UPDATE_ALWAYS || vp.UpdateMode == RS.ViewportUpdateMode.VIEWPORT_UPDATE_ONCE)
                {
                    visible = true;
                }

                if (vp.UpdateMode == RS.ViewportUpdateMode.VIEWPORT_UPDATE_WHEN_VISIBLE && RSG.TextureStorage.RenderTargetWasUsed(vp.RenderTarget))
                {
                    visible = true;
                }

                if (vp.UpdateMode == RS.ViewportUpdateMode.VIEWPORT_UPDATE_WHEN_PARENT_VISIBLE)
                {
                    var parent = this.viewportOwner.GetOrNull(vp.Parent);
                    if (parent != null && parent.LastPass == this.drawViewportsPass)
                    {
                        visible = true;
                    }
                }
            }

            visible = visible && vp.Size.X > 1 && vp.Size.Y > 1;

            if (visible)
            {
                vp.LastPass = this.drawViewportsPass;
            }
        }

        var verticesDrawn = 0;
        var objectsDrawn  = 0;
        var drawCallsUsed = 0;

        for (var i = 0; i < this.sortedActiveViewports.Count; i++)
        {
            var vp = this.sortedActiveViewports[i];

            if (vp.LastPass != this.drawViewportsPass)
            {
                continue; //should not draw
            }

            RENDER_TIMESTAMP("> Render Viewport " + i);

            RSG.TextureStorage.RenderTargetSetAsUnused(vp.RenderTarget);
            if (vp.UseXr && xrInterface != null)
            {
                // Inform XR interface we're about to render its viewport,
                // if this returns false we don't render.
                // This usually is a result of the player taking off their headset and OpenXR telling us to skip
                // rendering frames.
                if (xrInterface.PreDrawViewport(vp.RenderTarget))
                {
                    RSG.TextureStorage.RenderTargetSetOverride(
                        vp.RenderTarget,
                        xrInterface.ColorTexture,
                        xrInterface.DepthTexture,
                        xrInterface.VelocityTexture
                    );

                    // render...
                    RSG.Scene.DebugDrawMode = vp.DebugDraw;

                    // and draw viewport
                    this.DrawViewport(vp);

                    // commit our eyes
                    var blits = xrInterface.PostDrawViewport(vp.RenderTarget, vp.ViewportToScreenRect);
                    if (vp.ViewportToScreen != DisplayServer.INVALID_WINDOW_ID)
                    {
                        if (OS.Singleton.CurrentRenderingDriverName == "opengl3")
                        {
                            if (blits.Count > 0)
                            {
                                RSG.Rasterizer.BlitRenderTargetsToScreen(vp.ViewportToScreen, blits, blits.Count);
                            }

                            RSG.Rasterizer.EndFrame(true);
                        }
                        else if (blits.Count > 0)
                        {
                            if (!blitToScreenList.TryGetValue(vp.ViewportToScreen, out var list))
                            {
                                blitToScreenList[vp.ViewportToScreen] = list = new();
                            }

                            for (var b = 0; b < blits.Count; b++)
                            {
                                list.Add(blits[b]);
                            }
                        }
                    }
                }
            }
            else
            {
                RSG.TextureStorage.RenderTargetSetOverride(vp.RenderTarget, default, default, default);

                RSG.Scene.DebugDrawMode = vp.DebugDraw;

                // render standard mono camera
                this.DrawViewport(vp);

                if (vp.ViewportToScreen != DisplayServer.INVALID_WINDOW_ID && (!vp.ViewportRenderDirectToScreen || !RendererCompositor.IsLowEnd))
                {
                    //copy to screen if set as such
                    var blit = new BlitToScreen
                    {
                        RenderTarget = vp.RenderTarget,
                        DstRect      = vp.ViewportToScreenRect != default ? vp.ViewportToScreenRect.As<int>() : new(new(), vp.Size)
                    };

                    if (!blitToScreenList.TryGetValue(vp.ViewportToScreen, out var list))
                    {
                        blitToScreenList[vp.ViewportToScreen] = list = new();
                    }

                    if (OS.Singleton.CurrentRenderingDriverName == "opengl3")
                    {
                        var blitToScreenVec = new List<BlitToScreen>
                        {
                            blit
                        };

                        RSG.Rasterizer.BlitRenderTargetsToScreen(vp.ViewportToScreen, blitToScreenVec, 1);
                        RSG.Rasterizer.EndFrame(true);
                    }
                    else
                    {
                        list.Add(blit);
                    }
                }
            }

            if (vp.UpdateMode == RS.ViewportUpdateMode.VIEWPORT_UPDATE_ONCE)
            {
                vp.UpdateMode = RS.ViewportUpdateMode.VIEWPORT_UPDATE_DISABLED;
            }

            RENDER_TIMESTAMP("< Render Viewport " + i);

            objectsDrawn  += vp.RenderInfo.Info[(int)RS.ViewportRenderInfoType.VIEWPORT_RENDER_INFO_TYPE_VISIBLE, (int)RS.ViewportRenderInfo.VIEWPORT_RENDER_INFO_OBJECTS_IN_FRAME]    + vp.RenderInfo.Info[(int)RS.ViewportRenderInfoType.VIEWPORT_RENDER_INFO_TYPE_SHADOW, (int)RS.ViewportRenderInfo.VIEWPORT_RENDER_INFO_OBJECTS_IN_FRAME];
            verticesDrawn += vp.RenderInfo.Info[(int)RS.ViewportRenderInfoType.VIEWPORT_RENDER_INFO_TYPE_VISIBLE, (int)RS.ViewportRenderInfo.VIEWPORT_RENDER_INFO_PRIMITIVES_IN_FRAME] + vp.RenderInfo.Info[(int)RS.ViewportRenderInfoType.VIEWPORT_RENDER_INFO_TYPE_SHADOW, (int)RS.ViewportRenderInfo.VIEWPORT_RENDER_INFO_PRIMITIVES_IN_FRAME];
            drawCallsUsed += vp.RenderInfo.Info[(int)RS.ViewportRenderInfoType.VIEWPORT_RENDER_INFO_TYPE_VISIBLE, (int)RS.ViewportRenderInfo.VIEWPORT_RENDER_INFO_DRAW_CALLS_IN_FRAME] + vp.RenderInfo.Info[(int)RS.ViewportRenderInfoType.VIEWPORT_RENDER_INFO_TYPE_SHADOW, (int)RS.ViewportRenderInfo.VIEWPORT_RENDER_INFO_DRAW_CALLS_IN_FRAME];
        }

        RSG.Scene.DebugDrawMode = RS.ViewportDebugDraw.VIEWPORT_DEBUG_DRAW_DISABLED;

        this.totalObjectsDrawn  = objectsDrawn;
        this.totalVerticesDrawn = verticesDrawn;
        this.totalDrawCallsUsed = drawCallsUsed;

        RENDER_TIMESTAMP("< Render Viewports");
        //this needs to be called to make screen swapping more efficient
        RSG.Rasterizer.PrepareForBlittingRenderTargets();

        foreach (var entry in this.blitToScreenList)
        {
            RSG.Rasterizer.BlitRenderTargetsToScreen(entry.Key, entry.Value, entry.Value.Count);
        }
    }

    public void SetDefaultClearColor(in Color color) =>
        RSG.TextureStorage.DefaultClearColor = color;

    public void ViewportAttachCanvas(Guid viewportId, Guid canvasId)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_NULL(viewport) || ERR_FAIL_COND(viewport.CanvasMap.ContainsKey(canvasId)))
        {
            return;
        }

        var canvas = RSG.Canvas.CanvasOwner.GetOrNull(canvasId);

        if (ERR_FAIL_NULL(canvas))
        {
            return;
        }

        canvas.Viewports.Add(viewportId);

        var canvasData = new Viewport.CanvasData
        {
            Canvas   = canvas,
            Layer    = 0,
            Sublayer = 0,
        };

        viewport.CanvasMap[canvasId] = canvasData;
    }

    public void ViewportAttachToScreen(Guid viewportId, Rect2<int> rect, int screen)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND(viewport == null))
        {
            return;
        }

        if (screen != DisplayServer.INVALID_WINDOW_ID)
        {
            // If using OpenGL we can optimize this operation by rendering directly to system_fbo
            // instead of rendering to fbo and copying to system_fbo after
            if (RendererCompositor.IsLowEnd && viewport!.ViewportRenderDirectToScreen)
            {
                RSG.TextureStorage.RenderTargetSetSize(viewport.RenderTarget, rect.Size.X, rect.Size.Y, viewport.ViewCount);
                RSG.TextureStorage.RenderTargetSetPosition(viewport.RenderTarget, rect.Position.X, rect.Position.Y);
            }

            viewport!.ViewportToScreenRect = rect.As<RealT>();
            viewport.ViewportToScreen      = screen;
        }
        else
        {
            // if render_direct_to_screen was used, reset size and position
            if (RendererCompositor.IsLowEnd && viewport!.ViewportRenderDirectToScreen)
            {
                RSG.TextureStorage.RenderTargetSetPosition(viewport.RenderTarget, 0, 0);
                RSG.TextureStorage.RenderTargetSetSize(viewport.RenderTarget, viewport.Size.X, viewport.Size.Y, viewport.ViewCount);
            }

            viewport!.ViewportToScreenRect = default;
            viewport.ViewportToScreen     = DisplayServer.INVALID_WINDOW_ID;
        }
    }

    public Guid ViewportGetTexture(Guid viewportId)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        return ERR_FAIL_COND_V(viewport == null)
            ? default
            : RSG.TextureStorage.RenderTargetGetTexture(viewport!.RenderTarget);
    }

    public void ViewportInitialize(Guid id)
    {
        var viewport = this.viewportOwner.Initialize(id);

        viewport.Self                         = id;
        viewport.RenderTarget                 = RSG.TextureStorage.RenderTargetCreate();
        viewport.ShadowAtlas                  = RSG.LightStorage.ShadowAtlasCreate();
        viewport.ViewportRenderDirectToScreen = false;
        viewport.FsrEnabled                   = !RendererCompositor.IsLowEnd && !viewport.Disable3D;
    }

    public void ViewportSetActive(Guid viewportId, bool active)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND_V(viewport == null))
        {
            return;
        }

        if (active)
        {
            if (ERR_FAIL_COND_MSG(this.activeViewports.Contains(viewport!), "Can't make active a Viewport that is already active."))
            {
                return;
            }

            viewport!.OcclusionBufferDirty = true;
            this.activeViewports.Add(viewport);
        }
        else
        {
            this.activeViewports.Remove(viewport!);
        }

        this.sortedActiveViewportsDirty = true;
    }

    public void ViewportSetCanvasCullMask(Guid viewportId, uint canvasCullMask)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND_V(viewport == null))
        {
            return;
        }

        viewport!.CanvasCullMask = canvasCullMask;
    }

    public void ViewportSetCanvasTransform(Guid viewportId, Guid canvarId, Transform2D<RealT> offset)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND_V(viewport == null) || ERR_FAIL_COND(!viewport!.CanvasMap.ContainsKey(canvarId)))
        {
            return;
        }

        viewport!.CanvasMap[canvarId].Transform = offset;
    }

    public void ViewportSetFsrSharpness(Guid viewportId, float fsrSharpness)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND_V(viewport == null))
        {
            return;
        }

        viewport!.FsrSharpness = fsrSharpness;

        if (viewport.RenderBuffers != null)
        {
            viewport.RenderBuffers.FsrSharpness = fsrSharpness;
        }
    }

    public void ViewportSetGlobalCanvasTransform(Guid viewportId, Transform2D<float> transform)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND(viewport == null))
        {
            return;
        }

        viewport!.GlobalTransform = transform;
    }

    public void ViewportSetMeshLodThreshold(Guid viewportId, float meshLodThreshold)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND_V(viewport == null))
        {
            return;
        }

        viewport!.MeshLodThreshold = meshLodThreshold;
    }

    public void ViewportSetPositionalShadowAtlasQuadrantSubdivision(Guid viewportId, int quadrant, int subdiv)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND_V(viewport == null))
        {
            return;
        }

        RSG.LightStorage.ShadowAtlasSetQuadrantSubdivision(viewport!.ShadowAtlas, quadrant, subdiv);
    }

    public void ViewportSetPositionalShadowAtlasSize(Guid viewportId, int size, bool positionalShadowAtlas16Bits)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND_V(viewport == null))
        {
            return;
        }

        viewport!.ShadowAtlasSize  = size;
        viewport.ShadowAtlas16Bits = positionalShadowAtlas16Bits;

        RSG.LightStorage.ShadowAtlasSetSize(viewport.ShadowAtlas, viewport.ShadowAtlasSize, viewport.ShadowAtlas16Bits);
    }

    public void ViewportSetMsaa2D(Guid viewportId, RS.ViewportMSAA msaa)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND_V(viewport == null))
        {
            return;
        }

        if (viewport!.Msaa2D == msaa)
        {
            return;
        }

        viewport.Msaa2D = msaa;
        RSG.TextureStorage.RenderTargetSetMsaa(viewport.RenderTarget, msaa);
    }

    public void ViewportSetMsaa3D(Guid viewportId, RS.ViewportMSAA msaa)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND_V(viewport == null))
        {
            return;
        }

        if (viewport!.Msaa3D == msaa)
        {
            return;
        }

        viewport.Msaa3D = msaa;
        this.Configure3DRenderBuffers(viewport);
    }

    public void ViewportSetParentViewport(Guid viewportId, Guid parentViewportId)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND_V(viewport == null))
        {
            return;
        }

        viewport!.Parent = parentViewportId;
    }

    public void ViewportSetScaling3DMode(Guid viewportId, RS.ViewportScaling3DMode scaling3DMode)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND_V(viewport == null))
        {
            return;
        }

        viewport!.Scaling3DMode = scaling3DMode;

        this.Configure3DRenderBuffers(viewport);
    }

    public void ViewportSetScaling3DScale(Guid viewportId, float scaling3DScale)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND_V(viewport == null))
        {
            return;
        }

        // Clamp to reasonable values that are actually useful.
        // Values above 2.0 don't serve a practical purpose since the viewport
        // isn't displayed with mipmaps.
        if (viewport!.Scaling3DScale == Math.Clamp(scaling3DScale, 0.1f, 2.0f))
        {
            return;
        }

        viewport.Scaling3DScale = Math.Clamp(scaling3DScale, 0.1f, 2.0f);

        this.Configure3DRenderBuffers(viewport);
    }

    public void ViewportSetScenario(Guid viewportId, Guid scenarioId)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND_V(viewport == null))
        {
            return;
        }

        if (viewport!.Scenario != default)
        {
            RSG.Scene.ScenarioRemoveViewportVisibilityMask(viewport.Scenario, viewportId);
        }

        viewport.Scenario = scenarioId;

        if (viewport.UseOcclusionCulling)
        {
            RendererSceneOcclusionCull.Singleton.BufferSetScenario(viewportId, scenarioId);
        }
    }

    public void ViewportSetSdfOversizeAndScale(Guid viewportId, RS.ViewportSDFOversize sdfOversize, RS.ViewportSDFScale sdfScale)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND_V(viewport == null))
        {
            return;
        }

        RSG.TextureStorage.RenderTargetSetSdfSizeAndScale(viewport!.RenderTarget, sdfOversize, sdfScale);
    }

    public void ViewportSetSize(Guid viewportId, int width, int height)
    {
        if (ERR_FAIL_COND(width < 0 || height < 0))
        {
            return;
        }

        var viewport = this.viewportOwner.GetOrNull(viewportId);
        if (ERR_FAIL_COND(viewport == null) || ERR_FAIL_COND_MSG(viewport!.UseXr, "Cannot set viewport size when using XR"))
        {
            return;
        }

        this.ViewportSetSize(viewport, width, height, 1);
    }

    public void ViewportSetSnap2DTransformsToPixel(Guid viewportId, bool snap2DTransformsToPixel)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND_V(viewport == null))
        {
            return;
        }

        viewport!.Snap2DTransformsToPixel = snap2DTransformsToPixel;
    }

    public void ViewportSetSnap2DVerticesToPixel(Guid viewportId, bool snap2DVerticesToPixel)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND_V(viewport == null))
        {
            return;
        }

        viewport!.Snap2DVerticesToPixel = snap2DVerticesToPixel;
    }

    public void ViewportSetTextureMipmapBias(Guid viewportId, float textureMipmapBias)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND_V(viewport == null))
        {
            return;
        }

        viewport!.TextureMipmapBias = textureMipmapBias;

        if (viewport.RenderBuffers != null)
        {
            viewport.RenderBuffers.TextureMipmapBias = textureMipmapBias;
        }
    }

    public void ViewportSetTransparentBackground(Guid viewportId, bool transparentBackground)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND(viewport == null))
        {
            return;
        }

        RSG.TextureStorage.RenderTargetSetTransparent(viewport!.RenderTarget, transparentBackground);
        viewport.TransparentBg = transparentBackground;
    }

    public void ViewportSetVrsMode(Guid viewportId, RS.ViewportVRSMode vrsMode)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND(viewport == null))
        {
            return;
        }

        RSG.TextureStorage.RenderTargetSetVrsMode(viewport!.RenderTarget, vrsMode);
        this.Configure3DRenderBuffers(viewport);
    }

    public void ViewportSetUpdateMode(Guid viewportId, RS.ViewportUpdateMode updateMode)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND_V(viewport == null))
        {
            return;
        }

        viewport!.UpdateMode = updateMode;
    }

    public void ViewportSetUseDebanding(Guid viewportId, bool useDebanding)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND(viewport == null))
        {
            return;
        }

        if (viewport!.UseDebanding == useDebanding)
        {
            return;
        }

        viewport.UseDebanding = useDebanding;
        if (viewport.RenderBuffers != null)
        {
            viewport.RenderBuffers.UseDebanding = useDebanding;
        }
    }

    public void ViewportSetUseOcclusionCulling(Guid viewportId, bool useOcclusionCulling)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND(viewport == null))
        {
            return;
        }

        if (viewport!.UseOcclusionCulling == useOcclusionCulling)
        {
            return;
        }

        viewport.UseOcclusionCulling = useOcclusionCulling;

        if (viewport.UseOcclusionCulling)
        {
            RendererSceneOcclusionCull.Singleton.AddBuffer(viewportId);
            RendererSceneOcclusionCull.Singleton.BufferSetScenario(viewportId, viewport.Scenario);
        }
        else
        {
            RendererSceneOcclusionCull.Singleton.RemoveBuffer(viewportId);
        }

        viewport.OcclusionBufferDirty = true;
    }

    public void ViewportSetUseTAA(Guid viewportId, bool useTAA)
    {
        var viewport = this.viewportOwner.GetOrNull(viewportId);

        if (ERR_FAIL_COND(viewport == null))
        {
            return;
        }

        if (ERR_FAIL_COND_EDMSG(OS.Singleton.CurrentRenderingMethod != "forward_plus", "TAA is only available when using the Forward+ renderer."))
        {
            return;
        }

        if (viewport!.UseTAA == useTAA)
        {
            return;
        }

        viewport.UseTAA = useTAA;

        this.Configure3DRenderBuffers(viewport);
    }
    #endregion public methods
}
