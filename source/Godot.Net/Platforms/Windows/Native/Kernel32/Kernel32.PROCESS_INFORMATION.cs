
namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.InteropServices;

internal static partial class Kernel32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PROCESS_INFORMATION
    {
        public HANDLE hProcess;
        public HANDLE hThread;
        public DWORD  dwProcessId;
        public DWORD  dwThreadId;
    }
}
