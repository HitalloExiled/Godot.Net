
namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.InteropServices;

internal static partial class Kernel32
{
    [StructLayout(LayoutKind.Sequential)]
    public struct STARTUPINFOW
    {
        public DWORD  cb;
        public LPWSTR lpReserved;
        public LPWSTR lpDesktop;
        public LPWSTR lpTitle;
        public DWORD  dwX;
        public DWORD  dwY;
        public DWORD  dwXSize;
        public DWORD  dwYSize;
        public DWORD  dwXCountChars;
        public DWORD  dwYCountChars;
        public DWORD  dwFillAttribute;
        public DWORD  dwFlags;
        public WORD   wShowWindow;
        public WORD   cbReserved2;
        public LPBYTE lpReserved2;
        public HANDLE hStdInput;
        public HANDLE hStdOutput;
        public HANDLE hStdError;
    }
}
