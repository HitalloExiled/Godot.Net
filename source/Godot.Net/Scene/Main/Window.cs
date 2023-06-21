#define DEBUG_ENABLED

namespace Godot.Net.Scene.Main;

using System.Collections.Generic;
using Godot.Net.Core;
using Godot.Net.Core.Config;
using Godot.Net.Core.Input;
using Godot.Net.Core.Math;
using Godot.Net.Core.String;
using Godot.Net.Scene.GUI;
using Godot.Net.Scene.Theme;
using Godot.Net.Servers;

using Theme = Resources.Theme;

#pragma warning disable CS0067, IDE0052, IDE0044, CS0649, CS0414 // TODO - REMOVE

public partial class Window : Viewport
{
    #region events
    public event Action?             CloseRequested;
    public event Action?             FocusEntered;
    public event Action?             GoBackRequested;
    public event Action?             VisibilityChanged;
    public event Action<InputEvent>? WindowInput;
    #endregion events

    private const int DEFAULT_WINDOW_SIZE = 100;

    #region private readonly fields
    private readonly bool[]                                flags = new bool[8];
    private readonly Vector2<int>                          maxSize;
    private readonly PositionalShadowAtlasQuadrantSubdiv[] positionalShadowAtlasQuadrantSubdiv = new PositionalShadowAtlasQuadrantSubdiv[4];
    private readonly ThemeOwner                            themeOwner;
    private readonly HashSet<Window>                       transientChildren = new();
    #endregion private readonly fields

    #region private fields
    private ContentScaleAspect    contentScaleAspect;
    private float                 contentScaleFactor = 1.0f;
    private ContentScaleMode      contentScaleMode;
    private Vector2<int>          contentScaleSize;
    private int                   currentScreen;
    private Viewport?             embedder;
    private bool                  exclusive;
    private WindowInitialPosition initialPosition;
    private bool                  isAudioListener2DEnabled;
    private bool                  isAudioListener3DEnabled;
    private LayoutDirection       layoutDir = LayoutDirection.LAYOUT_DIRECTION_INHERITED;
    private Vector2<int>          maxSizeUsed;
    private Vector2<int>          minSize;
    private Mode                  mode = Mode.MODE_WINDOWED;
    private Vector2<RealT>[]      mpath = Array.Empty<Vector2<RealT>>();
    private MSAA                  msaa2D;
    private MSAA                  msaa3D;
    private Vector2<int>          position;
    private Theme?                theme;
    private string?               themeTypeVariation;
    private string?               title;
    private bool                  transient;
    private Window?               transientParent;
    private bool                  updatingChildControls;
    private bool                  useFontOversampling;
    private int                   windowId   = DisplayServer.INVALID_WINDOW_ID;
    private Vector2<int>          windowSize = new(DEFAULT_WINDOW_SIZE, DEFAULT_WINDOW_SIZE);
    private Transform2D<RealT>    windowTransform = new();
    private bool                  wrapControls;
    #endregion private fields

    #region public readonly properties
    public bool HasFocus   { get; private set; }
    public bool IsEmbedded => !ERR_FAIL_COND_V(!this.IsInsideTree) && this.GetEmbedder() != null;

    public bool            HasThemeOwnerNode   => this.themeOwner?.OwnerNode != null;
    public bool            IsInEditedSceneRoot => Engine.Singleton.IsEditorHint && this.Tree.EditedSceneRoot != null && (this.Tree.EditedSceneRoot.IsAncestorOf(this) || this.Tree.EditedSceneRoot == this);
    public bool            IsVisible           { get; private set; } = true;
    public HashSet<Window> TransientChildren   { get; } = new();

    #endregion public readonly properties

    #region public properties

    public bool AsAudioListener2D
    {
        get => this.isAudioListener2DEnabled;
        set
        {
            if (value == this.isAudioListener2DEnabled)
            {
                return;
            }

            this.isAudioListener2DEnabled = value;
            // _update_audio_listener_2d(); TODO
        }
    }

    public bool AsAudioListener3D
    {
        get => this.isAudioListener3DEnabled;
        set
        {
            if (value == this.isAudioListener3DEnabled)
            {
                return;
            }

            this.isAudioListener3DEnabled = value;
            // _update_audio_listener_3d(); TODO
        }
    }

    public Window? ExclusiveChild      { get; set; }

