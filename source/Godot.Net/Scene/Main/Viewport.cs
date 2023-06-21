namespace Godot.Net.Scene.Main;

using Godot.Net.Core.Config;
using Godot.Net.Core.Input;
using Godot.Net.Core.Math;
using Godot.Net.Core.Object;
using Godot.Net.Scene.GUI;
using Godot.Net.Scene.Resources;
using Godot.Net.Servers;

#pragma warning disable CS0649 // TODO Remove

public partial class Viewport : Node
{
    public event Action? SizeChanged;

    private static readonly int[] subdivisions = new int[(int)PositionalShadowAtlasQuadrantSubdiv.SHADOW_ATLAS_QUADRANT_SUBDIV_MAX] { 0, 1, 4, 16, 64, 256, 1024 };

    #region private readonly fields
    private readonly HashSet<CanvasLayer>                  canvasLayers                        = new();
    private readonly ViewportTexture                       defaultTexture;
    private readonly GUI                                   gui                                 = new();
    private readonly List<InputEvent>                      physicsPickingEvents                = new();
    private readonly PositionalShadowAtlasQuadrantSubdiv[] positionalShadowAtlasQuadrantSubdiv = new PositionalShadowAtlasQuadrantSubdiv[4];
    private readonly Ref<Guid>                             textureId;
    private readonly HashSet<ViewportTexture>              viewportTextures                    = new();
    #endregion private readonly fields

    #region private fields
    private uint               canvasCullMask            = 0xffffffff;
    private Transform2D<RealT> canvasTransform           = new();
    private Guid               currentCanvas;
    private float              fsrSharpness;
    private string             guiInputGroup;
    private string             inputGroup;
    private float              meshLodThreshold          = 1.0f;
    private World3D?           ownWorld3D;
    private bool               physicsObjectPicking;
    private bool               positionalShadowAtlas16Bits;
    private int                positionalShadowAtlasSize = 2048;
    private Scaling3DModeType  scaling3DMode;
    private float              scaling3DScale;
    private ScreenSpaceAAType  screenSpaceAA;
    private SDFOversize        sdfOversize               = SDFOversize.SDF_OVERSIZE_120_PERCENT;
    private SDFScale           sdfScale                  = SDFScale.SDF_SCALE_50_PERCENT;
    private string             shortcutInputGroup;
    private Vector2<int>       size                      = new(512, 512);
    private Vector2<int>       size2DOverride;
    private bool               sizeAllocated;
    private bool               snap2DTransformsToPixel;
    private bool               snap2DVerticesToPixel;
    private Transform2D<RealT> stretchTransform          = new();
    private float              textureMipmapBias;
    private bool               transparentBg;
    private string             unhandledInputGroup;
    private string             unhandledKeyInputGroup;
    private bool               useDebanding;
    private bool               useOcclusionCulling;
    private bool               useTAA;
    private bool               useXR;
    private VRSMode            vrsMode;
    private ImageTexture?      vrsTexture;
    private World2D            world2D                   = new();
    private World3D?           world3D;
    #endregion private fields

    #region public readonly properties
    public Transform2D<RealT> GlobalCanvasTransform { get; } = new();
    public Guid               ViewportId            { get; }
    public bool               IsEmbeddingSubwindows => this.gui.EmbedSubwindowsHint;
    public Viewport?          ParentViewport        { get; private set; }
    #endregion public readonly properties

    #region public properties
    public bool EmbeddingSubwindows { get; set; }

    public float FsrSharpness
    {
        get => this.fsrSharpness;
        set
        {
            if (this.fsrSharpness != value)
            {
                this.fsrSharpness = Math.Max(0.0f, value);

                RS.Singleton.ViewportSetFsrSharpness(this.ViewportId, this.fsrSharpness);
            }
        }
    }

    public float MeshLodThreshold
    {
        get => this.meshLodThreshold;
        set
        {
            this.meshLodThreshold = value;
            RS.Singleton.ViewportSetMeshLodThreshold(this.ViewportId, value);
        }
    }

