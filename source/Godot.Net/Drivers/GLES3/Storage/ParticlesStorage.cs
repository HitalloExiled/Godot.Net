namespace Godot.Net.Drivers.GLES3.Storage;

using Godot.Net.Core.Math;
using Godot.Net.Servers.Rendering.Storage;

#pragma warning disable IDE0044, CS0649 // TODO Remove;

public class ParticlesStorage : RendererParticlesStorage
{
    private static ParticlesStorage? singleton;
    public static ParticlesStorage Singleton => singleton ?? throw new NullReferenceException();

    // TODO
    // public ParticlesStorage() => throw new NotImplementedException();

    public int ParticlesGetAmount(Guid particles) => throw new NotImplementedException();
    public uint ParticlesGetGlBuffer(Guid particles) => throw new NotImplementedException();
    public RS.ParticlesMode ParticlesGetMode(Guid particles) => throw new NotImplementedException();

    public bool ParticlesHasCollision(Guid particles) => throw new NotImplementedException();

    public void ParticlesSetCanvasSdfCollision(
        Guid                  particles,
        bool                  enable,
        in Transform2D<RealT> xform,
        in Rect2<RealT>       toScreen,
        uint                  texture
    ) => throw new NotImplementedException();

    public override int ParticlesGetDrawPasses(Guid particles) => throw new NotImplementedException();
    public override Guid ParticlesGetDrawPassMesh(Guid particles, int pass) => throw new NotImplementedException();
    public override bool ParticlesIsInactive(Guid particles) => throw new NotImplementedException();
    public override void ParticlesRequestProcess(Guid particles) => throw new NotImplementedException();
}
