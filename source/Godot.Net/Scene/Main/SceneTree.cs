#define TOOLS_ENABLED

namespace Godot.Net.Scene.Main;

using System;
using System.Linq;
using Godot.Net.Core;
using Godot.Net.Core.Config;
using Godot.Net.Core.Error;
using Godot.Net.Core.IO;
using Godot.Net.Core.Math;
using Godot.Net.Core.OS;
using Godot.Net.Extensions;
using Godot.Net.Scene.Resources;

using Environment = Resources.Environment;

#pragma warning disable CS0414, CS0649 // TODO Remove;

public partial class SceneTree : MainLoop
{
    public event Action<Node>? NodeAdded;
    public event Action<Node>? NodeConfigurationWarningChanged;
    public event Action<Node>? NodeRemoved;
    public event Action<Node>? NodeRenamed;
    public event Action?       ProcessFrame;
    public event Action?       TreeChanged;
    public event Action?       TreeProcessModeChanged;

    private static readonly string[] allExt = new[] { "*." };

    public static SceneTree Singleton { get; private set; } = null!;

    private readonly HashSet<Node>                callSkip         = new();
    private readonly Dictionary<string, Group>    groupMap         = new();
    private readonly Dictionary<UGCall, object[]> uniqueGroupCalls = new();

    #region private fields
    private int    callLock;
    private int    collisionDebugContacts;
    private Color  debugCollisionContactColor;
    private Color  debugCollisionsColor;
    private Color  debugPathsColor;
    private double debugPathsWidth;
    private Node?  editedSceneRoot;
    private bool   initialized;
    private bool   quit;
    private int    rootLock;
    private bool   ugcLocked;
    #endregion private fields

    #region public readonly properties
    public Node?               CurrentScene              { get; }
    public bool                IsDebuggingCollisionsHint { get; }
    public double              ProcessTime               { get; private set; }
    public SelfList<Node>.List XformChangeList           { get; } = new();
    #endregion public readonly properties

    #region public properties
    public bool   AutoAcceptQuit  { get; set; }
    public Node?  EditedSceneRoot { get; set; }
    public bool   QuitOnGoBack    { get; set; }
    public Window Root            { get; set; } = null!;

    #endregion public properties

