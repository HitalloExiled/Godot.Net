namespace Godot.Net.Core.IO;

using Error = Error.Error;

#pragma warning disable IDE0052 // TODO Remove

public partial class PackedData
{
    private static PackedData? singleton;

    public static PackedData Singleton => singleton ?? throw new NullReferenceException();

    private readonly PackedDir        root;
    private readonly List<PackSource> sources = new();

    public bool IsDisabled { get; set; }

    public PackedData()
    {
        singleton = this;
        this.root = new PackedDir();

        this.AddPackSource(new PackedSourcePCK());
    }

    public Error AddPack(string path, bool replaceFiles, ulong offset)
    {
        foreach (var source in this.sources)
        {
            if (source.TryOpenPack(path, replaceFiles, offset))
            {
                return Error.OK;
            }
        }

        return Error.ERR_FILE_UNRECOGNIZED;
    }

    public void AddPath(string pkgPath, string path, ulong ofs, ulong size, byte[] md5, PackSource src, bool replaceFiles, bool encrypted = false) => throw new NotImplementedException(); // for PackSource

    public void AddPackSource(PackSource source) => this.sources.Add(source);
}
