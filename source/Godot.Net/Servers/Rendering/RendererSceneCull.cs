namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core;
using Godot.Net.Core.Generics;
using Godot.Net.Core.Math;
using Godot.Net.Servers.Rendering.Storage;

#pragma warning disable CS0649, IDE0044, IDE0052, IDE0059, IDE0060 // TODO - Remove

public partial class RendererSceneCull : RenderingMethod
{
    private const int TAA_JITTER_COUNT = 16;

    #region private_readonly_fields
    private readonly GuidOwner<Camera>                             cameraOwner                   = new();
    private readonly RendererSceneOcclusionCull                    dummyOcclusionCulling;
    private readonly PagedArrayPool<InstanceBounds>                instanceAABBPagePool           = new();
    private readonly PagedArrayPool<Instance>                      instanceCullPagePool           = new();
    private readonly PagedArray<Instance>                          instanceCullResult             = new();
    private readonly PagedArrayPool<InstanceData>                  instanceDataPagePool           = new();
    private readonly Dictionary<Guid, Instance>                    instances                      = new();
    private readonly PagedArray<Instance>                          instanceShadowCullResult       = new();
    private readonly Queue<Instance>                               instanceUpdateList             = new();
    private readonly PagedArrayPool<InstanceVisibilityData>        instanceVisibilityDataPagePool = new();
    private readonly GuidOwner<Scenario>                           scenarioOwner                  = new();
    private readonly List<Vector2<RealT>>                          taaJitterArray                 = new();
    private readonly SelfList<InstanceVisibilityNotifierData>.List visibleNotifierList            = new();
    #endregion private_readonly_fields

    #region private_fields
    private uint geometryInstancePairMask;
    private int indexerUpdateIterations;
    private int renderPass;
    private RendererSceneRender? sceneRender;

    #endregion private_fields

    // PASS1(set_debug_draw_mode, RS::ViewportDebugDraw)
    public override RS.ViewportDebugDraw DebugDrawMode { get => this.SceneRender.DebugDrawMode; set => this.SceneRender.DebugDrawMode = value; }

    public RendererSceneRender SceneRender
    {
        get => this.sceneRender!;
        set
        {
            this.geometryInstancePairMask = value.GeometryInstanceGetPairMask();
            this.sceneRender = value;
        }
    }

    public RendererSceneCull()
    {
        this.renderPass = 1;
        Singleton = this;

        this.instanceCullResult.SetPagePool(this.instanceCullPagePool);
        this.instanceShadowCullResult.SetPagePool(this.instanceCullPagePool);

        // for (uint32_t i = 0; i < MAX_UPDATE_SHADOWS; i++)
        // {
        //     render_shadow_data[i].instances.set_page_pool(&geometry_instance_cull_page_pool);
        // }
        // for (uint32_t i = 0; i < SDFGI_MAX_CASCADES * SDFGI_MAX_REGIONS_PER_CASCADE; i++)
        // {
        //     render_sdfgi_data[i].instances.set_page_pool(&geometry_instance_cull_page_pool);
        // }

        // scene_cull_result.init(&rid_cull_page_pool, &geometry_instance_cull_page_pool, &instance_cull_page_pool);
        // scene_cull_result_threads.resize(WorkerThreadPool::get_singleton().get_thread_count());
        // for (InstanceCullResult &thread : scene_cull_result_threads) {
        //     thread.init(&rid_cull_page_pool, &geometry_instance_cull_page_pool, &instance_cull_page_pool);
        // }

        // indexer_update_iterations = GLOBAL_GET("rendering/limits/spatial_indexer/update_iterations_per_frame");
        // thread_cull_threshold = GLOBAL_GET("rendering/limits/spatial_indexer/threaded_cull_minimum_instances");
        // thread_cull_threshold = MAX(thread_cull_threshold, (uint32_t)WorkerThreadPool::get_singleton().get_thread_count()); //make sure there is at least one thread per CPU

        // taa_jitter_array.resize(TAA_JITTER_COUNT);
        // for (int i = 0; i < TAA_JITTER_COUNT; i++) {
        //     taa_jitter_array[i].x = get_halton_value(i, 2);
        //     taa_jitter_array[i].y = get_halton_value(i, 3);
        // }

        this.dummyOcclusionCulling = new RendererSceneOcclusionCull();
    }