    public bool PhysicsObjectPicking
    {
        get => this.physicsObjectPicking;
        set
        {
            this.physicsObjectPicking = value;

            if (this.physicsObjectPicking)
            {
                this.AddToGroup("_picking_viewports");
            }
            else
            {
                this.physicsPickingEvents.Clear();
                if (this.IsInGroup("_picking_viewports"))
                {
                    this.RemoveFromGroup("_picking_viewports");
                }
            }
        }
    }

    public bool PositionalShadowAtlas16Bits
    {
        get => this.positionalShadowAtlas16Bits;
        set
        {
            if (this.positionalShadowAtlas16Bits == value)
            {
                return;
            }

            this.positionalShadowAtlas16Bits = value;

            RS.Singleton.ViewportSetPositionalShadowAtlasSize(this.ViewportId, this.positionalShadowAtlasSize, this.positionalShadowAtlas16Bits);
        }
    }

    public int PositionalShadowAtlasSize
    {
        get => this.positionalShadowAtlasSize;
        set
        {
            this.positionalShadowAtlasSize = value;

            RS.Singleton.ViewportSetPositionalShadowAtlasSize(this.ViewportId, this.positionalShadowAtlasSize, this.positionalShadowAtlas16Bits);
        }
    }

    public Scaling3DModeType Scaling3DMode
    {
        get => this.scaling3DMode;
        set
        {
            if (this.scaling3DMode != value)
            {
                this.scaling3DMode = value;

                RS.Singleton.ViewportSetScaling3DMode(this.ViewportId, (RS.ViewportScaling3DMode)value);
            }
        }
    }

    public float Scaling3DScale
    {
        get => this.scaling3DScale;
        set
        {
            this.scaling3DScale = Math.Clamp(value, 0.1f, 2.0f);

            RS.Singleton.ViewportSetScaling3DScale(this.ViewportId, this.scaling3DScale);
        }
    }

    public ScreenSpaceAAType ScreenSpaceAA
    {
        get => this.screenSpaceAA;
        set
        {
            if (ERR_FAIL_INDEX(value, ScreenSpaceAAType.SCREEN_SPACE_AA_MAX))
            {
                return;
            }

            if (this.screenSpaceAA == value)
            {
                return;
            }
            this.screenSpaceAA = value;

            RS.Singleton.ViewportSetScreenSpaceAA(this.ViewportId, (RS.ViewportScreenSpaceAA)value);
        }
    }

    public SDFOversize SdfOversize
    {
        get => this.sdfOversize;
        set
        {
            if (ERR_FAIL_INDEX((int)value, (int)SDFOversize.SDF_OVERSIZE_MAX))
            {
                return;
            }

            this.sdfOversize = value;
            RS.Singleton.ViewportSetSdfOversizeAndScale(this.ViewportId, (RS.ViewportSDFOversize)this.sdfOversize, (RS.ViewportSDFScale)this.sdfScale);
        }
    }

    public SDFScale SdfScale
    {
        get => this.SdfScale;
        set
        {
            if (ERR_FAIL_INDEX(value, SDFScale.SDF_SCALE_MAX))
            {
                return;
            }

            this.sdfScale = value;
            RS.Singleton.ViewportSetSdfOversizeAndScale(this.ViewportId, (RS.ViewportSDFOversize)this.sdfOversize, (RS.ViewportSDFScale)this.sdfScale);
        }
    }

    public bool Snap2dTransformsToPixel
    {
        get => this.snap2DTransformsToPixel;
        set
        {
            this.snap2DTransformsToPixel = value;
            RS.Singleton.ViewportSetSnap2DTransformsToPixel(this.ViewportId, this.snap2DTransformsToPixel);
        }
    }

    public bool Snap2dVerticesToPixel
    {
        get => this.snap2DVerticesToPixel;
        set
        {
            this.snap2DVerticesToPixel = value;

            RS.Singleton.ViewportSetSnap2DVerticesToPixel(this.ViewportId, this.snap2DVerticesToPixel);
        }
    }

    public bool SnapControlsToPixelsEnabled { get; set; } = true;

