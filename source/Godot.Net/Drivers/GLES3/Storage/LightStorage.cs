namespace Godot.Net.Drivers.GLES3.Storage;

using System;
using Godot.Net.Servers.Rendering.Storage;

public partial class LightStorage : RendererLightStorage
{
    // TODO
    // public LightStorage() => throw new NotImplementedException();

    public override Guid ReflectionAtlasCreate() => default;

    public override Guid ShadowAtlasCreate() => default;

    public override void ShadowAtlasSetQuadrantSubdivision(Guid atlas, int quadrant, int subdivision)
    {
        // Do Nothing
    }

    public override void ShadowAtlasSetSize(Guid shadowAtlas, int shadowAtlasSize, bool shadowAtlas16Bits)
    {
        // Do Nothing
    }
}
