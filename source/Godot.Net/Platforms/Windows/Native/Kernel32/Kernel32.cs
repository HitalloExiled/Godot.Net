
namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.InteropServices;

internal static partial class Kernel32
{
    public static readonly HANDLE INVALID_HANDLE_VALUE = new(-1);

    public const int LANG_NEUTRAL   = 0x00;
    public const int LANG_INVARIANT = 0x7f;

    public const int SUBLANG_NEUTRAL            = 0x00;
    public const int SUBLANG_DEFAULT            = 0x01;
    public const int SUBLANG_SYS_DEFAULT        = 0x02;
    public const int SUBLANG_CUSTOM_DEFAULT     = 0x03;
    public const int SUBLANG_CUSTOM_UNSPECIFIED = 0x04;
    public const int SUBLANG_UI_CUSTOM_DEFAULT  = 0x05;

    public const uint ATTACH_PARENT_PROCESS = 0x0ffffffff;
    public const int ERROR_ACCESS_DENIED = 5; // process was already attached to another console

    ///<summary>The value indicated by lpString1 is less than the value indicated by lpString2.</summary>
    public const int CSTR_LESS_THAN = -1;
    ///<summary>The value indicated by lpString1 equals the value indicated by lpString2.</summary>
    public const int CSTR_EQUAL = 0;
    ///<summary>The value indicated by lpString1 is greater than the value indicated by lpString2.</summary>
    public const int CSTR_GREATER_THAN = 1;

    [LibraryImport(nameof(Kernel32))]
    public static partial BOOL AttachConsole(DWORD dwProcessId);

    [LibraryImport(nameof(Kernel32))]
    public static partial BOOL CloseHandle(HANDLE hObject);

    [LibraryImport(nameof(Kernel32))]
    public static partial int CompareStringOrdinal(LPCWCH lpString1, int cchCount1, LPCWCH lpString2, int cchCount2, BOOL bIgnoreCase);

    [LibraryImport(nameof(Kernel32))]
    public static partial BOOL FreeLibrary(HMODULE hLibModule);

    public static int CompareStringOrdinal(string string1, int cchCount1, string string2, int cchCount2, BOOL bIgnoreCase)
    {
        using var lpString1 = new LPCWCH(string1);
        using var lpString2 = new LPCWCH(string2);

        return CompareStringOrdinal(lpString1, cchCount1, lpString2, cchCount2, bIgnoreCase);
    }

    [LibraryImport(nameof(Kernel32))]
    public static unsafe partial DWORD FormatMessageW(FORMAT_MESSAGE_FLAGS dwFlags, LPCVOID lpSource, DWORD dwMessageId, DWORD dwLanguageId, in LPWSTR lpBuffer, DWORD nSize, nint Arguments);

    public unsafe static DWORD FormatMessageW(FORMAT_MESSAGE_FLAGS dwFlags, LPCVOID lpSource, DWORD dwMessageId, DWORD dwLanguageId, out string? message, DWORD nSize, nint arguments)
    {
        LPWSTR lpBuffer = null;

        var result = FormatMessageW(dwFlags, lpSource, dwMessageId, dwLanguageId, lpBuffer, nSize, arguments);

        message = Marshal.PtrToStringUni(lpBuffer.Value);

        lpBuffer.Dispose();

        return result;
    }

    [LibraryImport(nameof(Kernel32))]
    public static partial HANDLE GetCurrentProcess();

    [LibraryImport(nameof(Kernel32))]
    public static partial DWORD GetCurrentProcessId();

    [LibraryImport(nameof(Kernel32))]
    public static partial HANDLE GetCurrentThread();

    [LibraryImport(nameof(Kernel32))]
    public static partial DWORD GetCurrentThreadId();

    [LibraryImport(nameof(Kernel32))]
    public static partial DWORD GetLastError();

    [LibraryImport(nameof(Kernel32))]
    public static partial HMODULE GetModuleHandleW(LPCWSTR lpModuleName);

    public static HMODULE GetModuleHandleW(string? moduleName)
    {
        using var lpModuleName = new LPCWSTR(moduleName);

        return GetModuleHandleW(lpModuleName);
    }

    [LibraryImport(nameof(Kernel32))]
    public static partial FARPROC GetProcAddress(HMODULE hModule, LPCSTR lpProcName);

    public static FARPROC GetProcAddress(HMODULE hModule, string procName)
    {
        using var lpProcName = new LPCSTR(procName);

        return GetProcAddress(hModule, lpProcName);
    }

    [LibraryImport(nameof(Kernel32))]
    public static partial HMODULE LoadLibraryExW(LPCWSTR lpLibFileName, HANDLE hFile, LOAD_LIBRARY_FLAGS dwFlags);

    public static HMODULE LoadLibraryExW(string libFileName, HANDLE hFile, LOAD_LIBRARY_FLAGS dwFlags)
    {
        using var lpLibFileName = new LPCWSTR(libFileName);

        return LoadLibraryExW(lpLibFileName, hFile, dwFlags);
    }

    [LibraryImport(nameof(Kernel32))]
    public static partial HMODULE LoadLibraryW(LPCWSTR lpLibFileName);

    public static HMODULE LoadLibraryW(string libFileName)
    {
        using var lpLibFileName = new LPCWSTR(libFileName);

        return LoadLibraryW(lpLibFileName);
    }


    [LibraryImport(nameof(Kernel32))]
    public static partial BOOL PowerClearRequest(HANDLE powerRequest, POWER_REQUEST_TYPE requestType);

    [LibraryImport(nameof(Kernel32))]
    public static partial HANDLE PowerCreateRequest(PREASON_CONTEXT context);

    [LibraryImport(nameof(Kernel32))]
    public static partial HANDLE PowerCreateRequest(in REASON_CONTEXT context);

    [LibraryImport(nameof(Kernel32))]
    public static partial BOOL PowerSetRequest(HANDLE powerRequest, POWER_REQUEST_TYPE requestType);

    [LibraryImport(nameof(Kernel32))]
    public static partial void OutputDebugStringW(LPCWSTR lpOutputString);

    public static void OutputDebugStringW(string? outputString)
    {
        using var lpOutputString = new LPCWSTR(outputString);

        OutputDebugStringW(lpOutputString);
    }

    [LibraryImport(nameof(Kernel32))]
    public static partial BOOL QueryPerformanceCounter(in LARGE_INTEGER lpPerformanceCount);

    public static BOOL QueryPerformanceCounter(out long performanceCount)
    {
        LARGE_INTEGER largeInteger = default;

        var result = QueryPerformanceCounter(largeInteger);

        performanceCount = largeInteger.QuadPart;

        return result;
    }

    [LibraryImport(nameof(Kernel32))]
    public static partial BOOL QueryPerformanceFrequency(in LARGE_INTEGER lpFrequency);

    public static BOOL QueryPerformanceFrequency(out long frequency)
    {
        LARGE_INTEGER largeInteger = default;

        var result = QueryPerformanceFrequency(largeInteger);

        frequency = largeInteger.QuadPart;

        return result;
    }

    [LibraryImport(nameof(Kernel32))]
    public static partial BOOL SetPriorityClass(HANDLE handle, PROCESS_CREATION_FLAGS priorityClass);

    [LibraryImport(nameof(Kernel32))]
    public static partial BOOL SetThreadPriority(HANDLE hThread, THREAD_PRIORITY nPriority);
}