    public float TextureMipmapBias
    {
        get => this.textureMipmapBias;
        set
        {
            if (this.textureMipmapBias != value)
            {
                this.textureMipmapBias = value;

                RS.Singleton.ViewportSetTextureMipmapBias(this.ViewportId, value);
            }
        }
    }

    public bool TransparentBackground
    {
        get => this.transparentBg;
        set
        {
            this.transparentBg = value;
            RS.Singleton.ViewportSetTransparentBackground(this.ViewportId, value);
        }
    }

    public bool UseDebanding
    {
        get => this.useDebanding;
        set
        {
            if (this.useDebanding == value)
            {
                return;
            }
            this.useDebanding = value;
            RS.Singleton.ViewportSetUseDebanding(this.ViewportId, value);
        }
    }

    public bool UseOcclusionCulling
    {
        get => this.useOcclusionCulling;
        set
        {
            if (this.useOcclusionCulling == value)
            {
                return;
            }

            this.useOcclusionCulling = value;
            RS.Singleton.ViewportSetUseOcclusionCulling(this.ViewportId, value);

            this.NotifyPropertyListChanged();
        }
    }

    public bool UseTAA
    {
        get => this.useTAA;
        set
        {
            if (this.useTAA == value)
            {
                return;
            }

            this.useTAA = value;
            RS.Singleton.ViewportSetUseTAA(this.ViewportId, value);
        }
    }

    public VRSMode VrsMode
    {
        get => this.vrsMode;
        set
        {
            // Note, set this even if not supported on this hardware, it will only be used if it is but we want to save the value as set by the user.
            this.vrsMode = value;

            switch (value)
            {
                case VRSMode.VRS_TEXTURE:
                    RS.Singleton.ViewportSetVrsMode(this.ViewportId, RS.ViewportVRSMode.VIEWPORT_VRS_TEXTURE);
                    break;
                case VRSMode.VRS_XR:
                    RS.Singleton.ViewportSetVrsMode(this.ViewportId, RS.ViewportVRSMode.VIEWPORT_VRS_XR);
                    break;
                default:
                    RS.Singleton.ViewportSetVrsMode(this.ViewportId, RS.ViewportVRSMode.VIEWPORT_VRS_DISABLED);
                    break;
            }

            this.NotifyPropertyListChanged();
        }
    }

    public ImageTexture? VrsTexture
    {
        get => this.vrsTexture;
        set => throw new NotImplementedException();
    }

    public World2D World2D
    {
        get => this.world2D;
        set => throw new NotImplementedException();
    }

    public World3D? World3D
    {
        get => this.world3D;
        set
        {
            if (this.world3D == value)
            {
                return;
            }

            if (this.IsInsideTree)
            {
                this.PropagateExitWorld3D(this);
            }

            if (this.ownWorld3D != null && this.world3D != null)
            {
                this.world3D.Changed -= this.OwnWorld3DChanged;
            }

            this.world3D = value;

            if (this.ownWorld3D != null)
            {
                if (this.world3D != null)
                {
                    this.ownWorld3D = (World3D)this.world3D.Duplicate();
                    this.world3D.Changed += this.OwnWorld3DChanged;
                }
                else
                {
                    this.ownWorld3D = new World3D();
                }
            }

            if (this.IsInsideTree)
            {
                this.PropagateEnterWorld3D(this);
            }

            if (this.IsInsideTree)
            {
                RS.Singleton.ViewportSetScenario(this.ViewportId, this.FindWorld3D()!.Scenario);
            }

            this.UpdateAudioListener3D();
        }
    }
    #endregion public properties

    #region public virtual properties
    public virtual bool IsSize2DOverrideStretchEnabled => true;
    public virtual int  WindowId                       => DisplayServer.MAIN_WINDOW_ID;

    #endregion public virtual properties

