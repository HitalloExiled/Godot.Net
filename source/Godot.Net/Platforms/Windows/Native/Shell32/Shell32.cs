namespace Godot.Net.Platforms.Windows.Native;

using System.Runtime.InteropServices;

internal static partial class Shell32
{
    [LibraryImport(nameof(Shell32))]
    public static unsafe partial HRESULT SHGetKnownFolderPath(REFKNOWNFOLDERID rfid, KNOWN_FOLDER_FLAG dwFlags, HANDLE hToken, in PWSTR ppszPath);

    public static unsafe HRESULT SHGetKnownFolderPath(REFKNOWNFOLDERID rfid, KNOWN_FOLDER_FLAG dwFlags, HANDLE hToken, out string path)
    {
        using var ppszPath = default(PWSTR);

        var result = SHGetKnownFolderPath(rfid, dwFlags, hToken, ppszPath);

        path = (string)ppszPath!;

        return result;
    }
}
