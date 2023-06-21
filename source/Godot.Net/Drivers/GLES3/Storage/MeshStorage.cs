namespace Godot.Net.Drivers.GLES3.Storage;

using Godot.Net.Core.Math;
using Godot.Net.Servers.Rendering.Storage;
using Godot.Net.Drivers.GLES3.Shaders;
using Godot.Net.Drivers.GLES3.Api.Enums;

#pragma warning disable IDE0044 // TODO Remove

public class MeshStorage : RendererMeshStorage
{
    public record SkeletonShader
    {
        public SkeletonShaderGLES3 Shader = new();
        public Guid                ShaderVersion;
    }

    private readonly SkeletonShader skeletonShader = new();

    private static MeshStorage? singleton;

    public static MeshStorage Singleton => singleton ?? throw new NullReferenceException();

    public MeshStorage()
    {
        singleton = this;

        this.skeletonShader.Shader.Initialize();
        this.skeletonShader.ShaderVersion = this.skeletonShader.Shader.VersionCreate();
    }

    public void MeshInstanceCheckForUpdate(Guid meshInstance) => throw new NotImplementedException();
    public void MeshInstanceSetCanvasItemTransform(Guid meshInstance, Transform2D<float> transform2D) => throw new NotImplementedException();
    public void MeshInstanceSurfaceGetVertexArraysAndFormat(Guid meshInstance, int surfaceIndex, int inputMask, out uint vertexArrayGl) => throw new NotImplementedException();
    public uint MeshSurfaceGetIndexBuffer(RS.SurfaceData surface, uint lod) => throw new NotImplementedException();
    public DrawElementsType MeshSurfaceGetIndexType(RS.SurfaceData surface) => throw new NotImplementedException();
    public RS.PrimitiveType MeshSurfaceGetPrimitive(RS.SurfaceData surface) => throw new NotImplementedException();
    public void MeshSurfaceGetVertexArraysAndFormat(RS.SurfaceData surface, int inputMask, out uint vertexArrayGl) => throw new NotImplementedException();
    public int MeshSurfaceGetVerticesDrawnCount(RS.SurfaceData surface) => throw new NotImplementedException();
    public int MultimeshGetColorOffset(Guid multimesh) => throw new NotImplementedException();
    public uint MultimeshGetGlBuffer(Guid multimesh) => throw new NotImplementedException();
    public int MultimeshGetInstancesToDraw(Guid multimesh) => throw new NotImplementedException();
    public int MultimeshGetStride(Guid multimesh) => throw new NotImplementedException();
    public RS.MultimeshTransformFormat MultimeshGetTransformFormat(Guid multimesh) => throw new NotImplementedException();
    public bool MultimeshUsesColors(Guid multimesh) => throw new NotImplementedException();
    public bool MultimeshUsesCustomData(Guid multimesh) => throw new NotImplementedException();
    public void UpdateDirtyMultimeshes() => throw new NotImplementedException();
    public void UpdateMeshInstances() => throw new NotImplementedException();


    public override RS.SurfaceData MeshGetSurface(Guid mesh, int surface) => throw new NotImplementedException();
    public override int MeshGetSurfaceCount(Guid mesh) => throw new NotImplementedException();
    public override Guid MultimeshGetMesh(Guid multimesh) => throw new NotImplementedException();
}