    public Viewport()
    {
        this.ViewportId = RS.Singleton.ViewportCreate();
        this.textureId  = RS.Singleton.ViewportGetTexture(this.ViewportId);

        this.defaultTexture = new(this)
        {
            Proxy = RS.Singleton.TextureProxyCreate(this.textureId)
        };

        this.viewportTextures.Add(this.defaultTexture);

        this.canvasLayers.Add(new() { PostInitialize = true }); // This eases picking code (interpreted as the canvas of the Viewport).

        this.PositionalShadowAtlasSize = this.positionalShadowAtlasSize;

        for (var i = 0; i < 4; i++)
        {
            this.positionalShadowAtlasQuadrantSubdiv[i] = PositionalShadowAtlasQuadrantSubdiv.SHADOW_ATLAS_QUADRANT_SUBDIV_MAX;
        }

        this.SetPositionalShadowAtlasQuadrantSubdiv(0, PositionalShadowAtlasQuadrantSubdiv.SHADOW_ATLAS_QUADRANT_SUBDIV_4);
        this.SetPositionalShadowAtlasQuadrantSubdiv(1, PositionalShadowAtlasQuadrantSubdiv.SHADOW_ATLAS_QUADRANT_SUBDIV_4);
        this.SetPositionalShadowAtlasQuadrantSubdiv(2, PositionalShadowAtlasQuadrantSubdiv.SHADOW_ATLAS_QUADRANT_SUBDIV_16);
        this.SetPositionalShadowAtlasQuadrantSubdiv(3, PositionalShadowAtlasQuadrantSubdiv.SHADOW_ATLAS_QUADRANT_SUBDIV_64);

        this.MeshLodThreshold = this.meshLodThreshold;

        var id = this.GetHashCode().ToString();

        this.inputGroup             = "_vp_input" + id;
        this.guiInputGroup          = "_vp_gui_input" + id;
        this.unhandledInputGroup    = "_vp_unhandled_input" + id;
        this.shortcutInputGroup     = "_vp_shortcut_input" + id;
        this.unhandledKeyInputGroup = "_vp_unhandled_key_input" + id;

        // Window tooltip.
        this.gui.TooltipDelay = GLOBAL_DEF(new PropertyInfo(VariantType.FLOAT, "gui/timers/tooltip_delay_sec", PropertyHint.PROPERTY_HINT_RANGE, "0,5,0.01,or_greater"), 0.5);

        #if !_3D_DISABLED
        this.Scaling3DMode = GLOBAL_GET<Scaling3DModeType>("rendering/scaling_3d/mode");
        this.Scaling3DScale = GLOBAL_GET<float>("rendering/scaling_3d/scale");
        this.FsrSharpness = GLOBAL_GET<float>("rendering/scaling_3d/fsr_sharpness");
        this.TextureMipmapBias = GLOBAL_GET<float>("rendering/textures/default_filters/texture_mipmap_bias");
        #endif // _3D_DISABLED

        this.SdfOversize = this.sdfOversize; // Set to server.
    }

    #region private methods
    private void OwnWorld3DChanged() => throw new NotImplementedException();
    private void PropagateEnterWorld3D(Viewport viewport) => throw new NotImplementedException();
    private void PropagateExitWorld3D(Viewport viewport) => throw new NotImplementedException();
    private void UpdateAudioListener3D() { /* TODO */ }
    #endregion private methods

    #region protected methods
    protected void SetSize(in Vector2<int> size, in Vector2<int> size2DOverride, bool allocated)
    {
        var stretchTransformNew = new Transform2D<RealT>();

        if (this.IsSize2DOverrideStretchEnabled && size2DOverride.X > 0 && size2DOverride.Y > 0)
        {
            var scale = size.As<RealT>() / size2DOverride.As<RealT>();
            stretchTransformNew.Scale(scale);
        }

        var newSize = size.Max(new(2, 2));
        if (this.size == newSize && this.sizeAllocated == allocated && this.stretchTransform == stretchTransformNew && size2DOverride == this.size2DOverride)
        {
            return;
        }

        this.size             = newSize;
        this.sizeAllocated    = allocated;
        this.size2DOverride   = size2DOverride;
        this.stretchTransform = stretchTransformNew;

        #region TODO
        #if !_3D_DISABLED
        if (!this.useXR)
        {
        #endif

            if (allocated)
            {
                RS.Singleton.ViewportSetSize(this.ViewportId, size.X, size.Y);
            }
            else
            {
                RS.Singleton.ViewportSetSize(this.ViewportId, 0, 0);
            }
        #if !_3D_DISABLED
        } // if (!use_xr)
        #endif
        #endregion TODO

        this.UpdateGlobalTransform();
        this.UpdateConfigurationWarnings();

        this.UpdateCanvasItems();

        foreach (var item in this.viewportTextures)
        {
            item.EmitChanged();
        }

        this.SizeChanged?.Invoke();
    }
    #endregion protected methods

