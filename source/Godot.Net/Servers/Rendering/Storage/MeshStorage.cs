namespace Godot.Net.Servers.Rendering.Storage;

// Filename and type are different
public abstract class RendererMeshStorage
{
    public abstract RS.SurfaceData MeshGetSurface(Guid mesh, int surface);
    public abstract int MeshGetSurfaceCount(Guid mesh);
    public abstract Guid MultimeshGetMesh(Guid multimesh);
}
