namespace Godot.Net.Core.IO;

using System.IO;
using Godot.Net.Extensions;
using OS = OS.OS;

public partial class PackedSourcePCK : PackSource
{

    private const int PACK_HEADER_MAGIC   = 0x43504447;
    private const int PACK_FORMAT_VERSION = 2;

    public override FileAccess GetFile(string path, PackedData.PackedFile file) => throw new NotImplementedException();

    public override bool TryOpenPack(string path, bool replaceFiles, ulong offset)
    {
        if (!File.Exists(path))
        {
            return false;
        }

        using var file   = File.OpenRead(path);
        using var reader = new BinaryReader(file);

        var pckHeaderFound = false;

        // Search for the header at the start offset - standalone PCK file.
        file.Seek((int)offset, SeekOrigin.Begin);
        var magic = reader.ReadUInt32();
        if (magic == PACK_HEADER_MAGIC)
        {
            pckHeaderFound = true;
        }

        // Search for the header in the executable "pck" section - self contained executable.
        if (!pckHeaderFound)
        {
            // Loading with offset feature not supported for self contained exe files.
            if (offset != 0)
            {
                ERR_FAIL_V_MSG(false, "Loading self-contained executable with offset not supported.");
            }

            var pckOff = OS.Singleton.GetEmbeddedPckOffset();
            if (pckOff != 0)
            {
                // Search for the header, in case PCK start and section have different alignment.
                for (var i = 0; i < 8; i++)
                {
                    file.Seek(pckOff, SeekOrigin.Begin);

                    magic = reader.ReadUInt32();
                    if (magic == PACK_HEADER_MAGIC)
                    {
                        #if DEBUG
                        PrintVerbose($"PCK header found in executable pck section, loading from offset 0x{pckOff - 4:X}");
                        #endif
                        pckHeaderFound = true;
                        break;
                    }
                    pckOff++;
                }
            }
        }

        // Search for the header at the end of file - self contained executable.
        if (!pckHeaderFound)
        {
            // Loading with offset feature not supported for self contained exe files.
            if (offset != 0)
            {
                ERR_FAIL_V_MSG(false, "Loading self-contained executable with offset not supported.");
            }

            file.Seek(-4, SeekOrigin.End);

            magic = reader.ReadUInt32();

            if (magic == PACK_HEADER_MAGIC)
            {
                file.Seek(file.Position - 12, SeekOrigin.Begin);

                var ds = reader.ReadUInt64();

                file.Seek(file.Position - (long)ds - 8, SeekOrigin.Begin);

                magic = reader.ReadUInt32();
                if (magic == PACK_HEADER_MAGIC)
                {
                    #if ENABLED
                    PrintVerbose("PCK header found at the end of executable, loading from offset 0x" + String::num_int64(file.Position - 4, 16));
                    #endif

                    pckHeaderFound = true;
                }
            }
        }

        if (!pckHeaderFound)
        {
            return false;
        }

        var version  = reader.ReadUInt32();
        var verMajor = reader.ReadUInt32();
        var verMinor = reader.ReadUInt32();

        reader.ReadUInt32();

        if (ERR_FAIL_COND_V_MSG(version != PACK_FORMAT_VERSION, $"Pack version unsupported: {version}."))
        {
            return false;
        }

        if (ERR_FAIL_COND_V_MSG(verMajor > GodotVersion.VERSION_MAJOR || verMajor == GodotVersion.VERSION_MAJOR && verMinor > GodotVersion.VERSION_MINOR, $"Pack created with a newer version of the engine: {verMajor}.{verMinor}."))
        {
            return false;
        }

        var packFlags = (PackFlags)reader.ReadUInt32();
        var fileBase  = reader.ReadUInt64();

        var encDirectory = packFlags.HasFlag(PackFlags.PACK_DIR_ENCRYPTED);

        for (var i = 0; i < 16; i++)
        {
            //reserved
            reader.ReadUInt32();
        }

        var file_count = reader.ReadUInt32();

        if (encDirectory)
        {
            #region TODO
            // Ref<FileAccessEncrypted> fae;
            // fae.instantiate();
            // ERR_FAIL_COND_V_MSG(fae.is_null(), false, "Can't open encrypted pack directory.");

            // Vector<uint8_t> key;
            // key.resize(32);
            // for (int i = 0; i < key.size(); i++)
            // {
            //     key.write[i] = script_encryption_key[i];
            // }

            // Error err = fae->open_and_parse(file, key, FileAccessEncrypted::MODE_READ, false);
            // ERR_FAIL_COND_V_MSG(err, false, "Can't open encrypted pack directory.");
            // f = fae;
            #endregion TODO
        }

        for (var i = 0; i < file_count; i++)
        {
            var sl = (int)reader.ReadUInt32();
            var cs = new byte[sl + 1];

            file.Read(cs, 0, cs.Length);

            cs[sl] = 0;

            var storedPath = cs.ConvertToString();

            var ofs  = fileBase + reader.ReadUInt64();
            var size = reader.ReadUInt64();
            var md5  = new byte[16];
            file.Read(md5, 0, md5.Length);

            var flags = reader.ReadUInt32();

            PackedData.Singleton.AddPath(path, storedPath, ofs + offset, size, md5, this, replaceFiles, ((PackFileFlags)flags).HasFlag(PackFileFlags.PACK_FILE_ENCRYPTED));
        }

        return true;
    }
}
