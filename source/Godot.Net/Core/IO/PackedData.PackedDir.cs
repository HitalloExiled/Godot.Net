namespace Godot.Net.Core.IO;

public partial class PackedData
{
    private record PackedDir
    {
		public HashSet<string>               Files   { get; } = new();
		public Dictionary<string, PackedDir> Subdirs { get; } = new();

		public string     Name   { get; set; } = "";
		public PackedDir? Parent { get; set; }
	}
}
