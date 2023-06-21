namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public record StageFunctionInfo
    {
        public record Argument(string? Name = default, DataType Type = DataType.TYPE_VOID);

        public Argument[] Arguments  { get; }
        public DataType   ReturnType { get; set; } = DataType.TYPE_VOID;

        public StageFunctionInfo(DataType returnType = DataType.TYPE_VOID, params Argument[] arguments)
        {
            this.ReturnType = returnType;
            this.Arguments  = arguments;
        }
    }
}