    public SceneTree()
    {
        Singleton ??= this;

        this.debugCollisionsColor       = GLOBAL_DEF("debug/shapes/collision/shape_color", new Color(0.0f, 0.6f, 0.7f, 0.42f));
        this.debugCollisionContactColor = GLOBAL_DEF("debug/shapes/collision/contact_color", new Color(1.0f, 0.2f, 0.1f, 0.8f));
        this.debugPathsColor            = GLOBAL_DEF("debug/shapes/paths/geometry_color", new Color(0.1f, 1.0f, 0.7f, 0.4f));
        this.debugPathsWidth            = GLOBAL_DEF("debug/shapes/paths/geometry_width", 2.0);
        this.collisionDebugContacts     = GLOBAL_DEF(new PropertyInfo(VariantType.INT, "debug/shapes/collision/max_contacts_displayed", PropertyHint.PROPERTY_HINT_RANGE, "0,20000,1"), 10000);

        GLOBAL_DEF("debug/shapes/collision/draw_2d_outlines", true);

        // Create with mainloop.

        this.Root = new Window
        {
            PostInitialize = true,
            MinSize        = new(64, 64), // Define a very small minimum window size to prevent bugs such as GH-37242.
            ProcessMode    = Node.ProcessModeKind.PROCESS_MODE_PAUSABLE,
            Name           = "root",
            Title          = GLOBAL_GET<string>("application/config/name")
        };

        #if !_3D_DISABLED
        this.Root.World3D ??= new World3D();
        this.Root.AsAudioListener3D = true;
        #endif

        #region TODO
        //     // Initialize network state.
        //     set_multiplayer(MultiplayerAPI::create_default_interface());
        #endregion TODO

        this.Root.AsAudioListener2D = true;
        this.CurrentScene = null;

        this.Root.Msaa2D                  = GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.INT, "rendering/anti_aliasing/quality/msaa_2d", PropertyHint.PROPERTY_HINT_ENUM, "Disabled (Fastest),2× (Average),4× (Slow),8× (Slowest)"), (Viewport.MSAA)0);
        this.Root.Msaa3D                  = GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.INT, "rendering/anti_aliasing/quality/msaa_3d", PropertyHint.PROPERTY_HINT_ENUM, "Disabled (Fastest),2× (Average),4× (Slow),8× (Slowest)"), (Viewport.MSAA)0);
        this.Root.TransparentBackground   = GLOBAL_DEF("rendering/viewport/transparent_background", false);
        this.Root.ScreenSpaceAA           = GLOBAL_DEF_BASIC(new PropertyInfo(VariantType.INT, "rendering/anti_aliasing/quality/screen_space_aa", PropertyHint.PROPERTY_HINT_ENUM, "Disabled (Fastest),FXAA (Fast)"), (Viewport.ScreenSpaceAAType)0);
        this.Root.UseTAA                  = GLOBAL_DEF_BASIC("rendering/anti_aliasing/quality/use_taa", false);
        this.Root.UseDebanding            = GLOBAL_DEF("rendering/anti_aliasing/quality/use_debanding", false);
        this.Root.UseOcclusionCulling     = GLOBAL_DEF("rendering/occlusion_culling/use_occlusion_culling", false);
        this.Root.MeshLodThreshold        = GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "rendering/mesh_lod/lod_change/threshold_pixels", PropertyHint.PROPERTY_HINT_RANGE, "0,1024,0.1"), 1.0f);
        this.Root.Snap2dTransformsToPixel = GLOBAL_DEF("rendering/2d/snap/snap_2d_transforms_to_pixel", false);
        this.Root.Snap2dVerticesToPixel   = GLOBAL_DEF("rendering/2d/snap/snap_2d_vertices_to_pixel", false);

        // We setup VRS for the main viewport here, in the editor this will have little effect.
        this.Root.VrsMode  = GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/vrs/mode", PropertyHint.PROPERTY_HINT_ENUM, "Disabled,Texture,XR"), (Viewport.VRSMode)0);
        var vrsTexturePath = GLOBAL_DEF(new PropertyInfo(VariantType.STRING, "rendering/vrs/texture", PropertyHint.PROPERTY_HINT_FILE, "*.bmp,*.png,*.tga,*.webp"), "").Trim();

        if (this.Root.VrsMode == Viewport.VRSMode.VRS_TEXTURE && !string.IsNullOrEmpty(vrsTexturePath))
        {
            var vrsImage = new Image();
            var loadErr  = ImageLoader.LoadImage(vrsTexturePath, vrsImage);
            if (loadErr != Error.OK)
            {
                ERR_PRINT($"Non-existing or invalid VRS texture at '{vrsTexturePath}'.");
            }
            else
            {
                var vrsTexture = new ImageTexture();
                ImageTexture.CreateFromImage(vrsImage);
                this.Root.VrsTexture = vrsTexture;
            }
        }

        this.Root.PositionalShadowAtlasSize   = GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/lights_and_shadows/positional_shadow/atlas_size", PropertyHint.PROPERTY_HINT_RANGE, "256,16384"), 4096);
        this.Root.PositionalShadowAtlas16Bits = GLOBAL_DEF("rendering/lights_and_shadows/positional_shadow/atlas_16_bits", true);

        GLOBAL_DEF("rendering/lights_and_shadows/positional_shadow/atlas_size.mobile", 2048);

        var atlasQ0 = GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/lights_and_shadows/positional_shadow/atlas_quadrant_0_subdiv", PropertyHint.PROPERTY_HINT_ENUM, "Disabled,1 Shadow,4 Shadows,16 Shadows,64 Shadows,256 Shadows,1024 Shadows"), 2);
        var atlasQ1 = GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/lights_and_shadows/positional_shadow/atlas_quadrant_1_subdiv", PropertyHint.PROPERTY_HINT_ENUM, "Disabled,1 Shadow,4 Shadows,16 Shadows,64 Shadows,256 Shadows,1024 Shadows"), 2);
        var atlasQ2 = GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/lights_and_shadows/positional_shadow/atlas_quadrant_2_subdiv", PropertyHint.PROPERTY_HINT_ENUM, "Disabled,1 Shadow,4 Shadows,16 Shadows,64 Shadows,256 Shadows,1024 Shadows"), 3);
        var atlasQ3 = GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/lights_and_shadows/positional_shadow/atlas_quadrant_3_subdiv", PropertyHint.PROPERTY_HINT_ENUM, "Disabled,1 Shadow,4 Shadows,16 Shadows,64 Shadows,256 Shadows,1024 Shadows"), 4);

        this.Root.SetPositionalShadowAtlasQuadrantSubdiv(0, (Viewport.PositionalShadowAtlasQuadrantSubdiv)atlasQ0);
        this.Root.SetPositionalShadowAtlasQuadrantSubdiv(1, (Viewport.PositionalShadowAtlasQuadrantSubdiv)atlasQ1);
        this.Root.SetPositionalShadowAtlasQuadrantSubdiv(2, (Viewport.PositionalShadowAtlasQuadrantSubdiv)atlasQ2);
        this.Root.SetPositionalShadowAtlasQuadrantSubdiv(3, (Viewport.PositionalShadowAtlasQuadrantSubdiv)atlasQ3);

        this.Root.SdfOversize = GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/2d/sdf/oversize", PropertyHint.PROPERTY_HINT_ENUM, "100%,120%,150%,200%"), (Viewport.SDFOversize)1);
        this.Root.SdfScale    = GLOBAL_DEF(new PropertyInfo(VariantType.INT, "rendering/2d/sdf/scale", PropertyHint.PROPERTY_HINT_ENUM, "100%,50%,25%"), (Viewport.SDFScale)1);

        #if !_3D_DISABLED
        // Load default fallback environment.
        // Get possible extensions.
        var exts = new List<string>();
        ResourceLoader.GetRecognizedExtensionsForType("Environment", exts);
        // Get path.
        var envPath = GLOBAL_DEF(new PropertyInfo(VariantType.STRING, "rendering/environment/defaults/default_environment", PropertyHint.PROPERTY_HINT_FILE, string.Join(',', allExt.Concat(exts))), "");
        // Setup property.
        envPath = envPath.Trim();
        if (!string.IsNullOrEmpty(envPath))
        {
            var env = ResourceLoader.Load(envPath);
            if (env != null)
            {
                this.Root.World3D.FallbackEnvironment = (Environment)env;
            }
            else
            {
                if (Engine.Singleton.IsEditorHint)
                {
                    // File was erased, clear the field.
                    ProjectSettings.Singleton.Set("rendering/environment/defaults/default_environment", "");
                }
                else
                {
                    // File was erased, notify user.
                    ERR_PRINT("Default Environment as specified in Project Settings (Rendering -> Environment -> Default Environment) could not be loaded.");
                }
            }
        }
        #endif

        this.Root.PhysicsObjectPicking = GLOBAL_DEF("physics/common/enable_object_picking", true);

        this.Root.CloseRequested  += this.MainWindowClose;
        this.Root.GoBackRequested += this.MainWindowGoBack;
        this.Root.FocusEntered    += this.MainWindowFocusIn;

        #if TOOLS_ENABLED
        this.editedSceneRoot = null;
        #endif
    }

    #region private methods
    private void CallIdleCallbacks() { /* TODO */ }
    private void FlushDeleteQueue()  { /* TODO */ }
    private void FlushTransformNotifications()
    {
        var n = this.XformChangeList.First;
        while (n != null)
        {
            var node = n.Self!;
            var nx = n.Next;
            this.XformChangeList.Remove(n);
            n = nx;
            node.Notification(NotificationKind.WINDOW_NOTIFICATION_TRANSFORM_CHANGED);
        }
    }

    private void FlushUgc() { /* TODO */ }

    private void MainWindowFocusIn() => throw new NotImplementedException();
    private void MainWindowGoBack() => throw new NotImplementedException();
    private void MainWindowClose() => throw new NotImplementedException();
    private void NotifyGroupPause(string groupKey, NotificationKind notification)
    {
        if (!this.groupMap.TryGetValue(groupKey, out var group))
        {
            return;
        }

        if (group.Nodes.Count == 0)
        {
            return;
        }

        this.UpdateGroupOrder(group, notification is NotificationKind.NOTIFICATION_PROCESS or NotificationKind.NOTIFICATION_INTERNAL_PROCESS or NotificationKind.NOTIFICATION_PHYSICS_PROCESS or NotificationKind.NOTIFICATION_INTERNAL_PHYSICS_PROCESS);

        //copy, so copy on write happens in case something is removed from process while being called
        //performance is not lost because only if something is added/removed the vector is copied.
        var nodesCopy = group.Nodes.ToArray();

        this.callLock++;

        for (var i = 0; i < nodesCopy.Length; i++)
        {
            var n = nodesCopy[i];
            if (this.callLock > 0 && this.callSkip.Contains(n))
            {
                continue;
            }

            if (!n.CanProcess)
            {
                continue;
            }
            if (!n.CanProcessNotification(notification))
            {
                continue;
            }

            n.Notification(notification);
            //ERR_FAIL_COND(gr_node_count != g.nodes.size());
        }

        this.callLock--;
        if (this.callLock == 0)
        {
            this.callSkip.Clear();
        }
    }


    private void ProcessTimers(double delta, bool physicsFrame) { /* TODO */ }
    private void ProcessTweens(double delta, bool physicsFrame) { /* TODO */ }
    private void UpdateGroupOrder(Group g) => throw new NotImplementedException();
    private void UpdateGroupOrder(Group group, bool v) => throw new NotImplementedException();
    #endregion private methods

    #region public methods
    public Group? AddToGroup(string groupKey, Node node)
    {
        if (!this.groupMap.TryGetValue(groupKey, out var group))
        {
            this.groupMap.Add(groupKey, group = new Group());
        }

        if (ERR_FAIL_COND_V_MSG(group.Nodes.Contains(node), $"Already in group: {groupKey}."))
        {
            return group;
        }

        group.Nodes.Add(node);
        group.Changed = true;

        return group;
    }

    public void CallGroupFlags(GroupCallFlags callFlags, string group, string function, params object[] args)
    {
        // <Godot.Net>
        // Skip call_group_flags and call call_group_flagsp directly
        // void SceneTree::call_group_flagsp(uint32_t p_call_flags, const StringName &p_group, const StringName &p_function, const Variant **p_args, int p_argcount)
        // <Godot.Net>

        if (!this.groupMap.TryGetValue(group, out var g))
        {
            return;
        }

        if (g.Nodes.Count == 0)
        {
            return;
        }

        if (callFlags.HasFlag(GroupCallFlags.GROUP_CALL_UNIQUE) && callFlags.HasFlag(GroupCallFlags.GROUP_CALL_DEFERRED))
        {
            if (ERR_FAIL_COND(this.ugcLocked))
            {
                return;
            }

            var ug = new UGCall
            {
                Call  = function,
                Group = group
            };

            if (this.uniqueGroupCalls.ContainsKey(ug))
            {
                return;
            }

            this.uniqueGroupCalls[ug] = args.ToArray();
            return;
        }

        this.UpdateGroupOrder(g);

        var grNodes = g.Nodes.ToArray();

        this.callLock++;

        if (callFlags.HasFlag(GroupCallFlags.GROUP_CALL_REVERSE))
        {
            foreach (var node in grNodes)
            {
                if (this.callLock > 0 && this.callSkip.Contains(node))
                {
                    continue;
                }

                if (!callFlags.HasFlag(GroupCallFlags.GROUP_CALL_REVERSE))
                {
                    node.Call(function, args);
                }
                else
                {
                    MessageQueue.Singleton.PushCallable(() => node.Call(function, args));
                }
            }

        }
        else
        {
            foreach (var node in grNodes)
            {
                if (this.callLock > 0 && this.callSkip.Contains(node))
                {
                    continue;
                }

                if (!callFlags.HasFlag(GroupCallFlags.GROUP_CALL_DEFERRED))
                {
                    node.Call(function, args);
                }
                else
                {
                    MessageQueue.Singleton.PushCallable(() => node.Call(function, args));
                }
            }
        }

        this.callLock--;

        if (this.callLock == 0)
        {
            this.callSkip.Clear();
        }
    }

    #if TOOLS_ENABLED
	public bool IsNodeBeingEdited(Node node) => throw new NotImplementedException();
    #else
    #pragma warning disable CA1822
	public bool IsNodeBeingEdited(Node node) => false;
    #pragma warning restore CA1822
    #endif

    public void NotifyNodeAdded(Node node) => this.NodeAdded?.Invoke(node);
    public void NotifyNodeConfigurationWarningChanged(Node node) => this.NodeConfigurationWarningChanged?.Invoke(node);
    public void NotifyNodeRemoved(Node node) => this.NodeRemoved?.Invoke(node);
    public void NotifyNodeRenamed(Node node) => this.NodeRenamed?.Invoke(node);
    public void NotifyTreeChanged() => this.TreeChanged?.Invoke();
    public void NotifyTreeProcessModeChanged() => this.TreeProcessModeChanged?.Invoke();
    public void RemoveFromGroup(string key, Node node) => throw new NotImplementedException();

    #endregion public methods

    #region public override methods
    public override void Initialize()
    {
        if (ERR_FAIL_COND(this.Root == null))
        {
            return;
        }

        this.initialized = true;
        this.Root!.Tree = this;
        base.Initialize();
    }

    public override bool Process(double time)
    {
        this.rootLock++;

        base.Process(time);

        this.ProcessTime = time;

        #region TODO
        // if (multiplayer_poll) {
        //     multiplayer->poll();
        //     for (KeyValue<NodePath, Ref<MultiplayerAPI>> &E : custom_multiplayers) {
        //         E.value->poll();
        //     }
        // }
        #endregion TODO

        ProcessFrame?.Invoke();

        MessageQueue.Singleton.Flush(); //small little hack

        this.FlushTransformNotifications();

        this.NotifyGroupPause("_process_internal", NotificationKind.NOTIFICATION_INTERNAL_PROCESS);
        this.NotifyGroupPause(nameof(Process),     NotificationKind.NOTIFICATION_PROCESS);

        this.FlushUgc();
        MessageQueue.Singleton.Flush(); //small little hack
        this.FlushTransformNotifications(); //transforms after world update, to avoid unnecessary enter/exit notifications

        this.rootLock--;

        this.FlushDeleteQueue();

        this.ProcessTimers(time, false); //go through timers

        this.ProcessTweens(time, false);

        this.FlushTransformNotifications(); //additional transforms after timers update

        this.CallIdleCallbacks();

        #if TOOLS_ENABLED && !_3D_DISABLED
        if (Engine.Singleton.IsEditorHint)
        {
            //simple hack to reload fallback environment if it changed from editor
            var envPath  = GLOBAL_GET<string>("rendering/environment/defaults/default_environment")?.Trim();
            var cpath    = default(string);
            var fallback = this.Root!.World3D!.FallbackEnvironment;

            if (fallback != null)
            {
                cpath = fallback.Path;
            }
            if (cpath != envPath)
            {
                if (!string.IsNullOrEmpty(envPath))
                {
                    fallback = ResourceLoader.Load(envPath) as Environment;

                    if (fallback == null)
                    {
                        //could not load fallback, set as empty
                        ProjectSettings.Singleton.Set("rendering/environment/defaults/default_environment", "");
                    }
                }

                this.Root.World3D.FallbackEnvironment = fallback;
            }
        }
        #endif

        return this.quit;
    }
    #endregion public override methods

}