    public Vector2<int> MinSize
    {
        get => this.minSize;
        set
        {
            var minSizeClamped = ClampLimitSize(value);

            if (this.minSize == minSizeClamped)
            {
                return;
            }

            this.minSize = minSizeClamped;

            this.ValidateLimitSize();
            this.UpdateWindowSize();
        }
    }

    public MSAA Msaa2D
    {
        get => this.msaa2D;
        set
        {
            if (ERR_FAIL_INDEX(value, MSAA.MSAA_MAX))
            {
                return;
            }

            if (this.msaa2D == value)
            {
                return;
            }

            this.msaa2D = value;

            RS.Singleton.ViewportSetMsaa2D(this.ViewportId, (RS.ViewportMSAA)value);
        }
    }

    public MSAA Msaa3D
    {
        get => this.msaa3D;
        set
        {
            if (ERR_FAIL_INDEX(value, MSAA.MSAA_MAX))
            {
                return;
            }

            if (this.msaa3D == value)
            {
                return;
            }

            this.msaa3D = value;

            RS.Singleton.ViewportSetMsaa3D(this.ViewportId, (RS.ViewportMSAA)value);
        }
    }

    public Theme? Theme
    {
        get => this.theme;
        set
        {
            if (this.theme == value)
            {
                return;
            }

            if (this.theme != null)
            {
                this.theme.Changed -= this.ThemeChanged;
            }

            this.theme = value;

            if (this.theme != null)
            {
                ThemeOwner.PropagateThemeChanged(this, this, this.IsInsideTree, true);
                this.theme.Changed += this.ThemeChanged;

                return;
            }

            if (this.Parent is Control parentControl && parentControl.HasThemeOwnerNode)
            {
                ThemeOwner.PropagateThemeChanged(this, parentControl.ThemeOwnerNode, this.IsInsideTree, true);

                return;
            }
            if (this.Parent is Window parentWindow && parentWindow.HasThemeOwnerNode)
            {
                ThemeOwner.PropagateThemeChanged(this, parentWindow.ThemeOwnerNode, this.IsInsideTree, true);

                return;
            }

            ThemeOwner.PropagateThemeChanged(this, null, this.IsInsideTree, true);
        }
    }

    public Node? ThemeOwnerNode
    {
        get => this.themeOwner.OwnerNode;
        set => this.themeOwner.OwnerNode = value;
    }

    public string? Title
    {
        get => this.title;
        set
        {
            this.title = value;

            if (this.embedder != null)
            {
                this.embedder.SubWindowUpdate(this);
            }
            else if (this.windowId != DisplayServer.INVALID_WINDOW_ID)
            {
                var trTitle = value;
                #if DEBUG_ENABLED
                if (this.windowId == DisplayServer.MAIN_WINDOW_ID)
                {
                    // Append a suffix to the window title to denote that the project is running
                    // from a debug build (including the editor). Since this results in lower performance,
                    // this should be clearly presented to the user.
                    trTitle = $"{trTitle} (DEBUG)";
                }
                #endif
                DisplayServer.Singleton.WindowSetTitle(trTitle, this.windowId);
            }
        }
    }

    public bool Transient
    {
        get => this.transient;
        set
        {
            if (this.transient == value)
            {
                return;
            }

            this.transient = value;

            if (!this.IsInsideTree)
            {
                return;
            }

            if (this.transient)
            {
                this.MakeTransient();
            }
            else
            {
                this.ClearTransient();
            }
        }
    }

