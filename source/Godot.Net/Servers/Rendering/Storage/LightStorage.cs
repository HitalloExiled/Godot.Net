namespace Godot.Net.Servers.Rendering.Storage;

// Filename and type are different
public abstract class RendererLightStorage
{
    public abstract Guid ReflectionAtlasCreate();
    public abstract Guid ShadowAtlasCreate();
    public abstract void ShadowAtlasSetQuadrantSubdivision(Guid atlas, int quadrant, int subdivision);
    public abstract void ShadowAtlasSetSize(Guid shadowAtlas, int shadowAtlasSize, bool shadowAtlas16Bits = default);
}
