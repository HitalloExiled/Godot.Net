namespace Godot.Net.Core.IO;
#pragma warning disable IDE0044 // TODO Remove

public abstract class PackSource
{
	public abstract FileAccess GetFile(string path, PackedData.PackedFile file);
    public abstract bool TryOpenPack(string path, bool replaceFiles, ulong offset);
}