    public bool Visible
    {
        get => this.IsVisible;
        set
        {
            if (this.IsVisible == value)
            {
                return;
            }

            if (!this.IsInsideTree)
            {
                this.IsVisible = value;
                return;
            }

            if (ERR_FAIL_COND_MSG(this.Parent == null, "Can't change visibility of main window."))
            {
                return;
            }

            this.IsVisible = value;

            // Stop any queued resizing, as the window will be resized right now.
            this.updatingChildControls = false;

            var embedderVp = this.GetEmbedder();

            if (embedderVp == null)
            {
                if (!value && this.windowId != DisplayServer.INVALID_WINDOW_ID)
                {
                    this.ClearWindow();
                }
                if (value && this.windowId == DisplayServer.INVALID_WINDOW_ID)
                {
                    this.MakeWindow();
                }
            }
            else
            {
                if (this.IsVisible)
                {
                    this.embedder = embedderVp;
                    this.embedder.SubWindowRegister(this);
                    RS.Singleton.ViewportSetUpdateMode(this.ViewportId, RS.ViewportUpdateMode.VIEWPORT_UPDATE_WHEN_PARENT_VISIBLE);
                }
                else
                {
                    this.embedder!.SubWindowRemove(this);
                    this.embedder = null;
                    RS.Singleton.ViewportSetUpdateMode(this.ViewportId, RS.ViewportUpdateMode.VIEWPORT_UPDATE_DISABLED);
                }
                this.UpdateWindowSize();
            }

            if (!this.IsVisible)
            {
                this.HasFocus = false;
            }
            this.Notification(NotificationKind.WINDOW_NOTIFICATION_VISIBILITY_CHANGED);
            VisibilityChanged?.Invoke();

            RS.Singleton.ViewportSetActive(this.ViewportId, this.IsVisible);

            //update transient exclusive
            if (this.transientParent != null)
            {
                if (this.exclusive && this.IsVisible)
                {
                    if (!this.IsInEditedSceneRoot)
                    {
                        if (ERR_FAIL_COND_MSG(this.transientParent.ExclusiveChild != null && this.transientParent.ExclusiveChild != this, "Transient parent has another exclusive child."))
                        {
                            return;
                        }

                        this.transientParent.ExclusiveChild = this;
                    }
                }
                else
                {
                    if (this.transientParent.ExclusiveChild == this)
                    {
                        this.transientParent.ExclusiveChild = null;
                    }
                }
            }
        }
    }

    public bool WrapControls
    {
        get => this.wrapControls;
        set
        {
            this.wrapControls = value;

            if (!this.IsInsideTree)
            {
                return;
            }

            if (this.updatingChildControls)
            {
                this.UpdateChildControls();
            }
            else
            {
                this.UpdateWindowSize();
            }
        }
    }
    #endregion public properties

    public Window()
    {
        this.maxSize = RS.Singleton.MaximumViewportSize;

        this.themeOwner = new ThemeOwner();

        RS.Singleton.ViewportSetUpdateMode(this.ViewportId, RS.ViewportUpdateMode.VIEWPORT_UPDATE_DISABLED);
    }

    #region private static methods
    // Size2i Window::_clamp_limit_size(const Size2i &p_limit_size)
    private static Vector2<int> ClampLimitSize(Vector2<int> limitSize)
    {
        // Force window limits to respect size limitations of rendering server.
        var maxWindowSize = RS.Singleton.MaximumViewportSize;

        return maxWindowSize != default ? limitSize.Clamp(default, maxWindowSize) : limitSize.Max(default);
    }
    #endregion private static methods

    #region private methods
    private void ClearTransient() => throw new NotImplementedException();
    private void ClearWindow() => throw new NotImplementedException();

    private void EventCallback(DisplayServer.WindowEvent obj) => throw new NotImplementedException();

    private Vector2<int> GetClampedMinimumSize() => !this.wrapControls ? this.minSize : this.minSize.Max(this.GetContentsMinimumSize().As<int>());

    private Vector2<RealT> GetContentsMinimumSize()
    {
        var max = new Vector2<RealT>();

        for (var i = 0; i < this.GetChildCount(); i++)
        {
            if (this.GetChild(i) is Control c)
            {
                var pos = c.Position;
                var min = c.CombinedMinimumSize;

                max.X = Math.Max(pos.X + min.X, max.X);
                max.Y = Math.Max(pos.Y + min.Y, max.Y);
            }
        }

        return max;
    }

    private void MakeTransient()
    {
        if (this.Parent == null)
        {
            //main window, can't be transient
            return;
        }
        //find transient parent
        var vp = this.Parent.Viewport;
        var window = default(Window);

        while (vp != null)
        {
            window = vp as Window;
            if (window != null)
            {
                break;
            }

            if (vp.Parent == null)
            {
                break;
            }

            vp = this.Parent.Viewport;
        }

        if (window != null)
        {
            this.transientParent = window;
            window.TransientChildren.Add(this);
            if (this.IsInsideTree && this.IsVisible && this.exclusive)
            {
                if (this.transientParent.ExclusiveChild == null)
                {
                    if (!this.IsInEditedSceneRoot)
                    {
                        this.transientParent.ExclusiveChild = this;
                    }
                }
                else if (this.transientParent.ExclusiveChild != this)
                {
                    ERR_PRINT("Making child transient exclusive, but parent has another exclusive child");
                }
            }
        }

        //see if we can make transient
        if (this.transientParent!.windowId != DisplayServer.INVALID_WINDOW_ID && this.windowId != DisplayServer.INVALID_WINDOW_ID)
        {
            DisplayServer.Singleton.WindowSetTransient(this.windowId, this.transientParent.windowId);
        }
    }

