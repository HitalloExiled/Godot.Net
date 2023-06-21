namespace Godot.Net.Servers.Rendering;

using Godot.Net.Core.Error;
using Godot.Net.Core.Math;

#pragma warning disable IDE0051, IDE0044, IDE0060, CS0169  // TODO Remove

public abstract partial class RenderingDevice
{
    public delegate string ShaderSPIRVGetCacheKeyFunction(RenderingDevice renderDevice);
    public delegate byte[] ShaderCompileToSPIRVFunction(ShaderStage stage, string sourceCode, ShaderLanguage language, out string error, RenderingDevice renderDevice);
    public delegate byte[] ShaderCacheFunction(ShaderStage stage, string sourceCode, ShaderLanguage language);
    public delegate void InvalidationCallback(object? value);

    public const int INVALID_ID        = -1;
    public const int INVALID_FORMAT_ID = -1;

    protected const uint MAX_UNIFORM_SETS = 16U;

    private static ShaderCompileToSPIRVFunction?   compileToSpirvFunction;
    private static ShaderCacheFunction?            cacheFunction;
    private static ShaderSPIRVGetCacheKeyFunction? getSpirvCacheKeyFunction;

    protected static string[] ShaderStageNames { get; } = new string[(int)ShaderStage.SHADER_STAGE_MAX];

    public static RenderingDevice Singleton { get; internal set; } = null!;

    protected Capabilities? DeviceCapabilities { get; set; }

    public RenderingDevice() => throw new NotImplementedException();

    protected Error BufferUpdate(Guid buffer, uint offset, uint size, byte[] data, BarrierMask postBarrier = BarrierMask.BARRIER_MASK_ALL_BARRIERS) => throw new NotImplementedException();
    protected void ComputeListSetPushConstant(long list, byte[] data, uint dataSize) => throw new NotImplementedException();
    protected Guid ComputePipelineCreate(Guid shader, RDPipelineSpecializationConstant[] specializationConstants) => throw new NotImplementedException();
    protected long[] DrawListBeginSplit(Guid framebuffer, uint splits, InitialAction initialColorAction, FinalAction finalColorAction, InitialAction initialDepthAction, FinalAction finalDepthAction, Color[]? clearColorValues = default, float clearDepth = 1.0f, uint clearStencil = 0, Rect2<RealT> region = default, Guid[]? storageTextures = default) => throw new NotImplementedException();
    protected void DrawListSetPushConstant(long list, byte[] data, uint dataSize) => throw new NotImplementedException();
    protected long[] DrawListSwitchToNextPassSplit(uint splits) => throw new NotImplementedException();
    protected Guid FramebufferCreateInternal(Guid[] textures, long formatCheck = INVALID_ID, uint viewCount = 1) => throw new NotImplementedException();
    protected Guid FramebufferCreateMultipass(Guid[] textures, RDFramebufferPass[] passes, long formatCheck = INVALID_ID, uint viewCount = 1) => throw new NotImplementedException();
    protected long FramebufferFormatCreate(RDAttachmentFormat[] attachments, uint viewCount) => throw new NotImplementedException();
    protected long FramebufferFormatCreateMultipass(RDAttachmentFormat[] attachments, RDFramebufferPass[] passes, uint viewCount) => throw new NotImplementedException();
    protected Error ReflectSpirv(ShaderStageSPIRVData[] spirv, out SpirvReflectionData reflectionData) => throw new NotImplementedException();
    protected Guid RenderPipelineCreate(Guid shader, long framebufferFormat, long vertexFormat, RenderPrimitive renderPrimitive, RDPipelineRasterizationState rasterizationState, RDPipelineMultisampleState multisampleState, RDPipelineDepthStencilState depthStencilState, RDPipelineColorBlendState blendState, PipelineDynamicStateFlags dynamicStateFlags, uint forRenderPass, RDPipelineSpecializationConstant[] specializationConstants) => throw new NotImplementedException();
    protected Guid SamplerCreate(RDSamplerState state) => throw new NotImplementedException();
    protected byte[] ShaderCompileBinaryFromSpirv(RDShaderSPIRV bytecode, string shaderName = "") => throw new NotImplementedException();
    protected RDShaderSPIRV ShaderCompileSpirvFromSource(RDShaderSource source, bool allowCache = true) => throw new NotImplementedException();
    protected Guid ShaderCreateFromSpirv(RDShaderSPIRV spirv, string shaderName = "") => throw new NotImplementedException();
    protected Guid TextureCreate(RDTextureFormat format, RDTextureView view, byte[]? data = default) => throw new NotImplementedException();
    protected Guid TextureCreateShared(RDTextureView view, Guid withTexture) => throw new NotImplementedException();
    protected Guid TextureCreateSharedFromSlice(RDTextureView view, Guid withTexture, uint layer, uint mipmap, uint mipmaps = 1, TextureSliceType sliceType = TextureSliceType.TEXTURE_SLICE_2D) => throw new NotImplementedException();
    protected Guid UniformSetCreate(RDUniform[] uniforms, Guid shader, uint shaderSet) => throw new NotImplementedException();
    protected Guid VertexArrayCreate(uint vertexCount, long vertexFormat,  Guid[] srcBuffers, long[]? offsets = default) => throw new NotImplementedException();
    protected long VertexFormatCreate(RDVertexAttribute[] vertexFormats) => throw new NotImplementedException();

