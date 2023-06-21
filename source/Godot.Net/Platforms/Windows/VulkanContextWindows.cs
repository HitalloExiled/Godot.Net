namespace Godot.Net.Platforms.Windows;

using Godot.Net.Core.Error;
using Godot.Net.Drivers.Vulkan;
using Godot.Net.Servers;
using Silk.NET.Vulkan.Extensions.KHR;

public class VulkanContextWindows : VulkanContext
{
    protected override string PlatformSurfaceExtension => KhrWin32Surface.ExtensionName;

    public void SetVsyncMode(int window, DisplayServer.VSyncMode vsyncMode) => throw new NotImplementedException();
    public Error WindowCreate(int id, DisplayServer.VSyncMode vsyncMode, nint hWnd, nint value, long v1, long v2) => throw new NotImplementedException();
    public void WindowResize(int window, int w, int h) => throw new NotImplementedException();
}
