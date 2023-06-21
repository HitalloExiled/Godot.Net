namespace Godot.Net.Platforms.Windows.Native.Winmm;

using System.Runtime.InteropServices;

internal static partial class Winmm
{
    [LibraryImport(nameof(Winmm), EntryPoint = "timeBeginPeriod")]
    public static partial MMRESULT TimeBeginPeriod(UINT uPeriod);
}