    private void MakeWindow()
    {
        if (ERR_FAIL_COND(this.windowId != DisplayServer.INVALID_WINDOW_ID))
        {
            return;
        }

        var f = 0;
        for (var i = 0; i < (int)Flags.FLAG_MAX; i++)
        {
            if (this.flags[i])
            {
                f |= 1 << i;
            }
        }

        var vsyncMode = DisplayServer.Singleton.WindowGetVsyncMode(DisplayServer.MAIN_WINDOW_ID);
        var windowRect = new Rect2<int>();

        if (this.initialPosition == WindowInitialPosition.WINDOW_INITIAL_POSITION_ABSOLUTE)
        {
            windowRect = new Rect2<int>(this.position, this.windowSize);
        }
        else if (this.initialPosition == WindowInitialPosition.WINDOW_INITIAL_POSITION_CENTER_PRIMARY_SCREEN)
        {
            windowRect = new Rect2<int>(DisplayServer.Singleton.ScreenGetPosition(DisplayServer.SCREEN_PRIMARY) + (DisplayServer.Singleton.ScreenGetSize(DisplayServer.SCREEN_PRIMARY) - this.windowSize) / 2, this.windowSize);
        }
        else if (this.initialPosition == WindowInitialPosition.WINDOW_INITIAL_POSITION_CENTER_MAIN_WINDOW_SCREEN)
        {
            windowRect = new Rect2<int>(DisplayServer.Singleton.ScreenGetPosition(DisplayServer.SCREEN_OF_MAIN_WINDOW) + (DisplayServer.Singleton.ScreenGetSize(DisplayServer.SCREEN_OF_MAIN_WINDOW) - this.windowSize) / 2, this.windowSize);
        }
        else if (this.initialPosition == WindowInitialPosition.WINDOW_INITIAL_POSITION_CENTER_OTHER_SCREEN)
        {
            windowRect = new Rect2<int>(DisplayServer.Singleton.ScreenGetPosition(this.currentScreen) + (DisplayServer.Singleton.ScreenGetSize(this.currentScreen) - this.windowSize) / 2, this.windowSize);
        }

        this.windowId = DisplayServer.Singleton.CreateSubWindow((DisplayServer.WindowMode)this.mode, vsyncMode, f, windowRect);

        if (ERR_FAIL_COND(this.windowId == DisplayServer.INVALID_WINDOW_ID))
        {
            return;
        }

        DisplayServer.Singleton.WindowSetMaxSize(new(), this.windowId);
        DisplayServer.Singleton.WindowSetMinSize(new(), this.windowId);
        DisplayServer.Singleton.WindowSetMousePassthrough(this.mpath, this.windowId);
        var trTitle = this.Title;
#if DEBUG_ENABLED
        if (this.windowId == DisplayServer.MAIN_WINDOW_ID)
        {
            // Append a suffix to the window title to denote that the project is running
            // from a debug build (including the editor). Since this results in lower performance,
            // this should be clearly presented to the user.
            trTitle = $"{this.Title} (DEBUG)";
        }
#endif
        DisplayServer.Singleton.WindowSetTitle(trTitle, this.windowId);
        DisplayServer.Singleton.WindowAttachInstanceId(this.InstanceId, this.windowId);

        if (this.IsInEditedSceneRoot)
        {
            DisplayServer.Singleton.WindowSetExclusive(this.windowId, false);
        }
        else
        {
            DisplayServer.Singleton.WindowSetExclusive(this.windowId, this.exclusive);
        }

        this.UpdateWindowSize();

        if (this.transientParent != null && this.transientParent.windowId != DisplayServer.INVALID_WINDOW_ID)
        {
            DisplayServer.Singleton.WindowSetTransient(this.windowId, this.transientParent.windowId);
        }

        foreach (var item in this.transientChildren)
        {
            if (item.windowId != DisplayServer.INVALID_WINDOW_ID)
            {
                DisplayServer.Singleton.WindowSetTransient(item.windowId, this.transientParent!.windowId);
            }
        }

        this.UpdateWindowCallbacks();

        RS.Singleton.ViewportSetUpdateMode(this.ViewportId, RS.ViewportUpdateMode.VIEWPORT_UPDATE_WHEN_VISIBLE);
        DisplayServer.Singleton.ShowWindow(this.windowId);
    }

