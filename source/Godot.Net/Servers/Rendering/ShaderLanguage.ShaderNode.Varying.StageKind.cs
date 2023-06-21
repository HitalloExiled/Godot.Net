namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public partial record ShaderNode
    {
        public partial record Varying
        {
            public enum StageKind
            {
                STAGE_UNKNOWN,
                STAGE_VERTEX, // transition stage to STAGE_VERTEX_TO_FRAGMENT_LIGHT, emits warning if it's not used
                STAGE_FRAGMENT, // transition stage to STAGE_FRAGMENT_TO_LIGHT, emits warning if it's not used
                STAGE_VERTEX_TO_FRAGMENT_LIGHT,
                STAGE_FRAGMENT_TO_LIGHT,
            }
        }
    }
}
