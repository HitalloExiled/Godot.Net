
namespace Godot.Net.Servers.Rendering.Storage;

using System.Runtime.CompilerServices;
using Godot.Net.Core.Math;

#pragma warning disable IDE0022 // TODO Remove

public abstract class RenderSceneBuffers
{
    private float fsrSharpness;
    private float textureMipmapBias;
    private bool useDebanding;

    public float FsrSharpness      { get => this.fsrSharpness; set => this.SetFsrSharpness(value); }
    public float TextureMipmapBias { get => this.textureMipmapBias; set => this.SetTextureMipmapBias(value); }
    public bool  UseDebanding      { get => this.useDebanding; set => this.SetUseDebanding(value); }

    public abstract void Configure(
        Guid                     renderTargetId,
        Vector2<int>             internalSize,
        Vector2<int>             targetSize,
        RealT                    fsrSharpness,
        double                   textureMipmapBias,
        RS.ViewportMSAA          msaa3D,
        RS.ViewportScreenSpaceAA screenSpaceAA,
        bool                     useTaa,
        bool                     useDebanding,
        uint                     viewCount
    );

    // void RenderSceneBuffers::set_fsr_sharpness(float p_fsr_sharpness)
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void SetFsrSharpness(float value)
    {
        this.fsrSharpness = value;
        // GDVIRTUAL_CALL(_set_fsr_sharpness, p_fsr_sharpness);
    }

    // void RenderSceneBuffers::set_texture_mipmap_bias(float p_texture_mipmap_bias)
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void SetTextureMipmapBias(float value)
    {
        this.textureMipmapBias = value;
        // GDVIRTUAL_CALL(_set_texture_mipmap_bias, p_texture_mipmap_bias);
    }

    // void RenderSceneBuffers::set_use_debanding(bool p_use_debanding)
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void SetUseDebanding(bool value)
    {
        this.useDebanding = value;
        // GDVIRTUAL_CALL(_set_use_debanding, p_use_debanding);
    }
}