    private void RectChangedCallback(Rect2<int> obj) => throw new NotImplementedException();

    private void ThemeChanged() => throw new NotImplementedException();

    private void UpdateChildControls()
    {
        if (!this.updatingChildControls)
        {
            return;
        }

        this.UpdateWindowSize();

        this.updatingChildControls = false;
    }

    private void UpdateFromWindow()
    {
        if (ERR_FAIL_COND(this.windowId == DisplayServer.INVALID_WINDOW_ID))
        {
            return;
        }

        this.mode = (Mode)DisplayServer.Singleton.WindowGetMode(this.windowId);

        for (var i = 0; i < (int)Flags.FLAG_MAX; i++)
        {
            this.flags[i] = DisplayServer.Singleton.WindowGetFlag((DisplayServer.WindowFlags)i, this.windowId);
        }
    }

    private void UpdateViewportSize()
    {
        //update the viewport part

        var finalSize = new Vector2<int>();
        var finalSizeOverride = new Vector2<int>();
        var attachToScreenRect = new Rect2<int>(default, this.windowSize);
        var fontOversampling = 1.0f;

        this.windowTransform = new();

        if (this.contentScaleMode == ContentScaleMode.CONTENT_SCALE_MODE_DISABLED || this.contentScaleSize.X == 0 || this.contentScaleSize.Y == 0)
        {
            fontOversampling = this.contentScaleFactor;
            finalSize = this.windowSize;
            finalSizeOverride = (this.windowSize.As<RealT>() / this.contentScaleFactor).As<int>();
        }
        else
        {
            //actual screen video mode
            var videoMode = this.windowSize.As<RealT>();
            var desiredRes = this.contentScaleSize.As<RealT>();

            var viewportSize = new Vector2<RealT>();
            var screenSize = new Vector2<RealT>();

            var viewportAspect = desiredRes.Aspect();
            var videoModeAspect = videoMode.Aspect();

            if (this.contentScaleAspect == ContentScaleAspect.CONTENT_SCALE_ASPECT_IGNORE || MathX.IsEqualApprox(viewportAspect, videoModeAspect))
            {
                //same aspect or ignore aspect
                viewportSize = desiredRes;
                screenSize = videoMode;
            }
            else if (viewportAspect < videoModeAspect)
            {
                // screen ratio is smaller vertically

                if (this.contentScaleAspect is ContentScaleAspect.CONTENT_SCALE_ASPECT_KEEP_HEIGHT or ContentScaleAspect.CONTENT_SCALE_ASPECT_EXPAND)
                {
                    //will stretch horizontally
                    viewportSize.X = desiredRes.Y * videoModeAspect;
                    viewportSize.Y = desiredRes.Y;
                    screenSize = videoMode;

                }
                else
                {
                    //will need black bars
                    viewportSize = desiredRes;
                    screenSize.X = videoMode.Y * viewportAspect;
                    screenSize.Y = videoMode.Y;
                }
            }
            else
            {
                //screen ratio is smaller horizontally
                if (this.contentScaleAspect is ContentScaleAspect.CONTENT_SCALE_ASPECT_KEEP_WIDTH or ContentScaleAspect.CONTENT_SCALE_ASPECT_EXPAND)
                {
                    //will stretch horizontally
                    viewportSize.X = desiredRes.X;
                    viewportSize.Y = desiredRes.X / videoModeAspect;
                    screenSize = videoMode;

                }
                else
                {
                    //will need black bars
                    viewportSize = desiredRes;
                    screenSize.X = videoMode.X;
                    screenSize.Y = videoMode.X / viewportAspect;
                }
            }

            screenSize = screenSize.Floor();
            viewportSize = viewportSize.Floor();

            var margin = new Vector2<RealT>();
            var offset = new Vector2<RealT>();

            if (this.contentScaleAspect != ContentScaleAspect.CONTENT_SCALE_ASPECT_EXPAND && screenSize.X < videoMode.X)
            {
                margin.X = MathF.Round((videoMode.X - screenSize.X) / 2.0f);
                offset.X = MathF.Round(margin.X * viewportSize.Y / screenSize.Y);
            }
            else if (this.contentScaleAspect != ContentScaleAspect.CONTENT_SCALE_ASPECT_EXPAND && screenSize.Y < videoMode.Y)
            {
                margin.Y = MathF.Round((videoMode.Y - screenSize.Y) / 2.0f);
                offset.Y = MathF.Round(margin.Y * viewportSize.X / screenSize.X);
            }

            switch (this.contentScaleMode)
            {
                case ContentScaleMode.CONTENT_SCALE_MODE_DISABLED:
                    // Already handled above
                    //_update_font_oversampling(1.0);
                    break;
                case ContentScaleMode.CONTENT_SCALE_MODE_CANVAS_ITEMS:
                    finalSize = screenSize.As<int>();
                    finalSizeOverride = (viewportSize / this.contentScaleFactor).As<int>();
                    attachToScreenRect = new(margin.As<int>(), screenSize.As<int>());
                    fontOversampling = screenSize.X / viewportSize.X * this.contentScaleFactor;

                    this.windowTransform.TranslateLocal(margin);
                    break;
                case ContentScaleMode.CONTENT_SCALE_MODE_VIEWPORT:
                    finalSize = (viewportSize / this.contentScaleFactor).Floor().As<int>();
                    attachToScreenRect = new(margin.As<int>(), screenSize.As<int>());

                    this.windowTransform.TranslateLocal(margin);

                    if (finalSize.X != 0 && finalSize.Y != 0)
                    {
                        var scaleTransform = new Transform2D<RealT>();
                        scaleTransform.Scale(attachToScreenRect.Size.As<RealT>() / finalSize.As<RealT>());
                        this.windowTransform *= scaleTransform;
                    }

                    break;
            }
        }

        var allocate = this.IsInsideTree && this.Visible && (this.windowId != DisplayServer.INVALID_WINDOW_ID || this.embedder != null);
        this.SetSize(finalSize, finalSizeOverride, allocate);

        if (this.windowId != DisplayServer.INVALID_WINDOW_ID)
        {
            RS.Singleton.ViewportAttachToScreen(this.ViewportId, attachToScreenRect, this.windowId);
        }
        else
        {
            RS.Singleton.ViewportAttachToScreen(this.ViewportId, new Rect2<int>(), DisplayServer.INVALID_WINDOW_ID);
        }

        if (this.windowId == DisplayServer.MAIN_WINDOW_ID)
        {
            if (!this.useFontOversampling)
            {
                fontOversampling = 1.0f;
            }
            if (TextServerManager.Singleton.FontGlobalOversampling != fontOversampling)
            {
                TextServerManager.Singleton.FontGlobalOversampling = fontOversampling;
            }
        }

        base.Notification(NotificationKind.NOTIFICATION_WM_SIZE_CHANGED);

        this.embedder?.SubWindowUpdate(this);
    }

