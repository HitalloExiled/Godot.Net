namespace Godot.Net.Servers.Rendering.RendererRD;

using Godot.Net.Servers.Rendering.Environment;
using Godot.Net.Servers.Rendering.Storage;
using RD = RenderingDevice;

public partial class RendererCompositorRD : RendererCompositor
{
    private Blit blit;

    public override RendererCanvasRender     Canvas           => throw new NotImplementedException();
    public override RendererFog              Fog              => throw new NotImplementedException();
    public override ulong                    FrameNumber      => throw new NotImplementedException();
    public override RendererGI               Gi               => throw new NotImplementedException();
    public override RendererLightStorage     LightStorage     => throw new NotImplementedException();
    public override RendererMaterialStorage  MaterialStorage  => throw new NotImplementedException();
    public override RendererMeshStorage      MeshStorage      => throw new NotImplementedException();
    public override RendererParticlesStorage ParticlesStorage => throw new NotImplementedException();
    public override RendererSceneRender      Scene            => throw new NotImplementedException();
    public override RendererTextureStorage   TextureStorage   => throw new NotImplementedException();
    public override double                   TotalTime        => throw new NotImplementedException();
    public override RendererUtilities        Utilities        => throw new NotImplementedException();

    public static void MakeCurrent() => throw new NotImplementedException();

    public override void BlitRenderTargetsToScreen(int screen, IList<BlitToScreen> renderTargets, int ammount) => throw new NotImplementedException();

    public override void BeginFrame(double frameStep) => throw new NotImplementedException();
    public override void EndFrame(bool swapBuffers) => throw new NotImplementedException();
    public override void EndFrame(double frameStep) => throw new NotImplementedException();
    public override void Initialize()
    {
        var blitModes = new List<string>
        {
            "\n",
            "\n#define USE_LAYER\n",
            "\n#define USE_LAYER\n#define APPLY_LENS_DISTORTION\n",
            "\n"
        };

        this.blit.Shader.Initialize(blitModes);

        this.blit.ShaderVersion = this.blit.Shader.VersionCreate();

        for (var i = 0; i < (int)BlitMode.BLIT_MODE_MAX; i++)
        {
            this.blit.Pipelines[i] = RD.Singleton.RenderPipelineCreate(
                this.blit.Shader.VersionGetShader(this.blit.ShaderVersion, i),
                RD.Singleton.ScreenGetFramebufferFormat(),
                RD.INVALID_ID,
                RD.RenderPrimitive.RENDER_PRIMITIVE_TRIANGLES,
                new RD.PipelineRasterizationState(),
                new RD.PipelineMultisampleState(),
                new RD.PipelineDepthStencilState(),
                i == (int)BlitMode.BLIT_MODE_NORMAL_ALPHA
                    ? RD.PipelineColorBlendState.CreateBlend()
                    : RD.PipelineColorBlendState.CreateDisabled(),
                0
            );
        }

        //create index array for copy shader
        var pv = new byte[6 * 4];

        pv[0] = 0;
        pv[1] = 1;
        pv[2] = 2;
        pv[3] = 0;
        pv[4] = 2;
        pv[5] = 3;

        this.blit.IndexBuffer = RD.Singleton.IndexBufferCreate(6, RD.IndexBufferFormat.INDEX_BUFFER_FORMAT_UINT32, pv);
        this.blit.Array       = RD.Singleton.IndexArrayCreate(this.blit.IndexBuffer, 0, 6);
        this.blit.Sampler     = RD.Singleton.SamplerCreate(new RD.SamplerState());
    }
    public override void PrepareForBlittingRenderTargets() => throw new NotImplementedException();
}