    public static void ShaderSetCompileToSpirvFunction(ShaderCompileToSPIRVFunction function) => throw new NotImplementedException();
    public static void ShaderSetSpirvCacheFunction(ShaderCacheFunction function) => throw new NotImplementedException();
    public static void ShaderSetGetCacheKeyFunction(ShaderSPIRVGetCacheKeyFunction function) => throw new NotImplementedException();

    public abstract void Barrier(BarrierMask from = BarrierMask.BARRIER_MASK_ALL_BARRIERS, BarrierMask to = BarrierMask.BARRIER_MASK_ALL_BARRIERS);
    public abstract Error BufferClear(Guid buffer, uint offset, uint size, BarrierMask postBarrier = BarrierMask.BARRIER_MASK_ALL_BARRIERS);
    public abstract byte[] BufferGetData(Guid buffer, uint offset = 0, uint size = 0); // This causes stall, only use to retrieve large buffers for sa;
    public abstract Error BufferUpdate(Guid buffer, uint offset, uint size, object? data, BarrierMask postBarrier = BarrierMask.BARRIER_MASK_ALL_BARRIERS);
    public abstract void CaptureTimestamp(string name);
    public abstract void ComputeListAddBarrier(long list);
    public abstract long ComputeListBegin(bool allowDrawOverlap = false);
    public abstract void ComputeListBindComputePipeline(long list, Guid computePipeline);
    public abstract void ComputeListBindUniformSet(long list, Guid uniformSet, uint index);
    public abstract void ComputeListDispatch(long list, uint xGroups, uint yGroups, uint zGroups);
    public abstract void ComputeListDispatchIndirect(long list, Guid buffer, uint offset);
    public abstract void ComputeListDispatchThreads(long list, uint xThreads, uint yThreads, uint zThreads);
    public abstract void ComputeListEnd(BarrierMask postBarrier = BarrierMask.BARRIER_MASK_ALL_BARRIERS);
    public abstract void ComputeListSetPushConstant(long list, object? data, uint dataSize);
    public abstract Guid ComputePipelineCreate(Guid shader, PipelineSpecializationConstant[]? specializationConstants = default);
    public abstract bool ComputePipelineIsValid(Guid pipeline);
    public abstract RenderingDevice CreateLocalDevice();
    public abstract void DrawCommandBeginLabel(string labelName, Color color = default);
    public abstract void DrawCommandEndLabel();
    public abstract void DrawCommandInsertLabel(string labelName, Color color = default);
    public abstract long DrawListBegin(Guid framebuffer, InitialAction initialColorAction, FinalAction finalColorAction, InitialAction initialDepthAction, FinalAction finalDepthAction, Color[]? clearColorValues = default, float clearDepth = 1.0f, uint clearStencil = 0, Rect2<RealT> region = default, Guid[]? storageTextures = default);
    public abstract long DrawListBeginForScreen(int screen = 0, Color clearColor = default);
    public abstract Error DrawListBeginSplit(Guid framebuffer, uint splits, out long splitIds, InitialAction initialColorAction, FinalAction finalColorAction, InitialAction initialDepthAction, FinalAction finalDepthAction, Color[]? clearColorValues = default, float clearDepth = 1.0f, uint clearStencil = 0, Rect2<RealT> region = default, Guid[]? storageTextures = default);
    public abstract void DrawListBindIndexArray(long list, Guid indexArray);
    public abstract void DrawListBindRenderPipeline(long list, Guid renderPipeline);
    public abstract void DrawListBindUniformSet(long list, Guid uniformSet, uint index);
    public abstract void DrawListBindVertexArray(long list, Guid vertexArray);
    public abstract void DrawListDisableScissor(long list);
    public abstract void DrawListDraw(long list, bool useIndices, uint instances = 1, uint proceduralVertices = 0);
    public abstract void DrawListEnableScissor(long list, Rect2<RealT> rect);
    public abstract void DrawListEnd(BarrierMask postBarrier = BarrierMask.BARRIER_MASK_ALL_BARRIERS);
    public abstract uint DrawListGetCurrentPass();
    public abstract void DrawListSetBlendConstants(long list, Color color);
    public abstract void DrawListSetLineWidth(long list, float width);
    public abstract void DrawListSetPushConstant(long list, object? data, uint dataSize);
    public abstract long DrawListSwitchToNextPass();
    public abstract Error DrawListSwitchToNextPassSplit(uint splits, out long splitIds);
    public abstract long FramebufferFormatCreate(AttachmentFormat[] format, uint viewCount = 1);
    public abstract long FramebufferFormatCreateEmpty(TextureSamples samples = TextureSamples.TEXTURE_SAMPLES_1);
    public abstract long FramebufferFormatCreateMultipass(AttachmentFormat[] attachments, FramebufferPass[] passes, uint viewCount = 1);
    public abstract TextureSamples FramebufferFormatGetTextureSamples(long format, uint pass = 0);
    public abstract long FramebufferGetFormat(Guid framebuffer);
    public abstract bool FramebufferIsValid(Guid framebuffer);
    public abstract void Free(Guid id);
    public abstract void FullBarrier();
    public abstract ulong GetCapturedTimestamcpuTime(uint index);
    public abstract ulong GetCapturedTimestamgpuTime(uint index);
    public abstract string GetCapturedTimestamname(uint index);
    public abstract uint GetCapturedTimestampsCount();
    public abstract ulong GetCapturedTimestampsFrame();
    public abstract string GetDeviceApiVersion();
    public abstract string GetDeviceName();
    public abstract string GetDevicePipelineCacheUuid();
    public abstract DeviceType GetDeviceType();
    public abstract string GetDeviceVendorName();
    public abstract ulong GetDriverResource(DriverResource resource, Guid id = default, ulong index = 0);
    public abstract uint GetFrameDelay();
    public abstract ulong GetMemoryUsage(MemoryType type);
    public abstract bool HasFeature(Features feature);
    public abstract Guid IndexArrayCreate(Guid indexBuffer, uint indexOffset, uint indexCount);
    public abstract Guid IndexBufferCreate(uint sizeIndices, IndexBufferFormat format, byte[]? data = default, bool useRestartIndices = false);
    public abstract ulong LimitGet(Limit limit);
    public abstract void PrepareScreenForDrawing();
    public abstract Guid RenderPipelineCreate(Guid shader, long framebufferFormat, long vertexFormat, RenderPrimitive renderPrimitive, PipelineRasterizationState rasterizationState, PipelineMultisampleState multisampleState, PipelineDepthStencilState depthStencilState, PipelineColorBlendState blendState, PipelineDynamicStateFlags dynamicStateFlags = default, uint forRenderPass = 0, PipelineSpecializationConstant[]? specializationConstants = default);
    public abstract bool RenderPipelineIsValid(Guid pipeline);
    public abstract Guid SamplerCreate(SamplerState state);
    public abstract long ScreenGetFramebufferFormat();
    public abstract int ScreenGetHeight(int screen = 0);
    public abstract int ScreenGetWidth(int screen = 0);
    public abstract void SetResourceName(Guid id, string name);
    public abstract byte[] ShaderCompileBinaryFromSpirv(ShaderStageSPIRVData[] spirv, string shaderName = "");
    public abstract Guid ShaderCreateFromBytecode(byte[] shaderBinary);
    public abstract string ShaderGetBinaryCacheKey();
    public abstract uint ShaderGetVertexInputAttributeMask(Guid shader);
    public abstract Guid StorageBufferCreate(uint size, byte[]? data = default, StorageBufferUsage usage = 0);
    public abstract void Submit();
    public abstract void Swabuffers();
    public abstract void Sync();
    public abstract Guid TextureBufferCreate(uint sizeElements, DataFormat format, byte[]? data = default);
    public abstract Error TextureClear(Guid texture, Color color, uint baseMipmap, uint mipmaps, uint baseLayer, uint layers, BarrierMask postBarrier = BarrierMask.BARRIER_MASK_ALL_BARRIERS);
    public abstract Error TextureCopy(Guid fromTexture, Guid toTexture, Vector3<RealT> from, Vector3<RealT> to, Vector3<RealT> size, uint srcMipmap, uint dstMipmap, uint srcLayer, uint dstLayer, BarrierMask postBarrier = BarrierMask.BARRIER_MASK_ALL_BARRIERS);
    public abstract Guid TextureCreate(TextureFormat format, TextureView view, byte[][]? data = default);
    public abstract Guid TextureCreateFromExtension(TextureType type, DataFormat format, TextureSamples samples, ulong flags, ulong image, ulong width, ulong height, ulong depth, ulong layers);
    public abstract Guid TextureCreateShared(TextureView view, Guid withTexture);
    public abstract Guid TextureCreateSharedFromSlice(TextureView view, Guid withTexture, uint layer, uint mipmap, uint mipmaps = 1, TextureSliceType sliceType = TextureSliceType.TEXTURE_SLICE_2D, uint layers = 0);
    public abstract byte[] TextureGetData(Guid texture, uint layer); // CPU textures will return immediately, while GPU textures will most likely force a ;
    public abstract bool TextureIsFormatSupportedForUsage(DataFormat format, TextureUsageBits usage);
    public abstract bool TextureIsShared(Guid texture);
    public abstract bool TextureIsValid(Guid texture);
    public abstract Error TextureResolveMultisample(Guid fromTexture, Guid toTexture, BarrierMask postBarrier = BarrierMask.BARRIER_MASK_ALL_BARRIERS);
    public abstract Vector2<int> TextureSize(Guid texture);
    public abstract Error TextureUpdate(Guid texture, uint layer, byte[] data, BarrierMask postBarrier = BarrierMask.BARRIER_MASK_ALL_BARRIERS);
    public abstract Guid UniformBufferCreate(uint sizeBytes, byte[]? data = default);
    public abstract Guid UniformSetCreate(Uniform[] uniforms, Guid shader, uint shaderSet);
    public abstract bool UniformSetIsValid(Guid uniformSet);
    public abstract void UniformSetSetInvalidationCallback(Guid uniformSet, InvalidationCallback callback, object? userdata);
    public abstract Guid VertexArrayCreate(uint vertexCount, long vertexFormat, Guid[] srcbuffers, ulong[]? offsets = default);
    public abstract Guid VertexBufferCreate(uint sizeBytes, byte[]? data = default, bool useAsStorage = false);
    public abstract long VertexFormatCreate(VertexAttribute[] vertexFormats);

