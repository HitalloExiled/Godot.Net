#define GLES_OVER_GL

namespace Godot.Net.Drivers.GLES3;
#pragma warning disable IDE0052, CA1822, CS0414, IDE0044 // TODO REMOVE

public partial class RasterizerSceneGLES3
{
    public partial record SceneState
    {
        public struct TonemapUBO
        {
            public float Exposure   { get; set; } = 1.0f;
            public int   Pad        { get; set; }
            public int   Tonemapper { get; set; }
            public float White      { get; set; } = 1.0f;

            public TonemapUBO()
            { }
        }
    }

}
