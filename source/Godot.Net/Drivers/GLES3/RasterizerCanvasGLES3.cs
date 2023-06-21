namespace Godot.Net.Drivers.GLES3;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Godot.Net.Core;
using Godot.Net.Core.Enums;
using Godot.Net.Core.Math;
using Godot.Net.Drivers.GLES3.Api.Enums;
using Godot.Net.Drivers.GLES3.Storage;
using Godot.Net.Servers.Rendering;

public partial class RasterizerCanvasGLES3 : RendererCanvasRender
{
    private readonly ShadowRender shadowRender = new();

    private const int BASE_UNIFORM_LOCATION         = 0;
    private const int DEFAULT_MAX_LIGHTS_PER_RENDER = 256;
    private const int FLOAT_SIZE                    = sizeof(float);
    private const int GLOBAL_UNIFORM_LOCATION       = 1;
    private const int INSTANCE_UNIFORM_LOCATION     = 3;
    private const int LIGHT_FLAGS_BLEND_MASK        = 3 << 16;
    private const int LIGHT_FLAGS_BLEND_MODE_ADD    = 0 << 16;
    private const int LIGHT_FLAGS_BLEND_MODE_MASK   = 3 << 16;
    private const int LIGHT_FLAGS_BLEND_MODE_MIX    = 2 << 16;
    private const int LIGHT_FLAGS_BLEND_MODE_SUB    = 1 << 16;
    private const int LIGHT_FLAGS_BLEND_SHIFT       = 16;
    private const int LIGHT_FLAGS_FILTER_SHIFT      = 2;
    private const int LIGHT_FLAGS_HAS_SHADOW        = 1 << 20;
    private const int LIGHT_FLAGS_TEXTURE_MASK      = 0xFFFF;
    private const int LIGHT_UNIFORM_LOCATION        = 2;
    private const int MATERIAL_UNIFORM_LOCATION     = 4;
    private const int MAX_LIGHT_TEXTURES            = 1024;
    private const int MAX_LIGHTS_PER_ITEM           = 16;
    private const int MAX_RENDER_ITEMS              = 256 * 1024;
    private const int USHORT_SIZE                   = sizeof(ushort);

    private static readonly PrimitiveType[] prim =
    {
        PrimitiveType.Points,
        PrimitiveType.Lines,
        PrimitiveType.LineStrip,
        PrimitiveType.Triangles,
        PrimitiveType.TriangleStrip,
    };

    private readonly GuidOwner<CanvasLight> canvasLightOwner = new();
    private readonly Data                   data             = new();
    private readonly Guid                   defaultCanvasGroupMaterial;
    private readonly Guid                   defaultCanvasGroupShader;
    private readonly Guid                   defaultCanvasTexture;
    private readonly Guid                   defaultClipChildrenMaterial;
    private readonly Guid                   defaultClipChildrenShader;
    private readonly Item[]                 items            = new Item[MAX_RENDER_ITEMS];
    private readonly PolygonBuffersData     polygonBuffers   = new();
    private readonly State                  state            = new();

    public override double Time
    {
        get => this.state.Time;
        set => this.state.Time = value;
    }