    private void UpdateWindowCallbacks()
    {
        DisplayServer.Singleton.WindowSetRectChangedCallback(this.RectChangedCallback, this.windowId);
        DisplayServer.Singleton.WindowSetWindowEventCallback(this.EventCallback, this.windowId);
        DisplayServer.Singleton.WindowSetInputEventCallback(this.WindowInputCallback, this.windowId);
        DisplayServer.Singleton.WindowSetInputTextCallback(this.WindowInputText, this.windowId);
        DisplayServer.Singleton.WindowSetDropFilesCallback(this.WindowDropFiles, this.windowId);
    }

    private void UpdateWindowSize()
    {
        var sizeLimit = this.GetClampedMinimumSize().As<int>();

        this.windowSize = this.windowSize.Max(sizeLimit);

        var resetMinFirst = false;

        if (this.maxSizeUsed != default)
        {
            // Force window size to respect size limitations of max_size_used.
            this.windowSize = this.windowSize.Min(this.maxSizeUsed);

            if (sizeLimit.X > this.maxSizeUsed.X)
            {
                sizeLimit.X = this.maxSizeUsed.X;
                resetMinFirst = true;
            }

            if (sizeLimit.Y > this.maxSizeUsed.Y)
            {
                sizeLimit.Y = this.maxSizeUsed.Y;
                resetMinFirst = true;
            }
        }

        if (this.embedder != null)
        {
            this.windowSize = new(Math.Max(this.windowSize.X, 1), Math.Max(this.windowSize.Y, 1));

            this.embedder.SubWindowUpdate(this);
        }
        else if (this.windowId != DisplayServer.INVALID_WINDOW_ID)
        {
            if (resetMinFirst && this.wrapControls)
            {
                // Avoid an error if setting max_size to a value between min_size and the previous size_limit.
                DisplayServer.Singleton.WindowSetMinSize(default, this.windowId);
            }

            DisplayServer.Singleton.WindowSetMaxSize(this.maxSizeUsed, this.windowId);
            DisplayServer.Singleton.WindowSetMinSize(sizeLimit, this.windowId);
            DisplayServer.Singleton.WindowSetSize(this.windowSize, this.windowId);
        }

        //update the viewport
        this.UpdateViewportSize();
    }