    #region private_methods
    private Guid RenderGetEnvironment(Guid cameraId, Guid scenarioId)
    {
        ASSERT_NOT_NULL(this.sceneRender);

        var camera = this.cameraOwner.GetOrNull(cameraId);
        if (camera != null && this.sceneRender.IsEnvironment(camera.Env))
        {
            return camera.Env;
        }

        var scenario = this.scenarioOwner.GetOrNull(scenarioId);

        return scenario == null
            ? default
            : this.sceneRender.IsEnvironment(scenario.Environment)
                ? scenario.Environment
                : this.sceneRender.IsEnvironment(scenario.FallbackEnvironment)
                    ? scenario.FallbackEnvironment
                    : default;
    }

    private void RenderScene(
        RendererSceneRender.CameraData cameraData,
        RenderSceneBuffers             renderBuffers,
        Guid                           environmentId,
        Guid                           attributes,
        uint                           visibleLayers,
        Guid                           scenarioId,
        Guid                           viewportId,
        Guid                           shadowAtlas,
        Guid                           reflectionProbeId,
        int                            reflectionProbePass,
        float                          screenMeshLodThreshold,
        bool                           usingShadows,
        RenderInfo                     renderInfo
    )
    {
        var renderReflectionProbe = this.instances[reflectionProbeId];
        var scenario              = this.scenarioOwner[scenarioId];

        this.renderPass++;

        this.SceneRender.SetScenePass(this.renderPass);
    }
    #endregion private_methods

    #region public_override_methods
    public override RS.EnvironmentBG EnvironmentGetBackground(Guid environmentId) => throw new NotImplementedException();
    public override int EnvironmentGetCanvasMaxLayer(Guid environmentId) => throw new NotImplementedException();
    public override bool IsCamera(Guid cameraId) => this.cameraOwner.Owns(cameraId);
    public override bool IsEnvironment(Guid environmentId) => this.SceneRender.IsEnvironment(environmentId);
    public override bool IsScenario(Guid scenarioId) => this.scenarioOwner.Owns(scenarioId);
    public override RenderSceneBuffers RenderBuffersCreate() => throw new NotImplementedException();

    public override void RenderCamera(
        RenderSceneBuffers renderBuffers,
        Guid cameraId,
        Guid scenarioId,
        Guid viewportId,
        Vector2<RealT> viewPortSize,
        bool useTaa,
        RealT screenMeshLodThreshold,
        Guid shadowAtlas,
        object? xrInterface, // TODO - XRInterface xrInterface,
        RenderInfo renderInfo
    )
    {
        var camera = this.cameraOwner[cameraId];

        var jitter = useTaa
            ? this.taaJitterArray[(int)(RSG.Rasterizer.FrameNumber % TAA_JITTER_COUNT)] / viewPortSize
            : new Vector2<RealT>();

        var cameraData = new RendererSceneRender.CameraData();

        if (xrInterface == null)
        {
            var transform = camera.Transform;
            var projection = new Projection<RealT>();
            var vaspect = camera.VAspect;
            var isOrthogonal = false;
            var aspect = viewPortSize.X / viewPortSize.Y;

            switch (camera.Type)
            {
                case CameraType.ORTHOGONAL:
                    projection.SetOrthogonal(
                        camera.Size,
                        aspect,
                        camera.ZNear,
                        camera.ZFar,
                        camera.VAspect
                    );
                    isOrthogonal = true;
                    break;
                case CameraType.PERSPECTIVE:
                    projection.SetPerspective(
                        camera.Fov,
                        aspect,
                        camera.ZNear,
                        camera.ZFar,
                        camera.VAspect
                    );
                    break;
                case CameraType.FRUSTUM:
                    projection.SetFrustum(
                        camera.Size,
                        aspect,
                        camera.Offset,
                        camera.ZNear,
                        camera.ZFar,
                        camera.VAspect
                    );
                    break;
            }

            cameraData.SetCamera(transform, projection, isOrthogonal, vaspect, jitter, camera.VisibleLayers);
        }
        else
        {
            // ~\servers\rendering\renderer_scene_cull.cpp[2532:2557]
        }

        var environmentId = this.RenderGetEnvironment(cameraId, scenarioId);

        // ~\servers\rendering\renderer_scene_cull.cpp[2562]

        RendererSceneOcclusionCull.Singleton.BufferUpdate(viewportId, cameraData.MainTransform, cameraData.MainProjection, cameraData.IsOrthogonal);

        this.RenderScene(
            cameraData,
            renderBuffers,
            environmentId,
            camera.Attributes,
            camera.VisibleLayers,
            scenarioId,
            viewportId,
            shadowAtlas,
            default,
            -1,
            screenMeshLodThreshold,
            true,
            renderInfo
        );
    }

