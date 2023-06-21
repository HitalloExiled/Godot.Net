namespace Godot.Net.Servers.Rendering.Storage;

using Godot.Net.Core.IO;
using Godot.Net.Core.Math;

public abstract partial class RendererTextureStorage
{
    public Color DefaultClearColor { get; set; }

    public abstract bool CanCreateResourcesAsync();
    public abstract Guid CanvasTextureAllocate();
    public abstract void CanvasTextureFree(Guid id);
    public abstract void CanvasTextureInitialize(Guid id);
    public abstract void CanvasTextureSetChannel(Guid canvasTexture, RS.CanvasTextureChannel channel, Guid textureId);
    public abstract void CanvasTextureSetShadingParameters(Guid canvasTexture, in Color baseColor, float shininess);
    public abstract void CanvasTextureSetTextureFilter(Guid item, RS.CanvasItemTextureFilter filter);
    public abstract void CanvasTextureSetTextureRepeat(Guid item, RS.CanvasItemTextureRepeat repeat);
    public abstract Guid DecalAllocate();
    public abstract void DecalFree(Guid id);
    public abstract AABB DecalGetAabb(Guid decal);
    public abstract uint DecalGetCullMask(Guid decal);
    public abstract void DecalInitialize(Guid id);
    public abstract Guid DecalInstanceCreate(Guid decal);
    public abstract void DecalInstanceFree(Guid decalInstance);
    public abstract void DecalInstanceSetSortingOffset(Guid decalInstance, float sortingOffset);    /* RENDER TARGET */
    public abstract void DecalInstanceSetTransform(Guid decalInstance, in Transform3D<RealT> transform);
    public abstract void DecalSetAlbedoMix(Guid decal, float mix);
    public abstract void DecalSetCullMask(Guid decal, uint layers);
    public abstract void DecalSetDistanceFade(Guid decal, bool enabled, float begin, float length);
    public abstract void DecalSetEmissionEnergy(Guid decal, float energy);
    public abstract void DecalSetFade(Guid decal, float above, float below);
    public abstract void DecalSetModulate(Guid decal, in Color modulate);
    public abstract void DecalSetNormalFade(Guid decal, float fade);
    public abstract void DecalSetSize(Guid decal, in Vector3<RealT> size);
    public abstract void DecalSetTexture(Guid decal, RS.DecalTexture type, Guid textureId);
    public abstract Guid RenderTargetCreate();
    public abstract void RenderTargetDisableClearRequest(Guid renderTargetId);
    public abstract void RenderTargetDoClearRequest(Guid renderTargetId);
    public abstract void RenderTargetFree(Guid renderTargetId);
    public abstract Color RenderTargetGetClearRequestColor(Guid renderTargetId);
    public abstract bool RenderTargetGetDirectToScreen(Guid renderTargetId);
    public abstract RS.ViewportMSAA RenderTargetGetMsaa(Guid renderTargetId);
    public abstract Guid RenderTargetGetOverrideColor(Guid renderTargetId);
    public abstract Guid RenderTargetGetOverrideDepth(Guid renderTargetId);
    public abstract Guid RenderTargetGetOverrideVelocity(Guid renderTargetId);    // get textures
    public abstract Vector2<int> RenderTargetGetPosition(Guid renderTargetId);
    public abstract Rect2<int> RenderTargetGetSdfRect(Guid renderTargetId);
    public abstract Vector2<int> RenderTargetGetSize(Guid renderTargetId);
    public abstract Guid RenderTargetGetTexture(Guid renderTargetId);
    public abstract bool RenderTargetGetTransparent(Guid renderTargetId);
    public abstract RS.ViewportVRSMode RenderTargetGetVrsMode(Guid renderTargetId);
    public abstract Guid RenderTargetGetVrsTexture(Guid renderTargetId);  // Override color, depth and velocity buffers (depth and velocity only for 3D)
    public abstract bool RenderTargetIsClearRequested(Guid renderTargetId);
    public abstract bool RenderTargetIsSdfEnabled(Guid renderTargetId);
    public abstract void RenderTargetMarkSdfEnabled(Guid renderTargetId, bool enabled);
    public abstract void RenderTargetRequestClear(Guid renderTargetId, in Color color = default);
    public abstract void RenderTargetSetAsUnused(Guid renderTargetId);
    public abstract void RenderTargetSetDirectToScreen(Guid renderTargetId, bool directToScreen);
    public abstract void RenderTargetSetMsaa(Guid renderTargetId, RS.ViewportMSAA msaa);
    public abstract void RenderTargetSetOverride(Guid renderTargetId, Guid colorTexture, Guid depthTexture, Guid velocityTexture);
    public abstract void RenderTargetSetPosition(Guid renderTargetId, int x, int y); // Q change input to Point2i &position ?
    public abstract void RenderTargetSetSdfSizeAndScale(Guid renderTargetId, RS.ViewportSDFOversize sdfOversize, RS.ViewportSDFScale sdfScale);
    public abstract void RenderTargetSetSize(Guid renderTargetId, int width, int height, uint viewCount); // Q change input to Size2i &size ?
    public abstract void RenderTargetSetTransparent(Guid renderTargetId, bool isTransparent);
    public abstract void RenderTargetSetVrsMode(Guid renderTargetId, RS.ViewportVRSMode mode);
    public abstract void RenderTargetSetVrsTexture(Guid renderTargetId, Guid textureId);
    public abstract bool RenderTargetWasUsed(Guid renderTargetId);
    public abstract Image Texture2DGet(Guid textureId);
    public abstract void Texture2DInitialize(Guid textureId, Image image);
    public abstract void Texture2DLayeredInitialize(Guid textureId, IList<Image> layers, RS.TextureLayeredType layeredType);
    public abstract void Texture2DLayeredPlaceholderInitialize(Guid textureId, RS.TextureLayeredType layeredType);
    public abstract Image Texture2DLayerGet(Guid textureId, int layer);
    public abstract void Texture2DPlaceholderInitialize(Guid textureId);
    public abstract void Texture2DUpdate(Guid textureId, Image image, int layer = 0);
    public abstract Image[] Texture3DGet(Guid textureId);
    public abstract void Texture3DInitialize(Guid textureId, ImageFormat format, int width, int height, int depth, bool mipmaps, IList<Image> data);
    public abstract void Texture3DPlaceholderInitialize(Guid textureId);
    public abstract void Texture3DUpdate(Guid textureId, Image[] data);
    public abstract void TextureAddToDecalAtlas(Guid textureId, bool panoramaToDp = false);
    public abstract Guid TextureAllocate();
    public abstract void TextureDebugUsage(out List<RS.TextureInfo> info);
    public abstract void TextureFree(Guid textureId);
    public abstract string TextureGetPath(Guid textureId);
    public abstract Guid TextureGetRdTextureGuid(Guid textureId, bool srgb = false);  /* Decal API */
    public abstract void TextureProxyInitialize(Guid textureId, Guid baseId); //all slices, then all the mipmaps, must be coherent
    public abstract void TextureProxyUpdate(Guid proxy, Guid @base);    //these two APIs can be used together or in combination with the others.
    public abstract void TextureRemoveFromDecalAtlas(Guid textureId, bool panoramaToDp = false);  /* DECAL INSTANCE */
    public abstract void TextureReplace(Guid textureId, Guid byTexture);
    public abstract void TextureSetDetect3DCallback(Guid textureId, RS.TextureDetectCallback callback, object? userdata);
    public abstract void TextureSetDetectNormalCallback(Guid textureId, RS.TextureDetectCallback callback, object? userdata);
    public abstract void TextureSetDetectRoughnessCallback(Guid textureId, RS.TextureDetectRoughnessCallback callback, object? userdata);
    public abstract void TextureSetForceRedrawIfVisible(Guid textureId, bool enable);
    public abstract void TextureSetPath(Guid textureId, string path);
    public abstract void TextureSetSizeOverride(Guid textureId, int width, int height);
    public abstract Vector2<RealT> TextureSizeWithProxy(Guid textureId);
}
