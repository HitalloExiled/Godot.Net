namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.InteropServices;

internal static partial class AVRT
{

    [LibraryImport(nameof(AVRT))]
    public static partial BOOL AvSetMmThreadPriority(HANDLE avrtHandle, AVRT_PRIORITY priority);

    [LibraryImport(nameof(AVRT))]
    public static partial HANDLE AvSetMmThreadCharacteristicsW(LPCWSTR taskName, LPDWORD taskIndex);

    public static unsafe HANDLE AvSetMmThreadCharacteristicsW(string? taskName, out DWORD taskIndex)
    {
        using var pTaskName = new LPCWSTR(taskName);

        fixed (DWORD* pTaskIndex = &taskIndex)
        {
            return AvSetMmThreadCharacteristicsW(pTaskName, pTaskIndex);
        }
    }
}
