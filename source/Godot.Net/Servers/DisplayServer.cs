namespace Godot.Net.Servers;

using Godot.Net.Core.Error;
using Godot.Net.Core.Input;
using Godot.Net.Core.Math;

public abstract partial class DisplayServer
{
    private const int MAX_SERVERS = 64;

    public const int INVALID_WINDOW_ID     = -1;
    public const int MAIN_WINDOW_ID        = 0;
    public const int SCREEN_OF_MAIN_WINDOW = -1;
    public const int SCREEN_PRIMARY        = -2;

    public delegate DisplayServer CreateFuncDelegate(string renderingDriver, WindowMode windowMode, VSyncMode windowVsyncMode, WindowFlagsBit windowFlags, out Vector2<int> windowPosition, Vector2<int> windowSize, int screen, out Error err);

    public static DisplayServer Singleton { get; protected set; } = null!;

    public static List<DisplayServerCreate> ServerCreateFunctions { get; } = new()
    {
        new DisplayServerCreate("headless", DisplayServerHeadless.CreateFunc, DisplayServerHeadless.GetRenderingDriversFunc),
    };

    public static bool  WindowEarlyClearOverrideEnabled { get; private set; }
    public static Color WindowEarlyClearOverrideColor   { get; private set; }

    public abstract bool CanAnyWindowDraw { get; }

    public virtual bool IsTouchscreenAvailable => Input.IsInitialized && Input.Singleton.EmulatingTouchFromMouse;
    public virtual bool SwapCancelOk           => false;

    public ContextType Context { get; set; }

    protected static void RegisterCreateFunction(string name, CreateFuncDelegate createFunc, Func<List<string>> getRenderingDriversFunc)
    {
        if (ERR_FAIL_COND(ServerCreateFunctions.Count == MAX_SERVERS))
        {
            return;
        }

        ServerCreateFunctions.Add(ServerCreateFunctions.Last());

        ServerCreateFunctions[^2] = new (name, createFunc, getRenderingDriversFunc);
    }

    public static DisplayServer? Create(int index, string renderingDriver, WindowMode windowMode, VSyncMode windowVsyncMode, WindowFlagsBit windowFlags, out Vector2<int> windowPosition, Vector2<int> windowSize, int screen, out Error err)
    {
        if (ERR_FAIL_INDEX_V(index, ServerCreateFunctions.Count))
        {
            windowPosition = default;
            err            = Error.FAILED;

            return null;
        }

        return ServerCreateFunctions[index].CreateFunction(
            renderingDriver,
            windowMode,
            windowVsyncMode,
            windowFlags,
            out windowPosition,
            windowSize,
            screen,
            out err
        );
    }

    public static void SetEarlyWindowClearColorOverride(bool enabled, in Color color)
    {
        WindowEarlyClearOverrideEnabled = enabled;
        WindowEarlyClearOverrideColor   = color;
    }

    #region public virtual methods
    public virtual Vector2<RealT> ImeGetSelection() => throw new NotImplementedException();
	public virtual string ImeGetText() => throw new NotImplementedException();
    public virtual void VirtualKeyboardHide() => throw new NotImplementedException();
    public virtual void WindowSetImeActive(bool active, int window = MAIN_WINDOW_ID) => throw new NotImplementedException();
    public virtual void WindowSetImePosition(in Vector2<RealT> pos, int window = MAIN_WINDOW_ID) => throw new NotImplementedException();
    #endregion public virtual methods

    #region public abstract methods
    public abstract int CreateSubWindow(WindowMode mode, VSyncMode vsyncMode, int flags, in Rect2<int> windowRect);
    public abstract int GetScreenCount();
    public abstract void GlWindowMakeCurrent(int window);
    public abstract bool HasFeature(Feature feature);
    public abstract void MakeRenderingThread();
    public abstract void ProcessEvents();
    public abstract void ReleaseRenderingThread();
    public abstract  int ScreenGetDpi(int screen = SCREEN_OF_MAIN_WINDOW);
    public abstract float ScreenGetMaxScale();
    public abstract Vector2<int> ScreenGetPosition(int window);
    public abstract Vector2<int> ScreenGetSize(int window);
    public abstract Rect2<int> ScreenGetUsableRect(int screen = SCREEN_OF_MAIN_WINDOW);
    public abstract void ScreenSetOrientation(ScreenOrientation orientation);
    public abstract void ShowWindow(int window);
    public abstract void SwapBuffers();
    public abstract void WindowAttachInstanceId(int instance, int window);
    public abstract bool WindowCanDraw();
    public abstract int WindowGetCurrentScreen(int window = MAIN_WINDOW_ID);
    public abstract bool WindowGetFlag(WindowFlags windowFlags, int window);
    public abstract WindowMode WindowGetMode(int window);
    public abstract Vector2<int> WindowGetPosition(int window);
    public abstract Vector2<int> WindowGetSize(int window = MAIN_WINDOW_ID);
    public abstract VSyncMode WindowGetVsyncMode(int window);
    public abstract void WindowSetCurrentScreen(int screen, int window = default);
    public abstract void WindowSetDropFilesCallback(Action<string[]> callback, int window);
    public abstract void WindowSetExclusive(int window, bool exclusive);
    public abstract void WindowSetFlag(WindowFlags flag, bool enabled, int window = default);
    public abstract void WindowSetInputEventCallback(Action<InputEvent> callback, int window);
    public abstract void WindowSetInputTextCallback(Action<string> callback, int window);
    public abstract void WindowSetMaxSize(in Vector2<int> size, int window);
    public abstract void WindowSetMinSize(in Vector2<int> size, int window);
    public abstract void WindowSetMode(WindowMode windowMode);
    public abstract void WindowSetMousePassthrough(in Vector2<float>[] mpath, int window);
    public abstract void WindowSetPosition(in Vector2<int> position, int window = default);
    public abstract void WindowSetRectChangedCallback(Action<Rect2<int>> callback, int window);
    public abstract void WindowSetSize(in Vector2<int> size, int window = MAIN_WINDOW_ID);
    public abstract void WindowSetTitle(string? title, int window = 0);
    public abstract void WindowSetTransient(int window, int parent);
    public abstract void WindowSetWindowEventCallback(Action<WindowEvent> callback, int window);
    #endregion public abstract methods
}
