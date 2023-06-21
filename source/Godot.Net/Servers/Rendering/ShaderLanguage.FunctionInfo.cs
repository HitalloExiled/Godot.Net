namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public record FunctionInfo
    {
        public Dictionary<string, BuiltInInfo>       BuiltIns        { get; init; } = new();
        public bool                                  CanDiscard      { get; set; }
        public bool                                  MainFunction    { get; set; }
        public Dictionary<string, StageFunctionInfo> StageFunctions  { get; init; } = new();
    }
}
