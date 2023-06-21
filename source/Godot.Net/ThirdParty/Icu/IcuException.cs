namespace Godot.Net.ThirdParty.Icu;

public class IcuException : Exception
{
    public IcuException(string message) : base(message)
    { }

    public static void ThrowIfError(UErrorCode errorCode)
    {
        if (errorCode != UErrorCode.U_ZERO_ERROR)
        {
            throw new IcuException($"Error code: {errorCode}");
        }
    }
}
