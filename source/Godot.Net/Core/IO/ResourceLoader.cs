namespace Godot.Net.Core.IO;

using Error = Error.Error;

#pragma warning disable IDE0060 // TODO - Remove

public class ResourceLoader
{
    public static void GetRecognizedExtensionsForType(string type, List<string> extensions)
    {
        #region TODO
        // for (int i = 0; i < loader_count; i++) {
        //     loader[i]->get_recognized_extensions_for_type(p_type, p_extensions);
        // }
        #endregion TODO
    }

    public static Resource? Load(string path, string typeHint = "", ResourceFormatLoader.CacheMode cacheMode = ResourceFormatLoader.CacheMode.CACHE_MODE_REUSE) =>
        Load(path, typeHint, cacheMode, out var _);

    public static Resource? Load(string path, string typeHint, ResourceFormatLoader.CacheMode cacheMode, out Error error)
    {
        error = Error.FAILED;

        // TODO
        return null;
    }
}