    // framebufferCreate
    public virtual Guid FramebufferCreate(Guid[] textureAttachments, long formatCheck = INVALID_ID, uint viewCount = 1) => throw new NotImplementedException();
    public virtual Guid FramebufferCreateEmpty(Vector2<int> size, TextureSamples samples = TextureSamples.TEXTURE_SAMPLES_1, long formatCheck = INVALID_ID) => throw new NotImplementedException();
    public virtual Guid FramebufferCreateMultipass(Guid[] textureAttachments, FramebufferPass[] passes, long formatCheck = INVALID_ID, uint viewCount = 1) => throw new NotImplementedException();
    public virtual void FramebufferSetInvalidationCallback(Guid framebuffer, InvalidationCallback callback, object? userdata) => throw new NotImplementedException();
    public virtual byte[] ShaderCompileSpirvFromSource(ShaderStage stage, string sourceCode, out string error, ShaderLanguage language = ShaderLanguage.SHADER_LANGUAGE_GLSL, bool allowCache = true) => throw new NotImplementedException();
    public virtual Guid ShaderCreateFromSpirv(ShaderStageSPIRVData[] spirv, string shaderName = "") => throw new NotImplementedException();
    public virtual string ShaderGetSpirvCacheKey() => throw new NotImplementedException();
}
