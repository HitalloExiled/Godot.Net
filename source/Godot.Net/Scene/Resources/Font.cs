namespace Godot.Net.Scene.Resources;

using System.Collections.Generic;
using Godot.Net.Core.IO;
using Godot.Net.Servers;

#pragma warning disable IDE0044, CS0649 // TODO Remove

public partial class Font : Resource
{
    private const int MAX_FALLBACK_DEPTH = 64;

    public const int DEFAULTFONTSIZE = 16;

    private readonly Dictionary<ShapedTextKey, TextLine>      cache = new();
    private readonly Dictionary<ShapedTextKey, TextParagraph> cacheWrap = new();

    private bool        dirtyIds;
    private IList<Font> fallbacks = Array.Empty<Font>();
    private List<Guid>  ids       = new();

    public virtual Guid Id => default;

    public IList<Guid> Ids
    {
        get
        {
            if (this.dirtyIds)
            {
                this.UpdateIds();
            }
            return this.ids;
        }
    }

    public IList<Font> Fallbacks
    {
        get => this.fallbacks;
        set
        {
            foreach (var f in value)
            {
                if (ERR_FAIL_COND_MSG(this.IsCyclic(f, 0), "Cyclic font fallback."))
                {
                    return;
                }
            }

            foreach (var f in this.fallbacks)
            {
                f.Changed -= this.InvalidateIds;
            }

            this.fallbacks = value;

            foreach (var f in this.fallbacks)
            {
                f.Changed += this.InvalidateIds;
            }

            this.InvalidateIds();
        }
    }

    public Dictionary<uint, TextServer.Feature> OpentypeFeatures { get; set; } = new();


    private void UpdateIds()
    {
        this.ids.Clear();
        this.UpdateIdsFb(this, 0);
        this.dirtyIds = false;
    }

    private void UpdateIdsFb(Font f, int depth)
    {
        if (ERR_FAIL_COND(depth > MAX_FALLBACK_DEPTH))
        {
            return;
        }

        var id = f.Id;

        if (id != default)
        {
            this.ids.Add(id);
        }

        foreach (var fallback in f.Fallbacks)
        {
            this.UpdateIdsFb(fallback, depth + 1);
        }
    }

    protected virtual bool IsCyclic(Font f, int depth)
    {
        if (ERR_FAIL_COND_V(depth > MAX_FALLBACK_DEPTH))
        {
            return true;
        }

        if (f == this)
        {
            return true;
        }

        foreach (var fallback in f.Fallbacks)
        {
            if (this.IsCyclic(fallback, depth + 1))
            {
                return true;
            }
        }

        return false;
    }

    public virtual RealT GetHeight(int fontSize)
    {
        if (this.dirtyIds)
        {
            this.UpdateIds();
        }
        var ret = (RealT)0;

        for (var i = 0; i < this.ids.Count; i++)
        {
            ret = (RealT)Math.Max(ret, TextServerManager.Singleton.PrimaryInterface.FontGetAscent(this.ids[i], fontSize) + TextServerManager.Singleton.PrimaryInterface.FontGetDescent(this.ids[i], fontSize));
        }

        return ret + this.GetSpacing(TextServer.SpacingType.SPACING_BOTTOM) + this.GetSpacing(TextServer.SpacingType.SPACING_TOP);
    }
    public virtual int GetSpacing(TextServer.SpacingType spacing) => 0;
    public virtual void InvalidateIds()
    {
        this.ids.Clear();
        this.dirtyIds = true;

        this.cache.Clear();
        this.cacheWrap.Clear();

        this.EmitChanged();
    }
}