    #region public methods
    public World2D FindWorld2D() =>
        this.world2D ?? this.ParentViewport?.FindWorld2D() ?? new();

    public World3D FindWorld3D() =>
        this.ownWorld3D ?? this.world3D ?? this.ParentViewport?.FindWorld3D() ?? new();

    public Window GetBaseWindow() => throw new NotImplementedException();
    public Control GuiAddRootControl(Control control) => throw new NotImplementedException();

    public bool GuiControlHasFocus(Control control) =>
        this.gui.KeyFocus == control;

    public void GuiHideControl(Control control) => throw new NotImplementedException();
    public void GuiRemoveControl(Control control) => throw new NotImplementedException();
    public void GuiRemoveRootControl(Control control) => throw new NotImplementedException();
    public void GuiResetCanvasSortIndex() => this.gui.CanvasSortIndex = 0;
    public void GuiSetRootOrderDirty() => throw new NotImplementedException();

    public Rect2<RealT> GetVisibleRect()
    {
        var r = this.size == default
            ? new Rect2<RealT>(new(), DisplayServer.Singleton.WindowGetSize(0).As<RealT>())
            : new Rect2<RealT>(new(), this.size.As<RealT>());

        if (this.size2DOverride != default)
        {
            r.Size = this.size2DOverride.As<RealT>();
        }

        return r;
    }

    public void SubWindowRemove(Window window) => throw new NotImplementedException();
    public void SubWindowRegister(Window window) => throw new NotImplementedException();
    public void SubWindowUpdate(Window window) => throw new NotImplementedException();

    public void SetPositionalShadowAtlasQuadrantSubdiv(int quadrant, PositionalShadowAtlasQuadrantSubdiv subdiv)
    {
        if (ERR_FAIL_INDEX(quadrant, 4))
        {
            return;
        }

        if (ERR_FAIL_INDEX((int)subdiv, (int)PositionalShadowAtlasQuadrantSubdiv.SHADOW_ATLAS_QUADRANT_SUBDIV_MAX))
        {
            return;
        }

        if (this.positionalShadowAtlasQuadrantSubdiv[quadrant] == subdiv)
        {
            return;
        }

        this.positionalShadowAtlasQuadrantSubdiv[quadrant] = subdiv;


        RS.Singleton.ViewportSetPositionalShadowAtlasQuadrantSubdivision(this.ViewportId, quadrant, subdivisions[(int)subdiv]);
    }

    public void UpdateCanvasItems()
    {
        if (!this.IsInsideTree)
        {
            return;
        }

        this.UpdateCanvasItems(this);
    }

    public void UpdateCanvasItems(Node node)
    {
        if (node != this)
        {
            if (node is Window w && (!w.IsInsideTree || !w.IsEmbedded))
            {
                return;
            }

            if (node is CanvasItem ci)
            {
                ci.QueueRedraw();
            }
        }

        var cc = node.GetChildCount();

        for (var i = 0; i < cc; i++)
        {
            this.UpdateCanvasItems(node.GetChild(i)!);
        }
    }

    public void UpdateGlobalTransform()
    {
        var sxform = this.stretchTransform * this.GlobalCanvasTransform;

        RS.Singleton.ViewportSetGlobalCanvasTransform(this.ViewportId, sxform);
    }
    #endregion public methods

