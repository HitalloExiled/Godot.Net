#define GLES_OVER_GL

namespace Godot.Net.Drivers.GLES3;
using Godot.Net.Servers.Rendering;

#pragma warning disable IDE0052, CA1822, CS0414, IDE0044 // TODO REMOVE

public partial class RasterizerSceneGLES3
{
    public partial record SceneState
    {
        public unsafe struct MultiviewUBO
        {
            public fixed float EyeOffset[RendererSceneRender.MAX_RENDER_VIEWS * 4];
            public fixed float InvProjectionMatrixView[RendererSceneRender.MAX_RENDER_VIEWS * 16];
            public fixed float ProjectionMatrixView[RendererSceneRender.MAX_RENDER_VIEWS * 16];
        }
    }

}
