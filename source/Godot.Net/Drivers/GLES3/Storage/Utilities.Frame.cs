namespace Godot.Net.Drivers.GLES3.Storage;

public partial class Utilities
{
    public record Frame
    {
        public long     Index                    { get; set; }
        public uint[]   Queries                  { get; set; } = new uint[MAX_QUERIES];
        public int      TimestampCount           { get; set; }
        public long[]   TimestampCpuResultValues { get; set; } = new long[MAX_QUERIES];
        public long[]   TimestampCpuValues       { get; set; } = new long[MAX_QUERIES];
        public string[] TimestampNames           { get; set; } = new string[MAX_QUERIES];
        public int      TimestampResultCount     { get; set; }
        public string[] TimestampResultNames     { get; set; } = new string[MAX_QUERIES];
        public long[]   TimestampResultValues    { get; set; } = new long[MAX_QUERIES];
    }
}