    public RasterizerCanvasGLES3()
    {
        //     singleton = this;
        var textureStorage  = TextureStorage.Singleton;
        var materialStorage = MaterialStorage.Singleton;
        var config          = Config.Singleton;
        var gl              = GL.Singleton;

        this.polygonBuffers.LastId = 1;
        // quad buffer
        gl.GenBuffers(out var canvasQuadVertices);
        this.data.CanvasQuadVertices = canvasQuadVertices;

        gl.BindBuffer(BufferTargetARB.ArrayBuffer, this.data.CanvasQuadVertices);

        var qv8 = new float[8]
        {
            0f, 0f,
            0f, 1f,
            1f, 1f,
            1f, 0f
        };

        gl.BufferData(BufferTargetARB.ArrayBuffer, qv8, BufferUsageARB.StaticDraw);

        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

        gl.GenVertexArrays(out var canvasQuadArray);
        this.data.CanvasQuadArray = canvasQuadArray;

        gl.BindVertexArray(this.data.CanvasQuadArray);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, this.data.CanvasQuadVertices);
        gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, FLOAT_SIZE * 2, default);
        gl.EnableVertexAttribArray(0);
        gl.BindVertexArray(0);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0); //unbind

        //particle quad buffers

        gl.GenBuffers(out var particleQuadVertices);
        this.data.ParticleQuadVertices = particleQuadVertices;

        gl.BindBuffer(BufferTargetARB.ArrayBuffer, this.data.ParticleQuadVertices);
        //quad of size 1, with pivot on the center for particles, then regular UVS. Color is general plus fetched from particle
        var qv16 = new float[16]
        {
            -0.5f, -0.5f,
             0.0f,  0.0f,
            -0.5f,  0.5f,
             0.0f,  1.0f,
             0.5f,  0.5f,
             1.0f,  1.0f,
             0.5f, -0.5f,
             1.0f,  0.0f
        };

        gl.BufferData(BufferTargetARB.ArrayBuffer, qv16, BufferUsageARB.StaticDraw);

        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0); //unbind

        gl.GenVertexArrays(out var particleQuadArray);
        this.data.ParticleQuadArray = particleQuadArray;

        gl.BindVertexArray(this.data.ParticleQuadArray);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, this.data.ParticleQuadVertices);
        gl.EnableVertexAttribArray((uint)RS.ArrayType.ARRAY_VERTEX);
        gl.VertexAttribPointer((uint)RS.ArrayType.ARRAY_VERTEX, 2, VertexAttribPointerType.Float, false, FLOAT_SIZE * 4, default);
        gl.EnableVertexAttribArray((uint)RS.ArrayType.ARRAY_TEX_UV);
        gl.VertexAttribPointer((uint)RS.ArrayType.ARRAY_TEX_UV, 2, VertexAttribPointerType.Float, false, FLOAT_SIZE * 4, new(8));
        gl.BindVertexArray(0);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0); //unbind

        // ninepatch buffers
        // array buffer
        gl.GenBuffers(out var ninepatchVertices);
        this.data.NinepatchVertices = ninepatchVertices;

        gl.BindBuffer(BufferTargetARB.ArrayBuffer, this.data.NinepatchVertices);

        gl.BufferData(BufferTargetARB.ArrayBuffer, FLOAT_SIZE * (16 + 16) * 2, default, BufferUsageARB.DynamicDraw);

        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);

        // element buffer
        gl.GenBuffers(out var ninepatchElements);
        this.data.NinepatchElements = ninepatchElements;

        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, this.data.NinepatchElements);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        static byte eidx(byte y, byte x) => (byte)(y * 4 + x);

        var elems = new byte[3 * 2 * 9]
        {
            // first row

            eidx(0, 0), eidx(0, 1), eidx(1, 1),
            eidx(1, 1), eidx(1, 0), eidx(0, 0),

            eidx(0, 1), eidx(0, 2), eidx(1, 2),
            eidx(1, 2), eidx(1, 1), eidx(0, 1),

            eidx(0, 2), eidx(0, 3), eidx(1, 3),
            eidx(1, 3), eidx(1, 2), eidx(0, 2),

            // second row

            eidx(1, 0), eidx(1, 1), eidx(2, 1),
            eidx(2, 1), eidx(2, 0), eidx(1, 0),

            // the center one would be here, but we'll put it at the end
            // so it's easier to disable the center and be able to use
            // one draw call for both

            eidx(1, 2), eidx(1, 3), eidx(2, 3),
            eidx(2, 3), eidx(2, 2), eidx(1, 2),

            // third row

            eidx(2, 0), eidx(2, 1), eidx(3, 1),
            eidx(3, 1), eidx(3, 0), eidx(2, 0),

            eidx(2, 1), eidx(2, 2), eidx(3, 2),
            eidx(3, 2), eidx(3, 1), eidx(2, 1),

            eidx(2, 2), eidx(2, 3), eidx(3, 3),
            eidx(3, 3), eidx(3, 2), eidx(2, 2),

            // center field

            eidx(1, 1), eidx(1, 2), eidx(2, 2),
            eidx(2, 2), eidx(2, 1), eidx(1, 1)
        };

        gl.BufferData(BufferTargetARB.ElementArrayBuffer, elems, BufferUsageARB.StaticDraw);

        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

        var uniformMaxSize = config.MaxUniformBufferSize;
        this.data.MaxLightsPerRender = uniformMaxSize < 65536 ? 64 : (uint)256;

        // Reserve 3 Uniform Buffers for instance data Frame N, N+1 and N+2
        this.data.MaxInstancesPerBuffer = (uint)GLOBAL_GET<int>("rendering/gl_compatibility/item_buffer_size");
        this.data.MaxInstanceBufferSize = (uint)(this.data.MaxInstancesPerBuffer * Marshal.SizeOf<InstanceData>()); // 16,384 instances * 128 bytes = 2,097,152 bytes = 2,048 kb

        this.state.CanvasInstanceDataBuffers.Capacity = 3;
        this.state.CanvasInstanceBatches.Capacity     = 200;

        for (var i = 0; i < 3; i++)
        {
            var newBuffers = new uint[3];

            gl.GenBuffers(newBuffers);
            // Batch UBO.
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, newBuffers[0]);
            gl.BufferData(BufferTargetARB.ArrayBuffer, this.data.MaxInstanceBufferSize, default, BufferUsageARB.StreamDraw);
            // Light uniform buffer.
            gl.BindBuffer(BufferTargetARB.UniformBuffer, newBuffers[1]);
            gl.BufferData(BufferTargetARB.UniformBuffer, (uint)(Marshal.SizeOf<LightUniform>() * this.data.MaxLightsPerRender), default, BufferUsageARB.StreamDraw);
            // State buffer.
            gl.BindBuffer(BufferTargetARB.UniformBuffer, newBuffers[2]);
            gl.BufferData(BufferTargetARB.UniformBuffer, Marshal.SizeOf<StateBuffer>(), default, BufferUsageARB.StreamDraw);
            var db = new DataBuffer
            {
                InstanceBuffers = { newBuffers[0] },
                LightUbo        = newBuffers[1],
                StateUbo        = newBuffers[2],
                LastFrameUsed   = 0,
                Fence           = gl.FenceSync(SyncCondition.SyncGpuCommandsComplete, 0),
            };

            this.state.CanvasInstanceDataBuffers.Add(db);
        }

        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        gl.BindBuffer(BufferTargetARB.UniformBuffer, 0);

        this.state.InstanceDataArray = new InstanceData[this.data.MaxInstancesPerBuffer];
        this.state.LightUniforms     = new LightUniform[this.data.MaxLightsPerRender];

        var indices = new uint[6] { 0, 2, 1, 3, 2, 0 };

        gl.GenVertexArrays(out var indexedQuadArray);
        this.data.IndexedQuadArray = indexedQuadArray;

        gl.BindVertexArray(this.data.IndexedQuadArray);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, this.data.CanvasQuadVertices);

        gl.GenBuffers(out var indexedQuadBuffer);
        this.data.IndexedQuadBuffer = indexedQuadBuffer;

        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, this.data.IndexedQuadBuffer);
        gl.BufferData(BufferTargetARB.ElementArrayBuffer, indices, BufferUsageARB.StaticDraw);
        gl.BindVertexArray(0);

        var globalDefines = // TODO: this MAX_GLOBAL_SHADER_UNIFORMS is arbitrary for now
            $"""
            #define MAX_GLOBAL_SHADER_UNIFORMS 256
            #define MAX_LIGHTS                 {this.data.MaxLightsPerRender}

            """;

        MaterialStorage.Singleton.Shaders.CanvasShader.Initialize(globalDefines, 1);
        this.data.CanvasShaderDefaultVersion = MaterialStorage.Singleton.Shaders.CanvasShader.VersionCreate();

        this.shadowRender.Shader.Initialize();
        this.shadowRender.ShaderVersion = this.shadowRender.Shader.VersionCreate();

        this.defaultCanvasGroupShader = Guid.NewGuid();
        materialStorage.ShaderInitialize(this.defaultCanvasGroupShader);

        materialStorage.ShaderSetCode(
            this.defaultCanvasGroupShader,
            """
            // Default CanvasGroup shader.

            shader_type canvas_item;
            render_mode unshaded;

            uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

            void fragment() {
                vec4 c = textureLod(screen_texture, SCREEN_UV, 0.0);

                if (c.a > 0.0001) {
                    c.rgb /= c.a;
                }

                COLOR *= c;
            }
            """
        );

        this.defaultCanvasGroupMaterial = Guid.NewGuid();

        materialStorage.MaterialInitialize(this.defaultCanvasGroupMaterial);

        materialStorage.MaterialSetShader(this.defaultCanvasGroupMaterial, this.defaultCanvasGroupShader);

        this.defaultClipChildrenShader = Guid.NewGuid();
        materialStorage.ShaderInitialize(this.defaultClipChildrenShader);

        materialStorage.ShaderSetCode(
            this.defaultClipChildrenShader,
            """
            // Default clip children shader.

            shader_type canvas_item;
            render_mode unshaded;

            uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;

            void fragment() {
                vec4 c = textureLod(screen_texture, SCREEN_UV, 0.0);
                COLOR.rgb = c.rgb;
            }
            """
        );
        this.defaultClipChildrenMaterial = Guid.NewGuid();
        materialStorage.MaterialInitialize(this.defaultClipChildrenMaterial);

        materialStorage.MaterialSetShader(this.defaultClipChildrenMaterial, this.defaultClipChildrenShader);

        this.defaultCanvasTexture = Guid.NewGuid();
        textureStorage.CanvasTextureInitialize(this.defaultCanvasTexture);

        this.state.Time = 0.0;
    }

    private void AddToBatch(ref int index, ref bool batchBroken)
    {
        this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].InstanceCount++;
        index++;
        if (index >= this.data.MaxInstancesPerBuffer)
        {
            var gl = GL.Singleton;
            // Copy over all data needed for rendering right away
            // then go back to recording item commands.
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].InstanceBuffers[this.state.CurrentInstanceBufferIndex]);
            #if WEB_ENABLED
            gl.BufferSubData(BufferTargetARB.ArrayBuffer, this.state.LastItemIndex * Marshal.SizeOf<InstanceData>(), this.state.InstanceDataArray);
            #else
            // On Desktop and mobile we map the memory without synchronizing for maximum speed.
            var buffer = gl.MapBufferRange(BufferTargetARB.ArrayBuffer, this.state.LastItemIndex * Marshal.SizeOf<InstanceData>(), index * Marshal.SizeOf<InstanceData>(), MapBufferAccessMask.MapWriteBit | MapBufferAccessMask.MapUnsynchronizedBit);

            UnmanagedUtils.Copy(this.state.InstanceDataArray, buffer, index);

            gl.UnmapBuffer(BufferTargetARB.ArrayBuffer);
            #endif
            this.AllocateInstanceBuffer();

            index = 0;

            this.state.LastItemIndex = 0;

            batchBroken = false; // Force a new batch to be created

            this.NewBatch(ref batchBroken);

            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Start = 0;
        }
    }

    private void AllocateInstanceBuffer()
    {
        var gl = GL.Singleton;

        this.state.CurrentInstanceBufferIndex++;

        if (this.state.CurrentInstanceBufferIndex < this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].InstanceBuffers.Count)
        {
            // We already allocated another buffer in a previous frame, so we can just use it.
            return;
        }

        gl.GenBuffers(out var newBuffer);

        gl.BindBuffer(BufferTargetARB.ArrayBuffer, newBuffer);
        gl.BufferData(BufferTargetARB.ArrayBuffer, this.data.MaxInstanceBufferSize, default, BufferUsageARB.StreamDraw);

        this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].InstanceBuffers.Add(newBuffer);

        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
    }

    private static void ResetCanvas()
    {
        var gl = GL.Singleton;

        gl.Disable(EnableCap.CullFace);
        gl.Disable(EnableCap.DepthTest);
        gl.Disable(EnableCap.ScissorTest);
        gl.Enable(EnableCap.Blend);
        gl.BlendEquation(BlendEquationModeEXT.FuncAdd);
        gl.BlendFuncSeparate(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha, BlendingFactor.Zero, BlendingFactor.One);

        gl.ActiveTexture(TextureUnit.Texture0 + Config.Singleton.MaxTextureImageUnits - 2);
        gl.BindTexture(TextureTarget.Texture2D, 0);
        gl.ActiveTexture(TextureUnit.Texture0 + Config.Singleton.MaxTextureImageUnits - 3);
        gl.BindTexture(TextureTarget.Texture2D, 0);
        gl.ActiveTexture(TextureUnit.Texture0);

        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
    }

    private static unsafe void UpdateTransformToMat4(Transform3D<RealT> transform, float* mat4)
    {
        mat4[0]  = transform.Basis[0, 0];
        mat4[1]  = transform.Basis[1, 0];
        mat4[2]  = transform.Basis[2, 0];
        mat4[3]  = 0;
        mat4[4]  = transform.Basis[0, 1];
        mat4[5]  = transform.Basis[1, 1];
        mat4[6]  = transform.Basis[2, 1];
        mat4[7]  = 0;
        mat4[8]  = transform.Basis[0, 2];
        mat4[9]  = transform.Basis[1, 2];
        mat4[10] = transform.Basis[2, 2];
        mat4[11] = 0;
        mat4[12] = transform.Origin.X;
        mat4[13] = transform.Origin.Y;
        mat4[14] = transform.Origin.Z;
        mat4[15] = 1;
    }

    private static unsafe void UpdateTransform2DToMat4(Transform2D<RealT> transform, float* mat4)
    {
        mat4[0]  = transform[0, 0];
        mat4[1]  = transform[0, 1];
        mat4[2]  = 0;
        mat4[3]  = 0;
        mat4[4]  = transform[1, 0];
        mat4[5]  = transform[1, 1];
        mat4[6]  = 0;
        mat4[7]  = 0;
        mat4[8]  = 0;
        mat4[9]  = 0;
        mat4[10] = 1;
        mat4[11] = 0;
        mat4[12] = transform[2, 0];
        mat4[13] = transform[2, 1];
        mat4[14] = 0;
        mat4[15] = 1;
    }

    private void AllocateInstanceDataBuffer() => throw new NotImplementedException();

    private void BindCanvasTexture(Guid textureId, RS.CanvasItemTextureFilter baseFilter, RS.CanvasItemTextureRepeat baseRepeat)
    {
        var textureStorage = TextureStorage.Singleton;
        var config         = Config.Singleton;

        if (textureId == default)
        {
            textureId = this.defaultCanvasTexture;
        }

        if (this.state.CurrentTex == textureId && this.state.CurrentFilterMode == baseFilter && this.state.CurrentRepeatMode == baseRepeat)
        {
            return;
        }

        this.state.CurrentTex        = textureId;
        this.state.CurrentFilterMode = baseFilter;
        this.state.CurrentRepeatMode = baseRepeat;

        var t = textureStorage.GetTexture(textureId);

        CanvasTexture? ct;
        if (t != null)
        {
            if (ERR_FAIL_COND(t.CanvasTexture == null))
            {
                return;
            }

            ct = t.CanvasTexture;
            if (t.RenderTarget != null)
            {
                t.RenderTarget.UsedInFrame = true;
            }
        }
        else
        {
            ct = textureStorage.GetCanvasTexture(textureId);
        }

        if (ct == null)
        {
            // Invalid Texture RID.
            this.BindCanvasTexture(this.defaultCanvasTexture, baseFilter, baseRepeat);
            return;
        }

        var filter = ct.TextureFilter != RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_DEFAULT ? ct.TextureFilter : baseFilter;
        if (ERR_FAIL_COND(filter == RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_DEFAULT))
        {
            return;
        }

        var repeat = ct.TextureRepeat != RS.CanvasItemTextureRepeat.CANVAS_ITEM_TEXTURE_REPEAT_DEFAULT ? ct.TextureRepeat : baseRepeat;
        if (ERR_FAIL_COND(repeat == RS.CanvasItemTextureRepeat.CANVAS_ITEM_TEXTURE_REPEAT_DEFAULT))
        {
            return;
        }

        var texture = textureStorage.GetTexture(ct.Diffuse);

        var gl = GL.Singleton;

        if (texture == null)
        {
            var tex = textureStorage.GetTexture(textureStorage.TextureGlGetDefault(DefaultGLTexture.DEFAULT_GL_TEXTURE_WHITE))!;
            gl.ActiveTexture(TextureUnit.Texture0);
            gl.BindTexture(TextureTarget.Texture2D, tex.TexId);
        }
        else
        {
            gl.ActiveTexture(TextureUnit.Texture0);
            gl.BindTexture(TextureTarget.Texture2D, texture.TexId);

            texture.GlSetFilter(filter);
            texture.GlSetRepeat(repeat);

            if (texture.RenderTarget != null)
            {
                texture.RenderTarget.UsedInFrame = true;
            }
        }

        var normalMap = textureStorage.GetTexture(ct.NormalMap);

        if (normalMap == null)
        {
            gl.ActiveTexture(TextureUnit.Texture0 + config.MaxTextureImageUnits - 6);
            var tex = textureStorage.GetTexture(textureStorage.TextureGlGetDefault(DefaultGLTexture.DEFAULT_GL_TEXTURE_NORMAL))!;
            gl.BindTexture(TextureTarget.Texture2D, tex.TexId);
        }
        else
        {
            gl.ActiveTexture(TextureUnit.Texture0 + config.MaxTextureImageUnits - 6);
            gl.BindTexture(TextureTarget.Texture2D, normalMap.TexId);

            normalMap.GlSetFilter(filter);
            normalMap.GlSetRepeat(repeat);

            if (normalMap.RenderTarget != null)
            {
                normalMap.RenderTarget.UsedInFrame = true;
            }
        }

        var specularMap = textureStorage.GetTexture(ct.Specular);

        if (specularMap == null)
        {
            gl.ActiveTexture(TextureUnit.Texture0 + config.MaxTextureImageUnits - 7);
            var tex = textureStorage.GetTexture(textureStorage.TextureGlGetDefault(DefaultGLTexture.DEFAULT_GL_TEXTURE_WHITE))!;
            gl.BindTexture(TextureTarget.Texture2D, tex.TexId);
        }
        else
        {
            gl.ActiveTexture(TextureUnit.Texture0 + config.MaxTextureImageUnits - 7);
            gl.BindTexture(TextureTarget.Texture2D, specularMap.TexId);

            specularMap.GlSetFilter(filter);
            specularMap.GlSetRepeat(repeat);

            if (specularMap.RenderTarget != null)
            {
                specularMap.RenderTarget.UsedInFrame = true;
            }
        }
    }

    private void EnableAttributes(int start, bool primitive, uint rate = 1)
    {
        var gl    = GL.Singleton;
        var split = primitive ? 11u : 12u;

        for (var i = 6u; i < split; i++)
        {
            gl.EnableVertexAttribArray(i);
            gl.VertexAttribPointer(i, 4, VertexAttribPointerType.Float, false, Marshal.SizeOf<InstanceData>(), (int)(start + (i - 6) * 4 * FLOAT_SIZE));
            gl.VertexAttribDivisor(i, rate);
        }

        for (var i = split; i <= 13u; i++)
        {
            gl.EnableVertexAttribArray(i);
            gl.VertexAttribIPointer(i, 4, VertexAttribIType.UnsignedInt, Marshal.SizeOf<InstanceData>(), (int)(start + (i - 6) * 4 * FLOAT_SIZE));
            gl.VertexAttribDivisor(i, rate);
        }
    }

    private void PrepareCanvasTexture(
        Guid                       textureId,
        RS.CanvasItemTextureFilter baseFilter,
        RS.CanvasItemTextureRepeat baseRepeat,
        ref int                    index,
        ref Vector2<RealT>         texpixelSize
    )
    {
        var textureStorage = TextureStorage.Singleton;

        if (textureId == default)
        {
            textureId = this.defaultCanvasTexture;
        }

        CanvasTexture? ct = null;

        var t = textureStorage.GetTexture(textureId);

        if (t != null)
        {
            //regular texture
            if (t.CanvasTexture == null)
            {
                t.CanvasTexture = new CanvasTexture
                {
                    Diffuse = textureId
                };
            }

            ct = t.CanvasTexture;
        }
        else
        {
            ct = textureStorage.GetCanvasTexture(textureId);
        }

        if (ct == null)
        {
            // Invalid Texture RID.
            this.PrepareCanvasTexture(
                this.defaultCanvasTexture,
                baseFilter,
                baseRepeat,
                ref index,
                ref texpixelSize
            );
            return;
        }

        var texture = textureStorage.GetTexture(ct.Diffuse);

        Vector2<RealT> sizeCache;

        if (texture == null)
        {
            ct.Diffuse = textureStorage.TextureGlGetDefault(DefaultGLTexture.DEFAULT_GL_TEXTURE_WHITE);
            var tex = textureStorage.GetTexture(ct.Diffuse)!;

            sizeCache = new(tex.Width, tex.Height);
        }
        else
        {
            sizeCache = new(texture.Width, texture.Height);
        }

        var normalMap = textureStorage.GetTexture(ct.NormalMap);

        if (ct.SpecularColor.A < 0.999)
        {
            this.state.InstanceDataArray[index].Flags |= (int)Flags.FLAGS_DEFAULT_SPECULAR_MAP_USED;
        }
        else
        {
            this.state.InstanceDataArray[index].Flags &= ~(int)Flags.FLAGS_DEFAULT_SPECULAR_MAP_USED;
        }

        if (normalMap != null)
        {
            this.state.InstanceDataArray[index].Flags |= (int)Flags.FLAGS_DEFAULT_NORMAL_MAP_USED;
        }
        else
        {
            this.state.InstanceDataArray[index].Flags &= ~(int)Flags.FLAGS_DEFAULT_NORMAL_MAP_USED;
        }

        this.state.InstanceDataArray[index].SpecularShininess = (uint)Math.Clamp(ct.SpecularColor.A * 255.0, 0, 255) << 24;
        this.state.InstanceDataArray[index].SpecularShininess |= (uint)Math.Clamp(ct.SpecularColor.B * 255.0, 0, 255) << 16;
        this.state.InstanceDataArray[index].SpecularShininess |= (uint)Math.Clamp(ct.SpecularColor.G * 255.0, 0, 255) << 8;
        this.state.InstanceDataArray[index].SpecularShininess |= (uint)Math.Clamp(ct.SpecularColor.R * 255.0, 0, 255);

        texpixelSize.X = 1 / (float)sizeCache.X;
        texpixelSize.Y = 1 / (float)sizeCache.Y;

        unsafe
        {
            this.state.InstanceDataArray[index].ColorTexturePixelSize[0] = texpixelSize.X;
            this.state.InstanceDataArray[index].ColorTexturePixelSize[1] = texpixelSize.Y;
        }
    }

    private unsafe void RecordItemCommands(
        Item                           item,
        Guid                           renderTarget,
        in Transform2D<RealT>          canvasTransformInverse,
        Item?                          currentClip,
        CanvasShaderData.BlendModeType blendMode,
        Light                          plights,
        ref int                        index,
        ref bool                       batchBroken,
        ref bool                       sdfUsed
    )
    {
        var textureFilter = item.TextureFilter == RS.CanvasItemTextureFilter.CANVAS_ITEM_TEXTURE_FILTER_DEFAULT ? this.state.DefaultFilter : item.TextureFilter;

        if (textureFilter != this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Filter)
        {
            this.NewBatch(ref batchBroken);

            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Filter = textureFilter;
        }

        var textureRepeat = item.TextureRepeat == RS.CanvasItemTextureRepeat.CANVAS_ITEM_TEXTURE_REPEAT_DEFAULT ? this.state.DefaultRepeat : item.TextureRepeat;

        if (textureRepeat != this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Repeat)
        {
            this.NewBatch(ref batchBroken);

            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Repeat = textureRepeat;
        }

        var baseTransform = canvasTransformInverse * item.FinalTransform;
        var drawTransform = new Transform2D<RealT>(); // Used by transform command
        var baseColor     = item.FinalModulate;
        var baseFlags     = 0;
        var texpixelSize  = new Vector2<RealT>();
        var reclip        = false;
        var skipping      = false;

        // TODO: consider making lights a per-batch property and then baking light operations in the shader for better performance.
        var lights     = new uint[4];
        var lightCount = 0;

        {
            var light = plights;

            while (light != null)
            {
                if (light.RenderIndexCache >= 0 && (item.LightMask & light.ItemMask) != 0 && item.ZFinal >= light.ZMin && item.ZFinal <= light.ZMax && item.GlobalRectCache.IntersectsTransformed(light.XformCache, light.RectCache))
                {
                    var lightIndex = light.RenderIndexCache;
                    var i          = lightCount >> 2;

                    lights[i] = lights[i] | (uint)(lightIndex << ((lightCount & 3) * 8));

                    lightCount++;

                    if (lightCount == this.data.MaxLightsPerItem - 1)
                    {
                        break;
                    }
                }

                light = light.Next;
            }

            baseFlags |= lightCount << (int)Flags.FLAGS_LIGHT_COUNT_SHIFT;
        }

        var lightsDisabled = lightCount == 0 && !this.state.UsingDirectionalLights;

        if (lightsDisabled != this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].LightsDisabled)
        {
            this.NewBatch(ref batchBroken);
            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].LightsDisabled = lightsDisabled;
        }

        var c = item.Commands;
        while (c != null)
        {
            if (skipping && c.Type != Item.CommandType.TYPE_ANIMATION_SLICE)
            {
                c = c.Next;
                continue;
            }

            if (c.Type != Item.CommandType.TYPE_MESH)
            {
                fixed (float* world = this.state.InstanceDataArray[index].World)
                {
                    // For Meshes, this gets updated below.
                    this.UpdateTransform2DToMat2x3(baseTransform * drawTransform, world);
                }
            }

            // Zero out most fields.
            for (var i = 0; i < 4; i++)
            {
                this.state.InstanceDataArray[index].Modulation[i]       = 0;
                this.state.InstanceDataArray[index].NinepatchMargins[i] = 0;
                this.state.InstanceDataArray[index].SrcRect[i]          = 0;
                this.state.InstanceDataArray[index].DstRect[i]          = 0;
                this.state.InstanceDataArray[index].Lights[i]           = 0u;
            }

            this.state.InstanceDataArray[index].ColorTexturePixelSize[0] = 0;
            this.state.InstanceDataArray[index].ColorTexturePixelSize[1] = 0;

            this.state.InstanceDataArray[index].Pad[0] = 0;
            this.state.InstanceDataArray[index].Pad[1] = 0;

            this.state.InstanceDataArray[index].Lights[0] = lights[0];
            this.state.InstanceDataArray[index].Lights[1] = lights[1];
            this.state.InstanceDataArray[index].Lights[2] = lights[2];
            this.state.InstanceDataArray[index].Lights[3] = lights[3];

            this.state.InstanceDataArray[index].Flags = baseFlags | (this.state.InstanceDataArray[index == 0 ? 0 : index - 1].Flags & (int)(Flags.FLAGS_DEFAULT_NORMAL_MAP_USED | Flags.FLAGS_DEFAULT_SPECULAR_MAP_USED)); //reset on each command for sanity, keep canvastexture binding config

            var blendColor = baseColor;
            if (c.Type == Item.CommandType.TYPE_RECT)
            {
                var rect = (Item.CommandRect)c;
                if (rect.Flags.HasFlag(CanvasRectFlags.CANVAS_RECT_LCD))
                {
                    blendMode  = CanvasShaderData.BlendModeType.BLEND_MODE_LCD;
                    blendColor = rect.Modulate * baseColor;
                }
            }

            if (blendMode != this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].BlendMode || blendColor != this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].BlendColor)
            {
                this.NewBatch(ref batchBroken);
                this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].BlendMode  = blendMode;
                this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].BlendColor = blendColor;
            }

            switch (c.Type)
            {
                case Item.CommandType.TYPE_RECT:
                    {
                        var rect = (Item.CommandRect)c;

                        if (rect.Flags.HasFlag(CanvasRectFlags.CANVAS_RECT_TILE) && this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Repeat != RS.CanvasItemTextureRepeat.CANVAS_ITEM_TEXTURE_REPEAT_ENABLED)
                        {
                            this.NewBatch(ref batchBroken);
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Repeat = RS.CanvasItemTextureRepeat.CANVAS_ITEM_TEXTURE_REPEAT_ENABLED;
                        }

                        if (rect.Texture != this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Tex || this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].CommandType != Item.CommandType.TYPE_RECT)
                        {
                            this.NewBatch(ref batchBroken);
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Tex = rect.Texture;
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].CommandType = Item.CommandType.TYPE_RECT;
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Command = c;
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].ShaderVariant = Shaders.CanvasShaderGLES3.ShaderVariant.MODE_QUAD;
                        }

                        this.PrepareCanvasTexture(
                            rect.Texture,
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Filter,
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Repeat,
                            ref index,
                            ref texpixelSize
                        );

                        Rect2<RealT> srcRect;
                        Rect2<RealT> dstRect;

                        if (rect.Texture != default)
                        {
                            srcRect = rect.Flags.HasFlag(CanvasRectFlags.CANVAS_RECT_REGION) ? new Rect2<RealT>(rect.Source.Position * texpixelSize, rect.Source.Size * texpixelSize) : new Rect2<RealT>(0, 0, 1, 1);
                            dstRect = new Rect2<RealT>(rect.Rect.Position, rect.Rect.Size);

                            if (dstRect.Size.X < 0)
                            {
                                dstRect.Position = dstRect.Position with { X = dstRect.Position.X + dstRect.Size.X };
                                dstRect.Size     = dstRect.Size     with { X = dstRect.Size.X * -1 };
                            }

                            if (dstRect.Size.Y < 0)
                            {
                                dstRect.Position = dstRect.Position with { Y = dstRect.Position.Y + dstRect.Size.Y };
                                dstRect.Size     = dstRect.Size     with { Y = dstRect.Size.Y * -1 };
                            }

                            if (rect.Flags.HasFlag(CanvasRectFlags.CANVAS_RECT_FLIP_H))
                            {
                                srcRect.Size = srcRect.Size with { X = srcRect.Size.X * -1 };

                                this.state.InstanceDataArray[index].Flags = (int)Flags.FLAGS_FLIP_H;
                            }

                            if (rect.Flags.HasFlag(CanvasRectFlags.CANVAS_RECT_FLIP_V))
                            {
                                srcRect.Size = srcRect.Size with { Y = srcRect.Size.Y * -1 };

                                this.state.InstanceDataArray[index].Flags |= (int)Flags.FLAGS_FLIP_V;
                            }

                            if (rect.Flags.HasFlag(CanvasRectFlags.CANVAS_RECT_TRANSPOSE))
                            {
                                this.state.InstanceDataArray[index].Flags |= (int)Flags.FLAGS_TRANSPOSE_RECT;
                            }

                            if (rect.Flags.HasFlag(CanvasRectFlags.CANVAS_RECT_CLIP_UV))
                            {
                                this.state.InstanceDataArray[index].Flags |= (int)Flags.FLAGS_CLIP_RECT_UV;
                            }

                        }
                        else
                        {
                            dstRect = new Rect2<RealT>(rect.Rect.Position, rect.Rect.Size);

                            if (dstRect.Size.X < 0)
                            {
                                dstRect.Position = dstRect.Position with { X = dstRect.Position.X + dstRect.Size.X };
                                dstRect.Size     = dstRect.Size     with { X = dstRect.Size.X * -1 };
                            }
                            if (dstRect.Size.Y < 0)
                            {
                                dstRect.Position = dstRect.Position with { Y = dstRect.Position.Y + dstRect.Size.Y };
                                dstRect.Size     = dstRect.Size     with { Y = dstRect.Size.Y * -1 };
                            }

                            srcRect = new Rect2<RealT>(0, 0, 1, 1);
                        }

                        if (rect.Flags.HasFlag(CanvasRectFlags.CANVAS_RECT_MSDF))
                        {
                            this.state.InstanceDataArray[index].Flags |= (int)Flags.FLAGS_USE_MSDF;
                            this.state.InstanceDataArray[index].Msdf[0] = rect.PxRange; // Pixel range.
                            this.state.InstanceDataArray[index].Msdf[1] = rect.Outline; // Outline size.
                            this.state.InstanceDataArray[index].Msdf[2] = 0; // Reserved.
                            this.state.InstanceDataArray[index].Msdf[3] = 0; // Reserved.
                        }
                        else if (rect.Flags.HasFlag(CanvasRectFlags.CANVAS_RECT_LCD))
                        {
                            this.state.InstanceDataArray[index].Flags |= (int)Flags.FLAGS_USE_LCD;
                        }

                        this.state.InstanceDataArray[index].Modulation[0] = rect.Modulate.R * baseColor.R;
                        this.state.InstanceDataArray[index].Modulation[1] = rect.Modulate.G * baseColor.G;
                        this.state.InstanceDataArray[index].Modulation[2] = rect.Modulate.B * baseColor.B;
                        this.state.InstanceDataArray[index].Modulation[3] = rect.Modulate.A * baseColor.A;

                        this.state.InstanceDataArray[index].SrcRect[0] = srcRect.Position.X;
                        this.state.InstanceDataArray[index].SrcRect[1] = srcRect.Position.Y;
                        this.state.InstanceDataArray[index].SrcRect[2] = srcRect.Size.X;
                        this.state.InstanceDataArray[index].SrcRect[3] = srcRect.Size.Y;

                        this.state.InstanceDataArray[index].DstRect[0] = dstRect.Position.X;
                        this.state.InstanceDataArray[index].DstRect[1] = dstRect.Position.Y;
                        this.state.InstanceDataArray[index].DstRect[2] = dstRect.Size.X;
                        this.state.InstanceDataArray[index].DstRect[3] = dstRect.Size.Y;

                        this.AddToBatch(ref index, ref batchBroken);
                    }
                    break;

                case Item.CommandType.TYPE_NINEPATCH:
                    {
                        var np = (Item.CommandNinePatch)c;

                        if (np.Texture != this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Tex || this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].CommandType != Item.CommandType.TYPE_NINEPATCH)
                        {
                            this.NewBatch(ref batchBroken);
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Tex = np.Texture;
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].CommandType = Item.CommandType.TYPE_NINEPATCH;
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Command = c;
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].ShaderVariant = Shaders.CanvasShaderGLES3.ShaderVariant.MODE_NINEPATCH;
                        }

                        this.PrepareCanvasTexture(
                            np.Texture,
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Filter,
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Repeat,
                            ref index,
                            ref texpixelSize
                        );

                        Rect2<RealT> srcRect;
                        var dstRect = new Rect2<RealT>(np.Rect.Position.X, np.Rect.Position.Y, np.Rect.Size.X, np.Rect.Size.Y);

                        if (np.Texture == default)
                        {
                            texpixelSize = new Vector2<RealT>(1, 1);
                            srcRect      = new Rect2<RealT>(0, 0, 1, 1);

                        }
                        else
                        {
                            if (np.Source != default)
                            {
                                srcRect = new Rect2<RealT>(np.Source.Position.X * texpixelSize.X, np.Source.Position.Y * texpixelSize.Y, np.Source.Size.X * texpixelSize.X, np.Source.Size.Y * texpixelSize.Y);
                                this.state.InstanceDataArray[index].ColorTexturePixelSize[0] = 1 / np.Source.Size.X;
                                this.state.InstanceDataArray[index].ColorTexturePixelSize[1] = 1 / np.Source.Size.Y;

                            }
                            else
                            {
                                srcRect = new Rect2<RealT>(0, 0, 1, 1);
                            }
                        }

                        this.state.InstanceDataArray[index].Modulation[0] = np.Color.R * baseColor.R;
                        this.state.InstanceDataArray[index].Modulation[1] = np.Color.G * baseColor.G;
                        this.state.InstanceDataArray[index].Modulation[2] = np.Color.B * baseColor.B;
                        this.state.InstanceDataArray[index].Modulation[3] = np.Color.A * baseColor.A;

                        this.state.InstanceDataArray[index].SrcRect[0] = srcRect.Position.X;
                        this.state.InstanceDataArray[index].SrcRect[1] = srcRect.Position.Y;
                        this.state.InstanceDataArray[index].SrcRect[2] = srcRect.Size.X;
                        this.state.InstanceDataArray[index].SrcRect[3] = srcRect.Size.Y;

                        this.state.InstanceDataArray[index].DstRect[0] = dstRect.Position.X;
                        this.state.InstanceDataArray[index].DstRect[1] = dstRect.Position.Y;
                        this.state.InstanceDataArray[index].DstRect[2] = dstRect.Size.X;
                        this.state.InstanceDataArray[index].DstRect[3] = dstRect.Size.Y;

                        this.state.InstanceDataArray[index].Flags |= (int)np.AxisX << (int)Flags.FLAGS_NINEPATCH_H_MODE_SHIFT;
                        this.state.InstanceDataArray[index].Flags |= (int)np.AxisY << (int)Flags.FLAGS_NINEPATCH_V_MODE_SHIFT;

                        if (np.DrawCenter)
                        {
                            this.state.InstanceDataArray[index].Flags |= (int)Flags.FLAGS_NINEPACH_DRAW_CENTER;
                        }

                        this.state.InstanceDataArray[index].NinepatchMargins[0] = np.Margin[(int)Side.SIDE_LEFT];
                        this.state.InstanceDataArray[index].NinepatchMargins[1] = np.Margin[(int)Side.SIDE_TOP];
                        this.state.InstanceDataArray[index].NinepatchMargins[2] = np.Margin[(int)Side.SIDE_RIGHT];
                        this.state.InstanceDataArray[index].NinepatchMargins[3] = np.Margin[(int)Side.SIDE_BOTTOM];

                        this.AddToBatch(ref index, ref batchBroken);

                        // Restore if overridden.
                        this.state.InstanceDataArray[index].ColorTexturePixelSize[0] = texpixelSize.X;
                        this.state.InstanceDataArray[index].ColorTexturePixelSize[1] = texpixelSize.Y;
                    }
                    break;

                case Item.CommandType.TYPE_POLYGON:
                    {
                        var polygon = (Item.CommandPolygon)c;

                        // Polygon's can't be batched, so always create a new batch
                        this.NewBatch(ref batchBroken);

                        this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Tex           = polygon.Texture;
                        this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].CommandType   = Item.CommandType.TYPE_POLYGON;
                        this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Command       = c;
                        this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].ShaderVariant = Shaders.CanvasShaderGLES3.ShaderVariant.MODE_ATTRIBUTES;

                        this.PrepareCanvasTexture(
                            polygon.Texture,
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Filter,
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Repeat,
                            ref index,
                            ref texpixelSize
                        );

                        this.state.InstanceDataArray[index].Modulation[0] = baseColor.R;
                        this.state.InstanceDataArray[index].Modulation[1] = baseColor.G;
                        this.state.InstanceDataArray[index].Modulation[2] = baseColor.B;
                        this.state.InstanceDataArray[index].Modulation[3] = baseColor.A;

                        for (var j = 0; j < 4; j++)
                        {
                            this.state.InstanceDataArray[index].SrcRect[j] = 0;
                            this.state.InstanceDataArray[index].DstRect[j] = 0;
                            this.state.InstanceDataArray[index].NinepatchMargins[j] = 0;
                        }

                        this.AddToBatch(ref index, ref batchBroken);
                    }
                    break;

                case Item.CommandType.TYPE_PRIMITIVE:
                    {
                        var primitive = (Item.CommandPrimitive)c;

                        if (primitive.PointCount != this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].PrimitivePoints || this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].CommandType != Item.CommandType.TYPE_PRIMITIVE)
                        {
                            this.NewBatch(ref batchBroken);
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Tex             = primitive.Texture;
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].PrimitivePoints = primitive.PointCount;
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].CommandType     = Item.CommandType.TYPE_PRIMITIVE;
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Command         = c;
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].ShaderVariant   = Shaders.CanvasShaderGLES3.ShaderVariant.MODE_PRIMITIVE;
                        }

                        this.PrepareCanvasTexture(
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Tex,
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Filter,
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Repeat,
                            ref index,
                            ref texpixelSize
                        );

                        for (var j = 0; j < Math.Min(3u, primitive.PointCount); j++)
                        {
                            this.state.InstanceDataArray[index].Points[j * 2 + 0] = primitive.Points[j].X;
                            this.state.InstanceDataArray[index].Points[j * 2 + 1] = primitive.Points[j].Y;
                            this.state.InstanceDataArray[index].Uvs[j * 2 + 0]    = primitive.Uvs[j].X;
                            this.state.InstanceDataArray[index].Uvs[j * 2 + 1]    = primitive.Uvs[j].Y;

                            var col = primitive.Colors[j] * baseColor;

                            this.state.InstanceDataArray[index].Colors[j * 2 + 0] = ((uint)(Half)col.G << 16) | (uint)(Half)col.R;
                            this.state.InstanceDataArray[index].Colors[j * 2 + 1] = ((uint)(Half)col.A << 16) | (uint)(Half)col.B;
                        }

                        this.AddToBatch(ref index, ref batchBroken);

                        if (primitive.PointCount == 4)
                        {
                            // Reset base data.
                            fixed (float* world = this.state.InstanceDataArray[index].World)
                            {
                                this.UpdateTransform2DToMat2x3(baseTransform * drawTransform, world);
                            }

                            this.PrepareCanvasTexture(
                                this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Tex,
                                this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Filter,
                                this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Repeat,
                                ref index,
                                ref texpixelSize
                            );

                            for (var j = 0; j < 3; j++)
                            {
                                var offset = j == 0 ? 0 : 1;
                                // Second triangle in the quad. Uses vertices 0, 2, 3.
                                this.state.InstanceDataArray[index].Points[j * 2 + 0] = primitive.Points[j + offset].X;
                                this.state.InstanceDataArray[index].Points[j * 2 + 1] = primitive.Points[j + offset].Y;
                                this.state.InstanceDataArray[index].Uvs[j * 2 + 0]    = primitive.Uvs[j + offset].X;
                                this.state.InstanceDataArray[index].Uvs[j * 2 + 1]    = primitive.Uvs[j + offset].Y;

                                var col = primitive.Colors[j + offset] * baseColor;

                                this.state.InstanceDataArray[index].Colors[j * 2 + 0] = ((uint)(Half)col.G << 16) | (uint)(Half)col.R;
                                this.state.InstanceDataArray[index].Colors[j * 2 + 1] = ((uint)(Half)col.A << 16) | (uint)(Half)col.B;
                            }

                            this.AddToBatch(ref index, ref batchBroken);
                        }
                    }
                    break;

                case Item.CommandType.TYPE_MESH:
                case Item.CommandType.TYPE_MULTIMESH:
                case Item.CommandType.TYPE_PARTICLES:
                    {
                        // Mesh's can't be batched, so always create a new batch
                        this.NewBatch(ref batchBroken);

                        var modulate = new Color(1, 1, 1, 1);

                        this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].ShaderVariant = Shaders.CanvasShaderGLES3.ShaderVariant.MODE_ATTRIBUTES;
                        if (c.Type == Item.CommandType.TYPE_MESH)
                        {
                            var m = (Item.CommandMesh)c;

                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Tex = m.Texture;

                            fixed (float* world = this.state.InstanceDataArray[index].World)
                            {
                                this.UpdateTransform2DToMat2x3(baseTransform * drawTransform * m.Transform, world);
                            }

                            modulate = m.Modulate;

                        }
                        else if (c.Type == Item.CommandType.TYPE_MULTIMESH)
                        {
                            var mm = (Item.CommandMultiMesh)c;

                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Tex = mm.Texture;
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].ShaderVariant = Shaders.CanvasShaderGLES3.ShaderVariant.MODE_INSTANCED;

                            if (MeshStorage.Singleton.MultimeshUsesColors(mm.Multimesh))
                            {
                                this.state.InstanceDataArray[index].Flags |= (int)Flags.FLAGS_INSTANCING_HAS_COLORS;
                            }
                            if (MeshStorage.Singleton.MultimeshUsesCustomData(mm.Multimesh))
                            {
                                this.state.InstanceDataArray[index].Flags |= (int)Flags.FLAGS_INSTANCING_HAS_CUSTOM_DATA;
                            }
                        }
                        else if (c.Type == Item.CommandType.TYPE_PARTICLES)
                        {
                            var particlesStorage = ParticlesStorage.Singleton;
                            var textureStorage   = TextureStorage.Singleton;

                            var pt = (Item.CommandParticles)c;

                            var particles = pt.Particles;
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Tex           = pt.Texture;
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].ShaderVariant = Shaders.CanvasShaderGLES3.ShaderVariant.MODE_INSTANCED;
                            this.state.InstanceDataArray[index].Flags |= (int)Flags.FLAGS_INSTANCING_HAS_COLORS;
                            this.state.InstanceDataArray[index].Flags |= (int)Flags.FLAGS_INSTANCING_HAS_CUSTOM_DATA;

                            if (particlesStorage.ParticlesHasCollision(particles) && textureStorage.RenderTargetIsSdfEnabled(renderTarget))
                            {
                                // Pass collision information.
                                var xform = item.FinalTransform;

                                var sdfTexture = textureStorage.RenderTargetGetSdfTexture(renderTarget);

                                var toScreen = new Rect2<RealT>();
                                {
                                    var sdfRect = textureStorage.RenderTargetGetSdfRect(renderTarget).As<float>();

                                    toScreen.Size     = new Vector2<RealT>(1 / sdfRect.Size.X, 1 / sdfRect.Size.Y);
                                    toScreen.Position = sdfRect.Position * toScreen.Size;
                                }

                                particlesStorage.ParticlesSetCanvasSdfCollision(pt.Particles, true, xform, toScreen, sdfTexture);
                            }
                            else
                            {
                                particlesStorage.ParticlesSetCanvasSdfCollision(pt.Particles, false, default, default, 0);
                            }
                            sdfUsed |= particlesStorage.ParticlesHasCollision(particles);
                        }

                        this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Command = c;
                        this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].CommandType = c.Type;

                        this.PrepareCanvasTexture(
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Tex,
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Filter,
                            this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Repeat,
                            ref index,
                            ref texpixelSize
                        );

                        this.state.InstanceDataArray[index].Modulation[0] = baseColor.R * modulate.R;
                        this.state.InstanceDataArray[index].Modulation[1] = baseColor.G * modulate.G;
                        this.state.InstanceDataArray[index].Modulation[2] = baseColor.B * modulate.B;
                        this.state.InstanceDataArray[index].Modulation[3] = baseColor.A * modulate.A;

                        for (var j = 0; j < 4; j++)
                        {
                            this.state.InstanceDataArray[index].SrcRect[j]          = 0;
                            this.state.InstanceDataArray[index].DstRect[j]          = 0;
                            this.state.InstanceDataArray[index].NinepatchMargins[j] = 0;
                        }

                        this.AddToBatch(ref index, ref batchBroken);
                    }
                    break;

                case Item.CommandType.TYPE_TRANSFORM:
                    {
                        var transform = (Item.CommandTransform)c;
                        drawTransform = transform.Xform;
                    }
                    break;

                case Item.CommandType.TYPE_CLIP_IGNORE:
                    {
                        var ci = (Item.CommandClipIgnore)c;
                        if (currentClip != null)
                        {
                            if (ci.Ignore != reclip)
                            {
                                this.NewBatch(ref batchBroken);
                                if (ci.Ignore)
                                {
                                    this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Clip = null;
                                    reclip = true;
                                }
                                else
                                {
                                    this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Clip = currentClip;
                                    reclip = false;
                                }
                            }
                        }
                    }
                    break;

                case Item.CommandType.TYPE_ANIMATION_SLICE:
                    {
                        var @as = (Item.CommandAnimationSlice)c;
                        var currentTime = RSG.Rasterizer.TotalTime;

                        var localTime = currentTime - @as.Offset % @as.AnimationLength;
                        skipping = !(localTime >= @as.SliceBegin && localTime < @as.SliceEnd);

                        RenderingServerDefault.RedrawRequest(); // animation visible means redraw request
                    }
                    break;
            }

            c = c.Next;
            batchBroken = false;
        }

        if (currentClip != null && reclip)
        {
            //will make it re-enable clipping if needed afterwards
            currentClip = null;
        }
    }

    private void RenderBatch(Light lights, int index)
    {
        if (ERR_FAIL_COND(this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Command == null))
        {
            return;
        }

        this.BindCanvasTexture(this.state.CanvasInstanceBatches[index].Tex, this.state.CanvasInstanceBatches[index].Filter, this.state.CanvasInstanceBatches[index].Repeat);

        var gl = GL.Singleton;

        switch (this.state.CanvasInstanceBatches[index].CommandType)
        {
            case Item.CommandType.TYPE_RECT:
            case Item.CommandType.TYPE_NINEPATCH:
                {
                    gl.BindVertexArray(this.data.IndexedQuadArray);
                    gl.BindBuffer(BufferTargetARB.ArrayBuffer, this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].InstanceBuffers[this.state.CanvasInstanceBatches[index].InstanceBufferIndex]);
                    var rangeStart = this.state.CanvasInstanceBatches[index].Start * Marshal.SizeOf<InstanceData>();
                    this.EnableAttributes(rangeStart, false);

                    gl.DrawElementsInstanced(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 0, this.state.CanvasInstanceBatches[index].InstanceCount);
                    gl.BindVertexArray(0);
                }

                break;

            case Item.CommandType.TYPE_POLYGON:
                {
                    var polygon = (Item.CommandPolygon)this.state.CanvasInstanceBatches[index].Command!;

                    var pb = this.polygonBuffers.Polygons[polygon.Polygon.PolygonId];
                    if (ERR_FAIL_COND(pb == null))
                    {
                        return;
                    }

                    gl.BindVertexArray(pb!.VertexArray);
                    gl.BindBuffer(BufferTargetARB.ArrayBuffer, this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].InstanceBuffers[this.state.CanvasInstanceBatches[index].InstanceBufferIndex]);

                    var rangeStart = this.state.CanvasInstanceBatches[index].Start * Marshal.SizeOf<InstanceData>();
                    this.EnableAttributes(rangeStart, false);

                    if (pb.ColorDisabled && pb.Color != new Color(1, 1, 1, 1))
                    {
                        gl.VertexAttrib4f((uint)RS.ArrayType.ARRAY_COLOR, pb.Color.R, pb.Color.G, pb.Color.B, pb.Color.A);
                    }

                    if (pb.IndexBuffer != 0)
                    {
                        gl.DrawElementsInstanced(prim[(int)polygon.Primitive], pb.Count, DrawElementsType.UnsignedInt, default, 1);
                    }
                    else
                    {
                        gl.DrawArraysInstanced(prim[(int)polygon.Primitive], 0, pb.Count, 1);
                    }
                    gl.BindVertexArray(0);

                    if (pb.ColorDisabled && pb.Color != new Color(1, 1, 1, 1))
                    {
                        // Reset so this doesn't pollute other draw calls.
                        gl.VertexAttrib4f((uint)RS.ArrayType.ARRAY_COLOR, 1, 1, 1, 1);
                    }
                }
                break;

            case Item.CommandType.TYPE_PRIMITIVE:
                {
                    gl.BindVertexArray(this.data.CanvasQuadArray);
                    gl.BindBuffer(BufferTargetARB.ArrayBuffer, this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].InstanceBuffers[this.state.CanvasInstanceBatches[index].InstanceBufferIndex]);
                    var rangeStart = this.state.CanvasInstanceBatches[index].Start * Marshal.SizeOf<InstanceData>();
                    this.EnableAttributes(rangeStart, true);

                    var primitive = new[]
                    {
                        PrimitiveType.Points,
                        PrimitiveType.Points,
                        PrimitiveType.Lines,
                        PrimitiveType.Triangles,
                        PrimitiveType.Triangles
                    };

                    var instanceCount = this.state.CanvasInstanceBatches[index].InstanceCount;

                    if (ERR_FAIL_COND(instanceCount <= 0))
                    {
                        return;
                    }

                    if (instanceCount >= 1)
                    {
                        gl.DrawArraysInstanced(primitive[this.state.CanvasInstanceBatches[index].PrimitivePoints], 0, this.state.CanvasInstanceBatches[index].PrimitivePoints, instanceCount);
                    }

                }
                break;

            case Item.CommandType.TYPE_MESH:
            case Item.CommandType.TYPE_MULTIMESH:
            case Item.CommandType.TYPE_PARTICLES:
                {
                    var meshStorage      = MeshStorage.Singleton;
                    var particlesStorage = ParticlesStorage.Singleton;

                    var mesh                   = default(Guid);
                    var meshInstance           = default(Guid);
                    var instanceCount          = 1;
                    var instanceBuffer         = 0u;
                    var instanceStride         = 0;
                    var instanceColorOffset    = 0;
                    var instanceUsesColor      = false;
                    var instanceUsesCustomData = false;

                    if (this.state.CanvasInstanceBatches[index].CommandType == Item.CommandType.TYPE_MESH)
                    {
                        var m = (Item.CommandMesh)this.state.CanvasInstanceBatches[index].Command!;
                        mesh = m.Mesh;
                        meshInstance = m.MeshInstance;

                    }
                    else if (this.state.CanvasInstanceBatches[index].CommandType == Item.CommandType.TYPE_MULTIMESH)
                    {
                        var mm = (Item.CommandMultiMesh)this.state.CanvasInstanceBatches[index].Command!;

                        var multimesh = mm.Multimesh;
                        mesh = meshStorage.MultimeshGetMesh(multimesh);

                        if (meshStorage.MultimeshGetTransformFormat(multimesh) != RS.MultimeshTransformFormat.MULTIMESH_TRANSFORM_2D)
                        {
                            break;
                        }

                        instanceCount = meshStorage.MultimeshGetInstancesToDraw(multimesh);

                        if (instanceCount == 0)
                        {
                            break;
                        }

                        instanceBuffer         = meshStorage.MultimeshGetGlBuffer(multimesh);
                        instanceStride         = meshStorage.MultimeshGetStride(multimesh);
                        instanceColorOffset    = meshStorage.MultimeshGetColorOffset(multimesh);
                        instanceUsesColor      = meshStorage.MultimeshUsesColors(multimesh);
                        instanceUsesCustomData = meshStorage.MultimeshUsesCustomData(multimesh);

                    }
                    else if (this.state.CanvasInstanceBatches[index].CommandType == Item.CommandType.TYPE_PARTICLES)
                    {
                        var pt = (Item.CommandParticles)this.state.CanvasInstanceBatches[index].Command!;
                        var particles = pt.Particles;
                        mesh = particlesStorage.ParticlesGetDrawPassMesh(particles, 0);

                        if (ERR_BREAK(particlesStorage.ParticlesGetMode(particles) != RS.ParticlesMode.PARTICLES_MODE_2D))
                        {
                            break;
                        }

                        particlesStorage.ParticlesRequestProcess(particles);

                        if (particlesStorage.ParticlesIsInactive(particles))
                        {
                            break;
                        }

                        RenderingServerDefault.RedrawRequest(); // Active particles means redraw request.

                        var dpc = particlesStorage.ParticlesGetDrawPasses(particles);
                        if (dpc == 0)
                        {
                            break; // Nothing to draw.
                        }

                        instanceCount          = particlesStorage.ParticlesGetAmount(particles);
                        instanceBuffer         = particlesStorage.ParticlesGetGlBuffer(particles);
                        instanceStride         = 12; // 8 bytes for instance transform and 4 bytes for packed color and custom.
                        instanceColorOffset    = 8; // 8 bytes for instance transform.
                        instanceUsesColor      = true;
                        instanceUsesCustomData = true;
                    }

                    if (ERR_FAIL_COND(mesh == default))
                    {
                        return;
                    }

                    var surfCount = meshStorage.MeshGetSurfaceCount(mesh);

                    for (var j = 0; j < surfCount; j++)
                    {
                        var surface = meshStorage.MeshGetSurface(mesh, j);

                        var primitive = meshStorage.MeshSurfaceGetPrimitive(surface);
                        if (ERR_CONTINUE(primitive < 0 || primitive >= RS.PrimitiveType.PRIMITIVE_MAX))
                        {
                            continue;
                        }

                        var inputMask = 0; // 2D meshes always use the same vertex format

                        uint vertexArrayGl;
                        if (meshInstance != default)
                        {
                            meshStorage.MeshInstanceSurfaceGetVertexArraysAndFormat(meshInstance, j, inputMask, out vertexArrayGl);
                        }
                        else
                        {
                            meshStorage.MeshSurfaceGetVertexArraysAndFormat(surface, inputMask, out vertexArrayGl);
                        }

                        var indexArrayGl = meshStorage.MeshSurfaceGetIndexBuffer(surface, 0);
                        var useIndexBuffer = false;
                        gl.BindVertexArray(vertexArrayGl);
                        gl.BindBuffer(BufferTargetARB.ArrayBuffer, this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].InstanceBuffers[this.state.CanvasInstanceBatches[index].InstanceBufferIndex]);

                        var rangeStart = this.state.CanvasInstanceBatches[index].Start * Marshal.SizeOf<InstanceData>();
                        this.EnableAttributes(rangeStart, false, (uint)instanceCount);

                        if (indexArrayGl != 0)
                        {
                            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, indexArrayGl);
                            useIndexBuffer = true;
                        }

                        if (instanceCount > 1)
                        {
                            if (instanceBuffer == 0)
                            {
                                break;
                            }
                            // Bind instance buffers.
                            gl.BindBuffer(BufferTargetARB.ArrayBuffer, instanceBuffer);
                            gl.EnableVertexAttribArray(1);
                            gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, instanceStride * FLOAT_SIZE, 0);
                            gl.VertexAttribDivisor(1, 1);
                            gl.EnableVertexAttribArray(2);
                            gl.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, instanceStride * FLOAT_SIZE, 4 * 4);
                            gl.VertexAttribDivisor(2, 1);

                            if (instanceUsesColor || instanceUsesCustomData)
                            {
                                gl.EnableVertexAttribArray(5);
                                gl.VertexAttribIPointer(5, 4, VertexAttribIType.UnsignedInt, instanceStride * FLOAT_SIZE, instanceColorOffset * FLOAT_SIZE);
                                gl.VertexAttribDivisor(5, 1);
                            }
                        }

                        var primitiveGl = prim[(int)primitive];

                        if (useIndexBuffer)
                        {
                            gl.DrawElementsInstanced(primitiveGl, meshStorage.MeshSurfaceGetVerticesDrawnCount(surface), meshStorage.MeshSurfaceGetIndexType(surface), 0, instanceCount);
                        }
                        else
                        {
                            gl.DrawArraysInstanced(primitiveGl, 0, meshStorage.MeshSurfaceGetVerticesDrawnCount(surface), instanceCount);
                        }
                        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
                        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
                        if (instanceCount > 1)
                        {
                            gl.DisableVertexAttribArray(5);
                            gl.DisableVertexAttribArray(6);
                            gl.DisableVertexAttribArray(7);
                            gl.DisableVertexAttribArray(8);
                        }
                    }

                }
                break;
            case Item.CommandType.TYPE_TRANSFORM:
            case Item.CommandType.TYPE_CLIP_IGNORE:
            case Item.CommandType.TYPE_ANIMATION_SLICE:
                {
                    // Can ignore these as they only impact batch creation.
                }
                break;
        }
    }

    private void RenderItems(
        Guid                  toRenderTarget,
        int                   itemCount,
        in Transform2D<RealT> canvasTransformInverse,
        Light                 lights,
        out bool              sdfUsed,
        bool                  toBackbuffer = default
    )
    {
        sdfUsed = false;

        var materialStorage = MaterialStorage.Singleton;

        this.CanvasBegin(toRenderTarget, toBackbuffer);

        if (itemCount <= 0)
        {

            // Nothing to draw, just call canvas_begin() to clear the render target and return.
            return;
        }

        var index = 0;

        Item?             currentClip     = null;
        CanvasShaderData? shaderDataCache = null;

        // Record Batches.
        // First item always forms its own batch.
        var batchBroken = false;

        this.NewBatch(ref batchBroken);

        // Override the start position and index as we want to start from where we finished off last time.
        this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Start = this.state.LastItemIndex;
        index = 0;

        for (var i = 0; i < itemCount; i++)
        {
            var ci = this.items[i];

            if (ci.FinalClipOwner != this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Clip)
            {
                this.NewBatch(ref batchBroken);
                this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Clip = ci.FinalClipOwner;
                currentClip = ci.FinalClipOwner;
            }

            var material = ci.MaterialOwner == null ? ci.Material : ci.MaterialOwner.Material;
            if (ci.CanvasGroup != null)
            {
                if (ci.CanvasGroup.Mode == RS.CanvasGroupMode.CANVAS_GROUP_MODE_CLIP_AND_DRAW)
                {
                    if (!toBackbuffer)
                    {
                        material = this.defaultClipChildrenMaterial;
                    }
                }
                else
                {
                    if (material == default)
                    {
                        material = ci.CanvasGroup.Mode == RS.CanvasGroupMode.CANVAS_GROUP_MODE_CLIP_ONLY
                            ? this.defaultClipChildrenMaterial
                            : this.defaultCanvasGroupMaterial;
                    }
                }
            }

            if (material != this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Material)
            {
                this.NewBatch(ref batchBroken);

                CanvasMaterialData? materialData = null;

                if (material != default)
                {
                    materialData = (CanvasMaterialData)materialStorage.MaterialGetData(material, RS.ShaderMode.SHADER_CANVAS_ITEM);
                }
                shaderDataCache = null;
                if (materialData != null)
                {
                    if (materialData.ShaderData!.Version != default && materialData.ShaderData!.Valid)
                    {
                        shaderDataCache = materialData.ShaderData;
                    }
                }

                this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Material     = material;
                this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].MaterialData = materialData;
            }

            var blendMode = shaderDataCache != null ? shaderDataCache.BlendMode : CanvasShaderData.BlendModeType.BLEND_MODE_MIX;

            this.RecordItemCommands(ci, toRenderTarget, canvasTransformInverse, currentClip, blendMode, lights, ref index, ref batchBroken, ref sdfUsed);
        }

        if (index == 0)
        {
            // Nothing to render, just return.
            this.state.CurrentBatchIndex = 0;
            this.state.CanvasInstanceBatches.Clear();
            return;
        }

        var gl = GL.Singleton;

        // Copy over all data needed for rendering.
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].InstanceBuffers[this.state.CurrentInstanceBufferIndex]);
        #if WEB_ENABLED
        gl.BufferSubData(BufferTargetARB.ArrayBuffer, state.LastItemIndex * Marshal.SizeOf<InstanceData>(), state.InstanceDataArray);
        #else
        // On Desktop and mobile we map the memory without synchronizing for maximum speed.
        var buffer = gl.MapBufferRange(BufferTargetARB.ArrayBuffer, this.state.LastItemIndex * Marshal.SizeOf<InstanceData>(), index * Marshal.SizeOf<InstanceData>(), MapBufferAccessMask.MapWriteBit | MapBufferAccessMask.MapUnsynchronizedBit);

        UnmanagedUtils.Copy(this.state.InstanceDataArray, buffer, index);

        gl.UnmapBuffer(BufferTargetARB.ArrayBuffer);
        #endif

        gl.Disable(EnableCap.ScissorTest);
        currentClip = null;

        var lastBlendMode = CanvasShaderData.BlendModeType.BLEND_MODE_MIX;

        this.state.CurrentTex = default;

        for (var i = 0; i <= this.state.CurrentBatchIndex; i++)
        {
            //setup clip
            if (currentClip != this.state.CanvasInstanceBatches[i].Clip)
            {
                currentClip = this.state.CanvasInstanceBatches[i].Clip;
                if (currentClip != null)
                {
                    gl.Enable(EnableCap.ScissorTest);
                    gl.Scissor(
                        (int)currentClip.FinalClipRect.Position.X,
                        (int)currentClip.FinalClipRect.Position.Y,
                        (int)currentClip.FinalClipRect.Size.X,
                        (int)currentClip.FinalClipRect.Size.Y
                    );
                }
                else
                {
                    gl.Disable(EnableCap.ScissorTest);
                }
            }

            var materialData = this.state.CanvasInstanceBatches[i].MaterialData;
            var variant = this.state.CanvasInstanceBatches[i].ShaderVariant;
            var specialization = 0ul;
            specialization |= Convert.ToUInt64(this.state.CanvasInstanceBatches[i].LightsDisabled);
            specialization |= Convert.ToUInt64(!Config.Singleton.FloatTextureSupported) << 1;
            var shaderVersion = this.data.CanvasShaderDefaultVersion;

            if (materialData != null)
            {
                if (materialData.ShaderData!.Version != default && materialData.ShaderData.Valid)
                {
                    // Bind uniform buffer and textures
                    materialData.BindUniforms();
                    shaderVersion = materialData.ShaderData.Version;
                }
            }

            var success = MaterialStorage.Singleton.Shaders.CanvasShader.VersionBindShader(shaderVersion, variant, specialization);
            if (!success)
            {
                continue;
            }

            var blendMode = this.state.CanvasInstanceBatches[i].BlendMode;

            if (lastBlendMode != blendMode)
            {
                if (lastBlendMode == CanvasShaderData.BlendModeType.BLEND_MODE_DISABLED)
                {
                    // re-enable it
                    gl.Enable(EnableCap.Blend);
                }
                else if (blendMode == CanvasShaderData.BlendModeType.BLEND_MODE_DISABLED)
                {
                    // disable it
                    gl.Disable(EnableCap.Blend);
                }

                switch (blendMode)
                {
                    case CanvasShaderData.BlendModeType.BLEND_MODE_DISABLED:
                        {
                            // Nothing to do here.
                        }
                        break;
                    case CanvasShaderData.BlendModeType.BLEND_MODE_LCD:
                        gl.BlendEquation(BlendEquationModeEXT.FuncAdd);

                        if (this.state.TransparentRenderTarget)
                        {
                            gl.BlendFuncSeparate(BlendingFactor.ConstantColor, BlendingFactor.OneMinusSrcColor, BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
                        }
                        else
                        {
                            gl.BlendFuncSeparate(BlendingFactor.ConstantColor, BlendingFactor.OneMinusSrcColor, BlendingFactor.Zero, BlendingFactor.One);
                        }

                        var blendColor = this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].BlendColor;

                        gl.BlendColor(blendColor.R, blendColor.G, blendColor.B, blendColor.A);

                        break;
                    case CanvasShaderData.BlendModeType.BLEND_MODE_MIX:
                        gl.BlendEquation(BlendEquationModeEXT.FuncAdd);

                        if (this.state.TransparentRenderTarget)
                        {
                            gl.BlendFuncSeparate(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha, BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
                        }
                        else
                        {
                            gl.BlendFuncSeparate(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha, BlendingFactor.Zero, BlendingFactor.One);
                        }

                        break;
                    case CanvasShaderData.BlendModeType.BLEND_MODE_ADD:

                        gl.BlendEquation(BlendEquationModeEXT.FuncAdd);

                        if (this.state.TransparentRenderTarget)
                        {
                            gl.BlendFuncSeparate(BlendingFactor.SrcAlpha, BlendingFactor.One, BlendingFactor.SrcAlpha, BlendingFactor.One);
                        }
                        else
                        {
                            gl.BlendFuncSeparate(BlendingFactor.SrcAlpha, BlendingFactor.One, BlendingFactor.Zero, BlendingFactor.One);
                        }

                        break;
                    case CanvasShaderData.BlendModeType.BLEND_MODE_SUB:
                        gl.BlendEquation(BlendEquationModeEXT.FuncReverseSubtract);

                        if (this.state.TransparentRenderTarget)
                        {
                            gl.BlendFuncSeparate(BlendingFactor.SrcAlpha, BlendingFactor.One, BlendingFactor.SrcAlpha, BlendingFactor.One);
                        }
                        else
                        {
                            gl.BlendFuncSeparate(BlendingFactor.SrcAlpha, BlendingFactor.One, BlendingFactor.Zero, BlendingFactor.One);
                        }

                        break;
                    case CanvasShaderData.BlendModeType.BLEND_MODE_MUL:
                        gl.BlendEquation(BlendEquationModeEXT.FuncAdd);

                        if (this.state.TransparentRenderTarget)
                        {
                            gl.BlendFuncSeparate(BlendingFactor.DstColor, BlendingFactor.Zero, BlendingFactor.DstAlpha, BlendingFactor.Zero);
                        }
                        else
                        {
                            gl.BlendFuncSeparate(BlendingFactor.DstColor, BlendingFactor.Zero, BlendingFactor.Zero, BlendingFactor.One);
                        }

                        break;
                    case CanvasShaderData.BlendModeType.BLEND_MODE_PMALPHA:

                        gl.BlendEquation(BlendEquationModeEXT.FuncAdd);

                        if (this.state.TransparentRenderTarget)
                        {
                            gl.BlendFuncSeparate(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha, BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
                        }
                        else
                        {
                            gl.BlendFuncSeparate(BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha, BlendingFactor.Zero, BlendingFactor.One);
                        }

                        break;
                }

                lastBlendMode = blendMode;
            }

            this.RenderBatch(lights, i);
        }

        this.state.CurrentBatchIndex = 0;
        this.state.CanvasInstanceBatches.Clear();
        this.state.LastItemIndex += index;
    }

    private void NewBatch(ref bool batchBroken)
    {
        if (this.state.CanvasInstanceBatches.Count == 0)
        {
            this.state.CanvasInstanceBatches.Add(new());
            return;
        }

        if (batchBroken || this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].InstanceCount == 0)
        {
            return;
        }

        batchBroken = true;

        // Copy the properties of the current batch, we will manually update the things that changed.
        var newBatch = this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex];

        newBatch.InstanceCount       = 0;
        newBatch.Start               = this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].Start + this.state.CanvasInstanceBatches[this.state.CurrentBatchIndex].InstanceCount;
        newBatch.InstanceBufferIndex = this.state.CurrentInstanceBufferIndex;

        this.state.CurrentBatchIndex++;
        this.state.CanvasInstanceBatches.Add(newBatch);
    }

    private unsafe void UpdateTransform2DToMat2x3(Transform2D<RealT> transform, float* mat2x3)
    {
        mat2x3[0] = transform[0, 0];
        mat2x3[1] = transform[0, 1];
        mat2x3[2] = transform[1, 0];
        mat2x3[3] = transform[1, 1];
        mat2x3[4] = transform[2, 0];
        mat2x3[5] = transform[2, 1];
    }

    private unsafe void UpdateTransform2DToMat2x4(Transform2D<RealT> transform, float* mat2x4)
    {
        mat2x4[0] = transform[0, 0];
        mat2x4[1] = transform[1, 0];
        mat2x4[2] = 0;
        mat2x4[3] = transform[2, 0];

        mat2x4[4] = transform[0, 1];
        mat2x4[5] = transform[1, 1];
        mat2x4[6] = 0;
        mat2x4[7] = transform[2, 1];
    }

    public void CanvasBegin(Guid toRenderTarget, bool toBackbuffer)
    {
        var textureStorage = TextureStorage.Singleton;
        var config         = Config.Singleton;
        var gl             = GL.Singleton;

        var renderTarget = textureStorage.GetRenderTarget(toRenderTarget)!;

        if (toBackbuffer)
        {
            gl.BindFramebuffer(FramebufferTarget.Framebuffer, renderTarget.BackbufferFbo);
            gl.ActiveTexture(TextureUnit.Texture0 + config.MaxTextureImageUnits - 4);

            var tex1 = textureStorage.GetTexture(textureStorage.TextureGlGetDefault(DefaultGLTexture.DEFAULT_GL_TEXTURE_WHITE))!;

            gl.BindTexture(TextureTarget.Texture2D, tex1.TexId);
        }
        else
        {
            gl.BindFramebuffer(FramebufferTarget.Framebuffer, renderTarget.Fbo);
            gl.ActiveTexture(TextureUnit.Texture0 + config.MaxTextureImageUnits - 4);
            gl.BindTexture(TextureTarget.Texture2D, renderTarget.Backbuffer);
        }

        if (renderTarget.IsTransparent || toBackbuffer)
        {
            this.state.TransparentRenderTarget = true;
            gl.BlendFuncSeparate(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha, BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
        }
        else
        {
            this.state.TransparentRenderTarget = false;
            gl.BlendFuncSeparate(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha, BlendingFactor.Zero, BlendingFactor.One);
        }

        if (renderTarget != null && renderTarget.ClearRequested)
        {
            var col = renderTarget.ClearColor;

            gl.ClearColor(col.R, col.G, col.B, col.A);
            gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            renderTarget.ClearRequested = false;
        }

        gl.ActiveTexture(TextureUnit.Texture0);

        var tex = textureStorage.GetTexture(textureStorage.TextureGlGetDefault(DefaultGLTexture.DEFAULT_GL_TEXTURE_WHITE))!;

        gl.BindTexture(TextureTarget.Texture2D, tex.TexId);
    }

    #region public overrided methods
    public unsafe override void CanvasRenderItems(
        Guid                       toRenderTarget,
        Item?                      itemList,
        in Color                   modulate,
        Light?                     lightList,
        Light?                     directionalLightList,
        in Transform2D<RealT>      canvasTransform,
        RS.CanvasItemTextureFilter defaultFilter,
        RS.CanvasItemTextureRepeat defaultRepeat,
        bool                       snap2DVerticesToPixel,
        out bool                   sdfUsed
    )
    {
        var gl              = GL.Singleton;
        var textureStorage  = TextureStorage.Singleton;
        var materialStorage = MaterialStorage.Singleton;
        var meshStorage     = MeshStorage.Singleton;

        var canvasTransformInverse = canvasTransform.AffineInverse();

        // Clear out any state that may have been left from the 3D pass.
        ResetCanvas();

        if (this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].Fence != default)
        {
            gl.GetSynciv(this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].Fence, SyncParameterName.SyncStatus, 1, out var _, out var syncStatus);
            if (syncStatus == (int)GLEnum.Unsignaled)
            {
                // If older than 2 frames, wait for sync OpenGL can have up to 3 frames in flight, any more and we need to sync anyway.
                if (this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].LastFrameUsed < RSG.Rasterizer.FrameNumber - 2)
                {
                    #if !WEB_ENABLED
                    // On web, we do nothing as the glSubBufferData will force a sync anyway and WebGL does not like waiting.
                    gl.ClientWaitSync(this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].Fence, default, 100000000); // wait for up to 100ms
                    #endif

                    this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].LastFrameUsed = RSG.Rasterizer.FrameNumber;
                    gl.DeleteSync(this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].Fence);
                    this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].Fence = default;
                }
                else
                {
                    // Used in last frame or frame before that. OpenGL can get up to two frames behind, so these buffers may still be in use
                    // Allocate a new buffer and use that.
                    this.AllocateInstanceDataBuffer();
                }
            }
            else
            {
                // Already finished all rendering commands, we can use it.
                this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].LastFrameUsed = RSG.Rasterizer.FrameNumber;
                gl.DeleteSync(this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].Fence);
                this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].Fence = default;
            }
        }

        //setup directional lights if exist

        var lightCount            = 0;
        var directionalLightCount = 0;

        var l     = directionalLightList;
        var index = 0;

        while (l != null)
        {
            if (index == this.data.MaxLightsPerRender)
            {
                l.RenderIndexCache = -1;
                l = l.Next;
                continue;
            }

            var clight = this.canvasLightOwner.GetOrNull(l.LightInternal);
            if (clight != null)
            {
                //unused or invalid texture
                l.RenderIndexCache = -1;
                l = l.Next;

                if (ERR_CONTINUE(clight == null))
                {
                    continue;
                }
            }

            var canvasLightDir = l!.XformCache[1].Normalized;

            this.state.LightUniforms[index].Position[0] = -canvasLightDir.X;
            this.state.LightUniforms[index].Position[1] = -canvasLightDir.Y;

            fixed (float* shadowMatrixPtr = this.state.LightUniforms[index].ShadowMatrix)
            {
                this.UpdateTransform2DToMat2x4(clight!.Shadow.DirectionalXform, shadowMatrixPtr);
            }

            this.state.LightUniforms[index].Height = l.Height; //0..1 here

            for (var i = 0; i < 4; i++)
            {
                this.state.LightUniforms[index].ShadowColor[i] = (byte)Math.Clamp((int)(l.ShadowColor[i] * 255.0), 0, 255);
                this.state.LightUniforms[index].Color[i]       = l.Color[i];
            }

            this.state.LightUniforms[index].Color[3] = l.Energy; //use alpha for energy, so base color can go separate

            if (this.state.ShadowFb != 0)
            {
                this.state.LightUniforms[index].ShadowPixelSize = 1.0f / this.state.ShadowTextureSize * (1.0f + l.ShadowSmooth);
                this.state.LightUniforms[index].ShadowZFarInv   = 1.0f / clight.Shadow.ZFar;
                this.state.LightUniforms[index].ShadowYOfs      = clight.Shadow.YOffset;
            }
            else
            {
                this.state.LightUniforms[index].ShadowPixelSize = 1.0f;
                this.state.LightUniforms[index].ShadowZFarInv   = 1.0f;
                this.state.LightUniforms[index].ShadowYOfs      = 0;
            }

            this.state.LightUniforms[index].Flags = (uint)l.BlendMode << LIGHT_FLAGS_BLEND_SHIFT;
            this.state.LightUniforms[index].Flags |= (uint)l.ShadowFilter << LIGHT_FLAGS_FILTER_SHIFT;

            if (clight.Shadow.Enabled)
            {
                this.state.LightUniforms[index].Flags |= LIGHT_FLAGS_HAS_SHADOW;
            }

            l.RenderIndexCache = index;

            index++;
            l = l.Next;
        }

        lightCount            = index;
        directionalLightCount = lightCount;

        this.state.UsingDirectionalLights = directionalLightCount > 0;

        //setup lights if exist

        l     = lightList;
        index = lightCount;

        while (l != null)
        {
            if (index == this.data.MaxLightsPerRender)
            {
                l.RenderIndexCache = -1;
                l = l.Next;
                continue;
            }

            var clight = this.canvasLightOwner.GetOrNull(l.LightInternal);
            if (clight != null)
            {
                //unused or invalid texture
                l.RenderIndexCache = -1;
                l = l.Next;
                if (ERR_CONTINUE(clight == null)) // ???
                {
                    continue;
                }
            }

            var canvasLightPos = canvasTransform.Xform(l!.Xform.Origin); //convert light position to canvas coordinates, as all computation is done in canvas coords to avoid precision loss
            this.state.LightUniforms[index].Position[0] = canvasLightPos.X;
            this.state.LightUniforms[index].Position[1] = canvasLightPos.Y;

            fixed (float* matrixPtr = this.state.LightUniforms[index].Matrix)
            fixed (float* shadowMatrixPtr = this.state.LightUniforms[index].ShadowMatrix)
            {

                this.UpdateTransform2DToMat2x4(l.LightShaderXform.AffineInverse(), matrixPtr);
                this.UpdateTransform2DToMat2x4(l.XformCache.AffineInverse(), shadowMatrixPtr);
            }


            this.state.LightUniforms[index].Height = (float)(l.Height * (canvasTransform[0].Length + canvasTransform[1].Length) * 0.5f); //approximate height conversion to the canvas size, since all calculations are done in canvas coords to avoid precision loss
            for (var i = 0; i < 4; i++)
            {
                this.state.LightUniforms[index].ShadowColor[i] = (byte)Math.Clamp((int)(l.ShadowColor[i] * 255.0), 0, 255);
                this.state.LightUniforms[index].Color[i] = l.Color[i];
            }

            this.state.LightUniforms[index].Color[3] = l.Energy; //use alpha for energy, so base color can go separate

            if (this.state.ShadowFb != 0)
            {
                this.state.LightUniforms[index].ShadowPixelSize = (float)(1.0 / this.state.ShadowTextureSize * (1.0 + l.ShadowSmooth));
                this.state.LightUniforms[index].ShadowZFarInv = (float)(1.0 / clight!.Shadow.ZFar);
                this.state.LightUniforms[index].ShadowYOfs = clight.Shadow.YOffset;
            }
            else
            {
                this.state.LightUniforms[index].ShadowPixelSize = 1.0f;
                this.state.LightUniforms[index].ShadowZFarInv = 1.0f;
                this.state.LightUniforms[index].ShadowYOfs = 0f;
            }

            this.state.LightUniforms[index].Flags = (uint)l.BlendMode << LIGHT_FLAGS_BLEND_SHIFT;
            this.state.LightUniforms[index].Flags |= (uint)l.ShadowFilter << LIGHT_FLAGS_FILTER_SHIFT;

            if (clight!.Shadow.Enabled)
            {
                this.state.LightUniforms[index].Flags |= LIGHT_FLAGS_HAS_SHADOW;
            }

            if (clight.Texture != default)
            {
                var atlasRect = TextureStorage.Singleton.TextureAtlasGetTextureRect(clight.Texture);
                this.state.LightUniforms[index].AtlasRect[0] = atlasRect.Position.X;
                this.state.LightUniforms[index].AtlasRect[1] = atlasRect.Position.Y;
                this.state.LightUniforms[index].AtlasRect[2] = atlasRect.Size.X;
                this.state.LightUniforms[index].AtlasRect[3] = atlasRect.Size.Y;

            }
            else
            {
                this.state.LightUniforms[index].AtlasRect[0] = 0;
                this.state.LightUniforms[index].AtlasRect[1] = 0;
                this.state.LightUniforms[index].AtlasRect[2] = 0;
                this.state.LightUniforms[index].AtlasRect[3] = 0;
            }

            l.RenderIndexCache = index;

            index++;
            l = l.Next;
        }

        lightCount = index;

        if (lightCount > 0)
        {
            gl.BindBufferBase(
                BufferTargetARB.UniformBuffer,
                LIGHT_UNIFORM_LOCATION,
                this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].LightUbo
            );

            #if WEB_ENABLED
            // TODO glBufferSubData(GL_UNIFORM_BUFFER, 0, sizeof(LightUniform) * light_count, state.light_uniforms);
            #else
            // On Desktop and mobile we map the memory without synchronizing for maximum speed.
            var ubo = gl.MapBufferRange(BufferTargetARB.UniformBuffer, 0, Marshal.SizeOf<LightUniform>() * lightCount, MapBufferAccessMask.MapWriteBit | MapBufferAccessMask.MapUnsynchronizedBit);

            UnmanagedUtils.Copy(this.state.LightUniforms, ubo, lightCount);

            gl.UnmapBuffer(BufferTargetARB.UniformBuffer);
            #endif

            var textureAtlas = textureStorage.TextureAtlasGetTexture();

            if (textureAtlas == 0)
            {
                var tex = textureStorage.GetTexture(textureStorage.TextureGlGetDefault(DefaultGLTexture.DEFAULT_GL_TEXTURE_WHITE));
                textureAtlas = tex!.TexId;
            }

            gl.ActiveTexture(TextureUnit.Texture0 + Config.Singleton.MaxTextureImageUnits - 2);
            gl.BindTexture(TextureTarget.Texture2D, textureAtlas);

            var shadowTex = this.state.ShadowTexture;

            if (shadowTex == 0)
            {
                var tex = textureStorage.GetTexture(textureStorage.TextureGlGetDefault(DefaultGLTexture.DEFAULT_GL_TEXTURE_WHITE));
                shadowTex = tex!.TexId;
            }

            gl.ActiveTexture(TextureUnit.Texture0 + Config.Singleton.MaxTextureImageUnits - 3);
            gl.BindTexture(TextureTarget.Texture2D, shadowTex);
        }

        //update canvas state uniform buffer
        var stateBuffer = new StateBuffer();

        var ssize = textureStorage.RenderTargetGetSize(toRenderTarget);

        // If we've overridden the render target's color texture, then we need
        // to invert the Y axis, so 2D texture appear right side up.
        // We're probably rendering directly to an XR device.
        var yScale = textureStorage!.RenderTargetGetOverrideColor(toRenderTarget) != default ? -2.0f : 2.0f;

        var screenTransform = new Transform3D<RealT>();
        screenTransform.TranslateLocal(-(ssize.X / 2.0f), -(ssize.Y / 2.0f), 0.0f);
        screenTransform.Scale(new(2.0f / ssize.X, yScale / ssize.Y, 1.0f));

        UpdateTransformToMat4(screenTransform, stateBuffer.ScreenTransform);
        UpdateTransform2DToMat4(canvasTransform, stateBuffer.CanvasTransform);

        var normalTransform = canvasTransform;
        normalTransform[0] = normalTransform[0].Normalized;
        normalTransform[1] = normalTransform[1].Normalized;
        normalTransform[2] = new();

        UpdateTransform2DToMat4(normalTransform, stateBuffer.CanvasNormalTransform);

        stateBuffer.CanvasModulate[0] = modulate.R;
        stateBuffer.CanvasModulate[1] = modulate.G;
        stateBuffer.CanvasModulate[2] = modulate.B;
        stateBuffer.CanvasModulate[3] = modulate.A;

        var renderTargetSize = textureStorage.RenderTargetGetSize(toRenderTarget);
        stateBuffer.ScreenPixelSize[0] = (RealT)1.0 / renderTargetSize.X;
        stateBuffer.ScreenPixelSize[1] = (RealT)1.0 / renderTargetSize.Y;

        stateBuffer.Time         = (float)this.state.Time;
        stateBuffer.UsePixelSnap = Convert.ToUInt32(snap2DVerticesToPixel);

        stateBuffer.DirectionalLightCount = (uint)directionalLightCount;

        var canvasScale = canvasTransform.GetScale();

        stateBuffer.SdfToScreen[0] = renderTargetSize.X / canvasScale.X;
        stateBuffer.SdfToScreen[1] = renderTargetSize.Y / canvasScale.Y;

        stateBuffer.ScreenToSdf[0] = 1.0f / stateBuffer.SdfToScreen[0];
        stateBuffer.ScreenToSdf[1] = 1.0f / stateBuffer.SdfToScreen[1];

        var sdfRect    = textureStorage.RenderTargetGetSdfRect(toRenderTarget).As<RealT>();
        var sdfTexRect = new Rect2<RealT>(sdfRect.Position / canvasScale, sdfRect.Size / canvasScale);

        stateBuffer.SdfToTex[0] = 1.0f / sdfTexRect.Size.X;
        stateBuffer.SdfToTex[1] = 1.0f / sdfTexRect.Size.Y;
        stateBuffer.SdfToTex[2] = -sdfTexRect.Position.X / sdfTexRect.Size.X;
        stateBuffer.SdfToTex[3] = -sdfTexRect.Position.Y / sdfTexRect.Size.Y;

        stateBuffer.TexToSdf = 1.0f / ((canvasScale.X + canvasScale.Y) * 0.5f);

        gl.BindBufferBase(BufferTargetARB.UniformBuffer, BASE_UNIFORM_LOCATION, this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].StateUbo);
        gl.BufferData(BufferTargetARB.UniformBuffer, stateBuffer, BufferUsageARB.StreamDraw);

        var globalBuffer = materialStorage.GlobalShaderParametersGetUniformBuffer();

        gl.BindBufferBase(BufferTargetARB.UniformBuffer, GLOBAL_UNIFORM_LOCATION, globalBuffer);
        gl.BindBuffer(BufferTargetARB.UniformBuffer, 0);

        gl.ActiveTexture(TextureUnit.Texture0 + Config.Singleton.MaxTextureImageUnits - 5);
        gl.BindTexture(TextureTarget.Texture2D, textureStorage.RenderTargetGetSdfTexture(toRenderTarget));

        this.state.DefaultFilter = defaultFilter;
        this.state.DefaultRepeat = defaultRepeat;

        renderTargetSize = textureStorage.RenderTargetGetSize(toRenderTarget);
        gl.Viewport(0, 0, renderTargetSize.X, renderTargetSize.Y);

        sdfUsed = false;
        var itemCount                          = 0;
        var backbufferCleared                  = false;
        var timeUsed                           = false;
        var materialScreenTextureCached        = false;
        var materialScreenTextureMipmapsCached = false;
        var backBufferRect                     = new Rect2<RealT>();
        var backbufferCopy                     = false;
        var backbufferGenMipmaps               = false;
        var updateSkeletons                    = false;

        var ci = itemList;
        var canvasGroupOwner = default(Item);

        this.state.LastItemIndex = 0;

        while (ci != null)
        {
            if (ci.CopyBackBuffer != null && canvasGroupOwner == null)
            {
                backbufferCopy = true;

                backBufferRect = ci!.CopyBackBuffer.Full ? new() : ci.CopyBackBuffer.Rect;
            }

            // Check material for something that may change flow of rendering, but do not bind for now.
            var material = ci.MaterialOwner == null ? ci.Material : ci.MaterialOwner.Material;
            if (material != default)
            {
                var md = (CanvasMaterialData)materialStorage.MaterialGetData(material, RS.ShaderMode.SHADER_CANVAS_ITEM);
                if (md?.ShaderData != null)
                {
                    if (md.ShaderData.UsesScreenTexture && canvasGroupOwner == null)
                    {
                        if (!materialScreenTextureCached)
                        {
                            backbufferCopy = true;
                            backBufferRect = new();
                            backbufferGenMipmaps = md.ShaderData.UsesScreenTextureMipmaps;
                        }
                        else if (!materialScreenTextureMipmapsCached)
                        {
                            backbufferGenMipmaps = md.ShaderData.UsesScreenTextureMipmaps;
                        }
                    }

                    if (md.ShaderData.UsesSdf)
                    {
                        sdfUsed = true;
                    }
                    if (md.ShaderData.UsesTime)
                    {
                        timeUsed = true;
                    }
                }
            }

            if (ci!.Skeleton != default)
            {
                var c = ci.Commands;

                while (c != null)
                {
                    if (c.Type == Item.CommandType.TYPE_MESH)
                    {
                        var cm = (Item.CommandMesh)c!;
                        if (cm.MeshInstance != default)
                        {
                            meshStorage.MeshInstanceCheckForUpdate(cm.MeshInstance);
                            meshStorage.MeshInstanceSetCanvasItemTransform(cm.MeshInstance, canvasTransformInverse * ci.FinalTransform);
                            updateSkeletons = true;
                        }
                    }
                    c = c.Next;
                }
            }

            if (ci.CanvasGroupOwner != null)
            {
                if (canvasGroupOwner == null)
                {
                    if (updateSkeletons)
                    {
                        meshStorage.UpdateMeshInstances();
                        updateSkeletons = false;
                    }
                    // Canvas group begins here, render until before this item
                    this.RenderItems(toRenderTarget, itemCount, canvasTransformInverse, lightList!, out sdfUsed);
                    itemCount = 0;

                    if (ci.CanvasGroupOwner.CanvasGroup!.Mode != RS.CanvasGroupMode.CANVAS_GROUP_MODE_TRANSPARENT)
                    {
                        var groupRect = ci.CanvasGroupOwner.GlobalRectCache;
                        textureStorage.RenderTargetCopyToBackBuffer(toRenderTarget, groupRect, false);
                        if (ci.CanvasGroupOwner.CanvasGroup!.Mode == RS.CanvasGroupMode.CANVAS_GROUP_MODE_CLIP_AND_DRAW)
                        {
                            this.items[itemCount++] = ci.CanvasGroupOwner;
                        }
                    }
                    else if (!backbufferCleared)
                    {
                        textureStorage.RenderTargetClearBackBuffer(toRenderTarget, new(), new(0, 0, 0, 0));
                        backbufferCleared = true;
                    }

                    backbufferCopy = false;
                    canvasGroupOwner = ci.CanvasGroupOwner; //continue until owner found
                }

                ci.CanvasGroupOwner = null; //must be cleared
            }

            if (!backbufferCleared && canvasGroupOwner == null && ci.CanvasGroup != null && !backbufferCopy)
            {
                textureStorage.RenderTargetClearBackBuffer(toRenderTarget, new(), new(0, 0, 0, 0));
                backbufferCleared = true;
            }

            if (ci == canvasGroupOwner)
            {
                if (updateSkeletons)
                {
                    meshStorage.UpdateMeshInstances();
                    updateSkeletons = false;
                }
                this.RenderItems(toRenderTarget, itemCount, canvasTransformInverse, lightList!, out sdfUsed, true);
                itemCount = 0;

                if (ci.CanvasGroup!.BlurMipmaps)
                {
                    textureStorage.RenderTargetGenBackBufferMipmaps(toRenderTarget, ci.GlobalRectCache);
                }

                canvasGroupOwner = null;
                // Backbuffer is dirty now and needs to be re-cleared if another CanvasGroup needs it.
                backbufferCleared = false;
            }

            if (backbufferCopy)
            {
                if (updateSkeletons)
                {
                    meshStorage.UpdateMeshInstances();
                    updateSkeletons = false;
                }
                //render anything pending, including clearing if no items

                this.RenderItems(toRenderTarget, itemCount, canvasTransformInverse, lightList!, out sdfUsed);
                itemCount = 0;

                textureStorage.RenderTargetCopyToBackBuffer(toRenderTarget, backBufferRect, backbufferGenMipmaps);

                backbufferCopy = false;
                backbufferGenMipmaps = false;
                materialScreenTextureCached = true; // After a backbuffer copy, screen texture makes no further copies.
                materialScreenTextureMipmapsCached = backbufferGenMipmaps;
            }

            if (backbufferGenMipmaps)
            {
                textureStorage.RenderTargetGenBackBufferMipmaps(toRenderTarget, backBufferRect);

                backbufferGenMipmaps = false;
                materialScreenTextureMipmapsCached = true;
            }

            // just add all items for now
            this.items[itemCount++] = ci;

            if (ci.Next == null || itemCount == MAX_RENDER_ITEMS - 1)
            {
                if (updateSkeletons)
                {
                    meshStorage.UpdateMeshInstances();
                    updateSkeletons = false;
                }
                this.RenderItems(toRenderTarget, itemCount, canvasTransformInverse, lightList!, out sdfUsed);
                //then reset
                itemCount = 0;
            }

            ci = ci.Next;
        }

        if (timeUsed)
        {
            RenderingServerDefault.RedrawRequest();
        }

        this.state.CanvasInstanceDataBuffers[this.state.CurrentDataBufferIndex].Fence = gl.FenceSync(SyncCondition.SyncGpuCommandsComplete, 0);

        // Clear out state used in 2D pass
        ResetCanvas();
        this.state.CurrentDataBufferIndex = (this.state.CurrentDataBufferIndex + 1) % this.state.CanvasInstanceDataBuffers.Count;
        this.state.CurrentInstanceBufferIndex = 0;
    }

    public override bool Free(Guid id) => throw new NotImplementedException();
    public override void FreePolygon(ulong polygon) => throw new NotImplementedException();
    public override Guid LightCreate() => throw new NotImplementedException();
    public override void LightSetTexture(Guid id, Guid textureId) => throw new NotImplementedException();
    public override void LightSetUseShadow(Guid id, bool enable) => throw new NotImplementedException();
    public override void LightUpdateDirectionalShadow(Guid id, int shadowIndex, in Transform2D<float> lightXform, int lightMask, float cullDistance, in Rect2<float> clipRect, LightOccluderInstance occluders) => throw new NotImplementedException();
    public override void LightUpdateShadow(Guid id, int shadowIndex, in Transform2D<float> lightXform, int lightMask, float near, float far, LightOccluderInstance occluders) => throw new NotImplementedException();
    public override Guid OccluderPolygonCreate() => throw new NotImplementedException();
    public override void OccluderPolygonSetCullMode(Guid occluder, RS.CanvasOccluderPolygonCullMode mode) => throw new NotImplementedException();
    public override void OccluderPolygonSetShape(Guid occluder, Vector2<float>[] points, bool closed) => throw new NotImplementedException();
    public override void RenderSdf(Guid renderTarget, LightOccluderInstance occluders) => throw new NotImplementedException();

    public override ulong RequestPolygon(IList<int> indices, IList<Vector2<float>> points, IList<Color> colors, IList<Vector2<float>>? uvs = null, IList<int>? bones = null, IList<float>? weights = null)
    {
        // We interleave the vertex data into one big VBO to improve cache coherence
        var vertexCount = points.Count;
        var stride      = 2;

        if (colors.Count == vertexCount)
        {
            stride += 4;
        }
        if (uvs?.Count == vertexCount)
        {
            stride += 2;
        }
        if (bones?.Count == vertexCount * 4 && weights?.Count == vertexCount * 4)
        {
            stride += 4;
        }

        var gl = GL.Singleton;

        var pb = new PolygonBuffers();
        gl.GenBuffers(out var vertexBuffer);

        pb.VertexBuffer = vertexBuffer;

        gl.GenVertexArrays(out var vertexArray);

        pb.VertexArray = vertexArray;

        gl.BindVertexArray(pb.VertexArray);

        pb.Count       = vertexCount;
        pb.IndexBuffer = 0;

        var bufferSize = stride * points.Count;

        // | points   | colors        | uvs      | bones          | weights        |
        // | float[2] | float[4]      | float[2] | ushort[4]      | ushort[4]      |
        // | 0, 4     | 8, 12, 16, 20 | 24, 28   | 32, 34, 36, 38 | 40, 42, 44, 46 |

        var polygonBuffer = new byte[bufferSize * FLOAT_SIZE];

        {
            gl.BindBuffer(BufferTargetARB.ArrayBuffer, pb.VertexBuffer);

            var baseOffset = 0;

            {
                // Always uses vertex positions
                gl.EnableVertexAttribArray((int)RS.ArrayType.ARRAY_VERTEX);
                gl.VertexAttribPointer((int)RS.ArrayType.ARRAY_VERTEX, 2, VertexAttribPointerType.Float, false, stride * FLOAT_SIZE, default);

                for (var i = 0; i < vertexCount; i++)
                {
                    var offset = baseOffset * FLOAT_SIZE + i * stride * FLOAT_SIZE;

                    Array.Copy(BitConverter.GetBytes(points[i].X), 0, polygonBuffer, offset + FLOAT_SIZE * 0, FLOAT_SIZE);
                    Array.Copy(BitConverter.GetBytes(points[i].Y), 0, polygonBuffer, offset + FLOAT_SIZE * 1, FLOAT_SIZE);
                }

                baseOffset += 2;
            }

            // Next add colors
            if (colors.Count == vertexCount)
            {
                gl.EnableVertexAttribArray((int)RS.ArrayType.ARRAY_COLOR);
                gl.VertexAttribPointer((int)RS.ArrayType.ARRAY_COLOR, 4, VertexAttribPointerType.Float, false, stride * FLOAT_SIZE, baseOffset * FLOAT_SIZE);

                for (var i = 0; i < vertexCount; i++)
                {
                    var offset = baseOffset * FLOAT_SIZE + i * stride * FLOAT_SIZE;

                    Array.Copy(BitConverter.GetBytes(colors[i].R), 0, polygonBuffer, offset + FLOAT_SIZE * 0, FLOAT_SIZE);
                    Array.Copy(BitConverter.GetBytes(colors[i].G), 0, polygonBuffer, offset + FLOAT_SIZE * 1, FLOAT_SIZE);
                    Array.Copy(BitConverter.GetBytes(colors[i].B), 0, polygonBuffer, offset + FLOAT_SIZE * 2, FLOAT_SIZE);
                    Array.Copy(BitConverter.GetBytes(colors[i].A), 0, polygonBuffer, offset + FLOAT_SIZE * 3, FLOAT_SIZE);
                }

                baseOffset += 4;
            }
            else
            {
                gl.DisableVertexAttribArray((int)RS.ArrayType.ARRAY_COLOR);
                pb.ColorDisabled = true;
                pb.Color = colors.Count == 1 ? colors[0] : new(1, 1, 1, 1);
            }

            if (uvs?.Count == vertexCount)
            {
                gl.EnableVertexAttribArray((int)RS.ArrayType.ARRAY_TEX_UV);
                gl.VertexAttribPointer((int)RS.ArrayType.ARRAY_TEX_UV, 2, VertexAttribPointerType.Float, false, stride * FLOAT_SIZE, baseOffset * FLOAT_SIZE);

                for (var i = 0; i < vertexCount; i++)
                {
                    var offset = baseOffset * FLOAT_SIZE + i * stride * FLOAT_SIZE;

                    Array.Copy(BitConverter.GetBytes(uvs[i].X), 0, polygonBuffer, offset + FLOAT_SIZE * 0, FLOAT_SIZE);
                    Array.Copy(BitConverter.GetBytes(uvs[i].Y), 0, polygonBuffer, offset + FLOAT_SIZE * 1, FLOAT_SIZE);
                }

                baseOffset += 2;
            }
            else
            {
                gl.DisableVertexAttribArray((int)RS.ArrayType.ARRAY_TEX_UV);
            }

            if (indices.Count == vertexCount * 4 && weights?.Count == vertexCount * 4)
            {
                gl.EnableVertexAttribArray((int)RS.ArrayType.ARRAY_BONES);
                gl.VertexAttribPointer((int)RS.ArrayType.ARRAY_BONES, 4, VertexAttribPointerType.UnsignedInt, false, stride * FLOAT_SIZE, baseOffset * FLOAT_SIZE);

                for (var i = 0; i < vertexCount; i++)
                {
                    var offset = baseOffset * FLOAT_SIZE + i * stride * FLOAT_SIZE;

                    Array.Copy(BitConverter.GetBytes((ushort)bones![i * 4 + 0]), 0, polygonBuffer, offset + USHORT_SIZE * 0, USHORT_SIZE);
                    Array.Copy(BitConverter.GetBytes((ushort)bones![i * 4 + 1]), 0, polygonBuffer, offset + USHORT_SIZE * 0, USHORT_SIZE);
                    Array.Copy(BitConverter.GetBytes((ushort)bones![i * 4 + 2]), 0, polygonBuffer, offset + USHORT_SIZE * 0, USHORT_SIZE);
                    Array.Copy(BitConverter.GetBytes((ushort)bones![i * 4 + 3]), 0, polygonBuffer, offset + USHORT_SIZE * 0, USHORT_SIZE);
                }

                baseOffset += 2;
            }
            else
            {
                gl.DisableVertexAttribArray((int)RS.ArrayType.ARRAY_BONES);
            }

            if (weights?.Count == vertexCount * 4)
            {
                gl.EnableVertexAttribArray((int)RS.ArrayType.ARRAY_WEIGHTS);
                gl.VertexAttribPointer((int)RS.ArrayType.ARRAY_WEIGHTS, 4, VertexAttribPointerType.Float, false, stride * FLOAT_SIZE, baseOffset * FLOAT_SIZE);

                for (var i = 0; i < vertexCount; i++)
                {
                    var offset = baseOffset * FLOAT_SIZE + i * stride * FLOAT_SIZE;

                    Array.Copy(BitConverter.GetBytes((ushort)Math.Clamp(weights[i * 4 + 0] * 65535, 0, 65535)), 0, polygonBuffer, offset + USHORT_SIZE * 0, USHORT_SIZE);
                    Array.Copy(BitConverter.GetBytes((ushort)Math.Clamp(weights[i * 4 + 1] * 65535, 0, 65535)), 0, polygonBuffer, offset + USHORT_SIZE * 0, USHORT_SIZE);
                    Array.Copy(BitConverter.GetBytes((ushort)Math.Clamp(weights[i * 4 + 2] * 65535, 0, 65535)), 0, polygonBuffer, offset + USHORT_SIZE * 0, USHORT_SIZE);
                    Array.Copy(BitConverter.GetBytes((ushort)Math.Clamp(weights[i * 4 + 3] * 65535, 0, 65535)), 0, polygonBuffer, offset + USHORT_SIZE * 0, USHORT_SIZE);
                }

                baseOffset += 2;
            }
            else
            {
                gl.DisableVertexAttribArray((int)RS.ArrayType.ARRAY_WEIGHTS);
            }

            if (ERR_FAIL_COND_V(baseOffset != stride))
            {
                return 0;
            }

            gl.BufferData(BufferTargetARB.ArrayBuffer, polygonBuffer, BufferUsageARB.StaticDraw);
        }

        if (indices?.Count > 0)
        {
            //create indices, as indices were requested
            // var indexBuffer = new byte[indices.Count * sizeof(int)];
            var indexBuffer = indices.SelectMany(BitConverter.GetBytes).ToArray();

            gl.GenBuffers(out var outIndexBuffer);
            pb.IndexBuffer = outIndexBuffer;

            gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, pb.IndexBuffer);
            gl.BufferData(BufferTargetARB.ElementArrayBuffer, indexBuffer, BufferUsageARB.StaticDraw);

            pb.Count = indices.Count;
        }

        gl.BindVertexArray(0);
        gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
        gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);

        var id = this.polygonBuffers.LastId++;

        this.polygonBuffers.Polygons[id] = pb;

        return id;
    }

    public override void SetShadowTextureSize(int size) => throw new NotImplementedException();
    public override void Update() { /* NOOP */ }
    #endregion public overrided methods
}
