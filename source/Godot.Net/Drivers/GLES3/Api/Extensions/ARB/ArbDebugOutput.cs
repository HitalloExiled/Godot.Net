namespace Godot.Net.Drivers.GLES3.Api.Extensions.ARB;

using System.Runtime.CompilerServices;
using Godot.Net.Core;
using Godot.Net.Drivers.GLES3.Api.Enums;
using Godot.Net.Drivers.GLES3.Api.Interfaces;
using Godot.Net.Extensions;

public partial class ArbDebugOutput : IExtension
{
    public static string Name => "GL_ARB_debug_output";

    public delegate void DebugProcNoParam(GLEnum source, GLEnum type, uint id, GLEnum severity, string message);
    public delegate void DebugProc<T>(GLEnum source, GLEnum type, uint id, GLEnum severity, string message, in T userParam = default) where T : unmanaged;
    public unsafe delegate void DebugProc(GLEnum source, GLEnum type, uint id, GLEnum severity, nint length, byte* message, nint userParam);

    private delegate void GLDebugMessageCallbackARB(DebugProc callback, nint userParam);

    private readonly ILoader loader;
    private readonly GLDebugMessageCallbackARB glDebugMessageCallbackARB;

    public ArbDebugOutput(ILoader loader)
    {
        this.loader = loader;

        this.glDebugMessageCallbackARB = this.loader.Load<GLDebugMessageCallbackARB>("glDebugMessageCallbackARB");
    }

    public void DebugMessageCallback(DebugProc callback, nint userParam) =>
        this.glDebugMessageCallbackARB.Invoke(callback, userParam);

    public unsafe void DebugMessageCallback(DebugProcNoParam callback)
    {
        void unmanagedCallback(GLEnum source, GLEnum type, uint id, GLEnum severity, nint length, byte* message, nint userParam) =>
            callback(source, type, id, severity, UnmanagedUtils.PointerToArray(message, (int)length).ConvertToString());

        this.glDebugMessageCallbackARB.Invoke(unmanagedCallback, default);
    }

    public unsafe void DebugMessageCallback<T>(DebugProc<T> callback, in T userParam) where T : unmanaged
    {
        void unmanagedCallback(GLEnum source, GLEnum type, uint id, GLEnum severity, nint length, byte* message, nint userParam) =>
            callback(source, type, id, severity, UnmanagedUtils.PointerToArray(message, (int)length).ConvertToString(), Unsafe.AsRef<T>(userParam.ToPointer()));

        fixed (T* pointer = &userParam)
        {
            this.glDebugMessageCallbackARB.Invoke(unmanagedCallback, new(pointer));
        }
    }

    public static IExtension Create(ILoader loader) => new ArbDebugOutput(loader);
}
