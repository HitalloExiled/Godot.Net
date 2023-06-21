namespace Godot.Net.Servers.Rendering.Storage;

// Filename and type are different
public abstract class RendererParticlesStorage
{
    public abstract int ParticlesGetDrawPasses(Guid particles);
    public abstract Guid ParticlesGetDrawPassMesh(Guid particles, int pass);
    public abstract bool ParticlesIsInactive(Guid particles);
    public abstract void ParticlesRequestProcess(Guid particles);
}
