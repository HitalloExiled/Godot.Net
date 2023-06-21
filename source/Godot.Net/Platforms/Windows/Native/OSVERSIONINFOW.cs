namespace Godot.Net.Platforms.Windows.Native;

public unsafe struct OSVERSIONINFOW
{
    public DWORD      dwOSVersionInfoSize;
    public DWORD      dwMajorVersion;
    public DWORD      dwMinorVersion;
    public DWORD      dwBuildNumber;
    public DWORD      dwPlatformId;
    public fixed char szCSDVersion[128];
}
