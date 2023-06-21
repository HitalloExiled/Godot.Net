namespace Godot.Net.ThirdParty.Icu;

public class Character
{
    public static int GetIntPropertyValue(char character, UProperty which) =>
        IcuNative.Singleton.UGetIntPropertyValue(character, which);

    public static bool IsPunct(char character) =>
        IcuNative.Singleton.UIsPunct(character);

}