    public override void RenderEmptyScene(RenderSceneBuffers? renderBuffers, Guid scenarioId, Guid shadowAtlas) => throw new NotImplementedException();

    public override Guid ScenarioGetEnvironment(Guid scenarioId)
    {
        var scenario = this.scenarioOwner.GetOrNull(scenarioId);

        return ERR_FAIL_NULL(scenario) ? default : scenario.Environment;
    }

    // scenario_initialize
    public override void ScenarioInitialize(Guid scenarioId)
    {
        var scenario = this.scenarioOwner.Initialize(scenarioId);

        scenario.Self = scenarioId;

        scenario!.ReflectionProbeShadowAtlas = RSG.LightStorage.ShadowAtlasCreate();
        RSG.LightStorage.ShadowAtlasSetSize(scenario.ReflectionProbeShadowAtlas, 1024); //make enough shadows for close distance, don't bother with rest
        RSG.LightStorage.ShadowAtlasSetQuadrantSubdivision(scenario.ReflectionProbeShadowAtlas, 0, 4);
        RSG.LightStorage.ShadowAtlasSetQuadrantSubdivision(scenario.ReflectionProbeShadowAtlas, 1, 4);
        RSG.LightStorage.ShadowAtlasSetQuadrantSubdivision(scenario.ReflectionProbeShadowAtlas, 2, 4);
        RSG.LightStorage.ShadowAtlasSetQuadrantSubdivision(scenario.ReflectionProbeShadowAtlas, 3, 8);

        scenario.ReflectionAtlas = RSG.LightStorage.ReflectionAtlasCreate();

        scenario.InstanceAABBs.SetPagePool(this.instanceAABBPagePool);
        scenario.InstanceData.SetPagePool(this.instanceDataPagePool);
        scenario.InstanceVisibility.SetPagePool(this.instanceVisibilityDataPagePool);

        RendererSceneOcclusionCull.Singleton.AddScenario(scenarioId);
    }

    public override void ScenarioRemoveViewportVisibilityMask(Guid scenarioId, Guid viewportId) => throw new NotImplementedException();

    public override void Update()
    {
        // TODO - servers\rendering\renderer_scene_cull.cpp[3963:3965]
        foreach (var scenario in this.scenarioOwner)
        {
            scenario.Indexers[(int)Scenario.IndexerType.INDEXER_GEOMETRY].OptimizeIncremental(this.indexerUpdateIterations);
            scenario.Indexers[(int)Scenario.IndexerType.INDEXER_VOLUMES].OptimizeIncremental(this.indexerUpdateIterations);
        }

        this.SceneRender.Update();
        // TODO this.UpdateDirtyInstances();
        // TODO - servers\rendering\renderer_scene_cull.cpp[3971]
    }

    public override void UpdateVisibilityNotifiers()
    {
        var e = this.visibleNotifierList.First;

        while (e != null)
        {
            var n = e.Next;

            var visibilityNotifier = e.Self!;
            if (visibilityNotifier.JustVisible)
            {
                visibilityNotifier.JustVisible = false;

                RSG.Utilities.VisibilityNotifierCall(visibilityNotifier.Base, true, RSG.Threaded);
            }
            else
            {
                if (visibilityNotifier.VisibleInFrame != RSG.Rasterizer.FrameNumber)
                {
                    this.visibleNotifierList.Remove(e);

                    RSG.Utilities.VisibilityNotifierCall(visibilityNotifier.Base, false, RSG.Threaded);
                }
            }

            e = n;
        }
    }
    #endregion public_override_methods
}
