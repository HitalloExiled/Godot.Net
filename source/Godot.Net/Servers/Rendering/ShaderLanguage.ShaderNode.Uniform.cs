namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public partial record ShaderNode
    {
        public partial record Uniform
        {
            public uint                          ArraySize      { get; set; }
            public List<ConstantNode.ValueUnion> DefaultValue   { get; set; } = new();
            public TextureFilter                 Filter         { get; set; } = TextureFilter.FILTER_DEFAULT;
            public string?                       Group          { get; set; }
            public HintKind                      Hint           { get; set; } = HintKind.HINT_NONE;
            public float[]                       HintRange      { get; set; } = new float[3] { 0.0f, 1.0f, 0.001f };
            public int                           InstanceIndex  { get; set; }
            public int                           Order          { get; set; }
            public DataPrecision                 Precision      { get; set; } = DataPrecision.PRECISION_DEFAULT;
            public TextureRepeat                 Repeat         { get; set; } = TextureRepeat.REPEAT_DEFAULT;
            public ScopeKind                     Scope          { get; set; } = ScopeKind.SCOPE_LOCAL;
            public string?                       Subgroup       { get; set; }
            public uint                          TextureBinding { get; set; }
            public int                           TextureOrder   { get; set; }
            public DataType                      Type           { get; set; } = DataType.TYPE_VOID;
            public bool                          UseColor       { get; set; }
        }
    }
}
