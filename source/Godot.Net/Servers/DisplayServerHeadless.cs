namespace Godot.Net.Servers;

using Godot.Net.Core.Error;
using Godot.Net.Core.Input;
using Godot.Net.Core.Math;

public class DisplayServerHeadless : DisplayServer
{
    public override bool CanAnyWindowDraw => throw new NotImplementedException();

    public static DisplayServer CreateFunc(string renderingDriver, WindowMode windowMode, VSyncMode windowVsyncMode, WindowFlagsBit windowFlags, out Vector2<int> windowPosition, Vector2<int> windowSize, int screen, out Error err) => throw new NotImplementedException();

    public override int CreateSubWindow(WindowMode mode, VSyncMode vsyncMode, int flags, in Rect2<int> windowRect) => throw new NotImplementedException();
    public static List<string> GetRenderingDriversFunc() => throw new NotImplementedException();
    public override int GetScreenCount() => throw new NotImplementedException();
    public override void GlWindowMakeCurrent(int window) => throw new NotImplementedException();
    public override bool HasFeature(Feature feature) => throw new NotImplementedException();
    public override void MakeRenderingThread() => throw new NotImplementedException();
    public override void ProcessEvents() => throw new NotImplementedException();
    public override void ReleaseRenderingThread() => throw new NotImplementedException();
    public override int ScreenGetDpi(int screen = -1) => throw new NotImplementedException();
    public override float ScreenGetMaxScale() => throw new NotImplementedException();
    public override Vector2<int> ScreenGetPosition(int window) => throw new NotImplementedException();
    public override Vector2<int> ScreenGetSize(int window) => throw new NotImplementedException();
    public override Rect2<int> ScreenGetUsableRect(int screen = -1) => throw new NotImplementedException();
    public override void ScreenSetOrientation(ScreenOrientation orientation) => throw new NotImplementedException();
    public override void ShowWindow(int window) => throw new NotImplementedException();
    public override void SwapBuffers() => throw new NotImplementedException();
    public override void WindowAttachInstanceId(int instance, int window) => throw new NotImplementedException();
    public override bool WindowCanDraw() => throw new NotImplementedException();
    public override int WindowGetCurrentScreen(int window = 0) => throw new NotImplementedException();
    public override bool WindowGetFlag(WindowFlags windowFlags, int window) => throw new NotImplementedException();
    public override WindowMode WindowGetMode(int window) => throw new NotImplementedException();
    public override Vector2<int> WindowGetPosition(int window) => throw new NotImplementedException();
    public override Vector2<int> WindowGetSize(int window) => throw new NotImplementedException();
    public override VSyncMode WindowGetVsyncMode(int window) => throw new NotImplementedException();
    public override void WindowSetCurrentScreen(int screen, int window = default) => throw new NotImplementedException();
    public override void WindowSetDropFilesCallback(Action<string[]> callback, int window) => throw new NotImplementedException();
    public override void WindowSetExclusive(int window, bool exclusive) => throw new NotImplementedException();
    public override void WindowSetFlag(WindowFlags flag, bool enabled, int window = default) => throw new NotImplementedException();
    public override void WindowSetInputEventCallback(Action<InputEvent> callback, int window) => throw new NotImplementedException();
    public override void WindowSetInputTextCallback(Action<string> callback, int window) => throw new NotImplementedException();
    public override void WindowSetMaxSize(in Vector2<int> size, int window) => throw new NotImplementedException();
    public override void WindowSetMinSize(in Vector2<int> size, int window) => throw new NotImplementedException();
    public override void WindowSetMode(WindowMode windowMode) => throw new NotImplementedException();
    public override void WindowSetMousePassthrough(in Vector2<float>[] mpath, int window) => throw new NotImplementedException();
    public override void WindowSetPosition(in Vector2<int> position, int window = default) => throw new NotImplementedException();
    public override void WindowSetRectChangedCallback(Action<Rect2<int>> callback, int window) => throw new NotImplementedException();
    public override void WindowSetSize(in Vector2<int> size, int window) => throw new NotImplementedException();
    public override void WindowSetTitle(string? title, int window) => throw new NotImplementedException();
    public override void WindowSetTransient(int window, int parent) => throw new NotImplementedException();
    public override void WindowSetWindowEventCallback(Action<WindowEvent> callback, int window) => throw new NotImplementedException();
}
