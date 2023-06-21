namespace Godot.Net.Scene.Resources;

using System.Runtime.CompilerServices;
using Godot.Net.Servers;

#pragma warning disable CS0649, IDE0044 // TODO Remove

public class FontFile : Font
{
    private readonly List<Guid> cache = new();

    private bool                           allowSystemFallback;
    private TextServer.FontAntialiasing    antialiasing;
    private byte[]                         data = Array.Empty<byte>();
    private long                           fixedSize;
    private bool                           forceAutohinter;
    private TextServer.Hinting             hinting;
    private bool                           mipmaps;
    private bool                           msdf;
    private long                           msdfPixelRange;
    private long                           msdfSize;
    private double                         oversampling;
    private TextServer.SubpixelPositioning subpixelPositioning;

    #region public properties
    public TextServer.FontAntialiasing Antialiasing
    {
        get => this.antialiasing;
        set
        {
            if (this.antialiasing != value)
            {
                this.antialiasing = value;
                for (var i = 0; i < this.cache.Count; i++)
                {
                    this.EnsureId(i);
                    TextServerManager.Singleton.FontSetAntialiasing(this.cache[i], value);
                }
                this.EmitChanged();
            }
        }
    }

    public byte[] Data
    {
        get => this.data.ToArray();
        set
        {
            this.data = value;

            for (var i = 0; i < this.cache.Count; i++)
            {
                if (this.cache[i] != default)
                {
                    TextServerManager.Singleton.FontSetData(this.cache[i], value);
                }
            }
        }
    }

    public bool ForceAutohinter
    {
        get => this.forceAutohinter;
        set
        {
            if (this.forceAutohinter != value)
            {
                this.forceAutohinter = value;
                for (var i = 0; i < this.cache.Count; i++)
                {
                    this.EnsureId(i);
                    TextServerManager.Singleton.FontSetForceAutohinter(this.cache[i], this.forceAutohinter);
                }
                this.EmitChanged();
            }
        }
}

    public bool GenerateMipmaps
    {
        get => this.mipmaps;
        set
        {
            if (this.mipmaps != value)
            {
                this.mipmaps = value;
                for (var i = 0; i < this.cache.Count; i++)
                {
                    this.EnsureId(i);
                    TextServerManager.Singleton.FontSetGenerateMipmaps(this.cache[i], this.mipmaps);
                }
                this.EmitChanged();
            }
        }
    }

    public TextServer.Hinting Hinting
    {
        get => this.hinting;
        set
        {
            if (this.hinting != value)
            {
                this.hinting = value;
                for (var i = 0; i < this.cache.Count; i++)
                {
                    this.EnsureId(i);
                    TextServerManager.Singleton.FontSetHinting(this.cache[i], value);
                }
                this.EmitChanged();
            }
        }
    }

    public bool MultichannelSignedDistanceField
    {
        get => this.msdf;
        set
        {
            if (this.msdf != value)
            {
                this.msdf = value;
                for (var i = 0; i < this.cache.Count; i++)
                {
                    this.EnsureId(i);
                    TextServerManager.Singleton.FontSetMultichannelSignedDistanceField(this.cache[i], value);
                }
                this.EmitChanged();
            }
        }
    }

    public TextServer.SubpixelPositioning SubpixelPositioning
    {
        get => this.subpixelPositioning;
        set
        {
            if (this.subpixelPositioning != value)
            {
                this.subpixelPositioning = value;
                for (var i = 0; i < this.cache.Count; i++)
                {
                    this.EnsureId(i);
                    TextServerManager.Singleton.FontSetSubpixelPositioning(this.cache[i], this.subpixelPositioning);
                }
                this.EmitChanged();
            }
        }
    }
    #endregion public properties

    #region public overrided properties
    public override Guid Id
    {
        get
        {
            this.EnsureId(0);
	        return this.cache[0];
        }
    }
    #endregion public overrided properties

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void EnsureId(int cacheIndex)
    {
        if (cacheIndex >= this.cache.Count)
        {
            this.cache.Capacity = cacheIndex + 1;
        }

        if (this.cache[cacheIndex] != default)
        {
            this.cache[cacheIndex] = TextServerManager.Singleton.CreateFont();
            TextServerManager.Singleton.FontSetData(this.cache[cacheIndex], this.data);
            TextServerManager.Singleton.FontSetAntialiasing(this.cache[cacheIndex], this.antialiasing);
            TextServerManager.Singleton.FontSetGenerateMipmaps(this.cache[cacheIndex], this.mipmaps);
            TextServerManager.Singleton.FontSetMultichannelSignedDistanceField(this.cache[cacheIndex], this.msdf);
            TextServerManager.Singleton.FontSetMsdfPixelRange(this.cache[cacheIndex], this.msdfPixelRange);
            TextServerManager.Singleton.FontSetMsdfSize(this.cache[cacheIndex], this.msdfSize);
            TextServerManager.Singleton.FontSetFixedSize(this.cache[cacheIndex], this.fixedSize);
            TextServerManager.Singleton.FontSetForceAutohinter(this.cache[cacheIndex], this.forceAutohinter);
            TextServerManager.Singleton.FontSetAllowSystemFallback(this.cache[cacheIndex], this.allowSystemFallback);
            TextServerManager.Singleton.FontSetHinting(this.cache[cacheIndex], this.hinting);
            TextServerManager.Singleton.FontSetSubpixelPositioning(this.cache[cacheIndex], this.subpixelPositioning);
            TextServerManager.Singleton.FontSetOversampling(this.cache[cacheIndex], this.oversampling);
        }
    }


}