    private void ValidateLimitSize()
    {
        // When max_size is invalid, max_size_used falls back to respect size limitations of rendering server.
        var maxSizeValid = (this.maxSize.X > 0 || this.maxSize.Y > 0) && this.maxSize.X >= this.minSize.X && this.maxSize.Y >= this.minSize.Y;
        this.maxSizeUsed = maxSizeValid ? this.maxSize : RS.Singleton.MaximumViewportSize;
    }

    private void WindowDropFiles(string[] obj) => throw new NotImplementedException();

    private void WindowInputCallback(InputEvent obj) => throw new NotImplementedException();
    private void WindowInputText(string obj) => throw new NotImplementedException();
    #endregion private methods

    #region public methods
    public void ChildControlsChanged()
    {
        if (!this.IsInsideTree || !this.Visible || this.updatingChildControls)
        {
            return;
        }

        this.updatingChildControls = true;
        MessageQueue.Singleton.PushCallable(this.UpdateChildControls);
    }

    public Viewport? GetEmbedder()
    {
        var vp = this.ParentViewport;

        while (vp != null)
        {
            if (vp.IsEmbeddingSubwindows)
            {
                return vp;
            }

            vp = vp.Parent?.Viewport;
        }

        return null;
    }

    public string? GetThemeTypeVariation() => this.themeTypeVariation;

    public bool IsLayoutRtl()
    {
        if (this.layoutDir == LayoutDirection.LAYOUT_DIRECTION_INHERITED)
        {
            if (this.Parent is Window parent_w)
            {
                return parent_w.IsLayoutRtl();
            }
            else
            {
                if (GLOBAL_GET<bool>("internationalization/rendering/force_right_to_left_layout_direction"))
                {
                    return true;
                }
                var locale = TranslationServer.Singleton.GetToolLocale();
                return TextServerManager.Singleton.IsLocaleRightToLeft(locale);
            }
        }
        else if (this.layoutDir == LayoutDirection.LAYOUT_DIRECTION_LOCALE)
        {
            if (GLOBAL_GET<bool>("internationalization/rendering/force_right_to_left_layout_direction"))
            {
                return true;
            }

            var locale = TranslationServer.Singleton.GetToolLocale();
            return TextServerManager.Singleton.IsLocaleRightToLeft(locale);
        }
        else
        {
            return this.layoutDir == LayoutDirection.LAYOUT_DIRECTION_RTL;
        }
    }

    public bool IsWrappingControls() => this.wrapControls;

    public void SetFlag(Flags flag, bool enabled)
    {
        if (ERR_FAIL_INDEX((int)flag, (int)Flags.FLAG_MAX))
        {
            return;
        }

        this.flags[(int)flag] = enabled;

        if (this.embedder != null)
        {
            this.embedder.SubWindowUpdate(this);

        }
        else if (this.windowId != DisplayServer.INVALID_WINDOW_ID)
        {
            if (!this.IsInEditedSceneRoot)
            {
                DisplayServer.Singleton.WindowSetFlag((DisplayServer.WindowFlags)flag, enabled, this.windowId);
            }
        }
    }

    public void UpdateMouseCursorShape() => throw new NotImplementedException();
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
            case NotificationKind.NOTIFICATION_POSTINITIALIZE:
                #region TODO
                // _invalidate_theme_cache();
                // _update_theme_item_cache();
                #endregion TODO
                break;
            #region TODO
            //         case NOTIFICATION_PARENTED: {
            //             theme_owner->assign_theme_on_parented(this);
            //         } break;

