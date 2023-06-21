namespace Godot.Net.Servers.Rendering;

public partial class ShaderLanguage
{
    public partial record ShaderNode
    {
        public partial record Uniform
        {
            public enum HintKind
            {
                HINT_NONE,
                HINT_RANGE,
                HINT_SOURCE_COLOR,
                HINT_NORMAL,
                HINT_ROUGHNESS_NORMAL,
                HINT_ROUGHNESS_R,
                HINT_ROUGHNESS_G,
                HINT_ROUGHNESS_B,
                HINT_ROUGHNESS_A,
                HINT_ROUGHNESS_GRAY,
                HINT_DEFAULT_BLACK,
                HINT_DEFAULT_WHITE,
                HINT_DEFAULT_TRANSPARENT,
                HINT_ANISOTROPY,
                HINT_SCREEN_TEXTURE,
                HINT_NORMAL_ROUGHNESS_TEXTURE,
                HINT_DEPTH_TEXTURE,
                HINT_MAX
            }
        }
    }
}
