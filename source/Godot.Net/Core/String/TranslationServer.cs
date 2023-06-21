namespace Godot.Net.Core.String;

public class TranslationServer
{
    private static TranslationServer? singleton;

    public static TranslationServer Singleton => singleton ?? throw new NullReferenceException();

    public TranslationServer()
    {
        singleton = this;
	    this.InitLocaleInfo();
    }

    #pragma warning disable CA1822, IDE0022, IDE0060 // TODO Remove
    private void InitLocaleInfo()
    {
        // TODO
    }

    public int CompareLocales(string localeA, string localeB)
    {
        return 0;
        // TODO
    }

    public string GetToolLocale()
    {
        // TODO

        return "en";
    }
    #pragma warning restore CA1822, IDE0022, IDE0060 // TODO Remove
}
