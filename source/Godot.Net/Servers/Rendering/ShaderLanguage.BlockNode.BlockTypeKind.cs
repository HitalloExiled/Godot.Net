namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public partial record BlockNode
    {
        public enum BlockTypeKind
        {
            BLOCK_TYPE_STANDARD,
            BLOCK_TYPE_FOR_INIT,
            BLOCK_TYPE_FOR_CONDITION,
            BLOCK_TYPE_FOR_EXPRESSION,
            BLOCK_TYPE_SWITCH,
            BLOCK_TYPE_CASE,
            BLOCK_TYPE_DEFAULT,
        };
    }
}