    #region public override methods
    public override void Notification(NotificationKind notification, bool reversed = false)
    {
        if (!reversed)
        {
            base.Notification(notification, reversed);
        }

        switch (notification)
        {
            case NotificationKind.NOTIFICATION_ENTER_TREE:
                if (this.Parent != null)
                {
                    this.ParentViewport = this.Parent.Viewport!;
                    RS.Singleton.ViewportSetParentViewport(this.ViewportId, this.ParentViewport.ViewportId);
                }
                else
                {
                    this.ParentViewport = null;
                }

                this.currentCanvas = this.FindWorld2D().Canvas;
                RS.Singleton.ViewportAttachCanvas(this.ViewportId, this.currentCanvas);
                RS.Singleton.ViewportSetCanvasTransform(this.ViewportId, this.currentCanvas, this.canvasTransform);
                RS.Singleton.ViewportSetCanvasCullMask(this.ViewportId, this.canvasCullMask);
                // TODO _update_audio_listener_2d();
                #if !_3D_DISABLED
                RS.Singleton.ViewportSetScenario(this.ViewportId, this.FindWorld3D().Scenario);
                // TODO _update_audio_listener_3d();
                #endif // _3D_DISABLED

                this.AddToGroup("_viewports");
                #region TODO
                //             if (get_tree()->is_debugging_collisions_hint()) {
                //                 PhysicsServer2D::get_singleton()->space_set_debug_contacts(find_world_2d()->get_space(), get_tree()->get_collision_debug_contact_count());
                //                 contact_2d_debug = RenderingServer::get_singleton()->canvas_item_create();
                //                 RenderingServer::get_singleton()->canvas_item_set_parent(contact_2d_debug, current_canvas);
                // #ifndef _3D_DISABLED
                //                 PhysicsServer3D::get_singleton()->space_set_debug_contacts(find_world_3d()->get_space(), get_tree()->get_collision_debug_contact_count());
                //                 contact_3d_debug_multimesh = RenderingServer::get_singleton()->multimesh_create();
                //                 RenderingServer::get_singleton()->multimesh_allocate_data(contact_3d_debug_multimesh, get_tree()->get_collision_debug_contact_count(), RS::MULTIMESH_TRANSFORM_3D, false);
                //                 RenderingServer::get_singleton()->multimesh_set_visible_instances(contact_3d_debug_multimesh, 0);
                //                 RenderingServer::get_singleton()->multimesh_set_mesh(contact_3d_debug_multimesh, get_tree()->get_debug_contact_mesh()->get_rid());
                //                 contact_3d_debug_instance = RenderingServer::get_singleton()->instance_create();
                //                 RenderingServer::get_singleton()->instance_set_base(contact_3d_debug_instance, contact_3d_debug_multimesh);
                //                 RenderingServer::get_singleton()->instance_set_scenario(contact_3d_debug_instance, find_world_3d()->get_scenario());
                //                 RenderingServer::get_singleton()->instance_geometry_set_flag(contact_3d_debug_instance, RS::INSTANCE_FLAG_DRAW_NEXT_FRAME_IF_VISIBLE, true);
                // #endif // _3D_DISABLED
                //                 set_physics_process_internal(true);
                //             }
                #endregion TODO
                break;

                //         case NOTIFICATION_READY: {
                // #if !_3D_DISABLED
                //             if (audio_listener_3d_set.size() && !audio_listener_3d) {
                //                 AudioListener3D *first = nullptr;
                //                 for (AudioListener3D *E : audio_listener_3d_set) {
                //                     if (first == nullptr || first->is_greater_than(E)) {
                //                         first = E;
                //                     }
                //                 }

                //                 if (first) {
                //                     first->make_current();
                //                 }
                //             }

                //             if (camera_3d_set.size() && !camera_3d) {
                //                 // There are cameras but no current camera, pick first in tree and make it current.
                //                 Camera3D *first = nullptr;
                //                 for (Camera3D *E : camera_3d_set) {
                //                     if (first == nullptr || first->is_greater_than(E)) {
                //                         first = E;
                //                     }
                //                 }

                //                 if (first) {
                //                     first->make_current();
                //                 }
                //             }
                // #endif // _3D_DISABLED
                //         } break;

                //         case NOTIFICATION_EXIT_TREE: {
                //             _gui_cancel_tooltip();

                //             RS.Singleton.ViewportSetScenario(viewport, RID());
                //             RS.Singleton.ViewportRemoveCanvas(viewport, current_canvas);
                //             if (contact_2d_debug.is_valid()) {
                //                 RS.Singleton.Free(contact_2d_debug);
                //                 contact_2d_debug = RID();
                //             }

                //             if (contact3DDebugMultimesh.is_valid()) {
                //                 RS.Singleton.Free(contact3DDebugMultimesh);
                //                 RS.Singleton.Free(contact_3d_debug_instance);
                //                 contact_3d_debug_instance = RID();
                //                 contact3DDebugMultimesh = RID();
                //             }

                //             remove_from_group("_viewports");
                //             set_physics_process_internal(false);

                //             RS.Singleton.ViewportSetActive(viewport, false);
                //             RS.Singleton.ViewportSetParentViewport(viewport, RID());
                //         } break;

                //         case NOTIFICATION_INTERNAL_PHYSICS_PROCESS: {
                //             if (!get_tree()) {
                //                 return;
                //             }

                //             if (get_tree()->is_debugging_collisions_hint() && contact_2d_debug.is_valid()) {
                //                 RS.Singleton.CanvasItemClear(contact_2d_debug);
                //                 RS.Singleton.CanvasItemSetDrawIndex(contact_2d_debug, 0xFFFFF); //very high index

                //                 Vector<Vector2> points = PhysicsServer2D.Singleton.SpaceGetContacts(find_world_2d()->get_space());
                //                 int point_count = PhysicsServer2D.Singleton.SpaceGetContactCount(find_world_2d()->get_space());
                //                 Color ccol = get_tree()->get_debug_collision_contact_color();

                //                 for (int i = 0; i < point_count; i++) {
                //                     RS.Singleton.CanvasItemAddRect(contact_2d_debug, Rect2(points[i] - Vector2(2, 2), Vector2(5, 5)), ccol);
                //                 }
                //             }
                // #if !_3D_DISABLED
                //             if (get_tree()->is_debugging_collisions_hint() && contact3DDebugMultimesh.is_valid()) {
                //                 Vector<Vector3> points = PhysicsServer3D.Singleton.SpaceGetContacts(FindWorld3D()->get_space());
                //                 int point_count = PhysicsServer3D.Singleton.SpaceGetContactCount(FindWorld3D()->get_space());

                //                 RS.Singleton.MultimeshSetVisibleInstances(contact3DDebugMultimesh, point_count);

                //                 for (int i = 0; i < point_count; i++) {
                //                     Transform3D point_transform;
                //                     point_transform.origin = points[i];
                //                     RS.Singleton.MultimeshInstanceSetTransform(contact3DDebugMultimesh, i, point_transform);
                //                 }
                //             }
                // #endif // _3D_DISABLED
                //         } break;

                //         case NOTIFICATION_VP_MOUSE_ENTER: {
                //             gui.mouse_in_viewport = true;
                //         } break;

                //         case NOTIFICATION_VP_MOUSE_EXIT: {
                //             gui.mouse_in_viewport = false;
                //             _drop_physics_mouseover();
                //             _drop_mouse_over();
                //             // When the mouse exits the viewport, we want to end mouse_over, but
                //             // not mouse_focus, because, for example, we want to continue
                //             // dragging a scrollbar even if the mouse has left the viewport.
                //         } break;

                //         case NOTIFICATION_WM_WINDOW_FOCUS_OUT: {
                //             _gui_cancel_tooltip();
                //             _drop_physics_mouseover();
                //             if (gui.mouse_focus && !gui.forced_mouse_focus) {
                //                 _drop_mouse_focus();
                //             }
                //             // When the window focus changes, we want to end mouse_focus, but
                //             // not the mouse_over. Note: The OS will trigger a separate mouse
                //             // exit event if the change in focus results in the mouse exiting
                //             // the window.
                //         } break;

                //         case NOTIFICATION_PREDELETE: {
                //             if (gui_parent) {
                //                 gui_parent->gui.tooltip_popup = nullptr;
                //                 gui_parent->gui.tooltip_label = nullptr;
                //             }
                //         } break;
        }

        if (reversed)
        {
            base.Notification(notification, reversed);
        }
    }
    #endregion public override methods
}
