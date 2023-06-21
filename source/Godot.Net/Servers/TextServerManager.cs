namespace Godot.Net.Servers;

using Godot.Net.Core.OS;

public class TextServerManager
{
    private static TextServerManager? singleton;
    public static TextServerManager Singleton => singleton ?? throw new NullReferenceException();
    private readonly List<TextServer> interfaces = new();

    public event Action<string>? InterfaceAdded;

    #pragma warning disable IDE0032, IDE0027
    private double fontGlobalOversampling;
    private TextServer? primaryInterface;

    public double FontGlobalOversampling
    {
        get
        {
            // TODO
            // double ret = 0;
            // GDVIRTUAL_CALL(_font_get_global_oversampling, ret);
            // return ret;

            return this.fontGlobalOversampling;
        }
        set
        {
            this.fontGlobalOversampling = value;
            // TODO// GDVIRTUAL_CALL(_font_set_global_oversampling, value);
        }
    }
    #pragma warning restore IDE0032, IDE0027

    public IList<TextServer> Interfaces => this.interfaces;

    public TextServer PrimaryInterface
    {
        get => this.primaryInterface ?? throw new NullReferenceException();
        set
        {
            if (value == null)
            {
                PrintVerbose("TextServer: Clearing primary interface");
                this.primaryInterface = null;
            }
            else
            {
                this.primaryInterface = value;
                PrintVerbose($"TextServer: Primary interface set to: \"{this.primaryInterface.Name}\".");

                OS.Singleton.MainLoop?.Notification(Scene.Main.NotificationKind.MAIN_LOOP_NOTIFICATION_TEXT_SERVER_CHANGED);
            }
        }
    }

    public TextServerManager() => singleton = this;

    #pragma warning disable IDE0022

    public void AddInterface(TextServer @interface)
    {
        for (var i = 0; i < this.interfaces.Count; i++)
        {
            if (this.interfaces[i] == @interface)
            {
                ERR_PRINT("TextServer: Interface was already added.");
                return;
            };
        };

        this.interfaces.Add(@interface);
        PrintVerbose($"TextServer: Added interface \"{@interface.Name}\"");

        InterfaceAdded?.Invoke(@interface.Name);
    }

    #pragma warning disable CA1822,IDE0060
    public Guid CreateFont()
    {
        var ret = Guid.NewGuid();
        // GDVIRTUAL_CALL(_create_font, ret);
        return ret;
    }

    public bool IsLocaleRightToLeft(string locale)
    {
        var ret = false;
        // TODO GDVIRTUAL_CALL(_is_locale_right_to_left, locale, ret);
        return ret;
    }

    public void FontSetAllowSystemFallback(Guid fontId, bool allowSystemFallback)
    {
        // TODO GDVIRTUAL_CALL(_font_set_allow_system_fallback, fontRid, allowSystemFallback);
    }

    public void FontSetAntialiasing(Guid fontId, TextServer.FontAntialiasing antialiasing)
    {
        // GDVIRTUAL_CALL(_font_set_antialiasing, fontRid, antialiasing);
    }

    public void FontSetData(Guid fontId, byte[] data)
    {
        // TODO GDVIRTUAL_CALL(_font_set_data_ptr, fontRid, dataPtr, dataSize);
    }

    public void FontSetFixedSize(Guid fontId, long fixedSize)
    {
        // TODO GDVIRTUAL_CALL(_font_set_fixed_size, fontRid, fixedSize);
    }

    public void FontSetForceAutohinter(Guid fontId, bool forceAutohinter)
    {
        // TODO GDVIRTUAL_CALL(_font_set_force_autohinter, fontRid, forceAutohinter);
    }

    public void FontSetGenerateMipmaps(Guid fontId, bool generateMipmaps)
    {
        // TODO GDVIRTUAL_CALL(_font_set_generate_mipmaps, fontRid, generateMipmaps);
    }

    public void FontSetHinting(Guid fontId, TextServer.Hinting hinting)
    {
        // TODO GDVIRTUAL_CALL(_font_set_hinting, fontRid, hinting);
    }

    public void FontSetMsdfPixelRange(Guid fontId, long msdfPixelRange)
    {
        // TODO GDVIRTUAL_CALL(_font_set_msdf_pixel_range, fontRid, msdfPixelRange);
    }

    public void FontSetMsdfSize(Guid fontId, long msdfSize)
    {
        // TODO GDVIRTUAL_CALL(_font_set_msdf_size, fontRid, msdfSize);
    }

    public void FontSetMultichannelSignedDistanceField(Guid fontId, bool msdf)
    {
        // TODO GDVIRTUAL_CALL(_font_set_multichannel_signed_distance_field, fontRid, msdf);
    }

    public void FontSetOversampling(Guid fontId, double oversampling)
    {
        // TODO GDVIRTUAL_CALL(_font_set_oversampling, fontRid, oversampling);
    }

    public void FontSetSubpixelPositioning(Guid fontId, TextServer.SubpixelPositioning subpixel)
    {
        // TODO GDVIRTUAL_CALL(_font_set_subpixel_positioning, fontRid, subpixel);
    }

#pragma warning restore IDE0022, CA1822, IDE0060

}
