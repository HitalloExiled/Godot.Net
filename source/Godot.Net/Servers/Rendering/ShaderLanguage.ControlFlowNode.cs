namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public record ControlFlowNode : Node
    {
        public override NodeType Type => NodeType.TYPE_CONTROL_FLOW;

        public List<BlockNode> Blocks      { get; set; } = new();
        public List<Node>      Expressions { get; set; } = new();
        public FlowOperation   FlowOp      { get; set; } = FlowOperation.FLOW_OP_IF;
    }
}