            //         case NOTIFICATION_UNPARENTED: {
            //             theme_owner->clear_theme_on_unparented(this);
            //         } break;
            #endregion TODO
            case NotificationKind.NOTIFICATION_ENTER_TREE:
                {
                    var embedded = false;
                    this.embedder = this.GetEmbedder();

                    if (this.embedder != null)
                    {
                        embedded = true;
                        if (!this.Visible)
                        {
                            this.embedder = null; // Not yet since not visible.
                        }
                    }

                    if (embedded)
                    {
                        // Create as embedded.
                        if (this.embedder != null)
                        {
                            this.embedder.SubWindowRegister(this);
                            RS.Singleton.ViewportSetUpdateMode(this.ViewportId, RS.ViewportUpdateMode.VIEWPORT_UPDATE_WHEN_PARENT_VISIBLE);
                            this.UpdateWindowSize();
                        }
                    }
                    else
                    {
                        if (this.Parent == null)
                        {
                            // It's the root window!
                            this.Visible = true; // Always visible.
                            this.windowId = DisplayServer.MAIN_WINDOW_ID;
                            DisplayServer.Singleton.WindowAttachInstanceId(this.InstanceId, this.windowId);
                            this.UpdateFromWindow();
                            // Since this window already exists (created on start), we must update pos and size from it.

                            this.position = DisplayServer.Singleton.WindowGetPosition(this.windowId);
                            this.windowSize = DisplayServer.Singleton.WindowGetSize(this.windowId);

                            this.UpdateWindowSize(); // Inform DisplayServer of minimum and maximum size.
                            this.UpdateViewportSize(); // Then feed back to the viewport.
                            this.UpdateWindowCallbacks();
                            RS.Singleton.ViewportSetUpdateMode(this.ViewportId, RS.ViewportUpdateMode.VIEWPORT_UPDATE_WHEN_VISIBLE);
                        }
                        else
                        {
                            // Create.
                            if (this.Visible)
                            {
                                this.MakeWindow();
                            }
                        }
                    }

                    if (this.transient)
                    {
                        this.MakeTransient();
                    }

                    if (this.Visible)
                    {
                        base.Notification(NotificationKind.WINDOW_NOTIFICATION_VISIBILITY_CHANGED);
                        VisibilityChanged?.Invoke();
                        RS.Singleton.ViewportSetActive(this.ViewportId, true);
                    }

                    base.Notification(NotificationKind.WINDOW_NOTIFICATION_THEME_CHANGED);
                }
                break;

            //         case NOTIFICATION_THEME_CHANGED: {
            //             emit_signal(SceneStringNames::Singleton.theme_changed);
            //             _invalidate_theme_cache();
            //             _update_theme_item_cache();
            //         } break;

            case NotificationKind.NOTIFICATION_READY:
                if (this.wrapControls)
                {
                    // Finish any resizing immediately so it doesn't interfere on stuff overriding _ready().
                    this.UpdateChildControls();
                }
                break;

            //         case NOTIFICATION_TRANSLATION_CHANGED: {
            //             _invalidate_theme_cache();
            //             _update_theme_item_cache();

            //             if (!embedder && window_id != DisplayServer.INVALID_WINDOW_ID) {
            //                 String tr_title = atr(title);
            // #ifdef DEBUG_ENABLED
            //                 if (window_id == DisplayServer.MAIN_WINDOW_ID) {
            //                     // Append a suffix to the window title to denote that the project is running
            //                     // from a debug build (including the editor). Since this results in lower performance,
            //                     // this should be clearly presented to the user.
            //                     tr_title = vformat("%s (DEBUG)", tr_title);
            //                 }
            // #endif
            //                 DisplayServer.Singleton.window_set_title(tr_title, window_id);
            //             }
            //         } break;

            case NotificationKind.NOTIFICATION_EXIT_TREE:
                if (this.transient)
                {
                    this.ClearTransient();
                }

                if (!this.IsEmbedded && this.windowId != DisplayServer.INVALID_WINDOW_ID)
                {
                    if (this.windowId == DisplayServer.MAIN_WINDOW_ID)
                    {
                        RS.Singleton.ViewportSetUpdateMode(this.ViewportId, RS.ViewportUpdateMode.VIEWPORT_UPDATE_DISABLED);
                        this.UpdateWindowCallbacks();
                    }
                    else
                    {
                        this.ClearWindow();
                    }
                }
                else
                {
                    if (this.embedder != null)
                    {
                        this.embedder.SubWindowRemove(this);
                        this.embedder = null;

                        RS.Singleton.ViewportSetUpdateMode(this.ViewportId, RS.ViewportUpdateMode.VIEWPORT_UPDATE_DISABLED);
                    }

                    this.UpdateViewportSize(); //called by clear and make, which does not happen here
                }

                RS.Singleton.ViewportSetActive(this.ViewportId, false);
                break;
        }

        if (reversed)
        {
            base.Notification(notification, reversed);
        }
    }
    #endregion public override methods
}
