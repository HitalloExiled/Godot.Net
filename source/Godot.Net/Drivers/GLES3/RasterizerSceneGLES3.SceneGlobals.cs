#define GLES_OVER_GL

namespace Godot.Net.Drivers.GLES3;
#pragma warning disable IDE0052, CA1822, CS0414, IDE0044 // TODO REMOVE

public partial class RasterizerSceneGLES3
{
    public record SceneGlobals
    {
        public Guid CubemapFilterShaderVersion { get; set; }
        public Guid DefaultMaterial            { get; set; }
        public Guid DefaultShader              { get; set; }
        public Guid ShaderDefaultVersion       { get; set; }
    }

}
