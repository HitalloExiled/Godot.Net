namespace Godot.Net.Core.IO;

public class ZipArchive : PackSource
{
    public override FileAccess GetFile(string path, PackedData.PackedFile file) => throw new NotImplementedException();
    public override bool TryOpenPack(string path, bool replaceFiles, ulong offset)
    {
        // load with offset feature only supported for PCK files
        if (ERR_FAIL_COND_V_MSG(offset != 0, "Invalid PCK data. Note that loading files with a non-zero offset isn't supported with ZIP archives."))
        {
            return false;
        }

        if (Path.GetExtension(path).ToLower() is not "zip" and not "pcz")
        {
            return false;
        }

        #region TODO
        // zlib_filefunc_def io;
        // memset(&io, 0, sizeof(io));

        // io.opaque = nullptr;
        // io.zopen_file = godot_open;
        // io.zread_file = godot_read;
        // io.zwrite_file = godot_write;

        // io.ztell_file = godot_tell;
        // io.zseek_file = godot_seek;
        // io.zclose_file = godot_close;
        // io.zerror_file = godot_testerror;

        // unzFile zfile = unzOpen2(p_path.utf8().get_data(), &io);
        // ERR_FAIL_COND_V(!zfile, false);

        // unz_global_info64 gi;
        // int err = unzGetGlobalInfo64(zfile, &gi);
        // ERR_FAIL_COND_V(err != UNZ_OK, false);

        // Package pkg;
        // pkg.filename = p_path;
        // pkg.zfile = zfile;
        // packages.push_back(pkg);
        // int pkg_num = packages.size() - 1;

        // for (uint64_t i = 0; i < gi.number_entry; i++) {
        //     char filename_inzip[256];

        //     unz_file_info64 file_info;
        //     err = unzGetCurrentFileInfo64(zfile, &file_info, filename_inzip, sizeof(filename_inzip), nullptr, 0, nullptr, 0);
        //     ERR_CONTINUE(err != UNZ_OK);

        //     File f;
        //     f.package = pkg_num;
        //     unzGetFilePos(zfile, &f.file_pos);

        //     String fname = String("res://") + String::utf8(filename_inzip);
        //     files[fname] = f;

        //     uint8_t md5[16] = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        //     PackedData::get_singleton()->add_path(p_path, fname, 1, 0, md5, this, p_replace_files, false);
        //     //printf("packed data add path %s, %s\n", p_name.utf8().get_data(), fname.utf8().get_data());

        //     if ((i + 1) < gi.number_entry) {
        //         unzGoToNextFile(zfile);
        //     }
        // }
        #endregion TODO

        return true;
    }
}
