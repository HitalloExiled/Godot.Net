#define TOOLS_ENABLED

namespace Godot.Net.Core.IO;

using Godot.Net.Core.Object;

#pragma warning disable IDE0052, IDE0051 // TODO Remove

public partial class Resource : GodotObject
{
    public class ResourceCache
    {
        #if TOOLS_ENABLED
        private static readonly Dictionary<string, Dictionary<string, string>> resourcePathCache = new(); // Each tscn has a set of resource paths and IDs.
        // private static RWLock path_cache_lock;
        #endif

        public static Dictionary<string, Resource> Resources { get; } = new();

        #if TOOLS_ENABLED
        public static Mutex Lock { get; } = new();
        #endif

        private static void Clear() => throw new NotImplementedException();

        public static int GetCachedResourceCount() => throw new NotImplementedException();
        public static void GetCachedResources(List<Resource> resources) => throw new NotImplementedException();

        public static Resource? GetRef(string path)
        {
            Lock.WaitOne();

            if (Resources.TryGetValue(path, out var res))
            {
                Resources.Remove(path);
            }

            Lock.ReleaseMutex();

            return res;
        }

        public static bool Has(string path) => throw new NotImplementedException();

        private void RegisterCoreTypes() => throw new NotImplementedException();
        private void UnregisterCoreTypes() => throw new NotImplementedException();
    }
}
