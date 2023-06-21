namespace Godot.Net.Platforms.Windows;

using Godot.Net.Core.Input;
using Godot.Net.Core.Math;
using Godot.Net.Platforms.Windows.Native;

public partial class DisplayServerWindows
{
    public record WindowData
    {
        public bool                         AlwaysOnTop         { get; set; }
        public bool                         BlockMm             { get; set; }
        public bool                         Borderless          { get; set; }
        public bool                         ContextCreated      { get; set; }
        public Action<string[]>?            DropFilesCallback   { get; set; }
        public Action<WindowEvent>?         EventCallback       { get; set; }
        public bool                         Exclusive           { get; set; }
        public UINT_PTR                     FocusTimerId        { get; set; }
        public bool                         Fullscreen          { get; set; }
        public int                          Height              { get; set; }
        public HWND                         HWnd                { get; set; }
        public Vector2<RealT>               ImPosition          { get; set; }
        public Action<InputEvent>?          InputEventCallback  { get; set; }
        public Action<string>?              InputTextCallback   { get; set; }
        public int                          InstanceId          { get; set; }
        public bool                         IsPopup             { get; set; }
        public bool                         LastPenInverted     { get; set; }
        public Vector2<int>                 LastPos             { get; set; }
        public float                        LastPressure        { get; set; }
        public int                          LastPressureUpdate  { get; set; }
        public Vector2<RealT>               LastTilt            { get; set; }
        public bool                         LayeredWindow       { get; set; }
        public bool                         Maximized           { get; set; }
        public int                          MaxPressure         { get; set; }
        public Vector2<RealT>               MaxSize             { get; set; }
        public bool                         Minimized           { get; set; }
        public int                          MinPressure         { get; set; }
        public Vector2<RealT>               MinSize             { get; set; }
        public bool                         Mpass               { get; set; }
        public List<Vector2<RealT>>         Mpath               { get; } = new();
        public bool                         MultiwindowFs       { get; set; }
        public bool                         NoFocus             { get; set; }
        public bool                         PreFsValid          { get; set; }
        public Action<Rect2<int>>?          RectChangedCallback { get; set; }
        public bool                         Resizable           { get; set; }
        public LPARAM                       SavedLParam         { get; set; }
        public WPARAM                       SavedWParam         { get; set; }
        public bool                         TiltSupported       { get; set; }
        public List<int>                    TransientChildren   { get; } = new();
        public int                          TransientParent     { get; set; }
        public int                          Width               { get; set; }
        public Action<WindowEvent>?         WindowEventCallback { get; set; }
        public bool                         WindowFocused       { get; set; }
        public bool                         WindowHasFocus      { get; set; }
        public bool                         Wtctx               { get; set; }
        public LOGCONTEXTW                  Wtlc                { get; set; }
    }
}

