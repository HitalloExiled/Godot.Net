namespace Godot.Net.Drivers.GLES3.Api.Extensions.KHR;

using System.Runtime.CompilerServices;
using Godot.Net.Core;
using Godot.Net.Drivers.GLES3.Api.Interfaces;
using Godot.Net.Extensions;

public class KhrDebug : IExtension
{
    public static string Name => "GL_KHR_debug";

    public delegate void DebugProc<T>(int source, int type, uint id, int severity, string message, in T userParam);
    public unsafe delegate void DebugProc(int source, int type, uint id, int severity, nint length, byte* message, nint userParam);

    private delegate void GLDebugMessageCallbackKHR(DebugProc callback, nint userParam);
    private readonly GLDebugMessageCallbackKHR glDebugMessageCallbackKHR;

    public KhrDebug(ILoader loader) => this.glDebugMessageCallbackKHR = loader.Load<GLDebugMessageCallbackKHR>("glDebugMessageCallbackKHR");

    public static IExtension Create(ILoader loader) => new KhrDebug(loader);

    public void DebugMessageCallback(DebugProc callback, nint userParam) =>
        this.glDebugMessageCallbackKHR.Invoke(callback, userParam);

    public unsafe void DebugMessageCallback<T>(DebugProc<T> callback, in T userParam) where T : unmanaged
    {
        void unmanagedCallback(int source, int type, uint id, int severity, nint length, byte* message, nint userParam) =>
            callback(source, type, id, severity, UnmanagedUtils.PointerToArray(message, (int)length).ConvertToString(), Unsafe.AsRef<T>(userParam.ToPointer()));

        fixed (T* pointer = &userParam)
        {
            this.glDebugMessageCallbackKHR.Invoke(unmanagedCallback, new(pointer));
        }
    }
}
