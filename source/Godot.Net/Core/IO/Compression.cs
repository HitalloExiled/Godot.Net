namespace Godot.Net.Core.IO;

public partial class Compression
{
    private const int Z_DEFAULT_COMPRESSION = -1;

    public static int  GzipChunk                { get; set; } = 16384;
    public static int  GzipLevel                { get; set; } = Z_DEFAULT_COMPRESSION;
    public static int  ZlibLevel                { get; set; } = Z_DEFAULT_COMPRESSION;
    public static int  ZstdLevel                { get; set; } = 3;
    public static bool ZstdLongDistanceMatching { get; set; }
    public static int  ZstdWindowLogSize        { get; set; } = 27; // ZSTD_WINDOWLOG_LIMIT_DEFAULT

    public static int Compress(byte[] dst, byte[] src, int srcSize, Mode mode = Mode.MODE_ZSTD) => throw new NotImplementedException();
	public static int Decompress(byte[] dst, int dstMaxSize, byte[] src, int srcSize, Mode mode = Mode.MODE_ZSTD) => throw new NotImplementedException();
	public static int DecompressDynamic(byte[] dst_vect, int maxDstSize, byte[] src, int srcSize, Mode mode) => throw new NotImplementedException();
	public static int GetMaxCompressedBufferSize(int srcSize, Mode mode = Mode.MODE_ZSTD) => throw new NotImplementedException();
}
