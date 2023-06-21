namespace Godot.Net.Core.IO;

public partial class PackedData
{
    public record PackedFile
    {
		public byte[] Md5 { get; } = new byte[16];

		public bool        Encrypted { get; set; }
		public ulong       Offset    { get; set; } //if offset is ZERO, the file was ERASED
		public string      Pack      { get; set; } = "";
		public ulong       Size      { get; set; }
		public PackSource? Src       { get; set; }
	};
}
