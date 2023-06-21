namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public partial record FunctionNode
    {
        public record Argument
        {
            public required string   Name  { get; init; }
            public required int      Index { get; init; }
            public required DataType Type  { get; init; }

            public uint                             ArraySize          { get; set; }
            public bool                             IsConst            { get; set; }
            public DataPrecision                    Precision          { get; set; }
            public ArgumentQualifier                Qualifier          { get; set; }
            public bool                             TexArgumentCheck   { get; set; }
            public Dictionary<string, HashSet<int>> TexArgumentConnect { get; init; } = new();
            public TextureFilter                    TexArgumentFilter  { get; set; }
            public TextureRepeat                    TexArgumentRepeat  { get; set; }
            public string                           TexBuiltin         { get; set; } = "";
            public bool                             TexBuiltinCheck    { get; set; }
            public string                           TypeStr            { get; set; } = "";
        }
    }
}
