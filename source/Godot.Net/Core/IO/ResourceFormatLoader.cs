namespace Godot.Net.Core.IO;

public class ResourceFormatLoader
{
    public enum CacheMode
    {
		CACHE_MODE_IGNORE, // Resource and subresources do not use path cache, no path is set into resource.
		CACHE_MODE_REUSE, // Resource and subresources use patch cache, reuse existing loaded resources instead of loading from disk when available.
		CACHE_MODE_REPLACE, // Resource and subresource use path cache, but replace existing loaded resources when available with information from disk.
	}
}
